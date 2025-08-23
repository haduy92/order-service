using Consumer.Abstractions;

namespace Consumer.Abstractions;

/// <summary>
/// Registry for managing message handler configurations
/// </summary>
public interface IMessageHandlerRegistry
{
    void RegisterHandler<THandler>() where THandler : class, IMessageHandler;
    IEnumerable<IMessageConsumerConfiguration> GetConfigurations();
}
