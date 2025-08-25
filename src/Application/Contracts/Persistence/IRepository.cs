using System.Linq.Expressions;
using Ardalis.Specification;

namespace Application.Contracts.Persistence;

/// <summary>
/// This interface is a shortcut to IRepository<TEntity, int>.
/// </summary>
public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, Domain.Entities.IEntity<int>
{ }

/// <summary>
/// This interface is implemented by all repositories to ensure implementation of fixed methods.
/// </summary>
/// <typeparam name="TEntity">Main Entity type this repository works on</typeparam>
/// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
public interface IRepository<TEntity, TPrimaryKey> where TEntity : class, Domain.Entities.IEntity<TPrimaryKey>
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

    // Specification-based methods
    /// <summary>
    /// Gets a single entity that matches the specification.
    /// </summary>
    /// <param name="specification">Specification to filter entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entity that matches the specification</returns>
    Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single projected result that matches the specification.
    /// </summary>
    /// <typeparam name="TResult">Type of the projected result</typeparam>
    /// <param name="specification">Specification to filter and project entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Projected result that matches the specification</returns>
    Task<TResult?> GetBySpecAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that match the specification.
    /// </summary>
    /// <param name="specification">Specification to filter entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of entities that match the specification</returns>
    Task<IEnumerable<TEntity>> GetListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all projected results that match the specification.
    /// </summary>
    /// <typeparam name="TResult">Type of the projected result</typeparam>
    /// <param name="specification">Specification to filter and project entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of projected results that match the specification</returns>
    Task<IEnumerable<TResult>> GetListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of entities that match the specification.
    /// </summary>
    /// <param name="specification">Specification to filter entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Count of entities that match the specification</returns>
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entity matches the specification.
    /// </summary>
    /// <param name="specification">Specification to filter entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any entity matches the specification</returns>
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}
