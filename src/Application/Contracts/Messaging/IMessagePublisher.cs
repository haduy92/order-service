namespace Application.Contracts.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default) where T : class;
}
