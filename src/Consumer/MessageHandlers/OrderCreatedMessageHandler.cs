using Application.Models.Message;
using Consumer.MessageHandlers;
using Consumer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Constants;

namespace Consumer.MessageHandlers;

/// <summary>
/// Handles order created messages from the message queue
/// Follows Single Responsibility Principle - only handles OrderCreated events
/// </summary>
public class OrderCreatedMessageHandler : BaseMessageHandler
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OrderCreatedMessageHandler(
        ILogger<OrderCreatedMessageHandler> logger, 
        IServiceScopeFactory serviceScopeFactory) : base(logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override string Topic => MessagingTopics.OrderCreated;
    public override string QueueName => MessagingTopics.OrderCreatedQueue;

    protected override async Task ProcessMessageAsync(string messageText)
    {
        var orderCreated = JsonConvert.DeserializeObject<OrderCreatedMessage>(messageText);
        
        if (orderCreated == null)
        {
            throw new InvalidOperationException($"Failed to deserialize OrderCreated message: {messageText}");
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var orderProcessingService = scope.ServiceProvider.GetRequiredService<IOrderProcessingService>();
        
        await orderProcessingService.ProcessOrderCreatedAsync(orderCreated);
    }
}
