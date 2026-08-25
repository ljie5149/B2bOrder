namespace B2bOrder.Resources.ShareCore
{
    /// <summary>
    /// SchedulerDB V1 Shared Core Schema
    /// 設計目標：
    /// 1. 分散式排程任務定義 (Cron Job Definition)：維護系統例行性排程 Cron 運算式、執行微服務端點與參數
    /// 2. 分散式任務執行鎖 (Distributed Task Lock)：防止微服務多節點 (Multi-Instance) 重複執行同一個排程
    /// 3. 排程執行歷程日誌 (Job Execution History Log)：紀錄每次排程執行的起訖時間、執行狀態、耗時與 Error Log
    /// 4. 延遲與一次性異步任務 (Delayed & One-off Tasks)：處理特定時間點執行的訂單逾期取消、推播提醒等任務
    /// 5. 整合與串接各微服務 (SalesOrderDB, PIM, NotificationDB, SearchDB) 之異步批次排程
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class SchedulerDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 分散式排程任務主檔 (Cron Job Master Definition)
-- =========================================================

CREATE TABLE IF NOT EXISTS sch_job_definition (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '任務序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    job_code                VARCHAR(100)                        NOT NULL COMMENT '任務唯一代碼 (例: JOB_CANCEL_EXPIRED_ORDERS)',
    job_group               VARCHAR(50)                         NOT NULL DEFAULT 'DEFAULT' COMMENT '任務分組 (例: ORDER, SYSTEM, SEARCH, INVENTORY)',
    job_name                VARCHAR(150)                        NOT NULL COMMENT '任務名稱 (例: 自動取消逾期未付款訂單)',
    cron_expression         VARCHAR(100)                        NOT NULL COMMENT 'Cron 觸發運算式 (例: 0 0/15 * * * ?)',
    target_service          VARCHAR(50)                         NOT NULL COMMENT '目標微服務名稱 (例: SalesOrderDB, SearchDB)',
    target_endpoint         VARCHAR(255)                        NOT NULL COMMENT '呼叫 API 端點或 Message Queue Topic',
    http_method             VARCHAR(10)                         NOT NULL DEFAULT 'POST' COMMENT 'HTTP 動詞 (POST, GET, PUT)',
    payload_json            JSON                                    NULL COMMENT '執行帶入參數 (JSON)',
    concurrent_allowed      VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT 'Y允許並行執行;N單一實例執行',
    max_retry_count         INT                                 NOT NULL DEFAULT 3 COMMENT '失敗最大重試次數',
    timeout_seconds         INT                                 NOT NULL DEFAULT 3600 COMMENT '任務逾時時間(秒)',
    job_status              VARCHAR(20)                         NOT NULL DEFAULT 'PAUSED' COMMENT 'SCHEDULED排程中;PAUSED已暫停;RUNNING執行中',
    last_executed_at        DATETIME                                NULL COMMENT '最後一次執行時間',
    next_fire_time          DATETIME                                NULL COMMENT '下一次預計觸發時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註/任務說明',
    CONSTRAINT uk_sjd_company_job UNIQUE (company_sid, job_code),
    INDEX idx_sjd_company_sid (company_sid),
    INDEX idx_sjd_job_group (job_group),
    INDEX idx_sjd_status (job_status),
    INDEX idx_sjd_next_fire (next_fire_time),
    INDEX idx_sjd_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='排程任務定義主檔';

-- =========================================================
-- 02. 分散式任務執行鎖 (Distributed Job Lock)
-- =========================================================

CREATE TABLE IF NOT EXISTS sch_job_lock (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '鎖定序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    job_code                VARCHAR(100)                        NOT NULL UNIQUE COMMENT '任務代碼',
    locked_by_node          VARCHAR(100)                        NOT NULL COMMENT '取得排程鎖之微服務 Node/IP 識別名稱',
    locked_at               DATETIME                            NOT NULL COMMENT '上鎖時間',
    lock_expired_at         DATETIME                            NOT NULL COMMENT '鎖過期時間 (防止 Deadlock)',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    INDEX idx_sjl_expired_at (lock_expired_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='分散式排程搶佔與搶鎖檔';

-- =========================================================
-- 03. 排程執行歷程日誌 (Job Execution History)
-- =========================================================

CREATE TABLE IF NOT EXISTS sch_execution_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '執行紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '觸發時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    job_code                VARCHAR(100)                        NOT NULL COMMENT '任務代碼',
    executed_by_node        VARCHAR(100)                        NOT NULL COMMENT '執行該任務之 Node 實例',
    start_time              DATETIME                            NOT NULL COMMENT '實際開始時間',
    end_time                DATETIME                                NULL COMMENT '實際結束時間',
    execution_time_ms       BIGINT                                  NULL COMMENT '執行耗時 (毫秒)',
    execution_status        VARCHAR(20)                         NOT NULL DEFAULT 'RUNNING' COMMENT 'RUNNING執行中;SUCCESS成功;FAILED失敗;TIMEOUT逾時;SKIPPED跳過',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '已重試次數',
    affected_rows           INT                                 NOT NULL DEFAULT 0 COMMENT '影響/處理資料筆數',
    result_message          TEXT                                    NULL COMMENT '執行結果簡述/回傳內容',
    error_stack_trace       TEXT                                    NULL COMMENT '失敗時之 Error Stack Trace',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_sel_company_sid (company_sid),
    INDEX idx_sel_job_code (job_code),
    INDEX idx_sel_status (execution_status),
    INDEX idx_sel_start_time (start_time),
    INDEX idx_sel_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='排程任務執行歷程檔 (Append-Only)';

-- =========================================================
-- 04. 延遲與一次性異步任務 (Delayed & One-off Tasks)
-- =========================================================

CREATE TABLE IF NOT EXISTS sch_delayed_task (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '延遲任務序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    task_type               VARCHAR(50)                         NOT NULL COMMENT '任務類型 (例: ORDER_PAYMENT_TIMEOUT, EDM_DELAYED_SEND)',
    business_key            VARCHAR(100)                        NOT NULL COMMENT '業務關聯 Key (例: 訂單序號 Order SID)',
    scheduled_execute_time  DATETIME                            NOT NULL COMMENT '預計執行時間',
    target_service          VARCHAR(50)                         NOT NULL COMMENT '目標微服務名稱',
    target_endpoint         VARCHAR(255)                        NOT NULL COMMENT '呼叫端點/Topic',
    payload_json            JSON                                    NULL COMMENT '任務參數 (JSON)',
    task_status             VARCHAR(20)                         NOT NULL DEFAULT 'WAITING' COMMENT 'WAITING等待執行;PROCESSING處理中;COMPLETED已完成;FAILED失敗;CANCELLED已取消',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '已重試次數',
    max_retry_count         INT                                 NOT NULL DEFAULT 3 COMMENT '最大重試次數',
    executed_at             DATETIME                                NULL COMMENT '實際完成時間',
    error_message           TEXT                                    NULL COMMENT '失敗訊息',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_sdt_task_business UNIQUE (company_sid, task_type, business_key),
    INDEX idx_sdt_company_sid (company_sid),
    INDEX idx_sdt_execute_time (scheduled_execute_time, task_status),
    INDEX idx_sdt_status (task_status),
    INDEX idx_sdt_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='一次性與延遲異步任務佇列檔';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}