using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.ShareCore.Models;

namespace B2bOrder.Resources.ShareCore
{
    public class SearchDbContext : DbContext
    {
        public SearchDbContext(DbContextOptions<SearchDbContext> options) : base(options) { }

        #region DbSets
        public DbSet<SchIndexRegistry> SchIndexRegistries { get; set; } = null!;
        public DbSet<SchSynonymGroup> SchSynonymGroups { get; set; } = null!;
        public DbSet<SchCustomDictionary> SchCustomDictionaries { get; set; } = null!;
        public DbSet<SchHotKeyword> SchHotKeywords { get; set; } = null!;
        public DbSet<SchSearchLog> SchSearchLogs { get; set; } = null!;
        public DbSet<SchClickTracking> SchClickTrackings { get; set; } = null!;
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region 01. 搜尋索引與同步狀態管理 (Index Task & Sync Registry)
            modelBuilder.Entity<SchIndexRegistry>(entity =>
            {
                entity.ToTable("sch_index_registry");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.IndexName, e.EntityType, e.EntitySid }).HasDatabaseName("uk_sir_entity").IsUnique();

                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_sir_company_sid");
                entity.HasIndex(e => e.SyncStatus).HasDatabaseName("idx_sir_status");
                entity.HasIndex(e => new { e.EntityType, e.EntitySid }).HasDatabaseName("idx_sir_entity_lookup");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_sir_avalible");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.IndexName).HasColumnName("index_name").HasMaxLength(100).IsRequired();
                entity.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(50).IsRequired();
                entity.Property(e => e.EntitySid).HasColumnName("entity_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ActionType).HasColumnName("action_type").HasMaxLength(20).HasDefaultValue("UPSERT");
                entity.Property(e => e.SyncStatus).HasColumnName("sync_status").HasMaxLength(20).HasDefaultValue("PENDING");
                entity.Property(e => e.RetryCount).HasColumnName("retry_count").HasDefaultValue(0);
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
                entity.Property(e => e.LastSyncedAt).HasColumnName("last_synced_at");
                entity.Property(e => e.VersionNo).HasColumnName("version_no").HasDefaultValue(0UL);
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });
            #endregion

            #region 02. 同義詞與搜尋字典 (Synonyms & Dictionaries)
            modelBuilder.Entity<SchSynonymGroup>(entity =>
            {
                entity.ToTable("sch_synonym_group");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_ssg_company_sid");
                entity.HasIndex(e => e.SynonymStatus).HasDatabaseName("idx_ssg_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_ssg_avalible");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.GroupName).HasColumnName("group_name").HasMaxLength(100).IsRequired();
                entity.Property(e => e.SynonymType).HasColumnName("synonym_type").HasMaxLength(20).HasDefaultValue("EQUIVALENT");
                entity.Property(e => e.SynonymWords).HasColumnName("synonym_words").IsRequired();
                entity.Property(e => e.SynonymStatus).HasColumnName("synonym_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.VersionNo).HasColumnName("version_no").HasDefaultValue(0UL);
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });

            modelBuilder.Entity<SchCustomDictionary>(entity =>
            {
                entity.ToTable("sch_custom_dictionary");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.CompanySid, e.Word, e.WordType }).HasDatabaseName("uk_scd_company_word").IsUnique();

                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_scd_company_sid");
                entity.HasIndex(e => e.WordType).HasDatabaseName("idx_scd_word_type");
                entity.HasIndex(e => e.DictStatus).HasDatabaseName("idx_scd_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_scd_avalible");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.Word).HasColumnName("word").HasMaxLength(100).IsRequired();
                entity.Property(e => e.WordType).HasColumnName("word_type").HasMaxLength(30).HasDefaultValue("CUSTOM_WORD");
                entity.Property(e => e.Frequency).HasColumnName("frequency").HasDefaultValue(1000);
                entity.Property(e => e.DictStatus).HasColumnName("dict_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });
            #endregion

            #region 03. 搜尋熱詞與建議詞 (Hot Keywords & Auto-Complete)
            modelBuilder.Entity<SchHotKeyword>(entity =>
            {
                entity.ToTable("sch_hot_keyword");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.CompanySid, e.Keyword }).HasDatabaseName("uk_shk_company_keyword").IsUnique();

                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_shk_company_sid");
                entity.HasIndex(e => new { e.PriorityScore, e.SearchCount }).HasDatabaseName("idx_shk_score");
                entity.HasIndex(e => e.KeywordStatus).HasDatabaseName("idx_shk_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_shk_avalible");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.Keyword).HasColumnName("keyword").HasMaxLength(150).IsRequired();
                entity.Property(e => e.DisplayTitle).HasColumnName("display_title").HasMaxLength(150);
                entity.Property(e => e.SearchCount).HasColumnName("search_count").HasDefaultValue(0);
                entity.Property(e => e.PriorityScore).HasColumnName("priority_score").HasDefaultValue(0);
                entity.Property(e => e.IsPinned).HasColumnName("is_pinned").HasMaxLength(2).HasDefaultValue("N");
                entity.Property(e => e.KeywordStatus).HasColumnName("keyword_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.VersionNo).HasColumnName("version_no").HasDefaultValue(0UL);
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });
            #endregion

            #region 04. 搜尋軌跡與數據分析 (Search Analytics & Click Tracking)
            modelBuilder.Entity<SchSearchLog>(entity =>
            {
                entity.ToTable("sch_search_log");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_ssl_company_sid");
                entity.HasIndex(e => e.SearchKeyword).HasDatabaseName("idx_ssl_keyword");
                entity.HasIndex(e => e.CustomerSid).HasDatabaseName("idx_ssl_customer_sid");
                entity.HasIndex(e => e.IsZeroResult).HasDatabaseName("idx_ssl_zero_result");
                entity.HasIndex(e => e.CreateDate).HasDatabaseName("idx_ssl_create_date");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CustomerSid).HasColumnName("customer_sid").HasMaxLength(32);
                entity.Property(e => e.SessionId).HasColumnName("session_id").HasMaxLength(100);
                entity.Property(e => e.SearchKeyword).HasColumnName("search_keyword").HasMaxLength(200).IsRequired();
                entity.Property(e => e.AppliedFiltersJson).HasColumnName("applied_filters_json").HasColumnType("json");
                entity.Property(e => e.ResultCount).HasColumnName("result_count").HasDefaultValue(0);
                entity.Property(e => e.IsZeroResult).HasColumnName("is_zero_result").HasMaxLength(2).HasDefaultValue("N");
                entity.Property(e => e.ClientIp).HasColumnName("client_ip").HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(500);
                entity.Property(e => e.Remark).HasColumnName("remark");
            });

            modelBuilder.Entity<SchClickTracking>(entity =>
            {
                entity.ToTable("sch_click_tracking");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.HasIndex(e => e.SearchLogNid).HasDatabaseName("idx_sct_search_log_nid");
                entity.HasIndex(e => new { e.EntityType, e.EntitySid }).HasDatabaseName("idx_sct_entity");
                entity.HasIndex(e => e.ClickPosition).HasDatabaseName("idx_sct_position");
                entity.HasIndex(e => e.ConvertedToOrder).HasDatabaseName("idx_sct_conversion");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.SearchLogNid).HasColumnName("search_log_nid").IsRequired();
                entity.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(50).HasDefaultValue("PRODUCT");
                entity.Property(e => e.EntitySid).HasColumnName("entity_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ClickPosition).HasColumnName("click_position").IsRequired();
                entity.Property(e => e.ConvertedToCart).HasColumnName("converted_to_cart").HasMaxLength(2).HasDefaultValue("N");
                entity.Property(e => e.ConvertedToOrder).HasColumnName("converted_to_order").HasMaxLength(2).HasDefaultValue("N");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasOne(d => d.SearchLog)
                    .WithMany(p => p.ClickTrackings)
                    .HasForeignKey(d => d.SearchLogNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sct_search_log");
            });
            #endregion
        }
    }
}