using Shared.Domain;

namespace Application.Models.Order;

public record OrderDetailsDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostCode { get; set; }
    public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
}

