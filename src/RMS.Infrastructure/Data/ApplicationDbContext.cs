using System.Reflection;
using System.Linq.Expressions;
using RMS.Application.Interfaces;
using RMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace RMS.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService,
            IMediator mediator) : base(options)
        {
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RiskAssessment> RiskAssessments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<UsrInfo> UsrInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Map domain UsrInfo to database table
            modelBuilder.Entity<UsrInfo>().ToTable("UsrInfo");
            modelBuilder.Entity<UsrInfo>().HasKey(x => x.UsrId);

            // Global query filter for soft delete
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var condition = Expression.Equal(property, Expression.Constant(false));
                    var lambda = Expression.Lambda(condition, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedBy = _currentUserService.UserId;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        entry.Entity.DeletedBy = _currentUserService.UserId;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            // Dispatch domain events
            await DispatchDomainEventsAsync();

            return result;
        }

        private async Task DispatchDomainEventsAsync()
        {
            var domainEntities = ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }
    }
}
