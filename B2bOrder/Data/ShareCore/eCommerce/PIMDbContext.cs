using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.ShareCore.eCommerce.Models;

namespace B2bOrder.Resources.ShareCore.eCommerce
{
    public class MIMDbContext : DbContext
    {
        public MIMDbContext(DbContextOptions<MIMDbContext> options) : base(options)
        {
        }

        public DbSet<PimItem> PimItems { get; set; }
        public DbSet<PimItemTranslation> PimItemTranslations { get; set; }
        public DbSet<PimVariant> PimVariants { get; set; }
        public DbSet<PimIdentifier> PimIdentifiers { get; set; }
        public DbSet<PimSpecification> PimSpecifications { get; set; }
        public DbSet<PimSpecificationValue> PimSpecificationValues { get; set; }
        public DbSet<PimVariantSpecification> PimVariantSpecifications { get; set; }
        public DbSet<PimAttributeGroup> PimAttributeGroups { get; set; }
        public DbSet<PimAttribute> PimAttributes { get; set; }
        public DbSet<PimAttributeOption> PimAttributeOptions { get; set; }
        public DbSet<PimAttributeValue> PimAttributeValues { get; set; }
        public DbSet<PimMedia> PimMediaList { get; set; }
        public DbSet<PimCertification> PimCertifications { get; set; }
        public DbSet<PimItemParty> PimItemParties { get; set; }
        public DbSet<PimItemRelation> PimItemRelations { get; set; }
        public DbSet<PimApplicationScope> PimApplicationScopes { get; set; }
        public DbSet<PimStructure> PimStructures { get; set; }
        public DbSet<PimStructureComponent> PimStructureComponents { get; set; }
        public DbSet<PimItemVersion> PimItemVersions { get; set; }
        public DbSet<PimPublishTarget> PimPublishTargets { get; set; }
        public DbSet<PimTargetItem> PimTargetItems { get; set; }
        public DbSet<PimPublishJob> PimPublishJobs { get; set; }
        public DbSet<PimImportJob> PimImportJobs { get; set; }
        public DbSet<PimImportError> PimImportErrors { get; set; }
        public DbSet<PimQualityRule> PimQualityRules { get; set; }
        public DbSet<PimQualityResult> PimQualityResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UNIQUE Constraints
            modelBuilder.Entity<PimItem>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<PimItem>()
                .HasIndex(e => e.ItemNo).IsUnique();
            modelBuilder.Entity<PimItem>()
                .HasIndex(e => e.PrimaryBarcode).IsUnique();

            modelBuilder.Entity<PimItemTranslation>()
                .HasIndex(e => new { e.ItemNid, e.LanguageSid }).IsUnique();

            modelBuilder.Entity<PimVariant>()
                .HasIndex(e => e.VariantNo).IsUnique();
            modelBuilder.Entity<PimVariant>()
                .HasIndex(e => e.Barcode).IsUnique();

            modelBuilder.Entity<PimIdentifier>()
                .HasIndex(e => new { e.IdentifierType, e.IdentifierValue }).IsUnique();

            modelBuilder.Entity<PimSpecification>()
                .HasIndex(e => e.SpecificationCode).IsUnique();

            modelBuilder.Entity<PimSpecificationValue>()
                .HasIndex(e => new { e.SpecificationNid, e.ValueCode }).IsUnique();

            modelBuilder.Entity<PimVariantSpecification>()
                .HasIndex(e => new { e.VariantNid, e.SpecificationNid }).IsUnique();

            modelBuilder.Entity<PimAttributeGroup>()
                .HasIndex(e => e.GroupCode).IsUnique();

            modelBuilder.Entity<PimAttribute>()
                .HasIndex(e => e.AttributeCode).IsUnique();

            modelBuilder.Entity<PimAttributeOption>()
                .HasIndex(e => new { e.AttributeNid, e.OptionCode }).IsUnique();

            modelBuilder.Entity<PimAttributeValue>()
                .HasIndex(e => new { e.ItemNid, e.VariantNid, e.AttributeNid, e.LanguageSid }).IsUnique();

            modelBuilder.Entity<PimMedia>()
                .HasIndex(e => new { e.ItemNid, e.VariantNid, e.FileSid, e.UsageType }).IsUnique();

            modelBuilder.Entity<PimItemParty>()
                .HasIndex(e => new { e.ItemNid, e.VariantNid, e.PartySid, e.RelationshipType }).IsUnique();

            modelBuilder.Entity<PimItemRelation>()
                .HasIndex(e => new { e.ItemNid, e.RelatedItemSid, e.RelationType }).IsUnique();

            modelBuilder.Entity<PimApplicationScope>()
                .HasIndex(e => new { e.ItemNid, e.ScopeType, e.ScopeSid }).IsUnique();

            modelBuilder.Entity<PimStructure>()
                .HasIndex(e => new { e.ItemNid, e.StructureType, e.StructureVersion }).IsUnique();

            modelBuilder.Entity<PimStructureComponent>()
                .HasIndex(e => new { e.StructureNid, e.ComponentItemSid, e.ComponentVariantSid }).IsUnique();

            modelBuilder.Entity<PimItemVersion>()
                .HasIndex(e => new { e.ItemNid, e.VersionNo }).IsUnique();

            modelBuilder.Entity<PimPublishTarget>()
                .HasIndex(e => e.TargetCode).IsUnique();

            modelBuilder.Entity<PimTargetItem>()
                .HasIndex(e => new { e.PublishTargetNid, e.ItemNid }).IsUnique();

            modelBuilder.Entity<PimPublishJob>()
                .HasIndex(e => e.JobNo).IsUnique();

            modelBuilder.Entity<PimImportJob>()
                .HasIndex(e => e.JobNo).IsUnique();

            modelBuilder.Entity<PimQualityRule>()
                .HasIndex(e => e.RuleCode).IsUnique();

            // Default Values setup
            modelBuilder.Entity<PimItem>()
                .Property(e => e.SerialControl).HasDefaultValue(false);
            modelBuilder.Entity<PimItem>()
                .Property(e => e.BatchControl).HasDefaultValue(false);
            modelBuilder.Entity<PimItem>()
                .Property(e => e.HazardousMark).HasDefaultValue(false);
            modelBuilder.Entity<PimItem>()
                .Property(e => e.RestrictedMark).HasDefaultValue(false);
            modelBuilder.Entity<PimItem>()
                .Property(e => e.VirtualMark).HasDefaultValue(false);
            modelBuilder.Entity<PimItem>()
                .Property(e => e.ReusableMark).HasDefaultValue(false);
            modelBuilder.Entity<PimItem>()
                .Property(e => e.AssetMark).HasDefaultValue(false);
            modelBuilder.Entity<PimItem>()
                .Property(e => e.ContentCompleteness).HasDefaultValue(0m);
            modelBuilder.Entity<PimItem>()
                .Property(e => e.CurrentVersion).HasDefaultValue(1);
            modelBuilder.Entity<PimItem>()
                .Property(e => e.ItemStatus).HasDefaultValue("DRAFT");
            modelBuilder.Entity<PimItem>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<PimVariant>()
                .Property(e => e.MinimumOrderQty).HasDefaultValue(1m);
            modelBuilder.Entity<PimVariant>()
                .Property(e => e.OrderMultipleQty).HasDefaultValue(1m);
            modelBuilder.Entity<PimVariant>()
                .Property(e => e.DefaultMark).HasDefaultValue(false);
            modelBuilder.Entity<PimVariant>()
                .Property(e => e.VariantStatus).HasDefaultValue("ACTIVE");
            modelBuilder.Entity<PimVariant>()
                .Property(e => e.Avalible).HasDefaultValue("Y");
        }
    }
}