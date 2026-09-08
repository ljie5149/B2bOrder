using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.eCommerce
{
    #region 01. 採購類型與政策 (Purchase Type & Policy)

    [Table("pur_purchase_type")]
    public class PurPurchaseType
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("purchase_type_code")]
        public string PurchaseTypeCode { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("purchase_type_name")]
        public string PurchaseTypeName { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("purchase_category")]
        public string PurchaseCategory { get; set; } = null!; // MATERIAL材料;PRODUCT商品;EQUIPMENT設備;SERVICE服務;LABOR勞務;RENTAL租賃;SUBCONTRACT發包;GENERAL一般

        [Column("approval_required")]
        public bool ApprovalRequired { get; set; } = true;

        [Column("receipt_required")]
        public bool ReceiptRequired { get; set; } = true;

        [Column("inspection_required")]
        public bool InspectionRequired { get; set; } = false;

        [Column("inventory_effect")]
        public bool InventoryEffect { get; set; } = true;

        [Column("contract_required")]
        public bool ContractRequired { get; set; } = false;

        [Column("budget_check_required")]
        public bool BudgetCheckRequired { get; set; } = false;

        [Required]
        [MaxLength(20)]
        [Column("type_status")]
        public string TypeStatus { get; set; } = "ACTIVE"; // ACTIVE啟用;INACTIVE停用

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y"; // Y可用;D刪除;W停用

        [Column("remark")]
        public string? Remark { get; set; }
    }

    [Table("pur_policy")]
    public class PurPolicy
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("policy_code")]
        public string PolicyCode { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("policy_name")]
        public string PolicyName { get; set; } = null!;

        [MaxLength(32)]
        [Column("company_sid")]
        public string? CompanySid { get; set; }

        [MaxLength(32)]
        [Column("purchase_type_sid")]
        public string? PurchaseTypeSid { get; set; }

        [Column("minimum_quote_count")]
        public int MinimumQuoteCount { get; set; } = 1;

        [Column("approval_amount", TypeName = "decimal(20,4)")]
        public decimal? ApprovalAmount { get; set; }

        [Column("tender_amount", TypeName = "decimal(20,4)")]
        public decimal? TenderAmount { get; set; }

        [Column("single_source_allowed")]
        public bool SingleSourceAllowed { get; set; } = false;

        [Column("emergency_allowed")]
        public bool EmergencyAllowed { get; set; } = true;

        [Column("split_order_allowed")]
        public bool SplitOrderAllowed { get; set; } = true;

        [Column("over_receipt_rate", TypeName = "decimal(8,4)")]
        public decimal OverReceiptRate { get; set; }

        [Column("under_receipt_rate", TypeName = "decimal(8,4)")]
        public decimal UnderReceiptRate { get; set; }

        [Column("price_variance_rate", TypeName = "decimal(8,4)")]
        public decimal PriceVarianceRate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("policy_status")]
        public string PolicyStatus { get; set; } = "ACTIVE"; // ACTIVE啟用;INACTIVE停用

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y"; // Y可用;D刪除;W停用

        [Column("remark")]
        public string? Remark { get; set; }
    }

    #endregion

    #region 02. 請購 (Requisition)

    [Table("pur_requisition")]
    public class PurRequisition
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("requisition_no")]
        public string RequisitionNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [MaxLength(32)]
        [Column("business_unit_sid")]
        public string? BusinessUnitSid { get; set; }

        [MaxLength(32)]
        [Column("department_sid")]
        public string? DepartmentSid { get; set; }

        [MaxLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [MaxLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [MaxLength(32)]
        [Column("cost_center_sid")]
        public string? CostCenterSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("purchase_type_sid")]
        public string PurchaseTypeSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("requester_user_sid")]
        public string RequesterUserSid { get; set; } = null!;

        [MaxLength(32)]
        [Column("requester_employee_sid")]
        public string? RequesterEmployeeSid { get; set; }

        [Column("request_date")]
        public DateOnly RequestDate { get; set; }

        [Column("required_date")]
        public DateOnly? RequiredDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("estimated_amount", TypeName = "decimal(20,4)")]
        public decimal EstimatedAmount { get; set; }

        [Column("emergency_mark")]
        public bool EmergencyMark { get; set; } = false;

        [Required]
        [MaxLength(30)]
        [Column("source_type")]
        public string SourceType { get; set; } = "MANUAL"; // MANUAL人工;REORDER補貨;PROJECT專案;MAINTENANCE維修;SALES訂單;SYSTEM系統

        [MaxLength(32)]
        [Column("source_reference_sid")]
        public string? SourceReferenceSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("requisition_status")]
        public string RequisitionStatus { get; set; } = "DRAFT"; // DRAFT草稿;SUBMITTED已送出;REVIEW待審;APPROVED核准;REJECTED駁回;PARTIAL_ORDERED部分轉單;ORDERED已轉單;CANCELLED取消;CLOSED結案

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        [Column("reject_reason")]
        public string? RejectReason { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }

        public virtual ICollection<PurRequisitionItem> Items { get; set; } = new List<PurRequisitionItem>();
    }

    [Table("pur_requisition_item")]
    public class PurRequisitionItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("requisition_nid")]
        public ulong RequisitionNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [MaxLength(32)]
        [Column("item_sid")]
        public string? ItemSid { get; set; }

        [MaxLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Required]
        [MaxLength(1000)]
        [Column("item_description")]
        public string ItemDescription { get; set; } = null!;

        [Column("specification_text")]
        public string? SpecificationText { get; set; }

        [Column("quantity", TypeName = "decimal(20,6)")]
        public decimal Quantity { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [Column("estimated_unit_price", TypeName = "decimal(20,6)")]
        public decimal EstimatedUnitPrice { get; set; }

        [Column("estimated_amount", TypeName = "decimal(20,4)")]
        public decimal EstimatedAmount { get; set; }

        [Column("required_date")]
        public DateOnly? RequiredDate { get; set; }

        [MaxLength(32)]
        [Column("warehouse_sid")]
        public string? WarehouseSid { get; set; }

        [MaxLength(32)]
        [Column("location_sid")]
        public string? LocationSid { get; set; }

        [MaxLength(32)]
        [Column("delivery_address_sid")]
        public string? DeliveryAddressSid { get; set; }

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [MaxLength(32)]
        [Column("budget_sid")]
        public string? BudgetSid { get; set; }

        [MaxLength(32)]
        [Column("budget_item_sid")]
        public string? BudgetItemSid { get; set; }

        [MaxLength(32)]
        [Column("contract_sid")]
        public string? ContractSid { get; set; }

        [MaxLength(32)]
        [Column("contract_item_sid")]
        public string? ContractItemSid { get; set; }

        [MaxLength(32)]
        [Column("preferred_supplier_sid")]
        public string? PreferredSupplierSid { get; set; }

        [Column("ordered_qty", TypeName = "decimal(20,6)")]
        public decimal OrderedQty { get; set; }

        [Column("cancelled_qty", TypeName = "decimal(20,6)")]
        public decimal CancelledQty { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "OPEN";

        [Column("remark")]
        public string? Remark { get; set; }

        [ForeignKey("RequisitionNid")]
        public virtual PurRequisition Requisition { get; set; } = null!;
    }

    #endregion

    #region 03. 詢價與比價 (RFQ & Quotation)

    [Table("pur_rfq")]
    public class PurRfq
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("rfq_no")]
        public string RfqNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [MaxLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("purchase_type_sid")]
        public string PurchaseTypeSid { get; set; } = null!;

        [MaxLength(32)]
        [Column("requisition_sid")]
        public string? RequisitionSid { get; set; }

        [Column("issue_date")]
        public DateOnly IssueDate { get; set; }

        [Column("response_deadline")]
        public DateTime ResponseDeadline { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("quotation_valid_days")]
        public int QuotationValidDays { get; set; } = 30;

        [Required]
        [MaxLength(30)]
        [Column("evaluation_method")]
        public string EvaluationMethod { get; set; } = "LOWEST_TOTAL";

        [Required]
        [MaxLength(30)]
        [Column("rfq_status")]
        public string RfqStatus { get; set; } = "DRAFT";

        [Required]
        [MaxLength(32)]
        [Column("create_user_sid")]
        public string CreateUserSid { get; set; } = null!;

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }

        public virtual ICollection<PurRfqItem> Items { get; set; } = new List<PurRfqItem>();
        public virtual ICollection<PurRfqSupplier> Suppliers { get; set; } = new List<PurRfqSupplier>();
    }

    [Table("pur_rfq_item")]
    public class PurRfqItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("rfq_nid")]
        public ulong RfqNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [MaxLength(32)]
        [Column("requisition_item_sid")]
        public string? RequisitionItemSid { get; set; }

        [MaxLength(32)]
        [Column("item_sid")]
        public string? ItemSid { get; set; }

        [MaxLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Required]
        [MaxLength(1000)]
        [Column("item_description")]
        public string ItemDescription { get; set; } = null!;

        [Column("specification_text")]
        public string? SpecificationText { get; set; }

        [Column("quantity", TypeName = "decimal(20,6)")]
        public decimal Quantity { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [Column("required_date")]
        public DateOnly? RequiredDate { get; set; }

        [MaxLength(32)]
        [Column("warehouse_sid")]
        public string? WarehouseSid { get; set; }

        [MaxLength(32)]
        [Column("delivery_address_sid")]
        public string? DeliveryAddressSid { get; set; }

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [MaxLength(32)]
        [Column("budget_item_sid")]
        public string? BudgetItemSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "OPEN";

        [Column("remark")]
        public string? Remark { get; set; }

        [ForeignKey("RfqNid")]
        public virtual PurRfq Rfq { get; set; } = null!;
    }

    [Table("pur_rfq_supplier")]
    public class PurRfqSupplier
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("rfq_nid")]
        public ulong RfqNid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("supplier_party_sid")]
        public string SupplierPartySid { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("invitation_status")]
        public string InvitationStatus { get; set; } = "PENDING";

        [Column("invitation_date")]
        public DateTime? InvitationDate { get; set; }

        [Column("viewed_date")]
        public DateTime? ViewedDate { get; set; }

        [Column("response_date")]
        public DateTime? ResponseDate { get; set; }

        [MaxLength(255)]
        [Column("access_token_hash")]
        public string? AccessTokenHash { get; set; }

        [Column("token_expiry_date")]
        public DateTime? TokenExpiryDate { get; set; }

        [ForeignKey("RfqNid")]
        public virtual PurRfq Rfq { get; set; } = null!;
    }

    [Table("pur_supplier_quote")]
    public class PurSupplierQuote
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("quote_no")]
        public string QuoteNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("rfq_sid")]
        public string RfqSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("supplier_party_sid")]
        public string SupplierPartySid { get; set; } = null!;

        [MaxLength(150)]
        [Column("supplier_quote_no")]
        public string? SupplierQuoteNo { get; set; }

        [Column("quote_date")]
        public DateOnly QuoteDate { get; set; }

        [Column("valid_until")]
        public DateOnly? ValidUntil { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("exchange_rate", TypeName = "decimal(20,10)")]
        public decimal ExchangeRate { get; set; } = 1;

        [Column("subtotal_amount", TypeName = "decimal(20,4)")]
        public decimal SubtotalAmount { get; set; }

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; }

        [Column("freight_amount", TypeName = "decimal(20,4)")]
        public decimal FreightAmount { get; set; }

        [Column("other_amount", TypeName = "decimal(20,4)")]
        public decimal OtherAmount { get; set; }

        [Column("total_amount", TypeName = "decimal(20,4)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(32)]
        [Column("payment_term_sid")]
        public string? PaymentTermSid { get; set; }

        [MaxLength(50)]
        [Column("delivery_term_code")]
        public string? DeliveryTermCode { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("quote_status")]
        public string QuoteStatus { get; set; } = "DRAFT";

        [Column("submitted_date")]
        public DateTime? SubmittedDate { get; set; }

        [MaxLength(32)]
        [Column("file_sid")]
        public string? FileSid { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }

        public virtual ICollection<PurSupplierQuoteItem> Items { get; set; } = new List<PurSupplierQuoteItem>();
    }

    [Table("pur_supplier_quote_item")]
    public class PurSupplierQuoteItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("supplier_quote_nid")]
        public ulong SupplierQuoteNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("rfq_item_sid")]
        public string RfqItemSid { get; set; } = null!;

        [MaxLength(150)]
        [Column("supplier_item_no")]
        public string? SupplierItemNo { get; set; }

        [Required]
        [MaxLength(1000)]
        [Column("item_description")]
        public string ItemDescription { get; set; } = null!;

        [Column("quoted_qty", TypeName = "decimal(20,6)")]
        public decimal QuotedQty { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [Column("unit_price", TypeName = "decimal(20,6)")]
        public decimal UnitPrice { get; set; }

        [Column("discount_rate", TypeName = "decimal(8,4)")]
        public decimal DiscountRate { get; set; }

        [MaxLength(32)]
        [Column("tax_sid")]
        public string? TaxSid { get; set; }

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; }

        [Column("line_amount", TypeName = "decimal(20,4)")]
        public decimal LineAmount { get; set; }

        [Column("lead_time_days")]
        public int LeadTimeDays { get; set; }

        [Column("promised_date")]
        public DateOnly? PromisedDate { get; set; }

        [Column("minimum_order_qty", TypeName = "decimal(20,6)")]
        public decimal? MinimumOrderQty { get; set; }

        [Column("warranty_months")]
        public int? WarrantyMonths { get; set; }

        [Column("compliance_result")]
        public string? ComplianceResult { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("award_status")]
        public string AwardStatus { get; set; } = "PENDING";

        [Column("awarded_qty", TypeName = "decimal(20,6)")]
        public decimal AwardedQty { get; set; }

        [Column("remark")]
        public string? Remark { get; set; }

        [ForeignKey("SupplierQuoteNid")]
        public virtual PurSupplierQuote SupplierQuote { get; set; } = null!;
    }

    [Table("pur_quote_evaluation")]
    public class PurQuoteEvaluation
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(32)]
        [Column("rfq_sid")]
        public string RfqSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("supplier_quote_sid")]
        public string SupplierQuoteSid { get; set; } = null!;

        [Column("price_score", TypeName = "decimal(8,4)")]
        public decimal PriceScore { get; set; }

        [Column("delivery_score", TypeName = "decimal(8,4)")]
        public decimal DeliveryScore { get; set; }

        [Column("quality_score", TypeName = "decimal(8,4)")]
        public decimal QualityScore { get; set; }

        [Column("service_score", TypeName = "decimal(8,4)")]
        public decimal ServiceScore { get; set; }

        [Column("compliance_score", TypeName = "decimal(8,4)")]
        public decimal ComplianceScore { get; set; }

        [Column("total_score", TypeName = "decimal(8,4)")]
        public decimal TotalScore { get; set; }

        [Column("rank_no")]
        public int? RankNo { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("recommendation")]
        public string Recommendation { get; set; } = "PENDING";

        [MaxLength(32)]
        [Column("evaluator_user_sid")]
        public string? EvaluatorUserSid { get; set; }

        [Column("evaluation_note")]
        public string? EvaluationNote { get; set; }
    }

    #endregion

    #region 04. 採購單 (Purchase Order)

    [Table("pur_order")]
    public class PurOrder
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("purchase_order_no")]
        public string PurchaseOrderNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [MaxLength(32)]
        [Column("business_unit_sid")]
        public string? BusinessUnitSid { get; set; }

        [MaxLength(32)]
        [Column("department_sid")]
        public string? DepartmentSid { get; set; }

        [MaxLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [MaxLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [MaxLength(32)]
        [Column("cost_center_sid")]
        public string? CostCenterSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("purchase_type_sid")]
        public string PurchaseTypeSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("supplier_party_sid")]
        public string SupplierPartySid { get; set; } = null!;

        [MaxLength(32)]
        [Column("requisition_sid")]
        public string? RequisitionSid { get; set; }

        [MaxLength(32)]
        [Column("rfq_sid")]
        public string? RfqSid { get; set; }

        [MaxLength(32)]
        [Column("supplier_quote_sid")]
        public string? SupplierQuoteSid { get; set; }

        [MaxLength(32)]
        [Column("agreement_sid")]
        public string? AgreementSid { get; set; }

        [MaxLength(32)]
        [Column("contract_sid")]
        public string? ContractSid { get; set; }

        [Column("order_date")]
        public DateOnly OrderDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("exchange_rate", TypeName = "decimal(20,10)")]
        public decimal ExchangeRate { get; set; } = 1;

        [Column("subtotal_amount", TypeName = "decimal(20,4)")]
        public decimal SubtotalAmount { get; set; }

        [Column("discount_amount", TypeName = "decimal(20,4)")]
        public decimal DiscountAmount { get; set; }

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; }

        [Column("freight_amount", TypeName = "decimal(20,4)")]
        public decimal FreightAmount { get; set; }

        [Column("other_amount", TypeName = "decimal(20,4)")]
        public decimal OtherAmount { get; set; }

        [Column("total_amount", TypeName = "decimal(20,4)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(32)]
        [Column("payment_term_sid")]
        public string? PaymentTermSid { get; set; }

        [MaxLength(32)]
        [Column("payment_method_sid")]
        public string? PaymentMethodSid { get; set; }

        [MaxLength(50)]
        [Column("delivery_term_code")]
        public string? DeliveryTermCode { get; set; }

        [Column("expected_delivery_date")]
        public DateOnly? ExpectedDeliveryDate { get; set; }

        [MaxLength(32)]
        [Column("warehouse_sid")]
        public string? WarehouseSid { get; set; }

        [MaxLength(32)]
        [Column("delivery_address_sid")]
        public string? DeliveryAddressSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("order_status")]
        public string OrderStatus { get; set; } = "DRAFT";

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        [Column("issued_date")]
        public DateTime? IssuedDate { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }

        public virtual ICollection<PurOrderItem> Items { get; set; } = new List<PurOrderItem>();
    }

    [Table("pur_order_item")]
    public class PurOrderItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("purchase_order_nid")]
        public ulong PurchaseOrderNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [MaxLength(32)]
        [Column("requisition_item_sid")]
        public string? RequisitionItemSid { get; set; }

        [MaxLength(32)]
        [Column("rfq_item_sid")]
        public string? RfqItemSid { get; set; }

        [MaxLength(32)]
        [Column("quote_item_sid")]
        public string? QuoteItemSid { get; set; }

        [MaxLength(32)]
        [Column("item_sid")]
        public string? ItemSid { get; set; }

        [MaxLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Required]
        [MaxLength(1000)]
        [Column("item_description")]
        public string ItemDescription { get; set; } = null!;

        [Column("specification_text")]
        public string? SpecificationText { get; set; }

        [MaxLength(150)]
        [Column("supplier_item_no")]
        public string? SupplierItemNo { get; set; }

        [Column("order_qty", TypeName = "decimal(20,6)")]
        public decimal OrderQty { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [Column("unit_price", TypeName = "decimal(20,6)")]
        public decimal UnitPrice { get; set; }

        [Column("discount_rate", TypeName = "decimal(8,4)")]
        public decimal DiscountRate { get; set; }

        [Column("discount_amount", TypeName = "decimal(20,4)")]
        public decimal DiscountAmount { get; set; }

        [MaxLength(32)]
        [Column("tax_sid")]
        public string? TaxSid { get; set; }

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; }

        [Column("line_amount", TypeName = "decimal(20,4)")]
        public decimal LineAmount { get; set; }

        [Column("expected_delivery_date")]
        public DateOnly? ExpectedDeliveryDate { get; set; }

        [MaxLength(32)]
        [Column("warehouse_sid")]
        public string? WarehouseSid { get; set; }

        [MaxLength(32)]
        [Column("location_sid")]
        public string? LocationSid { get; set; }

        [MaxLength(32)]
        [Column("delivery_address_sid")]
        public string? DeliveryAddressSid { get; set; }

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [MaxLength(32)]
        [Column("budget_sid")]
        public string? BudgetSid { get; set; }

        [MaxLength(32)]
        [Column("budget_item_sid")]
        public string? BudgetItemSid { get; set; }

        [MaxLength(32)]
        [Column("contract_item_sid")]
        public string? ContractItemSid { get; set; }

        [Column("received_qty", TypeName = "decimal(20,6)")]
        public decimal ReceivedQty { get; set; }

        [Column("accepted_qty", TypeName = "decimal(20,6)")]
        public decimal AcceptedQty { get; set; }

        [Column("returned_qty", TypeName = "decimal(20,6)")]
        public decimal ReturnedQty { get; set; }

        [Column("cancelled_qty", TypeName = "decimal(20,6)")]
        public decimal CancelledQty { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "OPEN";

        [Column("remark")]
        public string? Remark { get; set; }

        [ForeignKey("PurchaseOrderNid")]
        public virtual PurOrder PurchaseOrder { get; set; } = null!;
        public virtual ICollection<PurOrderSchedule> Schedules { get; set; } = new List<PurOrderSchedule>();
    }

    [Table("pur_order_schedule")]
    public class PurOrderSchedule
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("purchase_order_item_nid")]
        public ulong PurchaseOrderItemNid { get; set; }

        [Column("schedule_no")]
        public int ScheduleNo { get; set; }

        [Column("scheduled_qty", TypeName = "decimal(20,6)")]
        public decimal ScheduledQty { get; set; }

        [Column("planned_delivery_date")]
        public DateOnly PlannedDeliveryDate { get; set; }

        [Column("confirmed_delivery_date")]
        public DateOnly? ConfirmedDeliveryDate { get; set; }

        [Column("actual_delivery_date")]
        public DateOnly? ActualDeliveryDate { get; set; }

        [MaxLength(32)]
        [Column("warehouse_sid")]
        public string? WarehouseSid { get; set; }

        [MaxLength(32)]
        [Column("delivery_address_sid")]
        public string? DeliveryAddressSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("schedule_status")]
        public string ScheduleStatus { get; set; } = "PLANNED";

        [Column("delay_reason")]
        public string? DelayReason { get; set; }

        [ForeignKey("PurchaseOrderItemNid")]
        public virtual PurOrderItem PurchaseOrderItem { get; set; } = null!;
    }

    #endregion

    #region 05. 收貨與到場 (Receipt)

    [Table("pur_receipt")]
    public class PurReceipt
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("receipt_no")]
        public string ReceiptNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("purchase_order_sid")]
        public string PurchaseOrderSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("supplier_party_sid")]
        public string SupplierPartySid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [MaxLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [MaxLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [Column("receipt_date")]
        public DateTime ReceiptDate { get; set; }

        [MaxLength(150)]
        [Column("delivery_note_no")]
        public string? DeliveryNoteNo { get; set; }

        [MaxLength(100)]
        [Column("vehicle_no")]
        public string? VehicleNo { get; set; }

        [MaxLength(100)]
        [Column("driver_name")]
        public string? DriverName { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("receiver_user_sid")]
        public string ReceiverUserSid { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("source_type")]
        public string SourceType { get; set; } = "PURCHASE_ORDER";

        [MaxLength(32)]
        [Column("inventory_receipt_sid")]
        public string? InventoryReceiptSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("receipt_status")]
        public string ReceiptStatus { get; set; } = "DRAFT";

        [MaxLength(32)]
        [Column("file_sid")]
        public string? FileSid { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }

        public virtual ICollection<PurReceiptItem> Items { get; set; } = new List<PurReceiptItem>();
    }

    [Table("pur_receipt_item")]
    public class PurReceiptItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("receipt_nid")]
        public ulong ReceiptNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("purchase_order_item_sid")]
        public string PurchaseOrderItemSid { get; set; } = null!;

        [MaxLength(32)]
        [Column("schedule_sid")]
        public string? ScheduleSid { get; set; }

        [MaxLength(32)]
        [Column("item_sid")]
        public string? ItemSid { get; set; }

        [MaxLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Column("received_qty", TypeName = "decimal(20,6)")]
        public decimal ReceivedQty { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [Column("accepted_qty", TypeName = "decimal(20,6)")]
        public decimal AcceptedQty { get; set; }

        [Column("rejected_qty", TypeName = "decimal(20,6)")]
        public decimal RejectedQty { get; set; }

        [Column("pending_qty", TypeName = "decimal(20,6)")]
        public decimal PendingQty { get; set; }

        [MaxLength(150)]
        [Column("batch_no")]
        public string? BatchNo { get; set; }

        [Column("manufacture_date")]
        public DateOnly? ManufactureDate { get; set; }

        [Column("expiry_date")]
        public DateOnly? ExpiryDate { get; set; }

        [MaxLength(32)]
        [Column("location_sid")]
        public string? LocationSid { get; set; }

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [Column("inspection_required")]
        public bool InspectionRequired { get; set; } = false;

        [Required]
        [MaxLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "RECEIVED";

        [Column("remark")]
        public string? Remark { get; set; }

        [ForeignKey("ReceiptNid")]
        public virtual PurReceipt Receipt { get; set; } = null!;
    }

    #endregion

    #region 06. 驗收 (Inspection)

    [Table("pur_inspection")]
    public class PurInspection
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("inspection_no")]
        public string InspectionNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("receipt_sid")]
        public string ReceiptSid { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("inspection_type")]
        public string InspectionType { get; set; } = null!;

        [Column("inspection_date")]
        public DateTime InspectionDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("inspector_user_sid")]
        public string InspectorUserSid { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("inspection_result")]
        public string InspectionResult { get; set; } = "PENDING";

        [Required]
        [MaxLength(20)]
        [Column("inspection_status")]
        public string InspectionStatus { get; set; } = "DRAFT";

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [MaxLength(32)]
        [Column("file_sid")]
        public string? FileSid { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }

        public virtual ICollection<PurInspectionItem> Items { get; set; } = new List<PurInspectionItem>();
    }

    [Table("pur_inspection_item")]
    public class PurInspectionItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("inspection_nid")]
        public ulong InspectionNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("receipt_item_sid")]
        public string ReceiptItemSid { get; set; } = null!;

        [Column("inspected_qty", TypeName = "decimal(20,6)")]
        public decimal InspectedQty { get; set; }

        [Column("accepted_qty", TypeName = "decimal(20,6)")]
        public decimal AcceptedQty { get; set; }

        [Column("rejected_qty", TypeName = "decimal(20,6)")]
        public decimal RejectedQty { get; set; }

        [Column("conditional_qty", TypeName = "decimal(20,6)")]
        public decimal ConditionalQty { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("result")]
        public string Result { get; set; } = null!;

        [MaxLength(100)]
        [Column("defect_code")]
        public string? DefectCode { get; set; }

        [Column("defect_description")]
        public string? DefectDescription { get; set; }

        [MaxLength(30)]
        [Column("disposition_type")]
        public string? DispositionType { get; set; }

        [MaxLength(32)]
        [Column("responsible_party_sid")]
        public string? ResponsiblePartySid { get; set; }

        [Column("due_date")]
        public DateOnly? DueDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "OPEN";

        [Column("remark")]
        public string? Remark { get; set; }

        [ForeignKey("InspectionNid")]
        public virtual PurInspection Inspection { get; set; } = null!;
    }

    #endregion

    #region 07. 採購退貨 (Return)

    [Table("pur_return")]
    public class PurReturn
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("return_no")]
        public string ReturnNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("supplier_party_sid")]
        public string SupplierPartySid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("purchase_order_sid")]
        public string PurchaseOrderSid { get; set; } = null!;

        [MaxLength(32)]
        [Column("receipt_sid")]
        public string? ReceiptSid { get; set; }

        [MaxLength(32)]
        [Column("inspection_sid")]
        public string? InspectionSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [MaxLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [Column("return_date")]
        public DateOnly ReturnDate { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("reason_code")]
        public string ReasonCode { get; set; } = null!;

        [Column("reason_description")]
        public string? ReasonDescription { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("total_amount", TypeName = "decimal(20,4)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("return_status")]
        public string ReturnStatus { get; set; } = "DRAFT";

        [MaxLength(32)]
        [Column("inventory_issue_sid")]
        public string? InventoryIssueSid { get; set; }

        [MaxLength(32)]
        [Column("allowance_sid")]
        public string? AllowanceSid { get; set; }

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }

        public virtual ICollection<PurReturnItem> Items { get; set; } = new List<PurReturnItem>();
    }

    [Table("pur_return_item")]
    public class PurReturnItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("return_nid")]
        public ulong ReturnNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("purchase_order_item_sid")]
        public string PurchaseOrderItemSid { get; set; } = null!;

        [MaxLength(32)]
        [Column("receipt_item_sid")]
        public string? ReceiptItemSid { get; set; }

        [MaxLength(32)]
        [Column("inspection_item_sid")]
        public string? InspectionItemSid { get; set; }

        [MaxLength(32)]
        [Column("item_sid")]
        public string? ItemSid { get; set; }

        [MaxLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Column("return_qty", TypeName = "decimal(20,6)")]
        public decimal ReturnQty { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [Column("unit_price", TypeName = "decimal(20,6)")]
        public decimal UnitPrice { get; set; }

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; }

        [Column("line_amount", TypeName = "decimal(20,4)")]
        public decimal LineAmount { get; set; }

        [MaxLength(150)]
        [Column("batch_no")]
        public string? BatchNo { get; set; }

        [Column("serial_data")]
        public string? SerialData { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("disposition_type")]
        public string DispositionType { get; set; } = "RETURN";

        [Required]
        [MaxLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "OPEN";

        [Column("remark")]
        public string? Remark { get; set; }

        [ForeignKey("ReturnNid")]
        public virtual PurReturn Return { get; set; } = null!;
    }

    #endregion

    #region 08. 採購變更與取消 (Order Change)

    [Table("pur_order_change")]
    public class PurOrderChange
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("change_no")]
        public string ChangeNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("purchase_order_sid")]
        public string PurchaseOrderSid { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("change_type")]
        public string ChangeType { get; set; } = null!;

        [Column("before_data")]
        public string? BeforeData { get; set; }

        [Required]
        [Column("requested_data")]
        public string RequestedData { get; set; } = null!;

        [Column("changed_fields")]
        public string? ChangedFields { get; set; }

        [Required]
        [Column("change_reason")]
        public string ChangeReason { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("requester_user_sid")]
        public string RequesterUserSid { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("change_status")]
        public string ChangeStatus { get; set; } = "DRAFT";

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        [Column("applied_date")]
        public DateTime? AppliedDate { get; set; }
    }

    #endregion

    #region 09. 供應商績效 (Supplier Performance)

    [Table("pur_supplier_performance")]
    public class PurSupplierPerformance
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(32)]
        [Column("supplier_party_sid")]
        public string SupplierPartySid { get; set; } = null!;

        [MaxLength(32)]
        [Column("company_sid")]
        public string? CompanySid { get; set; }

        [MaxLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [Column("period_start_date")]
        public DateOnly PeriodStartDate { get; set; }

        [Column("period_end_date")]
        public DateOnly PeriodEndDate { get; set; }

        [Column("order_count")]
        public int OrderCount { get; set; }

        [Column("on_time_delivery_rate", TypeName = "decimal(8,4)")]
        public decimal OnTimeDeliveryRate { get; set; }

        [Column("acceptance_rate", TypeName = "decimal(8,4)")]
        public decimal AcceptanceRate { get; set; }

        [Column("return_rate", TypeName = "decimal(8,4)")]
        public decimal ReturnRate { get; set; }

        [Column("price_variance_rate", TypeName = "decimal(8,4)")]
        public decimal PriceVarianceRate { get; set; }

        [Column("response_score", TypeName = "decimal(8,4)")]
        public decimal ResponseScore { get; set; }

        [Column("quality_score", TypeName = "decimal(8,4)")]
        public decimal QualityScore { get; set; }

        [Column("delivery_score", TypeName = "decimal(8,4)")]
        public decimal DeliveryScore { get; set; }

        [Column("service_score", TypeName = "decimal(8,4)")]
        public decimal ServiceScore { get; set; }

        [Column("compliance_score", TypeName = "decimal(8,4)")]
        public decimal ComplianceScore { get; set; }

        [Column("total_score", TypeName = "decimal(8,4)")]
        public decimal TotalScore { get; set; }

        [MaxLength(20)]
        [Column("rating_level")]
        public string? RatingLevel { get; set; }

        [Column("evaluation_data")]
        public string? EvaluationData { get; set; }

        [MaxLength(32)]
        [Column("evaluator_user_sid")]
        public string? EvaluatorUserSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("performance_status")]
        public string PerformanceStatus { get; set; } = "ACTIVE";
    }

    #endregion

    #region 10. 採購事件與歷程 (Events & History)

    [Table("pur_status_history")]
    public class PurStatusHistory
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(30)]
        [Column("entity_type")]
        public string EntityType { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("entity_sid")]
        public string EntitySid { get; set; } = null!;

        [MaxLength(30)]
        [Column("old_status")]
        public string? OldStatus { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("new_status")]
        public string NewStatus { get; set; } = null!;

        [MaxLength(100)]
        [Column("event_code")]
        public string? EventCode { get; set; }

        [MaxLength(80)]
        [Column("source_service_code")]
        public string? SourceServiceCode { get; set; }

        [MaxLength(32)]
        [Column("source_reference_sid")]
        public string? SourceReferenceSid { get; set; }

        [MaxLength(32)]
        [Column("operator_user_sid")]
        public string? OperatorUserSid { get; set; }

        [MaxLength(50)]
        [Column("reason_code")]
        public string? ReasonCode { get; set; }

        [Column("reason")]
        public string? Reason { get; set; }

        [MaxLength(100)]
        [Column("correlation_id")]
        public string? CorrelationId { get; set; }
    }

    [Table("pur_event")]
    public class PurEvent
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(30)]
        [Column("entity_type")]
        public string EntityType { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("entity_sid")]
        public string EntitySid { get; set; } = null!;

        [Required]
        [MaxLength(120)]
        [Column("event_code")]
        public string EventCode { get; set; } = null!;

        [Column("event_version")]
        public int EventVersion { get; set; } = 1;

        [Column("event_data")]
        public string? EventData { get; set; }

        [MaxLength(100)]
        [Column("source_event_id")]
        public string? SourceEventId { get; set; }

        [MaxLength(100)]
        [Column("correlation_id")]
        public string? CorrelationId { get; set; }

        [MaxLength(100)]
        [Column("causation_id")]
        public string? CausationId { get; set; }

        [MaxLength(32)]
        [Column("outbox_event_sid")]
        public string? OutboxEventSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("process_status")]
        public string ProcessStatus { get; set; } = "PENDING";

        [Column("processed_date")]
        public DateTime? ProcessedDate { get; set; }

        [Column("error_message")]
        public string? ErrorMessage { get; set; }
    }

    #endregion
}