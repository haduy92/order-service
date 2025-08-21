namespace Shared.Domain;

public enum OrderStatus
{
    Unknown = 0,
    Created,
    Processing,
    Completed,
    Error,
    Cancelled
}

