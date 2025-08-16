using CleanArchJwt.Domain.Entities;

namespace CleanArchJwt.Application.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(string email, string password, string role, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<bool> ForgotPasswordAsync(string email, CancellationToken ct = default);
    Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
}
