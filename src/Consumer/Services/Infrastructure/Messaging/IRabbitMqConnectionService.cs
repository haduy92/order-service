using RabbitMQ.Client;

namespace Consumer.Services.Infrastructure.Messaging;

/// <summary>
/// Interface for managing RabbitMQ connections
/// Part of Infrastructure layer - handles external messaging infrastructure
/// </summary>
public interface IRabbitMqConnectionService
{
    Task<IConnection> CreateConnectionAsync();
    Task<IChannel> CreateChannelAsync(IConnection connection);
    Task SetupExchangeAsync(IChannel channel, string exchangeName, string exchangeType);
}
