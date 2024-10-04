using EnglishClass.Domain.Entities.Histories;
using EnglishClass.Infrastructure.Interfaces.Repositories;

namespace EnglishClass.Infrastructure.Repositories;

public class ChangeSetRepository : IChangeSetRepository
{
    private readonly AppDbContext _dbContext;

    public ChangeSetRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Save(EntityChangeSet entityChangeSet)
    {
        _dbContext.EntityChangeSets.Add(entityChangeSet);
        _dbContext.SaveChanges();
    }

    public async Task SaveAsync(EntityChangeSet entityChangeSet)
    {
        _dbContext.EntityChangeSets.Add(entityChangeSet);
        await _dbContext.SaveChangesAsync();
    }
}
