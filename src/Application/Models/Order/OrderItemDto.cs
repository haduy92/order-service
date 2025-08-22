namespace Application.Models.Order;

public sealed record OrderItemDto
{
    public int Id { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    public decimal Total { get; init; }
    public int OrderId { get; init; }
}
