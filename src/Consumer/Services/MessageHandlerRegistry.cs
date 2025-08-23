using Consumer.Abstractions;
using Consumer.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;

namespace Consumer.Services;

/// <summary>
/// Registry implementation for managing message handler configurations
/// Follows Single Responsibility Principle - only manages handler registrations
/// Uses auto-discovery to follow Open/Closed Principle
/// </summary>
public class MessageHandlerRegistry : IMessageHandlerRegistry
{
    private readonly ConcurrentDictionary<string, IMessageConsumerConfiguration> _configurations = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageHandlerRegistry> _logger;

    public MessageHandlerRegistry(IServiceProvider serviceProvider, ILogger<MessageHandlerRegistry> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        
        InitializeHandlers();
    }

    public void RegisterHandler<THandler>() where THandler : class, IMessageHandler
    {
        var handlerType = typeof(THandler);
        
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var tempInstance = ActivatorUtilities.CreateInstance<THandler>(scope.ServiceProvider);
            
            if (tempInstance != null)
            {
                var config = new ConsumerConfiguration
                {
                    Topic = tempInstance.Topic,
                    QueueName = tempInstance.QueueName,
                    HandlerType = handlerType
                };

                var added = _configurations.TryAdd(tempInstance.Topic, config);
                
                if (added)
                {
                    _logger.LogDebug("Registered message handler {HandlerType} for topic {Topic}", 
                        handlerType.Name, tempInstance.Topic);
                }
                else
                {
                    _logger.LogWarning("Handler for topic {Topic} already exists, skipping registration of {HandlerType}", 
                        tempInstance.Topic, handlerType.Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register handler {HandlerType}", handlerType.Name);
        }
    }

    public IEnumerable<IMessageConsumerConfiguration> GetConfigurations()
    {
        return _configurations.Values;
    }

    private void InitializeHandlers()
    {
        try
        {
            // Auto-discover and register handlers from the current assembly
            var assembly = Assembly.GetExecutingAssembly();
            var handlerTypes = assembly.GetTypes()
                .Where(t => typeof(IMessageHandler).IsAssignableFrom(t) && 
                           !t.IsAbstract && 
                           !t.IsInterface &&
                           t != typeof(IMessageHandler))
                .ToList();

            _logger.LogInformation("Found {HandlerCount} message handlers to register", handlerTypes.Count);

            foreach (var handlerType in handlerTypes)
            {
                RegisterHandlerType(handlerType);
            }

            _logger.LogInformation("Successfully registered {ConfigurationCount} message handler configurations", 
                _configurations.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize message handlers");
        }
    }

    private void RegisterHandlerType(Type handlerType)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = ActivatorUtilities.CreateInstance(scope.ServiceProvider, handlerType) as IMessageHandler;
            
            if (handler != null)
            {
                var config = new ConsumerConfiguration
                {
                    Topic = handler.Topic,
                    QueueName = handler.QueueName,
                    HandlerType = handlerType
                };

                var added = _configurations.TryAdd(handler.Topic, config);
                
                if (added)
                {
                    _logger.LogDebug("Auto-registered message handler {HandlerType} for topic {Topic}", 
                        handlerType.Name, handler.Topic);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to auto-register handler {HandlerType}", handlerType.Name);
        }
    }
}
