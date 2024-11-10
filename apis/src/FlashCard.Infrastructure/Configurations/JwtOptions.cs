namespace FlashCard.Infrastructure.Configurations;

public record JwtOptions
{
    public const string SectionName = "Jwt";
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string Secret { get; init; }
    public required string RefreshTokenSecret { get; init; }
    public int AccessTokenExpirationMinutes { get; init; } = 5; // default
    public int RefreshTokenExpirationDays { get; init; } = 24; // default
}
