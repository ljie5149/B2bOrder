namespace B2bOrder.Resources.System
{
    /// <summary>
    /// ReportDB Core Schema
    /// 規範：
    /// 1. 報表資料以彙總、快照及匯出任務為主，不作為交易資料來源
    /// 2. MasterDB、CatalogDB、ShoppingDB、InventoryDB、PurchaseDB 等跨資料庫參照使用 sid
    /// 3. 所有彙總表均保留統計日期與資料更新時間
    /// 4. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class ReportDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 報表定義與匯出任務
-- =========================================================

CREATE TABLE IF NOT EXISTS rpt_report_definition (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '報表定義序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    report_code             VARCHAR(100)                        NOT NULL COMMENT '報表代碼',
    report_name             VARCHAR(200)                        NOT NULL COMMENT '報表名稱',
    report_category         VARCHAR(50)                         NOT NULL COMMENT 'SALES銷售;INVENTORY庫存;PURCHASE採購;FINANCE財務;MEMBER會員;STORE商店',
    source_service_code     VARCHAR(80)                             NULL COMMENT '主要來源服務代碼',
    query_config            JSON                                    NULL COMMENT '查詢條件與欄位設定',
    default_format          VARCHAR(20)                         NOT NULL DEFAULT 'XLSX' COMMENT 'XLSX;CSV;PDF',
    allow_schedule          TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否允許排程',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_rrd_report_code UNIQUE (report_code),
    INDEX idx_rrd_category (report_category),
    INDEX idx_rrd_source_service (source_service_code),
    INDEX idx_rrd_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='報表定義';

CREATE TABLE IF NOT EXISTS rpt_export_job (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '報表匯出工作序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    job_no                  VARCHAR(80)                         NOT NULL COMMENT '匯出工作編號',
    report_definition_nid   BIGINT UNSIGNED                     NOT NULL COMMENT '報表定義流水號',
    requester_sid           VARCHAR(32)                         NOT NULL COMMENT '申請人員序號',
    filter_json             JSON                                    NULL COMMENT '報表篩選條件',
    export_format           VARCHAR(20)                         NOT NULL DEFAULT 'XLSX' COMMENT 'XLSX;CSV;PDF',
    job_status              VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING等待;PROCESSING處理中;SUCCESS成功;FAILED失敗;EXPIRED過期',
    progress_percent        INT                                 NOT NULL DEFAULT 0 COMMENT '完成百分比',
    file_name               VARCHAR(255)                            NULL COMMENT '輸出檔名',
    file_url                VARCHAR(1000)                           NULL COMMENT '檔案網址',
    file_size               BIGINT UNSIGNED                         NULL COMMENT '檔案大小Bytes',
    expiry_date             DATETIME                                NULL COMMENT '檔案到期時間',
    start_date              DATETIME                                NULL COMMENT '開始處理時間',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    error_message           LONGTEXT                                NULL COMMENT '錯誤訊息',
    CONSTRAINT fk_rej_report_definition
        FOREIGN KEY (report_definition_nid) REFERENCES rpt_report_definition(nid),
    CONSTRAINT uk_rej_job_no UNIQUE (job_no),
    INDEX idx_rej_report_definition_nid (report_definition_nid),
    INDEX idx_rej_requester_sid (requester_sid),
    INDEX idx_rej_status (job_status),
    INDEX idx_rej_create_date (create_date),
    INDEX idx_rej_expiry_date (expiry_date),
    CHECK (progress_percent BETWEEN 0 AND 100)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='報表匯出工作';

CREATE TABLE IF NOT EXISTS rpt_schedule (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '報表排程序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    schedule_name           VARCHAR(200)                        NOT NULL COMMENT '排程名稱',
    report_definition_nid   BIGINT UNSIGNED                     NOT NULL COMMENT '報表定義流水號',
    cron_expression         VARCHAR(100)                        NOT NULL COMMENT 'Cron排程表示式',
    timezone_code           VARCHAR(100)                        NOT NULL DEFAULT 'Asia/Taipei' COMMENT '執行時區',
    filter_json             JSON                                    NULL COMMENT '報表篩選條件',
    export_format           VARCHAR(20)                         NOT NULL DEFAULT 'XLSX' COMMENT 'XLSX;CSV;PDF',
    delivery_type           VARCHAR(20)                         NOT NULL DEFAULT 'EMAIL' COMMENT 'EMAIL;SYSTEM;STORAGE',
    recipients_json         JSON                                    NULL COMMENT '收件人清單',
    last_run_date           DATETIME                                NULL COMMENT '最後執行時間',
    next_run_date           DATETIME                                NULL COMMENT '下次執行時間',
    schedule_status         VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;PAUSED暫停;DISABLED停用',
    owner_sid               VARCHAR(32)                         NOT NULL COMMENT '排程擁有人序號',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_rs_report_definition
        FOREIGN KEY (report_definition_nid) REFERENCES rpt_report_definition(nid),
    INDEX idx_rs_report_definition_nid (report_definition_nid),
    INDEX idx_rs_status (schedule_status),
    INDEX idx_rs_next_run_date (next_run_date),
    INDEX idx_rs_owner_sid (owner_sid),
    INDEX idx_rs_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='報表排程';

-- =========================================================
-- 02. 銷售彙總
-- =========================================================

CREATE TABLE IF NOT EXISTS rpt_sales_daily (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '每日銷售彙總序號',
    statistics_date         DATE                                NOT NULL COMMENT '統計日期',
    store_sid               VARCHAR(32)                             NULL COMMENT 'StoreDB商店序號，NULL代表全平台',
    currency_sid            VARCHAR(32)                         NOT NULL COMMENT 'MasterDB幣別序號',
    order_count             BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '訂單數',
    paid_order_count        BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '已付款訂單數',
    cancelled_order_count   BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '取消訂單數',
    completed_order_count   BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '完成訂單數',
    item_quantity           DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '銷售商品數量',
    gross_sales_amount      DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '銷售總額',
    discount_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '折扣金額',
    freight_amount          DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '運費',
    tax_amount              DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '稅額',
    refund_amount           DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '退款金額',
    net_sales_amount        DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '淨銷售額',
    average_order_amount    DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '平均客單價',
    new_member_order_count  BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '新會員訂單數',
    update_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新時間',
    CONSTRAINT uk_rsd_date_store_currency UNIQUE (statistics_date, store_sid, currency_sid),
    INDEX idx_rsd_statistics_date (statistics_date),
    INDEX idx_rsd_store_sid (store_sid),
    INDEX idx_rsd_currency_sid (currency_sid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='每日銷售彙總';

CREATE TABLE IF NOT EXISTS rpt_product_sales_daily (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '商品每日銷售彙總序號',
    statistics_date         DATE                                NOT NULL COMMENT '統計日期',
    store_sid               VARCHAR(32)                             NULL COMMENT 'StoreDB商店序號',
    product_sid             VARCHAR(32)                         NOT NULL COMMENT 'CatalogDB商品序號',
    sku_sid                 VARCHAR(32)                         NOT NULL COMMENT 'CatalogDB SKU序號',
    category_sid            VARCHAR(32)                             NULL COMMENT 'MasterDB分類序號',
    brand_sid               VARCHAR(32)                             NULL COMMENT 'MasterDB品牌序號',
    currency_sid            VARCHAR(32)                         NOT NULL COMMENT 'MasterDB幣別序號',
    order_count             BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '訂單數',
    sold_qty                DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '銷售數量',
    returned_qty            DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '退貨數量',
    gross_sales_amount      DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '銷售總額',
    discount_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '折扣金額',
    refund_amount           DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '退款金額',
    net_sales_amount        DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '淨銷售額',
    cost_amount             DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '銷貨成本',
    gross_profit_amount     DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '毛利',
    update_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新時間',
    CONSTRAINT uk_rpsd_date_store_sku UNIQUE (statistics_date, store_sid, sku_sid),
    INDEX idx_rpsd_statistics_date (statistics_date),
    INDEX idx_rpsd_store_sid (store_sid),
    INDEX idx_rpsd_product_sid (product_sid),
    INDEX idx_rpsd_sku_sid (sku_sid),
    INDEX idx_rpsd_category_sid (category_sid),
    INDEX idx_rpsd_brand_sid (brand_sid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商品每日銷售彙總';

-- =========================================================
-- 03. 會員彙總
-- =========================================================

CREATE TABLE IF NOT EXISTS rpt_member_daily (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '會員每日彙總序號',
    statistics_date         DATE                                NOT NULL COMMENT '統計日期',
    price_group_sid         VARCHAR(32)                             NULL COMMENT 'MasterDB價格群組序號',
    registered_count        BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '新增會員數',
    active_member_count     BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '活躍會員數',
    purchasing_member_count BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '購買會員數',
    first_purchase_count    BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '首次購買會員數',
    repeat_purchase_count   BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '回購會員數',
    login_count             BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '登入次數',
    average_order_amount    DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '平均客單價',
    total_sales_amount      DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '會員銷售金額',
    update_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新時間',
    CONSTRAINT uk_rmd_date_price_group UNIQUE (statistics_date, price_group_sid),
    INDEX idx_rmd_statistics_date (statistics_date),
    INDEX idx_rmd_price_group_sid (price_group_sid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='會員每日統計';

-- =========================================================
-- 04. 庫存彙總
-- =========================================================

CREATE TABLE IF NOT EXISTS rpt_inventory_daily (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '每日庫存彙總序號',
    statistics_date         DATE                                NOT NULL COMMENT '統計日期',
    warehouse_sid           VARCHAR(32)                         NOT NULL COMMENT 'MasterDB倉庫序號',
    product_sid             VARCHAR(32)                         NOT NULL COMMENT 'CatalogDB商品序號',
    sku_sid                 VARCHAR(32)                         NOT NULL COMMENT 'CatalogDB SKU序號',
    on_hand_qty             DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '帳面庫存量',
    reserved_qty            DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '預留數量',
    available_qty           DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '可用庫存量',
    damaged_qty             DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '不良品數量',
    in_transit_qty          DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '在途數量',
    inventory_value         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '庫存金額',
    average_cost            DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '平均成本',
    days_of_inventory       DECIMAL(12,2)                           NULL COMMENT '庫存可售天數',
    no_movement_days        INT                                 NOT NULL DEFAULT 0 COMMENT '無異動天數',
    update_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新時間',
    CONSTRAINT uk_rid_date_warehouse_sku UNIQUE (statistics_date, warehouse_sid, sku_sid),
    INDEX idx_rid_statistics_date (statistics_date),
    INDEX idx_rid_warehouse_sid (warehouse_sid),
    INDEX idx_rid_product_sid (product_sid),
    INDEX idx_rid_sku_sid (sku_sid),
    INDEX idx_rid_available_qty (available_qty),
    INDEX idx_rid_no_movement_days (no_movement_days)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='每日庫存彙總';

CREATE TABLE IF NOT EXISTS rpt_inventory_alert (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '庫存警示序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    alert_date              DATE                                NOT NULL COMMENT '警示日期',
    warehouse_sid           VARCHAR(32)                         NOT NULL COMMENT 'MasterDB倉庫序號',
    product_sid             VARCHAR(32)                         NOT NULL COMMENT 'CatalogDB商品序號',
    sku_sid                 VARCHAR(32)                         NOT NULL COMMENT 'CatalogDB SKU序號',
    alert_type              VARCHAR(30)                         NOT NULL COMMENT 'LOW_STOCK低庫存;OUT_OF_STOCK缺貨;OVER_STOCK高庫存;EXPIRY效期;NO_MOVEMENT呆滯',
    alert_level             VARCHAR(20)                         NOT NULL DEFAULT 'WARNING' COMMENT 'INFO資訊;WARNING警告;CRITICAL嚴重',
    current_qty             DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '目前數量',
    threshold_qty           DECIMAL(20,6)                           NULL COMMENT '警示門檻',
    expiry_date             DATE                                    NULL COMMENT '商品效期',
    no_movement_days        INT                                     NULL COMMENT '無異動天數',
    alert_status            VARCHAR(20)                         NOT NULL DEFAULT 'OPEN' COMMENT 'OPEN未處理;ACKNOWLEDGED已確認;RESOLVED已解決;IGNORED忽略',
    resolved_user_sid       VARCHAR(32)                             NULL COMMENT '處理人員序號',
    resolved_date           DATETIME                                NULL COMMENT '解決時間',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ria_alert UNIQUE (alert_date, warehouse_sid, sku_sid, alert_type),
    INDEX idx_ria_alert_date (alert_date),
    INDEX idx_ria_warehouse_sid (warehouse_sid),
    INDEX idx_ria_product_sid (product_sid),
    INDEX idx_ria_sku_sid (sku_sid),
    INDEX idx_ria_alert_type (alert_type),
    INDEX idx_ria_alert_level (alert_level),
    INDEX idx_ria_status (alert_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='庫存警示';

-- =========================================================
-- 05. 採購與供應商績效
-- =========================================================

CREATE TABLE IF NOT EXISTS rpt_purchase_daily (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '每日採購彙總序號',
    statistics_date         DATE                                NOT NULL COMMENT '統計日期',
    supplier_sid            VARCHAR(32)                             NULL COMMENT 'PurchaseDB供應商序號，NULL代表全部',
    currency_sid            VARCHAR(32)                         NOT NULL COMMENT 'MasterDB幣別序號',
    purchase_order_count    BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '採購單數',
    ordered_qty             DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '採購數量',
    received_qty            DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '收貨數量',
    accepted_qty            DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '驗收合格數量',
    rejected_qty            DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '驗收不良數量',
    returned_qty            DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '採購退貨數量',
    purchase_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '採購金額',
    received_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '收貨金額',
    return_amount           DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '退貨金額',
    update_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新時間',
    CONSTRAINT uk_rpd_date_supplier_currency UNIQUE (statistics_date, supplier_sid, currency_sid),
    INDEX idx_rpd_statistics_date (statistics_date),
    INDEX idx_rpd_supplier_sid (supplier_sid),
    INDEX idx_rpd_currency_sid (currency_sid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='每日採購彙總';

CREATE TABLE IF NOT EXISTS rpt_supplier_performance (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '供應商績效序號',
    period_type             VARCHAR(20)                         NOT NULL COMMENT 'MONTH月;QUARTER季;YEAR年',
    period_code             VARCHAR(20)                         NOT NULL COMMENT '統計期間代碼',
    supplier_sid            VARCHAR(32)                         NOT NULL COMMENT 'PurchaseDB供應商序號',
    purchase_order_count    BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '採購單數',
    purchase_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '採購金額',
    on_time_delivery_rate   DECIMAL(8,4)                        NOT NULL DEFAULT 0 COMMENT '準時交貨率',
    acceptance_rate         DECIMAL(8,4)                        NOT NULL DEFAULT 0 COMMENT '驗收合格率',
    return_rate             DECIMAL(8,4)                        NOT NULL DEFAULT 0 COMMENT '退貨率',
    average_lead_time_days  DECIMAL(10,2)                       NOT NULL DEFAULT 0 COMMENT '平均交期天數',
    price_variance_rate     DECIMAL(8,4)                        NOT NULL DEFAULT 0 COMMENT '價格差異率',
    performance_score       DECIMAL(8,2)                        NOT NULL DEFAULT 0 COMMENT '績效總分',
    update_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新時間',
    CONSTRAINT uk_rsp_period_supplier UNIQUE (period_type, period_code, supplier_sid),
    INDEX idx_rsp_period (period_type, period_code),
    INDEX idx_rsp_supplier_sid (supplier_sid),
    INDEX idx_rsp_score (performance_score),
    CHECK (on_time_delivery_rate >= 0),
    CHECK (acceptance_rate >= 0),
    CHECK (return_rate >= 0),
    CHECK (average_lead_time_days >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='供應商績效統計';

-- =========================================================
-- 06. 財務與商店結算彙總
-- =========================================================

CREATE TABLE IF NOT EXISTS rpt_finance_daily (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '每日財務彙總序號',
    statistics_date         DATE                                NOT NULL COMMENT '統計日期',
    currency_sid            VARCHAR(32)                         NOT NULL COMMENT 'MasterDB幣別序號',
    receivable_amount       DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '新增應收',
    receipt_amount          DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '收款金額',
    refund_amount           DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '退款金額',
    payable_amount          DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '新增應付',
    payment_amount          DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '付款金額',
    platform_fee_amount     DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '平台服務費收入',
    payment_fee_amount      DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '金流手續費',
    shipping_fee_amount     DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '物流費',
    outstanding_ar_amount   DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '期末未收款',
    outstanding_ap_amount   DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '期末未付款',
    update_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新時間',
    CONSTRAINT uk_rfd_date_currency UNIQUE (statistics_date, currency_sid),
    INDEX idx_rfd_statistics_date (statistics_date),
    INDEX idx_rfd_currency_sid (currency_sid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='每日財務彙總';

CREATE TABLE IF NOT EXISTS rpt_store_settlement_period (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '商店結算統計序號',
    period_start_date       DATE                                NOT NULL COMMENT '期間開始日',
    period_end_date         DATE                                NOT NULL COMMENT '期間結束日',
    store_sid               VARCHAR(32)                         NOT NULL COMMENT 'StoreDB商店序號',
    currency_sid            VARCHAR(32)                         NOT NULL COMMENT 'MasterDB幣別序號',
    order_count             BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '結算訂單數',
    gross_sales_amount      DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '銷售總額',
    refund_amount           DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '退款金額',
    discount_share_amount   DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '商店負擔優惠',
    platform_fee_amount     DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '平台服務費',
    payment_fee_amount      DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '金流手續費',
    shipping_fee_amount     DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '物流費',
    adjustment_amount       DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '調整金額',
    payable_amount          DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '應付商店金額',
    paid_amount             DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '已付商店金額',
    update_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新時間',
    CONSTRAINT uk_rssp_period_store_currency UNIQUE (period_start_date, period_end_date, store_sid, currency_sid),
    INDEX idx_rssp_period (period_start_date, period_end_date),
    INDEX idx_rssp_store_sid (store_sid),
    INDEX idx_rssp_currency_sid (currency_sid),
    CHECK (period_end_date >= period_start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商店結算期間統計';

-- =========================================================
-- 07. 彙總工作紀錄
-- =========================================================

CREATE TABLE IF NOT EXISTS rpt_build_job (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '彙總工作序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    job_no                  VARCHAR(80)                         NOT NULL COMMENT '彙總工作編號',
    job_type                VARCHAR(50)                         NOT NULL COMMENT 'SALES_DAILY;PRODUCT_SALES;MEMBER;INVENTORY;PURCHASE;FINANCE;STORE_SETTLEMENT',
    statistics_date         DATE                                    NULL COMMENT '統計日期',
    period_start_date       DATE                                    NULL COMMENT '期間開始日',
    period_end_date         DATE                                    NULL COMMENT '期間結束日',
    start_date              DATETIME                            NOT NULL COMMENT '開始時間',
    end_date                DATETIME                                NULL COMMENT '結束時間',
    read_count              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '讀取筆數',
    write_count             BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '寫入筆數',
    error_count             BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '錯誤筆數',
    job_status              VARCHAR(20)                         NOT NULL DEFAULT 'RUNNING' COMMENT 'RUNNING執行中;SUCCESS成功;PARTIAL部分成功;FAILED失敗',
    error_message           LONGTEXT                                NULL COMMENT '錯誤訊息',
    CONSTRAINT uk_rbj_job_no UNIQUE (job_no),
    INDEX idx_rbj_job_type (job_type),
    INDEX idx_rbj_statistics_date (statistics_date),
    INDEX idx_rbj_period (period_start_date, period_end_date),
    INDEX idx_rbj_status (job_status),
    INDEX idx_rbj_start_date (start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='報表彙總工作紀錄';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
