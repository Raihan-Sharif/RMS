namespace SimRMS.Domain.Interfaces.Common;

/// <summary>
/// Simplified UnitOfWork interface with only common transaction operations
/// Each repository/service can inject this for transaction management
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Begin a new database transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensure database connection is open
    /// </summary>
    Task EnsureConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if transaction is active
    /// </summary>
    bool IsTransactionActive { get; }

    /// <summary>
    /// Execute operation within a transaction scope
    /// </summary>
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute operation within a transaction scope (void)
    /// </summary>
    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
}
