using CleanArchJwt.Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchJwt.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    /// <summary>Registers a new user. Role: Admin or User (default User).</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var role = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role!;
        var result = await _auth.RegisterAsync(request.Email, request.Password, role, ct);
        return Ok(result);
    }

    /// <summary>Logs in and returns JWT access and refresh tokens.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _auth.LoginAsync(request.Email, request.Password, ct);
        return Ok(result);
    }

    /// <summary>Requests a password reset (stub: returns true if email exists).</summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<bool>> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
    {
        var ok = await _auth.ForgotPasswordAsync(request.Email, ct);
        return Ok(ok);
    }

    /// <summary>Refreshes access token using a valid refresh token.</summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await _auth.RefreshAsync(request.RefreshToken, ct);
        return Ok(result);
    }
}
