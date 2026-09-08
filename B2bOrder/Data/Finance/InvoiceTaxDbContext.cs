using B2bOrder.Resources.eCommerce;
using B2bOrder.Resources.ERP;
using B2bOrder.Resources.Finance.Models;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Resources.Finance
{
    /// <summary>
    /// InvoiceTax 資料庫內容物件 (DbContext)
    /// </summary>
    public class HumanResourcePayrollDbContext : DbContext
    {
        public HumanResourcePayrollDbContext(DbContextOptions<HumanResourcePayrollDbContext> options)
            : base(options)
        {
        }

        // 01. 稅別與發票政策
        public DbSet<TaxType> TaxTypes => Set<TaxType>();
        public DbSet<TaxInvoicePolicy> TaxInvoicePolicies => Set<TaxInvoicePolicy>();

        // 02. 發票字軌與號碼
        public DbSet<TaxInvoiceTrack> TaxInvoiceTracks => Set<TaxInvoiceTrack>();
        public DbSet<TaxInvoiceNumber> TaxInvoiceNumbers => Set<TaxInvoiceNumber>();

        // 03. 銷項發票
        public DbSet<TaxSalesInvoice> TaxSalesInvoices => Set<TaxSalesInvoice>();
        public DbSet<TaxSalesInvoiceItem> TaxSalesInvoiceItems => Set<TaxSalesInvoiceItem>();

        // 04. 進項發票
        public DbSet<TaxPurchaseInvoice> TaxPurchaseInvoices => Set<TaxPurchaseInvoice>();
        public DbSet<TaxPurchaseInvoiceItem> TaxPurchaseInvoiceItems => Set<TaxPurchaseInvoiceItem>();

        // 05. 發票請求與開立工作
        public DbSet<TaxInvoiceRequest> TaxInvoiceRequests => Set<TaxInvoiceRequest>();

        // 06. 作廢
        public DbSet<TaxInvoiceVoid> TaxInvoiceVoids => Set<TaxInvoiceVoid>();

        // 07. 折讓與退貨調整
        public DbSet<TaxAllowance> TaxAllowances => Set<TaxAllowance>();
        public DbSet<TaxAllowanceItem> TaxAllowanceItems => Set<TaxAllowanceItem>();

        // 08. 載具、捐贈與買受人資料
        public DbSet<TaxCarrier> TaxCarriers => Set<TaxCarrier>();
        public DbSet<TaxDonationCode> TaxDonationCodes => Set<TaxDonationCode>();

        // 09. 三方匹配與稅務驗證
        public DbSet<TaxInvoiceMatch> TaxInvoiceMatches => Set<TaxInvoiceMatch>();
        public DbSet<TaxValidationLog> TaxValidationLogs => Set<TaxValidationLog>();

        // 10. 電子發票平台交換
        public DbSet<TaxPlatformMessage> TaxPlatformMessages => Set<TaxPlatformMessage>();

        // 11. 稅務申報
        public DbSet<TaxFilingBatch> TaxFilingBatches => Set<TaxFilingBatch>();
        public DbSet<TaxFilingItem> TaxFilingItems => Set<TaxFilingItem>();

        // 12. 狀態歷程與事件
        public DbSet<TaxStatusHistory> TaxStatusHistories => Set<TaxStatusHistory>();
        public DbSet<TaxEvent> TaxEvents => Set<TaxEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique Constraints 與額外索引設定
            modelBuilder.Entity<TaxType>()
                .HasIndex(e => e.TaxCode)
                .IsUnique();

            modelBuilder.Entity<TaxInvoicePolicy>()
                .HasIndex(e => e.PolicyCode)
                .IsUnique();

            modelBuilder.Entity<TaxInvoiceTrack>()
                .HasIndex(e => new { e.CompanySid, e.BusinessUnitSid, e.InvoiceYear, e.InvoicePeriod, e.TrackCode })
                .IsUnique();

            modelBuilder.Entity<TaxInvoiceNumber>()
                .HasIndex(e => e.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<TaxInvoiceNumber>()
                .HasIndex(e => new { e.InvoiceTrackSid, e.NumberValue })
                .IsUnique();

            modelBuilder.Entity<TaxSalesInvoice>()
                .HasIndex(e => e.InvoiceNo)
                .IsUnique();

            modelBuilder.Entity<TaxSalesInvoice>()
                .HasIndex(e => e.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<TaxSalesInvoiceItem>()
                .HasIndex(e => new { e.SalesInvoiceNid, e.LineNo })
                .IsUnique();

            modelBuilder.Entity<TaxPurchaseInvoice>()
                .HasIndex(e => e.PurchaseInvoiceNo)
                .IsUnique();

            modelBuilder.Entity<TaxPurchaseInvoice>()
                .HasIndex(e => new { e.SupplierPartySid, e.SupplierInvoiceNo })
                .IsUnique();

            modelBuilder.Entity<TaxPurchaseInvoiceItem>()
                .HasIndex(e => new { e.PurchaseInvoiceNid, e.LineNo })
                .IsUnique();

            modelBuilder.Entity<TaxInvoiceRequest>()
                .HasIndex(e => e.RequestNo)
                .IsUnique();

            modelBuilder.Entity<TaxInvoiceRequest>()
                .HasIndex(e => e.IdempotencyKey)
                .IsUnique();

            modelBuilder.Entity<TaxInvoiceVoid>()
                .HasIndex(e => e.VoidNo)
                .IsUnique();

            modelBuilder.Entity<TaxAllowance>()
                .HasIndex(e => e.AllowanceNo)
                .IsUnique();

            modelBuilder.Entity<TaxAllowance>()
                .HasIndex(e => e.AllowanceNumber)
                .IsUnique();

            modelBuilder.Entity<TaxAllowanceItem>()
                .HasIndex(e => new { e.AllowanceNid, e.LineNo })
                .IsUnique();

            modelBuilder.Entity<TaxCarrier>()
                .HasIndex(e => new { e.CarrierType, e.CarrierNoHash })
                .IsUnique();

            modelBuilder.Entity<TaxDonationCode>()
                .HasIndex(e => e.DonationCode)
                .IsUnique();

            modelBuilder.Entity<TaxPlatformMessage>()
                .HasIndex(e => e.MessageNo)
                .IsUnique();

            modelBuilder.Entity<TaxPlatformMessage>()
                .HasIndex(e => new { e.PlatformCode, e.ProviderMessageId })
                .IsUnique();

            modelBuilder.Entity<TaxFilingBatch>()
                .HasIndex(e => e.FilingNo)
                .IsUnique();

            modelBuilder.Entity<TaxFilingBatch>()
                .HasIndex(e => new { e.CompanySid, e.FilingType, e.PeriodStartDate, e.PeriodEndDate })
                .IsUnique();

            modelBuilder.Entity<TaxFilingItem>()
                .HasIndex(e => new { e.FilingBatchNid, e.LineNo })
                .IsUnique();

            modelBuilder.Entity<TaxEvent>()
                .HasIndex(e => e.SourceEventId)
                .IsUnique();
        }
    }
}