using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Construction.Models;

namespace B2bOrder.Resources.Construction
{
    public class ProjectExecutionDbContext : DbContext
    {
        public ProjectExecutionDbContext(DbContextOptions<ProjectExecutionDbContext> options) : base(options)
        {
        }

        #region DbSet Configurations - 資料集設定

        public DbSet<PexDailySiteLog> PexDailySiteLogs { get; set; }
        public DbSet<PexDailyLaborEntry> PexDailyLaborEntries { get; set; }
        public DbSet<PexDefectPunchList> PexDefectPunchLists { get; set; }
        public DbSet<PexMaterialMachineryInspection> PexMaterialMachineryInspections { get; set; }
        public DbSet<PexRequestForInformation> PexRequestForInformations { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Index Configurations - 唯一索引設定

            // 01. pex_daily_site_log
            modelBuilder.Entity<PexDailySiteLog>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<PexDailySiteLog>()
                .HasIndex(e => new { e.ProjectSid, e.LogDate })
                .IsUnique()
                .HasDatabaseName("uk_pdsl_project_date");

            // 02. pex_daily_labor_entry
            modelBuilder.Entity<PexDailyLaborEntry>()
                .HasIndex(e => e.Sid).IsUnique();

            // 03. pex_defect_punch_list
            modelBuilder.Entity<PexDefectPunchList>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<PexDefectPunchList>()
                .HasIndex(e => new { e.ProjectSid, e.DefectNo })
                .IsUnique()
                .HasDatabaseName("uk_pdpl_project_number");

            // 04. pex_material_machinery_inspection
            modelBuilder.Entity<PexMaterialMachineryInspection>()
                .HasIndex(e => e.Sid).IsUnique();

            // 05. pex_request_for_information
            modelBuilder.Entity<PexRequestForInformation>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<PexRequestForInformation>()
                .HasIndex(e => new { e.ProjectSid, e.RfiNumber })
                .IsUnique()
                .HasDatabaseName("uk_prfi_project_number");

            #endregion

            #region Composite Index Configurations - 複合索引與一般索引設定

            // 01. pex_daily_site_log
            modelBuilder.Entity<PexDailySiteLog>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<PexDailySiteLog>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<PexDailySiteLog>()
                .HasIndex(e => e.LogDate);
            modelBuilder.Entity<PexDailySiteLog>()
                .HasIndex(e => e.Avalible);

            // 02. pex_daily_labor_entry
            modelBuilder.Entity<PexDailyLaborEntry>()
                .HasIndex(e => e.DailyLogSid);
            modelBuilder.Entity<PexDailyLaborEntry>()
                .HasIndex(e => e.VendorSid);
            modelBuilder.Entity<PexDailyLaborEntry>()
                .HasIndex(e => e.WbsSid);

            // 03. pex_defect_punch_list
            modelBuilder.Entity<PexDefectPunchList>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<PexDefectPunchList>()
                .HasIndex(e => e.ContractorVendorSid);
            modelBuilder.Entity<PexDefectPunchList>()
                .HasIndex(e => e.RectificationStatus);
            modelBuilder.Entity<PexDefectPunchList>()
                .HasIndex(e => e.Avalible);

            // 04. pex_material_machinery_inspection
            modelBuilder.Entity<PexMaterialMachineryInspection>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<PexMaterialMachineryInspection>()
                .HasIndex(e => e.VendorSid);
            modelBuilder.Entity<PexMaterialMachineryInspection>()
                .HasIndex(e => e.EntryType);
            modelBuilder.Entity<PexMaterialMachineryInspection>()
                .HasIndex(e => e.Avalible);

            // 05. pex_request_for_information
            modelBuilder.Entity<PexRequestForInformation>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<PexRequestForInformation>()
                .HasIndex(e => e.RfiStatus);
            modelBuilder.Entity<PexRequestForInformation>()
                .HasIndex(e => e.Avalible);

            #endregion

            #region Default Value Configurations - 預設值設定

            // 01. pex_daily_site_log
            modelBuilder.Entity<PexDailySiteLog>()
                .Property(e => e.TotalWorkersCount).HasDefaultValue(0);
            modelBuilder.Entity<PexDailySiteLog>()
                .Property(e => e.TotalMachineryCount).HasDefaultValue(0);
            modelBuilder.Entity<PexDailySiteLog>()
                .Property(e => e.LogStatus).HasDefaultValue("DRAFT");
            modelBuilder.Entity<PexDailySiteLog>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<PexDailySiteLog>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 02. pex_daily_labor_entry
            modelBuilder.Entity<PexDailyLaborEntry>()
                .Property(e => e.WorkerCount).HasDefaultValue(0);

            // 03. pex_defect_punch_list
            modelBuilder.Entity<PexDefectPunchList>()
                .Property(e => e.DefectCategory).HasDefaultValue("SAFETY");
            modelBuilder.Entity<PexDefectPunchList>()
                .Property(e => e.SeverityLevel).HasDefaultValue("MEDIUM");
            modelBuilder.Entity<PexDefectPunchList>()
                .Property(e => e.RectificationStatus).HasDefaultValue("OPEN");
            modelBuilder.Entity<PexDefectPunchList>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<PexDefectPunchList>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 04. pex_material_machinery_inspection
            modelBuilder.Entity<PexMaterialMachineryInspection>()
                .Property(e => e.EntryType).HasDefaultValue("MATERIAL");
            modelBuilder.Entity<PexMaterialMachineryInspection>()
                .Property(e => e.Quantity).HasDefaultValue(0.0000m);
            modelBuilder.Entity<PexMaterialMachineryInspection>()
                .Property(e => e.InspectionResult).HasDefaultValue("PASS");
            modelBuilder.Entity<PexMaterialMachineryInspection>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<PexMaterialMachineryInspection>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 05. pex_request_for_information
            modelBuilder.Entity<PexRequestForInformation>()
                .Property(e => e.IsCostImpact).HasDefaultValue("N");
            modelBuilder.Entity<PexRequestForInformation>()
                .Property(e => e.IsScheduleImpact).HasDefaultValue("N");
            modelBuilder.Entity<PexRequestForInformation>()
                .Property(e => e.RfiStatus).HasDefaultValue("SUBMITTED");
            modelBuilder.Entity<PexRequestForInformation>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<PexRequestForInformation>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            #endregion
        }
    }
}