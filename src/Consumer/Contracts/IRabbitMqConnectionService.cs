using RabbitMQ.Client;

namespace Consumer.Contracts;

public interface IRabbitMqConnectionService
{
    Task<IConnection> CreateConnectionAsync();
    Task<IChannel> CreateChannelAsync(IConnection connection);
    Task SetupExchangeAsync(IChannel channel, string exchangeName, string exchangeType);
}
