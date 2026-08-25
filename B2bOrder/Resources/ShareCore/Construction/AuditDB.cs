namespace B2bOrder.Resources.ShareCore.Construction
{
    /// <summary>
    /// AuditDB V1 Shared Core Schema
    /// 設計目標：
    /// 1. 實體資料變更稽核 (Entity Data Change Log)：紀錄關鍵資料異動前後快照 (Before / After JSON Snapshots)
    /// 2. 使用者操作與 API 呼叫稽核 (Operation & Access Audit Log)：紀錄 API 請求參數、回應狀態與執行耗時
    /// 3. 身分驗證與安全事件稽核 (Authentication & Security Event Log)：紀錄登入成功/失敗、帳號鎖定、權限變更
    /// 4. 敏感資料檢視與匯出稽核 (Sensitive Data Access Log)：符合 GDPR / PII / 金融稽核要求，紀錄個資閱讀與匯出軌跡
    /// 5. 採用 Append-Only 唯寫設計 (非必要不提供 UPDATE 操作，確保稽核不可竄改性)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；D刪除；W停用
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class AuditDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 實體資料變更歷程稽核 (Data Change Audit / CDC Snapshot)
-- =========================================================

CREATE TABLE IF NOT EXISTS adt_data_change_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '資料變更紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '變更發生時間',
    company_sid             VARCHAR(32)                             NULL COMMENT '公司序號',
    service_name            VARCHAR(50)                         NOT NULL COMMENT '來源微服務名稱 (例: SalesOrderDB, CustomerDB)',
    table_name              VARCHAR(100)                        NOT NULL COMMENT '變更的資料表名稱 (例: ord_sales_order)',
    entity_sid              VARCHAR(32)                         NOT NULL COMMENT '變更的資料列/實體序號 (Entity SID)',
    action_type             VARCHAR(20)                         NOT NULL COMMENT '動作類型: INSERT, UPDATE, DELETE, SOFT_DELETE',
    before_data_json        JSON                                    NULL COMMENT '變更前的完整資料快照 (JSON)',
    after_data_json         JSON                                    NULL COMMENT '變更後的完整資料快照 (JSON)',
    changed_fields_json     JSON                                    NULL COMMENT '差異欄位清單 (Diff Fields Array)',
    operator_user_sid       VARCHAR(32)                             NULL COMMENT '操作人員帳號序號 (系統自動執行則為 NULL)',
    operator_name           VARCHAR(100)                            NULL COMMENT '操作人員姓名/系統服務代碼',
    trace_id                VARCHAR(100)                            NULL COMMENT '微服務分散式追蹤碼 (Distributed Trace ID)',
    remark                  TEXT                                    NULL COMMENT '備註/變更原因說明',
    INDEX idx_adcl_service_table (service_name, table_name),
    INDEX idx_adcl_entity_sid (entity_sid),
    INDEX idx_adcl_company_sid (company_sid),
    INDEX idx_adcl_operator (operator_user_sid),
    INDEX idx_adcl_trace_id (trace_id),
    INDEX idx_adcl_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='實體資料變更與 Diff 快照稽核檔 (Append-Only)';

-- =========================================================
-- 02. 操作行為與 API 存取稽核 (Operation & Access Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS adt_operation_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '操作紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '操作時間',
    company_sid             VARCHAR(32)                             NULL COMMENT '公司序號',
    service_name            VARCHAR(50)                         NOT NULL COMMENT '服務名稱 (例: AdminPortal, MobileAPI)',
    module_name             VARCHAR(100)                            NULL COMMENT '模組/功能名稱 (例: OrderManagement, MemberProfile)',
    action_title            VARCHAR(200)                        NOT NULL COMMENT '操作動作簡述 (例: 修改訂單金額, 匯出會員清單)',
    http_method             VARCHAR(10)                         NOT NULL COMMENT 'HTTP 動詞 (GET, POST, PUT, DELETE)',
    request_uri             VARCHAR(500)                        NOT NULL COMMENT '請求 URI 網址',
    request_params_json     JSON                                    NULL COMMENT '請求參數 JSON (已屏蔽敏感關鍵字)',
    response_code           INT                                 NOT NULL COMMENT 'HTTP 狀態碼或業務回應碼 (例: 200, 403, 500)',
    execution_time_ms       BIGINT                              NOT NULL DEFAULT 0 COMMENT '執行耗時 (毫秒)',
    operator_user_sid       VARCHAR(32)                             NULL COMMENT '操作人員序號',
    operator_ip             VARCHAR(45)                             NULL COMMENT '操作者 IP 位址',
    user_agent              VARCHAR(500)                            NULL COMMENT '瀏覽器/客戶端 User-Agent',
    trace_id                VARCHAR(100)                            NULL COMMENT '分散式追蹤 Trace ID',
    remark                  TEXT                                    NULL COMMENT '備註/例外錯誤訊息',
    INDEX idx_aol_service_module (service_name, module_name),
    INDEX idx_aol_operator (operator_user_sid),
    INDEX idx_aol_response_code (response_code),
    INDEX idx_aol_trace_id (trace_id),
    INDEX idx_aol_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='使用者操作與 API 存取日誌檔 (Append-Only)';

-- =========================================================
-- 03. 身分驗證與安全事件稽核 (Authentication & Security Event)
-- =========================================================

CREATE TABLE IF NOT EXISTS adt_security_event (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '安全事件序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '事件發生時間',
    company_sid             VARCHAR(32)                             NULL COMMENT '公司序號',
    user_sid                VARCHAR(32)                             NULL COMMENT '關聯帳號/會員序號',
    account_identifier      VARCHAR(150)                        NOT NULL COMMENT '嘗試登入之帳號/Email/手機',
    event_type              VARCHAR(50)                         NOT NULL COMMENT '事件類型: LOGIN_SUCCESS, LOGIN_FAILED, LOGOUT, PASSWORD_CHANGE, MFA_CHALLENGE_FAILED, ACCOUNT_LOCKED, PERMISSION_DENIED',
    severity_level          VARCHAR(20)                         NOT NULL DEFAULT 'INFO' COMMENT '嚴重程度: INFO一般, WARNING警告, ERROR高風險, CRITICAL極度危險',
    client_ip               VARCHAR(45)                         NOT NULL COMMENT '來源 IP 位址',
    location_geo            VARCHAR(100)                            NULL COMMENT '根據 IP 推算的地理位置 (城市/國家)',
    user_agent              VARCHAR(500)                            NULL COMMENT '客戶端裝置資訊',
    failure_reason          VARCHAR(200)                            NULL COMMENT '失敗或安全警告原因',
    trace_id                VARCHAR(100)                            NULL COMMENT '分散式追蹤 Trace ID',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_ase_user_sid (user_sid),
    INDEX idx_ase_account (account_identifier),
    INDEX idx_ase_event_type (event_type),
    INDEX idx_ase_severity (severity_level),
    INDEX idx_ase_client_ip (client_ip),
    INDEX idx_ase_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='身分驗證與安全事件稽核檔 (Append-Only)';

-- =========================================================
-- 04. 敏感個資檢視與匯出稽核 (PII / Sensitive Data Access)
-- =========================================================

CREATE TABLE IF NOT EXISTS adt_sensitive_access_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '敏感存取紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '存取時間',
    company_sid             VARCHAR(32)                             NULL COMMENT '公司序號',
    operator_user_sid       VARCHAR(32)                         NOT NULL COMMENT '調閱/存取資料之操作人員序號',
    data_type               VARCHAR(50)                         NOT NULL COMMENT '敏感資料類別: CUSTOMER_PII(個資), CREDIT_CARD(卡號), FINANCIAL_REPORT(財務報表), SALARY(薪資)',
    access_action           VARCHAR(20)                         NOT NULL DEFAULT 'VIEW' COMMENT '存取行為: VIEW檢視單筆, SEARCH查詢列表, EXPORT批次匯出, PRINT列印',
    target_entity_sids_json JSON                                    NULL COMMENT '被調閱的標的實體序號陣列 (例: [""CUST_001"", ""CUST_002""])',
    record_count            INT                                 NOT NULL DEFAULT 1 COMMENT '影響/讀取的資料筆數',
    query_condition_json    JSON                                    NULL COMMENT '匯出或查詢時所使用的篩選條件',
    justification_reason    VARCHAR(500)                            NULL COMMENT '調閱敏感資料之申請原因/工單號碼',
    client_ip               VARCHAR(45)                         NOT NULL COMMENT '操作者 IP 位址',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_asal_operator (operator_user_sid),
    INDEX idx_asal_data_type (data_type),
    INDEX idx_asal_access_action (access_action),
    INDEX idx_asal_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='敏感個資與高風險數據檢視/匯出稽核檔 (Append-Only)';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}