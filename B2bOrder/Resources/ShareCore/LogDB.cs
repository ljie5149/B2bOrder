namespace B2bOrder.Resources.ShareCore
{
    /// <summary>
    /// LogDB V1 Shared Core Schema
    /// 設計目標：
    /// 1. 分散式鏈路追蹤 (Distributed Tracing Log)：紀錄跨微服務 API 呼叫鏈路 (Trace ID / Span ID) 與執行耗時
    /// 2. 應用程式系統異常日誌 (System Error & Exception Log)：歸檔系統 Unhandled Exception、Stack Trace 與 Critical 告警
    /// 3. 第三方 API 對接通訊日誌 (Third-Party Integration Log)：紀錄金流、物流、電子發票、簡訊等外部 API 的 Request/Response 原生 Payload
    /// 4. 訊息佇列與事件流日誌 (Message Queue & Event Log)：追蹤 RabbitMQ / Kafka 事件發布 (Publish) 與消費 (Consume) 狀態
    /// 5. 採用 Append-Only 唯寫架構設計 (不提供 UPDATE，優化高併發 Write-Heavy 效能)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；D刪除；W停用
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class LogDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 分散式鏈路追蹤日誌 (Distributed Tracing Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS log_trace_span (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Span 紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '紀錄時間',
    trace_id                VARCHAR(100)                        NOT NULL COMMENT '全域分散式追蹤碼 (Trace ID)',
    span_id                 VARCHAR(100)                        NOT NULL COMMENT '當前 Span 識別碼 (Span ID)',
    parent_span_id          VARCHAR(100)                            NULL COMMENT '父級 Span 識別碼 (Parent Span ID)',
    service_name            VARCHAR(50)                         NOT NULL COMMENT '微服務名稱 (例: SalesOrderDB, PaymentDB)',
    operation_name          VARCHAR(200)                        NOT NULL COMMENT '操作名稱/API 路徑 (例: POST /api/v1/orders/checkout)',
    span_kind               VARCHAR(20)                         NOT NULL DEFAULT 'SERVER' COMMENT 'Span 類型: SERVER, CLIENT, PRODUCER, CONSUMER',
    start_time              DATETIME(3)                         NOT NULL COMMENT '開始時間 (精確至毫秒)',
    end_time                DATETIME(3)                         NOT NULL COMMENT '結束時間 (精確至毫秒)',
    duration_ms             BIGINT                              NOT NULL DEFAULT 0 COMMENT '執行耗時 (毫秒)',
    http_status_code        INT                                     NULL COMMENT 'HTTP 回應碼 (例: 200, 500)',
    has_error               VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT 'Y發生錯誤;N正常',
    host_ip                 VARCHAR(45)                             NULL COMMENT '執行該服務之伺服器 IP',
    tags_json               JSON                                    NULL COMMENT '自訂標籤 (例: { customer_sid: ""CUST123"", order_sid: ""ORD456"" })',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_lts_trace_id (trace_id),
    INDEX idx_lts_service_op (service_name, operation_name),
    INDEX idx_lts_duration (duration_ms DESC),
    INDEX idx_lts_has_error (has_error),
    INDEX idx_lts_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='分散式鏈路追蹤檔 (Append-Only)';

-- =========================================================
-- 02. 應用程式系統異常日誌 (System Error & Exception Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS log_system_exception (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '異常紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '發生時間',
    company_sid             VARCHAR(32)                             NULL COMMENT '公司序號',
    service_name            VARCHAR(50)                         NOT NULL COMMENT '微服務名稱',
    environment             VARCHAR(20)                         NOT NULL DEFAULT 'PROD' COMMENT '環境代碼: DEV, STAGE, PROD',
    log_level               VARCHAR(20)                         NOT NULL DEFAULT 'ERROR' COMMENT '日誌級別: WARN, ERROR, FATAL, CRITICAL',
    exception_type          VARCHAR(250)                        NOT NULL COMMENT '例外類別名稱 (例: System.NullReferenceException, SqlException)',
    message                 TEXT                                NOT NULL COMMENT '錯誤訊息摘要',
    stack_trace             MEDIUMTEXT                              NULL COMMENT '完整程式碼 Stack Trace',
    request_uri             VARCHAR(500)                            NULL COMMENT '觸發例外之 API URI',
    http_method             VARCHAR(10)                             NULL COMMENT 'HTTP 動詞 (GET, POST 等)',
    operator_user_sid       VARCHAR(32)                             NULL COMMENT '操作人員帳號序號',
    client_ip               VARCHAR(45)                             NULL COMMENT '客戶端 IP',
    trace_id                VARCHAR(100)                            NULL COMMENT '關聯之 Trace ID',
    is_resolved             VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT 'Y已處置/已排查;N未處理',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_lse_service_level (service_name, log_level),
    INDEX idx_lse_exception_type (exception_type(50)),
    INDEX idx_lse_trace_id (trace_id),
    INDEX idx_lse_resolved (is_resolved),
    INDEX idx_lse_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系統應用程式異常日誌檔 (Append-Only)';

-- =========================================================
-- 03. 第三方對接通訊日誌 (Third-Party Integration Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS log_third_party_api (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '通訊紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '請求時間',
    company_sid             VARCHAR(32)                             NULL COMMENT '公司序號',
    provider_name           VARCHAR(50)                         NOT NULL COMMENT '第三方服務商名稱 (例: ECPAY, LINE_PAY, TAIWAN_MOBILE_SMS, EINVOICE)',
    api_action              VARCHAR(100)                        NOT NULL COMMENT '對接功能/API名稱 (例: CREATE_PAYMENT, SEND_SMS, ISSUE_INVOICE)',
    business_key            VARCHAR(100)                            NULL COMMENT '業務關聯 Key (例: 訂單號, 刷卡單號)',
    request_url             VARCHAR(500)                        NOT NULL COMMENT '呼叫第三方之 Target URL',
    http_method             VARCHAR(10)                         NOT NULL DEFAULT 'POST' COMMENT 'HTTP 方法',
    request_header_json     JSON                                    NULL COMMENT '標頭內容 Header (遮蔽敏感 Token/Secret)',
    request_payload         MEDIUMTEXT                              NULL COMMENT '送出之原始 Request Body',
    response_code           INT                                     NULL COMMENT 'HTTP 回應碼 (例: 200, 400)',
    response_payload        MEDIUMTEXT                              NULL COMMENT '回傳之原始 Response Body',
    execution_time_ms       BIGINT                              NOT NULL DEFAULT 0 COMMENT '通訊耗時 (毫秒)',
    is_success              VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y成功;N失敗',
    error_message           TEXT                                    NULL COMMENT '失敗原因說明',
    trace_id                VARCHAR(100)                            NULL COMMENT '分散式追蹤 Trace ID',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_ltp_provider_action (provider_name, api_action),
    INDEX idx_ltp_business_key (business_key),
    INDEX idx_ltp_success (is_success),
    INDEX idx_ltp_trace_id (trace_id),
    INDEX idx_ltp_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='第三方 API 通訊紀錄檔 (Append-Only)';

-- =========================================================
-- 04. 訊息佇列與事件流日誌 (Message Queue & Event Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS log_message_queue (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'MQ 紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '事件發生時間',
    company_sid             VARCHAR(32)                             NULL COMMENT '公司序號',
    exchange_or_topic       VARCHAR(100)                        NOT NULL COMMENT 'Exchange 或 Topic 名稱 (例: order.event.exchange)',
    routing_key             VARCHAR(100)                        NOT NULL COMMENT 'Routing Key 或 Subject (例: order.created)',
    queue_name              VARCHAR(100)                            NULL COMMENT 'Queue 名稱 (Consumer 側紀錄)',
    action_type             VARCHAR(20)                         NOT NULL COMMENT '動作類型: PUBLISH發布, CONSUME消費, DEAD_LETTER死信',
    message_body_json       JSON                                    NULL COMMENT 'MQ 訊息內文 (JSON Payload)',
    process_status          VARCHAR(20)                         NOT NULL DEFAULT 'SUCCESS' COMMENT '狀態: SUCCESS成功, FAILED失敗, RETRYING重試中',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '已重試次數',
    error_message           TEXT                                    NULL COMMENT '消費失敗原因',
    trace_id                VARCHAR(100)                            NULL COMMENT '分散式追蹤 Trace ID',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_lmq_topic_routing (exchange_or_topic, routing_key),
    INDEX idx_lmq_action_status (action_type, process_status),
    INDEX idx_lmq_trace_id (trace_id),
    INDEX idx_lmq_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='訊息佇列與事件驅動歷程檔 (Append-Only)';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
