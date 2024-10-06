namespace FlashCard.Application.Models;

public record SearchCardRequest : PagedRequest
{
    public string? Search { get; init; }
}
