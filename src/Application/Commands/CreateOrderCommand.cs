using Application.Contracts.Persistence;
using Application.Mappers;
using Application.Models.Order;
using Domain.Events;
using MediatR;

namespace Application.Commands;

public record CreateOrderCommand : IRequest<int>
{
    public required OrderDetailsDto OrderDetails { get; init; }

    public class Handler(IOrderRepository orderRepository, IMediator mediator)
        : IRequestHandler<CreateOrderCommand, int>
    {
        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = OrderMapper.ToOrderEntity(request.OrderDetails);

            await orderRepository.InsertAsync(order, cancellationToken);

            // Publish domain event with only order ID
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id
            };

            await mediator.Publish(orderCreatedEvent, cancellationToken);

            return order.Id;
        }
    }
}
