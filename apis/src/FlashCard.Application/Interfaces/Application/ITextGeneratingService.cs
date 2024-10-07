namespace FlashCard.Application.Interfaces.Application;

public interface ITextGeneratingService
{
    Task<string> GenerateTextAsync(string prompt);
}
