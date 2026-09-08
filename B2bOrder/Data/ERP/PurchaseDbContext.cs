using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Resources.eCommerce
{
    public class PurchaseDbContext : DbContext
    {
        public PurchaseDbContext(DbContextOptions<PurchaseDbContext> options) : base(options)
        {
        }

        public DbSet<PurPurchaseType> PurPurchaseTypes { get; set; } = null!;
        public DbSet<PurPolicy> PurPolicies { get; set; } = null!;

        public DbSet<PurRequisition> PurRequisitions { get; set; } = null!;
        public DbSet<PurRequisitionItem> PurRequisitionItems { get; set; } = null!;

        public DbSet<PurRfq> PurRfqs { get; set; } = null!;
        public DbSet<PurRfqItem> PurRfqItems { get; set; } = null!;
        public DbSet<PurRfqSupplier> PurRfqSuppliers { get; set; } = null!;
        public DbSet<PurSupplierQuote> PurSupplierQuotes { get; set; } = null!;
        public DbSet<PurSupplierQuoteItem> PurSupplierQuoteItems { get; set; } = null!;
        public DbSet<PurQuoteEvaluation> PurQuoteEvaluations { get; set; } = null!;

        public DbSet<PurOrder> PurOrders { get; set; } = null!;
        public DbSet<PurOrderItem> PurOrderItems { get; set; } = null!;
        public DbSet<PurOrderSchedule> PurOrderSchedules { get; set; } = null!;

        public DbSet<PurReceipt> PurReceipts { get; set; } = null!;
        public DbSet<PurReceiptItem> PurReceiptItems { get; set; } = null!;

        public DbSet<PurInspection> PurInspections { get; set; } = null!;
        public DbSet<PurInspectionItem> PurInspectionItems { get; set; } = null!;

        public DbSet<PurReturn> PurReturns { get; set; } = null!;
        public DbSet<PurReturnItem> PurReturnItems { get; set; } = null!;

        public DbSet<PurOrderChange> PurOrderChanges { get; set; } = null!;
        public DbSet<PurSupplierPerformance> PurSupplierPerformances { get; set; } = null!;

        public DbSet<PurStatusHistory> PurStatusHistories { get; set; } = null!;
        public DbSet<PurEvent> PurEvents { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique indexes and constraints matching database schema
            modelBuilder.Entity<PurPurchaseType>()
                .HasIndex(e => e.PurchaseTypeCode)
                .IsUnique();

            modelBuilder.Entity<PurPolicy>()
                .HasIndex(e => e.PolicyCode)
                .IsUnique();

            modelBuilder.Entity<PurRequisition>()
                .HasIndex(e => e.RequisitionNo)
                .IsUnique();

            modelBuilder.Entity<PurRequisitionItem>()
                .HasIndex(e => new { e.RequisitionNid, e.LineNo })
                .IsUnique();

            modelBuilder.Entity<PurRfq>()
                .HasIndex(e => e.RfqNo)
                .IsUnique();

            modelBuilder.Entity<PurRfqItem>()
                .HasIndex(e => new { e.RfqNid, e.LineNo })
                .IsUnique();

            modelBuilder.Entity<PurRfqSupplier>()
                .HasIndex(e => new { e.RfqNid, e.SupplierPartySid })
                .IsUnique();

            modelBuilder.Entity<PurSupplierQuote>()
                .HasIndex(e => e.QuoteNo)
                .IsUnique();

            modelBuilder.Entity<PurSupplierQuote>()
                .HasIndex(e => new { e.RfqSid, e.SupplierPartySid })
                .IsUnique();

            modelBuilder.Entity<PurSupplierQuoteItem>()
                .HasIndex(e => new { e.SupplierQuoteNid, e.LineNo })
                .IsUnique();

            modelBuilder.Entity<PurQuoteEvaluation>()
                .HasIndex(e => new { e.RfqSid, e.SupplierQuoteSid })
                .IsUnique();

            modelBuilder.Entity<PurOrder>()
                .HasIndex(e => e.PurchaseOrderNo)
                .IsUnique();

            modelBuilder.Entity<PurOrderItem>()
                .HasIndex(e => new { e.PurchaseOrderNid, e.LineNo })
                .IsUnique();

            modelBuilder.Entity<PurOrderSchedule>()
                .HasIndex(e => new { e.PurchaseOrderItemNid, e.ScheduleNo })
                .IsUnique();

            modelBuilder.Entity<PurReceipt>()
                .HasIndex(e => e.ReceiptNo)
                .IsUnique();

            modelBuilder.Entity<PurReceiptItem>()
                .HasIndex(e => new { e.ReceiptNid, e.LineNo })
                .IsUnique();

            modelBuilder.Entity<PurInspection>()
                .HasIndex(e => e.InspectionNo)
                .IsUnique();

            modelBuilder.Entity<PurInspectionItem>()
                .HasIndex(e => new { e.InspectionNid, e.LineNo })
                .IsUnique();

            modelBuilder.Entity<PurReturn>()
                .HasIndex(e => e.ReturnNo)
                .IsUnique();

            modelBuilder.Entity<PurReturnItem>()
                .HasIndex(e => new { e.ReturnNid, e.LineNo })
                .IsUnique();

            modelBuilder.Entity<PurOrderChange>()
                .HasIndex(e => e.ChangeNo)
                .IsUnique();

            modelBuilder.Entity<PurSupplierPerformance>()
                .HasIndex(e => new { e.SupplierPartySid, e.CompanySid, e.ProjectSid, e.PeriodStartDate, e.PeriodEndDate })
                .IsUnique();

            modelBuilder.Entity<PurEvent>()
                .HasIndex(e => e.SourceEventId)
                .IsUnique();
        }
    }
}