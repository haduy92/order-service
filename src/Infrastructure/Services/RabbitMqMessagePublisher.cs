using Application.Contracts.Messaging;
using Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Infrastructure.Services;

public class RabbitMqMessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqMessagePublisher> _logger;

    public RabbitMqMessagePublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqMessagePublisher> logger)
    {
        _logger = logger;
        _options = options.Value;
        
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost
            };
            
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            
            // Declare the exchange
            _channel.ExchangeDeclareAsync(
                exchange: _options.Exchange,
                type: _options.ExchangeType,
                durable: true,
                autoDelete: false).GetAwaiter().GetResult();
                
            _logger.LogInformation("Successfully connected to RabbitMQ at {HostName}:{Port}", _options.HostName, _options.Port);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ at {HostName}:{Port}", _options.HostName, _options.Port);
            throw;
        }
    }

    public async Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            
            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };
            
            await _channel.BasicPublishAsync(
                exchange: _options.Exchange,
                routingKey: topic,
                basicProperties: properties,
                body: body,
                mandatory: false,
                cancellationToken: cancellationToken);
            
            _logger.LogDebug("Message published to topic {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to topic {Topic}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        try
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ connection disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ connection");
        }
    }
}
