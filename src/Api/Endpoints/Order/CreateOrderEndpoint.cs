using Application.Commands;
using Application.Models.Order;
using FastEndpoints;
using MediatR;

namespace Api.Endpoints.Order;

public class CreateOrderEndpoint(IMediator mediator) : Ep
    .Req<CreateOrderEndpoint.RequestDto>
    .Res<CreateOrderEndpoint.ResponseDto>
{
    public override void Configure()
    {
        Post("");
        Group<OrderGroup>();
        Description(bld => bld.WithName("Orders_CreateOrder")
            .Produces<ResponseDto>(statusCode: StatusCodes.Status200OK)
            .ProducesProblem(statusCode: StatusCodes.Status400BadRequest));
        Summary(s =>
        {
            s.Summary = "Create a new order.";
            s.Description = "An async endpoint to create a new order with order items.";
            s.ExampleRequest = new RequestDto
            {
                OrderDetails = new()
                {
                    TotalAmount = 1326.00m,
                    Street = "123 Main St",
                    City = "Anytown",
                    Country = "USA",
                    PostCode = "12345",
                    OrderItems = new List<OrderItemDto>
                    {
                        new OrderItemDto
                        {
                            ProductName = "Laptop Pro",
                            Quantity = 1,
                            Price = 1200.00m
                        },
                        new OrderItemDto
                        {
                            ProductName = "Wireless Mouse",
                            Quantity = 2,
                            Price = 25.50m
                        },
                        new OrderItemDto
                        {
                            ProductName = "Mechanical Keyboard",
                            Quantity = 1,
                            Price = 75.00m
                        }
                    }
                }
            };
            s.ResponseExamples[201] = new ResponseDto
            {
                OrderId = 1
            };
        });
    }

    public override async Task<ResponseDto> ExecuteAsync(RequestDto req, CancellationToken cancellationToken)
    {
        var command = req.ToCommand();
        var orderId = await mediator.Send(command, cancellationToken);
        return new ResponseDto { OrderId = orderId };
    }

    public sealed record RequestDto
    {
        public required OrderDetailsDto OrderDetails { get; init; }

        public CreateOrderCommand ToCommand()
        {
            return new()
            {
                OrderDetails = OrderDetails
            };
        }
    }

    public sealed record ResponseDto
    {
        public int OrderId { get; init; }
    }
}
