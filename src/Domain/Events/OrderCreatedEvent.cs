namespace Domain.Events;

public record OrderCreatedEvent : IDomainEvent
{
    public int OrderId { get; init; }
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
