using Ardalis.Specification;
using Domain.Entities;

namespace Domain.Specifications.Orders;

public class OrderByIdSpecification : BaseSpecification<Order>
{
    public OrderByIdSpecification(int orderId)
    {
        Query
            .Where(order => order.Id == orderId)
            .Include(order => order.OrderItems);
    }
}
