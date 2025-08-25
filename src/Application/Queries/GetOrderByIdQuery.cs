using Application.Contracts.Persistence;
using Application.Mappers;
using Application.Models.Order;
using Domain.Specifications.Orders;
using MediatR;
using Shared.Exceptions;

namespace Application.Queries;

public record GetOrderByIdQuery : IRequest<OrderDetailsDto>
{
    public required int Id { get; init; }

    public class Handler(IOrderRepository orderRepository)
        : IRequestHandler<GetOrderByIdQuery, OrderDetailsDto>
    {
        public async Task<OrderDetailsDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var specification = new OrderByIdSpecification(request.Id);
            var order = await orderRepository.GetBySpecAsync(specification, cancellationToken);
            
            if (order is null)
            {
                throw new NotFoundException($"Order with ID {request.Id} not found.");
            }

            return OrderMapper.ToOrderDetailDto(order);
        }
    }
}
