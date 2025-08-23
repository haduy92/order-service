using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IOrderRepository : IRepository<Order, int>
{
}

