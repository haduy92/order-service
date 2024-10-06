namespace FlashCard.Application.Models;

public class UpdateCardRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
}
