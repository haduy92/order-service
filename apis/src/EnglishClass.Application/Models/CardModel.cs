namespace EnglishClass.Application.Models;

public record CardModel
{
    public Guid Id { get; init; }
    public required string Text { get; init; }
    public required string Description { get; init; }

    public required string CreatorUserId { get; init; }
    public string? LastModifierUserId { get; init; }
    public DateTime CreationTime { get; init; }
    public DateTime? LastModificationTime { get; init; }
}
