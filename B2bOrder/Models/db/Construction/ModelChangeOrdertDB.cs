using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Construction.Models
{
    #region Change Order Master Module - 工程變更追加減主檔模組

    [Table("cho_change_order_master")]
    public class ChoChangeOrderMaster
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
        [Column("change_order_no")]
        public string ChangeOrderNo { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("change_title")]
        public string ChangeTitle { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("change_reason_type")]
        public string ChangeReasonType { get; set; }

        [Column("original_contract_amount")]
        public decimal OriginalContractAmount { get; set; }

        [Column("change_amount_before_tax")]
        public decimal ChangeAmountBeforeTax { get; set; }

        [Column("tax_rate")]
        public decimal TaxRate { get; set; }

        [Column("change_tax_amount")]
        public decimal ChangeTaxAmount { get; set; }

        [Column("change_total_amount")]
        public decimal ChangeTotalAmount { get; set; }

        [Column("revised_contract_amount")]
        public decimal RevisedContractAmount { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("is_time_extension_needed")]
        public string IsTimeExtensionNeeded { get; set; }

        [Column("requested_extension_days")]
        public int RequestedExtensionDays { get; set; }

        [Column("approved_extension_days")]
        public int ApprovedExtensionDays { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("order_status")]
        public string OrderStatus { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("applicant_user_sid")]
        public string ApplicantUserSid { get; set; }

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

    #region Change Order Line Module - 變更工項與單價明細模組

    [Table("cho_change_order_line")]
    public class ChoChangeOrderLine
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
        [Column("change_order_sid")]
        public string ChangeOrderSid { get; set; }

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string WbsSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("item_type")]
        public string ItemType { get; set; }

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

        [Column("original_quantity")]
        public decimal OriginalQuantity { get; set; }

        [Column("change_quantity")]
        public decimal ChangeQuantity { get; set; }

        [Column("revised_quantity")]
        public decimal RevisedQuantity { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("is_new_unit_price")]
        public string IsNewUnitPrice { get; set; }

        [Column("subtotal_amount")]
        public decimal SubtotalAmount { get; set; }

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

    #region Time Extension Analysis Module - 工期展延評估模組

    [Table("cho_time_extension_analysis")]
    public class ChoTimeExtensionAnalysis
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
        [Column("change_order_sid")]
        public string ChangeOrderSid { get; set; }

        [MaxLength(32)]
        [Column("impacted_wbs_sid")]
        public string ImpactedWbsSid { get; set; }

        [Required]
        [Column("impact_description")]
        public string ImpactDescription { get; set; }

        [Column("delay_start_date", TypeName = "date")]
        public DateTime? DelayStartDate { get; set; }

        [Column("delay_end_date", TypeName = "date")]
        public DateTime? DelayEndDate { get; set; }

        [Column("applied_days")]
        public int AppliedDays { get; set; }

        [Column("approved_days")]
        public int ApprovedDays { get; set; }

        [Column("revised_project_end_date", TypeName = "date")]
        public DateTime? RevisedProjectEndDate { get; set; }

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

    #region Approval Log Module - 變更單簽核歷程模組

    [Table("cho_approval_log")]
    public class ChoApprovalLog
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

        [Required]
        [MaxLength(32)]
        [Column("change_order_sid")]
        public string ChangeOrderSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("reviewer_user_sid")]
        public string ReviewerUserSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("reviewer_role")]
        public string ReviewerRole { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("review_action")]
        public string ReviewAction { get; set; }

        [Column("review_comment")]
        public string ReviewComment { get; set; }
    }

    #endregion
}