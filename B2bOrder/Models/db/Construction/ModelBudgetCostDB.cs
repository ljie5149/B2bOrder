using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Construction.Models
{
    #region Target Budget Module - 建案目標預算主檔與工項預算明細模組

    [Table("bgt_target_budget")]
    public class BgtTargetBudget
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
        [MaxLength(50)]
        [Column("budget_version_code")]
        public string BudgetVersionCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("budget_title")]
        public string BudgetTitle { get; set; }

        [Column("total_target_budget")]
        public decimal TotalTargetBudget { get; set; }

        [Column("approved_vo_budget")]
        public decimal ApprovedVoBudget { get; set; }

        [Column("revised_total_budget")]
        public decimal RevisedTotalBudget { get; set; }

        [Column("committed_amount")]
        public decimal CommittedAmount { get; set; }

        [Column("actual_cost_amount")]
        public decimal ActualCostAmount { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("budget_status")]
        public string BudgetStatus { get; set; }

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

    [Table("bgt_budget_item_line")]
    public class BgtBudgetItemLine
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
        [Column("budget_sid")]
        public string BudgetSid { get; set; }

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string WbsSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("cost_category_code")]
        public string CostCategoryCode { get; set; }

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

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("original_budget_amount")]
        public decimal OriginalBudgetAmount { get; set; }

        [Column("reallocated_amount")]
        public decimal ReallocatedAmount { get; set; }

        [Column("revised_budget_amount")]
        public decimal RevisedBudgetAmount { get; set; }

        [Column("committed_amount")]
        public decimal CommittedAmount { get; set; }

        [Column("actual_cost_amount")]
        public decimal ActualCostAmount { get; set; }

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

    #region Contract Commitments Module - 工程發包承諾成本模組

    [Table("bgt_contract_commitment")]
    public class BgtContractCommitment
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
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("budget_item_sid")]
        public string BudgetItemSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("vendor_sid")]
        public string VendorSid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("contract_number")]
        public string ContractNumber { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("contract_title")]
        public string ContractTitle { get; set; }

        [Column("original_contract_amount")]
        public decimal OriginalContractAmount { get; set; }

        [Column("vo_accumulated_amount")]
        public decimal VoAccumulatedAmount { get; set; }

        [Column("current_contract_amount")]
        public decimal CurrentContractAmount { get; set; }

        [Column("actual_invoiced_amount")]
        public decimal ActualInvoicedAmount { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("commitment_status")]
        public string CommitmentStatus { get; set; }

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

    #region Actual Cost Log Module - 實際成本開支歸攤模組

    [Table("bgt_actual_cost_log")]
    public class BgtActualCostLog
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
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("budget_item_sid")]
        public string BudgetItemSid { get; set; }

        [MaxLength(32)]
        [Column("commitment_sid")]
        public string CommitmentSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("source_document_type")]
        public string SourceDocumentType { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("source_document_sid")]
        public string SourceDocumentSid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("source_document_no")]
        public string SourceDocumentNo { get; set; }

        [MaxLength(32)]
        [Column("vendor_sid")]
        public string VendorSid { get; set; }

        [Column("cost_amount")]
        public decimal CostAmount { get; set; }

        [Column("tax_amount")]
        public decimal TaxAmount { get; set; }

        [Column("total_cost_amount")]
        public decimal TotalCostAmount { get; set; }

        [Column("posting_date", TypeName = "date")]
        public DateTime PostingDate { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Budget Reallocation Module - 預算挪移與流轉模組

    [Table("bgt_budget_reallocation")]
    public class BgtBudgetReallocation
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
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("transfer_number")]
        public string TransferNumber { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("from_budget_item_sid")]
        public string FromBudgetItemSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("to_budget_item_sid")]
        public string ToBudgetItemSid { get; set; }

        [Column("transfer_amount")]
        public decimal TransferAmount { get; set; }

        [Required]
        [Column("reason_description")]
        public string ReasonDescription { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("transfer_status")]
        public string TransferStatus { get; set; }

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
}