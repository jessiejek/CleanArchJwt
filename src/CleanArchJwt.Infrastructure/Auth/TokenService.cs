using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CleanArchJwt.Application.Auth;
using CleanArchJwt.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchJwt.Infrastructure.Auth;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly JwtSecurityTokenHandler _handler = new();

    public TokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public (string token, DateTime expiresAtUtc) CreateAccessToken(User user)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_settings.AccessTokenMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds);

        return (_handler.WriteToken(token), expires);
    }

    public (string token, DateTime expiresAtUtc) CreateRefreshToken()
    {
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                    .Replace("+","").Replace("/","").Replace("=","");
        var expires = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays);
        return (token, expires);
    }
}
