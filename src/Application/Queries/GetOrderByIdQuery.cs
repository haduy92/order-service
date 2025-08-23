using Application.Contracts.Persistence;
using Application.Mappers;
using Application.Models.Order;
using MediatR;
using Shared.Exceptions;

namespace Application.Queries;

public record GetOrderByIdQuery : IRequest<OrderDetailsDto>
{
    public required int Id { get; init; }

    public class Handler : IRequestHandler<GetOrderByIdQuery, OrderDetailsDto>
    {
        private readonly IOrderRepository _orderRepository;

        public Handler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDetailsDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetAsync(request.Id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Order with ID {request.Id} not found.");
            }

            return OrderMapper.ToOrderDetailDto(order);
        }
    }
}
