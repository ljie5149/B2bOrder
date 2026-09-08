// SalesOrderModels.cs
using System;
using System.Collections.Generic;

namespace B2bOrder.Resources.eCommerce
{
    #region 01. 銷售類型與政策 (Sales Types & Policies)

    /// <summary>
    /// 銷售單類型 (Sales Order Type)
    /// </summary>
    public class SalOrderType
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 銷售類型序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public string OrderTypeCode { get; set; } = null!;      // 銷售類型代碼
        public string OrderTypeName { get; set; } = null!;      // 銷售類型名稱
        public string OrderCategory { get; set; } = null!;      // RETAIL零售;B2B企業;PROJECT專案;CONTRACT合約;SERVICE服務;INTERNAL內部;RETURN退貨
        public bool InventoryEffect { get; set; }               // 是否影響庫存
        public bool ApprovalRequired { get; set; }              // 是否需要簽核
        public bool CreditCheckRequired { get; set; }           // 是否需信用檢查
        public bool ContractRequired { get; set; }              // 是否需合約
        public bool FulfillmentRequired { get; set; }           // 是否需履約
        public bool InvoiceRequired { get; set; }               // 是否需發票
        public string OrderTypeStatus { get; set; } = null!;    // ACTIVE啟用;INACTIVE停用
        public string Avalible { get; set; } = null!;           // Y可用;D刪除;W停用
        public string? Remark { get; set; }                     // 備註
    }

    /// <summary>
    /// 銷售政策 (Sales Policy)
    /// </summary>
    public class SalPolicy
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 銷售政策序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public string PolicyCode { get; set; } = null!;         // 政策代碼
        public string PolicyName { get; set; } = null!;         // 政策名稱
        public string? CompanySid { get; set; }                 // MasterDB公司序號
        public string? OrderTypeSid { get; set; }               // 銷售類型序號
        public bool AllowPartialDelivery { get; set; }          // 是否允許分批交貨
        public bool AllowBackorder { get; set; }                // 是否允許缺貨待補
        public bool AllowOverDelivery { get; set; }             // 是否允許超交
        public decimal OverDeliveryRate { get; set; }           // 允許超交率
        public bool CancellationAllowed { get; set; }           // 是否允許取消
        public int CancellationCutoffMin { get; set; }          // 取消截止分鐘
        public int PriceLockMinutes { get; set; }               // 價格鎖定分鐘
        public int StockLockMinutes { get; set; }               // 庫存鎖定分鐘
        public bool CreditHoldEnabled { get; set; }             // 是否啟用信用凍結
        public string PolicyStatus { get; set; } = null!;       // ACTIVE啟用;INACTIVE停用
        public string Avalible { get; set; } = null!;           // Y可用;D刪除;W停用
        public string? Remark { get; set; }                     // 備註
    }

    #endregion

    #region 02. 報價 (Quotations)

    /// <summary>
    /// 銷售報價單 (Sales Quotation)
    /// </summary>
    public class SalQuote
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 報價單序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public string QuoteNo { get; set; } = null!;            // 報價單號
        public string CompanySid { get; set; } = null!;         // 公司序號
        public string? BusinessUnitSid { get; set; }            // 營運單位序號
        public string? DepartmentSid { get; set; }              // 部門序號
        public string OrderTypeSid { get; set; } = null!;       // 銷售類型序號
        public string CustomerPartySid { get; set; } = null!;   // PartyDB客戶序號
        public string? ContactPartySid { get; set; }            // 聯絡人Party序號
        public string? ProjectSid { get; set; }                 // 專案序號
        public string? SiteSid { get; set; }                    // 工地序號
        public string? ContractSid { get; set; }                // 合約序號
        public string? OpportunitySid { get; set; }             // CRMDB商機序號
        public string? SalesOwnerUserSid { get; set; }          // 負責業務帳號序號
        public DateTime QuoteDate { get; set; }                 // 報價日期
        public DateTime? ValidUntil { get; set; }               // 有效期限
        public string CurrencySid { get; set; } = null!;        // 幣別序號
        public decimal ExchangeRate { get; set; }               // 匯率
        public string? PriceListSid { get; set; }               // PricingDB價格清單序號
        public string? AgreementSid { get; set; }               // PricingDB價格協議序號
        public decimal SubtotalAmount { get; set; }             // 未稅小計
        public decimal DiscountAmount { get; set; }             // 折扣金額
        public decimal TaxAmount { get; set; }                  // 稅額
        public decimal FreightAmount { get; set; }              // 運費
        public decimal OtherAmount { get; set; }                // 其他費用
        public decimal TotalAmount { get; set; }                // 報價總額
        public string? PaymentTermSid { get; set; }             // 付款條件序號
        public string? DeliveryTermCode { get; set; }           // 交貨條件
        public string QuoteStatus { get; set; } = null!;        // DRAFT草稿;SUBMITTED已送出;REVIEW待審;APPROVED核准;SENT已送客戶;ACCEPTED接受;REJECTED拒絕;EXPIRED過期;CONVERTED已轉單;CANCELLED取消
        public string? WorkflowInstanceSid { get; set; }        // WorkflowDB流程實例序號
        public string? ApprovedUserSid { get; set; }            // 核准人員序號
        public DateTime? ApprovedDate { get; set; }             // 核准時間
        public DateTime? AcceptedDate { get; set; }             // 客戶接受時間
        public ulong VersionNo { get; set; }                    // 樂觀鎖版本
        public string Avalible { get; set; } = null!;           // Y可用;D刪除;W停用
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual ICollection<SalQuoteItem> QuoteItems { get; set; } = new List<SalQuoteItem>();
    }

    /// <summary>
    /// 銷售報價明細 (Sales Quotation Item)
    /// </summary>
    public class SalQuoteItem
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 報價明細序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public ulong QuoteNid { get; set; }                     // 報價單流水號
        public int LineNo { get; set; }                         // 明細行號
        public string? ItemSid { get; set; }                    // PIM/MIMDB Item序號
        public string? VariantSid { get; set; }                 // PIM/MIMDB變體序號
        public string ItemDescription { get; set; } = null!;    // 品名或服務說明
        public string? SpecificationText { get; set; }          // 規格說明
        public decimal Quantity { get; set; }                   // 數量
        public string UnitSid { get; set; } = null!;            // 單位序號
        public decimal ListPrice { get; set; }                  // 牌價
        public decimal UnitPrice { get; set; }                  // 報價單價
        public decimal DiscountRate { get; set; }               // 折扣率
        public decimal DiscountAmount { get; set; }             // 折扣金額
        public string? TaxSid { get; set; }                     // 稅別序號
        public decimal TaxAmount { get; set; }                  // 稅額
        public decimal LineAmount { get; set; }                 // 明細總額
        public DateTime? RequestedDeliveryDate { get; set; }    // 客戶需求交期
        public string? ProjectSid { get; set; }                 // 專案序號
        public string? SiteSid { get; set; }                    // 工地序號
        public string? WbsSid { get; set; }                     // WBS序號
        public string? ContractItemSid { get; set; }            // 合約明細序號
        public string? PricingResultSid { get; set; }           // PricingDB計價結果序號
        public string? PriceSnapshot { get; set; }              // 價格計算快照 (JSON)
        public string ItemStatus { get; set; } = null!;         // OPEN有效;CANCELLED取消;CONVERTED已轉單
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual SalQuote Quote { get; set; } = null!;
    }

    #endregion

    #region 03. 銷售單 (Sales Orders)

    /// <summary>
    /// 共用銷售訂單 (Sales Order)
    /// </summary>
    public class SalOrder
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 銷售單序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public string SalesOrderNo { get; set; } = null!;       // 銷售單號
        public string? ExternalOrderNo { get; set; }            // 外部訂單號
        public string CompanySid { get; set; } = null!;         // 公司序號
        public string? BusinessUnitSid { get; set; }            // 營運單位序號
        public string? DepartmentSid { get; set; }              // 部門序號
        public string OrderTypeSid { get; set; } = null!;       // 銷售類型序號
        public string CustomerPartySid { get; set; } = null!;   // 客戶Party序號
        public string? MemberPartySid { get; set; }             // 會員Party序號
        public string? ContactPartySid { get; set; }            // 聯絡人Party序號
        public string? QuoteSid { get; set; }                   // 來源報價單序號
        public string? ChannelSid { get; set; }                 // 通路序號
        public string? StoreSid { get; set; }                   // 商店序號
        public string? ProjectSid { get; set; }                 // 專案序號
        public string? SiteSid { get; set; }                    // 工地序號
        public string? ContractSid { get; set; }                // 合約序號
        public string? SalesOwnerUserSid { get; set; }          // 負責業務帳號序號
        public DateTime OrderDate { get; set; }                 // 下單時間
        public DateTime? RequestedDeliveryDate { get; set; }    // 客戶需求交期
        public string CurrencySid { get; set; } = null!;        // 幣別序號
        public decimal ExchangeRate { get; set; }               // 匯率
        public string? PriceListSid { get; set; }               // 價格清單序號
        public string? PriceLevelSid { get; set; }              // 價格層級序號
        public string? AgreementSid { get; set; }               // 價格協議序號
        public decimal SubtotalAmount { get; set; }             // 未稅小計
        public decimal DiscountAmount { get; set; }             // 折扣金額
        public decimal PromotionAmount { get; set; }            // 活動優惠金額
        public decimal TaxAmount { get; set; }                  // 稅額
        public decimal FreightAmount { get; set; }              // 運費
        public decimal ServiceAmount { get; set; }              // 服務費
        public decimal OtherAmount { get; set; }                // 其他費用
        public decimal TotalAmount { get; set; }                // 訂單總額
        public decimal PaidAmount { get; set; }                 // 已付款金額
        public decimal RefundedAmount { get; set; }             // 已退款金額
        public string? PaymentTermSid { get; set; }             // 付款條件序號
        public string? PaymentMethodSid { get; set; }           // 付款方式序號
        public string? BillingAddressSid { get; set; }          // 帳單地址序號
        public string? ShippingAddressSid { get; set; }         // 配送地址序號
        public string? InvoiceTitle { get; set; }               // 發票抬頭
        public string? InvoiceTaxNo { get; set; }               // 發票統編
        public string SourceType { get; set; } = null!;         // SHOPPING商城;B2B;QUOTE報價;CRM商機;PROJECT專案;CONTRACT合約;API;MANUAL人工
        public string? SourceSid { get; set; }                  // 來源資料序號
        public string CreditCheckStatus { get; set; } = null!;  // NOT_REQUIRED不需要;PENDING待檢;PASS通過;HOLD凍結;FAIL失敗
        public string PaymentStatus { get; set; } = null!;      // UNPAID未付;PARTIAL部分付款;PAID已付;REFUNDING退款中;PARTIAL_REFUNDED部分退款;REFUNDED已退款
        public string FulfillmentStatus { get; set; } = null!;  // UNFULFILLED未履約;RESERVED已預留;PARTIAL部分履約;FULFILLED已完成;CANCELLED取消
        public string InvoiceStatus { get; set; } = null!;      // NOT_ISSUED未開;PARTIAL部分開立;ISSUED已開;VOID作廢;ALLOWANCE折讓
        public string OrderStatus { get; set; } = null!;        // DRAFT草稿;PENDING_PAYMENT待付款;SUBMITTED已送出;REVIEW待審;APPROVED核准;CONFIRMED已確認;PROCESSING處理中;PARTIAL_FULFILLED部分履約;COMPLETED完成;ON_HOLD暫停;CANCELLED取消;CLOSED結案
        public string? WorkflowInstanceSid { get; set; }        // WorkflowDB流程實例序號
        public string? ApprovedUserSid { get; set; }            // 核准人員序號
        public DateTime? ApprovedDate { get; set; }             // 核准時間
        public DateTime? ConfirmedDate { get; set; }            // 確認時間
        public DateTime? CompletedDate { get; set; }            // 完成時間
        public string? CancelReasonCode { get; set; }           // 取消原因代碼
        public string? CancelReason { get; set; }               // 取消原因
        public ulong VersionNo { get; set; }                    // 樂觀鎖版本
        public string Avalible { get; set; } = null!;           // Y可用;D刪除;W停用
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual ICollection<SalOrderItem> OrderItems { get; set; } = new List<SalOrderItem>();
        public virtual ICollection<SalOrderAddress> OrderAddresses { get; set; } = new List<SalOrderAddress>();
        public virtual ICollection<SalOrderContact> OrderContacts { get; set; } = new List<SalOrderContact>();
    }

    /// <summary>
    /// 共用銷售訂單明細 (Sales Order Item)
    /// </summary>
    public class SalOrderItem
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 銷售單明細序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public ulong SalesOrderNid { get; set; }                // 銷售單流水號
        public int LineNo { get; set; }                         // 明細行號
        public string? QuoteItemSid { get; set; }               // 來源報價明細序號
        public string? ItemSid { get; set; }                    // Item序號
        public string? VariantSid { get; set; }                 // 變體序號
        public string ItemType { get; set; } = null!;           // PRODUCT商品;MATERIAL材料;EQUIPMENT設備;SERVICE服務;BUNDLE組合;WORK_ITEM工項;OTHER
        public string ItemDescription { get; set; } = null!;    // 品名或服務說明
        public string? SpecificationText { get; set; }          // 規格說明
        public decimal Quantity { get; set; }                   // 訂購數量
        public string UnitSid { get; set; } = null!;            // 單位序號
        public decimal ListPrice { get; set; }                  // 牌價
        public decimal BasePrice { get; set; }                  // 基礎價格
        public decimal UnitPrice { get; set; }                  // 成交單價
        public decimal DiscountRate { get; set; }               // 折扣率
        public decimal DiscountAmount { get; set; }             // 折扣金額
        public decimal PromotionAmount { get; set; }            // 活動優惠金額
        public string? TaxSid { get; set; }                     // 稅別序號
        public decimal TaxRate { get; set; }                    // 稅率快照
        public decimal TaxAmount { get; set; }                  // 稅額
        public decimal LineAmount { get; set; }                 // 明細總額
        public DateTime? RequestedDeliveryDate { get; set; }    // 需求交期
        public string? WarehouseSid { get; set; }               // 履約倉庫序號
        public string? ProjectSid { get; set; }                 // 專案序號
        public string? SiteSid { get; set; }                    // 工地序號
        public string? WbsSid { get; set; }                     // WBS序號
        public string? ContractItemSid { get; set; }            // 合約明細序號
        public string? PricingResultSid { get; set; }           // PricingDB計價結果序號
        public string? PriceSnapshot { get; set; }              // 價格快照 (JSON)
        public string? ItemSnapshot { get; set; }               // Item名稱、規格與屬性快照 (JSON)
        public decimal ReservedQty { get; set; }                // 已預留數量
        public decimal FulfilledQty { get; set; }               // 已履約數量
        public decimal InvoicedQty { get; set; }                // 已開票數量
        public decimal ReturnedQty { get; set; }                // 已退貨數量
        public decimal CancelledQty { get; set; }               // 取消數量
        public decimal BackorderQty { get; set; }               // 缺貨待補數量
        public string ItemStatus { get; set; } = null!;         // OPEN待處理;RESERVED已預留;PARTIAL部分履約;FULFILLED已履約;BACKORDER缺貨待補;RETURNED已退貨;CANCELLED取消;CLOSED結束
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual SalOrder SalesOrder { get; set; } = null!;
        public virtual ICollection<SalDeliverySchedule> DeliverySchedules { get; set; } = new List<SalDeliverySchedule>();
    }

    #endregion

    #region 04. 交期與履約需求 (Delivery Schedules & Fulfillment Requests)

    /// <summary>
    /// 銷售交期與履約排程 (Sales Delivery Schedule)
    /// </summary>
    public class SalDeliverySchedule
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 交期排程序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public ulong SalesOrderItemNid { get; set; }            // 銷售明細流水號
        public int ScheduleNo { get; set; }                     // 排程序號
        public decimal ScheduledQty { get; set; }               // 排程數量
        public DateTime PlannedDeliveryDate { get; set; }       // 計畫交貨日
        public DateTime? ConfirmedDeliveryDate { get; set; }    // 確認交貨日
        public DateTime? ActualDeliveryDate { get; set; }       // 實際交貨日
        public string? WarehouseSid { get; set; }               // 履約倉庫序號
        public string? ShippingAddressSid { get; set; }         // 配送地址序號
        public string FulfillmentMethod { get; set; } = null!;  // DELIVERY配送;PICKUP自取;DIGITAL數位;SERVICE服務;PROJECT_SITE工地交付
        public string ScheduleStatus { get; set; } = null!;     // PLANNED計畫;CONFIRMED確認;RESERVED已預留;PARTIAL部分履約;FULFILLED完成;DELAYED延遲;CANCELLED取消
        public string? DelayReason { get; set; }                // 延遲原因

        // 導覽屬性 (Navigation Properties)
        public virtual SalOrderItem SalesOrderItem { get; set; } = null!;
    }

    /// <summary>
    /// 銷售訂單履約申請 (Sales Fulfillment Request)
    /// </summary>
    public class SalFulfillmentRequest
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 履約申請序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public string RequestNo { get; set; } = null!;          // 履約申請編號
        public string SalesOrderSid { get; set; } = null!;      // 銷售單序號
        public string FulfillmentType { get; set; } = null!;    // SHIPMENT出貨;PICKUP自取;DIGITAL數位;SERVICE服務;PROJECT_DELIVERY專案交付;INSTALLATION安裝
        public string? WarehouseSid { get; set; }               // 履約倉庫序號
        public string? ProjectSid { get; set; }                 // 專案序號
        public string? SiteSid { get; set; }                    // 工地序號
        public string? ShippingAddressSid { get; set; }         // 配送地址序號
        public DateTime RequestedDate { get; set; }             // 申請時間
        public DateTime? RequiredDate { get; set; }             // 要求完成時間
        public string? ReservationSid { get; set; }             // InventoryDB庫存預留序號
        public string? FulfillmentOrderSid { get; set; }        // FulfillmentDB履約單序號
        public string RequestStatus { get; set; } = null!;      // PENDING待處理;RESERVED已預留;ACCEPTED已接受;PROCESSING處理中;PARTIAL部分完成;COMPLETED完成;FAILED失敗;CANCELLED取消
        public string? CorrelationId { get; set; }              // 跨服務關聯識別碼
        public string? ErrorMessage { get; set; }               // 錯誤訊息 (LONGTEXT)

        // 導覽屬性 (Navigation Properties)
        public virtual ICollection<SalFulfillmentRequestItem> RequestItems { get; set; } = new List<SalFulfillmentRequestItem>();
    }

    /// <summary>
    /// 銷售履約申請明細 (Sales Fulfillment Request Item)
    /// </summary>
    public class SalFulfillmentRequestItem
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 履約申請明細序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public ulong FulfillmentRequestNid { get; set; }        // 履約申請流水號
        public int LineNo { get; set; }                         // 明細行號
        public string SalesOrderItemSid { get; set; } = null!;  // 銷售明細序號
        public string? DeliveryScheduleSid { get; set; }        // 交期排程序號
        public decimal RequestQty { get; set; }                 // 履約數量
        public decimal FulfilledQty { get; set; }               // 已履約數量
        public string UnitSid { get; set; } = null!;            // 單位序號
        public string ItemStatus { get; set; } = null!;         // PENDING待處理;RESERVED已預留;PROCESSING處理中;PARTIAL部分完成;COMPLETED完成;FAILED失敗;CANCELLED取消

        // 導覽屬性 (Navigation Properties)
        public virtual SalFulfillmentRequest FulfillmentRequest { get; set; } = null!;
    }

    #endregion

    #region 05. 訂單地址與聯絡快照 (Order Addresses & Contacts Snapshots)

    /// <summary>
    /// 訂單地址快照 (Sales Order Address)
    /// </summary>
    public class SalOrderAddress
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 訂單地址序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public ulong SalesOrderNid { get; set; }                // 銷售單流水號
        public string AddressType { get; set; } = null!;        // BILLING帳單;SHIPPING配送;CONTACT聯絡;PROJECT_SITE工地
        public string? SourceAddressSid { get; set; }           // PartyDB或MasterDB來源地址序號
        public string RecipientName { get; set; } = null!;      // 收件人
        public string? RecipientPhone { get; set; }             // 收件電話
        public string CountrySid { get; set; } = null!;         // 國家序號
        public string? RegionLevel1Sid { get; set; }            // 第一層行政區序號 (Region)
        public string? RegionLevel2Sid { get; set; }            // 第二層行政區序號 (Region)
        public string? RegionLevel3Sid { get; set; }            // 第三層行政區序號 (Region)
        public string? PostalCode { get; set; }                 // 郵遞區號
        public string AddressLine1 { get; set; } = null!;       // 主要地址
        public string? AddressLine2 { get; set; }               // 補充地址
        public decimal? Latitude { get; set; }                  // 緯度
        public decimal? Longitude { get; set; }                 // 經度
        public string? DeliveryNote { get; set; }               // 配送備註

        // 導覽屬性 (Navigation Properties)
        public virtual SalOrder SalesOrder { get; set; } = null!;
    }

    /// <summary>
    /// 訂單聯絡人快照 (Sales Order Contact)
    /// </summary>
    public class SalOrderContact
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 訂單聯絡人序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public ulong SalesOrderNid { get; set; }                // 銷售單流水號
        public string ContactType { get; set; } = null!;        // ORDER下單;BILLING帳務;SHIPPING配送;PROJECT專案;EMERGENCY緊急
        public string? SourcePartySid { get; set; }             // PartyDB來源Party序號
        public string ContactName { get; set; } = null!;        // 聯絡人姓名
        public string? ContactEmail { get; set; }               // Email
        public string? ContactMobile { get; set; }              // 手機
        public string? ContactPhone { get; set; }               // 電話
        public string? DepartmentName { get; set; }             // 部門名稱快照
        public string? JobTitle { get; set; }                   // 職稱快照
        public bool PrimaryMark { get; set; }                   // 是否主要聯絡人

        // 導覽屬性 (Navigation Properties)
        public virtual SalOrder SalesOrder { get; set; } = null!;
    }

    #endregion

    #region 06. 付款與應收串接 (Payments & Receivables Integration)

    /// <summary>
    /// 銷售訂單付款請求 (Sales Payment Request)
    /// </summary>
    public class SalPaymentRequest
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 付款請求序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public string PaymentRequestNo { get; set; } = null!;   // 付款請求編號
        public string SalesOrderSid { get; set; } = null!;      // 銷售單序號
        public string PaymentMethodSid { get; set; } = null!;   // 付款方式序號
        public string CurrencySid { get; set; } = null!;        // 幣別序號
        public decimal RequestedAmount { get; set; }            // 請求付款金額
        public DateTime? DueDate { get; set; }                  // 付款期限
        public string? ExternalPaymentSid { get; set; }         // PaymentDB付款單序號
        public string? VirtualAccountNo { get; set; }           // 虛擬帳號
        public string? PaymentUrl { get; set; }                 // 付款網址
        public string RequestStatus { get; set; } = null!;      // PENDING待付款;PROCESSING處理中;PAID已付款;FAILED失敗;EXPIRED過期;CANCELLED取消
        public DateTime? PaidDate { get; set; }                 // 付款完成時間
        public string? CorrelationId { get; set; }              // 跨服務關聯識別碼
        public string? ErrorMessage { get; set; }               // 錯誤訊息 (LONGTEXT)
    }

    /// <summary>
    /// 銷售訂單應收帳款建立請求 (Sales Receivable Request)
    /// </summary>
    public class SalReceivableRequest
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 應收建立請求序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public string SalesOrderSid { get; set; } = null!;      // 銷售單序號
        public string RequestType { get; set; } = null!;        // DEPOSIT訂金;MILESTONE里程碑;DELIVERY交貨;INVOICE開票;FINAL尾款;ADJUSTMENT調整
        public string? ReferenceType { get; set; }              // 來源類型
        public string? ReferenceSid { get; set; }               // 來源資料序號
        public string CurrencySid { get; set; } = null!;        // 幣別序號
        public decimal ReceivableAmount { get; set; }           // 應收金額
        public DateTime? DueDate { get; set; }                  // 到期日
        public string? AccountingReceivableSid { get; set; }    // AccountingDB應收單序號
        public string RequestStatus { get; set; } = null!;      // PENDING待處理;PROCESSING處理中;SUCCESS成功;FAILED失敗;CANCELLED取消
        public string? CorrelationId { get; set; }              // 跨服務關聯識別碼
        public string? ErrorMessage { get; set; }               // 錯誤訊息 (LONGTEXT)
    }

    #endregion

    #region 07. 發票串接 (Invoices Integration)

    /// <summary>
    /// 銷售訂單發票建立請求 (Sales Invoice Request)
    /// </summary>
    public class SalInvoiceRequest
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 發票建立請求序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public string InvoiceRequestNo { get; set; } = null!;   // 發票請求編號
        public string SalesOrderSid { get; set; } = null!;      // 銷售單序號
        public string RequestType { get; set; } = null!;        // FULL全額;PARTIAL部分;DEPOSIT訂金;MILESTONE里程碑;FINAL尾款
        public DateTime? InvoiceDate { get; set; }              // 預計開票日
        public string? InvoiceTitle { get; set; }               // 發票抬頭
        public string? InvoiceTaxNo { get; set; }               // 統一編號
        public string? CarrierType { get; set; }                // MOBILE手機條碼;CERT自然人憑證;DONATE捐贈;MEMBER會員載具;NONE無
        public string? CarrierNo { get; set; }                  // 載具號碼
        public string? DonationCode { get; set; }               // 捐贈碼
        public string CurrencySid { get; set; } = null!;        // 幣別序號
        public decimal InvoiceAmount { get; set; }              // 開票金額
        public decimal TaxAmount { get; set; }                  // 稅額
        public string? InvoiceTaxSid { get; set; }              // InvoiceTaxDB發票序號
        public string RequestStatus { get; set; } = null!;      // PENDING待處理;PROCESSING處理中;ISSUED已開立;FAILED失敗;CANCELLED取消
        public string? CorrelationId { get; set; }              // 跨服務關聯識別碼
        public string? ErrorMessage { get; set; }               // 錯誤訊息 (LONGTEXT)

        // 導覽屬性 (Navigation Properties)
        public virtual ICollection<SalInvoiceRequestItem> InvoiceRequestItems { get; set; } = new List<SalInvoiceRequestItem>();
    }

    /// <summary>
    /// 銷售訂單發票請求明細 (Sales Invoice Request Item)
    /// </summary>
    public class SalInvoiceRequestItem
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 發票請求明細序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public ulong InvoiceRequestNid { get; set; }            // 發票請求流水號
        public int LineNo { get; set; }                         // 明細行號
        public string SalesOrderItemSid { get; set; } = null!;  // 銷售單明細序號
        public decimal InvoiceQty { get; set; }                 // 開票數量
        public decimal UnitPrice { get; set; }                  // 開票單價
        public decimal TaxRate { get; set; }                    // 稅率
        public decimal TaxAmount { get; set; }                  // 稅額
        public decimal LineAmount { get; set; }                 // 開票金額

        // 導覽屬性 (Navigation Properties)
        public virtual SalInvoiceRequest InvoiceRequest { get; set; } = null!;
    }

    #endregion

    #region 08. 訂單變更與取消 (Order Changes & Cancellations)

    /// <summary>
    /// 銷售訂單變更申請 (Sales Order Change)
    /// </summary>
    public class SalOrderChange
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 銷售單變更序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public string ChangeNo { get; set; } = null!;           // 變更單號
        public string SalesOrderSid { get; set; } = null!;      // 銷售單序號
        public string ChangeType { get; set; } = null!;         // QUANTITY數量;PRICE價格;DELIVERY交期;ADDRESS地址;ITEM品項;PAYMENT付款;CANCEL取消;OTHER
        public string? BeforeData { get; set; }                 // 變更前資料 (JSON)
        public string RequestedData { get; set; } = null!;      // 申請變更資料 (JSON)
        public string? ChangedFields { get; set; }              // 變更欄位 (JSON)
        public decimal AmountDifference { get; set; }           // 金額差異
        public string ChangeReason { get; set; } = null!;       // 變更原因
        public string RequesterType { get; set; } = null!;      // CUSTOMER客戶;USER內部人員;SYSTEM系統
        public string? RequesterSid { get; set; }               // 申請對象序號
        public string ChangeStatus { get; set; } = null!;       // DRAFT草稿;SUBMITTED已送出;REVIEW待審;APPROVED核准;REJECTED駁回;APPLIED已套用;CANCELLED取消
        public string? WorkflowInstanceSid { get; set; }        // WorkflowDB流程實例序號
        public string? ApprovedUserSid { get; set; }            // 核准人員序號
        public DateTime? ApprovedDate { get; set; }             // 核准時間
        public DateTime? AppliedDate { get; set; }              // 套用時間
    }

    /// <summary>
    /// 銷售訂單取消流程 (Sales Order Cancellation)
    /// </summary>
    public class SalCancellation
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 訂單取消序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public string CancellationNo { get; set; } = null!;     // 取消申請編號
        public string SalesOrderSid { get; set; } = null!;      // 銷售單序號
        public string CancellationType { get; set; } = null!;   // FULL整單;PARTIAL部分
        public string ReasonCode { get; set; } = null!;         // 取消原因代碼
        public string? Reason { get; set; }                     // 取消原因
        public string RequestedByType { get; set; } = null!;    // CUSTOMER客戶;USER內部人員;SYSTEM系統
        public string? RequestedBySid { get; set; }             // 申請對象序號
        public bool RefundRequired { get; set; }                // 是否需要退款
        public bool RestockRequired { get; set; }               // 是否需要回補庫存
        public string CancellationStatus { get; set; } = null!; // PENDING待處理;REVIEW待審;APPROVED核准;REJECTED駁回;PROCESSING處理中;COMPLETED完成;FAILED失敗
        public string? WorkflowInstanceSid { get; set; }        // WorkflowDB流程實例序號
        public DateTime? CompletedDate { get; set; }            // 完成時間
        public string? ErrorMessage { get; set; }               // 錯誤訊息 (LONGTEXT)

        // 導覽屬性 (Navigation Properties)
        public virtual ICollection<SalCancellationItem> CancellationItems { get; set; } = new List<SalCancellationItem>();
    }

    /// <summary>
    /// 銷售訂單取消明細 (Sales Order Cancellation Item)
    /// </summary>
    public class SalCancellationItem
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 訂單取消明細序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public ulong CancellationNid { get; set; }              // 取消主檔流水號
        public string SalesOrderItemSid { get; set; } = null!;  // 銷售明細序號
        public decimal CancelQty { get; set; }                  // 取消數量
        public decimal CancelAmount { get; set; }               // 取消金額
        public decimal FulfilledQtySnapshot { get; set; }       // 取消時已履約數量
        public string ItemStatus { get; set; } = null!;         // PENDING待處理;APPROVED核准;REJECTED駁回;COMPLETED完成

        // 導覽屬性 (Navigation Properties)
        public virtual SalCancellation Cancellation { get; set; } = null!;
    }

    #endregion

    #region 09. 訂單註記與附件 (Order Notes & Attachments)

    /// <summary>
    /// 銷售訂單註記 (Sales Order Note)
    /// </summary>
    public class SalOrderNote
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 訂單註記序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public string SalesOrderSid { get; set; } = null!;      // 銷售單序號
        public string NoteType { get; set; } = null!;           // CUSTOMER客戶;INTERNAL內部;WAREHOUSE倉庫;PAYMENT付款;DELIVERY配送;PROJECT專案;SYSTEM系統
        public string NoteContent { get; set; } = null!;        // 註記內容
        public string Visibility { get; set; } = null!;         // INTERNAL內部;CUSTOMER客戶可見;PARTNER合作夥伴可見
        public bool ImportantMark { get; set; }                 // 是否重要
        public string? CreateUserSid { get; set; }              // 建立人員序號
    }

    /// <summary>
    /// 銷售訂單附件 (Sales Order Attachment)
    /// </summary>
    public class SalOrderAttachment
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 訂單附件序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public string SalesOrderSid { get; set; } = null!;      // 銷售單序號
        public string AttachmentType { get; set; } = null!;     // CONTRACT合約;PURCHASE_ORDER客戶採購單;DRAWING圖面;SPEC規格;APPROVAL核准;OTHER
        public string FileSid { get; set; } = null!;            // FileDB檔案序號
        public string? FileName { get; set; }                   // 檔名快照
        public int VersionNo { get; set; }                      // 附件版本
        public bool CustomerVisible { get; set; }               // 是否客戶可見
        public string? CreateUserSid { get; set; }              // 建立人員序號
    }

    #endregion

    #region 10. 狀態歷程與事件 (Status Histories & Domain Events)

    /// <summary>
    /// 銷售狀態歷程 (Sales Status History)
    /// </summary>
    public class SalStatusHistory
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 銷售狀態歷程序號
        public DateTime CreateDate { get; set; }                // 異動時間
        public string EntityType { get; set; } = null!;         // QUOTE報價;ORDER訂單;ITEM明細;FULFILLMENT履約;PAYMENT付款;INVOICE發票;CHANGE變更;CANCELLATION取消
        public string EntitySid { get; set; } = null!;          // 銷售實體序號
        public string? OldStatus { get; set; }                  // 原狀態
        public string NewStatus { get; set; } = null!;          // 新狀態
        public string? EventCode { get; set; }                  // 觸發事件代碼
        public string? SourceServiceCode { get; set; }          // 來源服務代碼
        public string? SourceReferenceSid { get; set; }         // 來源資料序號
        public string? OperatorUserSid { get; set; }            // 操作人員序號
        public string? ReasonCode { get; set; }                 // 原因代碼
        public string? Reason { get; set; }                     // 原因說明
        public string? CorrelationId { get; set; }              // 跨服務關聯識別碼
    }

    /// <summary>
    /// 銷售領域事件 (Sales Domain Event)
    /// </summary>
    public class SalEvent
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 銷售事件序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public string EntityType { get; set; } = null!;         // QUOTE;ORDER;ITEM;FULFILLMENT;PAYMENT;INVOICE;CHANGE;CANCELLATION
        public string EntitySid { get; set; } = null!;          // 實體序號
        public string EventCode { get; set; } = null!;          // 事件代碼
        public int EventVersion { get; set; }                   // 事件版本
        public string? EventData { get; set; }                  // 事件內容 (JSON)
        public string? SourceEventId { get; set; }              // 來源事件ID
        public string? CorrelationId { get; set; }              // 關聯識別碼
        public string? CausationId { get; set; }                // 因果事件ID
        public string? OutboxEventSid { get; set; }             // IntegrationDB Outbox事件序號
        public string ProcessStatus { get; set; } = null!;      // PENDING待處理;SUCCESS成功;FAILED失敗;IGNORED忽略
        public DateTime? ProcessedDate { get; set; }            // 處理時間
        public string? ErrorMessage { get; set; }               // 錯誤訊息 (TEXT)
    }

    #endregion
}