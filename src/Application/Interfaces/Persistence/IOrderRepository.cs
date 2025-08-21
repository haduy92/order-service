using Domain.Entities;

namespace Application.Interfaces.Persistence;

public interface IOrderRepository : IRepository<Order, int>
{
}

