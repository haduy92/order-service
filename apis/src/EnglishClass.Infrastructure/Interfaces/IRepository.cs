using System.Linq.Expressions;
using EnglishClass.Common.Dependencies;
using EnglishClass.Domain.Entities;

namespace EnglishClass.Infrastructure.Interfaces;

/// <summary>
/// This interface is a shortcut to IRepository<TEntity, Guid>.
/// </summary>
public interface IRepository<TEntity> : IRepository<TEntity, Guid> where TEntity : class, IEntity<Guid>
{ }

/// <summary>
/// This interface is implemented by all repositories to ensure implementation of fixed methods.
/// </summary>
/// <typeparam name="TEntity">Main Entity type this repository works on</typeparam>
/// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
public interface IRepository<TEntity, TPrimaryKey> : ITransientDependency where TEntity : class, IEntity<TPrimaryKey>
{
    /// <summary>
    /// Used to get a IQueryable that is used to retrieve entities from entire table.
    /// </summary>
    /// <param name="includes">A list of include expressions.</param>
    /// <returns>IQueryable to be used to select entities from database</returns>
    IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Used to get all entities.
    /// </summary>
    /// <returns>IEnumerable of all entities</returns>
    Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>
    /// Gets an entity with given primary key.
    /// </summary>
    /// <param name="id">Primary key of the entity to get</param>
    /// <returns>Entity</returns>
    Task<TEntity?> GetAsync(TPrimaryKey id);

    /// <summary>
    /// Inserts a new entity.
    /// </summary>
    /// <param name="entity">Inserted entity</param>
    Task<TEntity> InsertAsync(TEntity entity);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">Entity</param>
    Task<TEntity> UpdateAsync(TEntity entity);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">Entity to be deleted</param>
    Task DeleteAsync(TEntity entity);

    /// <summary>
    /// Deletes an entity by primary key.
    /// </summary>
    /// <param name="id">Primary key of the entity</param>
    Task DeleteAsync(TPrimaryKey id);
}