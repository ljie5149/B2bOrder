using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Construction.Models
{
    #region Contract Master Module - 工程與採購合約主檔模組

    [Table("ctr_contract_master")]
    public class CtrContractMaster
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
        [Column("vendor_sid")]
        public string VendorSid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("contract_number")]
        public string ContractNumber { get; set; }

        [Required]
        [MaxLength(250)]
        [Column("contract_title")]
        public string ContractTitle { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("contract_type")]
        public string ContractType { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("tax_type")]
        public string TaxType { get; set; }

        [Column("original_contract_amount")]
        public decimal OriginalContractAmount { get; set; }

        [Column("original_tax_amount")]
        public decimal OriginalTaxAmount { get; set; }

        [Column("original_total_amount")]
        public decimal OriginalTotalAmount { get; set; }

        [Column("current_contract_amount")]
        public decimal CurrentContractAmount { get; set; }

        [Column("sign_date", TypeName = "date")]
        public DateTime? SignDate { get; set; }

        [Column("effective_start_date", TypeName = "date")]
        public DateTime? EffectiveStartDate { get; set; }

        [Column("effective_end_date", TypeName = "date")]
        public DateTime? EffectiveEndDate { get; set; }

        [Column("retention_rate")]
        public decimal RetentionRate { get; set; }

        [Column("advance_payment_amount")]
        public decimal AdvancePaymentAmount { get; set; }

        [Column("liquidated_damages_rate")]
        public decimal LiquidatedDamagesRate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("contract_status")]
        public string ContractStatus { get; set; }

        [MaxLength(32)]
        [Column("legal_reviewed_by_sid")]
        public string LegalReviewedBySid { get; set; }

        [Column("legal_reviewed_at")]
        public DateTime? LegalReviewedAt { get; set; }

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

    #region Payment Terms Module - 合約估驗與付款條件模組

    [Table("ctr_payment_term")]
    public class CtrPaymentTerm
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
        [Column("contract_sid")]
        public string ContractSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("payment_stage_code")]
        public string PaymentStageCode { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("stage_name")]
        public string StageName { get; set; }

        [Column("percentage_rate")]
        public decimal PercentageRate { get; set; }

        [MaxLength(32)]
        [Column("milestone_wbs_sid")]
        public string MilestoneWbsSid { get; set; }

        [Column("cash_ratio")]
        public decimal CashRatio { get; set; }

        [Column("ticket_ratio")]
        public decimal TicketRatio { get; set; }

        [Column("ticket_days")]
        public int TicketDays { get; set; }

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

    #region Contract Amendment Module - 合約補充協議與變更單模組

    [Table("ctr_contract_amendment")]
    public class CtrContractAmendment
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
        [Column("contract_sid")]
        public string ContractSid { get; set; }

        [MaxLength(32)]
        [Column("variation_order_sid")]
        public string VariationOrderSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("amendment_number")]
        public string AmendmentNumber { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("amendment_title")]
        public string AmendmentTitle { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("change_type")]
        public string ChangeType { get; set; }

        [Column("change_amount")]
        public decimal ChangeAmount { get; set; }

        [Column("revised_total_amount")]
        public decimal RevisedTotalAmount { get; set; }

        [Column("extended_days")]
        public int ExtendedDays { get; set; }

        [Column("revised_end_date", TypeName = "date")]
        public DateTime? RevisedEndDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("amendment_status")]
        public string AmendmentStatus { get; set; }

        [Column("sign_date", TypeName = "date")]
        public DateTime? SignDate { get; set; }

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

    #region Bond Guarantee Module - 履約與保固保證金管理模組

    [Table("ctr_bond_guarantee")]
    public class CtrBondGuarantee
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
        [Column("contract_sid")]
        public string ContractSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("vendor_sid")]
        public string VendorSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("bond_type")]
        public string BondType { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("bond_mode")]
        public string BondMode { get; set; }

        [MaxLength(100)]
        [Column("guarantee_bank_name")]
        public string GuaranteeBankName { get; set; }

        [MaxLength(100)]
        [Column("bond_number")]
        public string BondNumber { get; set; }

        [Column("bond_amount")]
        public decimal BondAmount { get; set; }

        [Column("effective_start_date", TypeName = "date")]
        public DateTime EffectiveStartDate { get; set; }

        [Column("effective_end_date", TypeName = "date")]
        public DateTime EffectiveEndDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("bond_status")]
        public string BondStatus { get; set; }

        [Column("release_date", TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }

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

    #region Warranty Term Module - 工程保固條款與保固期模組

    [Table("ctr_warranty_term")]
    public class CtrWarrantyTerm
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
        [Column("contract_sid")]
        public string ContractSid { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("item_scope_name")]
        public string ItemScopeName { get; set; }

        [Column("warranty_months")]
        public int WarrantyMonths { get; set; }

        [Column("warranty_start_date", TypeName = "date")]
        public DateTime? WarrantyStartDate { get; set; }

        [Column("warranty_end_date", TypeName = "date")]
        public DateTime? WarrantyEndDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("warranty_status")]
        public string WarrantyStatus { get; set; }

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