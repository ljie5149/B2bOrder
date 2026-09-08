using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Construction.Models;

namespace B2bOrder.Resources.Construction
{
    public class ProgressPaymentDbContext : DbContext
    {
        public ProgressPaymentDbContext(DbContextOptions<ProgressPaymentDbContext> options) : base(options)
        {
        }

        #region DbSet Configurations - 資料集設定

        public DbSet<PgpPaymentValuation> PgpPaymentValuations { get; set; }
        public DbSet<PgpValuationItemLine> PgpValuationItemLines { get; set; }
        public DbSet<PgpBackchargeLine> PgpBackchargeLines { get; set; }
        public DbSet<PgpRetentionRelease> PgpRetentionReleases { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Index Configurations - 唯一索引設定

            // 01. pgp_payment_valuation
            modelBuilder.Entity<PgpPaymentValuation>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<PgpPaymentValuation>()
                .HasIndex(e => new { e.ContractSid, e.PeriodNumber })
                .IsUnique()
                .HasDatabaseName("uk_ppv_contract_period");

            // 02. pgp_valuation_item_line
            modelBuilder.Entity<PgpValuationItemLine>()
                .HasIndex(e => e.Sid).IsUnique();

            // 03. pgp_backcharge_line
            modelBuilder.Entity<PgpBackchargeLine>()
                .HasIndex(e => e.Sid).IsUnique();

            // 04. pgp_retention_release
            modelBuilder.Entity<PgpRetentionRelease>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<PgpRetentionRelease>()
                .HasIndex(e => new { e.ContractSid, e.ReleaseNumber })
                .IsUnique()
                .HasDatabaseName("uk_prr_contract_release");

            #endregion

            #region Index Configurations - 一般索引設定

            // 01. pgp_payment_valuation
            modelBuilder.Entity<PgpPaymentValuation>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<PgpPaymentValuation>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<PgpPaymentValuation>()
                .HasIndex(e => e.VendorSid);
            modelBuilder.Entity<PgpPaymentValuation>()
                .HasIndex(e => e.ValuationStatus);
            modelBuilder.Entity<PgpPaymentValuation>()
                .HasIndex(e => e.Avalible);

            // 02. pgp_valuation_item_line
            modelBuilder.Entity<PgpValuationItemLine>()
                .HasIndex(e => e.ValuationSid);
            modelBuilder.Entity<PgpValuationItemLine>()
                .HasIndex(e => e.WbsSid);
            modelBuilder.Entity<PgpValuationItemLine>()
                .HasIndex(e => e.Avalible);

            // 03. pgp_backcharge_line
            modelBuilder.Entity<PgpBackchargeLine>()
                .HasIndex(e => e.ValuationSid);
            modelBuilder.Entity<PgpBackchargeLine>()
                .HasIndex(e => e.DeductionType);
            modelBuilder.Entity<PgpBackchargeLine>()
                .HasIndex(e => e.Avalible);

            // 04. pgp_retention_release
            modelBuilder.Entity<PgpRetentionRelease>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<PgpRetentionRelease>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<PgpRetentionRelease>()
                .HasIndex(e => e.VendorSid);
            modelBuilder.Entity<PgpRetentionRelease>()
                .HasIndex(e => e.ReleaseStatus);
            modelBuilder.Entity<PgpRetentionRelease>()
                .HasIndex(e => e.Avalible);

            #endregion

            #region Default Value Configurations - 預設值設定

            // 01. pgp_payment_valuation
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.PeriodNumber).HasDefaultValue(1);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.AccumulatedPreviousAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.CurrentClaimedAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.CurrentApprovedAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.AccumulatedApprovedAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.RetentionRate).HasDefaultValue(5.00m);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.CurrentRetentionDeduction).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.CurrentAdvanceDeduction).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.CurrentBackchargeDeduction).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.PriceEscalationAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.NetApprovedBeforeTax).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.TaxRate).HasDefaultValue(5.00m);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.TaxAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.TotalPayableAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.ValuationStatus).HasDefaultValue("SUBMITTED");
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<PgpPaymentValuation>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 02. pgp_valuation_item_line
            modelBuilder.Entity<PgpValuationItemLine>()
                .Property(e => e.UnitPrice).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpValuationItemLine>()
                .Property(e => e.ContractQuantity).HasDefaultValue(0.0000m);
            modelBuilder.Entity<PgpValuationItemLine>()
                .Property(e => e.PreviousQuantity).HasDefaultValue(0.0000m);
            modelBuilder.Entity<PgpValuationItemLine>()
                .Property(e => e.CurrentClaimedQty).HasDefaultValue(0.0000m);
            modelBuilder.Entity<PgpValuationItemLine>()
                .Property(e => e.CurrentApprovedQty).HasDefaultValue(0.0000m);
            modelBuilder.Entity<PgpValuationItemLine>()
                .Property(e => e.AccumulatedApprovedQty).HasDefaultValue(0.0000m);
            modelBuilder.Entity<PgpValuationItemLine>()
                .Property(e => e.CompletionRate).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpValuationItemLine>()
                .Property(e => e.CurrentApprovedAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpValuationItemLine>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<PgpValuationItemLine>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 03. pgp_backcharge_line
            modelBuilder.Entity<PgpBackchargeLine>()
                .Property(e => e.DeductionType).HasDefaultValue("SAFETY_PENALTY");
            modelBuilder.Entity<PgpBackchargeLine>()
                .Property(e => e.DeductionAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpBackchargeLine>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<PgpBackchargeLine>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 04. pgp_retention_release
            modelBuilder.Entity<PgpRetentionRelease>()
                .Property(e => e.TotalRetainedAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpRetentionRelease>()
                .Property(e => e.ReleaseStage).HasDefaultValue("FINAL_INSPECTION");
            modelBuilder.Entity<PgpRetentionRelease>()
                .Property(e => e.RequestedReleaseAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpRetentionRelease>()
                .Property(e => e.ApprovedReleaseAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<PgpRetentionRelease>()
                .Property(e => e.ReleaseStatus).HasDefaultValue("SUBMITTED");
            modelBuilder.Entity<PgpRetentionRelease>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<PgpRetentionRelease>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            #endregion
        }
    }
}