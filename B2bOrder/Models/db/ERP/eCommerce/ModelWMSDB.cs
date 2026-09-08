using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.ERP.eCommerce.Models
{
    #region 01. 倉庫與儲位基礎架構 / Warehouse & Storage Location Infrastructure
    /// <summary>
    /// 倉庫主檔 / Warehouse Master
    /// </summary>
    [Table("wms_warehouse")]
    public class Warehouse
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("warehouse_code")]
        public string WarehouseCode { get; set; } = null!;

        [Required]
        [StringLength(200)]
        [Column("warehouse_name")]
        public string WarehouseName { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [StringLength(30)]
        [Column("warehouse_type")]
        public string WarehouseType { get; set; } = "PHYSICAL";

        [StringLength(32)]
        [Column("address_sid")]
        public string? AddressSid { get; set; }

        [StringLength(32)]
        [Column("manager_user_sid")]
        public string? ManagerUserSid { get; set; }

        [Column("is_temperature_controlled")]
        public bool IsTemperatureControlled { get; set; } = false;

        [Required]
        [StringLength(20)]
        [Column("warehouse_status")]
        public string WarehouseStatus { get; set; } = "ACTIVE";

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }

    /// <summary>
    /// 倉庫儲位檔 / Storage Location
    /// </summary>
    [Table("wms_location")]
    public class Location
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("warehouse_nid")]
        public ulong WarehouseNid { get; set; }

        [Required]
        [StringLength(100)]
        [Column("location_code")]
        public string LocationCode { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Column("zone_code")]
        public string ZoneCode { get; set; } = null!;

        [StringLength(20)]
        [Column("aisle_code")]
        public string? AisleCode { get; set; }

        [StringLength(20)]
        [Column("rack_code")]
        public string? RackCode { get; set; }

        [StringLength(20)]
        [Column("level_code")]
        public string? LevelCode { get; set; }

        [StringLength(20)]
        [Column("position_code")]
        public string? PositionCode { get; set; }

        [Required]
        [StringLength(30)]
        [Column("location_type")]
        public string LocationType { get; set; } = "STORAGE";

        [Column("max_weight", TypeName = "decimal(12,4)")]
        public decimal? MaxWeight { get; set; }

        [Column("max_volume", TypeName = "decimal(12,4)")]
        public decimal? MaxVolume { get; set; }

        [Column("is_locked")]
        public bool IsLocked { get; set; } = false;

        [Required]
        [StringLength(20)]
        [Column("location_status")]
        public string LocationStatus { get; set; } = "ACTIVE";

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 02. 庫存現有帳與批號序列號 / Stock Inventory & Lot/Serial Tracking
    /// <summary>
    /// 儲位庫存帳 / Storage Location Stock
    /// </summary>
    [Table("wms_stock")]
    public class Stock
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("warehouse_nid")]
        public ulong WarehouseNid { get; set; }

        [Column("location_nid")]
        public ulong LocationNid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Required]
        [StringLength(100)]
        [Column("lot_no")]
        public string LotNo { get; set; } = string.Empty;

        [Column("on_hand_qty", TypeName = "decimal(20,6)")]
        public decimal OnHandQty { get; set; } = 0.000000m;

        [Column("allocated_qty", TypeName = "decimal(20,6)")]
        public decimal AllocatedQty { get; set; } = 0.000000m;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("available_qty", TypeName = "decimal(20,6)")]
        public decimal AvailableQty { get; private set; }

        [Column("manufacture_date", TypeName = "date")]
        public DateTime? ManufactureDate { get; set; }

        [Column("expiration_date", TypeName = "date")]
        public DateTime? ExpirationDate { get; set; }

        [Required]
        [StringLength(20)]
        [Column("stock_status")]
        public string StockStatus { get; set; } = "AVAILABLE";

        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;
    }

    /// <summary>
    /// 單件商品序列號追蹤 / Single Item Serial Tracking
    /// </summary>
    [Table("wms_stock_serial")]
    public class StockSerial
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("stock_nid")]
        public ulong StockNid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [Column("serial_no")]
        public string SerialNo { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [Column("serial_status")]
        public string SerialStatus { get; set; } = "IN_STOCK";
    }
    #endregion

    #region 03. 進貨管理 / Inbound Management (Goods Receipt)
    /// <summary>
    /// 進貨入庫單 / Inbound Order
    /// </summary>
    [Table("wms_inbound_order")]
    public class InboundOrder
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("inbound_no")]
        public string InboundNo { get; set; } = null!;

        [StringLength(100)]
        [Column("external_asn_no")]
        public string? ExternalAsnNo { get; set; }

        [StringLength(32)]
        [Column("po_sid")]
        public string? PoSid { get; set; }

        [Required]
        [StringLength(30)]
        [Column("source_type")]
        public string SourceType { get; set; } = "PURCHASE";

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Column("warehouse_nid")]
        public ulong WarehouseNid { get; set; }

        [StringLength(32)]
        [Column("supplier_party_sid")]
        public string? SupplierPartySid { get; set; }

        [Column("expected_arrival_date")]
        public DateTime? ExpectedArrivalDate { get; set; }

        [Column("actual_arrival_date")]
        public DateTime? ActualArrivalDate { get; set; }

        [Required]
        [StringLength(30)]
        [Column("inbound_status")]
        public string InboundStatus { get; set; } = "PENDING";

        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }

    /// <summary>
    /// 進貨入庫明細 / Inbound Order Item
    /// </summary>
    [Table("wms_inbound_item")]
    public class InboundItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("inbound_nid")]
        public ulong InboundNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Column("expected_qty", TypeName = "decimal(20,6)")]
        public decimal ExpectedQty { get; set; }

        [Column("received_qty", TypeName = "decimal(20,6)")]
        public decimal ReceivedQty { get; set; } = 0.000000m;

        [Column("qc_pass_qty", TypeName = "decimal(20,6)")]
        public decimal QcPassQty { get; set; } = 0.000000m;

        [Column("qc_fail_qty", TypeName = "decimal(20,6)")]
        public decimal QcFailQty { get; set; } = 0.000000m;

        [Column("putaway_qty", TypeName = "decimal(20,6)")]
        public decimal PutawayQty { get; set; } = 0.000000m;

        [Required]
        [StringLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [StringLength(100)]
        [Column("lot_no")]
        public string? LotNo { get; set; }

        [Column("manufacture_date", TypeName = "date")]
        public DateTime? ManufactureDate { get; set; }

        [Column("expiration_date", TypeName = "date")]
        public DateTime? ExpirationDate { get; set; }

        [Column("target_location_nid")]
        public ulong? TargetLocationNid { get; set; }

        [Required]
        [StringLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "PENDING";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 04. 出貨與揀貨管理 / Outbound & Picking Management
    /// <summary>
    /// 出貨單檔 / Outbound Order
    /// </summary>
    [Table("wms_outbound_order")]
    public class OutboundOrder
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("outbound_no")]
        public string OutboundNo { get; set; } = null!;

        [StringLength(32)]
        [Column("fulfillment_request_sid")]
        public string? FulfillmentRequestSid { get; set; }

        [StringLength(32)]
        [Column("sales_order_sid")]
        public string? SalesOrderSid { get; set; }

        [Required]
        [StringLength(30)]
        [Column("source_type")]
        public string SourceType { get; set; } = "SALES";

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Column("warehouse_nid")]
        public ulong WarehouseNid { get; set; }

        [StringLength(32)]
        [Column("shipping_address_sid")]
        public string? ShippingAddressSid { get; set; }

        [StringLength(50)]
        [Column("carrier_code")]
        public string? CarrierCode { get; set; }

        [StringLength(100)]
        [Column("tracking_no")]
        public string? TrackingNo { get; set; }

        [Required]
        [StringLength(30)]
        [Column("outbound_status")]
        public string OutboundStatus { get; set; } = "PENDING";

        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }

    /// <summary>
    /// 出貨單明細 / Outbound Order Item
    /// </summary>
    [Table("wms_outbound_item")]
    public class OutboundItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("outbound_nid")]
        public ulong OutboundNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Column("plan_qty", TypeName = "decimal(20,6)")]
        public decimal PlanQty { get; set; }

        [Column("picked_qty", TypeName = "decimal(20,6)")]
        public decimal PickedQty { get; set; } = 0.000000m;

        [Column("shipped_qty", TypeName = "decimal(20,6)")]
        public decimal ShippedQty { get; set; } = 0.000000m;

        [Required]
        [StringLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; } = null!;

        [StringLength(100)]
        [Column("lot_no")]
        public string? LotNo { get; set; }

        [Column("from_location_nid")]
        public ulong? FromLocationNid { get; set; }

        [Required]
        [StringLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "PENDING";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 05. 庫內異動與盤點 / Internal Transfer & Stocktake Management
    /// <summary>
    /// 庫內移庫/跨倉調撥單 / Transfer Order
    /// </summary>
    [Table("wms_transfer_order")]
    public class TransferOrder
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("transfer_no")]
        public string TransferNo { get; set; } = null!;

        [Required]
        [StringLength(30)]
        [Column("transfer_type")]
        public string TransferType { get; set; } = "LOCATION_MOVE";

        [Column("from_warehouse_nid")]
        public ulong FromWarehouseNid { get; set; }

        [Column("to_warehouse_nid")]
        public ulong ToWarehouseNid { get; set; }

        [Required]
        [StringLength(30)]
        [Column("transfer_status")]
        public string TransferStatus { get; set; } = "DRAFT";

        [StringLength(32)]
        [Column("requested_user_sid")]
        public string? RequestedUserSid { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }

    /// <summary>
    /// 庫存盤點單 / Stocktake
    /// </summary>
    [Table("wms_stocktake")]
    public class Stocktake
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(100)]
        [Column("stocktake_no")]
        public string StocktakeNo { get; set; } = null!;

        [Column("warehouse_nid")]
        public ulong WarehouseNid { get; set; }

        [Required]
        [StringLength(30)]
        [Column("stocktake_type")]
        public string StocktakeType { get; set; } = "CYCLE_COUNT";

        [Required]
        [StringLength(30)]
        [Column("stocktake_status")]
        public string StocktakeStatus { get; set; } = "DRAFT";

        [Column("planned_date", TypeName = "date")]
        public DateTime PlannedDate { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 06. 庫存履歷與異動流水帳 / Inventory Ledger & History Tracking
    /// <summary>
    /// 庫存異動流水帳 / Inventory Ledger
    /// </summary>
    [Table("wms_inventory_ledger")]
    public class InventoryLedger
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("warehouse_nid")]
        public ulong WarehouseNid { get; set; }

        [Column("location_nid")]
        public ulong LocationNid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; } = null!;

        [StringLength(32)]
        [Column("variant_sid")]
        public string? VariantSid { get; set; }

        [Required]
        [StringLength(100)]
        [Column("lot_no")]
        public string LotNo { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        [Column("transaction_type")]
        public string TransactionType { get; set; } = null!;

        [Column("qty_change", TypeName = "decimal(20,6)")]
        public decimal QtyChange { get; set; }

        [Column("qty_after", TypeName = "decimal(20,6)")]
        public decimal QtyAfter { get; set; }

        [StringLength(50)]
        [Column("reference_doc_type")]
        public string? ReferenceDocType { get; set; }

        [StringLength(100)]
        [Column("reference_doc_no")]
        public string? ReferenceDocNo { get; set; }

        [StringLength(32)]
        [Column("operator_user_sid")]
        public string? OperatorUserSid { get; set; }

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion
}