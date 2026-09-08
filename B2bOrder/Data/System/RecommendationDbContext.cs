using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.System.Models;

namespace B2bOrder.Resources.System
{
    public class RecommendationDbContext : DbContext
    {
        public RecommendationDbContext(DbContextOptions<RecommendationDbContext> options)
            : base(options)
        {
        }

        #region 會員推薦偏好與特徵檔 (Customer Profile & Preferences)
        public DbSet<RecCustomerPreferenceModel> RecCustomerPreferences { get; set; } = null!;
        #endregion

        #region 商品關聯與相似度矩陣檔 (Item-to-Item Similarity)
        public DbSet<RecItemSimilarityModel> RecItemSimilarities { get; set; } = null!;
        #endregion

        #region 個人化推薦結果與快取檔 (Personalized Recommendations)
        public DbSet<RecPersonalRecommendationModel> RecPersonalRecommendations { get; set; } = null!;
        #endregion

        #region 推薦系統人工干預與調控規則檔 (Boost, Bury & Block Rules)
        public DbSet<RecRuleSettingModel> RecRuleSettings { get; set; } = null!;
        #endregion

        #region 推薦版位曝光、點擊與轉換歷程檔 (Recommendation Analytics)
        public DbSet<RecImpressionLogModel> RecImpressionLogs { get; set; } = null!;
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region 01. 會員推薦偏好與特徵檔設定 (Customer Preference Configuration)
            modelBuilder.Entity<RecCustomerPreferenceModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.CompanySid);
                entity.HasIndex(e => e.CustomerSid);
                entity.HasIndex(e => e.PriceSensitivityLevel);
                entity.HasIndex(e => e.Avalible);
                entity.HasAlternateKey(e => new { e.CompanySid, e.CustomerSid });
            });
            #endregion

            #region 02. 商品關聯與相似度矩陣檔設定 (Item Similarity Configuration)
            modelBuilder.Entity<RecItemSimilarityModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.CompanySid);
                entity.HasIndex(e => e.SourceItemSid);
                entity.HasIndex(e => e.TargetItemSid);
                entity.HasIndex(e => new { e.RelationType, e.SimilarityScore })
                      .IsDescending(false, true);
                entity.HasIndex(e => e.Avalible);
                entity.HasAlternateKey(e => new { e.CompanySid, e.SourceItemSid, e.TargetItemSid, e.RelationType });
            });
            #endregion

            #region 03. 個人化推薦結果與快取檔設定 (Personal Recommendation Configuration)
            modelBuilder.Entity<RecPersonalRecommendationModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.CompanySid);
                entity.HasIndex(e => new { e.CustomerSid, e.SceneCode, e.DisplayRank });
                entity.HasIndex(e => e.ExpiredAt);
                entity.HasIndex(e => e.Avalible);
                entity.HasAlternateKey(e => new { e.CompanySid, e.CustomerSid, e.SceneCode, e.RecommendedItemSid });
            });
            #endregion

            #region 04. 推薦系統人工干預與調控規則檔設定 (Rule Setting Configuration)
            modelBuilder.Entity<RecRuleSettingModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.CompanySid);
                entity.HasIndex(e => e.SceneCode);
                entity.HasIndex(e => e.ActionType);
                entity.HasIndex(e => e.RuleStatus);
                entity.HasIndex(e => e.Avalible);
            });
            #endregion

            #region 05. 推薦版位曝光、點擊與轉換歷程檔設定 (Impression Log Configuration)
            modelBuilder.Entity<RecImpressionLogModel>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.CompanySid);
                entity.HasIndex(e => new { e.SceneCode, e.RecommendedItemSid });
                entity.HasIndex(e => e.CustomerSid);
                entity.HasIndex(e => e.IsClicked);
                entity.HasIndex(e => e.ConvertedToOrder);
                entity.HasIndex(e => e.CreateDate);
            });
            #endregion
        }
    }
}