using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.eCommerce.Models
{
    #region 01. 銷售通路 (Sales Channel)
    [Table("oms_channel")]
    public class OmsChannel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(80)]
        [Column("channel_code")]
        public string ChannelCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [Column("channel_name")]
        public string ChannelName { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        [Column("channel_type")]
        public string ChannelType { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("platform_name")]
        public string? PlatformName { get; set; }

        [MaxLength(32)]
        [Column("store_sid")]
        public string? StoreSid { get; set; }

        [MaxLength(32)]
        [Column("default_warehouse_sid")]
        public string? DefaultWarehouseSid { get; set; }

        [MaxLength(32)]
        [Column("default_currency_sid")]
        public string? DefaultCurrencySid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("timezone_code")]
        public string TimezoneCode { get; set; } = "Asia/Taipei";

        [MaxLength(20)]
        [Column("order_prefix")]
        public string? OrderPrefix { get; set; }

        [Column("auto_accept_order")]
        public bool AutoAcceptOrder { get; set; } = true;

        [Column("auto_allocate_stock")]
        public bool AutoAllocateStock { get; set; } = true;

        [Column("allow_partial_fulfillment")]
        public bool AllowPartialFulfillment { get; set; } = true;

        [Required]
        [MaxLength(20)]
        [Column("channel_status")]
        public string ChannelStatus { get; set; } = "ACTIVE";

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }

        public virtual ICollection<OmsChannelAccount> ChannelAccounts { get; set; } = new HashSet<OmsChannelAccount>();
    }

    [Table("oms_channel_account")]
    public class OmsChannelAccount
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("channel_nid")]
        public ulong ChannelNid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("account_code")]
        public string AccountCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column("account_name")]
        public string AccountName { get; set; } = string.Empty;

        [MaxLength(200)]
        [Column("external_shop_id")]
        public string? ExternalShopId { get; set; }

        [MaxLength(32)]
        [Column("store_sid")]
        public string? StoreSid { get; set; }

        [Column("credential_encrypted")]
        public string? CredentialEncrypted { get; set; }

        [Column("config_json", TypeName = "json")]
        public string? ConfigJson { get; set; }

        [Column("last_sync_date")]
        public DateTime? LastSyncDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("account_status")]
        public string AccountStatus { get; set; } = "ACTIVE";

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }

        [ForeignKey("ChannelNid")]
        public virtual OmsChannel Channel { get; set; } = null!;
        public virtual ICollection<OmsOrderImport> OrderImports { get; set; } = new HashSet<OmsOrderImport>();
    }
    #endregion

    #region 02. 外部訂單接收 (External Order Import)
    [Table("oms_order_import")]
    public class OmsOrderImport
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("import_no")]
        public string ImportNo { get; set; } = string.Empty;

        [Column("channel_account_nid")]
        public ulong ChannelAccountNid { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("external_order_no")]
        public string ExternalOrderNo { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("external_order_version")]
        public string? ExternalOrderVersion { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("idempotency_key")]
        public string IdempotencyKey { get; set; } = string.Empty;

        [Required]
        [Column("raw_payload", TypeName = "json")]
        public string RawPayload { get; set; } = string.Empty;

        [Column("normalized_payload", TypeName = "json")]
        public string? NormalizedPayload { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("import_status")]
        public string ImportStatus { get; set; } = "RECEIVED";

        [Column("validation_result", TypeName = "json")]
        public string? ValidationResult { get; set; }

        [MaxLength(32)]
        [Column("oms_order_sid")]
        public string? OmsOrderSid { get; set; }

        [Column("retry_count")]
        public int RetryCount { get; set; } = 0;

        [MaxLength(100)]
        [Column("error_code")]
        public string? ErrorCode { get; set; }

        [Column("error_message")]
        public string? ErrorMessage { get; set; }

        [Column("imported_date")]
        public DateTime? ImportedDate { get; set; }

        [ForeignKey("ChannelAccountNid")]
        public virtual OmsChannelAccount ChannelAccount { get; set; } = null!;
    }
    #endregion

    #region 03. OMS訂單主檔與明細 (OMS Order Master & Details)
    [Table("oms_order")]
    public class OmsOrder
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(80)]
        [Column("oms_order_no")]
        public string OmsOrderNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        [Column("channel_sid")]
        public string ChannelSid { get; set; } = string.Empty;

        [MaxLength(32)]
        [Column("channel_account_sid")]
        public string? ChannelAccountSid { get; set; }

        [MaxLength(150)]
        [Column("external_order_no")]
        public string? ExternalOrderNo { get; set; }

        [MaxLength(32)]
        [Column("shopping_order_sid")]
        public string? ShoppingOrderSid { get; set; }

        [MaxLength(32)]
        [Column("member_sid")]
        public string? MemberSid { get; set; }

        [MaxLength(100)]
        [Column("customer_no_snapshot")]
        public string? CustomerNoSnapshot { get; set; }

        [MaxLength(200)]
        [Column("customer_name_snapshot")]
        public string? CustomerNameSnapshot { get; set; }

        [MaxLength(200)]
        [Column("customer_email_snapshot")]
        public string? CustomerEmailSnapshot { get; set; }

        [MaxLength(50)]
        [Column("customer_mobile_snapshot")]
        public string? CustomerMobileSnapshot { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = string.Empty;

        [Column("exchange_rate", TypeName = "decimal(20,10)")]
        public decimal ExchangeRate { get; set; } = 1;

        [Column("subtotal_amount", TypeName = "decimal(20,4)")]
        public decimal SubtotalAmount { get; set; } = 0;

        [Column("discount_amount", TypeName = "decimal(20,4)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column("freight_amount", TypeName = "decimal(20,4)")]
        public decimal FreightAmount { get; set; } = 0;

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; } = 0;

        [Column("total_amount", TypeName = "decimal(20,4)")]
        public decimal TotalAmount { get; set; } = 0;

        [Column("paid_amount", TypeName = "decimal(20,4)")]
        public decimal PaidAmount { get; set; } = 0;

        [Column("refunded_amount", TypeName = "decimal(20,4)")]
        public decimal RefundedAmount { get; set; } = 0;

        [Required]
        [MaxLength(30)]
        [Column("order_status")]
        public string OrderStatus { get; set; } = "NEW";

        [Required]
        [MaxLength(30)]
        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "UNPAID";

        [Required]
        [MaxLength(30)]
        [Column("allocation_status")]
        public string AllocationStatus { get; set; } = "UNALLOCATED";

        [Required]
        [MaxLength(30)]
        [Column("fulfillment_status")]
        public string FulfillmentStatus { get; set; } = "UNFULFILLED";

        [Required]
        [MaxLength(20)]
        [Column("risk_status")]
        public string RiskStatus { get; set; } = "PENDING";

        [Required]
        [MaxLength(20)]
        [Column("priority")]
        public string Priority { get; set; } = "NORMAL";

        [Column("promised_ship_date")]
        public DateTime? PromisedShipDate { get; set; }

        [Column("promised_delivery_date")]
        public DateTime? PromisedDeliveryDate { get; set; }

        [Column("accepted_date")]
        public DateTime? AcceptedDate { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [Column("cancelled_date")]
        public DateTime? CancelledDate { get; set; }

        [MaxLength(50)]
        [Column("cancel_reason_code")]
        public string? CancelReasonCode { get; set; }

        [MaxLength(100)]
        [Column("correlation_id")]
        public string? CorrelationId { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }

        public virtual ICollection<OmsOrderItem> OrderItems { get; set; } = new HashSet<OmsOrderItem>();
        public virtual ICollection<OmsOrderAddress> OrderAddresses { get; set; } = new HashSet<OmsOrderAddress>();
        public virtual ICollection<OmsFulfillmentOrder> FulfillmentOrders { get; set; } = new HashSet<OmsFulfillmentOrder>();
        public virtual ICollection<OmsOrderStatusHistory> StatusHistories { get; set; } = new HashSet<OmsOrderStatusHistory>();
        public virtual ICollection<OmsCancelRequest> CancelRequests { get; set; } = new HashSet<OmsCancelRequest>();
    }

    [Table("oms_order_item")]
    public class OmsOrderItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("oms_order_nid")]
        public ulong OmsOrderNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [MaxLength(100)]
        [Column("external_line_no")]
        public string? ExternalLineNo { get; set; }

        [MaxLength(32)]
        [Column("shopping_order_item_sid")]
        public string? ShoppingOrderItemSid { get; set; }

        [MaxLength(32)]
        [Column("store_sid")]
        public string? StoreSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("product_sid")]
        public string ProductSid { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        [Column("sku_sid")]
        public string SkuSid { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("product_no_snapshot")]
        public string ProductNoSnapshot { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        [Column("product_name_snapshot")]
        public string ProductNameSnapshot { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("sku_no_snapshot")]
        public string SkuNoSnapshot { get; set; } = string.Empty;

        [Column("spec_snapshot", TypeName = "json")]
        public string? SpecSnapshot { get; set; }

        [Column("quantity", TypeName = "decimal(20,6)")]
        public decimal Quantity { get; set; }

        [Column("cancelled_qty", TypeName = "decimal(20,6)")]
        public decimal CancelledQty { get; set; } = 0;

        [Column("allocated_qty", TypeName = "decimal(20,6)")]
        public decimal AllocatedQty { get; set; } = 0;

        [Column("fulfilled_qty", TypeName = "decimal(20,6)")]
        public decimal FulfilledQty { get; set; } = 0;

        [Column("returned_qty", TypeName = "decimal(20,6)")]
        public decimal ReturnedQty { get; set; } = 0;

        [Column("unit_price", TypeName = "decimal(20,4)")]
        public decimal UnitPrice { get; set; }

        [Column("original_price", TypeName = "decimal(20,4)")]
        public decimal OriginalPrice { get; set; }

        [Column("discount_amount", TypeName = "decimal(20,4)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; } = 0;

        [Column("line_amount", TypeName = "decimal(20,4)")]
        public decimal LineAmount { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "ACTIVE";

        [Column("substitution_allowed")]
        public bool SubstitutionAllowed { get; set; } = false;

        [Column("gift_mark")]
        public bool GiftMark { get; set; } = false;

        [Column("remark")]
        public string? Remark { get; set; }

        [ForeignKey("OmsOrderNid")]
        public virtual OmsOrder Order { get; set; } = null!;
        public virtual ICollection<OmsFulfillmentOrderItem> FulfillmentOrderItems { get; set; } = new HashSet<OmsFulfillmentOrderItem>();
        public virtual ICollection<OmsCancelItem> CancelItems { get; set; } = new HashSet<OmsCancelItem>();
    }

    [Table("oms_order_address")]
    public class OmsOrderAddress
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("oms_order_nid")]
        public ulong OmsOrderNid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("address_type")]
        public string AddressType { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column("receiver_name")]
        public string ReceiverName { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("receiver_mobile")]
        public string? ReceiverMobile { get; set; }

        [MaxLength(50)]
        [Column("receiver_phone")]
        public string? ReceiverPhone { get; set; }

        [MaxLength(20)]
        [Column("country_code")]
        public string? CountryCode { get; set; }

        [MaxLength(100)]
        [Column("country_name")]
        public string? CountryName { get; set; }

        [MaxLength(100)]
        [Column("city_name")]
        public string? CityName { get; set; }

        [MaxLength(100)]
        [Column("district_name")]
        public string? DistrictName { get; set; }

        [MaxLength(20)]
        [Column("postal_code")]
        public string? PostalCode { get; set; }

        [Required]
        [MaxLength(1000)]
        [Column("address_line1")]
        public string AddressLine1 { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Column("address_line2")]
        public string? AddressLine2 { get; set; }

        [Column("latitude", TypeName = "decimal(12,8)")]
        public decimal? Latitude { get; set; }

        [Column("longitude", TypeName = "decimal(12,8)")]
        public decimal? Longitude { get; set; }

        [ForeignKey("OmsOrderNid")]
        public virtual OmsOrder Order { get; set; } = null!;
    }
    #endregion

    #region 04. 拆單與履約訂單 (Fulfillment Orders)
    [Table("oms_fulfillment_order")]
    public class OmsFulfillmentOrder
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("fulfillment_order_no")]
        public string FulfillmentOrderNo { get; set; } = string.Empty;

        [Column("oms_order_nid")]
        public ulong OmsOrderNid { get; set; }

        [Column("split_group_no")]
        public int SplitGroupNo { get; set; }

        [MaxLength(32)]
        [Column("store_sid")]
        public string? StoreSid { get; set; }

        [MaxLength(32)]
        [Column("warehouse_sid")]
        public string? WarehouseSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("fulfillment_type")]
        public string FulfillmentType { get; set; } = "SHIPMENT";

        [Required]
        [MaxLength(30)]
        [Column("fulfillment_status")]
        public string FulfillmentStatus { get; set; } = "PENDING";

        [MaxLength(32)]
        [Column("inventory_reservation_sid")]
        public string? InventoryReservationSid { get; set; }

        [MaxLength(32)]
        [Column("store_order_sid")]
        public string? StoreOrderSid { get; set; }

        [MaxLength(32)]
        [Column("fulfillment_sid")]
        public string? FulfillmentSid { get; set; }

        [Column("promised_ship_date")]
        public DateTime? PromisedShipDate { get; set; }

        [Column("promised_delivery_date")]
        public DateTime? PromisedDeliveryDate { get; set; }

        [Column("routed_date")]
        public DateTime? RoutedDate { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }

        [ForeignKey("OmsOrderNid")]
        public virtual OmsOrder Order { get; set; } = null!;
        public virtual ICollection<OmsFulfillmentOrderItem> FulfillmentOrderItems { get; set; } = new HashSet<OmsFulfillmentOrderItem>();
    }

    [Table("oms_fulfillment_order_item")]
    public class OmsFulfillmentOrderItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("fulfillment_order_nid")]
        public ulong FulfillmentOrderNid { get; set; }

        [Column("oms_order_item_nid")]
        public ulong OmsOrderItemNid { get; set; }

        [Column("quantity", TypeName = "decimal(20,6)")]
        public decimal Quantity { get; set; }

        [Column("allocated_qty", TypeName = "decimal(20,6)")]
        public decimal AllocatedQty { get; set; } = 0;

        [Column("fulfilled_qty", TypeName = "decimal(20,6)")]
        public decimal FulfilledQty { get; set; } = 0;

        [Column("cancelled_qty", TypeName = "decimal(20,6)")]
        public decimal CancelledQty { get; set; } = 0;

        [MaxLength(32)]
        [Column("reservation_item_sid")]
        public string? ReservationItemSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "PENDING";

        [Column("remark")]
        public string? Remark { get; set; }

        [ForeignKey("FulfillmentOrderNid")]
        public virtual OmsFulfillmentOrder FulfillmentOrder { get; set; } = null!;

        [ForeignKey("OmsOrderItemNid")]
        public virtual OmsOrderItem OrderItem { get; set; } = null!;
    }
    #endregion

    #region 05. 訂單路由規則 (Routing Rules & Logs)
    [Table("oms_routing_rule")]
    public class OmsRoutingRule
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("rule_code")]
        public string RuleCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column("rule_name")]
        public string RuleName { get; set; } = string.Empty;

        [MaxLength(32)]
        [Column("channel_sid")]
        public string? ChannelSid { get; set; }

        [MaxLength(32)]
        [Column("store_sid")]
        public string? StoreSid { get; set; }

        [MaxLength(32)]
        [Column("product_sid")]
        public string? ProductSid { get; set; }

        [MaxLength(32)]
        [Column("sku_sid")]
        public string? SkuSid { get; set; }

        [MaxLength(20)]
        [Column("destination_country_code")]
        public string? DestinationCountryCode { get; set; }

        [MaxLength(50)]
        [Column("destination_region_code")]
        public string? DestinationRegionCode { get; set; }

        [Column("rule_condition", TypeName = "json")]
        public string? RuleCondition { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("route_type")]
        public string RouteType { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        [Column("target_sid")]
        public string TargetSid { get; set; } = string.Empty;

        [Column("priority")]
        public int Priority { get; set; } = 0;

        [Required]
        [MaxLength(30)]
        [Column("allocation_strategy")]
        public string AllocationStrategy { get; set; } = "AVAILABLE_STOCK";

        [Column("allow_split")]
        public bool AllowSplit { get; set; } = true;

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("rule_status")]
        public string RuleStatus { get; set; } = "ACTIVE";

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }
    }

    [Table("oms_routing_log")]
    public class OmsRoutingLog
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(32)]
        [Column("oms_order_sid")]
        public string OmsOrderSid { get; set; } = string.Empty;

        [MaxLength(32)]
        [Column("oms_order_item_sid")]
        public string? OmsOrderItemSid { get; set; }

        [MaxLength(32)]
        [Column("routing_rule_sid")]
        public string? RoutingRuleSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("selected_route_type")]
        public string SelectedRouteType { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        [Column("selected_target_sid")]
        public string SelectedTargetSid { get; set; } = string.Empty;

        [Column("candidate_json", TypeName = "json")]
        public string? CandidateJson { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("routing_result")]
        public string RoutingResult { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Column("reason")]
        public string? Reason { get; set; }

        [MaxLength(100)]
        [Column("correlation_id")]
        public string? CorrelationId { get; set; }
    }
    #endregion

    #region 06. 狀態歷程與狀態同步 (Status History & Sync)
    [Table("oms_order_status_history")]
    public class OmsOrderStatusHistory
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("oms_order_nid")]
        public ulong OmsOrderNid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("status_type")]
        public string StatusType { get; set; } = string.Empty;

        [MaxLength(30)]
        [Column("old_status")]
        public string? OldStatus { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("new_status")]
        public string NewStatus { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("event_code")]
        public string? EventCode { get; set; }

        [MaxLength(80)]
        [Column("source_service_code")]
        public string? SourceServiceCode { get; set; }

        [MaxLength(32)]
        [Column("source_sid")]
        public string? SourceSid { get; set; }

        [MaxLength(32)]
        [Column("operator_sid")]
        public string? OperatorSid { get; set; }

        [MaxLength(50)]
        [Column("reason_code")]
        public string? ReasonCode { get; set; }

        [Column("reason")]
        public string? Reason { get; set; }

        [MaxLength(100)]
        [Column("correlation_id")]
        public string? CorrelationId { get; set; }

        [ForeignKey("OmsOrderNid")]
        public virtual OmsOrder Order { get; set; } = null!;
    }

    [Table("oms_status_mapping")]
    public class OmsStatusMapping
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("channel_sid")]
        public string ChannelSid { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        [Column("status_type")]
        public string StatusType { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("external_status")]
        public string ExternalStatus { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        [Column("oms_status")]
        public string OmsStatus { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("direction")]
        public string Direction { get; set; } = "BOTH";

        [Column("priority")]
        public int Priority { get; set; } = 0;

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }
    }

    [Table("oms_status_sync_job")]
    public class OmsStatusSyncJob
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("job_no")]
        public string JobNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        [Column("oms_order_sid")]
        public string OmsOrderSid { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        [Column("channel_account_sid")]
        public string ChannelAccountSid { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        [Column("sync_type")]
        public string SyncType { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("target_status")]
        public string? TargetStatus { get; set; }

        [Column("request_payload", TypeName = "json")]
        public string? RequestPayload { get; set; }

        [Column("response_payload", TypeName = "json")]
        public string? ResponsePayload { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("sync_status")]
        public string SyncStatus { get; set; } = "PENDING";

        [Column("retry_count")]
        public int RetryCount { get; set; } = 0;

        [Column("max_retry_count")]
        public int MaxRetryCount { get; set; } = 5;

        [Column("next_retry_date")]
        public DateTime? NextRetryDate { get; set; }

        [MaxLength(100)]
        [Column("external_reference_no")]
        public string? ExternalReferenceNo { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [Column("error_message")]
        public string? ErrorMessage { get; set; }
    }
    #endregion

    #region 07. 訂單取消 (Cancel Requests)
    [Table("oms_cancel_request")]
    public class OmsCancelRequest
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("cancel_no")]
        public string CancelNo { get; set; } = string.Empty;

        [Column("oms_order_nid")]
        public ulong OmsOrderNid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("requester_type")]
        public string RequesterType { get; set; } = string.Empty;

        [MaxLength(32)]
        [Column("requester_sid")]
        public string? RequesterSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("cancel_scope")]
        public string CancelScope { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("reason_code")]
        public string ReasonCode { get; set; } = string.Empty;

        [Column("reason_description")]
        public string? ReasonDescription { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("cancel_status")]
        public string CancelStatus { get; set; } = "PENDING";

        [Column("refund_required")]
        public bool RefundRequired { get; set; } = false;

        [Column("inventory_release_required")]
        public bool InventoryReleaseRequired { get; set; } = true;

        [Column("fulfillment_cancel_required")]
        public bool FulfillmentCancelRequired { get; set; } = true;

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [Column("error_message")]
        public string? ErrorMessage { get; set; }

        [ForeignKey("OmsOrderNid")]
        public virtual OmsOrder Order { get; set; } = null!;
        public virtual ICollection<OmsCancelItem> CancelItems { get; set; } = new HashSet<OmsCancelItem>();
    }

    [Table("oms_cancel_item")]
    public class OmsCancelItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("cancel_request_nid")]
        public ulong CancelRequestNid { get; set; }

        [Column("oms_order_item_nid")]
        public ulong OmsOrderItemNid { get; set; }

        [Column("request_qty", TypeName = "decimal(20,6)")]
        public decimal RequestQty { get; set; }

        [Column("approved_qty", TypeName = "decimal(20,6)")]
        public decimal ApprovedQty { get; set; } = 0;

        [Column("completed_qty", TypeName = "decimal(20,6)")]
        public decimal CompletedQty { get; set; } = 0;

        [Column("cancel_amount", TypeName = "decimal(20,4)")]
        public decimal CancelAmount { get; set; } = 0;

        [Required]
        [MaxLength(20)]
        [Column("cancel_status")]
        public string CancelStatus { get; set; } = "PENDING";

        [Column("remark")]
        public string? Remark { get; set; }

        [ForeignKey("CancelRequestNid")]
        public virtual OmsCancelRequest CancelRequest { get; set; } = null!;

        [ForeignKey("OmsOrderItemNid")]
        public virtual OmsOrderItem OrderItem { get; set; } = null!;
    }
    #endregion

    #region 08. 訂單例外與人工處理 (Exceptions)
    [Table("oms_exception")]
    public class OmsException
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("exception_no")]
        public string ExceptionNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        [Column("oms_order_sid")]
        public string OmsOrderSid { get; set; } = string.Empty;

        [MaxLength(32)]
        [Column("fulfillment_order_sid")]
        public string? FulfillmentOrderSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("exception_type")]
        public string ExceptionType { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("exception_code")]
        public string ExceptionCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("severity")]
        public string Severity { get; set; } = "WARNING";

        [Required]
        [MaxLength(500)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("exception_data", TypeName = "json")]
        public string? ExceptionData { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("exception_status")]
        public string ExceptionStatus { get; set; } = "OPEN";

        [MaxLength(32)]
        [Column("assigned_user_sid")]
        public string? AssignedUserSid { get; set; }

        [Column("assigned_date")]
        public DateTime? AssignedDate { get; set; }

        [Column("resolved_date")]
        public DateTime? ResolvedDate { get; set; }

        [MaxLength(50)]
        [Column("resolution_code")]
        public string? ResolutionCode { get; set; }

        [Column("resolution_note")]
        public string? ResolutionNote { get; set; }

        [Column("retryable")]
        public bool Retryable { get; set; } = true;

        [Column("retry_count")]
        public int RetryCount { get; set; } = 0;

        [Column("next_retry_date")]
        public DateTime? NextRetryDate { get; set; }

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        public virtual ICollection<OmsExceptionAction> ExceptionActions { get; set; } = new HashSet<OmsExceptionAction>();
    }

    [Table("oms_exception_action")]
    public class OmsExceptionAction
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("exception_nid")]
        public ulong ExceptionNid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("action_code")]
        public string ActionCode { get; set; } = string.Empty;

        [MaxLength(32)]
        [Column("action_user_sid")]
        public string? ActionUserSid { get; set; }

        [Column("action_data", TypeName = "json")]
        public string? ActionData { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("action_result")]
        public string ActionResult { get; set; } = "SUCCESS";

        [Column("message")]
        public string? Message { get; set; }

        [ForeignKey("ExceptionNid")]
        public virtual OmsException Exception { get; set; } = null!;
    }
    #endregion

    #region 09. 事件與協調 (Events & SLA)
    [Table("oms_order_event")]
    public class OmsOrderEvent
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(32)]
        [Column("oms_order_sid")]
        public string OmsOrderSid { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        [Column("event_code")]
        public string EventCode { get; set; } = string.Empty;

        [Column("event_version")]
        public int EventVersion { get; set; } = 1;

        [Column("event_data", TypeName = "json")]
        public string? EventData { get; set; }

        [MaxLength(80)]
        [Column("source_service_code")]
        public string? SourceServiceCode { get; set; }

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

    [Table("oms_sla_monitor")]
    public class OmsSlaMonitor
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("oms_order_sid")]
        public string OmsOrderSid { get; set; } = string.Empty;

        [MaxLength(32)]
        [Column("fulfillment_order_sid")]
        public string? FulfillmentOrderSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("sla_type")]
        public string SlaType { get; set; } = string.Empty;

        [Column("target_date")]
        public DateTime TargetDate { get; set; }

        [Column("actual_date")]
        public DateTime? ActualDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("sla_status")]
        public string SlaStatus { get; set; } = "PENDING";

        [Column("warning_date")]
        public DateTime? WarningDate { get; set; }

        [Column("breached_date")]
        public DateTime? BreachedDate { get; set; }

        [MaxLength(32)]
        [Column("notification_sid")]
        public string? NotificationSid { get; set; }

        [MaxLength(32)]
        [Column("exception_sid")]
        public string? ExceptionSid { get; set; }
    }
    #endregion
}