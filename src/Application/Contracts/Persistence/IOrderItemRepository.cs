using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IOrderItemRepository : IRepository<OrderItem, int>
{
}

