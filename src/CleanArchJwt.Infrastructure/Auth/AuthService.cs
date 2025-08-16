using BCrypt.Net;
using CleanArchJwt.Application.Auth;
using CleanArchJwt.Domain.Entities;
using CleanArchJwt.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchJwt.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokens;

    public AuthService(AppDbContext db, ITokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    public async Task<AuthResponse> RegisterAsync(string email, string password, string role, CancellationToken ct = default)
    {
        if (await _db.Users.AnyAsync(u => u.Email == email, ct))
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResponse> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await _db.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return await IssueTokensAsync(user, ct);
    }

    public async Task<bool> ForgotPasswordAsync(string email, CancellationToken ct = default)
    {
        // Minimal stub: if user exists, pretend to send a reset email (log-only).
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        return user is not null;
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var existing = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, ct);

        if (existing is null || existing.ExpiresAtUtc < DateTime.UtcNow || existing.Revoked)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        // Revoke old token
        existing.Revoked = true;
        await _db.SaveChangesAsync(ct);

        return await IssueTokensAsync(existing.User, ct);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken ct)
    {
        var (access, accessExp) = _tokens.CreateAccessToken(user);
        var (refresh, refreshExp) = _tokens.CreateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            Token = refresh,
            ExpiresAtUtc = refreshExp,
            UserId = user.Id
        });

        await _db.SaveChangesAsync(ct);

        return new AuthResponse(access, refresh, accessExp);
    }
}
