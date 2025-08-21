using Domain.Entities;

namespace Application.Interfaces.Persistence;

public interface IOrderItemRepository : IRepository<OrderItem, int>
{
}

