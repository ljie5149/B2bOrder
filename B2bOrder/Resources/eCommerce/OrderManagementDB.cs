namespace B2bOrder.Resources.eCommerce
{
    /// <summary>
    /// OMSDB / Order Management System Core Schema
    /// 規範：
    /// 1. OMSDB 負責跨通路訂單接收、標準化、拆單、路由、狀態協調與例外處理
    /// 2. ShoppingDB、StoreDB、InventoryDB、FulfillmentDB、AccountingDB、IntegrationDB 跨資料庫參照使用 sid
    /// 3. OMS 不直接保存商品主資料與庫存主檔，只保存訂單快照與履約協調資料
    /// 4. 每個外部通路訂單必須具備冪等鍵，避免重複匯入
    /// 5. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class OrderManagementDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 銷售通路
-- =========================================================

CREATE TABLE IF NOT EXISTS oms_channel (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '銷售通路序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    channel_code            VARCHAR(80)                         NOT NULL COMMENT '通路代碼',
    channel_name            VARCHAR(150)                        NOT NULL COMMENT '通路名稱',
    channel_type            VARCHAR(30)                         NOT NULL COMMENT 'SHOPPING官網;APP;POS;MARKETPLACE外部平台;MANUAL人工;API',
    platform_name           VARCHAR(100)                            NULL COMMENT '平台名稱，例如Shopee、MOMO、Amazon',
    store_sid               VARCHAR(32)                             NULL COMMENT 'StoreDB預設商店序號',
    default_warehouse_sid   VARCHAR(32)                             NULL COMMENT 'MasterDB預設倉庫序號',
    default_currency_sid    VARCHAR(32)                             NULL COMMENT 'MasterDB預設幣別序號',
    timezone_code           VARCHAR(100)                        NOT NULL DEFAULT 'Asia/Taipei' COMMENT '通路時區',
    order_prefix            VARCHAR(20)                             NULL COMMENT 'OMS訂單編號前綴',
    auto_accept_order       TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否自動接單',
    auto_allocate_stock     TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否自動配貨',
    allow_partial_fulfillment TINYINT(1)                        NOT NULL DEFAULT 1 COMMENT '是否允許分批履約',
    channel_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;PAUSED暫停;DISABLED停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_oc_channel_code UNIQUE (channel_code),
    INDEX idx_oc_channel_type (channel_type),
    INDEX idx_oc_store_sid (store_sid),
    INDEX idx_oc_default_warehouse_sid (default_warehouse_sid),
    INDEX idx_oc_status (channel_status),
    INDEX idx_oc_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS銷售通路';

CREATE TABLE IF NOT EXISTS oms_channel_account (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '通路帳號序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    channel_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '銷售通路流水號',
    account_code            VARCHAR(100)                        NOT NULL COMMENT '通路帳號代碼',
    account_name            VARCHAR(200)                        NOT NULL COMMENT '通路帳號名稱',
    external_shop_id        VARCHAR(200)                            NULL COMMENT '外部平台店鋪ID',
    store_sid               VARCHAR(32)                             NULL COMMENT 'StoreDB對應商店序號',
    credential_encrypted    LONGTEXT                                NULL COMMENT '加密後連線憑證',
    config_json             JSON                                    NULL COMMENT '通路介接設定',
    last_sync_date          DATETIME                                NULL COMMENT '最後同步時間',
    account_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;EXPIRED憑證失效;PAUSED暫停;DISABLED停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_oca_channel
        FOREIGN KEY (channel_nid) REFERENCES oms_channel(nid),
    CONSTRAINT uk_oca_channel_account UNIQUE (channel_nid, account_code),
    INDEX idx_oca_channel_nid (channel_nid),
    INDEX idx_oca_external_shop_id (external_shop_id),
    INDEX idx_oca_store_sid (store_sid),
    INDEX idx_oca_status (account_status),
    INDEX idx_oca_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS通路帳號';

-- =========================================================
-- 02. 外部訂單接收
-- =========================================================

CREATE TABLE IF NOT EXISTS oms_order_import (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '訂單匯入序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    import_no               VARCHAR(100)                        NOT NULL COMMENT '匯入工作編號',
    channel_account_nid     BIGINT UNSIGNED                     NOT NULL COMMENT '通路帳號流水號',
    external_order_no       VARCHAR(150)                        NOT NULL COMMENT '外部訂單編號',
    external_order_version  VARCHAR(50)                             NULL COMMENT '外部訂單版本',
    idempotency_key         VARCHAR(200)                        NOT NULL COMMENT '冪等鍵',
    raw_payload             JSON                                NOT NULL COMMENT '原始訂單資料',
    normalized_payload      JSON                                    NULL COMMENT '標準化後訂單資料',
    import_status           VARCHAR(30)                         NOT NULL DEFAULT 'RECEIVED' COMMENT 'RECEIVED已接收;VALIDATING驗證中;IMPORTED已匯入;DUPLICATE重複;FAILED失敗;IGNORED忽略',
    validation_result       JSON                                    NULL COMMENT '驗證結果',
    oms_order_sid           VARCHAR(32)                             NULL COMMENT '產生的OMS訂單序號',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '重試次數',
    error_code              VARCHAR(100)                            NULL COMMENT '錯誤代碼',
    error_message           LONGTEXT                                NULL COMMENT '錯誤訊息',
    imported_date           DATETIME                                NULL COMMENT '匯入完成時間',
    CONSTRAINT fk_ooi_channel_account
        FOREIGN KEY (channel_account_nid) REFERENCES oms_channel_account(nid),
    CONSTRAINT uk_ooi_import_no UNIQUE (import_no),
    CONSTRAINT uk_ooi_idempotency UNIQUE (channel_account_nid, idempotency_key),
    CONSTRAINT uk_ooi_external_order UNIQUE (channel_account_nid, external_order_no, external_order_version),
    INDEX idx_ooi_channel_account_nid (channel_account_nid),
    INDEX idx_ooi_external_order_no (external_order_no),
    INDEX idx_ooi_status (import_status),
    INDEX idx_ooi_oms_order_sid (oms_order_sid),
    INDEX idx_ooi_create_date (create_date),
    CHECK (retry_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='外部通路訂單匯入';

-- =========================================================
-- 03. OMS訂單主檔
-- =========================================================

CREATE TABLE IF NOT EXISTS oms_order (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'OMS訂單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    oms_order_no            VARCHAR(80)                         NOT NULL COMMENT 'OMS訂單編號',
    channel_sid             VARCHAR(32)                         NOT NULL COMMENT 'OMS通路序號',
    channel_account_sid     VARCHAR(32)                             NULL COMMENT 'OMS通路帳號序號',
    external_order_no       VARCHAR(150)                            NULL COMMENT '外部訂單編號',
    shopping_order_sid      VARCHAR(32)                             NULL COMMENT 'ShoppingDB訂單序號',
    member_sid              VARCHAR(32)                             NULL COMMENT 'ShoppingDB會員序號',
    customer_no_snapshot    VARCHAR(100)                            NULL COMMENT '客戶編號快照',
    customer_name_snapshot  VARCHAR(200)                            NULL COMMENT '客戶名稱快照',
    customer_email_snapshot VARCHAR(200)                            NULL COMMENT '客戶Email快照',
    customer_mobile_snapshot VARCHAR(50)                            NULL COMMENT '客戶手機快照',
    order_date              DATETIME                            NOT NULL COMMENT '下單時間',
    currency_sid            VARCHAR(32)                         NOT NULL COMMENT 'MasterDB幣別序號',
    exchange_rate           DECIMAL(20,10)                      NOT NULL DEFAULT 1 COMMENT '匯率',
    subtotal_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '商品小計',
    discount_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '折扣金額',
    freight_amount          DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '運費',
    tax_amount              DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '稅額',
    total_amount            DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '訂單總額',
    paid_amount             DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '已付款金額',
    refunded_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '已退款金額',
    order_status            VARCHAR(30)                         NOT NULL DEFAULT 'NEW' COMMENT 'NEW新訂單;VALIDATED已驗證;ACCEPTED已接單;ROUTING路由中;ALLOCATED已配貨;FULFILLING履約中;COMPLETED完成;CANCELLED取消;EXCEPTION異常',
    payment_status          VARCHAR(30)                         NOT NULL DEFAULT 'UNPAID' COMMENT 'UNPAID未付款;PARTIAL部分付款;PAID已付款;REFUNDED已退款',
    allocation_status       VARCHAR(30)                         NOT NULL DEFAULT 'UNALLOCATED' COMMENT 'UNALLOCATED未配貨;PARTIAL部分配貨;ALLOCATED已配貨;FAILED失敗',
    fulfillment_status      VARCHAR(30)                         NOT NULL DEFAULT 'UNFULFILLED' COMMENT 'UNFULFILLED未履約;PARTIAL部分履約;FULFILLED已履約;RETURNED已退回',
    risk_status             VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待檢查;PASS通過;REVIEW人工審查;BLOCKED攔截',
    priority                VARCHAR(20)                         NOT NULL DEFAULT 'NORMAL' COMMENT 'LOW低;NORMAL一般;HIGH高;URGENT緊急',
    promised_ship_date      DATETIME                                NULL COMMENT '承諾出貨時間',
    promised_delivery_date  DATETIME                                NULL COMMENT '承諾送達時間',
    accepted_date           DATETIME                                NULL COMMENT '接單時間',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    cancelled_date          DATETIME                                NULL COMMENT '取消時間',
    cancel_reason_code      VARCHAR(50)                             NULL COMMENT '取消原因代碼',
    correlation_id          VARCHAR(100)                            NULL COMMENT '跨服務關聯識別碼',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_oo_oms_order_no UNIQUE (oms_order_no),
    UNIQUE KEY uk_oo_channel_external_order (channel_account_sid, external_order_no),
    INDEX idx_oo_channel_sid (channel_sid),
    INDEX idx_oo_channel_account_sid (channel_account_sid),
    INDEX idx_oo_external_order_no (external_order_no),
    INDEX idx_oo_shopping_order_sid (shopping_order_sid),
    INDEX idx_oo_member_sid (member_sid),
    INDEX idx_oo_order_date (order_date),
    INDEX idx_oo_order_status (order_status),
    INDEX idx_oo_payment_status (payment_status),
    INDEX idx_oo_allocation_status (allocation_status),
    INDEX idx_oo_fulfillment_status (fulfillment_status),
    INDEX idx_oo_risk_status (risk_status),
    INDEX idx_oo_promised_ship_date (promised_ship_date),
    INDEX idx_oo_correlation_id (correlation_id),
    INDEX idx_oo_avalible (avalible),
    CHECK (exchange_rate > 0),
    CHECK (subtotal_amount >= 0),
    CHECK (discount_amount >= 0),
    CHECK (freight_amount >= 0),
    CHECK (tax_amount >= 0),
    CHECK (total_amount >= 0),
    CHECK (paid_amount >= 0),
    CHECK (refunded_amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS統一訂單';

CREATE TABLE IF NOT EXISTS oms_order_item (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'OMS訂單明細序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    oms_order_nid           BIGINT UNSIGNED                     NOT NULL COMMENT 'OMS訂單流水號',
    line_no                 INT                                 NOT NULL COMMENT '明細行號',
    external_line_no        VARCHAR(100)                            NULL COMMENT '外部訂單明細編號',
    shopping_order_item_sid VARCHAR(32)                             NULL COMMENT 'ShoppingDB訂單明細序號',
    store_sid               VARCHAR(32)                             NULL COMMENT 'StoreDB商店序號',
    product_sid             VARCHAR(32)                         NOT NULL COMMENT 'CatalogDB商品序號',
    sku_sid                 VARCHAR(32)                         NOT NULL COMMENT 'CatalogDB SKU序號',
    product_no_snapshot     VARCHAR(100)                        NOT NULL COMMENT '商品編號快照',
    product_name_snapshot   VARCHAR(500)                        NOT NULL COMMENT '商品名稱快照',
    sku_no_snapshot         VARCHAR(100)                        NOT NULL COMMENT 'SKU編號快照',
    spec_snapshot           JSON                                    NULL COMMENT '規格快照',
    quantity                DECIMAL(20,6)                       NOT NULL COMMENT '訂購數量',
    cancelled_qty           DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '取消數量',
    allocated_qty           DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '已配貨數量',
    fulfilled_qty           DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '已履約數量',
    returned_qty            DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '退貨數量',
    unit_price              DECIMAL(20,4)                       NOT NULL COMMENT '成交單價',
    original_price          DECIMAL(20,4)                       NOT NULL COMMENT '原始單價',
    discount_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '折扣金額',
    tax_amount              DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '稅額',
    line_amount             DECIMAL(20,4)                       NOT NULL COMMENT '明細總額',
    item_status             VARCHAR(30)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;BACKORDER缺貨待補;ALLOCATED已配貨;FULFILLED已履約;CANCELLED取消;RETURNED退貨;EXCEPTION異常',
    substitution_allowed    TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否允許替代品',
    gift_mark               TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否贈品',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_ooi_order
        FOREIGN KEY (oms_order_nid) REFERENCES oms_order(nid),
    CONSTRAINT uk_ooi_order_line UNIQUE (oms_order_nid, line_no),
    INDEX idx_ooi_order_nid (oms_order_nid),
    INDEX idx_ooi_external_line_no (external_line_no),
    INDEX idx_ooi_shopping_order_item_sid (shopping_order_item_sid),
    INDEX idx_ooi_store_sid (store_sid),
    INDEX idx_ooi_product_sid (product_sid),
    INDEX idx_ooi_sku_sid (sku_sid),
    INDEX idx_ooi_status (item_status),
    CHECK (line_no > 0),
    CHECK (quantity > 0),
    CHECK (cancelled_qty >= 0),
    CHECK (allocated_qty >= 0),
    CHECK (fulfilled_qty >= 0),
    CHECK (returned_qty >= 0),
    CHECK (cancelled_qty <= quantity),
    CHECK (allocated_qty <= quantity),
    CHECK (fulfilled_qty <= quantity),
    CHECK (returned_qty <= quantity),
    CHECK (unit_price >= 0),
    CHECK (original_price >= 0),
    CHECK (discount_amount >= 0),
    CHECK (tax_amount >= 0),
    CHECK (line_amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS訂單明細';

CREATE TABLE IF NOT EXISTS oms_order_address (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'OMS訂單地址序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    oms_order_nid           BIGINT UNSIGNED                     NOT NULL COMMENT 'OMS訂單流水號',
    address_type            VARCHAR(20)                         NOT NULL COMMENT 'SHIPPING收件;BILLING帳單',
    receiver_name           VARCHAR(200)                        NOT NULL COMMENT '收件人',
    receiver_mobile         VARCHAR(50)                             NULL COMMENT '手機',
    receiver_phone          VARCHAR(50)                             NULL COMMENT '電話',
    country_code            VARCHAR(20)                             NULL COMMENT '國家代碼',
    country_name            VARCHAR(100)                            NULL COMMENT '國家名稱',
    city_name               VARCHAR(100)                            NULL COMMENT '縣市名稱',
    district_name           VARCHAR(100)                            NULL COMMENT '行政區名稱',
    postal_code             VARCHAR(20)                             NULL COMMENT '郵遞區號',
    address_line1           VARCHAR(1000)                       NOT NULL COMMENT '主要地址',
    address_line2           VARCHAR(1000)                           NULL COMMENT '補充地址',
    latitude                DECIMAL(12,8)                           NULL COMMENT '緯度',
    longitude               DECIMAL(12,8)                           NULL COMMENT '經度',
    CONSTRAINT fk_ooa_order
        FOREIGN KEY (oms_order_nid) REFERENCES oms_order(nid),
    CONSTRAINT uk_ooa_order_type UNIQUE (oms_order_nid, address_type),
    INDEX idx_ooa_order_nid (oms_order_nid),
    INDEX idx_ooa_postal_code (postal_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS訂單地址快照';

-- =========================================================
-- 04. 拆單與履約訂單
-- =========================================================

CREATE TABLE IF NOT EXISTS oms_fulfillment_order (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '履約訂單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    fulfillment_order_no    VARCHAR(100)                        NOT NULL COMMENT '履約訂單編號',
    oms_order_nid           BIGINT UNSIGNED                     NOT NULL COMMENT 'OMS訂單流水號',
    split_group_no          INT                                 NOT NULL COMMENT '拆單群組序號',
    store_sid               VARCHAR(32)                             NULL COMMENT 'StoreDB商店序號',
    warehouse_sid           VARCHAR(32)                             NULL COMMENT 'MasterDB履約倉庫序號',
    fulfillment_type        VARCHAR(30)                         NOT NULL DEFAULT 'SHIPMENT' COMMENT 'SHIPMENT宅配;PICKUP自取;DIGITAL數位;DROPSHIP供應商直送',
    fulfillment_status      VARCHAR(30)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;ROUTED已路由;ALLOCATING配貨中;ALLOCATED已配貨;PICKING揀貨中;PACKED已包裝;SHIPPED已出貨;DELIVERED已送達;COMPLETED完成;CANCELLED取消;EXCEPTION異常',
    inventory_reservation_sid VARCHAR(32)                           NULL COMMENT 'InventoryDB庫存預留序號',
    store_order_sid         VARCHAR(32)                             NULL COMMENT 'StoreDB子訂單序號',
    fulfillment_sid         VARCHAR(32)                             NULL COMMENT 'FulfillmentDB出貨或履約序號',
    promised_ship_date      DATETIME                                NULL COMMENT '承諾出貨時間',
    promised_delivery_date  DATETIME                                NULL COMMENT '承諾送達時間',
    routed_date             DATETIME                                NULL COMMENT '完成路由時間',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_ofo_order
        FOREIGN KEY (oms_order_nid) REFERENCES oms_order(nid),
    CONSTRAINT uk_ofo_number UNIQUE (fulfillment_order_no),
    CONSTRAINT uk_ofo_order_split UNIQUE (oms_order_nid, split_group_no),
    INDEX idx_ofo_order_nid (oms_order_nid),
    INDEX idx_ofo_store_sid (store_sid),
    INDEX idx_ofo_warehouse_sid (warehouse_sid),
    INDEX idx_ofo_status (fulfillment_status),
    INDEX idx_ofo_inventory_reservation_sid (inventory_reservation_sid),
    INDEX idx_ofo_store_order_sid (store_order_sid),
    INDEX idx_ofo_fulfillment_sid (fulfillment_sid),
    INDEX idx_ofo_promised_ship_date (promised_ship_date),
    INDEX idx_ofo_avalible (avalible),
    CHECK (split_group_no > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS履約子訂單';

CREATE TABLE IF NOT EXISTS oms_fulfillment_order_item (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '履約訂單明細序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    fulfillment_order_nid   BIGINT UNSIGNED                     NOT NULL COMMENT '履約訂單流水號',
    oms_order_item_nid      BIGINT UNSIGNED                     NOT NULL COMMENT 'OMS訂單明細流水號',
    quantity                DECIMAL(20,6)                       NOT NULL COMMENT '履約數量',
    allocated_qty           DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '已配貨數量',
    fulfilled_qty           DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '已履約數量',
    cancelled_qty           DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '取消數量',
    reservation_item_sid    VARCHAR(32)                             NULL COMMENT 'InventoryDB預留明細序號',
    item_status             VARCHAR(30)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;ALLOCATED已配貨;FULFILLED已履約;BACKORDER缺貨待補;CANCELLED取消;EXCEPTION異常',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_ofoi_fulfillment_order
        FOREIGN KEY (fulfillment_order_nid) REFERENCES oms_fulfillment_order(nid),
    CONSTRAINT fk_ofoi_order_item
        FOREIGN KEY (oms_order_item_nid) REFERENCES oms_order_item(nid),
    CONSTRAINT uk_ofoi_order_item UNIQUE (fulfillment_order_nid, oms_order_item_nid),
    INDEX idx_ofoi_fulfillment_order_nid (fulfillment_order_nid),
    INDEX idx_ofoi_order_item_nid (oms_order_item_nid),
    INDEX idx_ofoi_reservation_item_sid (reservation_item_sid),
    INDEX idx_ofoi_status (item_status),
    CHECK (quantity > 0),
    CHECK (allocated_qty >= 0),
    CHECK (fulfilled_qty >= 0),
    CHECK (cancelled_qty >= 0),
    CHECK (allocated_qty <= quantity),
    CHECK (fulfilled_qty <= quantity),
    CHECK (cancelled_qty <= quantity)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS履約子訂單明細';

-- =========================================================
-- 05. 訂單路由規則
-- =========================================================

CREATE TABLE IF NOT EXISTS oms_routing_rule (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '訂單路由規則序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    rule_code               VARCHAR(100)                        NOT NULL COMMENT '規則代碼',
    rule_name               VARCHAR(200)                        NOT NULL COMMENT '規則名稱',
    channel_sid             VARCHAR(32)                             NULL COMMENT '限定通路序號',
    store_sid               VARCHAR(32)                             NULL COMMENT '限定商店序號',
    product_sid             VARCHAR(32)                             NULL COMMENT '限定商品序號',
    sku_sid                 VARCHAR(32)                             NULL COMMENT '限定SKU序號',
    destination_country_code VARCHAR(20)                            NULL COMMENT '目的國家代碼',
    destination_region_code VARCHAR(50)                             NULL COMMENT '目的區域代碼',
    rule_condition          JSON                                    NULL COMMENT '路由條件',
    route_type              VARCHAR(30)                         NOT NULL COMMENT 'WAREHOUSE倉庫;STORE商店;SUPPLIER供應商直送;DIGITAL數位',
    target_sid              VARCHAR(32)                         NOT NULL COMMENT '路由目標序號',
    priority                INT                                 NOT NULL DEFAULT 0 COMMENT '規則優先順序',
    allocation_strategy     VARCHAR(30)                         NOT NULL DEFAULT 'AVAILABLE_STOCK' COMMENT 'AVAILABLE_STOCK可用庫存;NEAREST最近倉庫;LOWEST_COST最低成本;PRIORITY固定優先;LOAD_BALANCE負載平衡',
    allow_split             TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否允許拆單',
    start_date              DATETIME                                NULL COMMENT '生效開始時間',
    end_date                DATETIME                                NULL COMMENT '生效結束時間',
    rule_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;PAUSED暫停;DISABLED停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_orr_rule_code UNIQUE (rule_code),
    INDEX idx_orr_channel_sid (channel_sid),
    INDEX idx_orr_store_sid (store_sid),
    INDEX idx_orr_product_sid (product_sid),
    INDEX idx_orr_sku_sid (sku_sid),
    INDEX idx_orr_route_target (route_type, target_sid),
    INDEX idx_orr_priority (priority),
    INDEX idx_orr_effective_date (start_date, end_date),
    INDEX idx_orr_status (rule_status),
    INDEX idx_orr_avalible (avalible),
    CHECK (end_date IS NULL OR start_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS訂單路由規則';

CREATE TABLE IF NOT EXISTS oms_routing_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '路由紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    oms_order_sid           VARCHAR(32)                         NOT NULL COMMENT 'OMS訂單序號',
    oms_order_item_sid      VARCHAR(32)                             NULL COMMENT 'OMS訂單明細序號',
    routing_rule_sid        VARCHAR(32)                             NULL COMMENT '套用路由規則序號',
    selected_route_type     VARCHAR(30)                         NOT NULL COMMENT '選定路由類型',
    selected_target_sid     VARCHAR(32)                         NOT NULL COMMENT '選定目標序號',
    candidate_json          JSON                                    NULL COMMENT '候選目標與評分',
    routing_result          VARCHAR(20)                         NOT NULL COMMENT 'SUCCESS成功;FAILED失敗;MANUAL需人工',
    reason                  VARCHAR(1000)                           NULL COMMENT '選擇原因或失敗原因',
    correlation_id          VARCHAR(100)                            NULL COMMENT '關聯識別碼',
    INDEX idx_orl_order_sid (oms_order_sid),
    INDEX idx_orl_order_item_sid (oms_order_item_sid),
    INDEX idx_orl_rule_sid (routing_rule_sid),
    INDEX idx_orl_target (selected_route_type, selected_target_sid),
    INDEX idx_orl_result (routing_result),
    INDEX idx_orl_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS訂單路由紀錄';

-- =========================================================
-- 06. 狀態歷程與狀態同步
-- =========================================================

CREATE TABLE IF NOT EXISTS oms_order_status_history (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '訂單狀態歷程序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '異動時間',
    oms_order_nid           BIGINT UNSIGNED                     NOT NULL COMMENT 'OMS訂單流水號',
    status_type             VARCHAR(30)                         NOT NULL COMMENT 'ORDER訂單;PAYMENT付款;ALLOCATION配貨;FULFILLMENT履約;RISK風險',
    old_status              VARCHAR(30)                             NULL COMMENT '原狀態',
    new_status              VARCHAR(30)                         NOT NULL COMMENT '新狀態',
    event_code              VARCHAR(100)                            NULL COMMENT '觸發事件代碼',
    source_service_code     VARCHAR(80)                             NULL COMMENT '來源服務代碼',
    source_sid              VARCHAR(32)                             NULL COMMENT '來源資料序號',
    operator_sid            VARCHAR(32)                             NULL COMMENT '操作人員序號',
    reason_code             VARCHAR(50)                             NULL COMMENT '原因代碼',
    reason                  TEXT                                    NULL COMMENT '原因說明',
    correlation_id          VARCHAR(100)                            NULL COMMENT '關聯識別碼',
    CONSTRAINT fk_oosh_order
        FOREIGN KEY (oms_order_nid) REFERENCES oms_order(nid),
    INDEX idx_oosh_order_nid (oms_order_nid),
    INDEX idx_oosh_status_type (status_type),
    INDEX idx_oosh_new_status (new_status),
    INDEX idx_oosh_event_code (event_code),
    INDEX idx_oosh_source (source_service_code, source_sid),
    INDEX idx_oosh_create_date (create_date),
    INDEX idx_oosh_correlation_id (correlation_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS訂單狀態歷程';

CREATE TABLE IF NOT EXISTS oms_status_mapping (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '狀態對應序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    channel_sid             VARCHAR(32)                         NOT NULL COMMENT '通路序號',
    status_type             VARCHAR(30)                         NOT NULL COMMENT 'ORDER;PAYMENT;FULFILLMENT;RETURN',
    external_status         VARCHAR(100)                        NOT NULL COMMENT '外部通路狀態',
    oms_status              VARCHAR(30)                         NOT NULL COMMENT 'OMS標準狀態',
    direction               VARCHAR(20)                         NOT NULL DEFAULT 'BOTH' COMMENT 'INBOUND匯入;OUTBOUND回寫;BOTH雙向',
    priority                INT                                 NOT NULL DEFAULT 0 COMMENT '優先順序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_osm_mapping UNIQUE (channel_sid, status_type, external_status, direction),
    INDEX idx_osm_channel_sid (channel_sid),
    INDEX idx_osm_status_type (status_type),
    INDEX idx_osm_oms_status (oms_status),
    INDEX idx_osm_direction (direction),
    INDEX idx_osm_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='外部通路與OMS狀態對應';

CREATE TABLE IF NOT EXISTS oms_status_sync_job (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '狀態同步工作序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    job_no                  VARCHAR(100)                        NOT NULL COMMENT '同步工作編號',
    oms_order_sid           VARCHAR(32)                         NOT NULL COMMENT 'OMS訂單序號',
    channel_account_sid     VARCHAR(32)                         NOT NULL COMMENT '通路帳號序號',
    sync_type               VARCHAR(30)                         NOT NULL COMMENT 'ORDER_STATUS訂單;SHIPMENT物流;CANCEL取消;REFUND退款',
    target_status           VARCHAR(100)                            NULL COMMENT '目標外部狀態',
    request_payload         JSON                                    NULL COMMENT '送出內容',
    response_payload        JSON                                    NULL COMMENT '回應內容',
    sync_status             VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待送;PROCESSING處理中;SUCCESS成功;FAILED失敗;RETRY重試;DEAD死信',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '重試次數',
    max_retry_count         INT                                 NOT NULL DEFAULT 5 COMMENT '最大重試次數',
    next_retry_date         DATETIME                                NULL COMMENT '下次重試時間',
    external_reference_no   VARCHAR(100)                            NULL COMMENT '外部回應編號',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    error_message           LONGTEXT                                NULL COMMENT '錯誤訊息',
    CONSTRAINT uk_ossj_job_no UNIQUE (job_no),
    INDEX idx_ossj_order_sid (oms_order_sid),
    INDEX idx_ossj_channel_account_sid (channel_account_sid),
    INDEX idx_ossj_sync_type (sync_type),
    INDEX idx_ossj_status (sync_status),
    INDEX idx_ossj_next_retry_date (next_retry_date),
    CHECK (retry_count >= 0),
    CHECK (max_retry_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS通路狀態回寫工作';

-- =========================================================
-- 07. 訂單取消
-- =========================================================

CREATE TABLE IF NOT EXISTS oms_cancel_request (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '取消申請序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    cancel_no               VARCHAR(100)                        NOT NULL COMMENT '取消申請編號',
    oms_order_nid           BIGINT UNSIGNED                     NOT NULL COMMENT 'OMS訂單流水號',
    requester_type          VARCHAR(30)                         NOT NULL COMMENT 'CUSTOMER客戶;CHANNEL通路;STORE商店;SYSTEM系統;ADMIN管理員',
    requester_sid           VARCHAR(32)                             NULL COMMENT '申請對象序號',
    cancel_scope            VARCHAR(20)                         NOT NULL COMMENT 'FULL整單;PARTIAL部分',
    reason_code             VARCHAR(50)                         NOT NULL COMMENT '取消原因代碼',
    reason_description      TEXT                                    NULL COMMENT '取消原因說明',
    cancel_status           VARCHAR(30)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;VALIDATING驗證中;APPROVED已核准;REJECTED駁回;PROCESSING取消中;COMPLETED完成;PARTIAL部分完成;FAILED失敗',
    refund_required         TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否需要退款',
    inventory_release_required TINYINT(1)                       NOT NULL DEFAULT 1 COMMENT '是否需要釋放庫存',
    fulfillment_cancel_required TINYINT(1)                      NOT NULL DEFAULT 1 COMMENT '是否需要取消履約',
    approved_user_sid       VARCHAR(32)                             NULL COMMENT '核准人員序號',
    approved_date           DATETIME                                NULL COMMENT '核准時間',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    workflow_instance_sid   VARCHAR(32)                             NULL COMMENT 'WorkflowDB流程實例序號',
    error_message           LONGTEXT                                NULL COMMENT '錯誤訊息',
    CONSTRAINT fk_ocr_order
        FOREIGN KEY (oms_order_nid) REFERENCES oms_order(nid),
    CONSTRAINT uk_ocr_cancel_no UNIQUE (cancel_no),
    INDEX idx_ocr_order_nid (oms_order_nid),
    INDEX idx_ocr_requester (requester_type, requester_sid),
    INDEX idx_ocr_status (cancel_status),
    INDEX idx_ocr_workflow_instance_sid (workflow_instance_sid),
    INDEX idx_ocr_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS訂單取消申請';

CREATE TABLE IF NOT EXISTS oms_cancel_item (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '取消申請明細序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    cancel_request_nid      BIGINT UNSIGNED                     NOT NULL COMMENT '取消申請流水號',
    oms_order_item_nid      BIGINT UNSIGNED                     NOT NULL COMMENT 'OMS訂單明細流水號',
    request_qty             DECIMAL(20,6)                       NOT NULL COMMENT '申請取消數量',
    approved_qty            DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '核准取消數量',
    completed_qty           DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '完成取消數量',
    cancel_amount           DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '取消金額',
    cancel_status           VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;APPROVED核准;COMPLETED完成;REJECTED駁回;FAILED失敗',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_oci_cancel_request
        FOREIGN KEY (cancel_request_nid) REFERENCES oms_cancel_request(nid),
    CONSTRAINT fk_oci_order_item
        FOREIGN KEY (oms_order_item_nid) REFERENCES oms_order_item(nid),
    CONSTRAINT uk_oci_request_item UNIQUE (cancel_request_nid, oms_order_item_nid),
    INDEX idx_oci_cancel_request_nid (cancel_request_nid),
    INDEX idx_oci_order_item_nid (oms_order_item_nid),
    INDEX idx_oci_status (cancel_status),
    CHECK (request_qty > 0),
    CHECK (approved_qty >= 0),
    CHECK (completed_qty >= 0),
    CHECK (approved_qty <= request_qty),
    CHECK (completed_qty <= approved_qty),
    CHECK (cancel_amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS訂單取消明細';

-- =========================================================
-- 08. 訂單例外與人工處理
-- =========================================================

CREATE TABLE IF NOT EXISTS oms_exception (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'OMS例外序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    exception_no            VARCHAR(100)                        NOT NULL COMMENT '例外編號',
    oms_order_sid           VARCHAR(32)                         NOT NULL COMMENT 'OMS訂單序號',
    fulfillment_order_sid   VARCHAR(32)                             NULL COMMENT '履約訂單序號',
    exception_type          VARCHAR(50)                         NOT NULL COMMENT 'INVALID_DATA資料錯誤;PAYMENT付款;STOCK庫存;ROUTING路由;FULFILLMENT履約;CHANNEL_SYNC通路同步;ADDRESS地址;RISK風險;OTHER其他',
    exception_code          VARCHAR(100)                        NOT NULL COMMENT '例外代碼',
    severity                VARCHAR(20)                         NOT NULL DEFAULT 'WARNING' COMMENT 'INFO資訊;WARNING警告;HIGH高;CRITICAL嚴重',
    title                   VARCHAR(500)                        NOT NULL COMMENT '例外標題',
    description             LONGTEXT                            NOT NULL COMMENT '例外說明',
    exception_data          JSON                                    NULL COMMENT '例外資料',
    exception_status        VARCHAR(20)                         NOT NULL DEFAULT 'OPEN' COMMENT 'OPEN未處理;ASSIGNED已分派;PROCESSING處理中;RESOLVED已解決;IGNORED忽略;CLOSED結案',
    assigned_user_sid       VARCHAR(32)                             NULL COMMENT '承辦人員序號',
    assigned_date           DATETIME                                NULL COMMENT '分派時間',
    resolved_date           DATETIME                                NULL COMMENT '解決時間',
    resolution_code         VARCHAR(50)                             NULL COMMENT '解決方式代碼',
    resolution_note         TEXT                                    NULL COMMENT '解決說明',
    retryable               TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否可重試',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '重試次數',
    next_retry_date         DATETIME                                NULL COMMENT '下次重試時間',
    workflow_instance_sid   VARCHAR(32)                             NULL COMMENT 'WorkflowDB流程實例序號',
    CONSTRAINT uk_oe_exception_no UNIQUE (exception_no),
    INDEX idx_oe_order_sid (oms_order_sid),
    INDEX idx_oe_fulfillment_order_sid (fulfillment_order_sid),
    INDEX idx_oe_exception_type (exception_type),
    INDEX idx_oe_exception_code (exception_code),
    INDEX idx_oe_severity (severity),
    INDEX idx_oe_status (exception_status),
    INDEX idx_oe_assigned_user_sid (assigned_user_sid),
    INDEX idx_oe_next_retry_date (next_retry_date),
    INDEX idx_oe_workflow_instance_sid (workflow_instance_sid),
    INDEX idx_oe_create_date (create_date),
    CHECK (retry_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS訂單例外';

CREATE TABLE IF NOT EXISTS oms_exception_action (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '例外處理紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '處理時間',
    exception_nid           BIGINT UNSIGNED                     NOT NULL COMMENT 'OMS例外流水號',
    action_code             VARCHAR(50)                         NOT NULL COMMENT 'ASSIGN分派;RETRY重試;REROUTE重新路由;CANCEL取消;OVERRIDE覆核;RESOLVE解決;IGNORE忽略',
    action_user_sid         VARCHAR(32)                             NULL COMMENT '操作人員序號',
    action_data             JSON                                    NULL COMMENT '處理資料',
    action_result           VARCHAR(20)                         NOT NULL DEFAULT 'SUCCESS' COMMENT 'SUCCESS成功;FAILED失敗',
    message                 TEXT                                    NULL COMMENT '處理說明',
    CONSTRAINT fk_oea_exception
        FOREIGN KEY (exception_nid) REFERENCES oms_exception(nid),
    INDEX idx_oea_exception_nid (exception_nid),
    INDEX idx_oea_action_code (action_code),
    INDEX idx_oea_action_user_sid (action_user_sid),
    INDEX idx_oea_result (action_result),
    INDEX idx_oea_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS例外處理紀錄';

-- =========================================================
-- 09. 事件與協調
-- =========================================================

CREATE TABLE IF NOT EXISTS oms_order_event (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'OMS訂單事件序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    oms_order_sid           VARCHAR(32)                         NOT NULL COMMENT 'OMS訂單序號',
    event_code              VARCHAR(120)                        NOT NULL COMMENT '事件代碼',
    event_version           INT                                 NOT NULL DEFAULT 1 COMMENT '事件版本',
    event_data              JSON                                    NULL COMMENT '事件內容',
    source_service_code     VARCHAR(80)                             NULL COMMENT '來源服務代碼',
    source_event_id         VARCHAR(100)                            NULL COMMENT '來源事件ID',
    correlation_id          VARCHAR(100)                            NULL COMMENT '關聯識別碼',
    causation_id            VARCHAR(100)                            NULL COMMENT '因果事件ID',
    outbox_event_sid        VARCHAR(32)                             NULL COMMENT 'IntegrationDB Outbox事件序號',
    process_status          VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;SUCCESS成功;FAILED失敗;IGNORED忽略',
    processed_date          DATETIME                                NULL COMMENT '處理時間',
    error_message           TEXT                                    NULL COMMENT '錯誤訊息',
    CONSTRAINT uk_ooe_source_event UNIQUE (source_service_code, source_event_id),
    INDEX idx_ooe_order_sid (oms_order_sid),
    INDEX idx_ooe_event_code (event_code),
    INDEX idx_ooe_correlation_id (correlation_id),
    INDEX idx_ooe_outbox_event_sid (outbox_event_sid),
    INDEX idx_ooe_status (process_status),
    INDEX idx_ooe_create_date (create_date),
    CHECK (event_version > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS訂單領域事件';

CREATE TABLE IF NOT EXISTS oms_sla_monitor (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'OMS SLA監控序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    oms_order_sid           VARCHAR(32)                         NOT NULL COMMENT 'OMS訂單序號',
    fulfillment_order_sid   VARCHAR(32)                             NULL COMMENT '履約訂單序號',
    sla_type                VARCHAR(30)                         NOT NULL COMMENT 'ACCEPT接單;ALLOCATE配貨;SHIP出貨;DELIVER送達;CANCEL取消;REFUND退款',
    target_date             DATETIME                            NOT NULL COMMENT '目標完成時間',
    actual_date             DATETIME                                NULL COMMENT '實際完成時間',
    sla_status              VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING監控中;MET達成;AT_RISK有風險;BREACHED違約;CANCELLED取消',
    warning_date            DATETIME                                NULL COMMENT '預警時間',
    breached_date           DATETIME                                NULL COMMENT '違約時間',
    notification_sid        VARCHAR(32)                             NULL COMMENT 'NotificationDB通知序號',
    exception_sid           VARCHAR(32)                             NULL COMMENT '產生的OMS例外序號',
    INDEX idx_osm_order_sid (oms_order_sid),
    INDEX idx_osm_fulfillment_order_sid (fulfillment_order_sid),
    INDEX idx_osm_sla_type (sla_type),
    INDEX idx_osm_target_date (target_date),
    INDEX idx_osm_status (sla_status),
    INDEX idx_osm_exception_sid (exception_sid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OMS訂單SLA監控';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
