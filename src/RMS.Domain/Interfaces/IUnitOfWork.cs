using RMS.Domain.Entities;

namespace RMS.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<RiskAssessment> RiskAssessments { get; }
    IRepository<AuditLog> AuditLogs { get; }
    IRepository<UsrInfo> UsrInfos { get; } // Now uses generic repository

    // Generic method to get any repository
    IRepository<TEntity> Repository<TEntity>() where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}