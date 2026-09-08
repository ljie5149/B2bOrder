using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.ERP.eCommerce.Models;

namespace B2bOrder.Resources.ERP.eCommerce.Data
{
    /// <summary>
    /// WMSDB 資料庫上下文 / WMSDB Database Context
    /// </summary>
    public class WMSDbContext : DbContext
    {
        public WMSDbContext(DbContextOptions<WMSDbContext> options) : base(options)
        {
        }

        #region DbSet 宣告 / DbSet Declarations

        /// <summary>
        /// 倉庫主檔 / Warehouses
        /// </summary>
        public DbSet<Warehouse> Warehouses { get; set; } = null!;

        /// <summary>
        /// 倉庫儲位檔 / Locations
        /// </summary>
        public DbSet<Location> Locations { get; set; } = null!;

        /// <summary>
        /// 儲位庫存帳 / Stocks
        /// </summary>
        public DbSet<Stock> Stocks { get; set; } = null!;

        /// <summary>
        /// 單件商品序列號追蹤 / Stock Serials
        /// </summary>
        public DbSet<StockSerial> StockSerials { get; set; } = null!;

        /// <summary>
        /// 進貨入庫單 / Inbound Orders
        /// </summary>
        public DbSet<InboundOrder> InboundOrders { get; set; } = null!;

        /// <summary>
        /// 進貨入庫明細 / Inbound Items
        /// </summary>
        public DbSet<InboundItem> InboundItems { get; set; } = null!;

        /// <summary>
        /// 出貨單檔 / Outbound Orders
        /// </summary>
        public DbSet<OutboundOrder> OutboundOrders { get; set; } = null!;

        /// <summary>
        /// 出貨單明細 / Outbound Items
        /// </summary>
        public DbSet<OutboundItem> OutboundItems { get; set; } = null!;

        /// <summary>
        /// 庫內移庫/跨倉調撥單 / Transfer Orders
        /// </summary>
        public DbSet<TransferOrder> TransferOrders { get; set; } = null!;

        /// <summary>
        /// 庫存盤點單 / Stocktakes
        /// </summary>
        public DbSet<Stocktake> Stocktakes { get; set; } = null!;

        /// <summary>
        /// 庫存異動流水帳 / Inventory Ledgers
        /// </summary>
        public DbSet<InventoryLedger> InventoryLedgers { get; set; } = null!;

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region 索引與限制條件設定 / Indexes and Constraints Configuration

            // 01. 倉庫與儲位基礎架構 / Warehouse & Storage Location Infrastructure
            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.CompanySid, e.WarehouseCode }).IsUnique().HasDatabaseName("uk_ww_warehouse_code");
                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_ww_company_sid");
                entity.HasIndex(e => e.WarehouseType).HasDatabaseName("idx_ww_type");
                entity.HasIndex(e => e.WarehouseStatus).HasDatabaseName("idx_ww_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_ww_avalible");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.WarehouseNid, e.LocationCode }).IsUnique().HasDatabaseName("uk_wl_loc_code");
                entity.HasIndex(e => e.WarehouseNid).HasDatabaseName("idx_wl_warehouse_nid");
                entity.HasIndex(e => e.ZoneCode).HasDatabaseName("idx_wl_zone_code");
                entity.HasIndex(e => e.LocationType).HasDatabaseName("idx_wl_location_type");
                entity.HasIndex(e => e.LocationStatus).HasDatabaseName("idx_wl_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wl_avalible");

                entity.HasOne<Warehouse>()
                      .WithMany()
                      .HasForeignKey(e => e.WarehouseNid)
                      .HasConstraintName("fk_wl_warehouse");
            });

            // 02. 庫存現有帳與批號序列號 / Stock Inventory & Lot/Serial Tracking
            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.WarehouseNid, e.LocationNid, e.ItemSid, e.VariantSid, e.LotNo, e.StockStatus })
                      .IsUnique()
                      .HasDatabaseName("uk_ws_stock_item");
                entity.HasIndex(e => e.WarehouseNid).HasDatabaseName("idx_ws_warehouse_nid");
                entity.HasIndex(e => e.LocationNid).HasDatabaseName("idx_ws_location_nid");
                entity.HasIndex(e => new { e.ItemSid, e.VariantSid }).HasDatabaseName("idx_ws_item_variant");
                entity.HasIndex(e => e.LotNo).HasDatabaseName("idx_ws_lot_no");
                entity.HasIndex(e => e.ExpirationDate).HasDatabaseName("idx_ws_expiration_date");
                entity.HasIndex(e => e.StockStatus).HasDatabaseName("idx_ws_status");

                entity.Property(e => e.AvailableQty)
                      .HasComputedColumnSql("(`on_hand_qty` - `allocated_qty`)", stored: true);

                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_Stock_OnHand", "on_hand_qty >= 0");
                    tb.HasCheckConstraint("CK_Stock_Allocated", "allocated_qty >= 0");
                    tb.HasCheckConstraint("CK_Stock_Allocated_Le_OnHand", "allocated_qty <= on_hand_qty");
                });

                entity.HasOne<Warehouse>()
                      .WithMany()
                      .HasForeignKey(e => e.WarehouseNid)
                      .HasConstraintName("fk_ws_warehouse");

                entity.HasOne<Location>()
                      .WithMany()
                      .HasForeignKey(e => e.LocationNid)
                      .HasConstraintName("fk_ws_location");
            });

            modelBuilder.Entity<StockSerial>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.ItemSid, e.SerialNo }).IsUnique().HasDatabaseName("uk_wss_item_serial");
                entity.HasIndex(e => e.StockNid).HasDatabaseName("idx_wss_stock_nid");
                entity.HasIndex(e => e.SerialNo).HasDatabaseName("idx_wss_serial_no");
                entity.HasIndex(e => e.SerialStatus).HasDatabaseName("idx_wss_status");

                entity.HasOne<Stock>()
                      .WithMany()
                      .HasForeignKey(e => e.StockNid)
                      .HasConstraintName("fk_wss_stock");
            });

            // 03. 進貨管理 / Inbound Management (Goods Receipt)
            modelBuilder.Entity<InboundOrder>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.InboundNo).IsUnique().HasDatabaseName("uk_wio_inbound_no");
                entity.HasIndex(e => e.PoSid).HasDatabaseName("idx_wio_po_sid");
                entity.HasIndex(e => e.WarehouseNid).HasDatabaseName("idx_wio_warehouse_nid");
                entity.HasIndex(e => e.SupplierPartySid).HasDatabaseName("idx_wio_supplier");
                entity.HasIndex(e => e.InboundStatus).HasDatabaseName("idx_wio_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wio_avalible");

                entity.HasOne<Warehouse>()
                      .WithMany()
                      .HasForeignKey(e => e.WarehouseNid)
                      .HasConstraintName("fk_wio_warehouse");
            });

            modelBuilder.Entity<InboundItem>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.InboundNid, e.LineNo }).IsUnique().HasDatabaseName("uk_wii_inbound_line");
                entity.HasIndex(e => e.InboundNid).HasDatabaseName("idx_wii_inbound_nid");
                entity.HasIndex(e => new { e.ItemSid, e.VariantSid }).HasDatabaseName("idx_wii_item_variant");
                entity.HasIndex(e => e.LotNo).HasDatabaseName("idx_wii_lot_no");
                entity.HasIndex(e => e.ItemStatus).HasDatabaseName("idx_wii_status");

                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_InboundItem_LineNo", "line_no > 0");
                    tb.HasCheckConstraint("CK_InboundItem_ExpectedQty", "expected_qty > 0");
                    tb.HasCheckConstraint("CK_InboundItem_ReceivedQty", "received_qty >= 0");
                    tb.HasCheckConstraint("CK_InboundItem_QcPassQty", "qc_pass_qty >= 0");
                    tb.HasCheckConstraint("CK_InboundItem_QcFailQty", "qc_fail_qty >= 0");
                    tb.HasCheckConstraint("CK_InboundItem_PutawayQty", "putaway_qty >= 0");
                });

                entity.HasOne<InboundOrder>()
                      .WithMany()
                      .HasForeignKey(e => e.InboundNid)
                      .HasConstraintName("fk_wii_inbound");

                entity.HasOne<Location>()
                      .WithMany()
                      .HasForeignKey(e => e.TargetLocationNid)
                      .HasConstraintName("fk_wii_location");
            });

            // 04. 出貨與揀貨管理 / Outbound & Picking Management
            modelBuilder.Entity<OutboundOrder>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.OutboundNo).IsUnique().HasDatabaseName("uk_woo_outbound_no");
                entity.HasIndex(e => e.FulfillmentRequestSid).HasDatabaseName("idx_woo_fulfillment_sid");
                entity.HasIndex(e => e.SalesOrderSid).HasDatabaseName("idx_woo_sales_order_sid");
                entity.HasIndex(e => e.WarehouseNid).HasDatabaseName("idx_woo_warehouse_nid");
                entity.HasIndex(e => e.OutboundStatus).HasDatabaseName("idx_woo_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_woo_avalible");

                entity.HasOne<Warehouse>()
                      .WithMany()
                      .HasForeignKey(e => e.WarehouseNid)
                      .HasConstraintName("fk_woo_warehouse");
            });

            modelBuilder.Entity<OutboundItem>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.OutboundNid, e.LineNo }).IsUnique().HasDatabaseName("uk_woi_outbound_line");
                entity.HasIndex(e => e.OutboundNid).HasDatabaseName("idx_woi_outbound_nid");
                entity.HasIndex(e => new { e.ItemSid, e.VariantSid }).HasDatabaseName("idx_woi_item_variant");
                entity.HasIndex(e => e.ItemStatus).HasDatabaseName("idx_woi_status");

                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_OutboundItem_LineNo", "line_no > 0");
                    tb.HasCheckConstraint("CK_OutboundItem_PlanQty", "plan_qty > 0");
                    tb.HasCheckConstraint("CK_OutboundItem_PickedQty", "picked_qty >= 0");
                    tb.HasCheckConstraint("CK_OutboundItem_ShippedQty", "shipped_qty >= 0");
                });

                entity.HasOne<OutboundOrder>()
                      .WithMany()
                      .HasForeignKey(e => e.OutboundNid)
                      .HasConstraintName("fk_woi_outbound");

                entity.HasOne<Location>()
                      .WithMany()
                      .HasForeignKey(e => e.FromLocationNid)
                      .HasConstraintName("fk_woi_location");
            });

            // 05. 庫內異動與盤點 / Internal Transfer & Stocktake Management
            modelBuilder.Entity<TransferOrder>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.TransferNo).IsUnique().HasDatabaseName("uk_wto_transfer_no");
                entity.HasIndex(e => e.FromWarehouseNid).HasDatabaseName("idx_wto_from_wh");
                entity.HasIndex(e => e.ToWarehouseNid).HasDatabaseName("idx_wto_to_wh");
                entity.HasIndex(e => e.TransferStatus).HasDatabaseName("idx_wto_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wto_avalible");

                entity.HasOne<Warehouse>()
                      .WithMany()
                      .HasForeignKey(e => e.FromWarehouseNid)
                      .HasConstraintName("fk_wto_from_wh");

                entity.HasOne<Warehouse>()
                      .WithMany()
                      .HasForeignKey(e => e.ToWarehouseNid)
                      .HasConstraintName("fk_wto_to_wh");
            });

            modelBuilder.Entity<Stocktake>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.StocktakeNo).IsUnique().HasDatabaseName("uk_wst_stocktake_no");
                entity.HasIndex(e => e.WarehouseNid).HasDatabaseName("idx_wst_warehouse_nid");
                entity.HasIndex(e => e.StocktakeStatus).HasDatabaseName("idx_wst_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wst_avalible");

                entity.HasOne<Warehouse>()
                      .WithMany()
                      .HasForeignKey(e => e.WarehouseNid)
                      .HasConstraintName("fk_wst_warehouse");
            });

            // 06. 庫存履歷與異動流水帳 / Inventory Ledger & History Tracking
            modelBuilder.Entity<InventoryLedger>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.WarehouseNid, e.LocationNid }).HasDatabaseName("idx_wil_warehouse_loc");
                entity.HasIndex(e => new { e.ItemSid, e.VariantSid }).HasDatabaseName("idx_wil_item_variant");
                entity.HasIndex(e => e.TransactionType).HasDatabaseName("idx_wil_tx_type");
                entity.HasIndex(e => new { e.ReferenceDocType, e.ReferenceDocNo }).HasDatabaseName("idx_wil_ref_doc");
                entity.HasIndex(e => e.CreateDate).HasDatabaseName("idx_wil_create_date");

                entity.HasOne<Warehouse>()
                      .WithMany()
                      .HasForeignKey(e => e.WarehouseNid)
                      .HasConstraintName("fk_wil_warehouse");

                entity.HasOne<Location>()
                      .WithMany()
                      .HasForeignKey(e => e.LocationNid)
                      .HasConstraintName("fk_wil_location");
            });

            #endregion
        }
    }
}