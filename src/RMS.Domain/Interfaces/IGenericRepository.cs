using System.Linq.Expressions;
using RMS.Shared.Models;

namespace RMS.Domain.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        // Basic CRUD operations with flexible key types
        Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdAsync(params object[] keyValues);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

        // Pagination
        Task<PagedResult<TEntity>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            bool ascending = true,
            CancellationToken cancellationToken = default);

        // Modification operations
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        // Utility methods
        Task<bool> ExistsAsync<TKey>(TKey id, CancellationToken cancellationToken = default);
        string GetPrimaryKeyName();
        Type GetPrimaryKeyType();
        TKey? GetEntityId<TKey>(TEntity entity);
    }
}