namespace FlashCard.Application.Models;

public record CreateCardRequest
{
    public required string Text { get; init; }
    public required string Description { get; init; }
}
