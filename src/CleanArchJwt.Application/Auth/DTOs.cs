namespace CleanArchJwt.Application.Auth;

public record RegisterRequest(string Email, string Password, string? Role);
public record LoginRequest(string Email, string Password);
public record ForgotPasswordRequest(string Email);
public record RefreshTokenRequest(string RefreshToken);
public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);
public record CurrentUserDto(Guid Id, string Email, string Role);
