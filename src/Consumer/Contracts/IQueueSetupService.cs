using RabbitMQ.Client;

namespace Consumer.Contracts;

public interface IQueueSetupService
{
    Task<string> SetupQueueAsync(IChannel channel, string exchangeName, string queueName, string routingKey);
}
