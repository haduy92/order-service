using Application.Contracts.Persistence;
using Application.Queries;
using Application.Tests.Helpers;
using Ardalis.Specification;
using AutoFixture;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Shared.Exceptions;
using Xunit;

namespace Application.Tests.Queries;

public class GetOrderByIdQueryTests : IDisposable
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly GetOrderByIdQuery.Handler _handler;
    private readonly Fixture _fixture;

    public GetOrderByIdQueryTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _handler = new GetOrderByIdQuery.Handler(_orderRepositoryMock.Object);
        _fixture = AutoFixtureCustomizations.CreateApplicationFixture();
    }

    [Fact]
    public async Task WhenHandle_GivenValidOrderId_ThenReturnOrderDetailsDto()
    {
        // Arrange
        var orderId = 1;
        var order = _fixture.CreateOrderWithItems(orderId, numberOfItems: 2);

        _orderRepositoryMock
            .Setup(x => x.GetBySpecAsync(It.IsAny<ISpecification<Order>>(), default))
            .ReturnsAsync(order)
            .Verifiable();

        var query = new GetOrderByIdQuery { Id = orderId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(order.Id);
        result.OrderDate.Should().Be(order.OrderDate);
        result.TotalAmount.Should().Be(order.TotalAmount);
        result.Status.Should().Be(order.Status);
        result.Street.Should().Be(order.ShippingAddress?.Street);
        result.City.Should().Be(order.ShippingAddress?.City);
        result.Country.Should().Be(order.ShippingAddress?.Country);
        result.PostCode.Should().Be(order.ShippingAddress?.PostCode);
        result.OrderItems.Should().HaveCount(2);
        result.OrderItems.Should().AllSatisfy(item =>
        {
            item.OrderId.Should().Be(orderId);
            item.ProductName.Should().NotBeNullOrEmpty();
            item.Quantity.Should().BeGreaterThan(0);
            item.Price.Should().BeGreaterThan(0);
        });

        _orderRepositoryMock.Verify(x => x.GetBySpecAsync(It.IsAny<ISpecification<Order>>(), default), Times.Once);
    }

    [Fact]
    public async Task WhenHandle_GivenNonExistentOrderId_ThenThrowNotFoundException()
    {
        // Arrange
        var orderId = 999;

        _orderRepositoryMock
            .Setup(x => x.GetBySpecAsync(It.IsAny<ISpecification<Order>>(), default))
            .ReturnsAsync((Order?)null)
            .Verifiable();

        var query = new GetOrderByIdQuery { Id = orderId };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));

        exception.Message.Should().Be($"Order with ID {orderId} not found.");

        _orderRepositoryMock.Verify(x => x.GetBySpecAsync(It.IsAny<ISpecification<Order>>(), default), Times.Once);
    }

    [Fact]
    public async Task WhenHandle_GivenOrderWithoutShippingAddress_ThenReturnOrderDetailsDtoWithNullAddressFields()
    {
        // Arrange
        var orderId = 2;
        var order = _fixture.Build<Order>()
            .With(o => o.Id, orderId)
            .With(o => o.OrderItems, new List<OrderItem>())
            .Without(o => o.ShippingAddress)
            .Create();

        _orderRepositoryMock
            .Setup(x => x.GetBySpecAsync(It.IsAny<ISpecification<Order>>(), default))
            .ReturnsAsync(order)
            .Verifiable();

        var query = new GetOrderByIdQuery { Id = orderId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(order.Id);
        result.OrderDate.Should().Be(order.OrderDate);
        result.TotalAmount.Should().Be(order.TotalAmount);
        result.Status.Should().Be(order.Status);
        result.Street.Should().BeNull();
        result.City.Should().BeNull();
        result.Country.Should().BeNull();
        result.PostCode.Should().BeNull();
        result.OrderItems.Should().BeEmpty();

        _orderRepositoryMock.Verify(x => x.GetBySpecAsync(It.IsAny<ISpecification<Order>>(), default), Times.Once);
    }

    public void Dispose()
    {
        _orderRepositoryMock.Reset();
        GC.SuppressFinalize(this);
    }
}
