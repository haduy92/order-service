namespace FlashCard.Application.Models;

public record CreateCardRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
}
