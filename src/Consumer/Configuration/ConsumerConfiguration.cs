using Consumer.Abstractions;

namespace Consumer.Configuration;

/// <summary>
/// Configuration record for message consumer setup
/// </summary>
public record ConsumerConfiguration : IMessageConsumerConfiguration
{
    public required string Topic { get; init; }
    public required string QueueName { get; init; }
    public required Type HandlerType { get; init; }
}

