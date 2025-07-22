using SimRMS.Domain.Entities;
using System.Data.Common;

namespace SimRMS.Domain.Interfaces;

/// <summary>
/// Unit of Work interface for managing database operations
/// Pure domain interface - no infrastructure dependencies
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Domain-specific repositories
    IUsrInfoRepository UsrInfoRepository { get; }
    IUsrLoginRepository UsrLoginRepository { get; }
    ILBRepository<AuditLog> AuditLogRepository { get; }

    // Generic repository for any entity
    ILBRepository<TEntity> Repository<TEntity>() where TEntity : class;

    // Transaction management
    Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    // Connection management
    Task EnsureConnectionAsync(CancellationToken cancellationToken = default);
    void CloseConnection();

    // Batch operations
    Task<int> ExecuteBatchAsync(List<(string CommandText, object?[]? Parameters)> commands, CancellationToken cancellationToken = default);
}