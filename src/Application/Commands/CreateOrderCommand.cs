using Application.Interfaces.Persistence;
using Application.Mappers;
using Application.Models.Order;
using MediatR;

namespace Application.Commands;

public record CreateOrderCommand : IRequest<int>
{
    public required OrderDetailsDto OrderDetails { get; init; }

    public class Handler(IOrderRepository orderRepository)
        : IRequestHandler<CreateOrderCommand, int>
    {
        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = OrderMapper.ToOrderEntity(request.OrderDetails);

            await orderRepository.InsertAsync(order, cancellationToken);

            return order.Id;
        }
    }
}
