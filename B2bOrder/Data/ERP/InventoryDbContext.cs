using B2bOrder.Resources.eCommerce.Models;
using B2bOrder.Resources.ShareCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace B2bOrder.Resources.eCommerce
{
    /// <summary>
    /// InventoryDB 資料庫上下文 (Inventory Database Context)
    /// </summary>
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        // 01. 庫存政策與 Item 庫存設定 (Inventory Policy and Item Inventory Settings)
        public DbSet<InvPolicy> InvPolicies { get; set; }
        public DbSet<InvItemSetting> InvItemSettings { get; set; }

        // 02. 即時庫存帳 (Real-Time Inventory Accounts)
        public DbSet<InvStockBalance> InvStockBalances { get; set; }
        public DbSet<InvStockSummary> InvStockSummaries { get; set; }

        // 03. 批號與序號 (Batches and Serials)
        public DbSet<InvBatch> InvBatches { get; set; }
        public DbSet<InvSerial> InvSerials { get; set; }

        // 04. 庫存異動與成本 (Inventory Transactions and Costs)
        public DbSet<InvTransaction> InvTransactions { get; set; }
        public DbSet<InvCostLayer> InvCostLayers { get; set; }

        // 05. 預留與配貨 (Reservations and Allocations)
        public DbSet<InvReservation> InvReservations { get; set; }
        public DbSet<InvReservationItem> InvReservationItems { get; set; }
        public DbSet<InvAllocation> InvAllocations { get; set; }

        // 06. 調撥 (Transfers)
        public DbSet<InvTransfer> InvTransfers { get; set; }
        public DbSet<InvTransferItem> InvTransferItems { get; set; }

        // 07. 盤點與調整 (Stock Counts and Adjustments)
        public DbSet<InvStockCount> InvStockCounts { get; set; }
        public DbSet<InvStockCountItem> InvStockCountItems { get; set; }
        public DbSet<InvAdjustment> InvAdjustments { get; set; }
        public DbSet<InvAdjustmentItem> InvAdjustmentItems { get; set; }

        // 08. 領料、退料與耗用 (Material Issues, Returns, and Consumptions)
        public DbSet<InvIssueRequest> InvIssueRequests { get; set; }
        public DbSet<InvIssueRequestItem> InvIssueRequestItems { get; set; }
        public DbSet<InvMaterialReturn> InvMaterialReturns { get; set; }
        public DbSet<InvMaterialReturnItem> InvMaterialReturnItems { get; set; }
        public DbSet<InvConsumption> InvConsumptions { get; set; }
        public DbSet<InvConsumptionItem> InvConsumptionItems { get; set; }

        // 09. 報廢與召回 (Scraps and Recalls)
        public DbSet<InvScrap> InvScraps { get; set; }
        public DbSet<InvScrapItem> InvScrapItems { get; set; }
        public DbSet<InvRecall> InvRecalls { get; set; }

        // 10. 庫存事件與歷程 (Inventory Events and History)
        public DbSet<InvStatusHistory> InvStatusHistories { get; set; }
        public DbSet<InvEvent> InvEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 設置主鍵、唯一約束與關聯對應設定
            modelBuilder.Entity<InvPolicy>().ToTable("inv_policy");
            modelBuilder.Entity<InvItemSetting>().ToTable("inv_item_setting");

            modelBuilder.Entity<InvStockBalance>().ToTable("inv_stock_balance");
            modelBuilder.Entity<InvStockSummary>().ToTable("inv_stock_summary");

            modelBuilder.Entity<InvBatch>().ToTable("inv_batch");
            modelBuilder.Entity<InvSerial>().ToTable("inv_serial");

            modelBuilder.Entity<InvTransaction>().ToTable("inv_transaction");
            modelBuilder.Entity<InvCostLayer>().ToTable("inv_cost_layer");

            modelBuilder.Entity<InvReservation>().ToTable("inv_reservation");
            modelBuilder.Entity<InvReservationItem>().ToTable("inv_reservation_item")
                .HasOne(e => e.Reservation)
                .WithMany(e => e.ReservationItems)
                .HasForeignKey(e => e.ReservationNid);

            modelBuilder.Entity<InvAllocation>().ToTable("inv_allocation");

            modelBuilder.Entity<InvTransfer>().ToTable("inv_transfer");
            modelBuilder.Entity<InvTransferItem>().ToTable("inv_transfer_item")
                .HasOne(e => e.Transfer)
                .WithMany(e => e.TransferItems)
                .HasForeignKey(e => e.TransferNid);

            modelBuilder.Entity<InvStockCount>().ToTable("inv_stock_count");
            modelBuilder.Entity<InvStockCountItem>().ToTable("inv_stock_count_item")
                .HasOne(e => e.StockCount)
                .WithMany(e => e.StockCountItems)
                .HasForeignKey(e => e.StockCountNid);

            modelBuilder.Entity<InvAdjustment>().ToTable("inv_adjustment");
            modelBuilder.Entity<InvAdjustmentItem>().ToTable("inv_adjustment_item")
                .HasOne(e => e.Adjustment)
                .WithMany(e => e.AdjustmentItems)
                .HasForeignKey(e => e.AdjustmentNid);

            modelBuilder.Entity<InvIssueRequest>().ToTable("inv_issue_request");
            modelBuilder.Entity<InvIssueRequestItem>().ToTable("inv_issue_request_item")
                .HasOne(e => e.IssueRequest)
                .WithMany(e => e.IssueRequestItems)
                .HasForeignKey(e => e.IssueRequestNid);

            modelBuilder.Entity<InvMaterialReturn>().ToTable("inv_material_return");
            modelBuilder.Entity<InvMaterialReturnItem>().ToTable("inv_material_return_item")
                .HasOne(e => e.MaterialReturn)
                .WithMany(e => e.MaterialReturnItems)
                .HasForeignKey(e => e.MaterialReturnNid);

            modelBuilder.Entity<InvConsumption>().ToTable("inv_consumption");
            modelBuilder.Entity<InvConsumptionItem>().ToTable("inv_consumption_item")
                .HasOne(e => e.Consumption)
                .WithMany(e => e.ConsumptionItems)
                .HasForeignKey(e => e.ConsumptionNid);

            modelBuilder.Entity<InvScrap>().ToTable("inv_scrap");
            modelBuilder.Entity<InvScrapItem>().ToTable("inv_scrap_item")
                .HasOne(e => e.Scrap)
                .WithMany(e => e.ScrapItems)
                .HasForeignKey(e => e.ScrapNid);

            modelBuilder.Entity<InvRecall>().ToTable("inv_recall");
            modelBuilder.Entity<InvStatusHistory>().ToTable("inv_status_history");
            modelBuilder.Entity<InvEvent>().ToTable("inv_event");
        }
    }
}