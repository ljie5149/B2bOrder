using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.eCommerce.Models;

namespace B2bOrder.Resources.eCommerce
{
    public class MarketingDbContext : DbContext
    {
        public MarketingDbContext(DbContextOptions<MarketingDbContext> options) : base(options) { }

        public DbSet<MktCampaign> MktCampaigns { get; set; }
        public DbSet<MktChannel> MktChannels { get; set; }
        public DbSet<MktCampaignChannel> MktCampaignChannels { get; set; }
        public DbSet<MktPromotion> MktPromotions { get; set; }
        public DbSet<MktCoupon> MktCoupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 唯一索引 (Unique Indexes)
            modelBuilder.Entity<MktCampaign>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<MktCampaign>().HasIndex(e => e.CampaignNo).IsUnique().HasDatabaseName("uk_mc_campaign_no");

            modelBuilder.Entity<MktChannel>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<MktChannel>().HasIndex(e => e.ChannelCode).IsUnique().HasDatabaseName("uk_mch_channel_code");

            modelBuilder.Entity<MktCampaignChannel>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<MktCampaignChannel>().HasIndex(e => new { e.CampaignSid, e.ChannelSid }).IsUnique().HasDatabaseName("uk_mcc_campaign_channel");

            modelBuilder.Entity<MktPromotion>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<MktPromotion>().HasIndex(e => e.PromotionNo).IsUnique().HasDatabaseName("uk_mp_promotion_no");

            modelBuilder.Entity<MktCoupon>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<MktCoupon>().HasIndex(e => e.CouponCode).IsUnique().HasDatabaseName("uk_mcoupon_coupon_code");

            // 欄位預設值設定 (Default Values)[cite: 17]
            modelBuilder.Entity<MktCampaign>().Property(e => e.BudgetAmount).HasDefaultValue(0.0000m);
            modelBuilder.Entity<MktCampaign>().Property(e => e.CampaignStatus).HasDefaultValue("DRAFT");
            modelBuilder.Entity<MktCampaign>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<MktChannel>().Property(e => e.TrackingEnabled).HasDefaultValue(true);
            modelBuilder.Entity<MktChannel>().Property(e => e.ChannelStatus).HasDefaultValue("ACTIVE");
            modelBuilder.Entity<MktChannel>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<MktCampaignChannel>().Property(e => e.PlannedBudget).HasDefaultValue(0.0000m);
            modelBuilder.Entity<MktCampaignChannel>().Property(e => e.ActualSpend).HasDefaultValue(0.0000m);
            modelBuilder.Entity<MktCampaignChannel>().Property(e => e.ChannelStatus).HasDefaultValue("PLANNED");

            modelBuilder.Entity<MktPromotion>().Property(e => e.ApprovalRequired).HasDefaultValue(false);
            modelBuilder.Entity<MktPromotion>().Property(e => e.PromotionStatus).HasDefaultValue("DRAFT");

            modelBuilder.Entity<MktCoupon>().Property(e => e.UsageLimit).HasDefaultValue(1);
            modelBuilder.Entity<MktCoupon>().Property(e => e.UsedCount).HasDefaultValue(0);
            modelBuilder.Entity<MktCoupon>().Property(e => e.CouponStatus).HasDefaultValue("ACTIVE");
        }
    }
}