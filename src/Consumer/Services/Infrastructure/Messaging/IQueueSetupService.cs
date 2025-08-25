using RabbitMQ.Client;

namespace Consumer.Services.Infrastructure.Messaging;

/// <summary>
/// Interface for setting up message queues
/// Part of Infrastructure layer - handles messaging queue setup
/// </summary>
public interface IQueueSetupService
{
    Task<string> SetupQueueAsync(IChannel channel, string exchangeName, string queueName, string routingKey);
}
