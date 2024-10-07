using AutoFixture;
using AutoFixture.Xunit2;
using FlashCard.Application.Interfaces.Persistence.Cards;
using FlashCard.Domain.Entities;
using FlashCard.Infrastructure.Data;
using FlashCard.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FlashCard.Infrastructure.Test.Repositories;

public class CardRepositoryTest
{
    private readonly AppDbContext _dbContext;
    private readonly ICardRepository _repository;

    public CardRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();

        _repository = new CardRepository(_dbContext);
    }

    [Theory]
    [AutoData]
    public async Task WhenGetById_GivenIdNotExisted_ThenShouldReturnNull(IFixture fixture)
    {
        // Arrange
        var id = fixture.Create<int>();

        // Act
        var act = await _repository.GetAsync(id);

        // Assert
        act.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task WhenGetById_GivenIdExisted_ThenShouldReturnCard(IFixture fixture)
    {
        // Arrange
        var expect = fixture.Create<Card>();
        _dbContext.Cards.Add(expect);
        _dbContext.SaveChanges();

        // Act
        var actual = await _repository.GetAsync(expect.Id);

        // Assert
        actual.Should().BeEquivalentTo(expect);
    }
}
