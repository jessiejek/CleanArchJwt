using System.Security.Claims;
using CleanArchJwt.Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchJwt.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IAuthService _auth;

    public UsersController(IAuthService auth)
    {
        _auth = auth;
    }

    /// <summary>Returns the current authenticated user.</summary>
    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserDto>> Me(CancellationToken ct)
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var user = await _auth.GetByEmailAsync(email, ct);
        if (user is null) return NotFound();

        return Ok(new CurrentUserDto(user.Id, user.Email, user.Role));
    }

    /// <summary>Admin-only sample endpoint.</summary>
    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin")]
    public ActionResult<string> AdminOnly() => Ok("Hello Admin!");
}
