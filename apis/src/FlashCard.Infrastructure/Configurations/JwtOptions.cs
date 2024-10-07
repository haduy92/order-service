namespace FlashCard.Infrastructure.Configurations;

public record JwtOptions
{
    public const string SectionName = "Jwt";
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string Key { get; init; }
    public int ExpirationInMinutes { get; init; } = 5; // default

    public DateTime ExpirationTime => DateTime.UtcNow.AddMinutes(ExpirationInMinutes);
}
