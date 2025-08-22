using Application.Models.Order;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using Shared.Domain;

namespace Application.Mappers;

public static class OrderMapper
{
    public static OrderDetailsDto ToOrderDetailDto(Order order)
    {
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order), "Order cannot be null");
        }
        return new OrderDetailsDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Street = order.ShippingAddress?.Street,
            City = order.ShippingAddress?.City,
            Country = order.ShippingAddress?.Country,
            PostCode = order.ShippingAddress?.PostCode,
            OrderItems = order.OrderItems.Select(item => new OrderItemDto
            {
                Id = item.Id,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Price,
                Total = item.Total,
                OrderId = item.OrderId
            }).ToList()
        };
    }
    
    public static OrderDto ToOrderDto(Order order)
    {
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order), "Order cannot be null");
        }
        return new OrderDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Street = order.ShippingAddress?.Street,
            City = order.ShippingAddress?.City,
            Country = order.ShippingAddress?.Country,
            PostCode = order.ShippingAddress?.PostCode
        };
    }
    
    public static Order ToOrderEntity(OrderDetailsDto orderDetails)
    {
        if (orderDetails == null)
        {
            throw new ArgumentNullException(nameof(orderDetails), "OrderDetails cannot be null");
        }

        return new Order
        {
            OrderDate = DateTime.UtcNow,
            TotalAmount = orderDetails.TotalAmount,
            Status = OrderStatus.Created,
            ShippingAddress = CreateAddress(
                orderDetails.Street,
                orderDetails.City,
                orderDetails.PostCode,
                orderDetails.Country),
            OrderItems = orderDetails.OrderItems.Select(item => new OrderItem
            {
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList()
        };
    }
    
    public static Address? CreateAddress(string? street, string? city, string? postCode, string? country)
    {
        if (string.IsNullOrWhiteSpace(street) && string.IsNullOrWhiteSpace(city) && 
            string.IsNullOrWhiteSpace(postCode) && string.IsNullOrWhiteSpace(country))
        {
            return null;
        }
        
        return new Address(
            street ?? string.Empty,
            city ?? string.Empty,
            postCode ?? string.Empty,
            country ?? string.Empty
        );
    }
}

