using Application.Contracts.Messaging;
using Application.Models.Message;
using Domain.Events;
using MediatR;
using Shared.Constants;

namespace Application.EventHandlers;

public class OrderCreatedEventHandler(IMessagePublisher messagePublisher)
    : INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        var message = OrderCreatedMessage.FromDomainEvent(notification);
        await messagePublisher.PublishAsync(message, MessagingTopics.OrderCreated, cancellationToken);
    }
}
