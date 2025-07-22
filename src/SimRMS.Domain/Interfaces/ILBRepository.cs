using SimRMS.Shared.Models;
using System.Linq.Expressions;

namespace SimRMS.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface for data operations
    /// Pure domain interface - no infrastructure dependencies
    /// </summary>
    public interface ILBRepository<TEntity> where TEntity : class
    {
        // Basic CRUD operations
        Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

        // Pagination
        Task<PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

        // Modification operations
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default);

        // Utility methods
        Task<bool> ExistsAsync<TKey>(TKey id, CancellationToken cancellationToken = default);
    }
}
