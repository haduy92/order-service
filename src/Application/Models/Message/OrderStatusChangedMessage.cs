using Domain.Events;
using Shared.Domain;

namespace Application.Models.Message;

public record OrderStatusChangedMessage
{
    public int OrderId { get; init; }
    public OrderStatus PreviousStatus { get; init; }
    public OrderStatus NewStatus { get; init; }
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

    public static OrderStatusChangedMessage FromDomainEvent(OrderStatusChangedEvent domainEvent)
    {
        return new OrderStatusChangedMessage
        {
            OrderId = domainEvent.OrderId,
            NewStatus = domainEvent.NewStatus,
            OccurredOn = domainEvent.OccurredAt
        };
    }
}
