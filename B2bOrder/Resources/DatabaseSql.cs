namespace B2bOrder.Resources
{
    internal static class DatabaseSql
    {
        // 將完整建表 SQL 放在這裡；注意使用 verbatim string (@"...") 避免轉譯問題
        public static readonly string CreateTables = @"
CREATE TABLE IF NOT EXISTS data_member (
    nid                     INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '會員序號',
    create_date             DATETIME                            NOT NULL        COMMENT '建立日期',
    modify_date             DATETIME                                NULL        COMMENT '最後更新日期',
    register_status         VARCHAR(20) DEFAULT 'UNREGISTER'                    COMMENT 'UNREGISTER未註冊;REGISTERED已註冊',
    parent_sid              VARCHAR(32)                             NULL        COMMENT '上線會員序號',
    mid                     VARCHAR(20)                         NOT NULL UNIQUE COMMENT '會員編號',
    join_date               DATETIME                                NULL        COMMENT '加入日期',
    continue_date           DATETIME                                NULL        COMMENT '續約日期',
    real_continue_date      DATETIME                                NULL        COMMENT '實際續約日期',
    hint_days               INT         DEFAULT 30              NOT NULL        COMMENT '提前通知天數',
    birthday                DATETIME                                NULL        COMMENT '生日',
    name                    VARCHAR(50)                             NULL        COMMENT '姓名',
    eng_name                VARCHAR(50)                             NULL        COMMENT '英文姓名',
    head_img                VARCHAR(255)                            NULL        COMMENT '頭像',
    iden                    VARCHAR(255)                            NULL        COMMENT '身份證字號(加密)',
    cmp_code                VARCHAR(10)                             NULL        COMMENT '統一編號',
    role                    VARCHAR(3)                          NOT NULL        COMMENT 'Stc一般會員;Smn直銷商;Ctl管理員',
    authorization_page      TEXT                                    NULL        COMMENT '授權頁面',
    address                 VARCHAR(500)                            NULL        COMMENT '地址',
    mobile                  VARCHAR(20)                             NULL        COMMENT '手機',
    tel                     VARCHAR(20)                             NULL        COMMENT '電話',
    fax                     VARCHAR(20)                             NULL        COMMENT '傳真',
    line_user_id            VARCHAR(100)                            NULL        COMMENT 'LINE會員Id',
    email                   VARCHAR(100)                            NULL        COMMENT '電子信箱',
    avalible                VARCHAR(2)                          NOT NULL        DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    start_date              DATETIME                                NULL        COMMENT '生效日期',
    signature_pic           VARCHAR(255)                            NULL        COMMENT '簽名圖',
    advertising_id          VARCHAR(200)                            NULL        COMMENT '廣告ID',
    device_id               VARCHAR(200)                            NULL        COMMENT '裝置ID',
    fcm_token               TEXT                                    NULL        COMMENT 'FCM推播Token',
    priority                INT                                     NULL        COMMENT '權限等級',
    level_name              VARCHAR(50)                             NULL        COMMENT '會員等級',
    notice_enable           VARCHAR(2)  DEFAULT 'Y'                             COMMENT '是否接收通知',
    last_login_date         DATETIME                                NULL        COMMENT '最後登入時間',
    register_source         VARCHAR(20)                             NULL        COMMENT 'WEB;APP;ADMIN;IMPORT',
    edit_sid                VARCHAR(32)                             NULL        COMMENT '最後編輯人員',
    cur_coupon              INT         DEFAULT 0                               COMMENT '目前折價券',
    cur_point               INT         DEFAULT 0                               COMMENT '目前點數',
    script                  TEXT                                    NULL        COMMENT '說明',
    remark                  TEXT                                    NULL        COMMENT '備註'
) COMMENT='會員資料表';

CREATE TABLE IF NOT EXISTS sys_user (
    nid                         INT         UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)             NOT NULL UNIQUE COMMENT '帳號序號',
    create_date                 DATETIME                NOT NULL COMMENT '建立日期',
    modify_date                 DATETIME                    NULL COMMENT '修改日期',
    account                     VARCHAR(50)             NOT NULL UNIQUE COMMENT '登入帳號',
    pwd                         VARCHAR(255)            NOT NULL COMMENT '登入密碼(加密)',
    password_reset_token        VARCHAR(100)                NULL COMMENT '重設密碼Token',
    password_reset_expiry       DATETIME                    NULL COMMENT 'Token到期時間',
    member_sid                  VARCHAR(32)                 NULL COMMENT '對應組織節點',
    register_verify_key         VARCHAR(100)                NULL COMMENT '管理員驗證碼',
    last_login_date             DATETIME                    NULL COMMENT '最後登入時間',
    last_read_all_notices_time  DATETIME                    NULL COMMENT '最後全部標為已讀的時間',
    avalible                    VARCHAR(2)              NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                      TEXT                        NULL COMMENT '備註'
) COMMENT='系統登入帳號資料表';

CREATE TABLE IF NOT EXISTS data_member_file (
    nid                 INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                 VARCHAR(32) NOT NULL UNIQUE COMMENT '附件序號',
    member_sid          VARCHAR(32) NOT NULL COMMENT '會員序號',
    file_type           VARCHAR(50) NOT NULL COMMENT 'HEAD_IMG;SIGNATURE;ID_CARD;CONTRACT;OTHER',
    file_name           VARCHAR(255) NOT NULL COMMENT '原始檔名',
    save_file_name      VARCHAR(255) NOT NULL COMMENT '儲存檔名',
    file_path           VARCHAR(500) NOT NULL COMMENT '檔案路徑',
    file_size           BIGINT NULL COMMENT '檔案大小(Byte)',
    create_date         DATETIME NOT NULL COMMENT '建立日期',
    modify_date         DATETIME NULL COMMENT '修改日期',
    avalible            VARCHAR(2) DEFAULT 'Y' COMMENT 'Y可用;D刪除',
    remark              TEXT NULL COMMENT '備註'
) COMMENT='會員附件資料表';

CREATE TABLE IF NOT EXISTS sys_config (
    nid                 INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    config_key          VARCHAR(100) NOT NULL UNIQUE COMMENT '設定鍵值',
    config_name         VARCHAR(100) NOT NULL COMMENT '設定名稱',
    config_value        TEXT NULL COMMENT '設定內容',
    config_type         VARCHAR(20) DEFAULT 'TEXT' COMMENT 'TEXT;NUMBER;BOOLEAN;JSON',
    category            VARCHAR(50) NULL COMMENT 'BASIC;LOGIN;COMPANY;SECURITY;INTEGRATION',
    script              TEXT NULL COMMENT '說明',
    create_date         DATETIME NOT NULL,
    modify_date         DATETIME NULL
) COMMENT='系統參數設定表';

CREATE TABLE IF NOT EXISTS sys_module (
    nid                 INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    module_code         VARCHAR(50) NOT NULL UNIQUE COMMENT 'MEMBER;NOTICE;ORG;IMPORT',
    module_name         VARCHAR(100) NOT NULL,
    module_desc         VARCHAR(500) NULL,
    icon                VARCHAR(100) NULL,
    route_url           VARCHAR(255) NULL,
    sort_no             INT DEFAULT 0,
    avalible            VARCHAR(2) DEFAULT 'Y' COMMENT 'Y啟用;N停用',
    create_date         DATETIME NOT NULL,
    modify_date         DATETIME NULL
) COMMENT='系統功能模組設定';

CREATE TABLE IF NOT EXISTS sys_notice_event (
    nid                 INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    event_code          VARCHAR(50) NOT NULL UNIQUE COMMENT '加上 UNIQUE 限制以利外鍵引用',
    event_name          VARCHAR(100) NOT NULL,
    script              VARCHAR(500) NULL,
    avalible            VARCHAR(2) DEFAULT 'Y',
    create_date         DATETIME NOT NULL,
    modify_date         DATETIME NULL
) COMMENT='通知事件設定';

CREATE TABLE IF NOT EXISTS sys_notice_channel (
    nid                 INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    event_code          VARCHAR(50) NOT NULL,
    channel_type        VARCHAR(20) NOT NULL COMMENT 'SYSTEM;EMAIL;LINE;FCM',
    receiver_type       VARCHAR(20) NOT NULL COMMENT 'SELF;ADMIN;ALL',
    avalible            VARCHAR(2) DEFAULT 'Y',
    create_date         DATETIME NOT NULL,
    modify_date         DATETIME NULL,
    CONSTRAINT fk_notice_event
        FOREIGN KEY (event_code)
        REFERENCES sys_notice_event(event_code)
) COMMENT='通知發送管道設定';

CREATE TABLE IF NOT EXISTS sys_notice_setting (
    nid                 INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    notice_type         VARCHAR(50) NOT NULL,
    channel_type        VARCHAR(20) NOT NULL COMMENT 'SYSTEM;EMAIL;LINE;FCM',
    days_before         INT DEFAULT 0,
    title               VARCHAR(200) NOT NULL,
    content             TEXT NOT NULL,
    avalible            VARCHAR(2) DEFAULT 'Y',
    create_date         DATETIME NOT NULL,
    modify_date         DATETIME NULL
) COMMENT='通知範本設定';

CREATE TABLE IF NOT EXISTS sys_integration (
    nid                 INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    service_code        VARCHAR(50) NOT NULL UNIQUE,
    service_name        VARCHAR(100) NOT NULL,
    service_type        VARCHAR(20) NOT NULL COMMENT 'LINE;FCM;SMTP;AWS;RECAPTCHA',
    connect_status      VARCHAR(20) DEFAULT 'DISCONNECT',
    last_sync_date      DATETIME NULL,
    avalible            VARCHAR(2) DEFAULT 'Y',
    create_date         DATETIME NOT NULL,
    modify_date         DATETIME NULL
) COMMENT='第三方服務整合設定';

CREATE TABLE IF NOT EXISTS sys_integration_config (
    nid                 INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    service_code        VARCHAR(50) NOT NULL,
    config_key          VARCHAR(100) NOT NULL,
    config_value        TEXT NULL,
    create_date         DATETIME NOT NULL,
    modify_date         DATETIME NULL,
    UNIQUE KEY uk_service_key(service_code, config_key)
) COMMENT='第三方服務參數';

CREATE TABLE IF NOT EXISTS sys_ip_whitelist (
    nid                 INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    ip_address          VARCHAR(100) NOT NULL,
    ip_name             VARCHAR(100) NULL,
    avalible            VARCHAR(2) DEFAULT 'Y',
    create_date         DATETIME NOT NULL,
    modify_date         DATETIME NULL,
    remark              TEXT NULL
) COMMENT='IP白名單';

CREATE TABLE IF NOT EXISTS sys_job (
    nid                 INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    job_code            VARCHAR(50) NOT NULL UNIQUE,
    job_name            VARCHAR(100) NOT NULL,
    cron_expression     VARCHAR(100) NOT NULL,
    last_run_time       DATETIME NULL,
    next_run_time       DATETIME NULL,
    avalible            VARCHAR(2) DEFAULT 'Y',
    create_date         DATETIME NOT NULL,
    modify_date         DATETIME NULL
) COMMENT='系統排程工作';

CREATE TABLE IF NOT EXISTS sys_job_log (
    nid                 INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    job_code            VARCHAR(50) NOT NULL,
    execute_time        DATETIME NOT NULL,
    execute_result      VARCHAR(20) COMMENT 'SUCCESS;FAIL',
    execute_message     TEXT NULL,
    spend_ms            BIGINT NULL
) COMMENT='排程執行紀錄';

CREATE TABLE IF NOT EXISTS log_push (
    nid                 INT         UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    member_sid          VARCHAR(32)     NOT NULL COMMENT '會員序號(data_member.sid)',
    member_mid          VARCHAR(20)         NULL COMMENT '會員編號(data_member.mid)',
    notify_type         VARCHAR(50)     NOT NULL COMMENT '通知類型',
    push_type           VARCHAR(20)     NOT NULL COMMENT '發送方式(FCM;EMAIL;LINE;SMS)',
    title               VARCHAR(200)    NOT NULL COMMENT '通知標題',
    message             TEXT            NOT NULL COMMENT '通知內容',
    send_date           DATETIME        NOT NULL COMMENT '發送時間',
    result              VARCHAR(20)         NULL COMMENT '發送結果(SUCCESS成功;FAIL失敗;PENDING待處理)',
    response_message    TEXT                NULL COMMENT '第三方回應訊息或錯誤內容',
    create_sid          VARCHAR(32)         NULL COMMENT '建立人員會員序號'
) COMMENT='推播通知紀錄表';

CREATE TABLE IF NOT EXISTS log_import (
    nid                 INT             UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    create_date         DATETIME                    NOT NULL COMMENT '匯入時間',
    create_sid          VARCHAR(32)                 NOT NULL COMMENT '匯入人員',
    file_name           VARCHAR(255)                NOT NULL COMMENT '檔案名稱',
    file_type           VARCHAR(10)                 NOT NULL COMMENT 'CSV;XLSX',
    total_count         INT             DEFAULT 0   NOT NULL COMMENT '總筆數',
    success_count       INT             DEFAULT 0   NOT NULL COMMENT '成功筆數',
    fail_count          INT             DEFAULT 0   NOT NULL COMMENT '失敗筆數',
    remark              TEXT NULL COMMENT '備註'
) COMMENT='會員匯入紀錄表';

CREATE TABLE IF NOT EXISTS log_export (
    nid                 INT             UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    create_date         DATETIME                    NOT NULL COMMENT '匯出時間',
    create_sid          VARCHAR(32)                 NOT NULL COMMENT '匯出人員',
    file_name           VARCHAR(255)                NOT NULL COMMENT '檔案名稱',
    file_type           VARCHAR(10)                 NOT NULL COMMENT 'CSV;XLSX',
    total_count         INT             DEFAULT 0   NOT NULL COMMENT '匯出筆數',
    search_condition    TEXT                            NULL COMMENT '查詢條件'
) COMMENT='會員匯出紀錄表';

CREATE TABLE IF NOT EXISTS log_action (
    nid                 INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    create_date         DATETIME        NOT NULL                COMMENT '時間',
    user_sid            VARCHAR(32)         NULL                COMMENT '操作帳號序號',
    user_account        VARCHAR(50)         NULL                COMMENT '操作帳號',
    user_name           VARCHAR(50)         NULL                COMMENT '操作人員',
    module_name         VARCHAR(50)         NULL                COMMENT '模組',
    action_name         VARCHAR(50)         NULL                COMMENT '操作',
    action_type         VARCHAR(50)     NOT NULL                COMMENT '操作類型',
    target_sid          VARCHAR(32)         NULL                COMMENT '目標對象序號',
    target_mid          VARCHAR(50)         NULL                COMMENT '目標對象帳號',
    ip                  VARCHAR(50)         NULL                COMMENT 'IP位址',
    result              VARCHAR(10)     NOT NULL DEFAULT '成功' COMMENT '結果：成功/失敗',
    page_name           VARCHAR(100)        NULL                COMMENT '操作頁面',
    user_agent          VARCHAR(255)        NULL                COMMENT '瀏覽器資訊',
    description         TEXT                NULL                COMMENT '操作描述',
    more_description    TEXT                NULL                COMMENT '更多資訊'
) COMMENT='系統操作紀錄表';

CREATE TABLE IF NOT EXISTS data_notice (
    nid                 INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                 VARCHAR(32)                         NOT NULL UNIQUE         COMMENT '系統公告序號',
    publish_date        DATE                                NOT NULL                COMMENT '發布日期',
    title               VARCHAR(255)                        NOT NULL                COMMENT '公告標題',
    content             TEXT                                NOT NULL                COMMENT '公告內文',
    is_top              TINYINT(1)                          NOT NULL DEFAULT 0      COMMENT '是否置頂：0=否 / 1=是',
    status              VARCHAR(10)                         NOT NULL DEFAULT '啟用' COMMENT '狀態：啟用/下架',
    start_time          DATETIME                                NULL                COMMENT '公告顯示開始時間',
    end_time            DATETIME                                NULL                COMMENT '公告顯示結束時間',
    publish_unit        VARCHAR(50) DEFAULT '系統管理部'     NOT NULL COMMENT '發布單位',
    category            VARCHAR(20) DEFAULT '一般公告'       NOT NULL COMMENT '公告分類',
    start_date          DATE                                    NULL COMMENT '公告生效起始日期',
    end_date            DATE                                    NULL COMMENT '公告生效結束日期',
    click_count         INT UNSIGNED DEFAULT 0                       NOT NULL COMMENT '點閱次數',
    create_user_sid     VARCHAR(32)                             NULL                COMMENT '建立人員序號',
    create_date         DATETIME                            NOT NULL                COMMENT '建立時間',
    modify_user_sid     VARCHAR(32)                             NULL                COMMENT '更新人員序號',
    modify_date         DATETIME                                NULL                COMMENT '更新時間',
    avalible            VARCHAR(2) DEFAULT 'Y'              NOT NULL                COMMENT 'Y可用;D刪除'
) COMMENT='系統公告資料表';

CREATE TABLE IF NOT EXISTS data_notice_read (
    id               INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    notice_sid       VARCHAR(32)                         NOT NULL COMMENT '對應 data_notice.sid',
    member_sid       VARCHAR(32)                         NOT NULL COMMENT '會員序號',
    read_time        DATETIME                            NOT NULL COMMENT '已讀時間',
    CONSTRAINT uid_notice_member UNIQUE (notice_sid, member_sid) COMMENT '避免重複記錄已讀'
) COMMENT='會員公告已讀紀錄表';

-- 為高頻率查詢建立索引（加快查詢某會員的已讀狀態）
CREATE INDEX idx_member_notice ON data_notice_read (member_sid, notice_sid);

-- 1. 建立個人待辦行事曆管理表
CREATE TABLE IF NOT EXISTS data_todo_calendar (
    nid                 INT             UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                 VARCHAR(32)     NOT NULL UNIQUE COMMENT '待辦事項唯一識別碼(UUID)',
    member_sid          VARCHAR(32)     NOT NULL COMMENT '會員/使用者序號',
    title               VARCHAR(200)    NOT NULL COMMENT '事項名稱',
    category            VARCHAR(50)     NOT NULL COMMENT '標籤分類(work工作;life生活;personal個人)',
    class_name          VARCHAR(50)     NOT NULL COMMENT 'FullCalendar對應樣式類別',
    start_date          DATETIME            NULL COMMENT '行事曆開始時間',
    end_date            DATETIME            NULL COMMENT '行事曆結束時間',
    is_all_day          TINYINT(1)      NOT NULL DEFAULT 0 COMMENT '是否為全天事件(0否;1是)',
    is_scheduled        TINYINT(1)      NOT NULL DEFAULT 0 COMMENT '是否已排程移入行事曆',
    is_completed        TINYINT(1)      NOT NULL DEFAULT 0 COMMENT '是否已完成',
    create_date         DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date         DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '最後更新時間(建議改為 NOT NULL 以利審計或 EF 處理)',
    create_sid          VARCHAR(32)         NULL COMMENT '建立人員會員序號',
    avalible            VARCHAR(2) DEFAULT 'Y'  NOT NULL                COMMENT 'Y可用;D刪除'
) COMMENT='個人待辦行事曆管理表';

-- 2. 為索引重新命名，避免全域名稱重複衝突
CREATE INDEX idx_dtc_member_scheduled ON data_todo_calendar(member_sid, is_scheduled);
CREATE INDEX idx_dtc_start_date ON data_todo_calendar(start_date);

CREATE INDEX idx_dm_parent_sid ON data_member(parent_sid);
CREATE INDEX idx_dm_register_status ON data_member(register_status);
CREATE INDEX idx_dm_role ON data_member(role);
CREATE INDEX idx_dm_mobile ON data_member(mobile);
CREATE INDEX idx_dm_email ON data_member(email);
CREATE INDEX idx_su_member_sid ON sys_user(member_sid);
CREATE INDEX idx_su_register_verify_key ON sys_user(register_verify_key);
CREATE INDEX idx_dm_parent_role ON data_member(parent_sid, role);

CREATE INDEX idx_lp_member_sid ON log_push(member_sid);
CREATE INDEX idx_lp_member_mid ON log_push(member_mid);
CREATE INDEX idx_lp_notify_type ON log_push(notify_type);
CREATE INDEX idx_lp_push_type ON log_push(push_type);
CREATE INDEX idx_lp_send_date ON log_push(send_date);

-- 追加優化索引
ALTER TABLE data_member ADD INDEX idx_dm_parent_avalible(parent_sid, avalible);
ALTER TABLE data_member ADD INDEX idx_dm_continue_date(continue_date);
";
    }
}