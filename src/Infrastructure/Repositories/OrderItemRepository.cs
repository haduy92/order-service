using Application.Interfaces.Persistence;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class OrderItemRepository : RepositoryBase<OrderItem, int>, IOrderItemRepository
{
    public OrderItemRepository(AppDbContext context) : base(context)
    {
    }
}
