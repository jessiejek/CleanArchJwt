namespace CleanArchJwt.Infrastructure.Auth;

public class JwtSettings
{
    public string Issuer { get; set; } = "CleanArchJwt";
    public string Audience { get; set; } = "CleanArchJwtAudience";
    public string Secret { get; set; } = "PLEASE_CHANGE_ME_TO_A_LONG_RANDOM_SECRET";
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;
}
