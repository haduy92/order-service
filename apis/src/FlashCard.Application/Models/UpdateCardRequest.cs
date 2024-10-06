namespace FlashCard.Application.Models;

public class UpdateCardRequest
{
    public required string Text { get; init; }
    public required string Description { get; init; }
}
