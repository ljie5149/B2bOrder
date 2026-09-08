using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Construction.Models;

namespace B2bOrder.Resources.Construction
{
    public class ChangeOrderDbContext : DbContext
    {
        public ChangeOrderDbContext(DbContextOptions<ChangeOrderDbContext> options) : base(options)
        {
        }

        #region DbSet Configurations - 資料集設定

        public DbSet<ChoChangeOrderMaster> ChoChangeOrderMasters { get; set; }
        public DbSet<ChoChangeOrderLine> ChoChangeOrderLines { get; set; }
        public DbSet<ChoTimeExtensionAnalysis> ChoTimeExtensionAnalyses { get; set; }
        public DbSet<ChoApprovalLog> ChoApprovalLogs { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Index Configurations - 唯一索引設定

            // 01. cho_change_order_master
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .HasIndex(e => new { e.ContractSid, e.ChangeOrderNo })
                .IsUnique()
                .HasDatabaseName("uk_ccom_contract_number");

            // 02. cho_change_order_line
            modelBuilder.Entity<ChoChangeOrderLine>()
                .HasIndex(e => e.Sid).IsUnique();

            // 03. cho_time_extension_analysis
            modelBuilder.Entity<ChoTimeExtensionAnalysis>()
                .HasIndex(e => e.Sid).IsUnique();

            // 04. cho_approval_log
            modelBuilder.Entity<ChoApprovalLog>()
                .HasIndex(e => e.Sid).IsUnique();

            #endregion

            #region Composite Index Configurations - 複合索引與一般索引設定

            // 01. cho_change_order_master
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .HasIndex(e => e.VendorSid);
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .HasIndex(e => e.OrderStatus);
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .HasIndex(e => e.Avalible);

            // 02. cho_change_order_line
            modelBuilder.Entity<ChoChangeOrderLine>()
                .HasIndex(e => e.ChangeOrderSid);
            modelBuilder.Entity<ChoChangeOrderLine>()
                .HasIndex(e => e.WbsSid);
            modelBuilder.Entity<ChoChangeOrderLine>()
                .HasIndex(e => e.Avalible);

            // 03. cho_time_extension_analysis
            modelBuilder.Entity<ChoTimeExtensionAnalysis>()
                .HasIndex(e => e.ChangeOrderSid);
            modelBuilder.Entity<ChoTimeExtensionAnalysis>()
                .HasIndex(e => e.ImpactedWbsSid);
            modelBuilder.Entity<ChoTimeExtensionAnalysis>()
                .HasIndex(e => e.Avalible);

            // 04. cho_approval_log
            modelBuilder.Entity<ChoApprovalLog>()
                .HasIndex(e => e.ChangeOrderSid);
            modelBuilder.Entity<ChoApprovalLog>()
                .HasIndex(e => e.ReviewerUserSid);

            #endregion

            #region Default Value Configurations - 預設值設定

            // 01. cho_change_order_master
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .Property(e => e.ChangeReasonType).HasDefaultValue("DESIGN_CHANGE");
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .Property(e => e.OriginalContractAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .Property(e => e.ChangeAmountBeforeTax).HasDefaultValue(0.00m);
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .Property(e => e.TaxRate).HasDefaultValue(5.00m);
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .Property(e => e.ChangeTaxAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .Property(e => e.ChangeTotalAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .Property(e => e.RevisedContractAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .Property(e => e.IsTimeExtensionNeeded).HasDefaultValue("N");
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .Property(e => e.RequestedExtensionDays).HasDefaultValue(0);
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .Property(e => e.ApprovedExtensionDays).HasDefaultValue(0);
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .Property(e => e.OrderStatus).HasDefaultValue("DRAFT");
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<ChoChangeOrderMaster>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 02. cho_change_order_line
            modelBuilder.Entity<ChoChangeOrderLine>()
                .Property(e => e.ItemType).HasDefaultValue("REVISED_ITEM");
            modelBuilder.Entity<ChoChangeOrderLine>()
                .Property(e => e.OriginalQuantity).HasDefaultValue(0.0000m);
            modelBuilder.Entity<ChoChangeOrderLine>()
                .Property(e => e.ChangeQuantity).HasDefaultValue(0.0000m);
            modelBuilder.Entity<ChoChangeOrderLine>()
                .Property(e => e.RevisedQuantity).HasDefaultValue(0.0000m);
            modelBuilder.Entity<ChoChangeOrderLine>()
                .Property(e => e.UnitPrice).HasDefaultValue(0.00m);
            modelBuilder.Entity<ChoChangeOrderLine>()
                .Property(e => e.IsNewUnitPrice).HasDefaultValue("N");
            modelBuilder.Entity<ChoChangeOrderLine>()
                .Property(e => e.SubtotalAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<ChoChangeOrderLine>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<ChoChangeOrderLine>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 03. cho_time_extension_analysis
            modelBuilder.Entity<ChoTimeExtensionAnalysis>()
                .Property(e => e.AppliedDays).HasDefaultValue(0);
            modelBuilder.Entity<ChoTimeExtensionAnalysis>()
                .Property(e => e.ApprovedDays).HasDefaultValue(0);
            modelBuilder.Entity<ChoTimeExtensionAnalysis>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<ChoTimeExtensionAnalysis>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            #endregion
        }
    }
}