using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using RMS.Domain.Entities;
using RMS.Domain.Interfaces;
using RMS.Infrastructure.Data;

namespace RMS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private IDbContextTransaction? _transaction;

    // Cache repositories to avoid creating multiple instances
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public UnitOfWork(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    public IRepository<User> Users => Repository<User>();
    public IRepository<RiskAssessment> RiskAssessments => Repository<RiskAssessment>();
    public IRepository<AuditLog> AuditLogs => Repository<AuditLog>();
    public IRepository<UsrInfo> UsrInfos => Repository<UsrInfo>(); // Uses generic repository

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        return (IRepository<TEntity>)_repositories.GetOrAdd(typeof(TEntity), _ =>
        {
            return _serviceProvider.GetRequiredService<IRepository<TEntity>>();
        });
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
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