using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.ShareCore.Construction.Models;

namespace B2bOrder.Resources.ShareCore.Construction
{
    public class MimDbContext : DbContext
    {
        public MimDbContext(DbContextOptions<MimDbContext> options) : base(options)
        {
        }

        #region DbSet Configurations - 資料集設定

        public DbSet<MimMaterialMaster> MimMaterialMasters { get; set; }
        public DbSet<MimInventoryStock> MimInventoryStocks { get; set; }
        public DbSet<MimInventoryTransaction> MimInventoryTransactions { get; set; }
        public DbSet<MimStockTaking> MimStockTakings { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Index Configurations - 唯一索引設定

            // 01. mim_material_master
            modelBuilder.Entity<MimMaterialMaster>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<MimMaterialMaster>()
                .HasIndex(e => e.MaterialCode).IsUnique();

            // 02. mim_inventory_stock
            modelBuilder.Entity<MimInventoryStock>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<MimInventoryStock>()
                .HasIndex(e => new { e.ProjectSid, e.WarehouseCode, e.MaterialSid, e.LocationBin })
                .IsUnique()
                .HasDatabaseName("uk_mis_project_wh_material");

            // 03. mim_inventory_transaction
            modelBuilder.Entity<MimInventoryTransaction>()
                .HasIndex(e => e.Sid).IsUnique();

            // 04. mim_stock_taking
            modelBuilder.Entity<MimStockTaking>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<MimStockTaking>()
                .HasIndex(e => e.AuditNo)
                .IsUnique()
                .HasDatabaseName("uk_mst_audit_no");

            #endregion

            #region Composite Index Configurations - 複合索引與一般索引設定

            // 01. mim_material_master
            modelBuilder.Entity<MimMaterialMaster>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<MimMaterialMaster>()
                .HasIndex(e => e.Category);
            modelBuilder.Entity<MimMaterialMaster>()
                .HasIndex(e => e.PccesCode);
            modelBuilder.Entity<MimMaterialMaster>()
                .HasIndex(e => e.Avalible);

            // 02. mim_inventory_stock
            modelBuilder.Entity<MimInventoryStock>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<MimInventoryStock>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<MimInventoryStock>()
                .HasIndex(e => e.MaterialSid);
            modelBuilder.Entity<MimInventoryStock>()
                .HasIndex(e => e.Avalible);

            // 03. mim_inventory_transaction
            modelBuilder.Entity<MimInventoryTransaction>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<MimInventoryTransaction>()
                .HasIndex(e => e.MaterialSid);
            modelBuilder.Entity<MimInventoryTransaction>()
                .HasIndex(e => e.TxnType);
            modelBuilder.Entity<MimInventoryTransaction>()
                .HasIndex(e => e.RefPoSid);

            // 04. mim_stock_taking
            modelBuilder.Entity<MimStockTaking>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<MimStockTaking>()
                .HasIndex(e => e.AuditStatus);
            modelBuilder.Entity<MimStockTaking>()
                .HasIndex(e => e.Avalible);

            #endregion

            #region Default Value Configurations - 預設值設定

            // 01. mim_material_master
            modelBuilder.Entity<MimMaterialMaster>()
                .Property(e => e.StandardUnitPrice).HasDefaultValue(0.00m);
            modelBuilder.Entity<MimMaterialMaster>()
                .Property(e => e.StandardWastageRate).HasDefaultValue(0.00m);
            modelBuilder.Entity<MimMaterialMaster>()
                .Property(e => e.IsHazmat).HasDefaultValue("N");
            modelBuilder.Entity<MimMaterialMaster>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<MimMaterialMaster>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 02. mim_inventory_stock
            modelBuilder.Entity<MimInventoryStock>()
                .Property(e => e.QtyOnHand).HasDefaultValue(0.0000m);
            modelBuilder.Entity<MimInventoryStock>()
                .Property(e => e.QtyAllocated).HasDefaultValue(0.0000m);
            modelBuilder.Entity<MimInventoryStock>()
                .Property(e => e.QtyAvailable).HasDefaultValue(0.0000m);
            modelBuilder.Entity<MimInventoryStock>()
                .Property(e => e.MinSafetyQty).HasDefaultValue(0.0000m);
            modelBuilder.Entity<MimInventoryStock>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<MimInventoryStock>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 03. mim_inventory_transaction
            modelBuilder.Entity<MimInventoryTransaction>()
                .Property(e => e.UnitPrice).HasDefaultValue(0.00m);
            modelBuilder.Entity<MimInventoryTransaction>()
                .Property(e => e.TotalAmount).HasDefaultValue(0.00m);

            // 04. mim_stock_taking
            modelBuilder.Entity<MimStockTaking>()
                .Property(e => e.AuditStatus).HasDefaultValue("DRAFT");
            modelBuilder.Entity<MimStockTaking>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<MimStockTaking>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            #endregion
        }
    }
}