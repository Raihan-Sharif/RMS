using RMS.Domain.Entities;

namespace RMS.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Domain entities (for your existing RMS entities)
    IRepository<User> Users { get; }
    IRepository<RiskAssessment> RiskAssessments { get; }
    IRepository<AuditLog> AuditLogs { get; }

    // Database entities (for scaffolded entities)
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

    // Transaction management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}