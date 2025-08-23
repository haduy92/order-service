using AutoFixture;
using Infrastructure.Data;
using Infrastructure.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Application.Contracts.Application;
using FluentAssertions;

namespace Infrastructure.Tests.Base;

/// <summary>
/// Base class for all Infrastructure layer tests that need DbContext and AutoFixture
/// </summary>
public abstract class EntityTestBase : IDisposable
{
    protected readonly AppDbContext Context;
    protected readonly Mock<ICurrentUser> CurrentUserMock;
    protected readonly Fixture Fixture;
    protected readonly string TestUserId = "test-user-id";
    protected readonly string TestUsername = "test-user";

    protected EntityTestBase()
    {
        // Setup AutoFixture with EF customizations
        Fixture = AutoFixtureCustomizations.CreateEntityFrameworkFixture();

        // Setup current user mock
        CurrentUserMock = new Mock<ICurrentUser>();
        CurrentUserMock.Setup(x => x.UserId).Returns(TestUserId);
        CurrentUserMock.Setup(x => x.Username).Returns(TestUsername);
        CurrentUserMock.Setup(x => x.IsAuthenticated).Returns(true);

        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new AppDbContext(options, CurrentUserMock.Object);
    }

    /// <summary>
    /// Saves the context and clears change tracking for fresh queries
    /// </summary>
    protected async Task SaveAndClearAsync()
    {
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    /// <summary>
    /// Asserts that audit properties are set correctly for creation
    /// </summary>
    protected void AssertCreationAuditProperties(Domain.Entities.Entity entity)
    {
        entity.CreatorUserId.Should().Be(TestUserId);
        entity.CreationTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        entity.LastModifierUserId.Should().BeNull();
        entity.LastModificationTime.Should().BeNull();
    }

    /// <summary>
    /// Asserts that audit properties are set correctly for modification
    /// </summary>
    protected void AssertModificationAuditProperties(Domain.Entities.Entity entity)
    {
        entity.CreatorUserId.Should().Be(TestUserId);
        entity.CreationTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        entity.LastModifierUserId.Should().Be(TestUserId);
        entity.LastModificationTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    public virtual void Dispose()
    {
        Context.Dispose();
        GC.SuppressFinalize(this);
    }
}
