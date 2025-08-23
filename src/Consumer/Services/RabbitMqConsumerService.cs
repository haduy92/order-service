using Consumer.Abstractions;
using Consumer.Configuration;
using Consumer.Contracts;
using Consumer.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.Services;

/// <summary>
/// Background service responsible for consuming messages from RabbitMQ
/// Follows the Single Responsibility Principle - only handles message consumption orchestration
/// </summary>
public class RabbitMqConsumerService : BackgroundService
{
    private const int InfiniteTimeout = -1;
    
    private readonly ILogger<RabbitMqConsumerService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IRabbitMqConnectionService _connectionService;
    private readonly IQueueSetupService _queueSetupService;
    private readonly IMessageHandlerRegistry _handlerRegistry;
    private readonly RabbitMqOptions _options;
    
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumerService(
        ILogger<RabbitMqConsumerService> logger,
        IServiceScopeFactory serviceScopeFactory,
        IRabbitMqConnectionService connectionService,
        IQueueSetupService queueSetupService,
        IMessageHandlerRegistry handlerRegistry,
        IOptions<RabbitMqOptions> options)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _connectionService = connectionService;
        _queueSetupService = queueSetupService;
        _handlerRegistry = handlerRegistry;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await InitializeConnectionAsync();
            await SetupQueuesAndConsumersAsync();
            
            _logger.LogInformation("RabbitMQ Consumer started, listening for messages...");

            await WaitForCancellationAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to start RabbitMQ consumer");
            throw;
        }
    }

    private async Task InitializeConnectionAsync()
    {
        _connection = await _connectionService.CreateConnectionAsync();
        _channel = await _connectionService.CreateChannelAsync(_connection);
        
        await _connectionService.SetupExchangeAsync(_channel, _options.Exchange, _options.ExchangeType);
        
        _logger.LogInformation("RabbitMQ connection initialized successfully");
    }

    private async Task SetupQueuesAndConsumersAsync()
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("Channel is not initialized");
        }

        var configurations = _handlerRegistry.GetConfigurations();

        foreach (var config in configurations)
        {
            await SetupQueueAndConsumerAsync(config);
        }
    }

    private async Task SetupQueueAndConsumerAsync(IMessageConsumerConfiguration config)
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("Channel is not initialized");
        }

        // Setup queue
        var queueName = await _queueSetupService.SetupQueueAsync(
            _channel, _options.Exchange, config.QueueName, config.Topic);

        // Setup consumer
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            await HandleMessageAsync(ea, config);
        };

        await _channel.BasicConsumeAsync(
            queue: queueName, 
            autoAck: false, 
            consumer: consumer);

        _logger.LogDebug("Consumer setup completed for queue '{QueueName}' with topic '{Topic}'", 
            queueName, config.Topic);
    }

    private async Task HandleMessageAsync(BasicDeliverEventArgs eventArgs, IMessageConsumerConfiguration config)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        
        try
        {
            var handlerFactory = scope.ServiceProvider.GetRequiredService<IMessageHandlerFactory>();
            var handler = handlerFactory.CreateHandler(config.HandlerType);
            
            await handler.HandleAsync(eventArgs, _channel!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle message for handler type {HandlerType}", config.HandlerType.Name);
            
            // Reject the message and don't requeue on handler creation/execution failure
            if (_channel != null)
            {
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
            }
        }
    }

    private static async Task WaitForCancellationAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(InfiniteTimeout, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
    }

    public override void Dispose()
    {
        try
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ consumer disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ consumer");
        }
        
        base.Dispose();
    }
}
