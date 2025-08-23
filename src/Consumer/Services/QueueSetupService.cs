using Consumer.Contracts;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Consumer.Services;

/// <summary>
/// Service responsible for setting up message queues
/// Follows Single Responsibility Principle - only handles queue setup operations
/// </summary>
public class QueueSetupService : IQueueSetupService
{
    private readonly ILogger<QueueSetupService> _logger;

    public QueueSetupService(ILogger<QueueSetupService> logger)
    {
        _logger = logger;
    }

    public async Task<string> SetupQueueAsync(IChannel channel, string exchangeName, string queueName, string routingKey)
    {
        var queueResult = await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: queueResult.QueueName,
            exchange: exchangeName,
            routingKey: routingKey);

        _logger.LogDebug("Queue '{QueueName}' declared and bound to routing key '{RoutingKey}'",
            queueResult.QueueName, routingKey);

        return queueResult.QueueName;
    }
}
