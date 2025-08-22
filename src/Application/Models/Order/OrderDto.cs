using Shared.Domain;

namespace Application.Models.Order;

public sealed record OrderDto
{
    public int Id { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public OrderStatus Status { get; init; }
    public string? Street { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public string? PostCode { get; init; }
}

