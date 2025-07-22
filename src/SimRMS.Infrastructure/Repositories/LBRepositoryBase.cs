using LB.DAL.Core.Common;
using Microsoft.Extensions.Logging;
using SimRMS.Domain.Interfaces;
using SimRMS.Infrastructure.Data;
using SimRMS.Shared.Models;
using System.Data;
using System.Linq.Expressions;


namespace SimRMS.Infrastructure.Repositories
{
    /// <summary>
    /// Base repository implementation using LB.DAL
    /// Implements domain interfaces while containing infrastructure logic
    /// </summary>
    public class LBRepositoryBase<TEntity> : ILBRepository<TEntity> where TEntity : class
    {
        protected readonly ILB_DAL _dal;
        protected readonly LBEntityMapper _entityMapper;
        protected readonly ILogger<LBRepositoryBase<TEntity>> _logger;
        protected readonly string _tableName;
        protected readonly string _primaryKeyColumn;

        public LBRepositoryBase(ILB_DAL dal, LBEntityMapper entityMapper, ILogger<LBRepositoryBase<TEntity>> logger)
        {
            _dal = dal;
            _entityMapper = entityMapper;
            _logger = logger;
            _tableName = _entityMapper.GetTableName<TEntity>();
            _primaryKeyColumn = _entityMapper.GetPrimaryKeyName<TEntity>();
        }

        #region Domain Interface Implementation

        public virtual async Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
        {
            if (id == null) return null;

            try
            {
                _logger.LogDebug("Getting {EntityType} by ID: {Id}", typeof(TEntity).Name, id);

                var query = $"SELECT * FROM {_tableName} WHERE {_primaryKeyColumn} = @{_primaryKeyColumn}";
                var parameters = new List<LB_DALParam> { new LB_DALParam(_primaryKeyColumn, id) };

                using var reader = await _dal.LB_GetDbDataReaderAsync(query, parameters, CommandType.Text);

                if (await reader.ReadAsync())
                {
                    return _entityMapper.MapToEntity<TEntity>(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting {EntityType} by ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting all {EntityType} records", typeof(TEntity).Name);

                var query = $"SELECT * FROM {_tableName}";
                using var reader = await _dal.LB_GetDbDataReaderAsync(query, null, CommandType.Text);
                return await _entityMapper.MapToListAsync<TEntity>(reader);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all {EntityType} records", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            // For this implementation, we'll use a simple approach
            // In a more sophisticated implementation, you'd translate the expression to SQL
            var all = await GetAllAsync(cancellationToken);
            return all.AsQueryable().FirstOrDefault(predicate);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            // For this implementation, we'll use a simple approach
            // In a more sophisticated implementation, you'd translate the expression to SQL
            var all = await GetAllAsync(cancellationToken);
            return all.AsQueryable().Where(predicate).ToList();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var all = await GetAllAsync(cancellationToken);
            return all.AsQueryable().Any(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                {
                    var query = $"SELECT COUNT(*) FROM {_tableName}";
                    var result = await _dal.LB_ExecuteScalarAsync(query, null, CommandType.Text);
                    return Convert.ToInt32(result);
                }
                else
                {
                    // For predicates, get all and filter (simple approach)
                    var all = await GetAllAsync(cancellationToken);
                    return all.AsQueryable().Count(predicate);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting {EntityType} records", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting paged {EntityType} - Page: {PageNumber}, Size: {PageSize}", typeof(TEntity).Name, pageNumber, pageSize);

                // Simple paging implementation - in production, you'd want more sophisticated SQL
                var offset = (pageNumber - 1) * pageSize;
                var query = $@"
                    SELECT * FROM {_tableName}
                    ORDER BY {_primaryKeyColumn}
                    OFFSET @Offset ROWS 
                    FETCH NEXT @PageSize ROWS ONLY";

                var parameters = new List<LB_DALParam>
                {
                    new LB_DALParam("Offset", offset),
                    new LB_DALParam("PageSize", pageSize)
                };

                using var reader = await _dal.LB_GetDbDataReaderAsync(query, parameters, CommandType.Text);
                var data = await _entityMapper.MapToListAsync<TEntity>(reader);

                // Get total count
                var totalCount = await CountAsync(predicate, cancellationToken);

                return new PagedResult<TEntity>
                {
                    Data = data,
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
            try
            {
                _logger.LogDebug("Adding {EntityType}", typeof(TEntity).Name);

                // Set creation date if entity has this property
                SetCreationDate(entity);

                var parameters = _entityMapper.CreateParametersFromEntity(entity);
                var insertQuery = GenerateInsertQuery();

                await _dal.LB_ExecuteNonQueryAsync(insertQuery, parameters, CommandType.Text);
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
            try
            {
                _logger.LogDebug("Adding range of {EntityType}", typeof(TEntity).Name);

                var entityList = entities.ToList();
                foreach (var entity in entityList)
                {
                    SetCreationDate(entity);
                    var parameters = _entityMapper.CreateParametersFromEntity(entity);
                    var insertQuery = GenerateInsertQuery();
                    await _dal.LB_ExecuteNonQueryAsync(insertQuery, parameters, CommandType.Text);
                }

                return entityList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding range of {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Updating {EntityType}", typeof(TEntity).Name);

                // Set last updated date if entity has this property
                SetLastUpdatedDate(entity);

                var parameters = _entityMapper.CreateParametersFromEntity(entity);
                var updateQuery = GenerateUpdateQuery();

                var rowsAffected = await _dal.LB_ExecuteNonQueryAsync(updateQuery, parameters, CommandType.Text);

                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException($"No {typeof(TEntity).Name} was updated. Entity may not exist.");
                }

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                var id = _entityMapper.GetEntityId<object, TEntity>(entity);
                return await DeleteByIdAsync(id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<bool> DeleteByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

                var query = $"DELETE FROM {_tableName} WHERE {_primaryKeyColumn} = @{_primaryKeyColumn}";
                var parameters = new List<LB_DALParam> { new LB_DALParam(_primaryKeyColumn, id) };

                var rowsAffected = await _dal.LB_ExecuteNonQueryAsync(query, parameters, CommandType.Text);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        public virtual async Task<bool> ExistsAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
        {
            if (id == null) return false;

            try
            {
                var query = $"SELECT COUNT(1) FROM {_tableName} WHERE {_primaryKeyColumn} = @{_primaryKeyColumn}";
                var parameters = new List<LB_DALParam> { new LB_DALParam(_primaryKeyColumn, id) };

                var result = await _dal.LB_ExecuteScalarAsync(query, parameters, CommandType.Text);
                var count = Convert.ToInt32(result);
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if {EntityType} exists with ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        #endregion

        #region Protected Helper Methods

        protected virtual string GenerateInsertQuery()
        {
            // This is a simplified version - implement based on your entity structure
            // In a full implementation, you'd analyze the entity properties and generate the INSERT statement
            return $"INSERT INTO {_tableName} VALUES (...)"; // You'll need to implement this based on entity properties
        }

        protected virtual string GenerateUpdateQuery()
        {
            // This is a simplified version - implement based on your entity structure
            // In a full implementation, you'd analyze the entity properties and generate the UPDATE statement
            return $"UPDATE {_tableName} SET ... WHERE {_primaryKeyColumn} = @{_primaryKeyColumn}";
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

        #endregion
    }
}
