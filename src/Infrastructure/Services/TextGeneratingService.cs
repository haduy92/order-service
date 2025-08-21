using Application.Interfaces.Application;
using Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace Infrastructure.Services;

public class TextGeneratingService : ITextGeneratingService
{
    private readonly ChatClient _client;
    private const string SystemChatMessageContent = "You are a useful assistant that replies using an educated style.";
    private const string PromptChatTemplate = "Provide from 1 to 3 short sentences but not more than 1000 characters, no special characters";

    public TextGeneratingService(IOptions<OpenAIOptions> options)
    {
        _client = new(model: options.Value.Model, apiKey: options.Value.ApiKey);
    }

    public async Task<string> GenerateTextAsync(string prompt)
    {
        // define system message
        var systemMessage = new SystemChatMessage(SystemChatMessageContent);
        var userQ = new UserChatMessage($"{PromptChatTemplate} about '{prompt}'");
        var messages = new List<ChatMessage>
        {
            systemMessage,
            userQ
        };

        // run the chat
        ChatCompletion completion = await _client.CompleteChatAsync(messages);

        if (completion is null || completion is null)
        {
            return string.Empty;
        }

        return string.Join(". ", completion.Content.Select(x => x.Text));
    }
}

