namespace CleanArchJwt.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "User"; // "Admin" or "User"
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
