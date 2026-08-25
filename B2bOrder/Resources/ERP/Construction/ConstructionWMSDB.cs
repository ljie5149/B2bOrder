namespace B2bOrder.Resources.ERP.Construction
{
    /// <summary>
    /// ConstructionWMSDB V1 Shared Core Schema (工地現場倉儲與物流調撥管理資料庫)
    /// 設計目標：
    /// 1. 工地現場儲位與區域規劃 (Site Yard & Zone Master)：管理工地區域 (卸貨區/鋼筋區/塔吊區)、儲位載重限制與容積
    /// 2. 進場交貨與卸貨時段預約 (Delivery Slot Appointment)：管理大貨車/預拌車進場時間軸排程，避免工地周邊塞車
    /// 3. 跨工地調撥與物流追蹤 (Site Transfer Order & Tracking)：管理建案間/總庫與工地間材料與設備調撥單及簽收歷程
    /// 4. 車輛過磅與進場簽收紀錄 (Weighbridge & Delivery Ticket)：紀錄進場車輛車牌、過磅單 (毛重/淨重) 與現場驗收品質
    /// 5. 整合串接 MIMDB (材料庫存)、ProcurementDB (採購單 PO)、ProjectDB (建案) 與 VendorDB (載運廠商)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class ConstructionWMSDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 工地區域與現場儲位主檔 (Site Yard & Storage Zone Master)
-- =========================================================

CREATE TABLE IF NOT EXISTS wms_site_zone_master (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '儲位區域序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    
    zone_code               VARCHAR(50)                         NOT NULL COMMENT '區域代碼 (例: ZONE-A, YARD-REBAR, B1F-STORE)',
    zone_name               VARCHAR(100)                        NOT NULL COMMENT '區域名稱 (例: A區塔吊直吊卸貨區, B1F電器資材庫房)',
    zone_type               VARCHAR(30)                         NOT NULL DEFAULT 'YARD' COMMENT '類別: UNLOADING_BAY卸貨區, YARD地面堆置場, INDOOR_STORE室內庫房, HAZMAT危險品庫, CRANE_ZONE塔吊覆蓋區',
    
    floor_level             VARCHAR(20)                         NOT NULL DEFAULT '1F' COMMENT '樓層層別 (例: 1F, B1F, 12F)',
    max_weight_capacity_kg  DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '樓板/區域最大耐重限制 (kg/m2)',
    is_crane_accessible     VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT '塔吊/移動式吊車是否可涵蓋到達 (Y/N)',
    
    zone_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT '狀態: ACTIVE可用, FULL已滿載, LOCKED封鎖維修中',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '區域安全注意事項與規範',
    CONSTRAINT uk_wszm_project_zone UNIQUE (project_sid, zone_code),
    INDEX idx_wszm_company_sid (company_sid),
    INDEX idx_wszm_project_sid (project_sid),
    INDEX idx_wszm_type (zone_type),
    INDEX idx_wszm_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工地區域與現場儲位主檔';

-- =========================================================
-- 02. 進場交貨與卸貨時段預約 (Delivery Slot Appointment)
-- =========================================================

CREATE TABLE IF NOT EXISTS wms_delivery_appointment (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '預約單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    zone_sid                VARCHAR(32)                         NOT NULL COMMENT '預定卸貨區域 (對應 wms_site_zone_master.sid)',
    vendor_sid              VARCHAR(32)                         NOT NULL COMMENT '交貨廠商/運輸公司序號 (VendorDB SID)',
    
    appointment_no          VARCHAR(50)                         NOT NULL COMMENT '預約單號 (例: APT-202608-088)',
    expected_arrival_time   DATETIME                            NOT NULL COMMENT '預計抵達時間 (時段 Slot)',
    estimated_duration_mins INT                                 NOT NULL DEFAULT 60 COMMENT '預計占用卸貨時間 (分鐘)',
    
    truck_plate_number      VARCHAR(50)                         NOT NULL COMMENT '進場車輛車牌號碼 (例: KE-8899)',
    driver_name             VARCHAR(100)                            NULL COMMENT '駕駛姓名',
    driver_phone            VARCHAR(50)                             NULL COMMENT '駕駛聯絡電話',
    cargo_description       VARCHAR(200)                        NOT NULL COMMENT '載運物料簡述 (例: 4000PSI 預拌混凝土車 3 車, #5鋼筋 20噸)',
    
    is_heavy_lifting_needed VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT '是否需要申請塔吊/吊車支援卸貨 (Y/N)',
    appointment_status      VARCHAR(20)                         NOT NULL DEFAULT 'BOOKED' COMMENT 'STATUS: BOOKED已預約;CHECKED_IN已到場簽到;UNLOADING卸貨中;COMPLETED已離場;CANCELLED取消',
    actual_arrival_time     DATETIME                                NULL COMMENT '實際進場簽到時間',
    actual_departure_time   DATETIME                                NULL COMMENT '實際離場時間',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '進場注意事項',
    CONSTRAINT uk_wda_appointment_no UNIQUE (appointment_no),
    INDEX idx_wda_project_sid (project_sid),
    INDEX idx_wda_zone_sid (zone_sid),
    INDEX idx_wda_arrival (expected_arrival_time),
    INDEX idx_wda_status (appointment_status),
    INDEX idx_wda_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='進場交貨與卸貨時段預約檔';

-- =========================================================
-- 03. 車輛過磅與現場簽收點收單 (Weighbridge & Delivery Ticket)
-- =========================================================

CREATE TABLE IF NOT EXISTS wms_weighbridge_ticket (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '過磅單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '開單時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    appointment_sid         VARCHAR(32)                             NULL COMMENT '對應預約單 wms_delivery_appointment.sid',
    ref_po_sid              VARCHAR(32)                             NULL COMMENT '採購單序號 (ProcurementDB PO SID)',
    
    ticket_no               VARCHAR(50)                         NOT NULL COMMENT '地磅單/點收單號 (例: WBT-202608-019)',
    truck_plate_number      VARCHAR(50)                         NOT NULL COMMENT '車牌號碼',
    material_sid            VARCHAR(32)                         NOT NULL COMMENT '對應 MIMDB mim_material_master.sid',
    
    gross_weight_kg         DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '毛重 (包含車重, kg)',
    tare_weight_kg          DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '空車皮重 (kg)',
    net_weight_kg           DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '淨重/實重 (kg = 毛重 - 皮重)',
    
    inspector_user_sid      VARCHAR(32)                         NOT NULL COMMENT '現場驗收工程師帳號序號',
    inspection_result       VARCHAR(20)                         NOT NULL DEFAULT 'PASSED' COMMENT '品質抽檢結果: PASSED合格, REJECTED退貨, CONDITIONALLY_ACCEPTED條件接收',
    weighbridge_photo_sid   VARCHAR(32)                             NULL COMMENT '地磅照片/簽單電子檔 (DMS)',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '扣重原因與備註',
    CONSTRAINT uk_wwt_ticket_no UNIQUE (ticket_no),
    INDEX idx_wwt_project_sid (project_sid),
    INDEX idx_wwt_appointment (appointment_sid),
    INDEX idx_wwt_po (ref_po_sid),
    INDEX idx_wwt_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='車輛過磅與現場簽收點收單';

-- =========================================================
-- 04. 跨工地材料與設備調撥單 (Site-to-Site Transfer Order)
-- =========================================================

CREATE TABLE IF NOT EXISTS wms_transfer_order (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '調撥單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '開單時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    
    transfer_no             VARCHAR(50)                         NOT NULL COMMENT '調撥單號 (例: TRF-202608-005)',
    from_project_sid        VARCHAR(32)                         NOT NULL COMMENT '調出建案/總庫序號',
    to_project_sid          VARCHAR(32)                         NOT NULL COMMENT '調入建案/工地序號',
    
    carrier_vendor_sid      VARCHAR(32)                             NULL COMMENT '承運托運廠商序號 (VendorDB SID)',
    dispatch_date           DATE                                NOT NULL COMMENT '預計起運日期',
    actual_received_date    DATETIME                                NULL COMMENT '實際調入簽收時間',
    
    transfer_status         VARCHAR(20)                         NOT NULL DEFAULT 'REQUESTED' COMMENT 'STATUS: REQUESTED申請中;APPROVED核准;IN_TRANSIT運送中;RECEIVED已簽收完成;REJECTED駁回',
    requested_by_user_sid   VARCHAR(32)                         NOT NULL COMMENT '調撥申請工程師序號',
    received_by_user_sid    VARCHAR(32)                             NULL COMMENT '調入點收工程師序號',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '調撥原因與說明',
    CONSTRAINT uk_wto_transfer_no UNIQUE (transfer_no),
    INDEX idx_wto_company_sid (company_sid),
    INDEX idx_wto_from_project (from_project_sid),
    INDEX idx_wto_to_project (to_project_sid),
    INDEX idx_wto_status (transfer_status),
    INDEX idx_wto_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='跨工地材料與設備調撥主檔';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}