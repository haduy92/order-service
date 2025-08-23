using Application.Models.Message;
using Consumer.MessageHandlers;
using Consumer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Constants;

namespace Consumer.MessageHandlers;

/// <summary>
/// Handles order status changed messages from the message queue
/// Follows Single Responsibility Principle - only handles OrderStatusChanged events
/// </summary>
public class OrderStatusChangedMessageHandler : BaseMessageHandler
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OrderStatusChangedMessageHandler(
        ILogger<OrderStatusChangedMessageHandler> logger, 
        IServiceScopeFactory serviceScopeFactory) : base(logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override string Topic => MessagingTopics.OrderStatusChanged;
    public override string QueueName => MessagingTopics.OrderStatusChangedQueue;

    protected override async Task ProcessMessageAsync(string messageText)
    {
        var orderStatusChanged = JsonConvert.DeserializeObject<OrderStatusChangedMessage>(messageText);
        
        if (orderStatusChanged == null)
        {
            throw new InvalidOperationException($"Failed to deserialize OrderStatusChanged message: {messageText}");
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var orderProcessingService = scope.ServiceProvider.GetRequiredService<IOrderProcessingService>();
        
        await orderProcessingService.ProcessOrderStatusChangedAsync(orderStatusChanged);
    }
}
