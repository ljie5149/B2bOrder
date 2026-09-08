using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Customer.Models;

namespace B2bOrder.Resources.Customer
{
    public class SubscriptionDbContext : DbContext
    {
        public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : base(options) { }

        public DbSet<SubPlan> SubPlans { get; set; }
        public DbSet<SubPlanPricing> SubPlanPricings { get; set; }
        public DbSet<SubSubscription> SubSubscriptions { get; set; }
        public DbSet<SubInvoice> SubInvoices { get; set; }
        public DbSet<SubPaymentAttempt> SubPaymentAttempts { get; set; }
        public DbSet<SubSubscriptionLog> SubSubscriptionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 唯一索引 (Unique Indexes)
            modelBuilder.Entity<SubPlan>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<SubPlan>().HasIndex(e => new { e.CompanySid, e.PlanCode }).IsUnique().HasDatabaseName("uk_sp_company_code");

            modelBuilder.Entity<SubPlanPricing>().HasIndex(e => e.Sid).IsUnique();

            modelBuilder.Entity<SubSubscription>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<SubSubscription>().HasIndex(e => e.SubscriptionNo).IsUnique().HasDatabaseName("uk_ss_subscription_no");

            modelBuilder.Entity<SubInvoice>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<SubInvoice>().HasIndex(e => e.InvoiceNo).IsUnique().HasDatabaseName("uk_si_invoice_no");

            modelBuilder.Entity<SubPaymentAttempt>().HasIndex(e => e.Sid).IsUnique();

            modelBuilder.Entity<SubSubscriptionLog>().HasIndex(e => e.Sid).IsUnique();

            // 欄位預設值 (Default Values)
            modelBuilder.Entity<SubPlan>().Property(e => e.PlanStatus).HasDefaultValue("ACTIVE");
            modelBuilder.Entity<SubPlan>().Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<SubPlan>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<SubPlanPricing>().Property(e => e.BillingCycleUnit).HasDefaultValue("MONTH");
            modelBuilder.Entity<SubPlanPricing>().Property(e => e.BillingCycleInterval).HasDefaultValue(1);
            modelBuilder.Entity<SubPlanPricing>().Property(e => e.CurrencyCode).HasDefaultValue("TWD");
            modelBuilder.Entity<SubPlanPricing>().Property(e => e.TrialPeriodDays).HasDefaultValue(0);
            modelBuilder.Entity<SubPlanPricing>().Property(e => e.SetupFeeAmount).HasDefaultValue(0.0000m);
            modelBuilder.Entity<SubPlanPricing>().Property(e => e.PricingStatus).HasDefaultValue("ACTIVE");
            modelBuilder.Entity<SubPlanPricing>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<SubSubscription>().Property(e => e.SubscriptionStatus).HasDefaultValue("PENDING");
            modelBuilder.Entity<SubSubscription>().Property(e => e.AutoRenew).HasDefaultValue("Y");
            modelBuilder.Entity<SubSubscription>().Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<SubSubscription>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<SubInvoice>().Property(e => e.SubtotalAmount).HasDefaultValue(0.0000m);
            modelBuilder.Entity<SubInvoice>().Property(e => e.DiscountAmount).HasDefaultValue(0.0000m);
            modelBuilder.Entity<SubInvoice>().Property(e => e.TaxAmount).HasDefaultValue(0.0000m);
            modelBuilder.Entity<SubInvoice>().Property(e => e.TotalAmount).HasDefaultValue(0.0000m);
            modelBuilder.Entity<SubInvoice>().Property(e => e.CurrencyCode).HasDefaultValue("TWD");
            modelBuilder.Entity<SubInvoice>().Property(e => e.InvoiceStatus).HasDefaultValue("DRAFT");
            modelBuilder.Entity<SubInvoice>().Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<SubInvoice>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<SubPaymentAttempt>().Property(e => e.AttemptNumber).HasDefaultValue(1);
            modelBuilder.Entity<SubPaymentAttempt>().Property(e => e.AttemptStatus).HasDefaultValue("PROCESSING");

            modelBuilder.Entity<SubSubscriptionLog>().Property(e => e.OperatorType).HasDefaultValue("SYSTEM");
        }
    }
}