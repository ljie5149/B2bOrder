namespace B2bOrder.Resources.ShareCore
{
    /// <summary>
    /// IntegrationDB Core Schema
    /// 規範：
    /// 1. 用於多資料庫、跨服務整合，不保存業務主資料
    /// 2. 採 Transactional Outbox / Inbox Pattern
    /// 3. 支援 API Idempotency、事件重試、Dead Letter 與同步檢查點
    /// 4. sid：Web / APP / API 對外識別碼
    /// 5. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class IntegrationDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 服務與事件定義
-- =========================================================

CREATE TABLE IF NOT EXISTS int_service_registry (
    nid                     INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '服務序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    service_code            VARCHAR(80)                         NOT NULL COMMENT '服務代碼',
    service_name            VARCHAR(150)                        NOT NULL COMMENT '服務名稱',
    service_url             VARCHAR(500)                            NULL COMMENT '服務網址',
    health_check_url        VARCHAR(500)                            NULL COMMENT '健康檢查網址',
    service_version         VARCHAR(50)                             NULL COMMENT '服務版本',
    timeout_seconds         INT                                 NOT NULL DEFAULT 30 COMMENT '逾時秒數',
    retry_count             INT                                 NOT NULL DEFAULT 3 COMMENT '預設重試次數',
    service_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE正常;MAINTENANCE維護;OFFLINE離線',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_isr_service_code UNIQUE (service_code),
    INDEX idx_isr_status (service_status),
    INDEX idx_isr_avalible (avalible),
    CHECK (timeout_seconds > 0),
    CHECK (retry_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='整合服務註冊表';

CREATE TABLE IF NOT EXISTS int_event_definition (
    nid                     INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '事件定義序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    event_code              VARCHAR(150)                        NOT NULL COMMENT '事件代碼',
    event_name              VARCHAR(200)                        NOT NULL COMMENT '事件名稱',
    aggregate_type          VARCHAR(100)                        NOT NULL COMMENT '聚合類型',
    source_service_code     VARCHAR(80)                         NOT NULL COMMENT '來源服務代碼',
    schema_version          VARCHAR(20)                         NOT NULL DEFAULT '1.0' COMMENT 'Payload Schema版本',
    payload_schema          JSON                                    NULL COMMENT '事件Payload Schema',
    retention_days          INT                                 NOT NULL DEFAULT 90 COMMENT '保留天數',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ied_event_code UNIQUE (event_code),
    INDEX idx_ied_source_service (source_service_code),
    INDEX idx_ied_aggregate_type (aggregate_type),
    INDEX idx_ied_avalible (avalible),
    CHECK (retention_days > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='整合事件定義';

-- =========================================================
-- 02. Transactional Outbox
-- =========================================================

CREATE TABLE IF NOT EXISTS int_outbox_event (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Outbox事件序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    event_id                CHAR(36)                            NOT NULL COMMENT '事件UUID',
    event_code              VARCHAR(150)                        NOT NULL COMMENT '事件代碼',
    aggregate_type          VARCHAR(100)                        NOT NULL COMMENT '聚合類型',
    aggregate_sid           VARCHAR(32)                         NOT NULL COMMENT '聚合資料序號',
    source_service_code     VARCHAR(80)                         NOT NULL COMMENT '來源服務代碼',
    correlation_id          VARCHAR(100)                            NULL COMMENT '關聯識別碼',
    causation_id            VARCHAR(100)                            NULL COMMENT '因果事件識別碼',
    trace_id                VARCHAR(100)                            NULL COMMENT '分散式追蹤識別碼',
    payload                 JSON                                NOT NULL COMMENT '事件內容',
    headers                 JSON                                    NULL COMMENT '事件標頭',
    event_version           INT                                 NOT NULL DEFAULT 1 COMMENT '事件版本',
    occurred_date           DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '業務事件時間',
    publish_status          VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待發送;PROCESSING發送中;PUBLISHED已發送;FAILED失敗;DEAD死信',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '重試次數',
    max_retry_count         INT                                 NOT NULL DEFAULT 10 COMMENT '最大重試次數',
    next_retry_date         DATETIME                                NULL COMMENT '下次重試時間',
    published_date          DATETIME                                NULL COMMENT '發送完成時間',
    lock_token              VARCHAR(100)                            NULL COMMENT '處理鎖定Token',
    lock_expiry_date        DATETIME                                NULL COMMENT '處理鎖定到期時間',
    last_error              TEXT                                    NULL COMMENT '最後錯誤訊息',
    CONSTRAINT uk_ioe_event_id UNIQUE (event_id),
    INDEX idx_ioe_publish_status (publish_status),
    INDEX idx_ioe_next_retry_date (next_retry_date),
    INDEX idx_ioe_aggregate (aggregate_type, aggregate_sid),
    INDEX idx_ioe_event_code (event_code),
    INDEX idx_ioe_source_service (source_service_code),
    INDEX idx_ioe_correlation_id (correlation_id),
    INDEX idx_ioe_trace_id (trace_id),
    INDEX idx_ioe_occurred_date (occurred_date),
    INDEX idx_ioe_lock_expiry (lock_expiry_date),
    CHECK (event_version > 0),
    CHECK (retry_count >= 0),
    CHECK (max_retry_count > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Transactional Outbox事件';

CREATE TABLE IF NOT EXISTS int_outbox_delivery (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Outbox派送序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    outbox_event_nid        BIGINT UNSIGNED                     NOT NULL COMMENT 'Outbox事件流水號',
    target_service_code     VARCHAR(80)                         NOT NULL COMMENT '目標服務代碼',
    endpoint_url            VARCHAR(500)                            NULL COMMENT '目標Endpoint',
    delivery_status         VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待送;PROCESSING處理中;SUCCESS成功;FAILED失敗;DEAD死信',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '重試次數',
    max_retry_count         INT                                 NOT NULL DEFAULT 10 COMMENT '最大重試次數',
    next_retry_date         DATETIME                                NULL COMMENT '下次重試時間',
    request_data            JSON                                    NULL COMMENT '送出內容',
    response_status_code    INT                                     NULL COMMENT '回應HTTP狀態碼',
    response_data           LONGTEXT                                NULL COMMENT '回應內容',
    delivered_date          DATETIME                                NULL COMMENT '派送成功時間',
    last_error              TEXT                                    NULL COMMENT '最後錯誤訊息',
    CONSTRAINT fk_iod_outbox_event
        FOREIGN KEY (outbox_event_nid) REFERENCES int_outbox_event(nid),
    CONSTRAINT uk_iod_event_target UNIQUE (outbox_event_nid, target_service_code),
    INDEX idx_iod_event_nid (outbox_event_nid),
    INDEX idx_iod_target_service (target_service_code),
    INDEX idx_iod_status (delivery_status),
    INDEX idx_iod_next_retry_date (next_retry_date),
    CHECK (retry_count >= 0),
    CHECK (max_retry_count > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Outbox事件目標服務派送';

-- =========================================================
-- 03. Inbox與事件消費
-- =========================================================

CREATE TABLE IF NOT EXISTS int_inbox_event (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Inbox事件序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    event_id                CHAR(36)                            NOT NULL COMMENT '來源事件UUID',
    event_code              VARCHAR(150)                        NOT NULL COMMENT '事件代碼',
    source_service_code     VARCHAR(80)                         NOT NULL COMMENT '來源服務代碼',
    target_service_code     VARCHAR(80)                         NOT NULL COMMENT '目標服務代碼',
    aggregate_type          VARCHAR(100)                        NOT NULL COMMENT '聚合類型',
    aggregate_sid           VARCHAR(32)                         NOT NULL COMMENT '聚合資料序號',
    correlation_id          VARCHAR(100)                            NULL COMMENT '關聯識別碼',
    trace_id                VARCHAR(100)                            NULL COMMENT '分散式追蹤識別碼',
    payload                 JSON                                NOT NULL COMMENT '事件內容',
    headers                 JSON                                    NULL COMMENT '事件標頭',
    received_date           DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '接收時間',
    process_status          VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;PROCESSING處理中;SUCCESS成功;FAILED失敗;DEAD死信;IGNORED忽略',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '重試次數',
    max_retry_count         INT                                 NOT NULL DEFAULT 10 COMMENT '最大重試次數',
    next_retry_date         DATETIME                                NULL COMMENT '下次重試時間',
    processed_date          DATETIME                                NULL COMMENT '處理完成時間',
    lock_token              VARCHAR(100)                            NULL COMMENT '處理鎖定Token',
    lock_expiry_date        DATETIME                                NULL COMMENT '處理鎖定到期時間',
    last_error              TEXT                                    NULL COMMENT '最後錯誤訊息',
    CONSTRAINT uk_iie_target_event UNIQUE (target_service_code, event_id),
    INDEX idx_iie_process_status (process_status),
    INDEX idx_iie_next_retry_date (next_retry_date),
    INDEX idx_iie_event_code (event_code),
    INDEX idx_iie_source_service (source_service_code),
    INDEX idx_iie_target_service (target_service_code),
    INDEX idx_iie_aggregate (aggregate_type, aggregate_sid),
    INDEX idx_iie_trace_id (trace_id),
    INDEX idx_iie_received_date (received_date),
    INDEX idx_iie_lock_expiry (lock_expiry_date),
    CHECK (retry_count >= 0),
    CHECK (max_retry_count > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Inbox事件與冪等消費紀錄';

CREATE TABLE IF NOT EXISTS int_event_handler_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '事件處理紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    inbox_event_nid         BIGINT UNSIGNED                     NOT NULL COMMENT 'Inbox事件流水號',
    handler_name            VARCHAR(200)                        NOT NULL COMMENT '事件處理器名稱',
    start_date              DATETIME                            NOT NULL COMMENT '開始時間',
    end_date                DATETIME                                NULL COMMENT '結束時間',
    elapsed_ms              INT                                 NOT NULL DEFAULT 0 COMMENT '處理耗時毫秒',
    handler_status          VARCHAR(20)                         NOT NULL COMMENT 'SUCCESS成功;FAILED失敗;SKIPPED略過',
    result_data             JSON                                    NULL COMMENT '處理結果',
    error_message           TEXT                                    NULL COMMENT '錯誤訊息',
    CONSTRAINT fk_iehl_inbox_event
        FOREIGN KEY (inbox_event_nid) REFERENCES int_inbox_event(nid),
    INDEX idx_iehl_inbox_event_nid (inbox_event_nid),
    INDEX idx_iehl_handler_name (handler_name),
    INDEX idx_iehl_status (handler_status),
    INDEX idx_iehl_create_date (create_date),
    CHECK (elapsed_ms >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='事件處理器執行紀錄';

-- =========================================================
-- 04. Dead Letter
-- =========================================================

CREATE TABLE IF NOT EXISTS int_dead_letter (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '死信序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    source_type             VARCHAR(20)                         NOT NULL COMMENT 'OUTBOX;INBOX;API;SYNC',
    source_sid              VARCHAR(32)                         NOT NULL COMMENT '來源資料序號',
    event_code              VARCHAR(150)                            NULL COMMENT '事件代碼',
    service_code            VARCHAR(80)                             NULL COMMENT '相關服務代碼',
    payload                 JSON                                    NULL COMMENT '失敗內容',
    error_message           LONGTEXT                            NOT NULL COMMENT '錯誤訊息',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '累計重試次數',
    dead_status             VARCHAR(20)                         NOT NULL DEFAULT 'OPEN' COMMENT 'OPEN待處理;REQUEUED已重新排程;RESOLVED已解決;IGNORED忽略',
    resolved_user_sid       VARCHAR(32)                             NULL COMMENT '處理人員序號',
    resolved_date           DATETIME                                NULL COMMENT '解決時間',
    resolution_note         TEXT                                    NULL COMMENT '處理說明',
    INDEX idx_idl_source (source_type, source_sid),
    INDEX idx_idl_event_code (event_code),
    INDEX idx_idl_service_code (service_code),
    INDEX idx_idl_status (dead_status),
    INDEX idx_idl_create_date (create_date),
    CHECK (retry_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='整合死信資料';

-- =========================================================
-- 05. API冪等與跨服務呼叫
-- =========================================================

CREATE TABLE IF NOT EXISTS int_idempotency_key (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '冪等紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    idempotency_key         VARCHAR(150)                        NOT NULL COMMENT '冪等Key',
    service_code            VARCHAR(80)                         NOT NULL COMMENT '服務代碼',
    operation_code          VARCHAR(150)                        NOT NULL COMMENT '操作代碼',
    request_hash            VARCHAR(128)                        NOT NULL COMMENT 'Request內容雜湊',
    request_data            JSON                                    NULL COMMENT 'Request內容',
    response_status_code    INT                                     NULL COMMENT '回應狀態碼',
    response_data           LONGTEXT                                NULL COMMENT '回應內容',
    process_status          VARCHAR(20)                         NOT NULL DEFAULT 'PROCESSING' COMMENT 'PROCESSING處理中;SUCCESS成功;FAILED失敗',
    expiry_date             DATETIME                            NOT NULL COMMENT '冪等紀錄到期時間',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    CONSTRAINT uk_iik_service_key UNIQUE (service_code, idempotency_key),
    INDEX idx_iik_operation_code (operation_code),
    INDEX idx_iik_status (process_status),
    INDEX idx_iik_expiry_date (expiry_date),
    CHECK (expiry_date > create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='API冪等請求紀錄';

CREATE TABLE IF NOT EXISTS int_api_call_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'API呼叫紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '呼叫時間',
    request_id              VARCHAR(100)                        NOT NULL COMMENT '請求識別碼',
    correlation_id          VARCHAR(100)                            NULL COMMENT '關聯識別碼',
    trace_id                VARCHAR(100)                            NULL COMMENT '追蹤識別碼',
    source_service_code     VARCHAR(80)                         NOT NULL COMMENT '來源服務',
    target_service_code     VARCHAR(80)                         NOT NULL COMMENT '目標服務',
    method                  VARCHAR(10)                         NOT NULL COMMENT 'HTTP Method',
    endpoint_url            VARCHAR(1000)                       NOT NULL COMMENT '呼叫網址',
    request_headers         JSON                                    NULL COMMENT 'Request Headers',
    request_body            LONGTEXT                                NULL COMMENT 'Request Body',
    response_status_code    INT                                     NULL COMMENT 'HTTP狀態碼',
    response_body           LONGTEXT                                NULL COMMENT 'Response Body',
    elapsed_ms              INT                                 NOT NULL DEFAULT 0 COMMENT '耗時毫秒',
    call_status             VARCHAR(20)                         NOT NULL COMMENT 'SUCCESS成功;FAILED失敗;TIMEOUT逾時',
    error_message           TEXT                                    NULL COMMENT '錯誤訊息',
    INDEX idx_iacl_request_id (request_id),
    INDEX idx_iacl_correlation_id (correlation_id),
    INDEX idx_iacl_trace_id (trace_id),
    INDEX idx_iacl_source_target (source_service_code, target_service_code),
    INDEX idx_iacl_create_date (create_date),
    INDEX idx_iacl_call_status (call_status),
    CHECK (elapsed_ms >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='跨服務API呼叫紀錄';

-- =========================================================
-- 06. 資料同步
-- =========================================================

CREATE TABLE IF NOT EXISTS int_sync_definition (
    nid                     INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '同步定義序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    sync_code               VARCHAR(100)                        NOT NULL COMMENT '同步代碼',
    sync_name               VARCHAR(200)                        NOT NULL COMMENT '同步名稱',
    source_service_code     VARCHAR(80)                         NOT NULL COMMENT '來源服務',
    target_service_code     VARCHAR(80)                         NOT NULL COMMENT '目標服務',
    entity_type             VARCHAR(100)                        NOT NULL COMMENT '同步實體類型',
    sync_mode               VARCHAR(30)                         NOT NULL DEFAULT 'INCREMENTAL' COMMENT 'FULL全量;INCREMENTAL增量;EVENT事件',
    schedule_expression     VARCHAR(100)                            NULL COMMENT '排程表示式',
    batch_size              INT                                 NOT NULL DEFAULT 500 COMMENT '每批筆數',
    max_retry_count         INT                                 NOT NULL DEFAULT 5 COMMENT '最大重試次數',
    sync_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;PAUSED暫停;DISABLED停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_isd_sync_code UNIQUE (sync_code),
    INDEX idx_isd_source_target (source_service_code, target_service_code),
    INDEX idx_isd_entity_type (entity_type),
    INDEX idx_isd_status (sync_status),
    INDEX idx_isd_avalible (avalible),
    CHECK (batch_size > 0),
    CHECK (max_retry_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='跨服務資料同步定義';

CREATE TABLE IF NOT EXISTS int_sync_checkpoint (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '同步檢查點序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    sync_definition_nid     INT UNSIGNED                        NOT NULL COMMENT '同步定義流水號',
    checkpoint_type         VARCHAR(30)                         NOT NULL COMMENT 'TIMESTAMP時間;SEQUENCE序號;CURSOR游標;VERSION版本',
    checkpoint_value        VARCHAR(1000)                       NOT NULL COMMENT '檢查點值',
    last_success_date       DATETIME                                NULL COMMENT '最後成功時間',
    last_record_count       INT                                 NOT NULL DEFAULT 0 COMMENT '最後同步筆數',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '版本號',
    CONSTRAINT fk_isc_sync_definition
        FOREIGN KEY (sync_definition_nid) REFERENCES int_sync_definition(nid),
    CONSTRAINT uk_isc_sync_definition UNIQUE (sync_definition_nid),
    INDEX idx_isc_last_success_date (last_success_date),
    CHECK (last_record_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='資料同步檢查點';

CREATE TABLE IF NOT EXISTS int_sync_job (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '同步工作序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    sync_definition_nid     INT UNSIGNED                        NOT NULL COMMENT '同步定義流水號',
    job_no                  VARCHAR(80)                         NOT NULL COMMENT '同步工作編號',
    start_date              DATETIME                            NOT NULL COMMENT '開始時間',
    end_date                DATETIME                                NULL COMMENT '結束時間',
    checkpoint_before       VARCHAR(1000)                           NULL COMMENT '執行前檢查點',
    checkpoint_after        VARCHAR(1000)                           NULL COMMENT '執行後檢查點',
    read_count              INT                                 NOT NULL DEFAULT 0 COMMENT '讀取筆數',
    success_count           INT                                 NOT NULL DEFAULT 0 COMMENT '成功筆數',
    fail_count              INT                                 NOT NULL DEFAULT 0 COMMENT '失敗筆數',
    skip_count              INT                                 NOT NULL DEFAULT 0 COMMENT '略過筆數',
    job_status              VARCHAR(20)                         NOT NULL DEFAULT 'RUNNING' COMMENT 'RUNNING執行中;SUCCESS成功;PARTIAL部分成功;FAILED失敗;CANCELLED取消',
    error_message           LONGTEXT                                NULL COMMENT '錯誤訊息',
    CONSTRAINT fk_isj_sync_definition
        FOREIGN KEY (sync_definition_nid) REFERENCES int_sync_definition(nid),
    CONSTRAINT uk_isj_job_no UNIQUE (job_no),
    INDEX idx_isj_sync_definition_nid (sync_definition_nid),
    INDEX idx_isj_start_date (start_date),
    INDEX idx_isj_status (job_status),
    CHECK (read_count >= 0),
    CHECK (success_count >= 0),
    CHECK (fail_count >= 0),
    CHECK (skip_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='資料同步工作';

CREATE TABLE IF NOT EXISTS int_sync_error (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '同步錯誤序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    sync_job_nid            BIGINT UNSIGNED                     NOT NULL COMMENT '同步工作流水號',
    source_record_sid       VARCHAR(32)                             NULL COMMENT '來源資料序號',
    target_record_sid       VARCHAR(32)                             NULL COMMENT '目標資料序號',
    operation_type          VARCHAR(20)                         NOT NULL COMMENT 'INSERT;UPDATE;DELETE;VALIDATE',
    source_data             JSON                                    NULL COMMENT '來源資料',
    error_code              VARCHAR(100)                            NULL COMMENT '錯誤代碼',
    error_message           LONGTEXT                            NOT NULL COMMENT '錯誤訊息',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '重試次數',
    error_status            VARCHAR(20)                         NOT NULL DEFAULT 'OPEN' COMMENT 'OPEN未處理;RETRIED已重試;RESOLVED已解決;IGNORED忽略',
    resolved_date           DATETIME                                NULL COMMENT '解決時間',
    CONSTRAINT fk_ise_sync_job
        FOREIGN KEY (sync_job_nid) REFERENCES int_sync_job(nid),
    INDEX idx_ise_sync_job_nid (sync_job_nid),
    INDEX idx_ise_source_record_sid (source_record_sid),
    INDEX idx_ise_operation_type (operation_type),
    INDEX idx_ise_status (error_status),
    INDEX idx_ise_create_date (create_date),
    CHECK (retry_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='資料同步錯誤';

-- =========================================================
-- 07. Saga流程協調
-- =========================================================

CREATE TABLE IF NOT EXISTS int_saga_instance (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Saga流程序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    saga_type               VARCHAR(100)                        NOT NULL COMMENT 'Saga流程類型',
    business_sid            VARCHAR(32)                         NOT NULL COMMENT '主要業務資料序號',
    correlation_id          VARCHAR(100)                        NOT NULL COMMENT '關聯識別碼',
    current_step            VARCHAR(100)                            NULL COMMENT '目前步驟',
    saga_status             VARCHAR(20)                         NOT NULL DEFAULT 'RUNNING' COMMENT 'RUNNING執行中;COMPLETED完成;COMPENSATING補償中;COMPENSATED已補償;FAILED失敗',
    state_data              JSON                                    NULL COMMENT 'Saga狀態資料',
    started_date            DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '開始時間',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    last_error              TEXT                                    NULL COMMENT '最後錯誤',
    CONSTRAINT uk_isi_correlation_id UNIQUE (correlation_id),
    INDEX idx_isi_saga_type (saga_type),
    INDEX idx_isi_business_sid (business_sid),
    INDEX idx_isi_status (saga_status),
    INDEX idx_isi_started_date (started_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='分散式Saga流程';

CREATE TABLE IF NOT EXISTS int_saga_step (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Saga步驟序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    saga_instance_nid       BIGINT UNSIGNED                     NOT NULL COMMENT 'Saga流程流水號',
    step_no                 INT                                 NOT NULL COMMENT '步驟順序',
    step_code               VARCHAR(100)                        NOT NULL COMMENT '步驟代碼',
    service_code            VARCHAR(80)                         NOT NULL COMMENT '執行服務代碼',
    command_name            VARCHAR(200)                        NOT NULL COMMENT '執行命令',
    compensation_command    VARCHAR(200)                            NULL COMMENT '補償命令',
    request_data            JSON                                    NULL COMMENT '命令內容',
    response_data           JSON                                    NULL COMMENT '回應內容',
    step_status             VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待執行;RUNNING執行中;SUCCESS成功;FAILED失敗;COMPENSATED已補償',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '重試次數',
    started_date            DATETIME                                NULL COMMENT '開始時間',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    last_error              TEXT                                    NULL COMMENT '最後錯誤',
    CONSTRAINT fk_iss_saga_instance
        FOREIGN KEY (saga_instance_nid) REFERENCES int_saga_instance(nid),
    CONSTRAINT uk_iss_saga_step UNIQUE (saga_instance_nid, step_no),
    INDEX idx_iss_saga_instance_nid (saga_instance_nid),
    INDEX idx_iss_step_code (step_code),
    INDEX idx_iss_service_code (service_code),
    INDEX idx_iss_status (step_status),
    CHECK (step_no > 0),
    CHECK (retry_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Saga流程步驟';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
