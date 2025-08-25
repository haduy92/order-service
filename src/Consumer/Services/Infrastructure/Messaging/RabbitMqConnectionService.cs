using Consumer.Configuration;
using Consumer.Services.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Consumer.Services.Infrastructure.Messaging;

/// <summary>
/// Service responsible for managing RabbitMQ connections
/// Follows Single Responsibility Principle - only handles connection management
/// Part of Infrastructure layer - handles external messaging infrastructure
/// </summary>
public class RabbitMqConnectionService : IRabbitMqConnectionService
{
    private readonly ILogger<RabbitMqConnectionService> _logger;
    private readonly RabbitMqOptions _options;

    public RabbitMqConnectionService(
        ILogger<RabbitMqConnectionService> logger,
        IOptions<RabbitMqOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<IConnection> CreateConnectionAsync()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        var connection = await factory.CreateConnectionAsync();
        _logger.LogInformation("RabbitMQ connection established successfully");

        return connection;
    }

    public async Task<IChannel> CreateChannelAsync(IConnection connection)
    {
        var channel = await connection.CreateChannelAsync();
        _logger.LogDebug("RabbitMQ channel created successfully");

        return channel;
    }

    public async Task SetupExchangeAsync(IChannel channel, string exchangeName, string exchangeType)
    {
        await channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: exchangeType,
            durable: true,
            autoDelete: false);

        _logger.LogDebug("Exchange '{ExchangeName}' of type '{ExchangeType}' declared successfully",
            exchangeName, exchangeType);
    }
}
