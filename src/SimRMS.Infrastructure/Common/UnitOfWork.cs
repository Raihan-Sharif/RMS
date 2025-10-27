using LB.DAL.Core.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimRMS.Infrastructure.Interfaces.Common;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       UnitOfWork
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Unit of Work Pattern for Database Transactions by using lb.DAL.Core.Common
/// Creation:    03/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Infrastructure.Common
{
    /// <summary>
    /// Simplified UnitOfWork implementation for transaction management
    /// Shared across all repositories and services
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ILB_DAL _dal;
        private readonly ILogger<UnitOfWork> _logger;
        private bool _disposed = false;
        private bool _isTransactionActive = false;

        public UnitOfWork([FromKeyedServices("DBApplication")] ILB_DAL dal, ILogger<UnitOfWork> logger)
        {
            _dal = dal;
            _logger = logger;
            _dal.LB_GetConnectionAsync().Wait();
        }

        public bool IsTransactionActive => _isTransactionActive;

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_isTransactionActive)
                {
                    _logger.LogWarning("Transaction is already active. Skipping BeginTransaction.");
                    return;
                }

                await _dal.LB_TransactionBeginAsync();
                _isTransactionActive = true;
                _logger.LogDebug("Transaction started successfully");
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
                if (!_isTransactionActive)
                {
                    _logger.LogWarning("No active transaction to commit");
                    return;
                }

                await _dal.LB_TransactionCommitAsync();
                _isTransactionActive = false;
                _logger.LogDebug("Transaction committed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing transaction");
                _isTransactionActive = false;
                throw;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (!_isTransactionActive)
                {
                    _logger.LogWarning("No active transaction to rollback");
                    return;
                }

                await _dal.LB_TransactionAbortAsync();
                _isTransactionActive = false;
                _logger.LogDebug("Transaction rolled back successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rolling back transaction");
                _isTransactionActive = false;
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

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
        {
            var wasTransactionActive = _isTransactionActive;

            if (!wasTransactionActive)
            {
                await BeginTransactionAsync(cancellationToken);
            }

            try
            {
                var result = await operation();

                if (!wasTransactionActive)
                {
                    await CommitTransactionAsync(cancellationToken);
                }

                return result;
            }
            catch
            {
                if (!wasTransactionActive && _isTransactionActive)
                {
                    await RollbackTransactionAsync(cancellationToken);
                }
                throw;
            }
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            await ExecuteInTransactionAsync(async () =>
            {
                await operation();
                return true;
            }, cancellationToken);
        }

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                if (_isTransactionActive)
                {
                    _dal.LB_TransactionAbortAsync().Wait();
                    _isTransactionActive = false;
                }

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