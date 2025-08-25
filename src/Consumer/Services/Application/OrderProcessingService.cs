using Application.Models.Message;
using Application.Models.Order;
using Consumer.Services.Infrastructure.ExternalApi;
using Microsoft.Extensions.Logging;

namespace Consumer.Services.Application;

/// <summary>
/// Service responsible for processing order-related business logic
/// Part of Application layer - orchestrates order business workflows
/// Follows Single Responsibility Principle - only handles order processing logic
/// </summary>
public class OrderProcessingService : IOrderProcessingService
{
    private readonly ILogger<OrderProcessingService> _logger;
    private readonly IOrderApiService _orderApiService;

    public OrderProcessingService(
        ILogger<OrderProcessingService> logger,
        IOrderApiService orderApiService)
    {
        _logger = logger;
        _orderApiService = orderApiService;
    }

    public async Task ProcessOrderCreatedAsync(OrderCreatedMessage orderMessage)
    {
        _logger.LogInformation("Processing order created event for order {OrderId}", orderMessage.OrderId);

        try
        {
            // Query for order details from the API
            var orderDetails = await _orderApiService.GetOrderAsync(orderMessage.OrderId);
            if (orderDetails == null)
            {
                _logger.LogError("Failed to retrieve order details for order {OrderId}", orderMessage.OrderId);
                return;
            }

            _logger.LogInformation("Retrieved order details for order {OrderId}: Total Amount = {TotalAmount}, Status = {Status}, Items Count = {ItemsCount}", 
                orderMessage.OrderId, orderDetails.TotalAmount, orderDetails.Status, orderDetails.OrderItems.Count);

            // Perform any additional processing for order created event
            await ProcessOrderBusinessLogic(orderMessage.OrderId, orderDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order created event for order {OrderId}", orderMessage.OrderId);
        }
    }

    public async Task ProcessOrderStatusChangedAsync(OrderStatusChangedMessage statusMessage)
    {
        _logger.LogInformation("Processing order status changed event for order {OrderId}: {PreviousStatus} ? {NewStatus}", 
            statusMessage.OrderId, statusMessage.PreviousStatus, statusMessage.NewStatus);

        try
        {
            // Handle different status changes
            switch (statusMessage.NewStatus)
            {
                case Shared.Domain.OrderStatus.Processing:
                    await HandleOrderProcessing(statusMessage.OrderId);
                    break;
                
                case Shared.Domain.OrderStatus.Completed:
                    await HandleOrderCompleted(statusMessage.OrderId);
                    break;
                
                case Shared.Domain.OrderStatus.Cancelled:
                    await HandleOrderCancelled(statusMessage.OrderId);
                    break;
                
                case Shared.Domain.OrderStatus.Error:
                    await HandleOrderError(statusMessage.OrderId);
                    break;
                
                default:
                    _logger.LogInformation("No specific handling required for status {Status} on order {OrderId}", 
                        statusMessage.NewStatus, statusMessage.OrderId);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order status changed event for order {OrderId}", statusMessage.OrderId);
        }
    }

    private async Task ProcessOrderBusinessLogic(int orderId, OrderDetailsDto orderDetails)
    {
        // Implement order business logic here
        _logger.LogDebug("Processing business logic for order {OrderId}", orderId);
        
        // Example: Validate order data, perform calculations, update external systems, etc.
        await Task.CompletedTask;
    }

    private async Task HandleOrderProcessing(int orderId)
    {
        _logger.LogInformation("Handling order processing for order {OrderId}", orderId);
        // Implement processing logic
        await Task.CompletedTask;
    }

    private async Task HandleOrderCompleted(int orderId)
    {
        _logger.LogInformation("Handling order completion for order {OrderId}", orderId);
        // Implement completion logic (e.g., send confirmation email, update inventory, etc.)
        await Task.CompletedTask;
    }

    private async Task HandleOrderCancelled(int orderId)
    {
        _logger.LogInformation("Handling order cancellation for order {OrderId}", orderId);
        // Implement cancellation logic (e.g., revert inventory, process refunds, etc.)
        await Task.CompletedTask;
    }

    private async Task HandleOrderError(int orderId)
    {
        _logger.LogError("Handling order error for order {OrderId}", orderId);
        // Implement error handling logic (e.g., notify support, retry mechanisms, etc.)
        await Task.CompletedTask;
    }
}
