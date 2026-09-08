using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.System.Models;

namespace B2bOrder.Resources.System
{
    public class ReportDBContext : DbContext
    {
        public ReportDBContext(DbContextOptions<ReportDBContext> options) : base(options)
        {
        }

        #region 01. 報表定義與匯出任務
        public DbSet<RptReportDefinitionModel> RptReportDefinitions { get; set; } = null!;
        public DbSet<RptExportJobModel> RptExportJobs { get; set; } = null!;
        public DbSet<RptScheduleModel> RptSchedules { get; set; } = null!;
        #endregion

        #region 02. 銷售彙總
        public DbSet<RptSalesDailyModel> RptSalesDailies { get; set; } = null!;
        public DbSet<RptProductSalesDailyModel> RptProductSalesDailies { get; set; } = null!;
        #endregion

        #region 03. 會員彙總
        public DbSet<RptMemberDailyModel> RptMemberDailies { get; set; } = null!;
        #endregion

        #region 04. 庫存彙總
        public DbSet<RptInventoryDailyModel> RptInventoryDailies { get; set; } = null!;
        public DbSet<RptInventoryAlertModel> RptInventoryAlerts { get; set; } = null!;
        #endregion

        #region 05. 採購與供應商績效
        public DbSet<RptPurchaseDailyModel> RptPurchaseDailies { get; set; } = null!;
        public DbSet<RptSupplierPerformanceModel> RptSupplierPerformances { get; set; } = null!;
        #endregion

        #region 06. 財務與商店結算彙總
        public DbSet<RptFinanceDailyModel> RptFinanceDailies { get; set; } = null!;
        public DbSet<RptStoreSettlementPeriodModel> RptStoreSettlementPeriods { get; set; } = null!;
        #endregion

        #region 07. 彙總工作紀錄
        public DbSet<RptBuildJobModel> RptBuildJobs { get; set; } = null!;
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 01. 報表定義與匯出任務
            modelBuilder.Entity<RptReportDefinitionModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ReportCode).IsUnique();
                entity.HasIndex(e => e.ReportCategory);
                entity.HasIndex(e => e.SourceServiceCode);
                entity.HasIndex(e => e.Avalible);
            });

            modelBuilder.Entity<RptExportJobModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.JobNo).IsUnique();
                entity.HasIndex(e => e.ReportDefinitionNid);
                entity.HasIndex(e => e.RequesterSid);
                entity.HasIndex(e => e.JobStatus);
                entity.HasIndex(e => e.CreateDate);
                entity.HasIndex(e => e.ExpiryDate);
            });

            modelBuilder.Entity<RptScheduleModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ReportDefinitionNid);
                entity.HasIndex(e => e.ScheduleStatus);
                entity.HasIndex(e => e.NextRunDate);
                entity.HasIndex(e => e.OwnerSid);
                entity.HasIndex(e => e.Avalible);
            });

            // 02. 銷售彙總
            modelBuilder.Entity<RptSalesDailyModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StatisticsDate, e.StoreSid, e.CurrencySid }).IsUnique();
                entity.HasIndex(e => e.StatisticsDate);
                entity.HasIndex(e => e.StoreSid);
                entity.HasIndex(e => e.CurrencySid);
            });

            modelBuilder.Entity<RptProductSalesDailyModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StatisticsDate, e.StoreSid, e.SkuSid }).IsUnique();
                entity.HasIndex(e => e.StatisticsDate);
                entity.HasIndex(e => e.StoreSid);
                entity.HasIndex(e => e.ProductSid);
                entity.HasIndex(e => e.SkuSid);
                entity.HasIndex(e => e.CategorySid);
                entity.HasIndex(e => e.BrandSid);
            });

            // 03. 會員彙總
            modelBuilder.Entity<RptMemberDailyModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StatisticsDate, e.PriceGroupSid }).IsUnique();
                entity.HasIndex(e => e.StatisticsDate);
                entity.HasIndex(e => e.PriceGroupSid);
            });

            // 04. 庫存彙總
            modelBuilder.Entity<RptInventoryDailyModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StatisticsDate, e.WarehouseSid, e.SkuSid }).IsUnique();
                entity.HasIndex(e => e.StatisticsDate);
                entity.HasIndex(e => e.WarehouseSid);
                entity.HasIndex(e => e.ProductSid);
                entity.HasIndex(e => e.SkuSid);
                entity.HasIndex(e => e.AvailableQty);
                entity.HasIndex(e => e.NoMovementDays);
            });

            modelBuilder.Entity<RptInventoryAlertModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.AlertDate, e.WarehouseSid, e.SkuSid, e.AlertType }).IsUnique();
                entity.HasIndex(e => e.AlertDate);
                entity.HasIndex(e => e.WarehouseSid);
                entity.HasIndex(e => e.ProductSid);
                entity.HasIndex(e => e.SkuSid);
                entity.HasIndex(e => e.AlertType);
                entity.HasIndex(e => e.AlertLevel);
                entity.HasIndex(e => e.AlertStatus);
            });

            // 05. 採購與供應商績效
            modelBuilder.Entity<RptPurchaseDailyModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StatisticsDate, e.SupplierSid, e.CurrencySid }).IsUnique();
                entity.HasIndex(e => e.StatisticsDate);
                entity.HasIndex(e => e.SupplierSid);
                entity.HasIndex(e => e.CurrencySid);
            });

            modelBuilder.Entity<RptSupplierPerformanceModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.PeriodType, e.PeriodCode, e.SupplierSid }).IsUnique();
                entity.HasIndex(e => new { e.PeriodType, e.PeriodCode });
                entity.HasIndex(e => e.SupplierSid);
                entity.HasIndex(e => e.PerformanceScore);
            });

            // 06. 財務與商店結算彙總
            modelBuilder.Entity<RptFinanceDailyModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StatisticsDate, e.CurrencySid }).IsUnique();
                entity.HasIndex(e => e.StatisticsDate);
                entity.HasIndex(e => e.CurrencySid);
            });

            modelBuilder.Entity<RptStoreSettlementPeriodModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.PeriodStartDate, e.PeriodEndDate, e.StoreSid, e.CurrencySid }).IsUnique();
                entity.HasIndex(e => new { e.PeriodStartDate, e.PeriodEndDate });
                entity.HasIndex(e => e.StoreSid);
                entity.HasIndex(e => e.CurrencySid);
            });

            // 07. 彙總工作紀錄
            modelBuilder.Entity<RptBuildJobModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.JobNo).IsUnique();
                entity.HasIndex(e => e.JobType);
                entity.HasIndex(e => e.StatisticsDate);
                entity.HasIndex(e => new { e.PeriodStartDate, e.PeriodEndDate });
                entity.HasIndex(e => e.JobStatus);
                entity.HasIndex(e => e.StartDate);
            });
        }
    }
}