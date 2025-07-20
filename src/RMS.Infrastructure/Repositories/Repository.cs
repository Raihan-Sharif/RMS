using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using RMS.Domain.Entities;
using RMS.Domain.Interfaces;
using RMS.Infrastructure.Data;
using RMS.Shared.Models;

namespace RMS.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbEfbtxLbslContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<Repository<T>> _logger;
    private readonly IEntityType _entityType;

    public Repository(DbEfbtxLbslContext context, ILogger<Repository<T>> logger)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _logger = logger;
        _entityType = _context.Model.FindEntityType(typeof(T))!;
    }

    public virtual async Task<T?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting {EntityType} by ID: {Id}", typeof(T).Name, id);

        if (id == null) return null;

        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<T?> GetByKeyAsync(params object[] keyValues)
    {
        _logger.LogDebug("Getting {EntityType} by key values: {KeyValues}", typeof(T).Name, string.Join(",", keyValues));
        return await _dbSet.FindAsync(keyValues);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all {EntityType} records", typeof(T).Name);
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Finding {EntityType} records with predicate", typeof(T).Name);
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting first {EntityType} with predicate", typeof(T).Name);
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var count = predicate == null
            ? await _dbSet.CountAsync(cancellationToken)
            : await _dbSet.CountAsync(predicate, cancellationToken);

        _logger.LogDebug("{EntityType} count: {Count}", typeof(T).Name, count);
        return count;
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding {EntityType}", typeof(T).Name);
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding range of {EntityType}", typeof(T).Name);
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating {EntityType}", typeof(T).Name);
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting {EntityType}", typeof(T).Name);
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting range of {EntityType}", typeof(T).Name);
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public virtual async Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting paged {EntityType} - Page: {PageNumber}, Size: {PageSize}",
            typeof(T).Name, pageNumber, pageSize);

        var query = _dbSet.AsQueryable();

        if (predicate != null)
        {
            query = query.Where(predicate);
            _logger.LogDebug("Applied predicate to {EntityType} query", typeof(T).Name);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        _logger.LogDebug("{EntityType} total count after filtering: {TotalCount}", typeof(T).Name, totalCount);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        _logger.LogDebug("Retrieved {ItemCount} {EntityType} items for page {PageNumber}",
            items.Count, typeof(T).Name, pageNumber);

        return new PagedResult<T>
        {
            Data = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public string GetPrimaryKeyName()
    {
        var primaryKey = _entityType.FindPrimaryKey();
        return primaryKey?.Properties.First().Name ?? "Id";
    }

    public Type GetPrimaryKeyType()
    {
        var primaryKey = _entityType.FindPrimaryKey();
        return primaryKey?.Properties.First().ClrType ?? typeof(object);
    }
}