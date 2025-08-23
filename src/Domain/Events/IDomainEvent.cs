using MediatR;

namespace Domain.Events;

public interface IDomainEvent : INotification
{
    DateTime OccurredAt { get; }
}
