using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Data
{
    public class MdmDbContext : DbContext
    {
        public MdmDbContext(DbContextOptions<MdmDbContext> options) : base(options) { }

        // 領域與來源
        public DbSet<MdmDomain> Domains { get; set; } = null!;
        public DbSet<MdmSourceSystem> SourceSystems { get; set; } = null!;
        public DbSet<MdmDomainSource> DomainSources { get; set; } = null!;

        // Golden Record
        public DbSet<MdmRecord> Records { get; set; } = null!;
        public DbSet<MdmGoldenRecord> GoldenRecords { get; set; } = null!;
        public DbSet<MdmGoldenRecordSource> GoldenRecordSources { get; set; } = null!;

        // 比對與合併
        public DbSet<MdmMatchRule> MatchRules { get; set; } = null!;
        public DbSet<MdmMatchCandidate> MatchCandidates { get; set; } = null!;
        public DbSet<MdmMergeJob> MergeJobs { get; set; } = null!;
        public DbSet<MdmMergeHistory> MergeHistories { get; set; } = null!;

        // 存續與標準化
        public DbSet<MdmSurvivorshipRule> SurvivorshipRules { get; set; } = null!;
        public DbSet<MdmNormalizationRule> NormalizationRules { get; set; } = null!;
        public DbSet<MdmCodeMapping> CodeMappings { get; set; } = null!;

        // 品質與變更
        public DbSet<MdmQualityRule> QualityRules { get; set; } = null!;
        public DbSet<MdmQualityResult> QualityResults { get; set; } = null!;
        public DbSet<MdmGoldenVersion> GoldenVersions { get; set; } = null!;
        public DbSet<MdmChangeRequest> ChangeRequests { get; set; } = null!;
        public DbSet<MdmPublishJob> PublishJobs { get; set; } = null!;
        public DbSet<MdmSyncCheckpoint> SyncCheckpoints { get; set; } = null!;
        public DbSet<MdmStewardAssignment> StewardAssignments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MdmDomain>().ToTable("mdm_domain");
            modelBuilder.Entity<MdmSourceSystem>().ToTable("mdm_source_system");
            modelBuilder.Entity<MdmDomainSource>().ToTable("mdm_domain_source");
            modelBuilder.Entity<MdmRecord>().ToTable("mdm_record");
            modelBuilder.Entity<MdmGoldenRecord>().ToTable("mdm_golden_record");
            modelBuilder.Entity<MdmGoldenRecordSource>().ToTable("mdm_golden_record_source");
            modelBuilder.Entity<MdmMatchRule>().ToTable("mdm_match_rule");
            modelBuilder.Entity<MdmMatchCandidate>().ToTable("mdm_match_candidate");
            modelBuilder.Entity<MdmMergeJob>().ToTable("mdm_merge_job");
            modelBuilder.Entity<MdmMergeHistory>().ToTable("mdm_merge_history");
            modelBuilder.Entity<MdmSurvivorshipRule>().ToTable("mdm_survivorship_rule");
            modelBuilder.Entity<MdmNormalizationRule>().ToTable("mdm_normalization_rule");
            modelBuilder.Entity<MdmCodeMapping>().ToTable("mdm_code_mapping");
            modelBuilder.Entity<MdmQualityRule>().ToTable("mdm_quality_rule");
            modelBuilder.Entity<MdmQualityResult>().ToTable("mdm_quality_result");
            modelBuilder.Entity<MdmGoldenVersion>().ToTable("mdm_golden_version");
            modelBuilder.Entity<MdmChangeRequest>().ToTable("mdm_change_request");
            modelBuilder.Entity<MdmPublishJob>().ToTable("mdm_publish_job");
            modelBuilder.Entity<MdmSyncCheckpoint>().ToTable("mdm_sync_checkpoint");
            modelBuilder.Entity<MdmStewardAssignment>().ToTable("mdm_steward_assignment");
        }
    }
}