namespace B2bOrder.Resources.ShareCore
{
    /// <summary>
    /// ConfigDB V1 Shared Core Schema
    /// 設計目標：
    /// 1. 分散式系統組態 (System Configurations)：統一維護各微服務在不同環境 (DEV/STAGE/PROD) 的動態設定參數
    /// 2. 功能開關與灰度發布 (Feature Flags & Canary Toggles)：提供功能開關、百分比流量分流 (A/B Test) 與白名單控制
    /// 3. 通用字典與代碼對照 (System Dictionaries & Code Lookups)：管理跨服務通用的下拉選單、列舉狀態與代碼說明
    /// 4. 組態版本與變更歷史 (Config Version History & Rollback)：紀錄組態變更前後的 JSON 差異，支援即時回滾
    /// 5. 整合與串接全站微服務 (SalesOrderDB, CustomerDB, PIM, PaymentDB, AuditDB 等)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class ConfigDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 分散式系統組態主檔 (System Configurations)
-- =========================================================

CREATE TABLE IF NOT EXISTS cfg_system_config (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '組態序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號 (GLOBAL 代表全集團通用)',
    env_code                VARCHAR(20)                         NOT NULL DEFAULT 'PROD' COMMENT '環境代碼: DEV, STAGE, PROD, LOCAL',
    service_name            VARCHAR(50)                         NOT NULL COMMENT '微服務名稱 (例: SalesOrderDB, GLOBAL 代表全服務)',
    config_group            VARCHAR(50)                         NOT NULL DEFAULT 'DEFAULT' COMMENT '組態分類/模組 (例: PAYMENT, THRESHOLD, SECURITY)',
    config_key              VARCHAR(100)                        NOT NULL COMMENT '組態 Key (例: max_order_limit_per_day)',
    config_value            TEXT                                NOT NULL COMMENT '組態 Value (字串, 數字, 或 JSON 結構)',
    value_type              VARCHAR(20)                         NOT NULL DEFAULT 'STRING' COMMENT '資料型別: STRING, INT, DECIMAL, BOOLEAN, JSON',
    is_encrypted            VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT 'Y敏感資料已加密;N明文',
    is_dynamic              VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y動態生效(無需重啟);N需重啟服務',
    config_status           VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '組態用途與說明',
    CONSTRAINT uk_csc_env_service_key UNIQUE (company_sid, env_code, service_name, config_key),
    INDEX idx_csc_company_sid (company_sid),
    INDEX idx_csc_lookup (env_code, service_name, config_group),
    INDEX idx_csc_status (config_status),
    INDEX idx_csc_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='分散式系統動態組態設定檔';

-- =========================================================
-- 02. 功能開關與灰度發布 (Feature Flags & Canary Toggles)
-- =========================================================

CREATE TABLE IF NOT EXISTS cfg_feature_flag (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '功能開關序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    feature_code            VARCHAR(100)                        NOT NULL COMMENT '功能代碼 (例: FEATURE_NEW_CHECKOUT_FLOW)',
    feature_name            VARCHAR(150)                        NOT NULL COMMENT '功能名稱 (例: 新版購物車結算流程)',
    toggle_strategy         VARCHAR(30)                         NOT NULL DEFAULT 'BOOLEAN' COMMENT '策略: BOOLEAN全開全關, PERCENTAGE百分比灰度, USER_WHITELIST指定用戶, COMPANY_WHITELIST指定企業',
    is_enabled              VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT 'Y總開關開啟;N總開關關閉',
    percentage_rollout      INT                                 NOT NULL DEFAULT 0 COMMENT '灰度放量百分比 (0 ~ 100)',
    whitelist_rules_json    JSON                                    NULL COMMENT '白名單規則 JSON (例: { customer_sids: [], company_sids: [] })',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '功能說明與預計上線時間',
    CONSTRAINT uk_cff_company_feature UNIQUE (company_sid, feature_code),
    INDEX idx_cff_company_sid (company_sid),
    INDEX idx_cff_enabled (is_enabled),
    INDEX idx_cff_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='功能開關與灰度發布控制檔';

-- =========================================================
-- 03. 通用字典與代碼對照 (System Dictionaries & Lookups)
-- =========================================================

CREATE TABLE IF NOT EXISTS cfg_dictionary_category (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '字典分類序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號 (GLOBAL 表示全域)',
    category_code           VARCHAR(50)                         NOT NULL COMMENT '字典分類代碼 (例: ORDER_STATUS, PAYMENT_METHOD)',
    category_name           VARCHAR(100)                        NOT NULL COMMENT '字典分類名稱 (例: 訂單狀態列舉, 付款方式分類)',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_cdc_company_category UNIQUE (company_sid, category_code),
    INDEX idx_cdc_company_sid (company_sid),
    INDEX idx_cdc_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系統字典分類主檔';

CREATE TABLE IF NOT EXISTS cfg_dictionary_item (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '字典細項序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    category_sid            VARCHAR(32)                         NOT NULL COMMENT '對應 cfg_dictionary_category.sid',
    item_code               VARCHAR(50)                         NOT NULL COMMENT '字典項目代碼 (例: PENDING_PAYMENT)',
    item_value              VARCHAR(200)                        NOT NULL COMMENT '顯示名稱/數值 (例: 待付款)',
    item_locale             VARCHAR(10)                         NOT NULL DEFAULT 'zh-TW' COMMENT '語系 (例: zh-TW, en-US)',
    sort_order              INT                                 NOT NULL DEFAULT 0 COMMENT '顯示排序 (數字越小越前面)',
    is_default              VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT 'Y預設值;N否',
    extra_attribute_json    JSON                                    NULL COMMENT '擴充屬性 (例: 顏色代碼, 轉載圖示 URL)',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_cdi_category_code_locale UNIQUE (category_sid, item_code, item_locale),
    INDEX idx_cdi_category_sid (category_sid),
    INDEX idx_cdi_sort (sort_order),
    INDEX idx_cdi_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系統字典項目代碼明細檔';

-- =========================================================
-- 04. 組態異動歷程與快照 (Config History & Snapshots)
-- =========================================================

CREATE TABLE IF NOT EXISTS cfg_change_history (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '歷史紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '變更時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    config_type             VARCHAR(30)                         NOT NULL COMMENT '變更類別: SYSTEM_CONFIG, FEATURE_FLAG, DICTIONARY',
    target_sid              VARCHAR(32)                         NOT NULL COMMENT '目標實體序號 (例: config_sid / feature_sid)',
    before_value_json       JSON                                    NULL COMMENT '變更前快照 JSON',
    after_value_json        JSON                                NOT NULL COMMENT '變更後快照 JSON',
    operator_user_sid       VARCHAR(32)                             NULL COMMENT '操作人員帳號序號',
    operator_name           VARCHAR(100)                            NULL COMMENT '操作人員姓名',
    change_reason           VARCHAR(500)                            NULL COMMENT '變更原因/工單單號',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_cch_company_sid (company_sid),
    INDEX idx_cch_target (config_type, target_sid),
    INDEX idx_cch_operator (operator_user_sid),
    INDEX idx_cch_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='組態變更歷史與稽核快照檔 (Append-Only)';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}