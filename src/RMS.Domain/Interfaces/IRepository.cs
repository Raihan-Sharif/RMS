using System.Linq.Expressions;
using RMS.Shared.Models;

namespace RMS.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    // Generic methods that work with any entity
    Task<T?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    // Additional helper methods
    Task<T?> GetByKeyAsync(params object[] keyValues);
    string GetPrimaryKeyName();
    Type GetPrimaryKeyType();
}