using FastEndpoints;
using MediatR;
using Application.Queries;
using Shared.Domain;
using Application.Models.Order;

namespace Api.Endpoints.Order;

public class GetOrderByIdEndpoint(IMediator mediator) : Ep
    .Req<GetOrderByIdEndpoint.RequestDto>
    .Res<OrderDetailsDto>
{
    public override void Configure()
    {
        Get("{id}");
        Group<OrderGroup>();
        Description(bld => bld.WithName("Orders_GetOrderById")
            .Produces<OrderDetailsDto>(statusCode: StatusCodes.Status200OK));
        Summary(s =>
        {
            s.Summary = "Get an order by ID.";
            s.Description = "An async endpoint to get an order by its ID.";
            s.ExampleRequest = new RequestDto
            {
                Id = 1
            };
            s.ResponseExamples[200] = new OrderDetailsDto
            {
                Id = 1,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 100.00m,
                Status = OrderStatus.Created,
                ShippingAddress = "123 Main St, Anytown, USA",
                OrderItems = new List<OrderItemDto>
                {
                    new OrderItemDto
                    {
                        Id = 1,
                        ProductName = "Laptop Pro",
                        Quantity = 1,
                        Price = 1200.00m,
                        Total = 1200.00m
                    },
                    new OrderItemDto
                    {
                        Id = 2,
                        ProductName = "Wireless Mouse",
                        Quantity = 2,
                        Price = 25.50m,
                        Total = 51.00m
                    },
                    new OrderItemDto
                    {
                        Id = 3,
                        ProductName = "Mechanical Keyboard",
                        Quantity = 1,
                        Price = 75.00m,
                        Total = 75.00m
                    }
                }
            };
        });
    }

    public override async Task<OrderDetailsDto> ExecuteAsync(RequestDto req, CancellationToken cancellationToken)
    {
        var query = req.ToQuery();
        return await mediator.Send(query, cancellationToken);
    }

    public sealed record RequestDto
    {
        [RouteParam]
        public required int Id { get; init; }

        public GetOrderByIdQuery ToQuery()
        {
            return new GetOrderByIdQuery
            {
                Id = Id
            };
        }
    }
}

