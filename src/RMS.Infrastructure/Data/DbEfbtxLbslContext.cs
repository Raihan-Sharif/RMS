using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RMS.Domain.Entities;

namespace RMS.Infrastructure.Data;

public partial class DbEfbtxLbslContext : DbContext
{
    public DbEfbtxLbslContext()
    {
    }

    public DbEfbtxLbslContext(DbContextOptions<DbEfbtxLbslContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UsrInfo> UsrInfos { get; set; }
    public virtual DbSet<UsrLogin> UsrLogins { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Remove hardcoded connection string - it should come from DI
        if (!optionsBuilder.IsConfigured)
        {
            // This will only be used if no connection string is provided via DI
            optionsBuilder.UseSqlServer("Server=192.168.100.48;Database=DB_EFBTX_LBSL;User Id=sa;Password=Excel@123force;TrustServerCertificate=true;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<UsrLogin>(entity =>
        {
            entity.HasKey(e => e.UsrId).HasName("PK_UsrLogin_UsrID");

            entity.ToTable("UsrLogin");

            entity.Property(e => e.UsrId)
                .HasMaxLength(50)
                .HasColumnName("UsrID");
            entity.Property(e => e.UsrActiveTime).HasColumnType("datetime");
            entity.Property(e => e.UsrActvCode)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UsrActvnCode).HasMaxLength(255);
            entity.Property(e => e.UsrDisableWrngDate).HasColumnType("datetime");
            entity.Property(e => e.UsrLastLoginDate)
                .HasDefaultValueSql("('')")
                .HasColumnType("datetime");
            entity.Property(e => e.UsrLastUpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UsrLogin1).HasColumnName("UsrLogin");
            entity.Property(e => e.UsrOtpcode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UsrOTPCode");
            entity.Property(e => e.UsrOtpexpiration)
                .HasColumnType("datetime")
                .HasColumnName("UsrOTPExpiration");
            entity.Property(e => e.UsrOtpresendAtt).HasColumnName("UsrOTPResendAtt");
            entity.Property(e => e.UsrOtpvldtAtt).HasColumnName("UsrOTPVldtAtt");
            entity.Property(e => e.UsrPwd)
                .HasMaxLength(100)
                .HasDefaultValue("");
            entity.Property(e => e.UsrPwd1).HasMaxLength(100);
            entity.Property(e => e.UsrPwdLastChgDate).HasColumnType("datetime");
            entity.Property(e => e.UsrPwdReset).HasDefaultValue(false);
            entity.Property(e => e.UsrPwdUnscsAtmpt).HasDefaultValue(0);
            entity.Property(e => e.UsrSecretAns1).HasMaxLength(255);
            entity.Property(e => e.UsrSecretAns2).HasMaxLength(255);
            entity.Property(e => e.UsrSecretAns3).HasMaxLength(255);
            entity.Property(e => e.UsrTrdgPin).HasMaxLength(30);
            entity.Property(e => e.UsrTrdgPinDisableWrngDate).HasColumnType("datetime");
            entity.Property(e => e.UsrTrdgPinLastChgDate).HasColumnType("datetime");
            entity.Property(e => e.UsrTrdgPinStat)
                .HasMaxLength(1)
                .HasComment("Y - Created; N - Not Created; R - Reseted; ");
            entity.Property(e => e.UsrTwoFactorAuth).HasDefaultValue(1);
            entity.Property(e => e.UsrTwoFactorAuthBypassExpiryDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

