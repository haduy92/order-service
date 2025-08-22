using FastEndpoints;

namespace Api.Endpoints.Order;

public class OrderGroup : Group
{
    public OrderGroup()
    {
        Configure("/v1/orders/", ep =>
        {
            const string tag = "Orders";

            ep.Options(x =>
            {
                x.WithTags([tag]);
                x.WithSummary("Order Management Operations");
                x.WithDescription("Endpoints for managing orders and order items");
            });
        });
    }
}

