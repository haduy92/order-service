using Application.Contracts.Persistence;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

/// <summary>
/// Base class to implement <see cref="IRepository{TEntity,TPrimaryKey}"/>.
/// It implements some methods in most simple way.
/// </summary>
/// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
/// <typeparam name="TPrimaryKey">Primary key of the entity</typeparam>
public abstract class RepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
    where TEntity : class, Domain.Entities.IEntity<TPrimaryKey>
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<TEntity> _db;
    private readonly ISpecificationEvaluator _specificationEvaluator;

    protected RepositoryBase(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _db = dbContext.Set<TEntity>();
        _specificationEvaluator = SpecificationEvaluator.Default;
    }

    public async Task DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.FindAsync([id], cancellationToken);
        if (entity != null)
        {
            _db.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public IQueryable<TEntity> GetQueryable(params Expression<Func<TEntity, object>>[] includes)
    {
        var query = _db.AsQueryable();
        if (includes != null)
        {
            query = includes.Aggregate(query,
                (current, include) => current.Include(include));
        }
        return query;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetQueryable().ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
    {
        return await _db.FindAsync([id], cancellationToken);
    }

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _db.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _db.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _db.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Specification-based methods implementation
    public async Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult?> GetBySpecAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TResult>> GetListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification, true).CountAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification, true).AnyAsync(cancellationToken);
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification, bool evaluateCriteriaOnly = false)
    {
        return _specificationEvaluator.GetQuery(_db.AsQueryable(), specification, evaluateCriteriaOnly);
    }

    private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<TEntity, TResult> specification)
    {
        return _specificationEvaluator.GetQuery(_db.AsQueryable(), specification);
    }
}

