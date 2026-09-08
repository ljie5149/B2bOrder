using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Construction.Models;

namespace B2bOrder.Resources.Construction
{
    public class QualitySafetyDbContext : DbContext
    {
        public QualitySafetyDbContext(DbContextOptions<QualitySafetyDbContext> options) : base(options)
        {
        }

        #region DbSet Configurations - 資料集設定

        public DbSet<QsmQualityInspection> QsmQualityInspections { get; set; }
        public DbSet<QsmSafetyPenaltyOrder> QsmSafetyPenaltyOrders { get; set; }
        public DbSet<QsmWorksiteIncidentLog> QsmWorksiteIncidentLogs { get; set; }
        public DbSet<QsmHighRiskWorkPermit> QsmHighRiskWorkPermits { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Index Configurations - 唯一索引設定

            // 01. qsm_quality_inspection
            modelBuilder.Entity<QsmQualityInspection>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<QsmQualityInspection>()
                .HasIndex(e => new { e.ProjectSid, e.InspectionNo })
                .IsUnique()
                .HasDatabaseName("uk_qqi_project_number");

            // 02. qsm_safety_penalty_order
            modelBuilder.Entity<QsmSafetyPenaltyOrder>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<QsmSafetyPenaltyOrder>()
                .HasIndex(e => new { e.ProjectSid, e.PenaltyNo })
                .IsUnique()
                .HasDatabaseName("uk_qspo_project_number");

            // 03. qsm_worksite_incident_log
            modelBuilder.Entity<QsmWorksiteIncidentLog>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<QsmWorksiteIncidentLog>()
                .HasIndex(e => new { e.ProjectSid, e.IncidentNo })
                .IsUnique()
                .HasDatabaseName("uk_qwil_project_number");

            // 04. qsm_high_risk_work_permit
            modelBuilder.Entity<QsmHighRiskWorkPermit>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<QsmHighRiskWorkPermit>()
                .HasIndex(e => new { e.ProjectSid, e.PermitNo })
                .IsUnique()
                .HasDatabaseName("uk_qhrwp_project_number");

            #endregion

            #region Composite Index Configurations - 複合索引與一般索引設定

            // 01. qsm_quality_inspection
            modelBuilder.Entity<QsmQualityInspection>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<QsmQualityInspection>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<QsmQualityInspection>()
                .HasIndex(e => e.VendorSid);
            modelBuilder.Entity<QsmQualityInspection>()
                .HasIndex(e => e.InspectionResult);
            modelBuilder.Entity<QsmQualityInspection>()
                .HasIndex(e => e.Avalible);

            // 02. qsm_safety_penalty_order
            modelBuilder.Entity<QsmSafetyPenaltyOrder>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<QsmSafetyPenaltyOrder>()
                .HasIndex(e => e.VendorSid);
            modelBuilder.Entity<QsmSafetyPenaltyOrder>()
                .HasIndex(e => e.IsDeducted);
            modelBuilder.Entity<QsmSafetyPenaltyOrder>()
                .HasIndex(e => e.Avalible);

            // 03. qsm_worksite_incident_log
            modelBuilder.Entity<QsmWorksiteIncidentLog>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<QsmWorksiteIncidentLog>()
                .HasIndex(e => e.VendorSid);
            modelBuilder.Entity<QsmWorksiteIncidentLog>()
                .HasIndex(e => e.SeverityLevel);
            modelBuilder.Entity<QsmWorksiteIncidentLog>()
                .HasIndex(e => e.Avalible);

            // 04. qsm_high_risk_work_permit
            modelBuilder.Entity<QsmHighRiskWorkPermit>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<QsmHighRiskWorkPermit>()
                .HasIndex(e => e.VendorSid);
            modelBuilder.Entity<QsmHighRiskWorkPermit>()
                .HasIndex(e => e.PermitStatus);
            modelBuilder.Entity<QsmHighRiskWorkPermit>()
                .HasIndex(e => e.Avalible);

            #endregion

            #region Default Value Configurations - 預設值設定

            // 01. qsm_quality_inspection
            modelBuilder.Entity<QsmQualityInspection>()
                .Property(e => e.IsHoldPoint).HasDefaultValue("Y");
            modelBuilder.Entity<QsmQualityInspection>()
                .Property(e => e.InspectionResult).HasDefaultValue("PASS");
            modelBuilder.Entity<QsmQualityInspection>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<QsmQualityInspection>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 02. qsm_safety_penalty_order
            modelBuilder.Entity<QsmSafetyPenaltyOrder>()
                .Property(e => e.DemeritPoints).HasDefaultValue(0);
            modelBuilder.Entity<QsmSafetyPenaltyOrder>()
                .Property(e => e.PenaltyAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<QsmSafetyPenaltyOrder>()
                .Property(e => e.IsDeducted).HasDefaultValue("N");
            modelBuilder.Entity<QsmSafetyPenaltyOrder>()
                .Property(e => e.Status).HasDefaultValue("ISSUED");
            modelBuilder.Entity<QsmSafetyPenaltyOrder>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<QsmSafetyPenaltyOrder>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 03. qsm_worksite_incident_log
            modelBuilder.Entity<QsmWorksiteIncidentLog>()
                .Property(e => e.SeverityLevel).HasDefaultValue("MINOR");
            modelBuilder.Entity<QsmWorksiteIncidentLog>()
                .Property(e => e.InjuredCount).HasDefaultValue(0);
            modelBuilder.Entity<QsmWorksiteIncidentLog>()
                .Property(e => e.IsReportedToGov).HasDefaultValue("N");
            modelBuilder.Entity<QsmWorksiteIncidentLog>()
                .Property(e => e.IncidentStatus).HasDefaultValue("INVESTIGATING");
            modelBuilder.Entity<QsmWorksiteIncidentLog>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<QsmWorksiteIncidentLog>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 04. qsm_high_risk_work_permit
            modelBuilder.Entity<QsmHighRiskWorkPermit>()
                .Property(e => e.SafetyMeasuresChecked).HasDefaultValue("Y");
            modelBuilder.Entity<QsmHighRiskWorkPermit>()
                .Property(e => e.PermitStatus).HasDefaultValue("APPROVED");
            modelBuilder.Entity<QsmHighRiskWorkPermit>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<QsmHighRiskWorkPermit>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            #endregion
        }
    }
}