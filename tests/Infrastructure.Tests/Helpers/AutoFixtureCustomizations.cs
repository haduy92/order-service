using AutoFixture;
using AutoFixture.Kernel;
using Domain.Entities;
using Domain.Entities.ValueObjects;

namespace Infrastructure.Tests.Helpers;

/// <summary>
/// AutoFixture customizations to handle Entity Framework navigation properties and circular references
/// </summary>
public static class AutoFixtureCustomizations
{
    /// <summary>
    /// Creates a configured Fixture that handles EF navigation properties correctly
    /// </summary>
    public static Fixture CreateEntityFrameworkFixture()
    {
        var fixture = new Fixture();

        // Remove throwing recursion behavior and add omit recursion behavior
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Add entity-specific customizations
        fixture.Customize<Order>(composer =>
            composer
                .With(o => o.Id, 0) // Always start with 0 for new entities
                .With(o => o.OrderItems, new List<OrderItem>()) // Empty collection to avoid circular reference
                .Without(o => o.ShippingAddress)); // Will set separately if needed

        fixture.Customize<OrderItem>(composer =>
            composer
                .With(oi => oi.Id, 0) // Always start with 0 for new entities
                .With(oi => oi.OrderId, 0) // Will be set manually in tests
                .Without(oi => oi.Order)); // Remove navigation property to avoid circular reference

        // Add specimen builder for Address value object
        fixture.Customizations.Add(new AddressSpecimenBuilder());

        return fixture;
    }

    /// <summary>
    /// Creates a minimal Order entity for testing without navigation properties
    /// </summary>
    public static Order CreateMinimalOrder(this Fixture fixture)
    {
        return new Order
        {
            Id = 0,
            OrderDate = fixture.Create<DateTime>(),
            TotalAmount = fixture.Create<decimal>(),
            Status = fixture.Create<Shared.Domain.OrderStatus>(),
            OrderItems = new List<OrderItem>(),
            ShippingAddress = null
        };
    }

    /// <summary>
    /// Creates a minimal OrderItem entity for testing without navigation properties
    /// </summary>
    public static OrderItem CreateMinimalOrderItem(this Fixture fixture, int orderId = 0)
    {
        return new OrderItem
        {
            Id = 0,
            OrderId = orderId,
            Order = null!, // Explicitly set to null to avoid circular reference
            ProductName = fixture.Create<string>(),
            Quantity = fixture.Create<int>(),
            Price = fixture.Create<decimal>()
        };
    }

    /// <summary>
    /// Creates an Order with OrderItems, handling the relationship correctly
    /// </summary>
    public static Order CreateOrderWithItems(this Fixture fixture, int numberOfItems = 3)
    {
        var order = fixture.CreateMinimalOrder();
        
        // Create order items and establish the relationship
        var orderItems = new List<OrderItem>();
        for (int i = 0; i < numberOfItems; i++)
        {
            var orderItem = fixture.CreateMinimalOrderItem();
            orderItem.OrderId = 0; // Will be set after order is saved
            orderItems.Add(orderItem);
        }
        
        order.OrderItems = orderItems;
        return order;
    }
}

/// <summary>
/// Custom specimen builder for Address value object
/// </summary>
public class AddressSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(Address))
        {
            return new Address(
                Street: context.Create<string>(),
                City: context.Create<string>(),
                PostCode: context.Create<string>(),
                Country: context.Create<string>()
            );
        }

        return new NoSpecimen();
    }
}
