using FastEndpoints;

namespace Api.Endpoints;

public class OrderGroup : Group
{
    public OrderGroup()
    {
        Configure("/api/v1/orders/", ep =>
        {
            const string tag = "orders";

            ep.Options(x =>
            {
                x.WithTags([tag]);
            });
        });
    }
}

