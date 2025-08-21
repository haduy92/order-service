using Application.Interfaces.Persistence;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class OrderRepository : RepositoryBase<Order, int>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context)
    {
    }
}

