using Application.Contracts.Persistence;
using Domain.Events;
using MediatR;
using Shared.Domain;
using Shared.Exceptions;

namespace Application.Commands;

public record UpdateOrderStatusCommand : IRequest
{
    public required int OrderId { get; init; }

    public required OrderStatus NewStatus { get; init; }

    public class Handler(IOrderRepository orderRepository, IMediator mediator)
        : IRequestHandler<UpdateOrderStatusCommand>
    {
        public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetAsync(request.OrderId, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Order with ID {request.OrderId} not found.");
            }

            order.Status = request.NewStatus;

            await orderRepository.UpdateAsync(order, cancellationToken);

            // Publish domain event
            var orderStatusChangedEvent = new OrderStatusChangedEvent
            {
                OrderId = order.Id,
                NewStatus = request.NewStatus
            };

            await mediator.Publish(orderStatusChangedEvent, cancellationToken);
        }
    }
}
