using Application.Contracts.Messaging;
using Application.Models.Message;
using Domain.Events;
using MediatR;
using Shared.Constants;

namespace Application.EventHandlers;

public class OrderStatusChangedEventHandler(IMessagePublisher messagePublisher)
    : INotificationHandler<OrderStatusChangedEvent>
{
    public async Task Handle(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        var message = OrderStatusChangedMessage.FromDomainEvent(notification);
        await messagePublisher.PublishAsync(message, MessagingTopics.OrderStatusChanged, cancellationToken);
    }
}
