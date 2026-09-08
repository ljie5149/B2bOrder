using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Customer.Models;

namespace B2bOrder.Resources.Customer
{
    public class MarketingCampaignDbContext : DbContext
    {
        public MarketingCampaignDbContext(DbContextOptions<MarketingCampaignDbContext> options) : base(options) { }

        public DbSet<LytTier> LytTiers { get; set; }
        public DbSet<LytCustomerTier> LytCustomerTiers { get; set; }
        public DbSet<LytPointsAccount> LytPointsAccounts { get; set; }
        public DbSet<LytPointsLedger> LytPointsLedgers { get; set; }
        public DbSet<LytCouponTemplate> LytCouponTemplates { get; set; }
        public DbSet<LytCustomerCoupon> LytCustomerCoupons { get; set; }
        public DbSet<LytRewardItem> LytRewardItems { get; set; }
        public DbSet<LytExchangeOrder> LytExchangeOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 唯一索引 (Unique Indexes)
            modelBuilder.Entity<LytTier>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<LytTier>().HasIndex(e => new { e.CompanySid, e.TierCode }).IsUnique().HasDatabaseName("uk_lt_company_tier");

            modelBuilder.Entity<LytCustomerTier>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<LytCustomerTier>().HasIndex(e => e.CustomerSid).IsUnique().HasDatabaseName("uk_lct_customer");

            modelBuilder.Entity<LytPointsAccount>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<LytPointsAccount>().HasIndex(e => new { e.CompanySid, e.CustomerSid }).IsUnique().HasDatabaseName("uk_lpa_company_customer");

            modelBuilder.Entity<LytPointsLedger>().HasIndex(e => e.Sid).IsUnique();

            modelBuilder.Entity<LytCouponTemplate>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<LytCouponTemplate>().HasIndex(e => new { e.CompanySid, e.TemplateCode }).IsUnique().HasDatabaseName("uk_lct_company_code");

            modelBuilder.Entity<LytCustomerCoupon>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<LytCustomerCoupon>().HasIndex(e => e.CouponCode).IsUnique().HasDatabaseName("uk_lcc_coupon_code");

            modelBuilder.Entity<LytRewardItem>().HasIndex(e => e.Sid).IsUnique();

            modelBuilder.Entity<LytExchangeOrder>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<LytExchangeOrder>().HasIndex(e => e.ExchangeNo).IsUnique().HasDatabaseName("uk_leo_exchange_no");

            // 欄位預設值 (Default Values)
            modelBuilder.Entity<LytTier>().Property(e => e.TierLevel).HasDefaultValue(1);
            modelBuilder.Entity<LytTier>().Property(e => e.MinAccumulatedSpend).HasDefaultValue(0.0000m);
            modelBuilder.Entity<LytTier>().Property(e => e.MinAccumulatedPoints).HasDefaultValue(0);
            modelBuilder.Entity<LytTier>().Property(e => e.PointsMultiplier).HasDefaultValue(1.00m);
            modelBuilder.Entity<LytTier>().Property(e => e.TierStatus).HasDefaultValue("ACTIVE");
            modelBuilder.Entity<LytTier>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<LytCustomerTier>().Property(e => e.CurrentPeriodSpend).HasDefaultValue(0.0000m);
            modelBuilder.Entity<LytCustomerTier>().Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<LytCustomerTier>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<LytPointsAccount>().Property(e => e.TotalPoints).HasDefaultValue(0);
            modelBuilder.Entity<LytPointsAccount>().Property(e => e.LockedPoints).HasDefaultValue(0);
            modelBuilder.Entity<LytPointsAccount>().Property(e => e.ExpiredPointsAccum).HasDefaultValue(0);
            modelBuilder.Entity<LytPointsAccount>().Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<LytPointsAccount>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<LytCouponTemplate>().Property(e => e.DiscountType).HasDefaultValue("FIXED_AMOUNT");
            modelBuilder.Entity<LytCouponTemplate>().Property(e => e.DiscountValue).HasDefaultValue(0.0000m);
            modelBuilder.Entity<LytCouponTemplate>().Property(e => e.MinPurchaseAmount).HasDefaultValue(0.0000m);
            modelBuilder.Entity<LytCouponTemplate>().Property(e => e.TotalQuantity).HasDefaultValue(-1);
            modelBuilder.Entity<LytCouponTemplate>().Property(e => e.IssuedQuantity).HasDefaultValue(0);
            modelBuilder.Entity<LytCouponTemplate>().Property(e => e.UsedQuantity).HasDefaultValue(0);
            modelBuilder.Entity<LytCouponTemplate>().Property(e => e.ValidityType).HasDefaultValue("DATE_RANGE");
            modelBuilder.Entity<LytCouponTemplate>().Property(e => e.TemplateStatus).HasDefaultValue("ACTIVE");
            modelBuilder.Entity<LytCouponTemplate>().Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<LytCouponTemplate>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<LytCustomerCoupon>().Property(e => e.CouponStatus).HasDefaultValue("UNUSED");
            modelBuilder.Entity<LytCustomerCoupon>().Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<LytCustomerCoupon>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<LytRewardItem>().Property(e => e.RewardType).HasDefaultValue("COUPON");
            modelBuilder.Entity<LytRewardItem>().Property(e => e.AdditionalAmount).HasDefaultValue(0.0000m);
            modelBuilder.Entity<LytRewardItem>().Property(e => e.StockQty).HasDefaultValue(0);
            modelBuilder.Entity<LytRewardItem>().Property(e => e.ItemStatus).HasDefaultValue("ACTIVE");
            modelBuilder.Entity<LytRewardItem>().Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<LytRewardItem>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<LytExchangeOrder>().Property(e => e.PaidAmount).HasDefaultValue(0.0000m);
            modelBuilder.Entity<LytExchangeOrder>().Property(e => e.ExchangeStatus).HasDefaultValue("COMPLETED");
            modelBuilder.Entity<LytExchangeOrder>().Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<LytExchangeOrder>().Property(e => e.Avalible).HasDefaultValue("Y");
        }
    }
}