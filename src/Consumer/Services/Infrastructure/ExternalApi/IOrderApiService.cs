using Application.Models.Order;
using Shared.Domain;

namespace Consumer.Services.Infrastructure.ExternalApi;

/// <summary>
/// Interface for interacting with the Order API
/// Part of Infrastructure layer - handles external API communication
/// </summary>
public interface IOrderApiService
{
    Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
    Task<OrderDetailsDto?> GetOrderAsync(int orderId);
}
