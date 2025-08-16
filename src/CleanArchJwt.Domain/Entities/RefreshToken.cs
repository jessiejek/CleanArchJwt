namespace CleanArchJwt.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool Revoked { get; set; } = false;
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
}
