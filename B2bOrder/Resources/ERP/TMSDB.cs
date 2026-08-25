namespace B2bOrder.Resources.ERP
{
    /// <summary>
    /// TMSDB V1 Shared Core Schema
    /// 設計目標：
    /// 1. 支援車隊 (Fleet)、車輛 (Vehicle)、司機 (Driver) 與承運商 (Carrier) 基礎資源管理
    /// 2. 管理運送單/託運單 (Waybill / Transport Order) 與多點停靠站 (Stop Points)
    /// 3. 支援派車排程單 (Dispatch Trip / Wave) 集中派遣與調度
    /// 4. 提供即時運送軌跡/狀態節點 (Tracking Events / GPS Logs) 紀錄
    /// 5. 整合與串接 FulfillmentDB (履約需求)、WMSDB (出貨/入庫) 與 SalesOrderDB
    /// 6. 支援運費試算與結算費用明細 (Freight Cost Settlement)
    /// 7. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 8. avalible：Y可用；W停用；D刪除
    /// 9. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class TMSDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 承運商、車隊、車輛與司機基礎架構
-- =========================================================

CREATE TABLE IF NOT EXISTS tms_carrier (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '承運商序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    carrier_code            VARCHAR(50)                         NOT NULL COMMENT '承運商代碼 (例: SF, HCT, TCAT)',
    carrier_name            VARCHAR(200)                        NOT NULL COMMENT '承運商名稱',
    carrier_type            VARCHAR(30)                         NOT NULL DEFAULT 'THIRD_PARTY' COMMENT 'OWN_FLEET自營車隊;THIRD_PARTY第三方物流;CROWD_SOURCED外包/機車快遞',
    contact_person          VARCHAR(100)                            NULL COMMENT '聯絡人',
    contact_phone           VARCHAR(50)                             NULL COMMENT '聯絡電話',
    carrier_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_tc_carrier_code UNIQUE (carrier_code),
    INDEX idx_tc_carrier_type (carrier_type),
    INDEX idx_tc_status (carrier_status),
    INDEX idx_tc_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='承運商/物流業者主檔';

CREATE TABLE IF NOT EXISTS tms_vehicle (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '車輛序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    carrier_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '所屬承運商流水號',
    license_plate           VARCHAR(20)                         NOT NULL COMMENT '車牌號碼',
    vehicle_type            VARCHAR(30)                         NOT NULL DEFAULT 'VAN' COMMENT 'MOTORCYCLE機車;VAN廂型車;TRUCK3_5T三噸半貨車;TRUCK_LARGE大型卡車;REEFER冷藏冷凍車',
    max_weight_capacity     DECIMAL(12,4)                           NULL COMMENT '最大載重(kg)',
    max_volume_capacity     DECIMAL(12,4)                           NULL COMMENT '最大容積(m³)',
    temperature_type        VARCHAR(20)                         NOT NULL DEFAULT 'NORMAL' COMMENT 'NORMAL常溫;COLD冷凍/冷藏;MULTI多溫層',
    vehicle_status          VARCHAR(20)                         NOT NULL DEFAULT 'IDLE' COMMENT 'IDLE空閒;IN_TRANSIT執勤中;MAINTENANCE保養修復中;INACTIVE停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_tv_carrier
        FOREIGN KEY (carrier_nid) REFERENCES tms_carrier(nid),
    CONSTRAINT uk_tv_license_plate UNIQUE (license_plate),
    INDEX idx_tv_carrier_nid (carrier_nid),
    INDEX idx_tv_vehicle_type (vehicle_type),
    INDEX idx_tv_status (vehicle_status),
    INDEX idx_tv_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='運輸車輛檔';

CREATE TABLE IF NOT EXISTS tms_driver (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '司機序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    carrier_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '所屬承運商流水號',
    driver_name             VARCHAR(100)                        NOT NULL COMMENT '司機姓名',
    phone_number            VARCHAR(50)                         NOT NULL COMMENT '手機號碼',
    license_type            VARCHAR(30)                         NOT NULL DEFAULT 'REGULAR' COMMENT 'REGULAR普通駕照;HEAVY大貨車駕照;TRAILER聯結車駕照',
    user_sid                VARCHAR(32)                             NULL COMMENT '關聯帳號/APP登入序號',
    driver_status           VARCHAR(20)                         NOT NULL DEFAULT 'AVAILABLE' COMMENT 'AVAILABLE可派單;ON_TRIP出勤中;OFF_DUTY休假中;INACTIVE停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_td_carrier
        FOREIGN KEY (carrier_nid) REFERENCES tms_carrier(nid),
    INDEX idx_td_carrier_nid (carrier_nid),
    INDEX idx_td_phone (phone_number),
    INDEX idx_td_status (driver_status),
    INDEX idx_td_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='駕駛司機檔';

-- =========================================================
-- 02. 派車車次 / 車程調度 (Dispatch Trip / Route)
-- =========================================================

CREATE TABLE IF NOT EXISTS tms_dispatch_trip (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '派車單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    trip_no                 VARCHAR(100)                        NOT NULL COMMENT '派車單號/車次號',
    carrier_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '承運商流水號',
    vehicle_nid             BIGINT UNSIGNED                         NULL COMMENT '指派車輛流水號',
    driver_nid              BIGINT UNSIGNED                         NULL COMMENT '指派司機流水號',
    planned_start_time      DATETIME                                NULL COMMENT '預計發車時間',
    planned_end_time        DATETIME                                NULL COMMENT '預計完成時間',
    actual_start_time       DATETIME                                NULL COMMENT '實際發車時間',
    actual_end_time         DATETIME                                NULL COMMENT '實際完成時間',
    total_distance_km       DECIMAL(10,2)                           NULL COMMENT '總預估/實際行駛里程(km)',
    trip_status             VARCHAR(30)                         NOT NULL DEFAULT 'PLANNED' COMMENT 'PLANNED已排程;DISPATCHED已派車;IN_TRANSIT運輸中;COMPLETED已完成;CANCELLED已取消',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_tdt_carrier
        FOREIGN KEY (carrier_nid) REFERENCES tms_carrier(nid),
    CONSTRAINT fk_tdt_vehicle
        FOREIGN KEY (vehicle_nid) REFERENCES tms_vehicle(nid),
    CONSTRAINT fk_tdt_driver
        FOREIGN KEY (driver_nid) REFERENCES tms_driver(nid),
    CONSTRAINT uk_tdt_trip_no UNIQUE (trip_no),
    INDEX idx_tdt_carrier_nid (carrier_nid),
    INDEX idx_tdt_vehicle_nid (vehicle_nid),
    INDEX idx_tdt_driver_nid (driver_nid),
    INDEX idx_tdt_status (trip_status),
    INDEX idx_tdt_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='派車車次單';

-- =========================================================
-- 03. 託運單與站點明細 (Waybill / Transport Order & Stops)
-- =========================================================

CREATE TABLE IF NOT EXISTS tms_waybill (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '託運單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    waybill_no              VARCHAR(100)                        NOT NULL COMMENT '託運單號/託運號碼',
    tracking_no             VARCHAR(100)                            NULL COMMENT '物流包裹追蹤碼 (外部/快遞單號)',
    dispatch_trip_nid       BIGINT UNSIGNED                         NULL COMMENT '歸屬派車單流水號',
    fulfillment_request_sid VARCHAR(32)                             NULL COMMENT 'FulfillmentDB 履約需求序號',
    outbound_order_sid      VARCHAR(32)                             NULL COMMENT 'WMSDB 出貨單序號',
    inbound_order_sid       VARCHAR(32)                             NULL COMMENT 'WMSDB 進貨/調撥單序號',
    sales_order_sid         VARCHAR(32)                             NULL COMMENT 'SalesOrderDB 銷售單序號',
    sender_name             VARCHAR(100)                        NOT NULL COMMENT '寄件人姓名',
    sender_phone            VARCHAR(50)                         NOT NULL COMMENT '寄件人電話',
    sender_address_sid      VARCHAR(32)                             NULL COMMENT '寄件地址序號',
    sender_full_address     VARCHAR(500)                        NOT NULL COMMENT '寄件完整地址',
    receiver_name           VARCHAR(100)                        NOT NULL COMMENT '收件人姓名',
    receiver_phone          VARCHAR(50)                         NOT NULL COMMENT '收件人電話',
    receiver_address_sid    VARCHAR(32)                             NULL COMMENT '收件地址序號',
    receiver_full_address   VARCHAR(500)                        NOT NULL COMMENT '收件完整地址',
    total_packages          INT                                 NOT NULL DEFAULT 1 COMMENT '總件數/箱數',
    total_weight_kg         DECIMAL(12,4)                           NULL COMMENT '總重量(kg)',
    total_volume_cbm        DECIMAL(12,4)                           NULL COMMENT '總體積(m³)',
    temperature_type        VARCHAR(20)                         NOT NULL DEFAULT 'NORMAL' COMMENT 'NORMAL常溫;CHILLED冷藏;FROZEN冷凍',
    cod_amount              DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '代收貨款金額 (Cash On Delivery)',
    waybill_status          VARCHAR(30)                         NOT NULL DEFAULT 'CREATED' COMMENT 'CREATED已建立;ASSIGNED已派單;PICKED_UP已攬收;IN_TRANSIT運輸中;DELIVERING配送中;DELIVERED已簽收;EXCEPTION異常;FAILED簽退/拒收;CANCELLED取消',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_tw_trip
        FOREIGN KEY (dispatch_trip_nid) REFERENCES tms_dispatch_trip(nid),
    CONSTRAINT uk_tw_waybill_no UNIQUE (waybill_no),
    INDEX idx_tw_tracking_no (tracking_no),
    INDEX idx_tw_trip_nid (dispatch_trip_nid),
    INDEX idx_tw_fulfillment_sid (fulfillment_request_sid),
    INDEX idx_tw_outbound_sid (outbound_order_sid),
    INDEX idx_tw_status (waybill_status),
    INDEX idx_tw_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='託運單/運單檔';

CREATE TABLE IF NOT EXISTS tms_waybill_item (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '運單明細序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    waybill_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '託運單流水號',
    line_no                 INT                                 NOT NULL COMMENT '行號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'PIM/MIMDB Item序號',
    variant_sid             VARCHAR(32)                             NULL COMMENT 'PIM/MIMDB 變體序號',
    item_name               VARCHAR(200)                        NOT NULL COMMENT '商品名稱',
    quantity                DECIMAL(20,6)                       NOT NULL COMMENT '托運數量',
    unit_sid                VARCHAR(32)                         NOT NULL COMMENT '單位序號',
    weight_kg               DECIMAL(12,4)                           NULL COMMENT '單件重量(kg)',
    volume_cbm              DECIMAL(12,4)                           NULL COMMENT '單件體積(m³)',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_twi_waybill
        FOREIGN KEY (waybill_nid) REFERENCES tms_waybill(nid),
    CONSTRAINT uk_twi_waybill_line UNIQUE (waybill_nid, line_no),
    INDEX idx_twi_waybill_nid (waybill_nid),
    INDEX idx_twi_item_variant (item_sid, variant_sid),
    CHECK (line_no > 0),
    CHECK (quantity > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='託運單包裹內容明細';

CREATE TABLE IF NOT EXISTS tms_trip_stop (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '車次站點序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    dispatch_trip_nid       BIGINT UNSIGNED                     NOT NULL COMMENT '派車單流水號',
    stop_sequence           INT                                 NOT NULL COMMENT '停靠順序 (1, 2, 3...)',
    stop_type               VARCHAR(20)                         NOT NULL DEFAULT 'DELIVERY' COMMENT 'PICKUP攬件/提貨;DELIVERY配送/卸貨;HUB轉運站/轉運中心',
    location_name           VARCHAR(200)                        NOT NULL COMMENT '站點/地點名稱',
    address_full            VARCHAR(500)                        NOT NULL COMMENT '完整地址',
    waybill_nid             BIGINT UNSIGNED                         NULL COMMENT '關聯託運單流水號',
    planned_arrival_time    DATETIME                                NULL COMMENT '預計抵達時間',
    actual_arrival_time     DATETIME                                NULL COMMENT '實際抵達時間',
    actual_departure_time   DATETIME                                NULL COMMENT '實際離開時間',
    stop_status             VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待停靠;ARRIVED已抵達;COMPLETED已完成作業;SKIPPED略過',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_tts_trip
        FOREIGN KEY (dispatch_trip_nid) REFERENCES tms_dispatch_trip(nid),
    CONSTRAINT fk_tts_waybill
        FOREIGN KEY (waybill_nid) REFERENCES tms_waybill(nid),
    CONSTRAINT uk_tts_trip_seq UNIQUE (dispatch_trip_nid, stop_sequence),
    INDEX idx_tts_trip_nid (dispatch_trip_nid),
    INDEX idx_tts_waybill_nid (waybill_nid),
    INDEX idx_tts_status (stop_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='派車車次停靠站點明細';

-- =========================================================
-- 04. 軌跡追蹤與 POD (Tracking Events & Proof of Delivery)
-- =========================================================

CREATE TABLE IF NOT EXISTS tms_tracking_event (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '軌跡事件序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '事件記錄時間',
    waybill_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '託運單流水號',
    event_code              VARCHAR(50)                         NOT NULL COMMENT '事件代碼 (例: PICKED_UP, HUB_ARRIVED, OUT_FOR_DELIVERY, DELIVERED, EXCEPTION)',
    event_time              DATETIME                            NOT NULL COMMENT '事件發生時間',
    event_location          VARCHAR(200)                            NULL COMMENT '發生地點/轉運站名稱',
    event_description       TEXT                                NOT NULL COMMENT '事件說明',
    operator_name           VARCHAR(100)                            NULL COMMENT '操作人員/司機姓名',
    latitude                DECIMAL(10,7)                           NULL COMMENT 'GPS 緯度',
    longitude               DECIMAL(10,7)                           NULL COMMENT 'GPS 經度',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_tte_waybill
        FOREIGN KEY (waybill_nid) REFERENCES tms_waybill(nid),
    INDEX idx_tte_waybill_nid (waybill_nid),
    INDEX idx_tte_event_code (event_code),
    INDEX idx_tte_event_time (event_time)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='物流軌跡與歷程檔';

CREATE TABLE IF NOT EXISTS tms_proof_of_delivery (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '簽收單/POD序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    waybill_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '託運單流水號',
    signed_by               VARCHAR(100)                        NOT NULL COMMENT '簽收人姓名',
    signed_time             DATETIME                            NOT NULL COMMENT '簽收時間',
    signature_image_url     VARCHAR(500)                            NULL COMMENT '電子簽名圖檔 URL',
    photo_evidence_url      VARCHAR(500)                            NULL COMMENT '現場拍照/簽收證明照片 URL',
    pod_status              VARCHAR(20)                         NOT NULL DEFAULT 'SUCCESS' COMMENT 'SUCCESS成功簽收;AGENT_RECEIVED管理室/代收;EXCEPTIONAL異常簽收',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_tpod_waybill
        FOREIGN KEY (waybill_nid) REFERENCES tms_waybill(nid),
    CONSTRAINT uk_tpod_waybill UNIQUE (waybill_nid),
    INDEX idx_tpod_waybill_nid (waybill_nid),
    INDEX idx_tpod_signed_time (signed_time)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='貨物簽收證明 (Proof of Delivery)';

-- =========================================================
-- 05. 運費核算與費用結算 (Freight Cost Settlement)
-- =========================================================

CREATE TABLE IF NOT EXISTS tms_freight_settlement (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '運費結算單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    settlement_no           VARCHAR(100)                        NOT NULL COMMENT '結算單號',
    carrier_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '承運商流水號',
    waybill_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '託運單流水號',
    base_freight_amount     DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '基本運費',
    fuel_surcharge          DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '燃油附加費',
    special_service_fee     DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '特殊服務費 (例: 上樓費、冷藏費)',
    total_amount            DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '運費總金額',
    currency_code           VARCHAR(10)                         NOT NULL DEFAULT 'TWD' COMMENT '幣別',
    settlement_status       VARCHAR(30)                         NOT NULL DEFAULT 'UNSETTLED' COMMENT 'UNSETTLED未結算;AUDITED已審核;PAID已付款;CANCELLED取消',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_tfs_carrier
        FOREIGN KEY (carrier_nid) REFERENCES tms_carrier(nid),
    CONSTRAINT fk_tfs_waybill
        FOREIGN KEY (waybill_nid) REFERENCES tms_waybill(nid),
    CONSTRAINT uk_tfs_settlement_no UNIQUE (settlement_no),
    INDEX idx_tfs_carrier_nid (carrier_nid),
    INDEX idx_tfs_waybill_nid (waybill_nid),
    INDEX idx_tfs_status (settlement_status),
    INDEX idx_tfs_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='運費結算明細檔';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}