using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Infrastructure.Data.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.TableName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.IpAddress)
                .HasMaxLength(50);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);

            builder.Property(x => x.Module)
                .HasMaxLength(100);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.TableName);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
