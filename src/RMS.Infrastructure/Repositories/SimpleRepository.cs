using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using RMS.Domain.Interfaces;
using RMS.Infrastructure.Data;
using RMS.Shared.Models;

namespace RMS.Infrastructure.Repositories;

public class SimpleRepository<T> : IRepository<T> where T : class
{
    protected readonly DbEfbtxLbslContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<SimpleRepository<T>> _logger;
    private readonly IEntityType? _entityType;

    public SimpleRepository(DbEfbtxLbslContext context, ILogger<SimpleRepository<T>> logger)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _logger = logger;
        _entityType = _context.Model.FindEntityType(typeof(T));
    }

    public virtual async Task<T?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting {EntityType} by ID: {Id}", typeof(T).Name, id);

        if (id == null) return null;

        try
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting {EntityType} by ID: {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public virtual async Task<T?> GetByKeyAsync(params object[] keyValues)
    {
        _logger.LogDebug("Getting {EntityType} by key values: {KeyValues}", typeof(T).Name, string.Join(",", keyValues));

        try
        {
            return await _dbSet.FindAsync(keyValues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting {EntityType} by key values", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all {EntityType} records", typeof(T).Name);

        try
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all {EntityType} records", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Finding {EntityType} records with predicate", typeof(T).Name);

        try
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding {EntityType} records", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting first {EntityType} with predicate", typeof(T).Name);

        try
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting first {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if {EntityType} exists", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var count = predicate == null
                ? await _dbSet.CountAsync(cancellationToken)
                : await _dbSet.CountAsync(predicate, cancellationToken);

            _logger.LogDebug("{EntityType} count: {Count}", typeof(T).Name, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting {EntityType} records", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding {EntityType}", typeof(T).Name);

        try
        {
            // Set creation date if entity has this property
            SetCreationDate(entity);

            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding range of {EntityType}", typeof(T).Name);

        try
        {
            var entityList = entities.ToList();
            foreach (var entity in entityList)
            {
                SetCreationDate(entity);
            }

            await _dbSet.AddRangeAsync(entityList, cancellationToken);
            return entityList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding range of {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating {EntityType}", typeof(T).Name);

        try
        {
            // Set last updated date if entity has this property
            SetLastUpdatedDate(entity);

            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting {EntityType}", typeof(T).Name);

        try
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting range of {EntityType}", typeof(T).Name);

        try
        {
            _dbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting range of {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged {EntityType} - Page: {PageNumber}, Size: {PageSize}",
            typeof(T).Name, pageNumber, pageSize);

        try
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            // Apply filter
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            // Apply default ordering by primary key for consistent pagination
            query = ApplyDefaultOrdering(query);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {ItemCount} {EntityType} items for page {PageNumber} out of {TotalCount} total",
                items.Count, typeof(T).Name, pageNumber, totalCount);

            return new PagedResult<T>
            {
                Data = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged {EntityType} data", typeof(T).Name);
            throw;
        }
    }

    public string GetPrimaryKeyName()
    {
        var primaryKey = _entityType?.FindPrimaryKey();
        return primaryKey?.Properties.First().Name ?? "Id";
    }

    public Type GetPrimaryKeyType()
    {
        var primaryKey = _entityType?.FindPrimaryKey();
        return primaryKey?.Properties.First().ClrType ?? typeof(object);
    }

    private void SetCreationDate(T entity)
    {
        var creationDateProperty = typeof(T).GetProperty("UsrCreationDate");
        if (creationDateProperty != null && creationDateProperty.CanWrite)
        {
            creationDateProperty.SetValue(entity, DateTime.UtcNow);
        }
    }

    private void SetLastUpdatedDate(T entity)
    {
        var lastUpdatedProperty = typeof(T).GetProperty("UsrLastUpdatedDate");
        if (lastUpdatedProperty != null && lastUpdatedProperty.CanWrite)
        {
            lastUpdatedProperty.SetValue(entity, DateTime.UtcNow);
        }
    }

    private IQueryable<T> ApplyDefaultOrdering(IQueryable<T> query)
    {
        try
        {
            var primaryKey = _entityType?.FindPrimaryKey();
            if (primaryKey != null)
            {
                var keyProperty = primaryKey.Properties.First();
                var parameter = Expression.Parameter(typeof(T));
                var property = Expression.Property(parameter, keyProperty.PropertyInfo!);
                var lambda = Expression.Lambda(property, parameter);

                var orderByMethod = "OrderBy";
                var orderByExpression = Expression.Call(
                    typeof(Queryable),
                    orderByMethod,
                    new Type[] { typeof(T), keyProperty.ClrType },
                    query.Expression,
                    Expression.Quote(lambda));

                return query.Provider.CreateQuery<T>(orderByExpression);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not apply default ordering for {EntityType}", typeof(T).Name);
        }

        return query;
    }
}