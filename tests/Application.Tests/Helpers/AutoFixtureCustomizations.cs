using AutoFixture;
using AutoFixture.Kernel;
using Domain.Entities;
using Domain.Entities.ValueObjects;

namespace Application.Tests.Helpers;

/// <summary>
/// AutoFixture customizations for Application layer tests
/// </summary>
public static class AutoFixtureCustomizations
{
    /// <summary>
    /// Creates a configured Fixture for Application layer testing
    /// </summary>
    public static Fixture CreateApplicationFixture()
    {
        var fixture = new Fixture();

        // Remove throwing recursion behavior and add omit recursion behavior
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Add entity-specific customizations
        fixture.Customize<Order>(composer =>
            composer
                .With(o => o.Id, () => fixture.Create<int>())
                .With(o => o.OrderItems, new List<OrderItem>())
                .Without(o => o.ShippingAddress));

        fixture.Customize<OrderItem>(composer =>
            composer
                .With(oi => oi.Id, () => fixture.Create<int>())
                .With(oi => oi.OrderId, () => fixture.Create<int>())
                .Without(oi => oi.Order));

        // Add specimen builder for Address value object
        fixture.Customizations.Add(new AddressSpecimenBuilder());

        return fixture;
    }

    /// <summary>
    /// Creates an Order with OrderItems for testing
    /// </summary>
    public static Order CreateOrderWithItems(this Fixture fixture, int? orderId = null, int numberOfItems = 3)
    {
        var order = fixture.Build<Order>()
            .With(o => o.Id, orderId ?? fixture.Create<int>())
            .With(o => o.OrderItems, new List<OrderItem>())
            .With(o => o.ShippingAddress, fixture.Create<Address>())
            .Create();

        var orderItems = fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order.Id)
            .Without(oi => oi.Order)
            .CreateMany(numberOfItems)
            .ToList();

        order.OrderItems = orderItems;

        return order;
    }
}

/// <summary>
/// Specimen builder for Address value object
/// </summary>
public class AddressSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(Address))
        {
            var street = context.Create<string>();
            var city = context.Create<string>();
            var postCode = context.Create<string>();
            var country = context.Create<string>();
            
            return new Address(street, city, postCode, country);
        }

        return new NoSpecimen();
    }
}
