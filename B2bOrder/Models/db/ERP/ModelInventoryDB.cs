using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.eCommerce.Models
{
    // =========================================================
    // 01. 庫存政策與 Item 庫存設定 (Inventory Policy and Item Inventory Settings)
    // =========================================================

    [Table("inv_policy")]
    public class InvPolicy
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("policy_code")]
        public string PolicyCode { get; set; } = null!;

        [Required]
        [StringLength(200)]
        [Column("policy_name")]
        public string PolicyName { get; set; } = null!;

        [StringLength(32)]
        [Column("company_sid")]
        public string? CompanySid { get; set; }

        [StringLength(32)]
        [Column("warehouse_sid")]
        public string? WarehouseSid { get; set; }

        [Column("negative_stock_allowed")]
        public sbyte NegativeStockAllowed { get; set; }

        [Column("reservation_required")]
        public sbyte ReservationRequired { get; set; }

        [Required]
        [StringLength(30)]
        [Column("batch_strategy")]
        public string BatchStrategy { get; set; } = "FIFO";

        [Required]
        [StringLength(30)]
        [Column("cost_method")]
        public string CostMethod { get; set; } = "MOVING_AVERAGE";

        [Column("over_issue_rate", TypeName = "decimal(8,4)")]
        public decimal OverIssueRate { get; set; }

        [Column("short_issue_rate", TypeName = "decimal(8,4)")]
        public decimal ShortIssueRate { get; set; }

        [Required]
        [StringLength(20)]
        [Column("policy_status")]
        public string PolicyStatus { get; set; } = "ACTIVE";

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }
    }

    [Table("inv_item_setting")]
    public class InvItemSetting
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [StringLength(32)]
        [Column("company_sid")]
        public string? CompanySid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("inventory_unit_sid")]
        public string InventoryUnitSid { get; set; } = null!;

        [StringLength(32)]
        [Column("issue_unit_sid")]
        public string? IssueUnitSid { get; set; }

        [StringLength(32)]
        [Column("receipt_unit_sid")]
        public string? ReceiptUnitSid { get; set; }

        [Column("batch_control")]
        public sbyte BatchControl { get; set; }

        [Column("serial_control")]
        public sbyte SerialControl { get; set; }

        [Column("expiry_control")]
        public sbyte ExpiryControl { get; set; }

        [Column("quality_hold_required")]
        public sbyte QualityHoldRequired { get; set; }

        [Column("safety_stock_qty", TypeName = "decimal(20,6)")]
        public decimal SafetyStockQty { get; set; }

        [Column("reorder_point_qty", TypeName = "decimal(20,6)")]
        public decimal ReorderPointQty { get; set; }

        [Column("reorder_qty", TypeName = "decimal(20,6)")]
        public decimal ReorderQty { get; set; }

        [Column("maximum_stock_qty", TypeName = "decimal(20,6)")]
        public decimal? MaximumStockQty { get; set; }

        [Column("minimum_issue_qty", TypeName = "decimal(20,6)")]
        public decimal MinimumIssueQty { get; set; }

        [Column("issue_multiple_qty", TypeName = "decimal(20,6)")]
        public decimal IssueMultipleQty { get; set; } = 1;

        [Required]
        [StringLength(20)]
        [Column("setting_status")]
        public string SettingStatus { get; set; } = "ACTIVE";

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }
    }

    // =========================================================
    // 02. 即時庫存帳 (Real-Time Inventory Accounts)
    // =========================================================

    [Table("inv_stock_balance")]
    public class InvStockBalance
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("location_sid")]
        public string LocationSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [StringLength(32)]
        [Column("batch_sid")]
        public string? BatchSid { get; set; }

        [StringLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [StringLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [StringLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [StringLength(32)]
        [Column("owner_party_sid")]
        public string? OwnerPartySid { get; set; }

        [Required]
        [StringLength(20)]
        [Column("stock_status")]
        public string StockStatus { get; set; } = "AVAILABLE";

        [Column("on_hand_qty", TypeName = "decimal(20,6)")]
        public decimal OnHandQty { get; set; }

        [Column("reserved_qty", TypeName = "decimal(20,6)")]
        public decimal ReservedQty { get; set; }

        [Column("allocated_qty", TypeName = "decimal(20,6)")]
        public decimal AllocatedQty { get; set; }

        [Column("available_qty", TypeName = "decimal(20,6)")]
        public decimal AvailableQty { get; set; }

        [Column("in_transit_qty", TypeName = "decimal(20,6)")]
        public decimal InTransitQty { get; set; }

        [Column("quality_hold_qty", TypeName = "decimal(20,6)")]
        public decimal QualityHoldQty { get; set; }

        [Column("blocked_qty", TypeName = "decimal(20,6)")]
        public decimal BlockedQty { get; set; }

        [Column("damaged_qty", TypeName = "decimal(20,6)")]
        public decimal DamagedQty { get; set; }

        [Column("average_cost", TypeName = "decimal(20,6)")]
        public decimal AverageCost { get; set; }

        [Column("total_cost", TypeName = "decimal(20,4)")]
        public decimal TotalCost { get; set; }

        [Column("last_movement_date")]
        public DateTime? LastMovementDate { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; }
    }

    [Table("inv_stock_summary")]
    public class InvStockSummary
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Column("total_on_hand_qty", TypeName = "decimal(20,6)")]
        public decimal TotalOnHandQty { get; set; }

        [Column("total_reserved_qty", TypeName = "decimal(20,6)")]
        public decimal TotalReservedQty { get; set; }

        [Column("total_available_qty", TypeName = "decimal(20,6)")]
        public decimal TotalAvailableQty { get; set; }

        [Column("total_in_transit_qty", TypeName = "decimal(20,6)")]
        public decimal TotalInTransitQty { get; set; }

        [Column("total_quality_hold_qty", TypeName = "decimal(20,6)")]
        public decimal TotalQualityHoldQty { get; set; }

        [Column("total_stock_value", TypeName = "decimal(20,4)")]
        public decimal TotalStockValue { get; set; }

        [Column("last_calculated_date")]
        public DateTime LastCalculatedDate { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; }
    }

    // =========================================================
    // 03. 批號與序號 (Batches and Serials)
    // =========================================================

    [Table("inv_batch")]
    public class InvBatch
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Required]
        [StringLength(150)]
        [Column("batch_no")]
        public string BatchNo { get; set; } = null!;

        [StringLength(32)]
        [Column("supplier_party_sid")]
        public string? SupplierPartySid { get; set; }

        [StringLength(32)]
        [Column("manufacturer_party_sid")]
        public string? ManufacturerPartySid { get; set; }

        [Column("manufacture_date", TypeName = "date")]
        public DateTime? ManufactureDate { get; set; }

        [Column("expiry_date", TypeName = "date")]
        public DateTime? ExpiryDate { get; set; }

        [StringLength(32)]
        [Column("receipt_sid")]
        public string? ReceiptSid { get; set; }

        [StringLength(32)]
        [Column("inspection_sid")]
        public string? InspectionSid { get; set; }

        [Required]
        [StringLength(20)]
        [Column("quality_status")]
        public string QualityStatus { get; set; } = "PENDING";

        [Required]
        [StringLength(20)]
        [Column("batch_status")]
        public string BatchStatus { get; set; } = "ACTIVE";

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }
    }

    [Table("inv_serial")]
    public class InvSerial
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Required]
        [StringLength(300)]
        [Column("serial_no")]
        public string SerialNo { get; set; } = null!;

        [StringLength(32)]
        [Column("batch_sid")]
        public string? BatchSid { get; set; }

        [StringLength(32)]
        [Column("warehouse_sid")]
        public string? WarehouseSid { get; set; }

        [StringLength(32)]
        [Column("location_sid")]
        public string? LocationSid { get; set; }

        [StringLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [StringLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [StringLength(32)]
        [Column("owner_party_sid")]
        public string? OwnerPartySid { get; set; }

        [StringLength(32)]
        [Column("receipt_sid")]
        public string? ReceiptSid { get; set; }

        [Column("warranty_start_date", TypeName = "date")]
        public DateTime? WarrantyStartDate { get; set; }

        [Column("warranty_end_date", TypeName = "date")]
        public DateTime? WarrantyEndDate { get; set; }

        [Required]
        [StringLength(30)]
        [Column("serial_status")]
        public string SerialStatus { get; set; } = "IN_STOCK";

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }
    }

    // =========================================================
    // 04. 庫存異動與成本 (Inventory Transactions and Costs)
    // =========================================================

    [Table("inv_transaction")]
    public class InvTransaction
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("transaction_no")]
        public string TransactionNo { get; set; } = null!;

        [Required]
        [StringLength(30)]
        [Column("transaction_type")]
        public string TransactionType { get; set; } = null!;

        [Column("transaction_date")]
        public DateTime TransactionDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("location_sid")]
        public string LocationSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [StringLength(32)]
        [Column("batch_sid")]
        public string? BatchSid { get; set; }

        [StringLength(32)]
        [Column("serial_sid")]
        public string? SerialSid { get; set; }

        [Column("quantity", TypeName = "decimal(20,6)")]
        public decimal Quantity { get; set; }

        [Required]
        [StringLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [Column("unit_cost", TypeName = "decimal(20,6)")]
        public decimal UnitCost { get; set; }

        [Column("total_cost", TypeName = "decimal(20,4)")]
        public decimal TotalCost { get; set; }

        [StringLength(20)]
        [Column("stock_status_from")]
        public string? StockStatusFrom { get; set; }

        [StringLength(20)]
        [Column("stock_status_to")]
        public string? StockStatusTo { get; set; }

        [StringLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [StringLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [StringLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [StringLength(32)]
        [Column("cost_center_sid")]
        public string? CostCenterSid { get; set; }

        [StringLength(32)]
        [Column("owner_party_sid")]
        public string? OwnerPartySid { get; set; }

        [Required]
        [StringLength(50)]
        [Column("source_type")]
        public string SourceType { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("source_sid")]
        public string SourceSid { get; set; } = null!;

        [StringLength(32)]
        [Column("source_line_sid")]
        public string? SourceLineSid { get; set; }

        [StringLength(32)]
        [Column("operator_user_sid")]
        public string? OperatorUserSid { get; set; }

        [StringLength(100)]
        [Column("correlation_id")]
        public string? CorrelationId { get; set; }
    }

    [Table("inv_cost_layer")]
    public class InvCostLayer
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [StringLength(32)]
        [Column("batch_sid")]
        public string? BatchSid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("source_transaction_sid")]
        public string SourceTransactionSid { get; set; } = null!;

        [Column("original_qty", TypeName = "decimal(20,6)")]
        public decimal OriginalQty { get; set; }

        [Column("remaining_qty", TypeName = "decimal(20,6)")]
        public decimal RemainingQty { get; set; }

        [Column("unit_cost", TypeName = "decimal(20,6)")]
        public decimal UnitCost { get; set; }

        [Required]
        [StringLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("exchange_rate", TypeName = "decimal(20,10)")]
        public decimal ExchangeRate { get; set; } = 1;

        [Column("cost_date")]
        public DateTime CostDate { get; set; }

        [Required]
        [StringLength(20)]
        [Column("layer_status")]
        public string LayerStatus { get; set; } = "OPEN";
    }

    // =========================================================
    // 05. 預留與配貨 (Reservations and Allocations)
    // =========================================================

    [Table("inv_reservation")]
    public class InvReservation
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("reservation_no")]
        public string ReservationNo { get; set; } = null!;

        [Required]
        [StringLength(30)]
        [Column("source_type")]
        public string SourceType { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("source_sid")]
        public string SourceSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [StringLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [StringLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [Column("requested_date")]
        public DateTime RequestedDate { get; set; }

        [Column("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        [Required]
        [StringLength(20)]
        [Column("priority")]
        public string Priority { get; set; } = "NORMAL";

        [Required]
        [StringLength(30)]
        [Column("reservation_status")]
        public string ReservationStatus { get; set; } = "PENDING";

        [Column("version_no")]
        public ulong VersionNo { get; set; }

        public virtual ICollection<InvReservationItem> ReservationItems { get; set; } = new List<InvReservationItem>();
    }

    [Table("inv_reservation_item")]
    public class InvReservationItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("reservation_nid")]
        public ulong ReservationNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [StringLength(32)]
        [Column("source_line_sid")]
        public string? SourceLineSid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Column("requested_qty", TypeName = "decimal(20,6)")]
        public decimal RequestedQty { get; set; }

        [Column("reserved_qty", TypeName = "decimal(20,6)")]
        public decimal ReservedQty { get; set; }

        [Column("allocated_qty", TypeName = "decimal(20,6)")]
        public decimal AllocatedQty { get; set; }

        [Column("released_qty", TypeName = "decimal(20,6)")]
        public decimal ReleasedQty { get; set; }

        [Required]
        [StringLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [StringLength(32)]
        [Column("preferred_warehouse_sid")]
        public string? PreferredWarehouseSid { get; set; }

        [StringLength(32)]
        [Column("preferred_location_sid")]
        public string? PreferredLocationSid { get; set; }

        [StringLength(30)]
        [Column("batch_strategy")]
        public string? BatchStrategy { get; set; }

        [Required]
        [StringLength(20)]
        [Column("reservation_status")]
        public string ReservationStatus { get; set; } = "PENDING";

        [ForeignKey("ReservationNid")]
        public virtual InvReservation Reservation { get; set; } = null!;
    }

    [Table("inv_allocation")]
    public class InvAllocation
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("reservation_item_sid")]
        public string ReservationItemSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("location_sid")]
        public string LocationSid { get; set; } = null!;

        [StringLength(32)]
        [Column("batch_sid")]
        public string? BatchSid { get; set; }

        [StringLength(32)]
        [Column("serial_sid")]
        public string? SerialSid { get; set; }

        [Column("allocated_qty", TypeName = "decimal(20,6)")]
        public decimal AllocatedQty { get; set; }

        [Required]
        [StringLength(20)]
        [Column("allocation_status")]
        public string AllocationStatus { get; set; } = "ALLOCATED";

        [Column("picked_date")]
        public DateTime? PickedDate { get; set; }

        [Column("issued_date")]
        public DateTime? IssuedDate { get; set; }
    }

    // =========================================================
    // 06. 調撥 (Transfers)
    // =========================================================

    [Table("inv_transfer")]
    public class InvTransfer
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("transfer_no")]
        public string TransferNo { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("from_warehouse_sid")]
        public string FromWarehouseSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("to_warehouse_sid")]
        public string ToWarehouseSid { get; set; } = null!;

        [StringLength(32)]
        [Column("from_project_sid")]
        public string? FromProjectSid { get; set; }

        [StringLength(32)]
        [Column("to_project_sid")]
        public string? ToProjectSid { get; set; }

        [StringLength(32)]
        [Column("from_site_sid")]
        public string? FromSiteSid { get; set; }

        [StringLength(32)]
        [Column("to_site_sid")]
        public string? ToSiteSid { get; set; }

        [Column("request_date", TypeName = "date")]
        public DateTime RequestDate { get; set; }

        [Column("planned_ship_date", TypeName = "date")]
        public DateTime? PlannedShipDate { get; set; }

        [Column("planned_receive_date", TypeName = "date")]
        public DateTime? PlannedReceiveDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("requester_user_sid")]
        public string RequesterUserSid { get; set; } = null!;

        [Required]
        [StringLength(30)]
        [Column("transfer_status")]
        public string TransferStatus { get; set; } = "DRAFT";

        [StringLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [StringLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("shipped_date")]
        public DateTime? ShippedDate { get; set; }

        [Column("received_date")]
        public DateTime? ReceivedDate { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        public virtual ICollection<InvTransferItem> TransferItems { get; set; } = new List<InvTransferItem>();
    }

    [Table("inv_transfer_item")]
    public class InvTransferItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("transfer_nid")]
        public ulong TransferNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Column("request_qty", TypeName = "decimal(20,6)")]
        public decimal RequestQty { get; set; }

        [Column("shipped_qty", TypeName = "decimal(20,6)")]
        public decimal ShippedQty { get; set; }

        [Column("received_qty", TypeName = "decimal(20,6)")]
        public decimal ReceivedQty { get; set; }

        [Column("damaged_qty", TypeName = "decimal(20,6)")]
        public decimal DamagedQty { get; set; }

        [Required]
        [StringLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [StringLength(32)]
        [Column("from_location_sid")]
        public string? FromLocationSid { get; set; }

        [StringLength(32)]
        [Column("to_location_sid")]
        public string? ToLocationSid { get; set; }

        [StringLength(32)]
        [Column("batch_sid")]
        public string? BatchSid { get; set; }

        [Required]
        [StringLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "OPEN";

        [ForeignKey("TransferNid")]
        public virtual InvTransfer Transfer { get; set; } = null!;
    }

    // =========================================================
    // 07. 盤點與調整 (Stock Counts and Adjustments)
    // =========================================================

    [Table("inv_stock_count")]
    public class InvStockCount
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("count_no")]
        public string CountNo { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [StringLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [StringLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [Required]
        [StringLength(30)]
        [Column("count_type")]
        public string CountType { get; set; } = null!;

        [Column("freeze_stock")]
        public sbyte FreezeStock { get; set; } = 1;

        [Column("planned_date", TypeName = "date")]
        public DateTime PlannedDate { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("responsible_user_sid")]
        public string ResponsibleUserSid { get; set; } = null!;

        [Required]
        [StringLength(30)]
        [Column("count_status")]
        public string CountStatus { get; set; } = "DRAFT";

        [StringLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [StringLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        public virtual ICollection<InvStockCountItem> StockCountItems { get; set; } = new List<InvStockCountItem>();
    }

    [Table("inv_stock_count_item")]
    public class InvStockCountItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("stock_count_nid")]
        public ulong StockCountNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [StringLength(32)]
        [Column("location_sid")]
        public string LocationSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [StringLength(32)]
        [Column("batch_sid")]
        public string? BatchSid { get; set; }

        [Column("system_qty", TypeName = "decimal(20,6)")]
        public decimal SystemQty { get; set; }

        [Column("counted_qty", TypeName = "decimal(20,6)")]
        public decimal? CountedQty { get; set; }

        [Column("variance_qty", TypeName = "decimal(20,6)")]
        public decimal? VarianceQty { get; set; }

        [Required]
        [StringLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [StringLength(32)]
        [Column("count_user_sid")]
        public string? CountUserSid { get; set; }

        [StringLength(32)]
        [Column("recount_user_sid")]
        public string? RecountUserSid { get; set; }

        [Column("count_date")]
        public DateTime? CountDate { get; set; }

        [StringLength(50)]
        [Column("variance_reason_code")]
        public string? VarianceReasonCode { get; set; }

        [Column("variance_note", TypeName = "TEXT")]
        public string? VarianceNote { get; set; }

        [Required]
        [StringLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "PENDING";

        [ForeignKey("StockCountNid")]
        public virtual InvStockCount StockCount { get; set; } = null!;
    }

    [Table("inv_adjustment")]
    public class InvAdjustment
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("adjustment_no")]
        public string AdjustmentNo { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [Required]
        [StringLength(30)]
        [Column("adjustment_type")]
        public string AdjustmentType { get; set; } = null!;

        [StringLength(30)]
        [Column("source_type")]
        public string? SourceType { get; set; }

        [StringLength(32)]
        [Column("source_sid")]
        public string? SourceSid { get; set; }

        [Column("adjustment_date")]
        public DateTime AdjustmentDate { get; set; }

        [Required]
        [StringLength(50)]
        [Column("reason_code")]
        public string ReasonCode { get; set; } = null!;

        [Column("reason", TypeName = "TEXT")]
        public string? Reason { get; set; }

        [Required]
        [StringLength(20)]
        [Column("adjustment_status")]
        public string AdjustmentStatus { get; set; } = "DRAFT";

        [StringLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [StringLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("posted_date")]
        public DateTime? PostedDate { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        public virtual ICollection<InvAdjustmentItem> AdjustmentItems { get; set; } = new List<InvAdjustmentItem>();
    }

    [Table("inv_adjustment_item")]
    public class InvAdjustmentItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("adjustment_nid")]
        public ulong AdjustmentNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [StringLength(32)]
        [Column("location_sid")]
        public string LocationSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [StringLength(32)]
        [Column("batch_sid")]
        public string? BatchSid { get; set; }

        [StringLength(32)]
        [Column("serial_sid")]
        public string? SerialSid { get; set; }

        [Column("before_qty", TypeName = "decimal(20,6)")]
        public decimal BeforeQty { get; set; }

        [Column("adjustment_qty", TypeName = "decimal(20,6)")]
        public decimal AdjustmentQty { get; set; }

        [Column("after_qty", TypeName = "decimal(20,6)")]
        public decimal AfterQty { get; set; }

        [Required]
        [StringLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [Column("before_unit_cost", TypeName = "decimal(20,6)")]
        public decimal BeforeUnitCost { get; set; }

        [Column("after_unit_cost", TypeName = "decimal(20,6)")]
        public decimal AfterUnitCost { get; set; }

        [StringLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [StringLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [StringLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        [ForeignKey("AdjustmentNid")]
        public virtual InvAdjustment Adjustment { get; set; } = null!;
    }

    // =========================================================
    // 08. 領料、退料與耗用 (Material Issues, Returns, and Consumptions)
    // =========================================================

    [Table("inv_issue_request")]
    public class InvIssueRequest
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("issue_no")]
        public string IssueNo { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [StringLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [StringLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [StringLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [StringLength(32)]
        [Column("cost_center_sid")]
        public string? CostCenterSid { get; set; }

        [StringLength(32)]
        [Column("contractor_party_sid")]
        public string? ContractorPartySid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("requester_user_sid")]
        public string RequesterUserSid { get; set; } = null!;

        [Column("request_date", TypeName = "date")]
        public DateTime RequestDate { get; set; }

        [Column("required_date", TypeName = "date")]
        public DateTime? RequiredDate { get; set; }

        [Required]
        [StringLength(30)]
        [Column("issue_purpose")]
        public string IssuePurpose { get; set; } = null!;

        [StringLength(32)]
        [Column("reservation_sid")]
        public string? ReservationSid { get; set; }

        [Required]
        [StringLength(30)]
        [Column("issue_status")]
        public string IssueStatus { get; set; } = "DRAFT";

        [StringLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [StringLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("issued_date")]
        public DateTime? IssuedDate { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        public virtual ICollection<InvIssueRequestItem> IssueRequestItems { get; set; } = new List<InvIssueRequestItem>();
    }

    [Table("inv_issue_request_item")]
    public class InvIssueRequestItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("issue_request_nid")]
        public ulong IssueRequestNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Column("request_qty", TypeName = "decimal(20,6)")]
        public decimal RequestQty { get; set; }

        [Column("approved_qty", TypeName = "decimal(20,6)")]
        public decimal ApprovedQty { get; set; }

        [Column("issued_qty", TypeName = "decimal(20,6)")]
        public decimal IssuedQty { get; set; }

        [Column("returned_qty", TypeName = "decimal(20,6)")]
        public decimal ReturnedQty { get; set; }

        [Column("consumed_qty", TypeName = "decimal(20,6)")]
        public decimal ConsumedQty { get; set; }

        [Required]
        [StringLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [StringLength(32)]
        [Column("preferred_location_sid")]
        public string? PreferredLocationSid { get; set; }

        [StringLength(32)]
        [Column("budget_item_sid")]
        public string? BudgetItemSid { get; set; }

        [StringLength(32)]
        [Column("contract_item_sid")]
        public string? ContractItemSid { get; set; }

        [StringLength(32)]
        [Column("bom_component_sid")]
        public string? BomComponentSid { get; set; }

        [Required]
        [StringLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "OPEN";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        [ForeignKey("IssueRequestNid")]
        public virtual InvIssueRequest IssueRequest { get; set; } = null!;
    }

    [Table("inv_material_return")]
    public class InvMaterialReturn
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("return_no")]
        public string ReturnNo { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("issue_request_sid")]
        public string IssueRequestSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [StringLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [StringLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [StringLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [StringLength(32)]
        [Column("contractor_party_sid")]
        public string? ContractorPartySid { get; set; }

        [Column("return_date")]
        public DateTime ReturnDate { get; set; }

        [Required]
        [StringLength(50)]
        [Column("return_reason_code")]
        public string ReturnReasonCode { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [Column("return_status")]
        public string ReturnStatus { get; set; } = "DRAFT";

        [StringLength(32)]
        [Column("receiver_user_sid")]
        public string? ReceiverUserSid { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        public virtual ICollection<InvMaterialReturnItem> MaterialReturnItems { get; set; } = new List<InvMaterialReturnItem>();
    }

    [Table("inv_material_return_item")]
    public class InvMaterialReturnItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("material_return_nid")]
        public ulong MaterialReturnNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [StringLength(32)]
        [Column("issue_request_item_sid")]
        public string IssueRequestItemSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Column("return_qty", TypeName = "decimal(20,6)")]
        public decimal ReturnQty { get; set; }

        [Column("accepted_qty", TypeName = "decimal(20,6)")]
        public decimal AcceptedQty { get; set; }

        [Column("damaged_qty", TypeName = "decimal(20,6)")]
        public decimal DamagedQty { get; set; }

        [Column("scrap_qty", TypeName = "decimal(20,6)")]
        public decimal ScrapQty { get; set; }

        [Required]
        [StringLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [StringLength(32)]
        [Column("batch_sid")]
        public string? BatchSid { get; set; }

        [StringLength(32)]
        [Column("target_location_sid")]
        public string? TargetLocationSid { get; set; }

        [Required]
        [StringLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "PENDING";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        [ForeignKey("MaterialReturnNid")]
        public virtual InvMaterialReturn MaterialReturn { get; set; } = null!;
    }

    [Table("inv_consumption")]
    public class InvConsumption
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("consumption_no")]
        public string ConsumptionNo { get; set; } = null!;

        [StringLength(32)]
        [Column("issue_request_sid")]
        public string? IssueRequestSid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [StringLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [StringLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [StringLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [StringLength(32)]
        [Column("contractor_party_sid")]
        public string? ContractorPartySid { get; set; }

        [Column("consumption_date")]
        public DateTime ConsumptionDate { get; set; }

        [Required]
        [StringLength(30)]
        [Column("source_type")]
        public string SourceType { get; set; } = null!;

        [StringLength(32)]
        [Column("source_sid")]
        public string? SourceSid { get; set; }

        [Required]
        [StringLength(20)]
        [Column("consumption_status")]
        public string ConsumptionStatus { get; set; } = "DRAFT";

        [StringLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [StringLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("posted_date")]
        public DateTime? PostedDate { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        public virtual ICollection<InvConsumptionItem> ConsumptionItems { get; set; } = new List<InvConsumptionItem>();
    }

    [Table("inv_consumption_item")]
    public class InvConsumptionItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("consumption_nid")]
        public ulong ConsumptionNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [StringLength(32)]
        [Column("issue_request_item_sid")]
        public string? IssueRequestItemSid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [StringLength(32)]
        [Column("batch_sid")]
        public string? BatchSid { get; set; }

        [Column("consumed_qty", TypeName = "decimal(20,6)")]
        public decimal ConsumedQty { get; set; }

        [Column("waste_qty", TypeName = "decimal(20,6)")]
        public decimal WasteQty { get; set; }

        [Required]
        [StringLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [Column("unit_cost", TypeName = "decimal(20,6)")]
        public decimal UnitCost { get; set; }

        [Column("total_cost", TypeName = "decimal(20,4)")]
        public decimal TotalCost { get; set; }

        [StringLength(32)]
        [Column("budget_item_sid")]
        public string? BudgetItemSid { get; set; }

        [StringLength(32)]
        [Column("contract_item_sid")]
        public string? ContractItemSid { get; set; }

        [StringLength(32)]
        [Column("bom_component_sid")]
        public string? BomComponentSid { get; set; }

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        [ForeignKey("ConsumptionNid")]
        public virtual InvConsumption Consumption { get; set; } = null!;
    }

    // =========================================================
    // 09. 報廢與召回 (Scraps and Recalls)
    // =========================================================

    [Table("inv_scrap")]
    public class InvScrap
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("scrap_no")]
        public string ScrapNo { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [StringLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [StringLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [Column("scrap_date")]
        public DateTime ScrapDate { get; set; }

        [Required]
        [StringLength(50)]
        [Column("reason_code")]
        public string ReasonCode { get; set; } = null!;

        [Required]
        [StringLength(30)]
        [Column("disposal_method")]
        public string DisposalMethod { get; set; } = null!;

        [Column("estimated_recovery_amount", TypeName = "decimal(20,4)")]
        public decimal EstimatedRecoveryAmount { get; set; }

        [Required]
        [StringLength(20)]
        [Column("scrap_status")]
        public string ScrapStatus { get; set; } = "DRAFT";

        [StringLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [StringLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("disposed_date")]
        public DateTime? DisposedDate { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        public virtual ICollection<InvScrapItem> ScrapItems { get; set; } = new List<InvScrapItem>();
    }

    [Table("inv_scrap_item")]
    public class InvScrapItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("scrap_nid")]
        public ulong ScrapNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [StringLength(32)]
        [Column("location_sid")]
        public string LocationSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [StringLength(32)]
        [Column("batch_sid")]
        public string? BatchSid { get; set; }

        [StringLength(32)]
        [Column("serial_sid")]
        public string? SerialSid { get; set; }

        [Column("scrap_qty", TypeName = "decimal(20,6)")]
        public decimal ScrapQty { get; set; }

        [Required]
        [StringLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [Column("unit_cost", TypeName = "decimal(20,6)")]
        public decimal UnitCost { get; set; }

        [Column("total_cost", TypeName = "decimal(20,4)")]
        public decimal TotalCost { get; set; }

        [Column("recovery_amount", TypeName = "decimal(20,4)")]
        public decimal RecoveryAmount { get; set; }

        [StringLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [StringLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }

        [ForeignKey("ScrapNid")]
        public virtual InvScrap Scrap { get; set; } = null!;
    }

    [Table("inv_recall")]
    public class InvRecall
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("recall_no")]
        public string RecallNo { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [StringLength(32)]
        [Column("batch_sid")]
        public string? BatchSid { get; set; }

        [StringLength(32)]
        [Column("supplier_party_sid")]
        public string? SupplierPartySid { get; set; }

        [StringLength(32)]
        [Column("manufacturer_party_sid")]
        public string? ManufacturerPartySid { get; set; }

        [Required]
        [Column("recall_reason", TypeName = "TEXT")]
        public string RecallReason { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [Column("risk_level")]
        public string RiskLevel { get; set; } = "HIGH";

        [Column("announced_date")]
        public DateTime AnnouncedDate { get; set; }

        [Column("effective_date")]
        public DateTime EffectiveDate { get; set; }

        [Required]
        [StringLength(20)]
        [Column("recall_status")]
        public string RecallStatus { get; set; } = "ACTIVE";

        [StringLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; }
    }

    // =========================================================
    // 10. 庫存事件與歷程 (Inventory Events and History)
    // =========================================================

    [Table("inv_status_history")]
    public class InvStatusHistory
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(30)]
        [Column("entity_type")]
        public string EntityType { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("entity_sid")]
        public string EntitySid { get; set; } = null!;

        [StringLength(30)]
        [Column("old_status")]
        public string? OldStatus { get; set; }

        [Required]
        [StringLength(30)]
        [Column("new_status")]
        public string NewStatus { get; set; } = null!;

        [StringLength(100)]
        [Column("event_code")]
        public string? EventCode { get; set; }

        [StringLength(32)]
        [Column("operator_user_sid")]
        public string? OperatorUserSid { get; set; }

        [Column("reason", TypeName = "TEXT")]
        public string? Reason { get; set; }

        [StringLength(100)]
        [Column("correlation_id")]
        public string? CorrelationId { get; set; }
    }

    [Table("inv_event")]
    public class InvEvent
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(30)]
        [Column("entity_type")]
        public string EntityType { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("entity_sid")]
        public string EntitySid { get; set; } = null!;

        [Required]
        [StringLength(120)]
        [Column("event_code")]
        public string EventCode { get; set; } = null!;

        [Column("event_version")]
        public int EventVersion { get; set; } = 1;

        [Column("event_data", TypeName = "JSON")]
        public string? EventData { get; set; }

        [StringLength(100)]
        [Column("source_event_id")]
        public string? SourceEventId { get; set; }

        [StringLength(100)]
        [Column("correlation_id")]
        public string? CorrelationId { get; set; }

        [StringLength(100)]
        [Column("causation_id")]
        public string? CausationId { get; set; }

        [StringLength(32)]
        [Column("outbox_event_sid")]
        public string? OutboxEventSid { get; set; }

        [Required]
        [StringLength(20)]
        [Column("process_status")]
        public string ProcessStatus { get; set; } = "PENDING";

        [Column("processed_date")]
        public DateTime? ProcessedDate { get; set; }

        [Column("error_message", TypeName = "TEXT")]
        public string? ErrorMessage { get; set; }
    }
}