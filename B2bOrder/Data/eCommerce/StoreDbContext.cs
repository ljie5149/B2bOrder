using Microsoft.EntityFrameworkCore;
using B2bOrder.Models;

namespace B2bOrder.Data
{
    /// <summary>
    /// Store 資料庫上下文 / Store Database Context
    /// </summary>
    public class ConstructionWMSDbContext : DbContext
    {
        public ConstructionWMSDbContext(DbContextOptions<ConstructionWMSDbContext> options) : base(options)
        {
        }

        #region DbSet 宣告 / DbSet Declarations

        /// <summary>
        /// 商店主檔 / Stores
        /// </summary>
        public DbSet<Store> Stores { get; set; } = null!;

        /// <summary>
        /// 商店設定 / Store Settings
        /// </summary>
        public DbSet<StoreSetting> StoreSettings { get; set; } = null!;

        /// <summary>
        /// 商店銀行帳戶 / Store Bank Accounts
        /// </summary>
        public DbSet<StoreBank> StoreBanks { get; set; } = null!;

        /// <summary>
        /// 商店員工 / Store Staffs
        /// </summary>
        public DbSet<StoreStaff> StoreStaffs { get; set; } = null!;

        /// <summary>
        /// 商店員工角色 / Store Staff Roles
        /// </summary>
        public DbSet<StoreStaffRole> StoreStaffRoles { get; set; } = null!;

        /// <summary>
        /// 商店商品 / Store Products
        /// </summary>
        public DbSet<StoreProduct> StoreProducts { get; set; } = null!;

        /// <summary>
        /// 商店SKU設定 / Store Product SKUs
        /// </summary>
        public DbSet<StoreProductSku> StoreProductSkus { get; set; } = null!;

        /// <summary>
        /// 商店商品價格 / Store Product Prices
        /// </summary>
        public DbSet<StoreProductPrice> StoreProductPrices { get; set; } = null!;

        /// <summary>
        /// 商店SKU可用倉庫 / Store Product Warehouses
        /// </summary>
        public DbSet<StoreProductWarehouse> StoreProductWarehouses { get; set; } = null!;

        /// <summary>
        /// 商店子訂單 / Store Orders
        /// </summary>
        public DbSet<StoreOrder> StoreOrders { get; set; } = null!;

        /// <summary>
        /// 商店訂單明細 / Store Order Items
        /// </summary>
        public DbSet<StoreOrderItem> StoreOrderItems { get; set; } = null!;

        /// <summary>
        /// 商店結算單 / Store Settlements
        /// </summary>
        public DbSet<StoreSettlement> StoreSettlements { get; set; } = null!;

        /// <summary>
        /// 商店結算明細 / Store Settlement Items
        /// </summary>
        public DbSet<StoreSettlementItem> StoreSettlementItems { get; set; } = null!;

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region 索引與限制條件設定 / Indexes and Constraints Configuration

            // sto_store
            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.StoreNo).IsUnique().HasDatabaseName("uk_ss_store_no");
                entity.HasIndex(e => e.StoreTypeSid).HasDatabaseName("idx_ss_store_type_sid");
                entity.HasIndex(e => e.OwnerMemberSid).HasDatabaseName("idx_ss_owner_member_sid");
                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_ss_company_sid");
                entity.HasIndex(e => e.DefaultWarehouseSid).HasDatabaseName("idx_ss_default_warehouse_sid");
                entity.HasIndex(e => e.StoreStatus).HasDatabaseName("idx_ss_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_ss_avalible");
                entity.ToTable(tb => tb.HasCheckConstraint("CK_Store_PlatformFeeRate", "platform_fee_rate >= 0"));
            });

            // sto_store_setting
            modelBuilder.Entity<StoreSetting>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StoreNid, e.SettingKey }).IsUnique().HasDatabaseName("uk_sss_store_key");
                entity.HasIndex(e => e.StoreNid).HasDatabaseName("idx_sss_store_nid");
                entity.HasIndex(e => e.Category).HasDatabaseName("idx_sss_category");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_sss_avalible");
                entity.HasOne<Store>().WithMany().HasForeignKey(d => d.StoreNid).HasConstraintName("fk_sss_store");
            });

            // sto_store_bank
            modelBuilder.Entity<StoreBank>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.StoreNid).HasDatabaseName("idx_ssb_store_nid");
                entity.HasIndex(e => e.BankCode).HasDatabaseName("idx_ssb_bank_code");
                entity.HasIndex(e => e.IsDefault).HasDatabaseName("idx_ssb_default");
                entity.HasIndex(e => e.VerificationStatus).HasDatabaseName("idx_ssb_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_ssb_avalible");
                entity.HasOne<Store>().WithMany().HasForeignKey(d => d.StoreNid).HasConstraintName("fk_ssb_store");
            });

            // sto_staff
            modelBuilder.Entity<StoreStaff>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StoreNid, e.UserSid }).IsUnique().HasDatabaseName("uk_ssf_store_user");
                entity.HasIndex(e => e.StoreNid).HasDatabaseName("idx_ssf_store_nid");
                entity.HasIndex(e => e.UserSid).HasDatabaseName("idx_ssf_user_sid");
                entity.HasIndex(e => e.StaffStatus).HasDatabaseName("idx_ssf_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_ssf_avalible");
                entity.HasOne<Store>().WithMany().HasForeignKey(d => d.StoreNid).HasConstraintName("fk_ssf_store");
                entity.ToTable(tb => tb.HasCheckConstraint("CK_StoreStaff_LeaveDate", "leave_date IS NULL OR join_date IS NULL OR leave_date >= join_date"));
            });

            // sto_staff_role
            modelBuilder.Entity<StoreStaffRole>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StaffNid, e.RoleSid }).IsUnique().HasDatabaseName("uk_ssr_staff_role");
                entity.HasIndex(e => e.StaffNid).HasDatabaseName("idx_ssr_staff_nid");
                entity.HasIndex(e => e.RoleSid).HasDatabaseName("idx_ssr_role_sid");
                entity.HasIndex(e => new { e.StartDate, e.EndDate }).HasDatabaseName("idx_ssr_date");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_ssr_avalible");
                entity.HasOne<StoreStaff>().WithMany().HasForeignKey(d => d.StaffNid).HasConstraintName("fk_ssr_staff");
                entity.ToTable(tb => tb.HasCheckConstraint("CK_StoreStaffRole_EndDate", "end_date IS NULL OR start_date IS NULL OR end_date >= start_date"));
            });

            // sto_product
            modelBuilder.Entity<StoreProduct>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StoreNid, e.ProductSid }).IsUnique().HasDatabaseName("uk_stp_store_product");
                entity.HasIndex(e => e.StoreNid).HasDatabaseName("idx_stp_store_nid");
                entity.HasIndex(e => e.ProductSid).HasDatabaseName("idx_stp_product_sid");
                entity.HasIndex(e => e.ShelfStatus).HasDatabaseName("idx_stp_shelf_status");
                entity.HasIndex(e => e.IsFeatured).HasDatabaseName("idx_stp_featured");
                entity.HasIndex(e => new { e.StartDate, e.EndDate }).HasDatabaseName("idx_stp_date");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_stp_avalible");
                entity.HasOne<Store>().WithMany().HasForeignKey(d => d.StoreNid).HasConstraintName("fk_stp_store");
            });

            // sto_product_sku
            modelBuilder.Entity<StoreProductSku>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StoreProductNid, e.SkuSid }).IsUnique().HasDatabaseName("uk_stps_store_product_sku");
                entity.HasIndex(e => e.StoreProductNid).HasDatabaseName("idx_stps_store_product_nid");
                entity.HasIndex(e => e.SkuSid).HasDatabaseName("idx_stps_sku_sid");
                entity.HasIndex(e => e.SaleStatus).HasDatabaseName("idx_stps_sale_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_stps_avalible");
                entity.HasOne<StoreProduct>().WithMany().HasForeignKey(d => d.StoreProductNid).HasConstraintName("fk_stps_store_product");
                entity.ToTable(tb => tb.HasCheckConstraint("CK_StoreProductSku_MaxSaleQty", "max_sale_qty IS NULL OR max_sale_qty > 0"));
            });

            // sto_product_price
            modelBuilder.Entity<StoreProductPrice>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StoreNid, e.SkuSid, e.PriceGroupSid, e.CurrencySid, e.StartDate }).IsUnique().HasDatabaseName("uk_stpp_price");
                entity.HasIndex(e => e.StoreNid).HasDatabaseName("idx_stpp_store_nid");
                entity.HasIndex(e => e.StoreProductNid).HasDatabaseName("idx_stpp_store_product_nid");
                entity.HasIndex(e => e.SkuSid).HasDatabaseName("idx_stpp_sku_sid");
                entity.HasIndex(e => e.PriceGroupSid).HasDatabaseName("idx_stpp_price_group_sid");
                entity.HasIndex(e => new { e.StartDate, e.EndDate }).HasDatabaseName("idx_stpp_effective");
                entity.HasIndex(e => e.Priority).HasDatabaseName("idx_stpp_priority");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_stpp_avalible");
                entity.HasOne<Store>().WithMany().HasForeignKey(d => d.StoreNid).HasConstraintName("fk_stpp_store");
                entity.HasOne<StoreProduct>().WithMany().HasForeignKey(d => d.StoreProductNid).HasConstraintName("fk_stpp_store_product");
                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_StoreProductPrice_Price", "price >= 0");
                    tb.HasCheckConstraint("CK_StoreProductPrice_Date", "end_date IS NULL OR start_date IS NULL OR end_date >= start_date");
                });
            });

            // sto_product_warehouse
            modelBuilder.Entity<StoreProductWarehouse>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StoreNid, e.SkuSid, e.WarehouseSid }).IsUnique().HasDatabaseName("uk_stpw_store_sku_warehouse");
                entity.HasIndex(e => e.StoreNid).HasDatabaseName("idx_stpw_store_nid");
                entity.HasIndex(e => e.SkuSid).HasDatabaseName("idx_stpw_sku_sid");
                entity.HasIndex(e => e.WarehouseSid).HasDatabaseName("idx_stpw_warehouse_sid");
                entity.HasIndex(e => e.IsPrimary).HasDatabaseName("idx_stpw_primary");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_stpw_avalible");
                entity.HasOne<Store>().WithMany().HasForeignKey(d => d.StoreNid).HasConstraintName("fk_stpw_store");
            });

            // sto_order
            modelBuilder.Entity<StoreOrder>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.StoreOrderNo).IsUnique().HasDatabaseName("uk_sto_store_order_no");
                entity.HasIndex(e => new { e.StoreNid, e.ShoppingOrderSid }).IsUnique().HasDatabaseName("uk_sto_store_shopping_order");
                entity.HasIndex(e => e.StoreNid).HasDatabaseName("idx_sto_store_nid");
                entity.HasIndex(e => e.ShoppingOrderSid).HasDatabaseName("idx_sto_shopping_order_sid");
                entity.HasIndex(e => e.OrderStatus).HasDatabaseName("idx_sto_order_status");
                entity.HasIndex(e => e.WarehouseSid).HasDatabaseName("idx_sto_warehouse_sid");
                entity.HasIndex(e => e.CreateDate).HasDatabaseName("idx_sto_create_date");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_sto_avalible");
                entity.HasOne<Store>().WithMany().HasForeignKey(d => d.StoreNid).HasConstraintName("fk_sto_store");
                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_StoreOrder_Subtotal", "subtotal_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreOrder_Discount", "discount_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreOrder_Freight", "freight_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreOrder_PlatformFee", "platform_fee_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreOrder_Payable", "payable_amount >= 0");
                });
            });

            // sto_order_item
            modelBuilder.Entity<StoreOrderItem>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StoreOrderNid, e.ShoppingOrderItemSid }).IsUnique().HasDatabaseName("uk_stoi_order_item");
                entity.HasIndex(e => e.StoreOrderNid).HasDatabaseName("idx_stoi_store_order_nid");
                entity.HasIndex(e => e.ShoppingOrderItemSid).HasDatabaseName("idx_stoi_shopping_order_item_sid");
                entity.HasIndex(e => e.ProductSid).HasDatabaseName("idx_stoi_product_sid");
                entity.HasIndex(e => e.SkuSid).HasDatabaseName("idx_stoi_sku_sid");
                entity.HasIndex(e => e.ItemStatus).HasDatabaseName("idx_stoi_status");
                entity.HasOne<StoreOrder>().WithMany().HasForeignKey(d => d.StoreOrderNid).HasConstraintName("fk_stoi_store_order");
                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_StoreOrderItem_Quantity", "quantity > 0");
                    tb.HasCheckConstraint("CK_StoreOrderItem_UnitPrice", "unit_price >= 0");
                    tb.HasCheckConstraint("CK_StoreOrderItem_Discount", "discount_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreOrderItem_LineAmount", "line_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreOrderItem_PlatformFee", "platform_fee_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreOrderItem_Settlement", "settlement_amount >= 0");
                });
            });

            // sto_settlement
            modelBuilder.Entity<StoreSettlement>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SettlementNo).IsUnique().HasDatabaseName("uk_sts_settlement_no");
                entity.HasIndex(e => new { e.StoreNid, e.PeriodStartDate, e.PeriodEndDate }).IsUnique().HasDatabaseName("uk_sts_store_period");
                entity.HasIndex(e => e.StoreNid).HasDatabaseName("idx_sts_store_nid");
                entity.HasIndex(e => new { e.PeriodStartDate, e.PeriodEndDate }).HasDatabaseName("idx_sts_period");
                entity.HasIndex(e => e.SettlementStatus).HasDatabaseName("idx_sts_status");
                entity.HasIndex(e => e.PaidDate).HasDatabaseName("idx_sts_paid_date");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_sts_avalible");
                entity.HasOne<Store>().WithMany().HasForeignKey(d => d.StoreNid).HasConstraintName("fk_sts_store");
                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_StoreSettlement_Period", "period_end_date >= period_start_date");
                    tb.HasCheckConstraint("CK_StoreSettlement_Gross", "gross_sales_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreSettlement_Refund", "refund_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreSettlement_Discount", "discount_share_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreSettlement_PlatformFee", "platform_fee_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreSettlement_PaymentFee", "payment_fee_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreSettlement_ShippingFee", "shipping_fee_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreSettlement_Payable", "payable_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreSettlement_Paid", "paid_amount >= 0");
                });
            });

            // sto_settlement_item
            modelBuilder.Entity<StoreSettlementItem>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.SettlementNid, e.SourceType, e.SourceSid }).IsUnique().HasDatabaseName("uk_stsi_source");
                entity.HasIndex(e => e.SettlementNid).HasDatabaseName("idx_stsi_settlement_nid");
                entity.HasIndex(e => e.StoreOrderSid).HasDatabaseName("idx_stsi_store_order_sid");
                entity.HasIndex(e => new { e.SourceType, e.SourceSid }).HasDatabaseName("idx_stsi_source");
                entity.HasIndex(e => e.TransactionDate).HasDatabaseName("idx_stsi_transaction_date");
                entity.HasOne<StoreSettlement>().WithMany().HasForeignKey(d => d.SettlementNid).HasConstraintName("fk_stsi_settlement");
                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_StoreSettlementItem_Gross", "gross_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreSettlementItem_Deduction", "deduction_amount >= 0");
                    tb.HasCheckConstraint("CK_StoreSettlementItem_Settlement", "settlement_amount >= 0");
                });
            });

            #endregion
        }
    }
}