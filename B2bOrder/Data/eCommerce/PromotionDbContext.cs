using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Resources.eCommerce
{
    /// <summary>
    /// PromotionDB V2 資料庫上下文 (Promotion Database Context)
    /// </summary>
    public class PromotionDbContext : DbContext
    {
        public PromotionDbContext(DbContextOptions<PromotionDbContext> options) : base(options)
        {
        }

        // =========================================================
        // 01. 促銷活動主檔 (Campaign Master & Translation)
        // =========================================================
        public DbSet<CampaignModel> Campaigns { get; set; } = null!;
        public DbSet<CampaignTranslationModel> CampaignTranslations { get; set; } = null!;

        // =========================================================
        // 02. 促銷規則與條件 (Rules, Conditions & Rewards)
        // =========================================================
        public DbSet<RuleModel> Rules { get; set; } = null!;
        public DbSet<ConditionModel> Conditions { get; set; } = null!;
        public DbSet<RewardModel> Rewards { get; set; } = null!;

        // =========================================================
        // 03. 活動適用與排除範圍 (Scopes & Time Schedules)
        // =========================================================
        public DbSet<CampaignScopeModel> CampaignScopes { get; set; } = null!;
        public DbSet<TimeScheduleModel> TimeSchedules { get; set; } = null!;

        // =========================================================
        // 04. 優惠券批次與優惠碼 (Coupon Batches, Coupons & Distribution)
        // =========================================================
        public DbSet<CouponBatchModel> CouponBatches { get; set; } = null!;
        public DbSet<CouponModel> Coupons { get; set; } = null!;
        public DbSet<CouponDistributionModel> CouponDistributions { get; set; } = null!;

        // =========================================================
        // 05. 優惠計算請求與結果 (Calculation Requests & Results)
        // =========================================================
        public DbSet<CalculationRequestModel> CalculationRequests { get; set; } = null!;
        public DbSet<CalculationResultModel> CalculationResults { get; set; } = null!;

        // =========================================================
        // 06. 優惠預留、核銷與撤銷 (Reservations, Redemptions & Reversals)
        // =========================================================
        public DbSet<RedemptionReservationModel> RedemptionReservations { get; set; } = null!;
        public DbSet<RedemptionModel> Redemptions { get; set; } = null!;
        public DbSet<RedemptionReversalModel> RedemptionReversals { get; set; } = null!;

        // =========================================================
        // 07. 贈品與加價購庫存 (Reward Inventory)
        // =========================================================
        public DbSet<RewardInventoryModel> RewardInventories { get; set; } = null!;

        // =========================================================
        // 08. 活動預算與成本 (Budget Ledger)
        // =========================================================
        public DbSet<BudgetLedgerModel> BudgetLedgers { get; set; } = null!;

        // =========================================================
        // 09. 促銷模板與複製 (Templates)
        // =========================================================
        public DbSet<TemplateModel> Templates { get; set; } = null!;

        // =========================================================
        // 10. 活動版本、狀態與事件 (Versions, Status Histories & Events)
        // =========================================================
        public DbSet<CampaignVersionModel> CampaignVersions { get; set; } = null!;
        public DbSet<StatusHistoryModel> StatusHistories { get; set; } = null!;
        public DbSet<EventModel> Events { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapping table names and configurations
            modelBuilder.Entity<CampaignModel>().ToTable("prm_campaign");
            modelBuilder.Entity<CampaignTranslationModel>().ToTable("prm_campaign_translation");
            modelBuilder.Entity<RuleModel>().ToTable("prm_rule");
            modelBuilder.Entity<ConditionModel>().ToTable("prm_condition");
            modelBuilder.Entity<RewardModel>().ToTable("prm_reward");
            modelBuilder.Entity<CampaignScopeModel>().ToTable("prm_campaign_scope");
            modelBuilder.Entity<TimeScheduleModel>().ToTable("prm_time_schedule");
            modelBuilder.Entity<CouponBatchModel>().ToTable("prm_coupon_batch");
            modelBuilder.Entity<CouponModel>().ToTable("prm_coupon");
            modelBuilder.Entity<CouponDistributionModel>().ToTable("prm_coupon_distribution");
            modelBuilder.Entity<CalculationRequestModel>().ToTable("prm_calculation_request");
            modelBuilder.Entity<CalculationResultModel>().ToTable("prm_calculation_result");
            modelBuilder.Entity<RedemptionReservationModel>().ToTable("prm_redemption_reservation");
            modelBuilder.Entity<RedemptionModel>().ToTable("prm_redemption");
            modelBuilder.Entity<RedemptionReversalModel>().ToTable("prm_redemption_reversal");
            modelBuilder.Entity<RewardInventoryModel>().ToTable("prm_reward_inventory");
            modelBuilder.Entity<BudgetLedgerModel>().ToTable("prm_budget_ledger");
            modelBuilder.Entity<TemplateModel>().ToTable("prm_template");
            modelBuilder.Entity<CampaignVersionModel>().ToTable("prm_campaign_version");
            modelBuilder.Entity<StatusHistoryModel>().ToTable("prm_status_history");
            modelBuilder.Entity<EventModel>().ToTable("prm_event");
        }
    }
}