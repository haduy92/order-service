using EnglishClass.Common.Dependencies;
using EnglishClass.Domain.Entities.Histories;

namespace EnglishClass.Infrastructure.Interfaces.Repositories;

public interface IChangeSetRepository : ITransientDependency
{
    /// <summary>
    /// Save entity change set to a persistent store.
    /// </summary>
    /// <param name="entityChangeSet">Entity change set</param>
    Task SaveAsync(EntityChangeSet entityChangeSet);

    /// <summary>
    /// Save entity change set to a persistent store.
    /// </summary>
    /// <param name="entityChangeSet">Entity change set</param>
    void Save(EntityChangeSet entityChangeSet);
}
