using Consumer.Abstractions;

namespace Consumer.Factories;

/// <summary>
/// Factory for creating message handler instances
/// Follows the Factory pattern and supports the Open/Closed Principle
/// </summary>
public interface IMessageHandlerFactory
{
    IMessageHandler CreateHandler(Type handlerType);
}

/// <summary>
/// Default implementation of message handler factory using dependency injection
/// </summary>
public class MessageHandlerFactory : IMessageHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MessageHandlerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IMessageHandler CreateHandler(Type handlerType)
    {
        var handler = _serviceProvider.GetService(handlerType) as IMessageHandler;
        
        if (handler == null)
        {
            throw new InvalidOperationException($"Handler of type {handlerType.Name} could not be created or does not implement IMessageHandler");
        }

        return handler;
    }
}
