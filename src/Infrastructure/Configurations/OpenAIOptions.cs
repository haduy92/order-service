namespace Infrastructure.Configurations;

public record OpenAIOptions
{
    public const string SectionName = "OpenAI";
    public required string ApiKey { get; init; }
    public required string Model { get; init; }
}

