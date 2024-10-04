using EnglishClass.Domain.Entities;
using EnglishClass.Infrastructure.Data;
using EnglishClass.Infrastructure.Interfaces.Repositories;

namespace EnglishClass.Infrastructure.Repositories;

public class CardRepository : RepositoryBase<Card, Guid>, ICardRepository
{
    public CardRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
