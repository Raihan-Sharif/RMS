using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimRMS.Domain.Entities;
using SimRMS.Domain.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LB.DAL.Core.Common;

namespace SimRMS.Infrastructure.Repositories
{
    /// <summary>
    /// Unit of Work implementation using LB.DAL
    /// Manages transactions and repository instances
    /// </summary>
    public class LBUnitOfWork : IUnitOfWork
    {
        private readonly ILB_DAL _dal;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LBUnitOfWork> _logger;
        private readonly ConcurrentDictionary<Type, object> _repositories;
        private DbTransaction? _currentTransaction;
        private bool _disposed;

        // Specific repositories
        private ILBRepository<UsrInfo>? _usrInfoRepository;
        private ILBRepository<UsrLogin>? _usrLoginRepository;
        private ILBRepository<AuditLog>? _auditLogRepository;

        public LBUnitOfWork(
            ILB_DAL dal,
            IServiceProvider serviceProvider,
            ILogger<LBUnitOfWork> logger)
        {
            _dal = dal;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _repositories = new ConcurrentDictionary<Type, object>();
        }

        public ILBRepository<UsrInfo> UsrInfoRepository
        {
            get
            {
                _usrInfoRepository ??= _serviceProvider.GetRequiredService<UsrInfoRepository>();
                return _usrInfoRepository;
            }
        }

        public ILBRepository<UsrLogin> UsrLoginRepository
        {
            get
            {
                _usrLoginRepository ??= _serviceProvider.GetRequiredService<UsrLoginRepository>();
                return _usrLoginRepository;
            }
        }

        public ILBRepository<AuditLog> AuditLogRepository
        {
            get
            {
                _auditLogRepository ??= Repository<AuditLog>();
                return _auditLogRepository;
            }
        }

        public ILBRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            return (ILBRepository<TEntity>)_repositories.GetOrAdd(typeof(TEntity), _ =>
            {
                var entityMapper = _serviceProvider.GetRequiredService<IEntityMapper>();
                var loggerType = typeof(ILogger<>).MakeGenericType(typeof(LBRepository<TEntity>));
                var logger = _serviceProvider.GetRequiredService(loggerType);

                return new LBRepository<TEntity>(_dal, entityMapper, (ILogger<LBRepository<TEntity>>)logger);
            });
        }

        public async Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_currentTransaction != null)
                {
                    _logger.LogWarning("Transaction already in progress");
                    return _currentTransaction;
                }

                await EnsureConnectionAsync(cancellationToken);
                _currentTransaction = await _dal.LB_TransactionBeginAsync();

                _logger.LogDebug("Transaction started");
                return _currentTransaction;
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
                if (_currentTransaction == null)
                {
                    _logger.LogWarning("No active transaction to commit");
                    return;
                }

                await _dal.LB_TransactionCommitAsync();
                _currentTransaction = null;

                _logger.LogDebug("Transaction committed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing transaction");
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_currentTransaction == null)
                {
                    _logger.LogWarning("No active transaction to rollback");
                    return;
                }

                await _dal.LB_TransactionAbortAsync();
                _currentTransaction = null;

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

        public void CloseConnection()
        {
            try
            {
                // LB.DAL manages connection lifecycle internally
                // Just ensure we clean up any pending transactions
                if (_currentTransaction != null)
                {
                    _dal.LB_TransactionAbort();
                    _currentTransaction = null;
                }

                _logger.LogDebug("Database connection closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing database connection");
            }
        }

        public ILB_DAL GetDAL()
        {
            return _dal;
        }

        public async Task<int> ExecuteBatchAsync(List<(string CommandText, List<LB_DALParam>? Parameters)> commands, CancellationToken cancellationToken = default)
        {
            if (commands == null || !commands.Any())
                return 0;

            try
            {
                _logger.LogDebug("Executing batch of {CommandCount} commands", commands.Count);

                var totalRowsAffected = 0;
                var wasInTransaction = _currentTransaction != null;

                // Start transaction if not already in one
                if (!wasInTransaction)
                {
                    await BeginTransactionAsync(cancellationToken);
                }

                try
                {
                    foreach (var (commandText, parameters) in commands)
                    {
                        var rowsAffected = await _dal.LB_ExecuteNonQueryAsync(commandText, parameters);
                        totalRowsAffected += rowsAffected;
                    }

                    // Commit if we started the transaction
                    if (!wasInTransaction)
                    {
                        await CommitTransactionAsync(cancellationToken);
                    }

                    _logger.LogDebug("Batch execution completed. Total rows affected: {RowsAffected}", totalRowsAffected);
                    return totalRowsAffected;
                }
                catch
                {
                    // Rollback if we started the transaction
                    if (!wasInTransaction)
                    {
                        await RollbackTransactionAsync(cancellationToken);
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing batch commands");
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                // Rollback any pending transaction
                if (_currentTransaction != null)
                {
                    _dal.LB_TransactionAbort();
                    _currentTransaction = null;
                }

                // Dispose the DAL (which handles connection cleanup)
                _dal?.Dispose();

                _repositories.Clear();
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
