using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Construction.Models;

namespace B2bOrder.Resources.Construction
{
    public class CustomerSalesCRMDbContext : DbContext
    {
        public CustomerSalesCRMDbContext(DbContextOptions<CustomerSalesCRMDbContext> options) : base(options)
        {
        }

        #region DbSet Configurations - 資料集設定

        public DbSet<BgtTargetBudget> BgtTargetBudgets { get; set; }
        public DbSet<BgtBudgetItemLine> BgtBudgetItemLines { get; set; }
        public DbSet<BgtContractCommitment> BgtContractCommitments { get; set; }
        public DbSet<BgtActualCostLog> BgtActualCostLogs { get; set; }
        public DbSet<BgtBudgetReallocation> BgtBudgetReallocations { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Index Configurations - 唯一索引設定

            // 01. bgt_target_budget
            modelBuilder.Entity<BgtTargetBudget>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<BgtTargetBudget>()
                .HasIndex(e => new { e.ProjectSid, e.BudgetVersionCode })
                .IsUnique()
                .HasDatabaseName("uk_btb_project_version");

            // 02. bgt_budget_item_line
            modelBuilder.Entity<BgtBudgetItemLine>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<BgtBudgetItemLine>()
                .HasIndex(e => new { e.BudgetSid, e.ItemCode })
                .IsUnique()
                .HasDatabaseName("uk_bil_budget_item");

            // 03. bgt_contract_commitment
            modelBuilder.Entity<BgtContractCommitment>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<BgtContractCommitment>()
                .HasIndex(e => new { e.ProjectSid, e.ContractNumber })
                .IsUnique()
                .HasDatabaseName("uk_bcc_project_contract");

            // 04. bgt_actual_cost_log
            modelBuilder.Entity<BgtActualCostLog>()
                .HasIndex(e => e.Sid).IsUnique();

            // 05. bgt_budget_reallocation
            modelBuilder.Entity<BgtBudgetReallocation>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<BgtBudgetReallocation>()
                .HasIndex(e => new { e.ProjectSid, e.TransferNumber })
                .IsUnique()
                .HasDatabaseName("uk_bbr_project_number");

            #endregion

            #region Composite Index Configurations - 複合索引與一般索引設定

            // 01. bgt_target_budget
            modelBuilder.Entity<BgtTargetBudget>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<BgtTargetBudget>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<BgtTargetBudget>()
                .HasIndex(e => e.BudgetStatus);
            modelBuilder.Entity<BgtTargetBudget>()
                .HasIndex(e => e.Avalible);

            // 02. bgt_budget_item_line
            modelBuilder.Entity<BgtBudgetItemLine>()
                .HasIndex(e => e.BudgetSid);
            modelBuilder.Entity<BgtBudgetItemLine>()
                .HasIndex(e => e.WbsSid);
            modelBuilder.Entity<BgtBudgetItemLine>()
                .HasIndex(e => e.CostCategoryCode);
            modelBuilder.Entity<BgtBudgetItemLine>()
                .HasIndex(e => e.Avalible);

            // 03. bgt_contract_commitment
            modelBuilder.Entity<BgtContractCommitment>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<BgtContractCommitment>()
                .HasIndex(e => e.BudgetItemSid);
            modelBuilder.Entity<BgtContractCommitment>()
                .HasIndex(e => e.VendorSid);
            modelBuilder.Entity<BgtContractCommitment>()
                .HasIndex(e => e.CommitmentStatus);
            modelBuilder.Entity<BgtContractCommitment>()
                .HasIndex(e => e.Avalible);

            // 04. bgt_actual_cost_log
            modelBuilder.Entity<BgtActualCostLog>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<BgtActualCostLog>()
                .HasIndex(e => e.BudgetItemSid);
            modelBuilder.Entity<BgtActualCostLog>()
                .HasIndex(e => e.CommitmentSid);
            modelBuilder.Entity<BgtActualCostLog>()
                .HasIndex(e => new { e.SourceDocumentType, e.SourceDocumentSid });
            modelBuilder.Entity<BgtActualCostLog>()
                .HasIndex(e => e.PostingDate);

            // 05. bgt_budget_reallocation
            modelBuilder.Entity<BgtBudgetReallocation>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<BgtBudgetReallocation>()
                .HasIndex(e => e.FromBudgetItemSid);
            modelBuilder.Entity<BgtBudgetReallocation>()
                .HasIndex(e => e.ToBudgetItemSid);
            modelBuilder.Entity<BgtBudgetReallocation>()
                .HasIndex(e => e.TransferStatus);
            modelBuilder.Entity<BgtBudgetReallocation>()
                .HasIndex(e => e.Avalible);

            #endregion

            #region Default Value Configurations - 預設值設定

            // 01. bgt_target_budget
            modelBuilder.Entity<BgtTargetBudget>()
                .Property(e => e.BudgetVersionCode).HasDefaultValue("V1.0");
            modelBuilder.Entity<BgtTargetBudget>()
                .Property(e => e.TotalTargetBudget).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtTargetBudget>()
                .Property(e => e.ApprovedVoBudget).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtTargetBudget>()
                .Property(e => e.RevisedTotalBudget).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtTargetBudget>()
                .Property(e => e.CommittedAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtTargetBudget>()
                .Property(e => e.ActualCostAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtTargetBudget>()
                .Property(e => e.BudgetStatus).HasDefaultValue("DRAFT");
            modelBuilder.Entity<BgtTargetBudget>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<BgtTargetBudget>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 02. bgt_budget_item_line
            modelBuilder.Entity<BgtBudgetItemLine>()
                .Property(e => e.UnitPrice).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtBudgetItemLine>()
                .Property(e => e.Quantity).HasDefaultValue(0.0000m);
            modelBuilder.Entity<BgtBudgetItemLine>()
                .Property(e => e.OriginalBudgetAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtBudgetItemLine>()
                .Property(e => e.ReallocatedAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtBudgetItemLine>()
                .Property(e => e.RevisedBudgetAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtBudgetItemLine>()
                .Property(e => e.CommittedAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtBudgetItemLine>()
                .Property(e => e.ActualCostAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtBudgetItemLine>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<BgtBudgetItemLine>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 03. bgt_contract_commitment
            modelBuilder.Entity<BgtContractCommitment>()
                .Property(e => e.OriginalContractAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtContractCommitment>()
                .Property(e => e.VoAccumulatedAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtContractCommitment>()
                .Property(e => e.CurrentContractAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtContractCommitment>()
                .Property(e => e.ActualInvoicedAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtContractCommitment>()
                .Property(e => e.CommitmentStatus).HasDefaultValue("ACTIVE");
            modelBuilder.Entity<BgtContractCommitment>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<BgtContractCommitment>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 04. bgt_actual_cost_log
            modelBuilder.Entity<BgtActualCostLog>()
                .Property(e => e.CostAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtActualCostLog>()
                .Property(e => e.TaxAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtActualCostLog>()
                .Property(e => e.TotalCostAmount).HasDefaultValue(0.00m);

            // 05. bgt_budget_reallocation
            modelBuilder.Entity<BgtBudgetReallocation>()
                .Property(e => e.TransferAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<BgtBudgetReallocation>()
                .Property(e => e.TransferStatus).HasDefaultValue("SUBMITTED");
            modelBuilder.Entity<BgtBudgetReallocation>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<BgtBudgetReallocation>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            #endregion
        }
    }
}