using B2bOrder.Models.Config;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Data
{
    public class PIMDbContext : DbContext
    {
        public PIMDbContext(DbContextOptions<PIMDbContext> options) : base(options) { }

        // 01. 分散式系統組態
        public DbSet<CfgSystemConfig> SystemConfigs { get; set; } = null!;

        // 02. 功能開關與灰度發布
        public DbSet<CfgFeatureFlag> FeatureFlags { get; set; } = null!;

        // 03. 通用字典與代碼對照
        public DbSet<CfgDictionaryCategory> DictionaryCategories { get; set; } = null!;
        public DbSet<CfgDictionaryItem> DictionaryItems { get; set; } = null!;

        // 04. 組態異動歷程與快照
        public DbSet<CfgChangeHistory> ChangeHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 表名與 Entity 對應 Mapping
            modelBuilder.Entity<CfgSystemConfig>(entity =>
            {
                entity.ToTable("cfg_system_config");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.CompanySid, e.EnvCode, e.ServiceName, e.ConfigKey })
                      .IsUnique()
                      .HasDatabaseName("uk_csc_env_service_key");
                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_csc_company_sid");
                entity.HasIndex(e => new { e.EnvCode, e.ServiceName, e.ConfigGroup }).HasDatabaseName("idx_csc_lookup");
                entity.HasIndex(e => e.ConfigStatus).HasDatabaseName("idx_csc_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_csc_avalible");
            });

            modelBuilder.Entity<CfgFeatureFlag>(entity =>
            {
                entity.ToTable("cfg_feature_flag");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.CompanySid, e.FeatureCode })
                      .IsUnique()
                      .HasDatabaseName("uk_cff_company_feature");
                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_cff_company_sid");
                entity.HasIndex(e => e.IsEnabled).HasDatabaseName("idx_cff_enabled");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_cff_avalible");
            });

            modelBuilder.Entity<CfgDictionaryCategory>(entity =>
            {
                entity.ToTable("cfg_dictionary_category");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.CompanySid, e.CategoryCode })
                      .IsUnique()
                      .HasDatabaseName("uk_cdc_company_category");
                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_cdc_company_sid");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_cdc_avalible");
            });

            modelBuilder.Entity<CfgDictionaryItem>(entity =>
            {
                entity.ToTable("cfg_dictionary_item");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.CategorySid, e.ItemCode, e.ItemLocale })
                      .IsUnique()
                      .HasDatabaseName("uk_cdi_category_code_locale");
                entity.HasIndex(e => e.CategorySid).HasDatabaseName("idx_cdi_category_sid");
                entity.HasIndex(e => e.SortOrder).HasDatabaseName("idx_cdi_sort");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_cdi_avalible");
            });

            modelBuilder.Entity<CfgChangeHistory>(entity =>
            {
                entity.ToTable("cfg_change_history");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_cch_company_sid");
                entity.HasIndex(e => new { e.ConfigType, e.TargetSid }).HasDatabaseName("idx_cch_target");
                entity.HasIndex(e => e.OperatorUserSid).HasDatabaseName("idx_cch_operator");
                entity.HasIndex(e => e.CreateDate).HasDatabaseName("idx_cch_create_date");
            });
        }
    }
}