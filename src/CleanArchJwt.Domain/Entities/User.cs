namespace CleanArchJwt.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Authentication
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "User"; // "Admin" or "User"
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Personal Details
    public string FirstName { get; set; } = default!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = default!;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    // Navigation
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
