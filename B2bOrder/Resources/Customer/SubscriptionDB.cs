namespace B2bOrder.Resources.Customer
{
    /// <summary>
    /// SubscriptionDB V1 Shared Core Schema
    /// 設計目標：
    /// 1. 管理訂閱服務方案 (Subscription Plan) 與計費週期定價 (Billing Cycles / Pricing Tiers)
    /// 2. 管理會員/客戶訂閱合約主檔 (Customer Subscription) 及其週期狀態 (Active, Paused, Cancelled, Past Due)
    /// 3. 管理自動週期扣款帳單 (Recurring Invoice) 與支付嘗試紀錄 (Payment Attempt / Retry Mechanism)
    /// 4. 記錄訂閱生命週期異動歷程 (Subscription Lifecycle Event Log，如升降級、暫停、復權、取消)
    /// 5. 整合與串接 SalesOrderDB (產生定期銷售單)、PaymentDB (金流定期定額扣款) 與 CustomerDB (客戶資料)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class SubscriptionDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 訂閱方案與計費週期定價 (Plans & Pricing Tiers)
-- =========================================================

CREATE TABLE IF NOT EXISTS sub_plan (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '訂閱方案序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    plan_code               VARCHAR(50)                         NOT NULL COMMENT '方案代碼 (例: BASIC_MONTHLY, PRO_ANNUAL)',
    plan_name               VARCHAR(200)                        NOT NULL COMMENT '方案名稱 (例: 專業版年繳方案)',
    description             TEXT                                    NULL COMMENT '方案詳細描述',
    plan_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE上架;INACTIVE下架/停售',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_sp_company_code UNIQUE (company_sid, plan_code),
    INDEX idx_sp_company_sid (company_sid),
    INDEX idx_sp_status (plan_status),
    INDEX idx_sp_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='訂閱方案主檔';

CREATE TABLE IF NOT EXISTS sub_plan_pricing (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '方案定價序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    plan_nid                BIGINT UNSIGNED                     NOT NULL COMMENT '訂閱方案流水號',
    billing_cycle_unit      VARCHAR(20)                         NOT NULL DEFAULT 'MONTH' COMMENT '計費週期單位: DAY天;WEEK週;MONTH月;YEAR年',
    billing_cycle_interval  INT                                 NOT NULL DEFAULT 1 COMMENT '週期間隔 (例: 1代表每月, 3代表季繳, 12代表年繳)',
    price_amount            DECIMAL(18,4)                       NOT NULL COMMENT '每期訂閱原價',
    currency_code           VARCHAR(10)                         NOT NULL DEFAULT 'TWD' COMMENT '幣別',
    trial_period_days       INT                                 NOT NULL DEFAULT 0 COMMENT '免費試用天數 (0代表無試用)',
    setup_fee_amount        DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '首次設定費/設定成本',
    pricing_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_spp_plan
        FOREIGN KEY (plan_nid) REFERENCES sub_plan(nid),
    INDEX idx_spp_plan_nid (plan_nid),
    INDEX idx_spp_status (pricing_status),
    INDEX idx_spp_avalible (avalible),
    CHECK (billing_cycle_interval > 0),
    CHECK (price_amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='訂閱方案週期定價檔';

-- =========================================================
-- 02. 會員訂閱合約主檔 (Customer Subscriptions)
-- =========================================================

CREATE TABLE IF NOT EXISTS sub_subscription (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '客戶訂閱序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    subscription_no         VARCHAR(100)                        NOT NULL COMMENT '訂閱合約單號',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    customer_sid            VARCHAR(32)                         NOT NULL COMMENT 'CustomerDB 會員/客戶序號',
    plan_nid                BIGINT UNSIGNED                     NOT NULL COMMENT '訂閱方案流水號',
    plan_pricing_nid        BIGINT UNSIGNED                     NOT NULL COMMENT '方案週期定價流水號',
    payment_method_sid      VARCHAR(32)                             NULL COMMENT 'PaymentDB 約定扣款卡號/信用卡 Token 序號',
    subscription_status     VARCHAR(30)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待啟用;TRIALING試用中;ACTIVE生效中;PAUSED暫停中;PAST_DUE逾期未繳;CANCELED已取消;EXPIRED已到期',
    start_date              DATETIME                            NOT NULL COMMENT '訂閱開始日期',
    trial_end_date          DATETIME                                NULL COMMENT '試用期結束時間',
    current_period_start    DATETIME                            NOT NULL COMMENT '當前計費週期開始時間',
    current_period_end      DATETIME                            NOT NULL COMMENT '當前計費週期結束時間',
    next_billing_date       DATETIME                                NULL COMMENT '下一次預計扣款/出帳日期',
    canceled_at             DATETIME                                NULL COMMENT '申請取消訂閱時間',
    ended_at                DATETIME                                NULL COMMENT '訂閱實際終止時間',
    auto_renew              VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y自動續訂;N不續訂',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_ss_plan
        FOREIGN KEY (plan_nid) REFERENCES sub_plan(nid),
    CONSTRAINT fk_ss_pricing
        FOREIGN KEY (plan_pricing_nid) REFERENCES sub_plan_pricing(nid),
    CONSTRAINT uk_ss_subscription_no UNIQUE (subscription_no),
    INDEX idx_ss_company_customer (company_sid, customer_sid),
    INDEX idx_ss_customer_sid (customer_sid),
    INDEX idx_ss_plan_nid (plan_nid),
    INDEX idx_ss_status (subscription_status),
    INDEX idx_ss_next_billing (next_billing_date),
    INDEX idx_ss_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='會員/客戶訂閱合約檔';

-- =========================================================
-- 03. 週期扣款帳單與扣款嘗試 (Recurring Invoices & Payment Attempts)
-- =========================================================

CREATE TABLE IF NOT EXISTS sub_invoice (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '訂閱帳單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    invoice_no              VARCHAR(100)                        NOT NULL COMMENT '訂閱帳單號碼',
    subscription_nid        BIGINT UNSIGNED                     NOT NULL COMMENT '訂閱合約流水號',
    customer_sid            VARCHAR(32)                         NOT NULL COMMENT '客戶序號',
    period_start            DATETIME                            NOT NULL COMMENT '本期計費起始時間',
    period_end              DATETIME                            NOT NULL COMMENT '本期計費結束時間',
    subtotal_amount         DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '小計金額',
    discount_amount         DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '折扣金額 (Loyalty/Coupon)',
    tax_amount              DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '稅額',
    total_amount            DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '應繳總金額',
    currency_code           VARCHAR(10)                         NOT NULL DEFAULT 'TWD' COMMENT '幣別',
    invoice_status          VARCHAR(30)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;OPEN待扣款;PAID已扣款成功;UNCOLLECTIBLE扣款失敗/呆帳;VOID已作廢',
    due_date                DATETIME                            NOT NULL COMMENT '應繳款截止日',
    paid_at                 DATETIME                                NULL COMMENT '實際扣款成功時間',
    sales_order_sid         VARCHAR(32)                             NULL COMMENT '關聯 SalesOrderDB 銷售單序號',
    payment_transaction_sid VARCHAR(32)                             NULL COMMENT '關聯 PaymentDB 扣款交易紀錄序號',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_si_subscription
        FOREIGN KEY (subscription_nid) REFERENCES sub_subscription(nid),
    CONSTRAINT uk_si_invoice_no UNIQUE (invoice_no),
    INDEX idx_si_subscription_nid (subscription_nid),
    INDEX idx_si_customer_sid (customer_sid),
    INDEX idx_si_status (invoice_status),
    INDEX idx_si_due_date (due_date),
    INDEX idx_si_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='訂閱週期出帳帳單檔';

CREATE TABLE IF NOT EXISTS sub_payment_attempt (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '扣款嘗試序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '嘗試時間',
    invoice_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '訂閱帳單流水號',
    attempt_number          INT                                 NOT NULL DEFAULT 1 COMMENT '重試次數 (第幾次扣款嘗試)',
    amount                  DECIMAL(18,4)                       NOT NULL COMMENT '嘗試扣款金額',
    payment_gateway_code    VARCHAR(50)                         NOT NULL COMMENT '金流通道代碼 (例: LINEPAY, STRIPE, TAPPAY)',
    payment_transaction_sid VARCHAR(32)                             NULL COMMENT 'PaymentDB 交易結果序號',
    attempt_status          VARCHAR(20)                         NOT NULL DEFAULT 'PROCESSING' COMMENT 'PROCESSING處理中;SUCCESS成功;FAILED失敗',
    failure_code            VARCHAR(100)                            NULL COMMENT '扣款失敗原因代碼 (例: INSUFFICIENT_FUNDS, CARD_EXPIRED)',
    failure_message         TEXT                                    NULL COMMENT '扣款失敗詳細訊息',
    next_retry_date         DATETIME                                NULL COMMENT '預計下次重試扣款時間',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_spa_invoice
        FOREIGN KEY (invoice_nid) REFERENCES sub_invoice(nid),
    INDEX idx_spa_invoice_nid (invoice_nid),
    INDEX idx_spa_status (attempt_status),
    INDEX idx_spa_next_retry (next_retry_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='訂閱扣款嘗試與重試紀錄檔';

-- =========================================================
-- 04. 訂閱生命週期歷程紀錄 (Subscription Lifecycle Events)
-- =========================================================

CREATE TABLE IF NOT EXISTS sub_subscription_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '訂閱歷程序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '事件發生時間',
    subscription_nid        BIGINT UNSIGNED                     NOT NULL COMMENT '訂閱合約流水號',
    event_type              VARCHAR(50)                         NOT NULL COMMENT '事件類型 (例: CREATED, RENEWED, UPGRADED, DOWNGRADED, PAUSED, RESUMED, CANCELED, EXPIRED)',
    previous_status         VARCHAR(30)                             NULL COMMENT '變更前狀態',
    new_status              VARCHAR(30)                         NOT NULL COMMENT '變更後狀態',
    previous_pricing_nid    BIGINT UNSIGNED                         NULL COMMENT '變更前定價方案流水號 (升降級時使用)',
    new_pricing_nid         BIGINT UNSIGNED                         NULL COMMENT '變更後定價方案流水號 (升降級時使用)',
    operator_type           VARCHAR(20)                         NOT NULL DEFAULT 'SYSTEM' COMMENT 'SYSTEM系統自動;CUSTOMER客戶自主;ADMIN管理者操作',
    operator_user_sid       VARCHAR(32)                             NULL COMMENT '操作人員帳號序號',
    reason_code             VARCHAR(100)                            NULL COMMENT '異動原因代碼 (例: TOO_EXPENSIVE, SWITCH_PLAN, HARDWARE_ISSUE)',
    remark                  TEXT                                    NULL COMMENT '備註/變更詳細說明',
    CONSTRAINT fk_ssl_subscription
        FOREIGN KEY (subscription_nid) REFERENCES sub_subscription(nid),
    INDEX idx_ssl_subscription_nid (subscription_nid),
    INDEX idx_ssl_event_type (event_type),
    INDEX idx_ssl_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='訂閱狀態異動歷程紀錄檔 (不可修改，僅供稽核)';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}