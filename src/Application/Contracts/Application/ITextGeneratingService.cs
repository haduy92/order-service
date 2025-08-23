namespace Application.Contracts.Application;

public interface ITextGeneratingService
{
    Task<string> GenerateTextAsync(string prompt);
}

