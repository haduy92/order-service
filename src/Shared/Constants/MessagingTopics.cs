namespace Shared.Constants;

/// <summary>
/// Centralized messaging topic constants for RabbitMQ routing keys.
/// These topics are used across Application (publishers) and Consumer (subscribers) projects.
/// </summary>
public static class MessagingTopics
{
    /// <summary>
    /// Topic for order created events
    /// </summary>
    public const string OrderCreated = "order.created";
    public static string OrderCreatedQueue => $"{OrderCreated}.queue";

    /// <summary>
    /// Topic for order status changed events
    /// </summary>
    public const string OrderStatusChanged = "order.status.changed";
    public static string OrderStatusChangedQueue => $"{OrderStatusChanged}.queue";
}
