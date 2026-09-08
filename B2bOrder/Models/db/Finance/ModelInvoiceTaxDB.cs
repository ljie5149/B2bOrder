using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Finance.Models
{
    #region 01. 稅別與發票政策 (Tax Types & Invoice Policies)

    [Table("tax_type")]
    public class TaxType
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [Column("tax_code")]
        [StringLength(100)]
        public string TaxCode { get; set; } = null!;

        [Required]
        [Column("tax_name")]
        [StringLength(200)]
        public string TaxName { get; set; } = null!;

        [Required]
        [Column("tax_category")]
        [StringLength(30)]
        public string TaxCategory { get; set; } = null!;

        [Column("tax_rate", TypeName = "decimal(8,4)")]
        public decimal TaxRate { get; set; }

        [Column("country_sid")]
        [StringLength(32)]
        public string? CountrySid { get; set; }

        [Column("region_sid")]
        [StringLength(32)]
        public string? RegionSid { get; set; }

        [Column("recoverable_mark")]
        public sbyte RecoverableMark { get; set; }

        [Column("inclusive_mark")]
        public sbyte InclusiveMark { get; set; }

        [Column("effective_start_date")]
        public DateOnly EffectiveStartDate { get; set; }

        [Column("effective_end_date")]
        public DateOnly? EffectiveEndDate { get; set; }

        [Required]
        [Column("tax_status")]
        [StringLength(20)]
        public string TaxStatus { get; set; } = "ACTIVE";

        [Required]
        [Column("avalible")]
        [StringLength(2)]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }
    }

    [Table("tax_invoice_policy")]
    public class TaxInvoicePolicy
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [Column("policy_code")]
        [StringLength(100)]
        public string PolicyCode { get; set; } = null!;

        [Required]
        [Column("policy_name")]
        [StringLength(200)]
        public string PolicyName { get; set; } = null!;

        [Column("company_sid")]
        [StringLength(32)]
        public string? CompanySid { get; set; }

        [Column("business_unit_sid")]
        [StringLength(32)]
        public string? BusinessUnitSid { get; set; }

        [Column("channel_sid")]
        [StringLength(32)]
        public string? ChannelSid { get; set; }

        [Required]
        [Column("invoice_mode")]
        [StringLength(30)]
        public string InvoiceMode { get; set; } = "ELECTRONIC";

        [Required]
        [Column("issue_trigger")]
        [StringLength(30)]
        public string IssueTrigger { get; set; } = "PAYMENT";

        [Column("issue_deadline_days")]
        public int IssueDeadlineDays { get; set; }

        [Column("auto_issue_enabled")]
        public sbyte AutoIssueEnabled { get; set; }

        [Column("split_invoice_allowed")]
        public sbyte SplitInvoiceAllowed { get; set; }

        [Column("consolidate_allowed")]
        public sbyte ConsolidateAllowed { get; set; }

        [Column("default_tax_type_sid")]
        [StringLength(32)]
        public string? DefaultTaxTypeSid { get; set; }

        [Required]
        [Column("rounding_mode")]
        [StringLength(20)]
        public string RoundingMode { get; set; } = "HALF_UP";

        [Column("rounding_scale")]
        public int RoundingScale { get; set; }

        [Required]
        [Column("policy_status")]
        [StringLength(20)]
        public string PolicyStatus { get; set; } = "ACTIVE";

        [Required]
        [Column("avalible")]
        [StringLength(2)]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }
    }

    #endregion

    #region 02. 發票字軌與號碼 (Invoice Tracks & Numbers)

    [Table("tax_invoice_track")]
    public class TaxInvoiceTrack
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [Column("track_code")]
        [StringLength(10)]
        public string TrackCode { get; set; } = null!;

        [Column("invoice_year")]
        public int InvoiceYear { get; set; }

        [Required]
        [Column("invoice_period")]
        [StringLength(10)]
        public string InvoicePeriod { get; set; } = null!;

        [Required]
        [Column("company_sid")]
        [StringLength(32)]
        public string CompanySid { get; set; } = null!;

        [Column("business_unit_sid")]
        [StringLength(32)]
        public string? BusinessUnitSid { get; set; }

        [Required]
        [Column("invoice_type")]
        [StringLength(30)]
        public string InvoiceType { get; set; } = null!;

        [Column("start_number")]
        public ulong StartNumber { get; set; }

        [Column("end_number")]
        public ulong EndNumber { get; set; }

        [Column("current_number")]
        public ulong CurrentNumber { get; set; }

        [Column("reserved_count")]
        public int ReservedCount { get; set; }

        [Column("used_count")]
        public int UsedCount { get; set; }

        [Column("void_count")]
        public int VoidCount { get; set; }

        [Required]
        [Column("track_status")]
        [StringLength(20)]
        public string TrackStatus { get; set; } = "ACTIVE";

        [Required]
        [Column("avalible")]
        [StringLength(2)]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }
    }

    [Table("tax_invoice_number")]
    public class TaxInvoiceNumber
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [Column("invoice_track_sid")]
        [StringLength(32)]
        public string InvoiceTrackSid { get; set; } = null!;

        [Required]
        [Column("invoice_number")]
        [StringLength(20)]
        public string InvoiceNumber { get; set; } = null!;

        [Column("number_value")]
        public ulong NumberValue { get; set; }

        [Column("reserved_reference_type")]
        [StringLength(30)]
        public string? ReservedReferenceType { get; set; }

        [Column("reserved_reference_sid")]
        [StringLength(32)]
        public string? ReservedReferenceSid { get; set; }

        [Column("reserved_date")]
        public DateTime? ReservedDate { get; set; }

        [Column("reservation_expiry_date")]
        public DateTime? ReservationExpiryDate { get; set; }

        [Column("invoice_sid")]
        [StringLength(32)]
        public string? InvoiceSid { get; set; }

        [Required]
        [Column("number_status")]
        [StringLength(20)]
        public string NumberStatus { get; set; } = "AVAILABLE";
    }

    #endregion

    #region 03. 銷項發票 (Sales Invoices)

    [Table("tax_sales_invoice")]
    public class TaxSalesInvoice
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [Column("invoice_no")]
        [StringLength(100)]
        public string InvoiceNo { get; set; } = null!;

        [Column("invoice_number")]
        [StringLength(20)]
        public string? InvoiceNumber { get; set; }

        [Column("invoice_track_sid")]
        [StringLength(32)]
        public string? InvoiceTrackSid { get; set; }

        [Column("invoice_policy_sid")]
        [StringLength(32)]
        public string? InvoicePolicySid { get; set; }

        [Required]
        [Column("company_sid")]
        [StringLength(32)]
        public string CompanySid { get; set; } = null!;

        [Column("business_unit_sid")]
        [StringLength(32)]
        public string? BusinessUnitSid { get; set; }

        [Column("seller_party_sid")]
        [StringLength(32)]
        public string? SellerPartySid { get; set; }

        [Column("buyer_party_sid")]
        [StringLength(32)]
        public string? BuyerPartySid { get; set; }

        [Column("sales_order_sid")]
        [StringLength(32)]
        public string? SalesOrderSid { get; set; }

        [Column("payment_request_sid")]
        [StringLength(32)]
        public string? PaymentRequestSid { get; set; }

        [Column("fulfillment_sid")]
        [StringLength(32)]
        public string? FulfillmentSid { get; set; }

        [Column("receivable_sid")]
        [StringLength(32)]
        public string? ReceivableSid { get; set; }

        [Column("contract_sid")]
        [StringLength(32)]
        public string? ContractSid { get; set; }

        [Column("project_sid")]
        [StringLength(32)]
        public string? ProjectSid { get; set; }

        [Column("invoice_date")]
        public DateTime InvoiceDate { get; set; }

        [Required]
        [Column("invoice_type")]
        [StringLength(30)]
        public string InvoiceType { get; set; } = null!;

        [Required]
        [Column("currency_sid")]
        [StringLength(32)]
        public string CurrencySid { get; set; } = null!;

        [Column("exchange_rate", TypeName = "decimal(20,10)")]
        public decimal ExchangeRate { get; set; } = 1;

        [Column("sales_amount", TypeName = "decimal(20,4)")]
        public decimal SalesAmount { get; set; }

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; }

        [Column("total_amount", TypeName = "decimal(20,4)")]
        public decimal TotalAmount { get; set; }

        [Column("taxable_amount", TypeName = "decimal(20,4)")]
        public decimal TaxableAmount { get; set; }

        [Column("zero_rate_amount", TypeName = "decimal(20,4)")]
        public decimal ZeroRateAmount { get; set; }

        [Column("exempt_amount", TypeName = "decimal(20,4)")]
        public decimal ExemptAmount { get; set; }

        [Column("buyer_tax_no")]
        [StringLength(50)]
        public string? BuyerTaxNo { get; set; }

        [Column("buyer_name")]
        [StringLength(300)]
        public string? BuyerName { get; set; }

        [Column("buyer_email")]
        [StringLength(200)]
        public string? BuyerEmail { get; set; }

        [Column("buyer_phone")]
        [StringLength(50)]
        public string? BuyerPhone { get; set; }

        [Column("random_number")]
        [StringLength(10)]
        public string? RandomNumber { get; set; }

        [Column("carrier_type")]
        [StringLength(30)]
        public string? CarrierType { get; set; }

        [Column("carrier_no_hash")]
        [StringLength(255)]
        public string? CarrierNoHash { get; set; }

        [Column("carrier_no_encrypted", TypeName = "LONGTEXT")]
        public string? CarrierNoEncrypted { get; set; }

        [Column("donation_code")]
        [StringLength(50)]
        public string? DonationCode { get; set; }

        [Column("print_mark")]
        public sbyte PrintMark { get; set; }

        [Column("customs_clearance_mark")]
        [StringLength(20)]
        public string? CustomsClearanceMark { get; set; }

        [Required]
        [Column("invoice_status")]
        [StringLength(30)]
        public string InvoiceStatus { get; set; } = "DRAFT";

        [Required]
        [Column("platform_submit_status")]
        [StringLength(20)]
        public string PlatformSubmitStatus { get; set; } = "PENDING";

        [Column("platform_invoice_id")]
        [StringLength(200)]
        public string? PlatformInvoiceId { get; set; }

        [Column("qr_code_left", TypeName = "TEXT")]
        public string? QrCodeLeft { get; set; }

        [Column("qr_code_right", TypeName = "TEXT")]
        public string? QrCodeRight { get; set; }

        [Column("bar_code")]
        [StringLength(100)]
        public string? BarCode { get; set; }

        [Column("issued_user_sid")]
        [StringLength(32)]
        public string? IssuedUserSid { get; set; }

        [Column("issued_date")]
        public DateTime? IssuedDate { get; set; }

        [Column("correlation_id")]
        [StringLength(100)]
        public string? CorrelationId { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [Column("avalible")]
        [StringLength(2)]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        public virtual ICollection<TaxSalesInvoiceItem> Items { get; set; } = new List<TaxSalesInvoiceItem>();
    }

    [Table("tax_sales_invoice_item")]
    public class TaxSalesInvoiceItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("sales_invoice_nid")]
        public ulong SalesInvoiceNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Column("sales_order_item_sid")]
        [StringLength(32)]
        public string? SalesOrderItemSid { get; set; }

        [Column("item_sid")]
        [StringLength(32)]
        public string? ItemSid { get; set; }

        [Column("variant_sid")]
        [StringLength(32)]
        public string? VariantSid { get; set; }

        [Required]
        [Column("item_name")]
        [StringLength(500)]
        public string ItemName { get; set; } = null!;

        [Column("item_description")]
        [StringLength(1000)]
        public string? ItemDescription { get; set; }

        [Column("quantity", TypeName = "decimal(20,6)")]
        public decimal Quantity { get; set; }

        [Column("unit_sid")]
        [StringLength(32)]
        public string? UnitSid { get; set; }

        [Column("unit_price", TypeName = "decimal(20,6)")]
        public decimal UnitPrice { get; set; }

        [Column("sales_amount", TypeName = "decimal(20,4)")]
        public decimal SalesAmount { get; set; }

        [Column("discount_amount", TypeName = "decimal(20,4)")]
        public decimal DiscountAmount { get; set; }

        [Required]
        [Column("tax_type_sid")]
        [StringLength(32)]
        public string TaxTypeSid { get; set; } = null!;

        [Column("tax_rate", TypeName = "decimal(8,4)")]
        public decimal TaxRate { get; set; }

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; }

        [Column("total_amount", TypeName = "decimal(20,4)")]
        public decimal TotalAmount { get; set; }

        [Column("project_sid")]
        [StringLength(32)]
        public string? ProjectSid { get; set; }

        [Column("wbs_sid")]
        [StringLength(32)]
        public string? WbsSid { get; set; }

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        [ForeignKey("SalesInvoiceNid")]
        public virtual TaxSalesInvoice SalesInvoice { get; set; } = null!;
    }

    #endregion

    #region 04. 進項發票 (Purchase Invoices)

    [Table("tax_purchase_invoice")]
    public class TaxPurchaseInvoice
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [Column("purchase_invoice_no")]
        [StringLength(100)]
        public string PurchaseInvoiceNo { get; set; } = null!;

        [Required]
        [Column("supplier_invoice_no")]
        [StringLength(100)]
        public string SupplierInvoiceNo { get; set; } = null!;

        [Required]
        [Column("company_sid")]
        [StringLength(32)]
        public string CompanySid { get; set; } = null!;

        [Required]
        [Column("supplier_party_sid")]
        [StringLength(32)]
        public string SupplierPartySid { get; set; } = null!;

        [Column("purchase_order_sid")]
        [StringLength(32)]
        public string? PurchaseOrderSid { get; set; }

        [Column("receipt_sid")]
        [StringLength(32)]
        public string? ReceiptSid { get; set; }

        [Column("payable_sid")]
        [StringLength(32)]
        public string? PayableSid { get; set; }

        [Column("contract_sid")]
        [StringLength(32)]
        public string? ContractSid { get; set; }

        [Column("project_sid")]
        [StringLength(32)]
        public string? ProjectSid { get; set; }

        [Column("invoice_date")]
        public DateOnly InvoiceDate { get; set; }

        [Column("received_date")]
        public DateOnly ReceivedDate { get; set; }

        [Column("accounting_period_sid")]
        [StringLength(32)]
        public string? AccountingPeriodSid { get; set; }

        [Required]
        [Column("currency_sid")]
        [StringLength(32)]
        public string CurrencySid { get; set; } = null!;

        [Column("exchange_rate", TypeName = "decimal(20,10)")]
        public decimal ExchangeRate { get; set; } = 1;

        [Column("purchase_amount", TypeName = "decimal(20,4)")]
        public decimal PurchaseAmount { get; set; }

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; }

        [Column("total_amount", TypeName = "decimal(20,4)")]
        public decimal TotalAmount { get; set; }

        [Column("deductible_tax_amount", TypeName = "decimal(20,4)")]
        public decimal DeductibleTaxAmount { get; set; }

        [Column("non_deductible_tax_amount", TypeName = "decimal(20,4)")]
        public decimal NonDeductibleTaxAmount { get; set; }

        [Column("supplier_tax_no")]
        [StringLength(50)]
        public string? SupplierTaxNo { get; set; }

        [Column("supplier_name")]
        [StringLength(300)]
        public string? SupplierName { get; set; }

        [Required]
        [Column("verification_status")]
        [StringLength(20)]
        public string VerificationStatus { get; set; } = "PENDING";

        [Required]
        [Column("matching_status")]
        [StringLength(20)]
        public string MatchingStatus { get; set; } = "UNMATCHED";

        [Required]
        [Column("invoice_status")]
        [StringLength(20)]
        public string InvoiceStatus { get; set; } = "RECEIVED";

        [Column("source_file_sid")]
        [StringLength(32)]
        public string? SourceFileSid { get; set; }

        [Column("verified_user_sid")]
        [StringLength(32)]
        public string? VerifiedUserSid { get; set; }

        [Column("verified_date")]
        public DateTime? VerifiedDate { get; set; }

        [Column("correlation_id")]
        [StringLength(100)]
        public string? CorrelationId { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [Column("avalible")]
        [StringLength(2)]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        public virtual ICollection<TaxPurchaseInvoiceItem> Items { get; set; } = new List<TaxPurchaseInvoiceItem>();
    }

    [Table("tax_purchase_invoice_item")]
    public class TaxPurchaseInvoiceItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("purchase_invoice_nid")]
        public ulong PurchaseInvoiceNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Column("purchase_order_item_sid")]
        [StringLength(32)]
        public string? PurchaseOrderItemSid { get; set; }

        [Column("receipt_item_sid")]
        [StringLength(32)]
        public string? ReceiptItemSid { get; set; }

        [Column("item_sid")]
        [StringLength(32)]
        public string? ItemSid { get; set; }

        [Column("variant_sid")]
        [StringLength(32)]
        public string? VariantSid { get; set; }

        [Required]
        [Column("item_name")]
        [StringLength(500)]
        public string ItemName { get; set; } = null!;

        [Column("quantity", TypeName = "decimal(20,6)")]
        public decimal? Quantity { get; set; }

        [Column("unit_sid")]
        [StringLength(32)]
        public string? UnitSid { get; set; }

        [Column("unit_price", TypeName = "decimal(20,6)")]
        public decimal? UnitPrice { get; set; }

        [Column("purchase_amount", TypeName = "decimal(20,4)")]
        public decimal PurchaseAmount { get; set; }

        [Required]
        [Column("tax_type_sid")]
        [StringLength(32)]
        public string TaxTypeSid { get; set; } = null!;

        [Column("tax_rate", TypeName = "decimal(8,4)")]
        public decimal TaxRate { get; set; }

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; }

        [Column("total_amount", TypeName = "decimal(20,4)")]
        public decimal TotalAmount { get; set; }

        [Column("project_sid")]
        [StringLength(32)]
        public string? ProjectSid { get; set; }

        [Column("site_sid")]
        [StringLength(32)]
        public string? SiteSid { get; set; }

        [Column("wbs_sid")]
        [StringLength(32)]
        public string? WbsSid { get; set; }

        [Column("cost_center_sid")]
        [StringLength(32)]
        public string? CostCenterSid { get; set; }

        [Column("expense_account_sid")]
        [StringLength(32)]
        public string? ExpenseAccountSid { get; set; }

        [ForeignKey("PurchaseInvoiceNid")]
        public virtual TaxPurchaseInvoice PurchaseInvoice { get; set; } = null!;
    }

    #endregion

    #region 05. 發票請求與開立工作 (Invoice Requests)

    [Table("tax_invoice_request")]
    public class TaxInvoiceRequest
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [Column("request_no")]
        [StringLength(100)]
        public string RequestNo { get; set; } = null!;

        [Required]
        [Column("request_type")]
        [StringLength(30)]
        public string RequestType { get; set; } = null!;

        [Required]
        [Column("source_type")]
        [StringLength(30)]
        public string SourceType { get; set; } = null!;

        [Required]
        [Column("source_sid")]
        [StringLength(32)]
        public string SourceSid { get; set; } = null!;

        [Column("party_sid")]
        [StringLength(32)]
        public string? PartySid { get; set; }

        [Required]
        [Column("company_sid")]
        [StringLength(32)]
        public string CompanySid { get; set; } = null!;

        [Column("invoice_policy_sid")]
        [StringLength(32)]
        public string? InvoicePolicySid { get; set; }

        [Required]
        [Column("request_data", TypeName = "JSON")]
        public string RequestData { get; set; } = null!;

        [Column("requested_date")]
        public DateTime RequestedDate { get; set; }

        [Column("issue_due_date")]
        public DateTime? IssueDueDate { get; set; }

        [Required]
        [Column("idempotency_key")]
        [StringLength(200)]
        public string IdempotencyKey { get; set; } = null!;

        [Required]
        [Column("request_status")]
        [StringLength(20)]
        public string RequestStatus { get; set; } = "PENDING";

        [Column("invoice_sid")]
        [StringLength(32)]
        public string? InvoiceSid { get; set; }

        [Column("retry_count")]
        public int RetryCount { get; set; }

        [Column("correlation_id")]
        [StringLength(100)]
        public string? CorrelationId { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [Column("error_message", TypeName = "LONGTEXT")]
        public string? ErrorMessage { get; set; }
    }

    #endregion

    #region 06. 作廢 (Invoice Voids)

    [Table("tax_invoice_void")]
    public class TaxInvoiceVoid
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [Column("void_no")]
        [StringLength(100)]
        public string VoidNo { get; set; } = null!;

        [Required]
        [Column("invoice_type")]
        [StringLength(20)]
        public string InvoiceType { get; set; } = null!;

        [Required]
        [Column("invoice_sid")]
        [StringLength(32)]
        public string InvoiceSid { get; set; } = null!;

        [Column("invoice_number")]
        [StringLength(20)]
        public string? InvoiceNumber { get; set; }

        [Column("void_date")]
        public DateTime VoidDate { get; set; }

        [Required]
        [Column("reason_code")]
        [StringLength(50)]
        public string ReasonCode { get; set; } = null!;

        [Required]
        [Column("reason", TypeName = "TEXT")]
        public string Reason { get; set; } = null!;

        [Column("requested_user_sid")]
        [StringLength(32)]
        public string? RequestedUserSid { get; set; }

        [Column("approved_user_sid")]
        [StringLength(32)]
        public string? ApprovedUserSid { get; set; }

        [Column("workflow_instance_sid")]
        [StringLength(32)]
        public string? WorkflowInstanceSid { get; set; }

        [Required]
        [Column("platform_submit_status")]
        [StringLength(20)]
        public string PlatformSubmitStatus { get; set; } = "PENDING";

        [Required]
        [Column("void_status")]
        [StringLength(20)]
        public string VoidStatus { get; set; } = "DRAFT";

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [Column("correlation_id")]
        [StringLength(100)]
        public string? CorrelationId { get; set; }
    }

    #endregion

    #region 07. 折讓與退貨調整 (Allowances)

    [Table("tax_allowance")]
    public class TaxAllowance
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [Column("allowance_no")]
        [StringLength(100)]
        public string AllowanceNo { get; set; } = null!;

        [Column("allowance_number")]
        [StringLength(50)]
        public string? AllowanceNumber { get; set; }

        [Required]
        [Column("allowance_type")]
        [StringLength(20)]
        public string AllowanceType { get; set; } = null!;

        [Required]
        [Column("invoice_sid")]
        [StringLength(32)]
        public string InvoiceSid { get; set; } = null!;

        [Column("invoice_number")]
        [StringLength(20)]
        public string? InvoiceNumber { get; set; }

        [Column("return_case_sid")]
        [StringLength(32)]
        public string? ReturnCaseSid { get; set; }

        [Column("supplier_claim_sid")]
        [StringLength(32)]
        public string? SupplierClaimSid { get; set; }

        [Column("credit_debit_note_sid")]
        [StringLength(32)]
        public string? CreditDebitNoteSid { get; set; }

        [Required]
        [Column("party_sid")]
        [StringLength(32)]
        public string PartySid { get; set; } = null!;

        [Column("allowance_date")]
        public DateTime AllowanceDate { get; set; }

        [Column("sales_amount", TypeName = "decimal(20,4)")]
        public decimal SalesAmount { get; set; }

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; }

        [Column("total_amount", TypeName = "decimal(20,4)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column("reason_code")]
        [StringLength(50)]
        public string ReasonCode { get; set; } = null!;

        [Column("reason", TypeName = "TEXT")]
        public string? Reason { get; set; }

        [Required]
        [Column("allowance_status")]
        [StringLength(30)]
        public string AllowanceStatus { get; set; } = "DRAFT";

        [Required]
        [Column("platform_submit_status")]
        [StringLength(20)]
        public string PlatformSubmitStatus { get; set; } = "PENDING";

        [Column("workflow_instance_sid")]
        [StringLength(32)]
        public string? WorkflowInstanceSid { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [Column("correlation_id")]
        [StringLength(100)]
        public string? CorrelationId { get; set; }

        public virtual ICollection<TaxAllowanceItem> Items { get; set; } = new List<TaxAllowanceItem>();
    }

    [Table("tax_allowance_item")]
    public class TaxAllowanceItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("allowance_nid")]
        public ulong AllowanceNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [Column("original_invoice_item_sid")]
        [StringLength(32)]
        public string OriginalInvoiceItemSid { get; set; } = null!;

        [Column("item_sid")]
        [StringLength(32)]
        public string? ItemSid { get; set; }

        [Required]
        [Column("item_name")]
        [StringLength(500)]
        public string ItemName { get; set; } = null!;

        [Column("quantity", TypeName = "decimal(20,6)")]
        public decimal Quantity { get; set; }

        [Column("unit_sid")]
        [StringLength(32)]
        public string? UnitSid { get; set; }

        [Column("sales_amount", TypeName = "decimal(20,4)")]
        public decimal SalesAmount { get; set; }

        [Required]
        [Column("tax_type_sid")]
        [StringLength(32)]
        public string TaxTypeSid { get; set; } = null!;

        [Column("tax_rate", TypeName = "decimal(8,4)")]
        public decimal TaxRate { get; set; }

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; }

        [Column("total_amount", TypeName = "decimal(20,4)")]
        public decimal TotalAmount { get; set; }

        [ForeignKey("AllowanceNid")]
        public virtual TaxAllowance Allowance { get; set; } = null!;
    }

    #endregion

    #region 08. 載具、捐贈與買受人資料 (Carriers & Donations)

    [Table("tax_carrier")]
    public class TaxCarrier
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("party_sid")]
        [StringLength(32)]
        public string? PartySid { get; set; }

        [Required]
        [Column("carrier_type")]
        [StringLength(30)]
        public string CarrierType { get; set; } = null!;

        [Required]
        [Column("carrier_no_hash")]
        [StringLength(255)]
        public string CarrierNoHash { get; set; } = null!;

        [Required]
        [Column("carrier_no_encrypted", TypeName = "LONGTEXT")]
        public string CarrierNoEncrypted { get; set; } = null!;

        [Column("carrier_no_masked")]
        [StringLength(200)]
        public string? CarrierNoMasked { get; set; }

        [Column("default_mark")]
        public sbyte DefaultMark { get; set; }

        [Column("verified_mark")]
        public sbyte VerifiedMark { get; set; }

        [Column("verified_date")]
        public DateTime? VerifiedDate { get; set; }

        [Required]
        [Column("carrier_status")]
        [StringLength(20)]
        public string CarrierStatus { get; set; } = "ACTIVE";
    }

    [Table("tax_donation_code")]
    public class TaxDonationCode
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [Column("donation_code")]
        [StringLength(50)]
        public string DonationCode { get; set; } = null!;

        [Column("organization_party_sid")]
        [StringLength(32)]
        public string? OrganizationPartySid { get; set; }

        [Required]
        [Column("organization_name")]
        [StringLength(300)]
        public string OrganizationName { get; set; } = null!;

        [Column("effective_start_date")]
        public DateOnly EffectiveStartDate { get; set; }

        [Column("effective_end_date")]
        public DateOnly? EffectiveEndDate { get; set; }

        [Required]
        [Column("donation_status")]
        [StringLength(20)]
        public string DonationStatus { get; set; } = "ACTIVE";

        [Required]
        [Column("avalible")]
        [StringLength(2)]
        public string Avalible { get; set; } = "Y";
    }

    #endregion

    #region 09. 三方匹配與稅務驗證 (Matching & Validation Logs)

    [Table("tax_invoice_match")]
    public class TaxInvoiceMatch
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [Column("invoice_type")]
        [StringLength(20)]
        public string InvoiceType { get; set; } = null!;

        [Required]
        [Column("invoice_sid")]
        [StringLength(32)]
        public string InvoiceSid { get; set; } = null!;

        [Required]
        [Column("match_type")]
        [StringLength(30)]
        public string MatchType { get; set; } = null!;

        [Column("order_sid")]
        [StringLength(32)]
        public string? OrderSid { get; set; }

        [Column("receipt_sid")]
        [StringLength(32)]
        public string? ReceiptSid { get; set; }

        [Column("inspection_sid")]
        [StringLength(32)]
        public string? InspectionSid { get; set; }

        [Column("accounting_sid")]
        [StringLength(32)]
        public string? AccountingSid { get; set; }

        [Column("amount_difference", TypeName = "decimal(20,4)")]
        public decimal AmountDifference { get; set; }

        [Column("tax_difference", TypeName = "decimal(20,4)")]
        public decimal TaxDifference { get; set; }

        [Column("quantity_difference", TypeName = "decimal(20,6)")]
        public decimal QuantityDifference { get; set; }

        [Column("tolerance_amount", TypeName = "decimal(20,4)")]
        public decimal ToleranceAmount { get; set; }

        [Required]
        [Column("match_result")]
        [StringLength(20)]
        public string MatchResult { get; set; } = "PENDING";

        [Column("matched_user_sid")]
        [StringLength(32)]
        public string? MatchedUserSid { get; set; }

        [Column("matched_date")]
        public DateTime? MatchedDate { get; set; }

        [Column("resolution_note", TypeName = "TEXT")]
        public string? ResolutionNote { get; set; }
    }

    [Table("tax_validation_log")]
    public class TaxValidationLog
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [Column("entity_type")]
        [StringLength(30)]
        public string EntityType { get; set; } = null!;

        [Required]
        [Column("entity_sid")]
        [StringLength(32)]
        public string EntitySid { get; set; } = null!;

        [Required]
        [Column("validation_type")]
        [StringLength(30)]
        public string ValidationType { get; set; } = null!;

        [Required]
        [Column("validation_result")]
        [StringLength(20)]
        public string ValidationResult { get; set; } = null!;

        [Column("validation_code")]
        [StringLength(100)]
        public string? ValidationCode { get; set; }

        [Column("validation_message")]
        [StringLength(2000)]
        public string? ValidationMessage { get; set; }

        [Column("validation_data", TypeName = "JSON")]
        public string? ValidationData { get; set; }

        [Required]
        [Column("validator_type")]
        [StringLength(20)]
        public string ValidatorType { get; set; } = "SYSTEM";

        [Column("validator_sid")]
        [StringLength(32)]
        public string? ValidatorSid { get; set; }
    }

    #endregion

    #region 10. 電子發票平台交換 (Platform Messages)

    [Table("tax_platform_message")]
    public class TaxPlatformMessage
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [Column("message_no")]
        [StringLength(100)]
        public string MessageNo { get; set; } = null!;

        [Required]
        [Column("platform_code")]
        [StringLength(100)]
        public string PlatformCode { get; set; } = null!;

        [Required]
        [Column("message_type")]
        [StringLength(50)]
        public string MessageType { get; set; } = null!;

        [Required]
        [Column("direction")]
        [StringLength(10)]
        public string Direction { get; set; } = null!;

        [Required]
        [Column("entity_type")]
        [StringLength(30)]
        public string EntityType { get; set; } = null!;

        [Required]
        [Column("entity_sid")]
        [StringLength(32)]
        public string EntitySid { get; set; } = null!;

        [Column("request_payload", TypeName = "LONGTEXT")]
        public string? RequestPayload { get; set; }

        [Column("response_payload", TypeName = "LONGTEXT")]
        public string? ResponsePayload { get; set; }

        [Column("provider_message_id")]
        [StringLength(200)]
        public string? ProviderMessageId { get; set; }

        [Column("response_code")]
        [StringLength(100)]
        public string? ResponseCode { get; set; }

        [Column("response_message")]
        [StringLength(2000)]
        public string? ResponseMessage { get; set; }

        [Column("send_count")]
        public int SendCount { get; set; }

        [Required]
        [Column("message_status")]
        [StringLength(20)]
        public string MessageStatus { get; set; } = "PENDING";

        [Column("next_retry_date")]
        public DateTime? NextRetryDate { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [Column("correlation_id")]
        [StringLength(100)]
        public string? CorrelationId { get; set; }
    }

    #endregion

    #region 11. 稅務申報 (Tax Filings)

    [Table("tax_filing_batch")]
    public class TaxFilingBatch
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [Column("filing_no")]
        [StringLength(100)]
        public string FilingNo { get; set; } = null!;

        [Required]
        [Column("company_sid")]
        [StringLength(32)]
        public string CompanySid { get; set; } = null!;

        [Required]
        [Column("filing_type")]
        [StringLength(30)]
        public string FilingType { get; set; } = null!;

        [Column("period_start_date")]
        public DateOnly PeriodStartDate { get; set; }

        [Column("period_end_date")]
        public DateOnly PeriodEndDate { get; set; }

        [Column("filing_year")]
        public int FilingYear { get; set; }

        [Required]
        [Column("filing_period")]
        [StringLength(20)]
        public string FilingPeriod { get; set; } = null!;

        [Required]
        [Column("currency_sid")]
        [StringLength(32)]
        public string CurrencySid { get; set; } = null!;

        [Column("sales_amount", TypeName = "decimal(20,4)")]
        public decimal SalesAmount { get; set; }

        [Column("output_tax_amount", TypeName = "decimal(20,4)")]
        public decimal OutputTaxAmount { get; set; }

        [Column("purchase_amount", TypeName = "decimal(20,4)")]
        public decimal PurchaseAmount { get; set; }

        [Column("input_tax_amount", TypeName = "decimal(20,4)")]
        public decimal InputTaxAmount { get; set; }

        [Column("deductible_tax_amount", TypeName = "decimal(20,4)")]
        public decimal DeductibleTaxAmount { get; set; }

        [Column("payable_tax_amount", TypeName = "decimal(20,4)")]
        public decimal PayableTaxAmount { get; set; }

        [Column("refundable_tax_amount", TypeName = "decimal(20,4)")]
        public decimal RefundableTaxAmount { get; set; }

        [Column("filing_file_sid")]
        [StringLength(32)]
        public string? FilingFileSid { get; set; }

        [Column("receipt_file_sid")]
        [StringLength(32)]
        public string? ReceiptFileSid { get; set; }

        [Required]
        [Column("filing_status")]
        [StringLength(30)]
        public string FilingStatus { get; set; } = "DRAFT";

        [Column("workflow_instance_sid")]
        [StringLength(32)]
        public string? WorkflowInstanceSid { get; set; }

        [Column("submitted_date")]
        public DateTime? SubmittedDate { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        public virtual ICollection<TaxFilingItem> Items { get; set; } = new List<TaxFilingItem>();
    }

    [Table("tax_filing_item")]
    public class TaxFilingItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("filing_batch_nid")]
        public ulong FilingBatchNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [Column("document_type")]
        [StringLength(30)]
        public string DocumentType { get; set; } = null!;

        [Required]
        [Column("document_sid")]
        [StringLength(32)]
        public string DocumentSid { get; set; } = null!;

        [Column("invoice_number")]
        [StringLength(50)]
        public string? InvoiceNumber { get; set; }

        [Column("document_date")]
        public DateOnly DocumentDate { get; set; }

        [Column("party_tax_no")]
        [StringLength(50)]
        public string? PartyTaxNo { get; set; }

        [Column("sales_amount", TypeName = "decimal(20,4)")]
        public decimal SalesAmount { get; set; }

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; }

        [Column("deductible_tax_amount", TypeName = "decimal(20,4)")]
        public decimal DeductibleTaxAmount { get; set; }

        [Column("filing_category")]
        [StringLength(50)]
        public string? FilingCategory { get; set; }

        [Required]
        [Column("filing_status")]
        [StringLength(20)]
        public string FilingStatus { get; set; } = "INCLUDED";

        [Column("error_message", TypeName = "TEXT")]
        public string? ErrorMessage { get; set; }

        [ForeignKey("FilingBatchNid")]
        public virtual TaxFilingBatch FilingBatch { get; set; } = null!;
    }

    #endregion

    #region 12. 狀態歷程與事件 (History & Events)

    [Table("tax_status_history")]
    public class TaxStatusHistory
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [Column("entity_type")]
        [StringLength(30)]
        public string EntityType { get; set; } = null!;

        [Required]
        [Column("entity_sid")]
        [StringLength(32)]
        public string EntitySid { get; set; } = null!;

        [Column("old_status")]
        [StringLength(30)]
        public string? OldStatus { get; set; }

        [Required]
        [Column("new_status")]
        [StringLength(30)]
        public string NewStatus { get; set; } = null!;

        [Column("event_code")]
        [StringLength(100)]
        public string? EventCode { get; set; }

        [Column("operator_user_sid")]
        [StringLength(32)]
        public string? OperatorUserSid { get; set; }

        [Column("reason", TypeName = "TEXT")]
        public string? Reason { get; set; }

        [Column("correlation_id")]
        [StringLength(100)]
        public string? CorrelationId { get; set; }
    }

    [Table("tax_event")]
    public class TaxEvent
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [Column("entity_type")]
        [StringLength(30)]
        public string EntityType { get; set; } = null!;

        [Required]
        [Column("entity_sid")]
        [StringLength(32)]
        public string EntitySid { get; set; } = null!;

        [Required]
        [Column("event_code")]
        [StringLength(120)]
        public string EventCode { get; set; } = null!;

        [Column("event_version")]
        public int EventVersion { get; set; } = 1;

        [Column("event_data", TypeName = "JSON")]
        public string? EventData { get; set; }

        [Column("source_event_id")]
        [StringLength(100)]
        public string? SourceEventId { get; set; }

        [Column("correlation_id")]
        [StringLength(100)]
        public string? CorrelationId { get; set; }

        [Column("causation_id")]
        [StringLength(100)]
        public string? CausationId { get; set; }

        [Column("outbox_event_sid")]
        [StringLength(32)]
        public string? OutboxEventSid { get; set; }

        [Required]
        [Column("process_status")]
        [StringLength(20)]
        public string ProcessStatus { get; set; } = "PENDING";

        [Column("processed_date")]
        public DateTime? ProcessedDate { get; set; }

        [Column("error_message", TypeName = "TEXT")]
        public string? ErrorMessage { get; set; }
    }

    #endregion
}