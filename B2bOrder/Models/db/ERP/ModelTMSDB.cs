// TMSModels.cs
using System;
using System.Collections.Generic;

namespace B2bOrder.Resources.ERP
{
    #region 01. Carriers, Vehicles, and Drivers (承運商、車隊、車輛與司機基礎架構)

    /// <summary>
    /// 承運商/物流業者主檔
    /// </summary>
    public class TmsCarrier
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 承運商序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public string CarrierCode { get; set; } = null!;        // 承運商代碼 (例: SF, HCT, TCAT)
        public string CarrierName { get; set; } = null!;        // 承運商名稱
        public string CarrierType { get; set; } = null!;        // OWN_FLEET自營車隊;THIRD_PARTY第三方物流;CROWD_SOURCED外包/機車快遞
        public string? ContactPerson { get; set; }              // 聯絡人
        public string? ContactPhone { get; set; }               // 聯絡電話
        public string CarrierStatus { get; set; } = null!;      // ACTIVE啟用;INACTIVE停用
        public string Avalible { get; set; } = null!;           // Y可用;D刪除;W停用
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual ICollection<TmsVehicle> Vehicles { get; set; } = new List<TmsVehicle>();
        public virtual ICollection<TmsDriver> Drivers { get; set; } = new List<TmsDriver>();
        public virtual ICollection<TmsDispatchTrip> DispatchTrips { get; set; } = new List<TmsDispatchTrip>();
        public virtual ICollection<TmsFreightSettlement> FreightSettlements { get; set; } = new List<TmsFreightSettlement>();
    }

    /// <summary>
    /// 運輸車輛檔
    /// </summary>
    public class TmsVehicle
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 車輛序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public ulong CarrierNid { get; set; }                   // 所屬承運商流水號
        public string LicensePlate { get; set; } = null!;       // 車牌號碼
        public string VehicleType { get; set; } = null!;        // MOTORCYCLE機車;VAN廂型車;TRUCK3_5T三噸半貨車;TRUCK_LARGE大型卡車;REEFER冷藏冷凍車
        public decimal? MaxWeightCapacity { get; set; }         // 最大載重(kg)
        public decimal? MaxVolumeCapacity { get; set; }         // 最大容積(m³)
        public string TemperatureType { get; set; } = null!;    // NORMAL常溫;COLD冷凍/冷藏;MULTI多溫層
        public string VehicleStatus { get; set; } = null!;      // IDLE空閒;IN_TRANSIT執勤中;MAINTENANCE保養修復中;INACTIVE停用
        public string Avalible { get; set; } = null!;           // Y可用;D刪除;W停用
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual TmsCarrier Carrier { get; set; } = null!;
        public virtual ICollection<TmsDispatchTrip> DispatchTrips { get; set; } = new List<TmsDispatchTrip>();
    }

    /// <summary>
    /// 駕駛司機檔
    /// </summary>
    public class TmsDriver
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 司機序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public ulong CarrierNid { get; set; }                   // 所屬承運商流水號
        public string DriverName { get; set; } = null!;         // 司機姓名
        public string PhoneNumber { get; set; } = null!;        // 手機號碼
        public string LicenseType { get; set; } = null!;        // REGULAR普通駕照;HEAVY大貨車駕照;TRAILER聯結車駕照
        public string? UserSid { get; set; }                    // 關聯帳號/APP登入序號
        public string DriverStatus { get; set; } = null!;       // AVAILABLE可派單;ON_TRIP出勤中;OFF_DUTY休假中;INACTIVE停用
        public string Avalible { get; set; } = null!;           // Y可用;D刪除;W停用
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual TmsCarrier Carrier { get; set; } = null!;
        public virtual ICollection<TmsDispatchTrip> DispatchTrips { get; set; } = new List<TmsDispatchTrip>();
    }

    #endregion

    #region 02. Dispatch Trips and Routes (派車車次 / 車程調度)

    /// <summary>
    /// 派車車次單
    /// </summary>
    public class TmsDispatchTrip
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 派車單序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public string TripNo { get; set; } = null!;             // 派車單號/車次號
        public ulong CarrierNid { get; set; }                   // 承運商流水號
        public ulong? VehicleNid { get; set; }                  // 指派車輛流水號
        public ulong? DriverNid { get; set; }                   // 指派司機流水號
        public DateTime? PlannedStartTime { get; set; }         // 預計發車時間
        public DateTime? PlannedEndTime { get; set; }           // 預計完成時間
        public DateTime? ActualStartTime { get; set; }          // 實際發車時間
        public DateTime? ActualEndTime { get; set; }            // 實際完成時間
        public decimal? TotalDistanceKm { get; set; }           // 總預估/實際行駛里程(km)
        public string TripStatus { get; set; } = null!;         // PLANNED已排程;DISPATCHED已派車;IN_TRANSIT運輸中;COMPLETED已完成;CANCELLED已取消
        public ulong VersionNo { get; set; }                    // 樂觀鎖版本
        public string Avalible { get; set; } = null!;           // Y可用;D刪除;W停用
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual TmsCarrier Carrier { get; set; } = null!;
        public virtual TmsVehicle? Vehicle { get; set; }
        public virtual TmsDriver? Driver { get; set; }
        public virtual ICollection<TmsWaybill> Waybills { get; set; } = new List<TmsWaybill>();
        public virtual ICollection<TmsTripStop> TripStops { get; set; } = new List<TmsTripStop>();
    }

    #endregion

    #region 03. Waybills, Items, and Stops (託運單與站點明細)

    /// <summary>
    /// 託運單/運單檔
    /// </summary>
    public class TmsWaybill
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 託運單序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public string WaybillNo { get; set; } = null!;          // 託運單號/託運號碼
        public string? TrackingNo { get; set; }                 // 物流包裹追蹤碼 (外部/快遞單號)
        public ulong? DispatchTripNid { get; set; }             // 歸屬派車單流水號
        public string? FulfillmentRequestSid { get; set; }      // FulfillmentDB 履約需求序號
        public string? OutboundOrderSid { get; set; }           // WMSDB 出貨單序號
        public string? InboundOrderSid { get; set; }            // WMSDB 進貨/調撥單序號
        public string? SalesOrderSid { get; set; }              // SalesOrderDB 銷售單序號
        public string SenderName { get; set; } = null!;         // 寄件人姓名
        public string SenderPhone { get; set; } = null!;        // 寄件人電話
        public string? SenderAddressSid { get; set; }           // 寄件地址序號
        public string SenderFullAddress { get; set; } = null!;  // 寄件完整地址
        public string ReceiverName { get; set; } = null!;       // 收件人姓名
        public string ReceiverPhone { get; set; } = null!;      // 收件人電話
        public string? ReceiverAddressSid { get; set; }         // 收件地址序號
        public string ReceiverFullAddress { get; set; } = null!; // 收件完整地址
        public int TotalPackages { get; set; }                  // 總件數/箱數
        public decimal? TotalWeightKg { get; set; }             // 總重量(kg)
        public decimal? TotalVolumeCbm { get; set; }            // 總體積(m³)
        public string TemperatureType { get; set; } = null!;    // NORMAL常溫;CHILLED冷藏;FROZEN冷凍
        public decimal CodAmount { get; set; }                  // 代收貨款金額 (Cash On Delivery)
        public string WaybillStatus { get; set; } = null!;      // CREATED已建立;ASSIGNED已派單;PICKED_UP已攬收;IN_TRANSIT運輸中;DELIVERING配送中;DELIVERED已簽收;EXCEPTION異常;FAILED簽退/拒收;CANCELLED取消
        public ulong VersionNo { get; set; }                    // 樂觀鎖版本
        public string Avalible { get; set; } = null!;           // Y可用;D刪除;W停用
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual TmsDispatchTrip? DispatchTrip { get; set; }
        public virtual ICollection<TmsWaybillItem> WaybillItems { get; set; } = new List<TmsWaybillItem>();
        public virtual ICollection<TmsTripStop> TripStops { get; set; } = new List<TmsTripStop>();
        public virtual ICollection<TmsTrackingEvent> TrackingEvents { get; set; } = new List<TmsTrackingEvent>();
        public virtual TmsProofOfDelivery? ProofOfDelivery { get; set; }
        public virtual ICollection<TmsFreightSettlement> FreightSettlements { get; set; } = new List<TmsFreightSettlement>();
    }

    /// <summary>
    /// 託運單包裹內容明細
    /// </summary>
    public class TmsWaybillItem
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 運單明細序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public ulong WaybillNid { get; set; }                   // 託運單流水號
        public int LineNo { get; set; }                         // 行號
        public string ItemSid { get; set; } = null!;            // PIM/MIMDB Item序號
        public string? VariantSid { get; set; }                 // PIM/MIMDB 變體序號
        public string ItemName { get; set; } = null!;           // 商品名稱
        public decimal Quantity { get; set; }                   // 托運數量
        public string UnitSid { get; set; } = null!;            // 單位序號
        public decimal? WeightKg { get; set; }                  // 單件重量(kg)
        public decimal? VolumeCbm { get; set; }                 // 單件體積(m³)
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual TmsWaybill Waybill { get; set; } = null!;
    }

    /// <summary>
    /// 派車車次停靠站點明細
    /// </summary>
    public class TmsTripStop
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 車次站點序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public ulong DispatchTripNid { get; set; }              // 派車單流水號
        public int StopSequence { get; set; }                   // 停靠順序 (1, 2, 3...)
        public string StopType { get; set; } = null!;           // PICKUP攬件/提貨;DELIVERY配送/卸貨;HUB轉運站/轉運中心
        public string LocationName { get; set; } = null!;       // 站點/地點名稱
        public string AddressFull { get; set; } = null!;        // 完整地址
        public ulong? WaybillNid { get; set; }                  // 關聯託運單流水號
        public DateTime? PlannedArrivalTime { get; set; }       // 預計抵達時間
        public DateTime? ActualArrivalTime { get; set; }        // 實際抵達時間
        public DateTime? ActualDepartureTime { get; set; }      // 實際離開時間
        public string StopStatus { get; set; } = null!;         // PENDING待停靠;ARRIVED已抵達;COMPLETED已完成作業;SKIPPED略過
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual TmsDispatchTrip DispatchTrip { get; set; } = null!;
        public virtual TmsWaybill? Waybill { get; set; }
    }

    #endregion

    #region 04. Tracking and Proof of Delivery (軌跡追蹤與 POD)

    /// <summary>
    /// 物流軌跡與歷程檔
    /// </summary>
    public class TmsTrackingEvent
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 軌跡事件序號
        public DateTime CreateDate { get; set; }                // 事件記錄時間
        public ulong WaybillNid { get; set; }                   // 託運單流水號
        public string EventCode { get; set; } = null!;          // 事件代碼 (例: PICKED_UP, HUB_ARRIVED, OUT_FOR_DELIVERY, DELIVERED, EXCEPTION)
        public DateTime EventTime { get; set; }                 // 事件發生時間
        public string? EventLocation { get; set; }              // 發生地點/轉運站名稱
        public string EventDescription { get; set; } = null!;   // 事件說明
        public string? OperatorName { get; set; }               // 操作人員/司機姓名
        public decimal? Latitude { get; set; }                  // GPS 緯度
        public decimal? Longitude { get; set; }                 // GPS 經度
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual TmsWaybill Waybill { get; set; } = null!;
    }

    /// <summary>
    /// 貨物簽收證明 (Proof of Delivery)
    /// </summary>
    public class TmsProofOfDelivery
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 簽收單/POD序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public ulong WaybillNid { get; set; }                   // 託運單流水號
        public string SignedBy { get; set; } = null!;           // 簽收人姓名
        public DateTime SignedTime { get; set; }                // 簽收時間
        public string? SignatureImageUrl { get; set; }          // 電子簽名圖檔 URL
        public string? PhotoEvidenceUrl { get; set; }           // 現場拍照/簽收證明照片 URL
        public string PodStatus { get; set; } = null!;          // SUCCESS成功簽收;AGENT_RECEIVED管理室/代收;EXCEPTIONAL異常簽收
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual TmsWaybill Waybill { get; set; } = null!;
    }

    #endregion

    #region 05. Freight Cost Settlement (運費核算與費用結算)

    /// <summary>
    /// 運費結算明細檔
    /// </summary>
    public class TmsFreightSettlement
    {
        public ulong Nid { get; set; }                          // 流水號
        public string Sid { get; set; } = null!;                // 運費結算單序號
        public DateTime CreateDate { get; set; }                // 建立日期
        public DateTime? ModifyDate { get; set; }               // 修改日期
        public string SettlementNo { get; set; } = null!;       // 結算單號
        public ulong CarrierNid { get; set; }                   // 承運商流水號
        public ulong WaybillNid { get; set; }                   // 託運單流水號
        public decimal BaseFreightAmount { get; set; }          // 基本運費
        public decimal FuelSurcharge { get; set; }              // 燃油附加費
        public decimal SpecialServiceFee { get; set; }          // 特殊服務費 (例: 上樓費、冷藏費)
        public decimal TotalAmount { get; set; }                // 運費總金額
        public string CurrencyCode { get; set; } = null!;       // 幣別
        public string SettlementStatus { get; set; } = null!;   // UNSETTLED未結算;AUDITED已審核;PAID已付款;CANCELLED取消
        public ulong VersionNo { get; set; }                    // 樂觀鎖版本
        public string Avalible { get; set; } = null!;           // Y可用;D刪除;W停用
        public string? Remark { get; set; }                     // 備註

        // 導覽屬性 (Navigation Properties)
        public virtual TmsCarrier Carrier { get; set; } = null!;
        public virtual TmsWaybill Waybill { get; set; } = null!;
    }

    #endregion
}