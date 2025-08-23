namespace Consumer.Abstractions;

/// <summary>
/// Represents a message consumption configuration
/// </summary>
public interface IMessageConsumerConfiguration
{
    string Topic { get; }
    string QueueName { get; }
    Type HandlerType { get; }
}
