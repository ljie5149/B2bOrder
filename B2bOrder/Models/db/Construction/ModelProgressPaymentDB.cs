using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Construction.Models
{
    #region Progress Payment Valuation Master Module - 工程估驗請款主檔模組

    [Table("pgp_payment_valuation")]
    public class PgpPaymentValuation
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
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("contract_sid")]
        public string ContractSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("vendor_sid")]
        public string VendorSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("valuation_no")]
        public string ValuationNo { get; set; }

        [Column("period_number")]
        public int PeriodNumber { get; set; }

        [Column("cutoff_date", TypeName = "date")]
        public DateTime CutoffDate { get; set; }

        [Column("accumulated_previous_amount")]
        public decimal AccumulatedPreviousAmount { get; set; }

        [Column("current_claimed_amount")]
        public decimal CurrentClaimedAmount { get; set; }

        [Column("current_approved_amount")]
        public decimal CurrentApprovedAmount { get; set; }

        [Column("accumulated_approved_amount")]
        public decimal AccumulatedApprovedAmount { get; set; }

        [Column("retention_rate")]
        public decimal RetentionRate { get; set; }

        [Column("current_retention_deduction")]
        public decimal CurrentRetentionDeduction { get; set; }

        [Column("current_advance_deduction")]
        public decimal CurrentAdvanceDeduction { get; set; }

        [Column("current_backcharge_deduction")]
        public decimal CurrentBackchargeDeduction { get; set; }

        [Column("price_escalation_amount")]
        public decimal PriceEscalationAmount { get; set; }

        [Column("net_approved_before_tax")]
        public decimal NetApprovedBeforeTax { get; set; }

        [Column("tax_rate")]
        public decimal TaxRate { get; set; }

        [Column("tax_amount")]
        public decimal TaxAmount { get; set; }

        [Column("total_payable_amount")]
        public decimal TotalPayableAmount { get; set; }

        [MaxLength(100)]
        [Column("invoice_number")]
        public string InvoiceNumber { get; set; }

        [Column("invoice_date", TypeName = "date")]
        public DateTime? InvoiceDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("valuation_status")]
        public string ValuationStatus { get; set; }

        [MaxLength(32)]
        [Column("approved_by_user_sid")]
        public string ApprovedByUserSid { get; set; }

        [Column("approved_at")]
        public DateTime? ApprovedAt { get; set; }

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

    #region Valuation Line Items Module - 工程估驗工項明細模組

    [Table("pgp_valuation_item_line")]
    public class PgpValuationItemLine
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
        [Column("valuation_sid")]
        public string ValuationSid { get; set; }

        [MaxLength(32)]
        [Column("contract_item_sid")]
        public string ContractItemSid { get; set; }

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string WbsSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("item_code")]
        public string ItemCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("item_name")]
        public string ItemName { get; set; }

        [MaxLength(20)]
        [Column("unit")]
        public string Unit { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("contract_quantity")]
        public decimal ContractQuantity { get; set; }

        [Column("previous_quantity")]
        public decimal PreviousQuantity { get; set; }

        [Column("current_claimed_qty")]
        public decimal CurrentClaimedQty { get; set; }

        [Column("current_approved_qty")]
        public decimal CurrentApprovedQty { get; set; }

        [Column("accumulated_approved_qty")]
        public decimal AccumulatedApprovedQty { get; set; }

        [Column("completion_rate")]
        public decimal CompletionRate { get; set; }

        [Column("current_approved_amount")]
        public decimal CurrentApprovedAmount { get; set; }

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

    #region Backcharge & Deductions Module - 工程代扣與扣罰款明細模組

    [Table("pgp_backcharge_line")]
    public class PgpBackchargeLine
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
        [Column("valuation_sid")]
        public string ValuationSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("deduction_type")]
        public string DeductionType { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("deduction_title")]
        public string DeductionTitle { get; set; }

        [Column("deduction_amount")]
        public decimal DeductionAmount { get; set; }

        [MaxLength(100)]
        [Column("supporting_document_no")]
        public string SupportingDocumentNo { get; set; }

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

    #region Retention Money Release Module - 工程保留金退還與釋放模組

    [Table("pgp_retention_release")]
    public class PgpRetentionRelease
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
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("contract_sid")]
        public string ContractSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("vendor_sid")]
        public string VendorSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("release_number")]
        public string ReleaseNumber { get; set; }

        [Column("total_retained_amount")]
        public decimal TotalRetainedAmount { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("release_stage")]
        public string ReleaseStage { get; set; }

        [Column("requested_release_amount")]
        public decimal RequestedReleaseAmount { get; set; }

        [Column("approved_release_amount")]
        public decimal ApprovedReleaseAmount { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("release_status")]
        public string ReleaseStatus { get; set; }

        [MaxLength(32)]
        [Column("approved_by_user_sid")]
        public string ApprovedByUserSid { get; set; }

        [Column("approved_at")]
        public DateTime? ApprovedAt { get; set; }

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