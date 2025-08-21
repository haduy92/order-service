using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Interfaces.Persistence;

/// <summary>
/// This interface is a shortcut to IRepository<TEntity, int>.
/// </summary>
public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, IEntity<int>
{ }

/// <summary>
/// This interface is implemented by all repositories to ensure implementation of fixed methods.
/// </summary>
/// <typeparam name="TEntity">Main Entity type this repository works on</typeparam>
/// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
public interface IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
{
    /// <summary>
    /// Used to get a IQueryable that is used to retrieve entities from entire table.
    /// </summary>
    /// <param name="includes">A list of include expressions.</param>
    /// <returns>IQueryable to be used to select entities from database</returns>
    IQueryable<TEntity> GetQueryable(params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Used to get all entities.
    /// </summary>
    /// <returns>IEnumerable of all entities</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity with given primary key.
    /// </summary>
    /// <param name="id">Primary key of the entity to get</param>
    /// <returns>Entity</returns>
    Task<TEntity?> GetAsync(TPrimaryKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new entity.
    /// </summary>
    /// <param name="entity">Inserted entity</param>
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">Entity</param>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">Entity to be deleted</param>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity by primary key.
    /// </summary>
    /// <param name="id">Primary key of the entity</param>
    Task DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default);
}
