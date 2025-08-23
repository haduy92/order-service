using Shared.Domain;

namespace Domain.Events;

public record OrderStatusChangedEvent : IDomainEvent
{
    public int OrderId { get; init; }
    public OrderStatus NewStatus { get; init; }
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
