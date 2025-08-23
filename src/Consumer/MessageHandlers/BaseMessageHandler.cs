using Consumer.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer.MessageHandlers;

/// <summary>
/// Base class for message handlers implementing common functionality
/// Follows DRY principle by providing shared message handling logic
/// </summary>
public abstract class BaseMessageHandler : IMessageHandler
{
    private readonly ILogger _logger;

    protected BaseMessageHandler(ILogger logger)
    {
        _logger = logger;
    }

    public abstract string Topic { get; }
    public abstract string QueueName { get; }

    public async Task HandleAsync(BasicDeliverEventArgs eventArgs, IChannel channel)
    {
        try
        {
            var messageText = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            _logger.LogInformation("Processing message for topic {Topic}: {Message}", Topic, messageText);

            await ProcessMessageAsync(messageText);

            await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
            _logger.LogDebug("Message acknowledged for topic {Topic}", Topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message for topic {Topic}", Topic);
            await channel.BasicNackAsync(eventArgs.DeliveryTag, false, true);
        }
    }

    /// <summary>
    /// Process the actual message content - to be implemented by derived classes
    /// </summary>
    /// <param name="messageText">The message content as string</param>
    /// <returns>Task representing the async operation</returns>
    protected abstract Task ProcessMessageAsync(string messageText);
}
