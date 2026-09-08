using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.System.Models
{
    public class ReportDBModel
    {
        // 此類別作為容器或可依專案需求拆分，以下為 ReportDB 各資料表 Model 與中文 Region
    }

    #region 01. 報表定義與匯出任務 (Report Definitions & Export Jobs)
    [Table("rpt_report_definition")]
    public class RptReportDefinitionModel
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
        [Column("report_code")]
        public string ReportCode { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("report_name")]
        public string ReportName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("report_category")]
        public string ReportCategory { get; set; } = null!;

        [MaxLength(80)]
        [Column("source_service_code")]
        public string? SourceServiceCode { get; set; }

        [Column("query_config", TypeName = "json")]
        public string? QueryConfig { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("default_format")]
        public string DefaultFormat { get; set; } = "XLSX";

        [Column("allow_schedule")]
        public bool AllowSchedule { get; set; } = true;

        [Column("sort_no")]
        public int SortNo { get; set; } = 0;

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }

    [Table("rpt_export_job")]
    public class RptExportJobModel
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
        [MaxLength(80)]
        [Column("job_no")]
        public string JobNo { get; set; } = null!;

        [Column("report_definition_nid")]
        public ulong ReportDefinitionNid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("requester_sid")]
        public string RequesterSid { get; set; } = null!;

        [Column("filter_json", TypeName = "json")]
        public string? FilterJson { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("export_format")]
        public string ExportFormat { get; set; } = "XLSX";

        [Required]
        [MaxLength(20)]
        [Column("job_status")]
        public string JobStatus { get; set; } = "PENDING";

        [Column("progress_percent")]
        public int ProgressPercent { get; set; } = 0;

        [MaxLength(255)]
        [Column("file_name")]
        public string? FileName { get; set; }

        [MaxLength(1000)]
        [Column("file_url")]
        public string? FileUrl { get; set; }

        [Column("file_size")]
        public ulong? FileSize { get; set; }

        [Column("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [Column("error_message", TypeName = "longtext")]
        public string? ErrorMessage { get; set; }
    }

    [Table("rpt_schedule")]
    public class RptScheduleModel
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
        [MaxLength(200)]
        [Column("schedule_name")]
        public string ScheduleName { get; set; } = null!;

        [Column("report_definition_nid")]
        public ulong ReportDefinitionNid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("cron_expression")]
        public string CronExpression { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("timezone_code")]
        public string TimezoneCode { get; set; } = "Asia/Taipei";

        [Column("filter_json", TypeName = "json")]
        public string? FilterJson { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("export_format")]
        public string ExportFormat { get; set; } = "XLSX";

        [Required]
        [MaxLength(20)]
        [Column("delivery_type")]
        public string DeliveryType { get; set; } = "EMAIL";

        [Column("recipients_json", TypeName = "json")]
        public string? RecipientsJson { get; set; }

        [Column("last_run_date")]
        public DateTime? LastRunDate { get; set; }

        [Column("next_run_date")]
        public DateTime? NextRunDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("schedule_status")]
        public string ScheduleStatus { get; set; } = "ACTIVE";

        [Required]
        [MaxLength(32)]
        [Column("owner_sid")]
        public string OwnerSid { get; set; } = null!;

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 02. 銷售彙總 (Sales Summaries)
    [Table("rpt_sales_daily")]
    public class RptSalesDailyModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("statistics_date", TypeName = "date")]
        public DateTime StatisticsDate { get; set; }

        [MaxLength(32)]
        [Column("store_sid")]
        public string? StoreSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("order_count")]
        public ulong OrderCount { get; set; } = 0;

        [Column("paid_order_count")]
        public ulong PaidOrderCount { get; set; } = 0;

        [Column("cancelled_order_count")]
        public ulong CancelledOrderCount { get; set; } = 0;

        [Column("completed_order_count")]
        public ulong CompletedOrderCount { get; set; } = 0;

        [Column("item_quantity", TypeName = "decimal(20,6)")]
        public decimal ItemQuantity { get; set; } = 0.000000m;

        [Column("gross_sales_amount", TypeName = "decimal(20,4)")]
        public decimal GrossSalesAmount { get; set; } = 0.0000m;

        [Column("discount_amount", TypeName = "decimal(20,4)")]
        public decimal DiscountAmount { get; set; } = 0.0000m;

        [Column("freight_amount", TypeName = "decimal(20,4)")]
        public decimal FreightAmount { get; set; } = 0.0000m;

        [Column("tax_amount", TypeName = "decimal(20,4)")]
        public decimal TaxAmount { get; set; } = 0.0000m;

        [Column("refund_amount", TypeName = "decimal(20,4)")]
        public decimal RefundAmount { get; set; } = 0.0000m;

        [Column("net_sales_amount", TypeName = "decimal(20,4)")]
        public decimal NetSalesAmount { get; set; } = 0.0000m;

        [Column("average_order_amount", TypeName = "decimal(20,4)")]
        public decimal AverageOrderAmount { get; set; } = 0.0000m;

        [Column("new_member_order_count")]
        public ulong NewMemberOrderCount { get; set; } = 0;

        [Column("update_date")]
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }

    [Table("rpt_product_sales_daily")]
    public class RptProductSalesDailyModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("statistics_date", TypeName = "date")]
        public DateTime StatisticsDate { get; set; }

        [MaxLength(32)]
        [Column("store_sid")]
        public string? StoreSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("product_sid")]
        public string ProductSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("sku_sid")]
        public string SkuSid { get; set; } = null!;

        [MaxLength(32)]
        [Column("category_sid")]
        public string? CategorySid { get; set; }

        [MaxLength(32)]
        [Column("brand_sid")]
        public string? BrandSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("order_count")]
        public ulong OrderCount { get; set; } = 0;

        [Column("sold_qty", TypeName = "decimal(20,6)")]
        public decimal SoldQty { get; set; } = 0.000000m;

        [Column("returned_qty", TypeName = "decimal(20,6)")]
        public decimal ReturnedQty { get; set; } = 0.000000m;

        [Column("gross_sales_amount", TypeName = "decimal(20,4)")]
        public decimal GrossSalesAmount { get; set; } = 0.0000m;

        [Column("discount_amount", TypeName = "decimal(20,4)")]
        public decimal DiscountAmount { get; set; } = 0.0000m;

        [Column("refund_amount", TypeName = "decimal(20,4)")]
        public decimal RefundAmount { get; set; } = 0.0000m;

        [Column("net_sales_amount", TypeName = "decimal(20,4)")]
        public decimal NetSalesAmount { get; set; } = 0.0000m;

        [Column("cost_amount", TypeName = "decimal(20,4)")]
        public decimal CostAmount { get; set; } = 0.0000m;

        [Column("gross_profit_amount", TypeName = "decimal(20,4)")]
        public decimal GrossProfitAmount { get; set; } = 0.0000m;

        [Column("update_date")]
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }
    #endregion

    #region 03. 會員彙總 (Member Summaries)
    [Table("rpt_member_daily")]
    public class RptMemberDailyModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("statistics_date", TypeName = "date")]
        public DateTime StatisticsDate { get; set; }

        [MaxLength(32)]
        [Column("price_group_sid")]
        public string? PriceGroupSid { get; set; }

        [Column("registered_count")]
        public ulong RegisteredCount { get; set; } = 0;

        [Column("active_member_count")]
        public ulong ActiveMemberCount { get; set; } = 0;

        [Column("purchasing_member_count")]
        public ulong PurchasingMemberCount { get; set; } = 0;

        [Column("first_purchase_count")]
        public ulong FirstPurchaseCount { get; set; } = 0;

        [Column("repeat_purchase_count")]
        public ulong RepeatPurchaseCount { get; set; } = 0;

        [Column("login_count")]
        public ulong LoginCount { get; set; } = 0;

        [Column("average_order_amount", TypeName = "decimal(20,4)")]
        public decimal AverageOrderAmount { get; set; } = 0.0000m;

        [Column("total_sales_amount", TypeName = "decimal(20,4)")]
        public decimal TotalSalesAmount { get; set; } = 0.0000m;

        [Column("update_date")]
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }
    #endregion

    #region 04. 庫存彙總 (Inventory Summaries)
    [Table("rpt_inventory_daily")]
    public class RptInventoryDailyModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("statistics_date", TypeName = "date")]
        public DateTime StatisticsDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("product_sid")]
        public string ProductSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("sku_sid")]
        public string SkuSid { get; set; } = null!;

        [Column("on_hand_qty", TypeName = "decimal(20,6)")]
        public decimal OnHandQty { get; set; } = 0.000000m;

        [Column("reserved_qty", TypeName = "decimal(20,6)")]
        public decimal ReservedQty { get; set; } = 0.000000m;

        [Column("available_qty", TypeName = "decimal(20,6)")]
        public decimal AvailableQty { get; set; } = 0.000000m;

        [Column("damaged_qty", TypeName = "decimal(20,6)")]
        public decimal DamagedQty { get; set; } = 0.000000m;

        [Column("in_transit_qty", TypeName = "decimal(20,6)")]
        public decimal InTransitQty { get; set; } = 0.000000m;

        [Column("inventory_value", TypeName = "decimal(20,4)")]
        public decimal InventoryValue { get; set; } = 0.0000m;

        [Column("average_cost", TypeName = "decimal(20,6)")]
        public decimal AverageCost { get; set; } = 0.000000m;

        [Column("days_of_inventory", TypeName = "decimal(12,2)")]
        public decimal? DaysOfInventory { get; set; }

        [Column("no_movement_days")]
        public int NoMovementDays { get; set; } = 0;

        [Column("update_date")]
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }

    [Table("rpt_inventory_alert")]
    public class RptInventoryAlertModel
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

        [Column("alert_date", TypeName = "date")]
        public DateTime AlertDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("warehouse_sid")]
        public string WarehouseSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("product_sid")]
        public string ProductSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("sku_sid")]
        public string SkuSid { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("alert_type")]
        public string AlertType { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("alert_level")]
        public string AlertLevel { get; set; } = "WARNING";

        [Column("current_qty", TypeName = "decimal(20,6)")]
        public decimal CurrentQty { get; set; } = 0.000000m;

        [Column("threshold_qty", TypeName = "decimal(20,6)")]
        public decimal? ThresholdQty { get; set; }

        [Column("expiry_date", TypeName = "date")]
        public DateTime? ExpiryDate { get; set; }

        [Column("no_movement_days")]
        public int? NoMovementDays { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("alert_status")]
        public string AlertStatus { get; set; } = "OPEN";

        [MaxLength(32)]
        [Column("resolved_user_sid")]
        public string? ResolvedUserSid { get; set; }

        [Column("resolved_date")]
        public DateTime? ResolvedDate { get; set; }

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 05. 採購與供應商績效 (Purchasing & Supplier Performance)
    [Table("rpt_purchase_daily")]
    public class RptPurchaseDailyModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("statistics_date", TypeName = "date")]
        public DateTime StatisticsDate { get; set; }

        [MaxLength(32)]
        [Column("supplier_sid")]
        public string? SupplierSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("purchase_order_count")]
        public ulong PurchaseOrderCount { get; set; } = 0;

        [Column("ordered_qty", TypeName = "decimal(20,6)")]
        public decimal OrderedQty { get; set; } = 0.000000m;

        [Column("received_qty", TypeName = "decimal(20,6)")]
        public decimal ReceivedQty { get; set; } = 0.000000m;

        [Column("accepted_qty", TypeName = "decimal(20,6)")]
        public decimal AcceptedQty { get; set; } = 0.000000m;

        [Column("rejected_qty", TypeName = "decimal(20,6)")]
        public decimal RejectedQty { get; set; } = 0.000000m;

        [Column("returned_qty", TypeName = "decimal(20,6)")]
        public decimal ReturnedQty { get; set; } = 0.000000m;

        [Column("purchase_amount", TypeName = "decimal(20,4)")]
        public decimal PurchaseAmount { get; set; } = 0.0000m;

        [Column("received_amount", TypeName = "decimal(20,4)")]
        public decimal ReceivedAmount { get; set; } = 0.0000m;

        [Column("return_amount", TypeName = "decimal(20,4)")]
        public decimal ReturnAmount { get; set; } = 0.0000m;

        [Column("update_date")]
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }

    [Table("rpt_supplier_performance")]
    public class RptSupplierPerformanceModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("period_type")]
        public string PeriodType { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("period_code")]
        public string PeriodCode { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("supplier_sid")]
        public string SupplierSid { get; set; } = null!;

        [Column("purchase_order_count")]
        public ulong PurchaseOrderCount { get; set; } = 0;

        [Column("purchase_amount", TypeName = "decimal(20,4)")]
        public decimal PurchaseAmount { get; set; } = 0.0000m;

        [Column("on_time_delivery_rate", TypeName = "decimal(8,4)")]
        public decimal OnTimeDeliveryRate { get; set; } = 0.0000m;

        [Column("acceptance_rate", TypeName = "decimal(8,4)")]
        public decimal AcceptanceRate { get; set; } = 0.0000m;

        [Column("return_rate", TypeName = "decimal(8,4)")]
        public decimal ReturnRate { get; set; } = 0.0000m;

        [Column("average_lead_time_days", TypeName = "decimal(10,2)")]
        public decimal AverageLeadTimeDays { get; set; } = 0.00m;

        [Column("price_variance_rate", TypeName = "decimal(8,4)")]
        public decimal PriceVarianceRate { get; set; } = 0.0000m;

        [Column("performance_score", TypeName = "decimal(8,2)")]
        public decimal PerformanceScore { get; set; } = 0.00m;

        [Column("update_date")]
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }
    #endregion

    #region 06. 財務與商店結算彙總 (Finance & Store Settlements)
    [Table("rpt_finance_daily")]
    public class RptFinanceDailyModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("statistics_date", TypeName = "date")]
        public DateTime StatisticsDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("receivable_amount", TypeName = "decimal(20,4)")]
        public decimal ReceivableAmount { get; set; } = 0.0000m;

        [Column("receipt_amount", TypeName = "decimal(20,4)")]
        public decimal ReceiptAmount { get; set; } = 0.0000m;

        [Column("refund_amount", TypeName = "decimal(20,4)")]
        public decimal RefundAmount { get; set; } = 0.0000m;

        [Column("payable_amount", TypeName = "decimal(20,4)")]
        public decimal PayableAmount { get; set; } = 0.0000m;

        [Column("payment_amount", TypeName = "decimal(20,4)")]
        public decimal PaymentAmount { get; set; } = 0.0000m;

        [Column("platform_fee_amount", TypeName = "decimal(20,4)")]
        public decimal PlatformFeeAmount { get; set; } = 0.0000m;

        [Column("payment_fee_amount", TypeName = "decimal(20,4)")]
        public decimal PaymentFeeAmount { get; set; } = 0.0000m;

        [Column("shipping_fee_amount", TypeName = "decimal(20,4)")]
        public decimal ShippingFeeAmount { get; set; } = 0.0000m;

        [Column("outstanding_ar_amount", TypeName = "decimal(20,4)")]
        public decimal OutstandingArAmount { get; set; } = 0.0000m;

        [Column("outstanding_ap_amount", TypeName = "decimal(20,4)")]
        public decimal OutstandingApAmount { get; set; } = 0.0000m;

        [Column("update_date")]
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }

    [Table("rpt_store_settlement_period")]
    public class RptStoreSettlementPeriodModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("period_start_date", TypeName = "date")]
        public DateTime PeriodStartDate { get; set; }

        [Column("period_end_date", TypeName = "date")]
        public DateTime PeriodEndDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("store_sid")]
        public string StoreSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("order_count")]
        public ulong OrderCount { get; set; } = 0;

        [Column("gross_sales_amount", TypeName = "decimal(20,4)")]
        public decimal GrossSalesAmount { get; set; } = 0.0000m;

        [Column("refund_amount", TypeName = "decimal(20,4)")]
        public decimal RefundAmount { get; set; } = 0.0000m;

        [Column("discount_share_amount", TypeName = "decimal(20,4)")]
        public decimal DiscountShareAmount { get; set; } = 0.0000m;

        [Column("platform_fee_amount", TypeName = "decimal(20,4)")]
        public decimal PlatformFeeAmount { get; set; } = 0.0000m;

        [Column("payment_fee_amount", TypeName = "decimal(20,4)")]
        public decimal PaymentFeeAmount { get; set; } = 0.0000m;

        [Column("shipping_fee_amount", TypeName = "decimal(20,4)")]
        public decimal ShippingFeeAmount { get; set; } = 0.0000m;

        [Column("adjustment_amount", TypeName = "decimal(20,4)")]
        public decimal AdjustmentAmount { get; set; } = 0.0000m;

        [Column("payable_amount", TypeName = "decimal(20,4)")]
        public decimal PayableAmount { get; set; } = 0.0000m;

        [Column("paid_amount", TypeName = "decimal(20,4)")]
        public decimal PaidAmount { get; set; } = 0.0000m;

        [Column("update_date")]
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }
    #endregion

    #region 07. 彙總工作紀錄 (Report Build Job Logs)
    [Table("rpt_build_job")]
    public class RptBuildJobModel
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
        [MaxLength(80)]
        [Column("job_no")]
        public string JobNo { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("job_type")]
        public string JobType { get; set; } = null!;

        [Column("statistics_date", TypeName = "date")]
        public DateTime? StatisticsDate { get; set; }

        [Column("period_start_date", TypeName = "date")]
        public DateTime? PeriodStartDate { get; set; }

        [Column("period_end_date", TypeName = "date")]
        public DateTime? PeriodEndDate { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("read_count")]
        public ulong ReadCount { get; set; } = 0;

        [Column("write_count")]
        public ulong WriteCount { get; set; } = 0;

        [Column("error_count")]
        public ulong ErrorCount { get; set; } = 0;

        [Required]
        [MaxLength(20)]
        [Column("job_status")]
        public string JobStatus { get; set; } = "RUNNING";

        [Column("error_message", TypeName = "longtext")]
        public string? ErrorMessage { get; set; }
    }
    #endregion
}