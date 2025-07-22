using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RMS.Domain.Interfaces;
using RMS.Infrastructure.Data;
using RMS.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Infrastructure.Repositories
{
    public class SimpleGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly DbEfbtxLbslContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ILogger<SimpleGenericRepository<TEntity>> _logger;
        private readonly IEntityType? _entityType;
        private readonly IProperty? _primaryKey;

        public SimpleGenericRepository(DbEfbtxLbslContext context, ILogger<SimpleGenericRepository<TEntity>> logger)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _logger = logger;
            _entityType = _context.Model.FindEntityType(typeof(TEntity));
            _primaryKey = _entityType?.FindPrimaryKey()?.Properties.First();
        }

        public virtual async Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Getting {EntityType} by ID: {Id}", typeof(TEntity).Name, id);

            if (id == null) return null;

            try
            {
                return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting {EntityType} by ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        public virtual async Task<TEntity?> GetByIdAsync(params object[] keyValues)
        {
            _logger.LogDebug("Getting {EntityType} by composite key: {KeyValues}", typeof(TEntity).Name, string.Join(",", keyValues));

            try
            {
                return await _dbSet.FindAsync(keyValues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting {EntityType} by composite key", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Getting all {EntityType} records", typeof(TEntity).Name);

            try
            {
                return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all {EntityType} records", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Finding {EntityType} records with predicate", typeof(TEntity).Name);

            try
            {
                return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding {EntityType} records", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Getting first {EntityType} with predicate", typeof(TEntity).Name);

            try
            {
                return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting first {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbSet.AnyAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if {EntityType} exists", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var count = predicate == null
                    ? await _dbSet.CountAsync(cancellationToken)
                    : await _dbSet.CountAsync(predicate, cancellationToken);

                _logger.LogDebug("{EntityType} count: {Count}", typeof(TEntity).Name, count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting {EntityType} records", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<PagedResult<TEntity>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            bool ascending = true,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting paged {EntityType} - Page: {PageNumber}, Size: {PageSize}",
                typeof(TEntity).Name, pageNumber, pageSize);

            try
            {
                var query = _dbSet.AsNoTracking().AsQueryable();

                // Apply filter
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                // Apply ordering
                if (orderBy != null)
                {
                    query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
                }
                else
                {
                    // Default ordering by primary key for consistent pagination
                    query = ApplyDefaultOrdering(query);
                }

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Retrieved {ItemCount} {EntityType} items for page {PageNumber} out of {TotalCount} total",
                    items.Count, typeof(TEntity).Name, pageNumber, totalCount);

                return new PagedResult<TEntity>
                {
                    Data = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged {EntityType} data", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Adding {EntityType}", typeof(TEntity).Name);

            try
            {
                // Set creation date if entity has UsrCreationDate property
                SetCreationDate(entity);

                await _dbSet.AddAsync(entity, cancellationToken);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Adding range of {EntityType}", typeof(TEntity).Name);

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
                _logger.LogError(ex, "Error adding range of {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Updating {EntityType}", typeof(TEntity).Name);

            try
            {
                // Set last updated date if entity has UsrLastUpdatedDate property
                SetLastUpdatedDate(entity);

                _dbSet.Update(entity);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Deleting {EntityType}", typeof(TEntity).Name);

            try
            {
                _dbSet.Remove(entity);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Deleting range of {EntityType}", typeof(TEntity).Name);

            try
            {
                _dbSet.RemoveRange(entities);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting range of {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<bool> ExistsAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
        {
            if (id == null) return false;

            try
            {
                var entity = await GetByIdAsync(id, cancellationToken);
                return entity != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if {EntityType} exists for ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        public string GetPrimaryKeyName()
        {
            return _primaryKey?.Name ?? "Id";
        }

        public Type GetPrimaryKeyType()
        {
            return _primaryKey?.ClrType ?? typeof(Guid);
        }

        public TKey? GetEntityId<TKey>(TEntity entity)
        {
            try
            {
                var keyProperty = _primaryKey?.PropertyInfo;
                if (keyProperty != null)
                {
                    var value = keyProperty.GetValue(entity);
                    return (TKey?)value;
                }
                return default(TKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entity ID for {EntityType}", typeof(TEntity).Name);
                return default(TKey);
            }
        }

        private void SetCreationDate(TEntity entity)
        {
            var creationDateProperty = typeof(TEntity).GetProperty("UsrCreationDate");
            if (creationDateProperty != null && creationDateProperty.CanWrite)
            {
                creationDateProperty.SetValue(entity, DateTime.UtcNow);
            }
        }

        private void SetLastUpdatedDate(TEntity entity)
        {
            var lastUpdatedProperty = typeof(TEntity).GetProperty("UsrLastUpdatedDate");
            if (lastUpdatedProperty != null && lastUpdatedProperty.CanWrite)
            {
                lastUpdatedProperty.SetValue(entity, DateTime.UtcNow);
            }
        }

        private IQueryable<TEntity> ApplyDefaultOrdering(IQueryable<TEntity> query)
        {
            try
            {
                if (_primaryKey != null)
                {
                    var keyProperty = _primaryKey.PropertyInfo;
                    if (keyProperty != null)
                    {
                        var parameter = Expression.Parameter(typeof(TEntity));
                        var property = Expression.Property(parameter, keyProperty);
                        var lambda = Expression.Lambda(property, parameter);

                        var orderByMethod = "OrderBy";
                        var orderByExpression = Expression.Call(
                            typeof(Queryable),
                            orderByMethod,
                            new Type[] { typeof(TEntity), keyProperty.PropertyType },
                            query.Expression,
                            Expression.Quote(lambda));

                        return query.Provider.CreateQuery<TEntity>(orderByExpression);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not apply default ordering for {EntityType}", typeof(TEntity).Name);
            }

            return query;
        }
    }
}