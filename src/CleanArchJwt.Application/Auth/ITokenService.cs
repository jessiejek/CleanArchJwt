using CleanArchJwt.Domain.Entities;

namespace CleanArchJwt.Application.Auth;

public interface ITokenService
{
    (string token, DateTime expiresAtUtc) CreateAccessToken(User user);
    (string token, DateTime expiresAtUtc) CreateRefreshToken();
}
