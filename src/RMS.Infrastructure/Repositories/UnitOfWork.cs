using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RMS.Domain.Entities;
using RMS.Domain.Interfaces;
using RMS.Infrastructure.Data;

namespace RMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbEfbtxLbslContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UnitOfWork> _logger;
        private IDbContextTransaction? _transaction;

        // Cache repositories to avoid creating multiple instances
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        public UnitOfWork(
            DbEfbtxLbslContext context,
            IServiceProvider serviceProvider,
            ILogger<UnitOfWork> logger)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // For domain entities, create simple repository wrappers
        //public IRepository<User> Users => GetRepository<User>();

        private IRepository<T> GetRepository<T>() where T : class
        {
            return (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ =>
            {
                // Create SimpleRepository directly if service provider doesn't have it
                var loggerType = typeof(ILogger<>).MakeGenericType(typeof(SimpleRepository<T>));
                var logger = _serviceProvider.GetRequiredService(loggerType);
                return new SimpleRepository<T>(_context, (ILogger<SimpleRepository<T>>)logger);
            });
        }

        // Generic repository for all entities (including scaffolded ones)
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            return (IGenericRepository<TEntity>)_repositories.GetOrAdd(typeof(TEntity), _ =>
            {
                // Create SimpleGenericRepository directly
                var loggerType = typeof(ILogger<>).MakeGenericType(typeof(SimpleGenericRepository<TEntity>));
                var logger = _serviceProvider.GetRequiredService(loggerType);
                return new SimpleGenericRepository<TEntity>(_context, (ILogger<SimpleGenericRepository<TEntity>>)logger);
            });
        }

        // Helper method to get DbContext for direct repository creation
        public DbEfbtxLbslContext GetContext() => _context;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _context.SaveChangesAsync(cancellationToken);
                _logger.LogDebug("SaveChanges completed. {Changes} changes saved", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving changes");
                throw;
            }
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                _logger.LogWarning("Transaction already in progress");
                return;
            }

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Transaction started");
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                _logger.LogWarning("No transaction to commit");
                return;
            }

            try
            {
                await _transaction.CommitAsync(cancellationToken);
                _logger.LogDebug("Transaction committed");
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                _logger.LogWarning("No transaction to rollback");
                return;
            }

            try
            {
                await _transaction.RollbackAsync(cancellationToken);
                _logger.LogDebug("Transaction rolled back");
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}