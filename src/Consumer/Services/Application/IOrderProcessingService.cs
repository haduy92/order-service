using Application.Models.Message;

namespace Consumer.Services.Application;

/// <summary>
/// Interface for order processing business logic
/// Part of Application layer - handles order-specific business rules and workflows
/// </summary>
public interface IOrderProcessingService
{
    Task ProcessOrderCreatedAsync(OrderCreatedMessage orderMessage);
    Task ProcessOrderStatusChangedAsync(OrderStatusChangedMessage statusMessage);
}
