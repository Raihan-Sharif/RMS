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
            modelBuilder.Entity<UsrInfo>(entity =>
            {
                entity.HasKey(e => e.UsrId);

                entity.ToTable("UsrInfo");

                entity.Property(e => e.UsrId)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("UsrID");
                entity.Property(e => e.Bfesname)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValue("")
                    .HasColumnName("BFESName");
                entity.Property(e => e.Category)
                    .HasMaxLength(5)
                    .IsUnicode(false);
                entity.Property(e => e.ChannelUpdFlag)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValue("N");
                entity.Property(e => e.ClntCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.CoBrchCode)
                    .HasMaxLength(6)
                    .IsUnicode(false);
                entity.Property(e => e.CoCode).HasMaxLength(5);
                entity.Property(e => e.DlrCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);
                entity.Property(e => e.GtcexpiryPeriod).HasColumnName("GTCExpiryPeriod");
                entity.Property(e => e.MimosMigrateDt).HasColumnType("datetime");
                entity.Property(e => e.MimosMigrateDtRms)
                    .HasColumnType("datetime")
                    .HasColumnName("MimosMigrateDt_rms");
                entity.Property(e => e.MktDepthEndDate).HasColumnType("datetime");
                entity.Property(e => e.MktDepthStartDate).HasColumnType("datetime");
                entity.Property(e => e.OriUsrEmail).HasMaxLength(255);
                entity.Property(e => e.Pid)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("PID");
                entity.Property(e => e.PidRms)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("PID_rms");
                entity.Property(e => e.Pidflag)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValue("N")
                    .HasColumnName("PIDFlag");
                entity.Property(e => e.PidflagRms)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValue("N")
                    .HasColumnName("PIDFlag_rms");
                entity.Property(e => e.RmsType)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValue("");
                entity.Property(e => e.ThirdPartyUsrId)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValue("")
                    .HasColumnName("ThirdPartyUsrID");
                entity.Property(e => e.UsrAccessFa)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValue("Y")
                    .HasColumnName("UsrAccessFA");
                entity.Property(e => e.UsrAddr).HasMaxLength(250);
                entity.Property(e => e.UsrAssctPwd).HasMaxLength(8);
                entity.Property(e => e.UsrBtxmode)
                    .HasMaxLength(1)
                    .HasColumnName("UsrBTXMode");
                entity.Property(e => e.UsrChannel)
                    .HasMaxLength(2)
                    .IsUnicode(false);
                entity.Property(e => e.UsrCreationDate).HasColumnType("datetime");
                entity.Property(e => e.UsrDob)
                    .HasColumnType("datetime")
                    .HasColumnName("UsrDOB");
                entity.Property(e => e.UsrEmail)
                    .HasMaxLength(255)
                    .HasComment("M - Male; F - Female; ");
                entity.Property(e => e.UsrExpiryDate).HasColumnType("datetime");
                entity.Property(e => e.UsrFax).HasMaxLength(15);
                entity.Property(e => e.UsrGender)
                    .HasMaxLength(2)
                    .HasComment("M - Male; F - Female; ");
                entity.Property(e => e.UsrGtdmode)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValue("B")
                    .HasColumnName("UsrGTDMode");
                entity.Property(e => e.UsrLastUpdatedDate).HasColumnType("datetime");
                entity.Property(e => e.UsrLicenseNo).HasMaxLength(30);
                entity.Property(e => e.UsrMobile).HasMaxLength(15);
                entity.Property(e => e.UsrName).HasMaxLength(255);
                entity.Property(e => e.UsrNicno)
                    .HasMaxLength(30)
                    .HasColumnName("UsrNICNo");
                entity.Property(e => e.UsrNotifierId).HasColumnName("UsrNotifierID");
                entity.Property(e => e.UsrPassNo).HasMaxLength(30);
                entity.Property(e => e.UsrPhone).HasMaxLength(15);
                entity.Property(e => e.UsrQualify)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.UsrRace)
                    .HasMaxLength(50)
                    .HasComment("M - Male; F - Female; ");
                entity.Property(e => e.UsrRegisterDate).HasColumnType("datetime");
                entity.Property(e => e.UsrResignDate).HasColumnType("datetime");
                entity.Property(e => e.UsrStatus)
                    .HasMaxLength(1)
                    .HasComment("A - Active; S - Suspend; C - Close; ");
                entity.Property(e => e.UsrSuperiorId).HasColumnName("UsrSuperiorID");
                entity.Property(e => e.UsrTdrdate)
                    .HasColumnType("datetime")
                    .HasColumnName("UsrTDRDate");
                entity.Property(e => e.WithoutClntList)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");
            });

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
