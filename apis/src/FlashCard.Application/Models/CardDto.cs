namespace FlashCard.Application.Models;

public record CardDto
{
    public int Id { get; init; }
    public string Text { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public DateTime CreationTime { get; init; }
    public DateTime? ModificationTime { get; init; }
}
