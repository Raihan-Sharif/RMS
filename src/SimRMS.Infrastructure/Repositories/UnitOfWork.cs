using LB.DAL.Core.Common;
using SimRMS.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimRMS.Domain.Interfaces.Repo;

namespace SimRMS.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Clean Architecture UnitOfWork implementation using LB.DAL
    /// Follows your existing UnitOfWorkCustomer pattern
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ILB_DAL _dal;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UnitOfWork> _logger;
        private bool _disposed = false;

        // Repositories
        private IUsrInfoRepository? _usrInfoRepository;

        public UnitOfWork([FromKeyedServices("DBApplication")] ILB_DAL dal,
                         IServiceProvider serviceProvider,
                         ILogger<UnitOfWork> logger)
        {
            _dal = dal;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _dal.LB_GetConnectionAsync().Wait();
        }

        public IUsrInfoRepository UsrInfoRepository
        {
            get
            {
                // Lazy initialization - following your pattern
                _usrInfoRepository ??= _serviceProvider.GetRequiredService<IUsrInfoRepository>();
                return _usrInfoRepository;
            }
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _dal.LB_TransactionBeginAsync();
                _logger.LogDebug("Transaction started");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting transaction");
                throw;
            }
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _dal.LB_TransactionCommitAsync();
                _logger.LogDebug("Transaction committed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing transaction");
                throw;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _dal.LB_TransactionAbortAsync();
                _logger.LogDebug("Transaction rolled back");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rolling back transaction");
                throw;
            }
        }

        public async Task EnsureConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _dal.LB_GetConnectionAsync();
                _logger.LogDebug("Database connection ensured");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring database connection");
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
              //  _dal?.Dispose();
                _disposed = true;
                _logger.LogDebug("UnitOfWork disposed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing UnitOfWork");
            }
        }
    }
}