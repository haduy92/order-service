using Domain.Events;

namespace Application.Models.Message;

public record OrderCreatedMessage
{
    public int OrderId { get; init; }

    public static OrderCreatedMessage FromDomainEvent(OrderCreatedEvent domainEvent)
    {
        return new OrderCreatedMessage
        {
            OrderId = domainEvent.OrderId,
        };
    }
}
