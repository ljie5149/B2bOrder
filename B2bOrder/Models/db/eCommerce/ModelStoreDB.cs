using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models
{
    #region 商店主檔 / Store Main Model
    /// <summary>
    /// 商店主檔 / Store Main Model
    /// </summary>
    [Table("sto_store")]
    public class Store
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(50)]
        [Column("store_no")]
        public string StoreNo { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("store_type_sid")]
        public string StoreTypeSid { get; set; } = null!;

        [StringLength(32)]
        [Column("owner_member_sid")]
        public string? OwnerMemberSid { get; set; }

        [StringLength(32)]
        [Column("company_sid")]
        public string? CompanySid { get; set; }

        [Required]
        [StringLength(200)]
        [Column("store_name")]
        public string StoreName { get; set; } = null!;

        [StringLength(100)]
        [Column("store_short_name")]
        public string? StoreShortName { get; set; }

        [StringLength(500)]
        [Column("logo_url")]
        public string? LogoUrl { get; set; }

        [StringLength(500)]
        [Column("banner_url")]
        public string? BannerUrl { get; set; }

        [Column("description", TypeName = "longtext")]
        public string? Description { get; set; }

        [StringLength(20)]
        [Column("tax_no")]
        public string? TaxNo { get; set; }

        [StringLength(100)]
        [Column("representative")]
        public string? Representative { get; set; }

        [StringLength(100)]
        [Column("contact_name")]
        public string? ContactName { get; set; }

        [StringLength(30)]
        [Column("phone")]
        public string? Phone { get; set; }

        [StringLength(30)]
        [Column("mobile")]
        public string? Mobile { get; set; }

        [StringLength(200)]
        [Column("email")]
        public string? Email { get; set; }

        [StringLength(500)]
        [Column("website")]
        public string? Website { get; set; }

        [StringLength(32)]
        [Column("country_sid")]
        public string? CountrySid { get; set; }

        [StringLength(32)]
        [Column("city_sid")]
        public string? CitySid { get; set; }

        [StringLength(32)]
        [Column("district_sid")]
        public string? DistrictSid { get; set; }

        [StringLength(10)]
        [Column("postal_code")]
        public string? PostalCode { get; set; }

        [StringLength(500)]
        [Column("address")]
        public string? Address { get; set; }

        [StringLength(32)]
        [Column("default_warehouse_sid")]
        public string? DefaultWarehouseSid { get; set; }

        [StringLength(32)]
        [Column("default_currency_sid")]
        public string? DefaultCurrencySid { get; set; }

        [StringLength(32)]
        [Column("default_tax_sid")]
        public string? DefaultTaxSid { get; set; }

        [Required]
        [StringLength(30)]
        [Column("settlement_cycle")]
        public string SettlementCycle { get; set; } = "MONTHLY";

        [Column("platform_fee_rate", TypeName = "decimal(8,4)")]
        public decimal PlatformFeeRate { get; set; }

        [Required]
        [StringLength(30)]
        [Column("store_status")]
        public string StoreStatus { get; set; } = "PENDING";

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        [StringLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("sort_no")]
        public int SortNo { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 商店設定 / Store Setting Model
    /// <summary>
    /// 商店設定 / Store Setting Model
    /// </summary>
    [Table("sto_store_setting")]
    public class StoreSetting
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("store_nid")]
        public long StoreNid { get; set; }

        [Required]
        [StringLength(100)]
        [Column("setting_key")]
        public string SettingKey { get; set; } = null!;

        [Column("setting_value", TypeName = "longtext")]
        public string? SettingValue { get; set; }

        [Required]
        [StringLength(20)]
        [Column("setting_type")]
        public string SettingType { get; set; } = "TEXT";

        [StringLength(50)]
        [Column("category")]
        public string? Category { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 商店銀行帳戶 / Store Bank Account Model
    /// <summary>
    /// 商店銀行帳戶 / Store Bank Account Model
    /// </summary>
    [Table("sto_store_bank")]
    public class StoreBank
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("store_nid")]
        public long StoreNid { get; set; }

        [Required]
        [StringLength(20)]
        [Column("bank_code")]
        public string BankCode { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [Column("bank_name")]
        public string BankName { get; set; } = null!;

        [StringLength(20)]
        [Column("branch_code")]
        public string? BranchCode { get; set; }

        [StringLength(100)]
        [Column("branch_name")]
        public string? BranchName { get; set; }

        [Required]
        [StringLength(200)]
        [Column("account_name")]
        public string AccountName { get; set; } = null!;

        [Required]
        [StringLength(500)]
        [Column("account_no_encrypted")]
        public string AccountNoEncrypted { get; set; } = null!;

        [StringLength(32)]
        [Column("currency_sid")]
        public string? CurrencySid { get; set; }

        [Column("is_default")]
        public sbyte IsDefault { get; set; } = 0;

        [Required]
        [StringLength(30)]
        [Column("verification_status")]
        public string VerificationStatus { get; set; } = "PENDING";

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 商店員工 / Store Staff Model
    /// <summary>
    /// 商店員工 / Store Staff Model
    /// </summary>
    [Table("sto_staff")]
    public class StoreStaff
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("store_nid")]
        public long StoreNid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("user_sid")]
        public string UserSid { get; set; } = null!;

        [StringLength(50)]
        [Column("employee_no")]
        public string? EmployeeNo { get; set; }

        [Required]
        [StringLength(100)]
        [Column("staff_name")]
        public string StaffName { get; set; } = null!;

        [StringLength(200)]
        [Column("email")]
        public string? Email { get; set; }

        [StringLength(30)]
        [Column("mobile")]
        public string? Mobile { get; set; }

        [Column("join_date", TypeName = "date")]
        public DateTime? JoinDate { get; set; }

        [Column("leave_date", TypeName = "date")]
        public DateTime? LeaveDate { get; set; }

        [Required]
        [StringLength(30)]
        [Column("staff_status")]
        public string StaffStatus { get; set; } = "ACTIVE";

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 商店員工角色 / Store Staff Role Model
    /// <summary>
    /// 商店員工角色 / Store Staff Role Model
    /// </summary>
    [Table("sto_staff_role")]
    public class StoreStaffRole
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("staff_nid")]
        public long StaffNid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("role_sid")]
        public string RoleSid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 商店商品 / Store Product Model
    /// <summary>
    /// 商店商品 / Store Product Model
    /// </summary>
    [Table("sto_product")]
    public class StoreProduct
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("store_nid")]
        public long StoreNid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("product_sid")]
        public string ProductSid { get; set; } = null!;

        [StringLength(80)]
        [Column("store_product_no")]
        public string? StoreProductNo { get; set; }

        [StringLength(100)]
        [Column("store_category_code")]
        public string? StoreCategoryCode { get; set; }

        [StringLength(255)]
        [Column("store_product_name")]
        public string? StoreProductName { get; set; }

        [Required]
        [StringLength(30)]
        [Column("shelf_status")]
        public string ShelfStatus { get; set; } = "DRAFT";

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("is_featured")]
        public sbyte IsFeatured { get; set; } = 0;

        [Column("sort_no")]
        public int SortNo { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 商店SKU設定 / Store Product SKU Model
    /// <summary>
    /// 商店SKU設定 / Store Product SKU Model
    /// </summary>
    [Table("sto_product_sku")]
    public class StoreProductSku
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("store_product_nid")]
        public long StoreProductNid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sku_sid")]
        public string SkuSid { get; set; } = null!;

        [StringLength(100)]
        [Column("store_sku_no")]
        public string? StoreSkuNo { get; set; }

        [Required]
        [StringLength(30)]
        [Column("sale_status")]
        public string SaleStatus { get; set; } = "ACTIVE";

        [Column("max_sale_qty", TypeName = "decimal(20,6)")]
        public decimal? MaxSaleQty { get; set; }

        [Column("sort_no")]
        public int SortNo { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 商店商品價格 / Store Product Price Model
    /// <summary>
    /// 商店商品價格 / Store Product Price Model
    /// </summary>
    [Table("sto_product_price")]
    public class StoreProductPrice
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("store_nid")]
        public long StoreNid { get; set; }

        [Column("store_product_nid")]
        public long StoreProductNid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sku_sid")]
        public string SkuSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("price_group_sid")]
        public string PriceGroupSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("price", TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("priority")]
        public int Priority { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 商店SKU可用倉庫 / Store Product Warehouse Model
    /// <summary>
    /// 商店SKU可用倉庫 / Store Product Warehouse Model
    /// </summary>
    [Table("sto_product_warehouse")]
    public class StoreProductWarehouse
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("store_nid")]
        public long StoreNid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sku_sid")]
        public string SkuSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [Column("is_primary")]
        public sbyte IsPrimary { get; set; } = 0;

        [Column("allocation_priority")]
        public int AllocationPriority { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 商店子訂單 / Store Order Model
    /// <summary>
    /// 商店子訂單 / Store Order Model
    /// </summary>
    [Table("sto_order")]
    public class StoreOrder
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("store_nid")]
        public long StoreNid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("shopping_order_sid")]
        public string ShoppingOrderSid { get; set; } = null!;

        [Required]
        [StringLength(60)]
        [Column("store_order_no")]
        public string StoreOrderNo { get; set; } = null!;

        [Required]
        [StringLength(30)]
        [Column("order_status")]
        public string OrderStatus { get; set; } = "PENDING";

        [Column("subtotal_amount", TypeName = "decimal(18,4)")]
        public decimal SubtotalAmount { get; set; } = 0;

        [Column("discount_amount", TypeName = "decimal(18,4)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column("freight_amount", TypeName = "decimal(18,4)")]
        public decimal FreightAmount { get; set; } = 0;

        [Column("platform_fee_amount", TypeName = "decimal(18,4)")]
        public decimal PlatformFeeAmount { get; set; } = 0;

        [Column("payable_amount", TypeName = "decimal(18,4)")]
        public decimal PayableAmount { get; set; } = 0;

        [StringLength(32)]
        [Column("warehouse_sid")]
        public string? WarehouseSid { get; set; }

        [Column("accepted_date")]
        public DateTime? AcceptedDate { get; set; }

        [StringLength(32)]
        [Column("processing_user_sid")]
        public string? ProcessingUserSid { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 商店訂單明細 / Store Order Item Model
    /// <summary>
    /// 商店訂單明細 / Store Order Item Model
    /// </summary>
    [Table("sto_order_item")]
    public class StoreOrderItem
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("store_order_nid")]
        public long StoreOrderNid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("shopping_order_item_sid")]
        public string ShoppingOrderItemSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("product_sid")]
        public string ProductSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("sku_sid")]
        public string SkuSid { get; set; } = null!;

        [Column("quantity", TypeName = "decimal(20,6)")]
        public decimal Quantity { get; set; }

        [Column("unit_price", TypeName = "decimal(18,4)")]
        public decimal UnitPrice { get; set; }

        [Column("discount_amount", TypeName = "decimal(18,4)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column("line_amount", TypeName = "decimal(18,4)")]
        public decimal LineAmount { get; set; }

        [Column("platform_fee_amount", TypeName = "decimal(18,4)")]
        public decimal PlatformFeeAmount { get; set; } = 0;

        [Column("settlement_amount", TypeName = "decimal(18,4)")]
        public decimal SettlementAmount { get; set; } = 0;

        [Required]
        [StringLength(30)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "ACTIVE";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 商店結算單 / Store Settlement Model
    /// <summary>
    /// 商店結算單 / Store Settlement Model
    /// </summary>
    [Table("sto_settlement")]
    public class StoreSettlement
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(60)]
        [Column("settlement_no")]
        public string SettlementNo { get; set; } = null!;

        [Column("store_nid")]
        public long StoreNid { get; set; }

        [Column("period_start_date", TypeName = "date")]
        public DateTime PeriodStartDate { get; set; }

        [Column("period_end_date", TypeName = "date")]
        public DateTime PeriodEndDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("gross_sales_amount", TypeName = "decimal(18,4)")]
        public decimal GrossSalesAmount { get; set; } = 0;

        [Column("refund_amount", TypeName = "decimal(18,4)")]
        public decimal RefundAmount { get; set; } = 0;

        [Column("discount_share_amount", TypeName = "decimal(18,4)")]
        public decimal DiscountShareAmount { get; set; } = 0;

        [Column("platform_fee_amount", TypeName = "decimal(18,4)")]
        public decimal PlatformFeeAmount { get; set; } = 0;

        [Column("payment_fee_amount", TypeName = "decimal(18,4)")]
        public decimal PaymentFeeAmount { get; set; } = 0;

        [Column("shipping_fee_amount", TypeName = "decimal(18,4)")]
        public decimal ShippingFeeAmount { get; set; } = 0;

        [Column("adjustment_amount", TypeName = "decimal(18,4)")]
        public decimal AdjustmentAmount { get; set; } = 0;

        [Column("payable_amount", TypeName = "decimal(18,4)")]
        public decimal PayableAmount { get; set; } = 0;

        [Column("paid_amount", TypeName = "decimal(18,4)")]
        public decimal PaidAmount { get; set; } = 0;

        [Required]
        [StringLength(30)]
        [Column("settlement_status")]
        public string SettlementStatus { get; set; } = "DRAFT";

        [StringLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        [Column("paid_date")]
        public DateTime? PaidDate { get; set; }

        [StringLength(32)]
        [Column("payment_reference_sid")]
        public string? PaymentReferenceSid { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 商店結算明細 / Store Settlement Item Model
    /// <summary>
    /// 商店結算明細 / Store Settlement Item Model
    /// </summary>
    [Table("sto_settlement_item")]
    public class StoreSettlementItem
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("settlement_nid")]
        public long SettlementNid { get; set; }

        [StringLength(32)]
        [Column("store_order_sid")]
        public string? StoreOrderSid { get; set; }

        [Required]
        [StringLength(30)]
        [Column("source_type")]
        public string SourceType { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("source_sid")]
        public string SourceSid { get; set; } = null!;

        [StringLength(60)]
        [Column("source_no")]
        public string? SourceNo { get; set; }

        [Column("transaction_date")]
        public DateTime TransactionDate { get; set; }

        [Column("gross_amount", TypeName = "decimal(18,4)")]
        public decimal GrossAmount { get; set; } = 0;

        [Column("deduction_amount", TypeName = "decimal(18,4)")]
        public decimal DeductionAmount { get; set; } = 0;

        [Column("settlement_amount", TypeName = "decimal(18,4)")]
        public decimal SettlementAmount { get; set; } = 0;

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion
}