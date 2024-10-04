using FlashCard.Application.Interfaces.Persistence.Cards;
using FlashCard.Domain.Entities;
using FlashCard.Infrastructure.Data;

namespace FlashCard.Infrastructure.Repositories;

public class CardRepository : RepositoryBase<Card, int>, ICardRepository
{
    public CardRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
