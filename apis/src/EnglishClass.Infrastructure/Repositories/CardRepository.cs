using EnglishClass.Application.Interfaces.Persistence.Cards;
using EnglishClass.Domain.Entities;
using EnglishClass.Infrastructure.Data;

namespace EnglishClass.Infrastructure.Repositories;

public class CardRepository : RepositoryBase<Card, int>, ICardRepository
{
    public CardRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
