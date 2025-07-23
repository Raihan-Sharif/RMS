using SimRMS.Domain.Interfaces.Repo;

namespace SimRMS.Domain.Interfaces
{
    /// <summary>
    /// Domain Unit of Work interface - follows Clean Architecture
    /// Contains only domain operations with no infrastructure dependencies
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Domain repositories
        IUsrInfoRepository UsrInfoRepository { get; }

        // Transaction management
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        // Connection management
        Task EnsureConnectionAsync(CancellationToken cancellationToken = default);
    }
}