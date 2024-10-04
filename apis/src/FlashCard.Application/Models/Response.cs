namespace FlashCard.Application.Models;

public record Response
{
    public bool Succeeded { get; set; }
    public IDictionary<string, string>? Errors { get; set; }
}
