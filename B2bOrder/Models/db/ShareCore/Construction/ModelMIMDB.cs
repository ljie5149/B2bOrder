using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.ShareCore.Construction.Models
{
    #region Material Master Module - 工程材料品類與主檔模組

    [Table("mim_material_master")]
    public class MimMaterialMaster
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
        [Column("material_code")]
        public string MaterialCode { get; set; }

        [MaxLength(50)]
        [Column("pcces_code")]
        public string PccesCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("material_name")]
        public string MaterialName { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("category")]
        public string Category { get; set; }

        [MaxLength(50)]
        [Column("sub_category")]
        public string SubCategory { get; set; }

        [MaxLength(200)]
        [Column("specification")]
        public string Specification { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("base_unit")]
        public string BaseUnit { get; set; }

        [Column("standard_unit_price")]
        public decimal StandardUnitPrice { get; set; }

        [Column("standard_wastage_rate")]
        public decimal StandardWastageRate { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("is_hazmat")]
        public string IsHazmat { get; set; }

        [MaxLength(32)]
        [Column("msds_document_sid")]
        public string MsdsDocumentSid { get; set; }

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

    #region Inventory Stock Module - 工地與倉庫庫存主檔模組

    [Table("mim_inventory_stock")]
    public class MimInventoryStock
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

        [MaxLength(32)]
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("warehouse_code")]
        public string WarehouseCode { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("material_sid")]
        public string MaterialSid { get; set; }

        [MaxLength(50)]
        [Column("location_bin")]
        public string LocationBin { get; set; }

        [Column("qty_on_hand")]
        public decimal QtyOnHand { get; set; }

        [Column("qty_allocated")]
        public decimal QtyAllocated { get; set; }

        [Column("qty_available")]
        public decimal QtyAvailable { get; set; }

        [Column("min_safety_qty")]
        public decimal MinSafetyQty { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }
    }

    #endregion

    #region Inventory Transaction Module - 材料異動與領退料紀錄模組

    [Table("mim_inventory_transaction")]
    public class MimInventoryTransaction
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
        [Column("material_sid")]
        public string MaterialSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("warehouse_code")]
        public string WarehouseCode { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("txn_type")]
        public string TxnType { get; set; }

        [MaxLength(32)]
        [Column("ref_po_sid")]
        public string RefPoSid { get; set; }

        [MaxLength(32)]
        [Column("ref_subcontractor_sid")]
        public string RefSubcontractorSid { get; set; }

        [MaxLength(100)]
        [Column("target_work_zone")]
        public string TargetWorkZone { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("operator_user_sid")]
        public string OperatorUserSid { get; set; }

        [MaxLength(100)]
        [Column("signed_by_receiver")]
        public string SignedByReceiver { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Stock Taking Module - 工地盤點與損耗調整單模組

    [Table("mim_stock_taking")]
    public class MimStockTaking
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
        [Column("warehouse_code")]
        public string WarehouseCode { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("audit_no")]
        public string AuditNo { get; set; }

        [Column("audit_date", TypeName = "date")]
        public DateTime AuditDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("audit_status")]
        public string AuditStatus { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("audited_by_user_sid")]
        public string AuditedByUserSid { get; set; }

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