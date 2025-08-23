using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace Consumer.Abstractions;

/// <summary>
/// Defines a contract for handling messages from message queues
/// </summary>
public interface IMessageHandler
{
    string Topic { get; }
    string QueueName { get; }
    Task HandleAsync(BasicDeliverEventArgs eventArgs, IChannel channel);
}
