using Application.Commands;
using FastEndpoints;
using MediatR;
using Shared.Domain;

namespace Api.Endpoints.Order;

public class UpdateOrderEndpoint(IMediator mediator) : Ep
    .Req<UpdateOrderEndpoint.RequestDto>
    .NoRes
{
    public override void Configure()
    {
        Patch("{id}/status");
        Group<OrderGroup>();
        Description(bld => bld.WithName("Orders_UpdateOrderStatus")
            .Produces(statusCode: StatusCodes.Status200OK)
            .ProducesProblem(statusCode: StatusCodes.Status400BadRequest)
            .ProducesProblem(statusCode: StatusCodes.Status404NotFound));
        Summary(s =>
        {
            s.Summary = "Update order status.";
            s.Description = "An async endpoint to update status of an existing order.";
            s.ExampleRequest = new RequestDto
            {
                Id = 1,
                Status = OrderStatus.Completed
            };
        });
    }

    public override async Task HandleAsync(RequestDto req, CancellationToken cancellationToken)
    {
        var command = req.ToCommand();
        await mediator.Send(command, cancellationToken);
    }

    public sealed record RequestDto
    {
        [RouteParam]
        public required int Id { get; init; }

        public required OrderStatus Status { get; init; }

        public UpdateOrderStatusCommand ToCommand()
        {
            return new()
            {
                OrderId = Id,
                NewStatus = Status
            };
        }
    }
}
