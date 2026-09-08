using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Customer.Models
{
    #region Loyalty Tier Module - 會員等級與權益

    [Table("lyt_tier")]
    public class LytTier
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("tier_code")]
        public string TierCode { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("tier_name")]
        public string TierName { get; set; }

        [Column("tier_level")]
        public int TierLevel { get; set; }

        [Column("min_accumulated_spend")]
        public decimal MinAccumulatedSpend { get; set; }

        [Column("min_accumulated_points")]
        public int MinAccumulatedPoints { get; set; }

        [Column("points_multiplier")]
        public decimal PointsMultiplier { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("tier_status")]
        public string TierStatus { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    [Table("lyt_customer_tier")]
    public class LytCustomerTier
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("customer_sid")]
        public string CustomerSid { get; set; }

        [Column("tier_nid")]
        public ulong TierNid { get; set; }

        [Column("effective_date")]
        public DateTime EffectiveDate { get; set; }

        [Column("expiration_date")]
        public DateTime? ExpirationDate { get; set; }

        [Column("current_period_spend")]
        public decimal CurrentPeriodSpend { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Loyalty Points Module - 會員點數帳戶與異動流水

    [Table("lyt_points_account")]
    public class LytPointsAccount
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("customer_sid")]
        public string CustomerSid { get; set; }

        [Column("total_points")]
        public int TotalPoints { get; set; }

        [Column("locked_points")]
        public int LockedPoints { get; set; }

        [Column("expired_points_accum")]
        public int ExpiredPointsAccum { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }
    }

    [Table("lyt_points_ledger")]
    public class LytPointsLedger
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("account_nid")]
        public ulong AccountNid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("customer_sid")]
        public string CustomerSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("transaction_type")]
        public string TransactionType { get; set; }

        [Column("points_change")]
        public int PointsChange { get; set; }

        [Column("points_after")]
        public int PointsAfter { get; set; }

        [Column("expiration_date")]
        public DateTime? ExpirationDate { get; set; }

        [MaxLength(50)]
        [Column("reference_doc_type")]
        public string ReferenceDocType { get; set; }

        [MaxLength(100)]
        [Column("reference_doc_no")]
        public string ReferenceDocNo { get; set; }

        [MaxLength(32)]
        [Column("operator_user_sid")]
        public string OperatorUserSid { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Loyalty Coupon Module - 優惠券 / 折扣券管理

    [Table("lyt_coupon_template")]
    public class LytCouponTemplate
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("template_code")]
        public string TemplateCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("discount_type")]
        public string DiscountType { get; set; }

        [Column("discount_value")]
        public decimal DiscountValue { get; set; }

        [Column("min_purchase_amount")]
        public decimal MinPurchaseAmount { get; set; }

        [Column("max_discount_amount")]
        public decimal? MaxDiscountAmount { get; set; }

        [Column("total_quantity")]
        public int TotalQuantity { get; set; }

        [Column("issued_quantity")]
        public int IssuedQuantity { get; set; }

        [Column("used_quantity")]
        public int UsedQuantity { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("validity_type")]
        public string ValidityType { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("relative_days")]
        public int? RelativeDays { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("template_status")]
        public string TemplateStatus { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    [Table("lyt_customer_coupon")]
    public class LytCustomerCoupon
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("coupon_template_nid")]
        public ulong CouponTemplateNid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("customer_sid")]
        public string CustomerSid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("coupon_code")]
        public string CouponCode { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("coupon_status")]
        public string CouponStatus { get; set; }

        [Column("used_time")]
        public DateTime? UsedTime { get; set; }

        [MaxLength(32)]
        [Column("used_sales_order_sid")]
        public string UsedSalesOrderSid { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Loyalty Reward & Exchange Module - 點數兌換商品與兌換訂單

    [Table("lyt_reward_item")]
    public class LytRewardItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("item_title")]
        public string ItemTitle { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("reward_type")]
        public string RewardType { get; set; }

        [Column("required_points")]
        public int RequiredPoints { get; set; }

        [Column("additional_amount")]
        public decimal AdditionalAmount { get; set; }

        [Column("target_coupon_tmpl_nid")]
        public ulong? TargetCouponTmplNid { get; set; }

        [MaxLength(32)]
        [Column("pim_item_sid")]
        public string PimItemSid { get; set; }

        [Column("stock_qty")]
        public int StockQty { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    [Table("lyt_exchange_order")]
    public class LytExchangeOrder
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("exchange_no")]
        public string ExchangeNo { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("customer_sid")]
        public string CustomerSid { get; set; }

        [Column("reward_item_nid")]
        public ulong RewardItemNid { get; set; }

        [Column("used_points")]
        public int UsedPoints { get; set; }

        [Column("paid_amount")]
        public decimal PaidAmount { get; set; }

        [MaxLength(32)]
        [Column("fulfillment_request_sid")]
        public string FulfillmentRequestSid { get; set; }

        [MaxLength(32)]
        [Column("issued_customer_coupon_sid")]
        public string IssuedCustomerCouponSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("exchange_status")]
        public string ExchangeStatus { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion
}