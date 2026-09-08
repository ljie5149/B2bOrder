using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Construction.Models;

namespace B2bOrder.Resources.Construction
{
    public class EquipmentDbContext : DbContext
    {
        public EquipmentDbContext(DbContextOptions<EquipmentDbContext> options) : base(options)
        {
        }

        #region DbSet Configurations - 資料集設定

        public DbSet<EqpEquipmentMaster> EqpEquipmentMasters { get; set; }
        public DbSet<EqpDispatchLog> EqpDispatchLogs { get; set; }
        public DbSet<EqpMaintenanceLog> EqpMaintenanceLogs { get; set; }
        public DbSet<EqpSafetyCertification> EqpSafetyCertifications { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Index Configurations - 唯一索引設定

            // 01. eqp_equipment_master
            modelBuilder.Entity<EqpEquipmentMaster>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<EqpEquipmentMaster>()
                .HasIndex(e => e.EquipmentCode).IsUnique();

            // 02. eqp_dispatch_log
            modelBuilder.Entity<EqpDispatchLog>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<EqpDispatchLog>()
                .HasIndex(e => e.DispatchNo)
                .IsUnique()
                .HasDatabaseName("uk_edl_dispatch_no");

            // 03. eqp_maintenance_log
            modelBuilder.Entity<EqpMaintenanceLog>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<EqpMaintenanceLog>()
                .HasIndex(e => e.MaintenanceNo)
                .IsUnique()
                .HasDatabaseName("uk_eml_maintenance_no");

            // 04. eqp_safety_certification
            modelBuilder.Entity<EqpSafetyCertification>()
                .HasIndex(e => e.Sid).IsUnique();

            #endregion

            #region Composite Index Configurations - 複合索引與一般索引設定

            // 01. eqp_equipment_master
            modelBuilder.Entity<EqpEquipmentMaster>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<EqpEquipmentMaster>()
                .HasIndex(e => e.Category);
            modelBuilder.Entity<EqpEquipmentMaster>()
                .HasIndex(e => e.OwnershipType);
            modelBuilder.Entity<EqpEquipmentMaster>()
                .HasIndex(e => e.CurrentStatus);
            modelBuilder.Entity<EqpEquipmentMaster>()
                .HasIndex(e => e.CurrentProjectSid);
            modelBuilder.Entity<EqpEquipmentMaster>()
                .HasIndex(e => e.Avalible);

            // 02. eqp_dispatch_log
            modelBuilder.Entity<EqpDispatchLog>()
                .HasIndex(e => e.EquipmentSid);
            modelBuilder.Entity<EqpDispatchLog>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<EqpDispatchLog>()
                .HasIndex(e => e.DispatchStatus);
            modelBuilder.Entity<EqpDispatchLog>()
                .HasIndex(e => e.Avalible);

            // 03. eqp_maintenance_log
            modelBuilder.Entity<EqpMaintenanceLog>()
                .HasIndex(e => e.EquipmentSid);
            modelBuilder.Entity<EqpMaintenanceLog>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<EqpMaintenanceLog>()
                .HasIndex(e => e.MaintenanceType);
            modelBuilder.Entity<EqpMaintenanceLog>()
                .HasIndex(e => e.MaintenanceStatus);
            modelBuilder.Entity<EqpMaintenanceLog>()
                .HasIndex(e => e.Avalible);

            // 04. eqp_safety_certification
            modelBuilder.Entity<EqpSafetyCertification>()
                .HasIndex(e => e.EquipmentSid);
            modelBuilder.Entity<EqpSafetyCertification>()
                .HasIndex(e => e.ExpirationDate);
            modelBuilder.Entity<EqpSafetyCertification>()
                .HasIndex(e => e.CertStatus);
            modelBuilder.Entity<EqpSafetyCertification>()
                .HasIndex(e => e.Avalible);

            #endregion

            #region Default Value Configurations - 預設值設定

            // 01. eqp_equipment_master
            modelBuilder.Entity<EqpEquipmentMaster>()
                .Property(e => e.OwnershipType).HasDefaultValue("OWNED");
            modelBuilder.Entity<EqpEquipmentMaster>()
                .Property(e => e.DailyRentalRate).HasDefaultValue(0.00m);
            modelBuilder.Entity<EqpEquipmentMaster>()
                .Property(e => e.MonthlyRentalRate).HasDefaultValue(0.00m);
            modelBuilder.Entity<EqpEquipmentMaster>()
                .Property(e => e.CurrentStatus).HasDefaultValue("IDLE");
            modelBuilder.Entity<EqpEquipmentMaster>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<EqpEquipmentMaster>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 02. eqp_dispatch_log
            modelBuilder.Entity<EqpDispatchLog>()
                .Property(e => e.DispatchStatus).HasDefaultValue("SCHEDULED");
            modelBuilder.Entity<EqpDispatchLog>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<EqpDispatchLog>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 03. eqp_maintenance_log
            modelBuilder.Entity<EqpMaintenanceLog>()
                .Property(e => e.MaintenanceType).HasDefaultValue("PREVENTIVE");
            modelBuilder.Entity<EqpMaintenanceLog>()
                .Property(e => e.PartsCost).HasDefaultValue(0.00m);
            modelBuilder.Entity<EqpMaintenanceLog>()
                .Property(e => e.LaborCost).HasDefaultValue(0.00m);
            modelBuilder.Entity<EqpMaintenanceLog>()
                .Property(e => e.TotalCost).HasDefaultValue(0.00m);
            modelBuilder.Entity<EqpMaintenanceLog>()
                .Property(e => e.MaintenanceStatus).HasDefaultValue("IN_PROGRESS");
            modelBuilder.Entity<EqpMaintenanceLog>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<EqpMaintenanceLog>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 04. eqp_safety_certification
            modelBuilder.Entity<EqpSafetyCertification>()
                .Property(e => e.CertStatus).HasDefaultValue("VALID");
            modelBuilder.Entity<EqpSafetyCertification>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<EqpSafetyCertification>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            #endregion
        }
    }
}