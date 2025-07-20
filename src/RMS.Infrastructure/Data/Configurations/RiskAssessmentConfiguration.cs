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
    public class RiskAssessmentConfiguration : IEntityTypeConfiguration<RiskAssessment>
    {
        public void Configure(EntityTypeBuilder<RiskAssessment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RiskTitle)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.RiskCategory)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.RiskLevel)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.MitigationStrategy)
                .HasMaxLength(2000);

            builder.Property(x => x.AssignedTo)
                .HasMaxLength(100);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}
