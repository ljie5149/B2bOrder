namespace B2bOrder.Resources.ERP.eCommerce
{
    /// <summary>
    /// WMSDB V1 Shared Core Schema
    /// 設計目標：
    /// 1. 支援多倉庫 (Warehouse)、多分區 (Zone)、儲位 (Location) 階層式架構與儲位容積限制
    /// 2. 支援批號 (Batch/Lot) 與序列號 (Serial Number) 精準追蹤
    /// 3. 管理進貨 (Inbound/GRN)、出貨揀貨 (Outbound/Picking)、移庫調撥 (Transfer) 與盤點 (Stocktake)
    /// 4. 提供即時庫存 (Stock) 與異動帳 (Inventory Ledger) 紀錄，確保帳實相符
    /// 5. 整合與串接 FulfillmentDB (履約需求) 及 InventoryDB (總庫存)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class WMSDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 倉庫與儲位基礎架構
-- =========================================================

CREATE TABLE IF NOT EXISTS wms_warehouse (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '倉庫序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    warehouse_code          VARCHAR(100)                        NOT NULL COMMENT '倉庫代碼',
    warehouse_name          VARCHAR(200)                        NOT NULL COMMENT '倉庫名稱',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    warehouse_type          VARCHAR(30)                         NOT NULL DEFAULT 'PHYSICAL' COMMENT 'PHYSICAL實體倉;VIRTUAL虛擬倉;BONDED保稅倉;THIRD_PARTY第三方倉;SITE_STORE工地臨時倉',
    address_sid             VARCHAR(32)                             NULL COMMENT '地址序號',
    manager_user_sid        VARCHAR(32)                             NULL COMMENT '倉管負責人序號',
    is_temperature_controlled TINYINT(1)                       NOT NULL DEFAULT 0 COMMENT '是否溫控',
    warehouse_status        VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ww_warehouse_code UNIQUE (company_sid, warehouse_code),
    INDEX idx_ww_company_sid (company_sid),
    INDEX idx_ww_type (warehouse_type),
    INDEX idx_ww_status (warehouse_status),
    INDEX idx_ww_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='倉庫主檔';

CREATE TABLE IF NOT EXISTS wms_location (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '儲位序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    warehouse_nid           BIGINT UNSIGNED                     NOT NULL COMMENT '倉庫流水號',
    location_code           VARCHAR(100)                        NOT NULL COMMENT '儲位代碼 (例: A-01-02-01)',
    zone_code               VARCHAR(50)                         NOT NULL COMMENT '分區代碼 (例: A區、冷凍區)',
    aisle_code              VARCHAR(20)                             NULL COMMENT '巷道號',
    rack_code               VARCHAR(20)                             NULL COMMENT '貨架號',
    level_code              VARCHAR(20)                             NULL COMMENT '層號',
    position_code           VARCHAR(20)                             NULL COMMENT '位號',
    location_type           VARCHAR(30)                         NOT NULL DEFAULT 'STORAGE' COMMENT 'STORAGE保管位;RECEIVING暫存進貨位;PICKING揀貨位;STAGING出貨暫存位;DAMAGE不良品位;TRANSIT中轉位',
    max_weight              DECIMAL(12,4)                           NULL COMMENT '最大載重(kg)',
    max_volume              DECIMAL(12,4)                           NULL COMMENT '最大容積(m³)',
    is_locked               TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否凍結/鎖定',
    location_status         VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_wl_warehouse
        FOREIGN KEY (warehouse_nid) REFERENCES wms_warehouse(nid),
    CONSTRAINT uk_wl_loc_code UNIQUE (warehouse_nid, location_code),
    INDEX idx_wl_warehouse_nid (warehouse_nid),
    INDEX idx_wl_zone_code (zone_code),
    INDEX idx_wl_location_type (location_type),
    INDEX idx_wl_status (location_status),
    INDEX idx_wl_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='倉庫儲位檔';

-- =========================================================
-- 02. 庫存現有帳與批號序列號
-- =========================================================

CREATE TABLE IF NOT EXISTS wms_stock (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '庫存記錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    warehouse_nid           BIGINT UNSIGNED                     NOT NULL COMMENT '倉庫流水號',
    location_nid            BIGINT UNSIGNED                     NOT NULL COMMENT '儲位流水號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'PIM/MIMDB Item序號',
    variant_sid             VARCHAR(32)                             NULL COMMENT 'PIM/MIMDB 變體序號',
    lot_no                  VARCHAR(100)                        NOT NULL DEFAULT '' COMMENT '批號',
    on_hand_qty             DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '現有庫存量',
    allocated_qty          DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '已分配/鎖定數量',
    available_qty          DECIMAL(20,6) GENERATED ALWAYS AS (on_hand_qty - allocated_qty) STORED COMMENT '可用數量',
    manufacture_date        DATE                                    NULL COMMENT '製造日期',
    expiration_date         DATE                                    NULL COMMENT '有效期限',
    stock_status            VARCHAR(20)                         NOT NULL DEFAULT 'AVAILABLE' COMMENT 'AVAILABLE可用;QUARANTINE隔離檢驗;HOLD凍結/不良品',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    CONSTRAINT fk_ws_warehouse
        FOREIGN KEY (warehouse_nid) REFERENCES wms_warehouse(nid),
    CONSTRAINT fk_ws_location
        FOREIGN KEY (location_nid) REFERENCES wms_location(nid),
    CONSTRAINT uk_ws_stock_item UNIQUE (warehouse_nid, location_nid, item_sid, IFNULL(variant_sid, ''), lot_no, stock_status),
    INDEX idx_ws_warehouse_nid (warehouse_nid),
    INDEX idx_ws_location_nid (location_nid),
    INDEX idx_ws_item_variant (item_sid, variant_sid),
    INDEX idx_ws_lot_no (lot_no),
    INDEX idx_ws_expiration_date (expiration_date),
    INDEX idx_ws_status (stock_status),
    CHECK (on_hand_qty >= 0),
    CHECK (allocated_qty >= 0),
    CHECK (allocated_qty <= on_hand_qty)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='儲位庫存帳';

CREATE TABLE IF NOT EXISTS wms_stock_serial (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '序列號紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    stock_nid               BIGINT UNSIGNED                     NOT NULL COMMENT '庫存帳流水號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'Item序號',
    serial_no               VARCHAR(100)                        NOT NULL COMMENT '商品唯一序列號 (S/N)',
    serial_status           VARCHAR(20)                         NOT NULL DEFAULT 'IN_STOCK' COMMENT 'IN_STOCK在庫;PICKED已揀貨;SHIPPED已出庫;RETURNED已退回;SCRAPPED報廢',
    CONSTRAINT fk_wss_stock
        FOREIGN KEY (stock_nid) REFERENCES wms_stock(nid),
    CONSTRAINT uk_wss_item_serial UNIQUE (item_sid, serial_no),
    INDEX idx_wss_stock_nid (stock_nid),
    INDEX idx_wss_serial_no (serial_no),
    INDEX idx_wss_status (serial_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='單件商品序列號追蹤';

-- =========================================================
-- 03. 進貨管理 (Inbound / Goods Receipt)
-- =========================================================

CREATE TABLE IF NOT EXISTS wms_inbound_order (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '進貨單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    inbound_no              VARCHAR(100)                        NOT NULL COMMENT '進貨單號',
    external_asn_no         VARCHAR(100)                            NULL COMMENT '外部預先送貨通知號 (ASN)',
    po_sid                  VARCHAR(32)                             NULL COMMENT '採購單序號 (ProcurementDB)',
    source_type             VARCHAR(30)                         NOT NULL DEFAULT 'PURCHASE' COMMENT 'PURCHASE採購入庫;RETURN退貨入庫;TRANSFER調撥入庫;PROD_OUTPUT生產入庫;MANUAL人工',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    warehouse_nid           BIGINT UNSIGNED                     NOT NULL COMMENT '預計入庫倉庫流水號',
    supplier_party_sid      VARCHAR(32)                             NULL COMMENT '供應商Party序號',
    expected_arrival_date   DATETIME                                NULL COMMENT '預計到貨時間',
    actual_arrival_date     DATETIME                                NULL COMMENT '實際到貨時間',
    inbound_status          VARCHAR(30)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待收貨;RECEIVING收貨中;QC_INSPECTING驗收檢驗中;PUTAWAY_PROCESSING上架中;COMPLETED已完成;CANCELLED取消',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_wio_warehouse
        FOREIGN KEY (warehouse_nid) REFERENCES wms_warehouse(nid),
    CONSTRAINT uk_wio_inbound_no UNIQUE (inbound_no),
    INDEX idx_wio_po_sid (po_sid),
    INDEX idx_wio_warehouse_nid (warehouse_nid),
    INDEX idx_wio_supplier (supplier_party_sid),
    INDEX idx_wio_status (inbound_status),
    INDEX idx_wio_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='進貨入庫單';

CREATE TABLE IF NOT EXISTS wms_inbound_item (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '進貨明細序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    inbound_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '進貨單流水號',
    line_no                 INT                                 NOT NULL COMMENT '行號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'Item序號',
    variant_sid             VARCHAR(32)                             NULL COMMENT '變體序號',
    expected_qty            DECIMAL(20,6)                       NOT NULL COMMENT '預計入庫量',
    received_qty            DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '清點實收量',
    qc_pass_qty             DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT 'QC合格量',
    qc_fail_qty             DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT 'QC不良量',
    putaway_qty             DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '已上架量',
    unit_sid                VARCHAR(32)                         NOT NULL COMMENT '單位序號',
    lot_no                  VARCHAR(100)                            NULL COMMENT '進貨批號',
    manufacture_date        DATE                                    NULL COMMENT '製造日期',
    expiration_date         DATE                                    NULL COMMENT '有效期限',
    target_location_nid     BIGINT UNSIGNED                         NULL COMMENT '建議上架儲位流水號',
    item_status             VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;RECEIVED已收貨;PUTAWAY已上架;CANCELLED取消',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_wii_inbound
        FOREIGN KEY (inbound_nid) REFERENCES wms_inbound_order(nid),
    CONSTRAINT fk_wii_location
        FOREIGN KEY (target_location_nid) REFERENCES wms_location(nid),
    CONSTRAINT uk_wii_inbound_line UNIQUE (inbound_nid, line_no),
    INDEX idx_wii_inbound_nid (inbound_nid),
    INDEX idx_wii_item_variant (item_sid, variant_sid),
    INDEX idx_wii_lot_no (lot_no),
    INDEX idx_wii_status (item_status),
    CHECK (line_no > 0),
    CHECK (expected_qty > 0),
    CHECK (received_qty >= 0),
    CHECK (qc_pass_qty >= 0),
    CHECK (qc_fail_qty >= 0),
    CHECK (putaway_qty >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='進貨入庫明細';

-- =========================================================
-- 04. 出貨與揀貨管理 (Outbound / Picking)
-- =========================================================

CREATE TABLE IF NOT EXISTS wms_outbound_order (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '出貨單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    outbound_no             VARCHAR(100)                        NOT NULL COMMENT '出貨單號',
    fulfillment_request_sid VARCHAR(32)                             NULL COMMENT 'FulfillmentDB 履約需求序號',
    sales_order_sid         VARCHAR(32)                             NULL COMMENT '銷售單序號 (SalOrderDB)',
    source_type             VARCHAR(30)                         NOT NULL DEFAULT 'SALES' COMMENT 'SALES銷售出貨;TRANSFER調撥出貨;RAW_MATERIAL領料出貨;RETURN退貨給廠商;MANUAL人工',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    warehouse_nid           BIGINT UNSIGNED                     NOT NULL COMMENT '出貨倉庫流水號',
    shipping_address_sid    VARCHAR(32)                             NULL COMMENT '配送地址序號',
    carrier_code            VARCHAR(50)                             NULL COMMENT '物流承運商代碼',
    tracking_no             VARCHAR(100)                            NULL COMMENT '物流包裹追蹤碼',
    outbound_status         VARCHAR(30)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;WAVE_ALLOCATED已分波次/預留;PICKING揀貨中;PACKING包裝中;STAGED已暫存待發;SHIPPED已出庫;CANCELLED取消',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_woo_warehouse
        FOREIGN KEY (warehouse_nid) REFERENCES wms_warehouse(nid),
    CONSTRAINT uk_woo_outbound_no UNIQUE (outbound_no),
    INDEX idx_woo_fulfillment_sid (fulfillment_request_sid),
    INDEX idx_woo_sales_order_sid (sales_order_sid),
    INDEX idx_woo_warehouse_nid (warehouse_nid),
    INDEX idx_woo_status (outbound_status),
    INDEX idx_woo_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='出貨單檔';

CREATE TABLE IF NOT EXISTS wms_outbound_item (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '出貨明細序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    outbound_nid            BIGINT UNSIGNED                     NOT NULL COMMENT '出貨單流水號',
    line_no                 INT                                 NOT NULL COMMENT '行號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'Item序號',
    variant_sid             VARCHAR(32)                             NULL COMMENT '變體序號',
    plan_qty                DECIMAL(20,6)                       NOT NULL COMMENT '應出數量',
    picked_qty              DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '已揀貨數量',
    shipped_qty             DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '實際出庫數量',
    unit_sid                VARCHAR(32)                         NOT NULL COMMENT '單位序號',
    lot_no                  VARCHAR(100)                            NULL COMMENT '指定或揀選批號',
    from_location_nid       BIGINT UNSIGNED                         NULL COMMENT '建議揀貨儲位流水號',
    item_status             VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;PICKED已揀貨;SHIPPED已出庫;SHORTAGE缺貨;CANCELLED取消',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_woi_outbound
        FOREIGN KEY (outbound_nid) REFERENCES wms_outbound_order(nid),
    CONSTRAINT fk_woi_location
        FOREIGN KEY (from_location_nid) REFERENCES wms_location(nid),
    CONSTRAINT uk_woi_outbound_line UNIQUE (outbound_nid, line_no),
    INDEX idx_woi_outbound_nid (outbound_nid),
    INDEX idx_woi_item_variant (item_sid, variant_sid),
    INDEX idx_woi_status (item_status),
    CHECK (line_no > 0),
    CHECK (plan_qty > 0),
    CHECK (picked_qty >= 0),
    CHECK (shipped_qty >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='出貨單明細';

-- =========================================================
-- 05. 庫內異動與盤點 (Transfer & Stocktake)
-- =========================================================

CREATE TABLE IF NOT EXISTS wms_transfer_order (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '移庫單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    transfer_no             VARCHAR(100)                        NOT NULL COMMENT '移庫/調撥單號',
    transfer_type           VARCHAR(30)                         NOT NULL DEFAULT 'LOCATION_MOVE' COMMENT 'LOCATION_MOVE館內移儲位;WAREHOUSE_MOVE跨倉調撥;STATUS_CHANGE庫存狀態變更',
    from_warehouse_nid      BIGINT UNSIGNED                     NOT NULL COMMENT '來源倉庫流水號',
    to_warehouse_nid        BIGINT UNSIGNED                     NOT NULL COMMENT '目的倉庫流水號',
    transfer_status         VARCHAR(30)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;APPROVED已核准;IN_TRANSIT調撥中;COMPLETED完成;CANCELLED取消',
    requested_user_sid      VARCHAR(32)                             NULL COMMENT '申請人帳號序號',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_wto_from_wh
        FOREIGN KEY (from_warehouse_nid) REFERENCES wms_warehouse(nid),
    CONSTRAINT fk_wto_to_wh
        FOREIGN KEY (to_warehouse_nid) REFERENCES wms_warehouse(nid),
    CONSTRAINT uk_wto_transfer_no UNIQUE (transfer_no),
    INDEX idx_wto_from_wh (from_warehouse_nid),
    INDEX idx_wto_to_wh (to_warehouse_nid),
    INDEX idx_wto_status (transfer_status),
    INDEX idx_wto_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='庫內移庫/跨倉調撥單';

CREATE TABLE IF NOT EXISTS wms_stocktake (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '盤點單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    stocktake_no            VARCHAR(100)                        NOT NULL COMMENT '盤點單號',
    warehouse_nid           BIGINT UNSIGNED                     NOT NULL COMMENT '盤點倉庫流水號',
    stocktake_type          VARCHAR(30)                         NOT NULL DEFAULT 'CYCLE_COUNT' COMMENT 'CYCLE_COUNT循環盤點;FULL_COUNT全面盤點;SPOT_CHECK抽盤',
    stocktake_status        VARCHAR(30)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;IN_PROGRESS盤點中;COUNTED已初盤;ADJUSTING盈虧調整中;CLOSED結案;CANCELLED取消',
    planned_date            DATE                                NOT NULL COMMENT '計畫盤點日期',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_wst_warehouse
        FOREIGN KEY (warehouse_nid) REFERENCES wms_warehouse(nid),
    CONSTRAINT uk_wst_stocktake_no UNIQUE (stocktake_no),
    INDEX idx_wst_warehouse_nid (warehouse_nid),
    INDEX idx_wst_status (stocktake_status),
    INDEX idx_wst_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='庫存盤點單';

-- =========================================================
-- 06. 庫存履歷與異動流水帳 (Inventory Ledger)
-- =========================================================

CREATE TABLE IF NOT EXISTS wms_inventory_ledger (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '異動流水序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '異動時間',
    warehouse_nid           BIGINT UNSIGNED                     NOT NULL COMMENT '倉庫流水號',
    location_nid            BIGINT UNSIGNED                     NOT NULL COMMENT '儲位流水號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'Item序號',
    variant_sid             VARCHAR(32)                             NULL COMMENT '變體序號',
    lot_no                  VARCHAR(100)                        NOT NULL DEFAULT '' COMMENT '批號',
    transaction_type        VARCHAR(30)                         NOT NULL COMMENT 'INBOUND入庫;OUTBOUND出庫;TRANSFER_IN調入;TRANSFER_OUT調出;ADJUSTMENT_GAIN盤盈;ADJUSTMENT_LOSS盤虧',
    qty_change              DECIMAL(20,6)                       NOT NULL COMMENT '變動數量 (正增加/負減少)',
    qty_after               DECIMAL(20,6)                       NOT NULL COMMENT '異動後結存數量',
    reference_doc_type      VARCHAR(50)                             NULL COMMENT '關聯單據類型 (例: INBOUND, OUTBOUND, TRANSFER, STOCKTAKE)',
    reference_doc_no        VARCHAR(100)                            NULL COMMENT '關聯單據編號',
    operator_user_sid       VARCHAR(32)                             NULL COMMENT '操作人員帳號序號',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_wil_warehouse
        FOREIGN KEY (warehouse_nid) REFERENCES wms_warehouse(nid),
    CONSTRAINT fk_wil_location
        FOREIGN KEY (location_nid) REFERENCES wms_location(nid),
    INDEX idx_wil_warehouse_loc (warehouse_nid, location_nid),
    INDEX idx_wil_item_variant (item_sid, variant_sid),
    INDEX idx_wil_tx_type (transaction_type),
    INDEX idx_wil_ref_doc (reference_doc_type, reference_doc_no),
    INDEX idx_wil_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='庫存異動流水帳 (不可修改，僅供稽核追蹤)';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}