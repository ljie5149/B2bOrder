namespace B2bOrder.Resources.ShareCore
{
    /// <summary>
    /// IdentityDB V2 Shared Core Schema
    /// 設計目標：
    /// 1. 同時支援 Shopping Platform 與 Construction ERP
    /// 2. 管理帳號、角色、權限、資料範圍、MFA、Session、API Client 與跨系統登入
    /// 3. 組織、公司、部門、專案等主資料由 MasterDB 提供，IdentityDB 僅保存 sid 關聯
    /// 4. 密碼、Token、Secret 僅保存雜湊或加密後內容
    /// 5. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 6. avalible：Y可用；W停用；D刪除
    /// 7. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class IdentityDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 使用者帳號
-- =========================================================

CREATE TABLE IF NOT EXISTS idn_user (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '帳號序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    account                     VARCHAR(100)                        NOT NULL COMMENT '登入帳號',
    display_name                VARCHAR(200)                            NULL COMMENT '顯示名稱',
    email                       VARCHAR(200)                            NULL COMMENT 'Email',
    mobile                      VARCHAR(50)                             NULL COMMENT '手機',
    user_type                   VARCHAR(30)                         NOT NULL DEFAULT 'INTERNAL' COMMENT 'INTERNAL內部;MEMBER會員;CUSTOMER客戶;SUPPLIER供應商;CONTRACTOR承包商;SERVICE服務帳號',
    member_sid                  VARCHAR(32)                             NULL COMMENT '對應會員或外部對象序號',
    employee_sid                VARCHAR(32)                             NULL COMMENT '對應員工序號',
    company_sid                 VARCHAR(32)                             NULL COMMENT '預設公司序號',
    business_unit_sid           VARCHAR(32)                             NULL COMMENT '預設營運單位序號',
    department_sid              VARCHAR(32)                             NULL COMMENT '預設部門序號',
    language_sid                VARCHAR(32)                             NULL COMMENT '預設語系序號',
    timezone_code               VARCHAR(100)                        NOT NULL DEFAULT 'Asia/Taipei' COMMENT '預設時區',
    account_status              VARCHAR(30)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待啟用;ACTIVE啟用;LOCKED鎖定;SUSPENDED停權;DISABLED停用;CLOSED關閉',
    email_verified              TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT 'Email是否驗證',
    mobile_verified             TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '手機是否驗證',
    force_password_change       TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '下次登入是否強制改密碼',
    password_changed_date       DATETIME                                NULL COMMENT '最後密碼變更時間',
    last_login_date             DATETIME                                NULL COMMENT '最後登入時間',
    last_login_ip               VARCHAR(50)                             NULL COMMENT '最後登入IP',
    failed_login_count          INT                                 NOT NULL DEFAULT 0 COMMENT '連續登入失敗次數',
    locked_until                DATETIME                                NULL COMMENT '鎖定到期時間',
    last_read_all_notices_time  DATETIME                                NULL COMMENT '最後全部通知標為已讀時間',
    version_no                  BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                    VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                      TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_iu_account UNIQUE (account),
    UNIQUE KEY uk_iu_email (email),
    INDEX idx_iu_mobile (mobile),
    INDEX idx_iu_user_type (user_type),
    INDEX idx_iu_member_sid (member_sid),
    INDEX idx_iu_employee_sid (employee_sid),
    INDEX idx_iu_company_sid (company_sid),
    INDEX idx_iu_department_sid (department_sid),
    INDEX idx_iu_status (account_status),
    INDEX idx_iu_locked_until (locked_until),
    INDEX idx_iu_avalible (avalible),
    CHECK (failed_login_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系統使用者帳號';

CREATE TABLE IF NOT EXISTS idn_user_password (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '帳號密碼序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    user_nid                    BIGINT UNSIGNED                     NOT NULL COMMENT '帳號流水號',
    password_hash               VARCHAR(500)                        NOT NULL COMMENT '密碼雜湊',
    password_algorithm          VARCHAR(50)                         NOT NULL DEFAULT 'ARGON2ID' COMMENT 'ARGON2ID;BCRYPT;PBKDF2',
    password_salt               VARCHAR(255)                            NULL COMMENT '密碼Salt',
    password_version            INT                                 NOT NULL DEFAULT 1 COMMENT '密碼版本',
    effective_date              DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '生效時間',
    expiry_date                 DATETIME                                NULL COMMENT '到期時間',
    current_mark                TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否目前密碼',
    compromised_mark            TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否疑似外洩',
    CONSTRAINT fk_iup_user
        FOREIGN KEY (user_nid) REFERENCES idn_user(nid),
    CONSTRAINT uk_iup_user_version UNIQUE (user_nid, password_version),
    INDEX idx_iup_user_nid (user_nid),
    INDEX idx_iup_current_mark (current_mark),
    INDEX idx_iup_expiry_date (expiry_date),
    CHECK (password_version > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='帳號密碼與歷程';

CREATE TABLE IF NOT EXISTS idn_password_reset (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '密碼重設序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    user_sid                    VARCHAR(32)                         NOT NULL COMMENT '帳號序號',
    reset_token_hash            VARCHAR(255)                        NOT NULL COMMENT '重設Token雜湊',
    reset_channel               VARCHAR(20)                         NOT NULL COMMENT 'EMAIL;SMS;ADMIN;SUPPORT',
    expiry_date                 DATETIME                            NOT NULL COMMENT 'Token到期時間',
    used_date                   DATETIME                                NULL COMMENT '使用時間',
    request_ip                  VARCHAR(50)                             NULL COMMENT '申請IP',
    request_user_agent          VARCHAR(1000)                           NULL COMMENT '申請裝置資訊',
    reset_status                VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待使用;USED已使用;EXPIRED過期;REVOKED撤銷',
    UNIQUE KEY uk_ipr_token_hash (reset_token_hash),
    INDEX idx_ipr_user_sid (user_sid),
    INDEX idx_ipr_expiry_date (expiry_date),
    INDEX idx_ipr_status (reset_status),
    CHECK (expiry_date > create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='密碼重設Token';

-- =========================================================
-- 02. 密碼與登入政策
-- =========================================================

CREATE TABLE IF NOT EXISTS idn_password_policy (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '密碼政策序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    policy_code                 VARCHAR(100)                        NOT NULL COMMENT '政策代碼',
    policy_name                 VARCHAR(200)                        NOT NULL COMMENT '政策名稱',
    user_type                   VARCHAR(30)                             NULL COMMENT '適用使用者類型，NULL代表全部',
    minimum_length              INT                                 NOT NULL DEFAULT 8 COMMENT '最小長度',
    maximum_length              INT                                 NOT NULL DEFAULT 128 COMMENT '最大長度',
    require_uppercase           TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否要求大寫',
    require_lowercase           TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否要求小寫',
    require_number              TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否要求數字',
    require_symbol              TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否要求符號',
    history_count               INT                                 NOT NULL DEFAULT 5 COMMENT '不得重複最近密碼數',
    expiry_days                 INT                                     NULL COMMENT '密碼有效天數',
    warning_days                INT                                     NULL COMMENT '到期前提醒天數',
    maximum_failed_attempts     INT                                 NOT NULL DEFAULT 5 COMMENT '最大登入失敗次數',
    lock_minutes                INT                                 NOT NULL DEFAULT 30 COMMENT '鎖定分鐘數',
    mfa_required                TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否強制MFA',
    policy_status               VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    avalible                    VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                      TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ipp_policy_code UNIQUE (policy_code),
    INDEX idx_ipp_user_type (user_type),
    INDEX idx_ipp_status (policy_status),
    INDEX idx_ipp_avalible (avalible),
    CHECK (minimum_length > 0),
    CHECK (maximum_length >= minimum_length),
    CHECK (history_count >= 0),
    CHECK (expiry_days IS NULL OR expiry_days > 0),
    CHECK (warning_days IS NULL OR warning_days >= 0),
    CHECK (maximum_failed_attempts > 0),
    CHECK (lock_minutes >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='密碼與登入安全政策';

CREATE TABLE IF NOT EXISTS idn_user_policy (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '帳號政策關聯序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    user_nid                    BIGINT UNSIGNED                     NOT NULL COMMENT '帳號流水號',
    password_policy_nid         BIGINT UNSIGNED                     NOT NULL COMMENT '密碼政策流水號',
    start_date                  DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '生效時間',
    end_date                    DATETIME                                NULL COMMENT '失效時間',
    CONSTRAINT fk_iupol_user
        FOREIGN KEY (user_nid) REFERENCES idn_user(nid),
    CONSTRAINT fk_iupol_policy
        FOREIGN KEY (password_policy_nid) REFERENCES idn_password_policy(nid),
    CONSTRAINT uk_iupol_user_policy UNIQUE (user_nid, password_policy_nid, start_date),
    INDEX idx_iupol_user_nid (user_nid),
    INDEX idx_iupol_policy_nid (password_policy_nid),
    INDEX idx_iupol_date (start_date, end_date),
    CHECK (end_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='帳號安全政策套用';

-- =========================================================
-- 03. 角色、權限與資源
-- =========================================================

CREATE TABLE IF NOT EXISTS idn_role (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '角色序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    role_code                   VARCHAR(100)                        NOT NULL COMMENT '角色代碼',
    role_name                   VARCHAR(200)                        NOT NULL COMMENT '角色名稱',
    role_type                   VARCHAR(30)                         NOT NULL DEFAULT 'BUSINESS' COMMENT 'SYSTEM系統;BUSINESS業務;EXTERNAL外部;PROJECT專案',
    parent_sid                  VARCHAR(32)                             NULL COMMENT '上層角色序號',
    system_mark                 TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否系統內建',
    assignable_mark             TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否可直接指派',
    role_status                 VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    avalible                    VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                      TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ir_role_code UNIQUE (role_code),
    INDEX idx_ir_role_type (role_type),
    INDEX idx_ir_parent_sid (parent_sid),
    INDEX idx_ir_status (role_status),
    INDEX idx_ir_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系統角色';

CREATE TABLE IF NOT EXISTS idn_resource (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '資源序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    service_code                VARCHAR(80)                         NOT NULL COMMENT '服務代碼',
    resource_code               VARCHAR(200)                        NOT NULL COMMENT '資源代碼',
    resource_name               VARCHAR(200)                        NOT NULL COMMENT '資源名稱',
    resource_type               VARCHAR(30)                         NOT NULL COMMENT 'MENU選單;PAGE頁面;API;BUTTON按鈕;REPORT報表;DATA資料;FEATURE功能',
    parent_sid                  VARCHAR(32)                             NULL COMMENT '上層資源序號',
    route_path                  VARCHAR(500)                            NULL COMMENT '前端路由',
    api_pattern                 VARCHAR(1000)                           NULL COMMENT 'API路徑樣式',
    http_method                 VARCHAR(20)                             NULL COMMENT 'HTTP方法',
    sort_no                     INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    resource_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    avalible                    VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                      TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ires_service_code UNIQUE (service_code, resource_code),
    INDEX idx_ires_resource_type (resource_type),
    INDEX idx_ires_parent_sid (parent_sid),
    INDEX idx_ires_route_path (route_path(191)),
    INDEX idx_ires_status (resource_status),
    INDEX idx_ires_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='受保護系統資源';

CREATE TABLE IF NOT EXISTS idn_permission (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '權限序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    permission_code             VARCHAR(200)                        NOT NULL COMMENT '權限代碼',
    permission_name             VARCHAR(200)                        NOT NULL COMMENT '權限名稱',
    permission_type             VARCHAR(30)                         NOT NULL DEFAULT 'ACTION' COMMENT 'ACTION操作;DATA資料;ADMIN管理;APPROVAL簽核;EXPORT匯出',
    action_code                 VARCHAR(50)                         NOT NULL COMMENT 'READ;CREATE;UPDATE;DELETE;APPROVE;EXPORT;MANAGE;EXECUTE',
    resource_sid                VARCHAR(32)                         NOT NULL COMMENT '資源序號',
    sensitive_mark              TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否敏感權限',
    require_mfa                 TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '使用時是否需MFA',
    permission_status           VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    avalible                    VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                      TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ip_permission_code UNIQUE (permission_code),
    INDEX idx_ip_permission_type (permission_type),
    INDEX idx_ip_resource_sid (resource_sid),
    INDEX idx_ip_action_code (action_code),
    INDEX idx_ip_sensitive_mark (sensitive_mark),
    INDEX idx_ip_status (permission_status),
    INDEX idx_ip_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系統權限';

CREATE TABLE IF NOT EXISTS idn_role_permission (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '角色權限序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    role_nid                    BIGINT UNSIGNED                     NOT NULL COMMENT '角色流水號',
    permission_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '權限流水號',
    effect_type                 VARCHAR(10)                         NOT NULL DEFAULT 'ALLOW' COMMENT 'ALLOW允許;DENY拒絕',
    condition_json              JSON                                    NULL COMMENT '附加條件',
    start_date                  DATETIME                                NULL COMMENT '生效時間',
    end_date                    DATETIME                                NULL COMMENT '失效時間',
    CONSTRAINT fk_irp_role
        FOREIGN KEY (role_nid) REFERENCES idn_role(nid),
    CONSTRAINT fk_irp_permission
        FOREIGN KEY (permission_nid) REFERENCES idn_permission(nid),
    CONSTRAINT uk_irp_role_permission UNIQUE (role_nid, permission_nid),
    INDEX idx_irp_role_nid (role_nid),
    INDEX idx_irp_permission_nid (permission_nid),
    INDEX idx_irp_effect_type (effect_type),
    INDEX idx_irp_date (start_date, end_date),
    CHECK (end_date IS NULL OR start_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色權限關聯';

CREATE TABLE IF NOT EXISTS idn_user_role (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '帳號角色序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    user_nid                    BIGINT UNSIGNED                     NOT NULL COMMENT '帳號流水號',
    role_nid                    BIGINT UNSIGNED                     NOT NULL COMMENT '角色流水號',
    assignment_type             VARCHAR(20)                         NOT NULL DEFAULT 'DIRECT' COMMENT 'DIRECT直接;GROUP群組;POSITION職務;PROJECT專案;EXTERNAL外部',
    assignment_source_sid       VARCHAR(32)                             NULL COMMENT '指派來源序號',
    company_sid                 VARCHAR(32)                             NULL COMMENT '限定公司序號',
    business_unit_sid           VARCHAR(32)                             NULL COMMENT '限定營運單位序號',
    department_sid              VARCHAR(32)                             NULL COMMENT '限定部門序號',
    project_sid                 VARCHAR(32)                             NULL COMMENT '限定專案序號',
    start_date                  DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '生效時間',
    end_date                    DATETIME                                NULL COMMENT '失效時間',
    granted_user_sid            VARCHAR(32)                             NULL COMMENT '授權人員序號',
    grant_reason                VARCHAR(1000)                           NULL COMMENT '授權原因',
    assignment_status           VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;EXPIRED過期;REVOKED撤銷',
    CONSTRAINT fk_iur_user
        FOREIGN KEY (user_nid) REFERENCES idn_user(nid),
    CONSTRAINT fk_iur_role
        FOREIGN KEY (role_nid) REFERENCES idn_role(nid),
    CONSTRAINT uk_iur_assignment UNIQUE (user_nid, role_nid, assignment_type, assignment_source_sid, company_sid, business_unit_sid, department_sid, project_sid),
    INDEX idx_iur_user_nid (user_nid),
    INDEX idx_iur_role_nid (role_nid),
    INDEX idx_iur_assignment_type (assignment_type),
    INDEX idx_iur_company_sid (company_sid),
    INDEX idx_iur_business_unit_sid (business_unit_sid),
    INDEX idx_iur_department_sid (department_sid),
    INDEX idx_iur_project_sid (project_sid),
    INDEX idx_iur_status (assignment_status),
    INDEX idx_iur_date (start_date, end_date),
    CHECK (end_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='帳號角色指派';

-- =========================================================
-- 04. 資料權限範圍
-- =========================================================

CREATE TABLE IF NOT EXISTS idn_data_scope (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '資料範圍序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    scope_code                  VARCHAR(100)                        NOT NULL COMMENT '資料範圍代碼',
    scope_name                  VARCHAR(200)                        NOT NULL COMMENT '資料範圍名稱',
    scope_type                  VARCHAR(30)                         NOT NULL COMMENT 'ALL全部;COMPANY公司;BUSINESS_UNIT營運單位;DEPARTMENT部門;PROJECT專案;STORE商店;WAREHOUSE倉庫;CUSTOM自訂',
    scope_expression            TEXT                                    NULL COMMENT '動態範圍運算式',
    scope_config                JSON                                    NULL COMMENT '範圍設定',
    scope_status                VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    avalible                    VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                      TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ids_scope_code UNIQUE (scope_code),
    INDEX idx_ids_scope_type (scope_type),
    INDEX idx_ids_status (scope_status),
    INDEX idx_ids_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='資料存取範圍';

CREATE TABLE IF NOT EXISTS idn_role_data_scope (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '角色資料範圍序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    role_nid                    BIGINT UNSIGNED                     NOT NULL COMMENT '角色流水號',
    data_scope_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '資料範圍流水號',
    resource_type               VARCHAR(100)                            NULL COMMENT '限定資料類型',
    effect_type                 VARCHAR(10)                         NOT NULL DEFAULT 'ALLOW' COMMENT 'ALLOW允許;DENY拒絕',
    CONSTRAINT fk_irds_role
        FOREIGN KEY (role_nid) REFERENCES idn_role(nid),
    CONSTRAINT fk_irds_scope
        FOREIGN KEY (data_scope_nid) REFERENCES idn_data_scope(nid),
    CONSTRAINT uk_irds_role_scope UNIQUE (role_nid, data_scope_nid, resource_type),
    INDEX idx_irds_role_nid (role_nid),
    INDEX idx_irds_data_scope_nid (data_scope_nid),
    INDEX idx_irds_resource_type (resource_type)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色資料範圍';

CREATE TABLE IF NOT EXISTS idn_user_data_scope (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '帳號資料範圍序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    user_nid                    BIGINT UNSIGNED                     NOT NULL COMMENT '帳號流水號',
    data_scope_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '資料範圍流水號',
    resource_type               VARCHAR(100)                            NULL COMMENT '限定資料類型',
    effect_type                 VARCHAR(10)                         NOT NULL DEFAULT 'ALLOW' COMMENT 'ALLOW允許;DENY拒絕',
    start_date                  DATETIME                                NULL COMMENT '生效時間',
    end_date                    DATETIME                                NULL COMMENT '失效時間',
    CONSTRAINT fk_iuds_user
        FOREIGN KEY (user_nid) REFERENCES idn_user(nid),
    CONSTRAINT fk_iuds_scope
        FOREIGN KEY (data_scope_nid) REFERENCES idn_data_scope(nid),
    CONSTRAINT uk_iuds_user_scope UNIQUE (user_nid, data_scope_nid, resource_type),
    INDEX idx_iuds_user_nid (user_nid),
    INDEX idx_iuds_data_scope_nid (data_scope_nid),
    INDEX idx_iuds_resource_type (resource_type),
    INDEX idx_iuds_date (start_date, end_date),
    CHECK (end_date IS NULL OR start_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='帳號額外資料範圍';

-- =========================================================
-- 05. MFA與驗證
-- =========================================================

CREATE TABLE IF NOT EXISTS idn_mfa_method (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'MFA方法序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    user_nid                    BIGINT UNSIGNED                     NOT NULL COMMENT '帳號流水號',
    method_type                 VARCHAR(30)                         NOT NULL COMMENT 'TOTP驗證器;SMS;EMAIL;WEBAUTHN;RECOVERY_CODE',
    method_name                 VARCHAR(100)                            NULL COMMENT '方法顯示名稱',
    secret_encrypted            LONGTEXT                                NULL COMMENT '加密後Secret',
    endpoint_masked             VARCHAR(200)                            NULL COMMENT '遮罩後接收端點',
    credential_id               VARCHAR(1000)                           NULL COMMENT 'WebAuthn Credential ID',
    public_key_data             LONGTEXT                                NULL COMMENT 'WebAuthn公開金鑰',
    verified                    TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否已驗證',
    verified_date               DATETIME                                NULL COMMENT '驗證時間',
    primary_mark                TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否主要MFA',
    method_status               VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;DISABLED停用;REVOKED撤銷',
    last_used_date              DATETIME                                NULL COMMENT '最後使用時間',
    CONSTRAINT fk_imm_user
        FOREIGN KEY (user_nid) REFERENCES idn_user(nid),
    INDEX idx_imm_user_nid (user_nid),
    INDEX idx_imm_method_type (method_type),
    INDEX idx_imm_primary_mark (primary_mark),
    INDEX idx_imm_status (method_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='多因素驗證方法';

CREATE TABLE IF NOT EXISTS idn_verification_code (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '驗證碼序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    user_sid                    VARCHAR(32)                             NULL COMMENT '帳號序號',
    purpose_code                VARCHAR(50)                         NOT NULL COMMENT 'REGISTER註冊;LOGIN登入;MFA;EMAIL_VERIFY;MOBILE_VERIFY;PASSWORD_RESET',
    channel_type                VARCHAR(20)                         NOT NULL COMMENT 'EMAIL;SMS;SYSTEM',
    endpoint_hash               VARCHAR(128)                            NULL COMMENT '接收端點雜湊',
    code_hash                   VARCHAR(255)                        NOT NULL COMMENT '驗證碼雜湊',
    expiry_date                 DATETIME                            NOT NULL COMMENT '到期時間',
    used_date                   DATETIME                                NULL COMMENT '使用時間',
    attempt_count               INT                                 NOT NULL DEFAULT 0 COMMENT '驗證嘗試次數',
    max_attempt_count           INT                                 NOT NULL DEFAULT 5 COMMENT '最大嘗試次數',
    verification_status         VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待驗證;SUCCESS成功;FAILED失敗;EXPIRED過期;LOCKED鎖定',
    request_ip                  VARCHAR(50)                             NULL COMMENT '申請IP',
    INDEX idx_ivc_user_sid (user_sid),
    INDEX idx_ivc_purpose_code (purpose_code),
    INDEX idx_ivc_endpoint_hash (endpoint_hash),
    INDEX idx_ivc_expiry_date (expiry_date),
    INDEX idx_ivc_status (verification_status),
    CHECK (expiry_date > create_date),
    CHECK (attempt_count >= 0),
    CHECK (max_attempt_count > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='通用驗證碼';

CREATE TABLE IF NOT EXISTS idn_recovery_code (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'MFA備援碼序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    user_nid                    BIGINT UNSIGNED                     NOT NULL COMMENT '帳號流水號',
    code_hash                   VARCHAR(255)                        NOT NULL COMMENT '備援碼雜湊',
    used_date                   DATETIME                                NULL COMMENT '使用時間',
    recovery_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE可用;USED已用;REVOKED撤銷',
    CONSTRAINT fk_irc_user
        FOREIGN KEY (user_nid) REFERENCES idn_user(nid),
    UNIQUE KEY uk_irc_code_hash (code_hash),
    INDEX idx_irc_user_nid (user_nid),
    INDEX idx_irc_status (recovery_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='MFA備援碼';

-- =========================================================
-- 06. Session與Token
-- =========================================================

CREATE TABLE IF NOT EXISTS idn_session (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '登入工作階段序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    user_nid                    BIGINT UNSIGNED                     NOT NULL COMMENT '帳號流水號',
    session_token_hash          VARCHAR(255)                        NOT NULL COMMENT 'Session Token雜湊',
    refresh_token_hash          VARCHAR(255)                            NULL COMMENT 'Refresh Token雜湊',
    client_type                 VARCHAR(30)                         NOT NULL COMMENT 'WEB;ANDROID;IOS;DESKTOP;API;DEVICE',
    client_id                   VARCHAR(100)                            NULL COMMENT 'OAuth或API Client ID',
    device_id                   VARCHAR(200)                            NULL COMMENT '裝置識別碼',
    device_name                 VARCHAR(200)                            NULL COMMENT '裝置名稱',
    ip_address                  VARCHAR(50)                             NULL COMMENT 'IP位址',
    user_agent                  VARCHAR(1000)                           NULL COMMENT '裝置或瀏覽器資訊',
    country_code                VARCHAR(10)                             NULL COMMENT '來源國家代碼',
    mfa_verified                TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否完成MFA',
    issued_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '簽發時間',
    expiry_date                 DATETIME                            NOT NULL COMMENT '到期時間',
    refresh_expiry_date         DATETIME                                NULL COMMENT 'Refresh Token到期時間',
    last_activity_date          DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '最後活動時間',
    revoked_date                DATETIME                                NULL COMMENT '撤銷時間',
    revoke_reason               VARCHAR(500)                            NULL COMMENT '撤銷原因',
    session_status              VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;EXPIRED過期;REVOKED撤銷;LOGOUT登出',
    CONSTRAINT fk_is_user
        FOREIGN KEY (user_nid) REFERENCES idn_user(nid),
    UNIQUE KEY uk_is_session_token_hash (session_token_hash),
    UNIQUE KEY uk_is_refresh_token_hash (refresh_token_hash),
    INDEX idx_is_user_nid (user_nid),
    INDEX idx_is_client_type (client_type),
    INDEX idx_is_client_id (client_id),
    INDEX idx_is_device_id (device_id),
    INDEX idx_is_expiry_date (expiry_date),
    INDEX idx_is_last_activity_date (last_activity_date),
    INDEX idx_is_status (session_status),
    CHECK (expiry_date > issued_date),
    CHECK (refresh_expiry_date IS NULL OR refresh_expiry_date >= expiry_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='登入工作階段';

CREATE TABLE IF NOT EXISTS idn_token_revocation (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Token撤銷序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    token_type                  VARCHAR(30)                         NOT NULL COMMENT 'ACCESS;REFRESH;API_KEY;RESET;VERIFICATION',
    token_hash                  VARCHAR(255)                        NOT NULL COMMENT 'Token雜湊',
    user_sid                    VARCHAR(32)                             NULL COMMENT '帳號序號',
    client_id                   VARCHAR(100)                            NULL COMMENT 'Client ID',
    expiry_date                 DATETIME                                NULL COMMENT 'Token原到期時間',
    revoke_reason               VARCHAR(500)                            NULL COMMENT '撤銷原因',
    revoked_user_sid            VARCHAR(32)                             NULL COMMENT '執行撤銷人員序號',
    UNIQUE KEY uk_itr_token_hash (token_hash),
    INDEX idx_itr_token_type (token_type),
    INDEX idx_itr_user_sid (user_sid),
    INDEX idx_itr_client_id (client_id),
    INDEX idx_itr_expiry_date (expiry_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Token撤銷清單';

-- =========================================================
-- 07. OAuth、SSO與外部身分
-- =========================================================

CREATE TABLE IF NOT EXISTS idn_identity_provider (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '身分提供者序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    provider_code               VARCHAR(80)                         NOT NULL COMMENT '提供者代碼',
    provider_name               VARCHAR(150)                        NOT NULL COMMENT '提供者名稱',
    provider_type               VARCHAR(30)                         NOT NULL COMMENT 'OIDC;SAML;OAUTH2;LDAP;AD;SOCIAL',
    issuer_url                  VARCHAR(1000)                           NULL COMMENT 'Issuer URL',
    authorization_url           VARCHAR(1000)                           NULL COMMENT 'Authorization URL',
    token_url                   VARCHAR(1000)                           NULL COMMENT 'Token URL',
    user_info_url               VARCHAR(1000)                           NULL COMMENT 'UserInfo URL',
    client_id                   VARCHAR(300)                            NULL COMMENT 'Provider Client ID',
    client_secret_encrypted     LONGTEXT                                NULL COMMENT '加密後Client Secret',
    scopes                     VARCHAR(1000)                           NULL COMMENT 'Scopes',
    attribute_mapping           JSON                                    NULL COMMENT '屬性對應',
    auto_create_user            TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否自動建立帳號',
    provider_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;DISABLED停用',
    avalible                    VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                      TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_iip_provider_code UNIQUE (provider_code),
    INDEX idx_iip_provider_type (provider_type),
    INDEX idx_iip_status (provider_status),
    INDEX idx_iip_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='外部身分提供者';

CREATE TABLE IF NOT EXISTS idn_external_identity (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '外部身分序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    user_nid                    BIGINT UNSIGNED                     NOT NULL COMMENT '帳號流水號',
    identity_provider_nid       BIGINT UNSIGNED                     NOT NULL COMMENT '身分提供者流水號',
    external_subject            VARCHAR(500)                        NOT NULL COMMENT '外部Subject ID',
    external_account            VARCHAR(300)                            NULL COMMENT '外部帳號',
    external_email              VARCHAR(300)                            NULL COMMENT '外部Email',
    profile_data                JSON                                    NULL COMMENT '外部Profile快照',
    linked_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '綁定時間',
    last_login_date             DATETIME                                NULL COMMENT '最後外部登入時間',
    link_status                 VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;UNLINKED解除;DISABLED停用',
    CONSTRAINT fk_iei_user
        FOREIGN KEY (user_nid) REFERENCES idn_user(nid),
    CONSTRAINT fk_iei_provider
        FOREIGN KEY (identity_provider_nid) REFERENCES idn_identity_provider(nid),
    CONSTRAINT uk_iei_provider_subject UNIQUE (identity_provider_nid, external_subject),
    INDEX idx_iei_user_nid (user_nid),
    INDEX idx_iei_external_account (external_account),
    INDEX idx_iei_external_email (external_email),
    INDEX idx_iei_status (link_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='帳號外部身分綁定';

-- =========================================================
-- 08. API Client與API Key
-- =========================================================

CREATE TABLE IF NOT EXISTS idn_api_client (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'API Client序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    client_id                   VARCHAR(150)                        NOT NULL COMMENT 'API Client ID',
    client_name                 VARCHAR(200)                        NOT NULL COMMENT 'API Client名稱',
    client_type                 VARCHAR(30)                         NOT NULL COMMENT 'CONFIDENTIAL機密;PUBLIC公開;SERVICE服務;DEVICE裝置',
    owner_type                  VARCHAR(30)                         NOT NULL COMMENT 'COMPANY;SERVICE;USER;PARTNER;SUPPLIER;CONTRACTOR',
    owner_sid                   VARCHAR(32)                             NULL COMMENT '擁有者序號',
    client_secret_hash          VARCHAR(500)                            NULL COMMENT 'Client Secret雜湊',
    redirect_uris               JSON                                    NULL COMMENT '允許Redirect URI',
    allowed_grant_types         JSON                                    NULL COMMENT '允許Grant Types',
    allowed_scopes              JSON                                    NULL COMMENT '允許Scopes',
    token_lifetime_seconds      INT                                 NOT NULL DEFAULT 3600 COMMENT 'Access Token有效秒數',
    refresh_lifetime_seconds    INT                                 NOT NULL DEFAULT 2592000 COMMENT 'Refresh Token有效秒數',
    ip_allowlist                JSON                                    NULL COMMENT '允許IP清單',
    client_status               VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;SUSPENDED暫停;REVOKED撤銷;DISABLED停用',
    last_used_date              DATETIME                                NULL COMMENT '最後使用時間',
    avalible                    VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                      TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_iac_client_id UNIQUE (client_id),
    INDEX idx_iac_client_type (client_type),
    INDEX idx_iac_owner (owner_type, owner_sid),
    INDEX idx_iac_status (client_status),
    INDEX idx_iac_avalible (avalible),
    CHECK (token_lifetime_seconds > 0),
    CHECK (refresh_lifetime_seconds > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OAuth與API Client';

CREATE TABLE IF NOT EXISTS idn_api_key (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'API Key序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    api_client_nid              BIGINT UNSIGNED                     NOT NULL COMMENT 'API Client流水號',
    key_name                    VARCHAR(150)                        NOT NULL COMMENT 'Key名稱',
    key_prefix                  VARCHAR(30)                         NOT NULL COMMENT 'Key前綴',
    key_hash                    VARCHAR(500)                        NOT NULL COMMENT 'API Key雜湊',
    allowed_scopes              JSON                                    NULL COMMENT '允許Scopes',
    ip_allowlist                JSON                                    NULL COMMENT '允許IP',
    start_date                  DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '生效時間',
    expiry_date                 DATETIME                                NULL COMMENT '到期時間',
    last_used_date              DATETIME                                NULL COMMENT '最後使用時間',
    revoked_date                DATETIME                                NULL COMMENT '撤銷時間',
    revoke_reason               VARCHAR(500)                            NULL COMMENT '撤銷原因',
    key_status                  VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;EXPIRED過期;REVOKED撤銷;DISABLED停用',
    CONSTRAINT fk_iak_api_client
        FOREIGN KEY (api_client_nid) REFERENCES idn_api_client(nid),
    UNIQUE KEY uk_iak_key_hash (key_hash),
    INDEX idx_iak_api_client_nid (api_client_nid),
    INDEX idx_iak_key_prefix (key_prefix),
    INDEX idx_iak_expiry_date (expiry_date),
    INDEX idx_iak_status (key_status),
    CHECK (expiry_date IS NULL OR expiry_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='API Key';

-- =========================================================
-- 09. 登入與安全事件
-- =========================================================

CREATE TABLE IF NOT EXISTS idn_login_event (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '登入事件序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '事件時間',
    user_sid                    VARCHAR(32)                             NULL COMMENT '帳號序號',
    account_snapshot            VARCHAR(150)                            NULL COMMENT '帳號快照',
    event_code                  VARCHAR(50)                         NOT NULL COMMENT 'LOGIN_SUCCESS;LOGIN_FAIL;LOGOUT;MFA_SUCCESS;MFA_FAIL;LOCK;UNLOCK',
    authentication_method       VARCHAR(30)                             NULL COMMENT 'PASSWORD;MFA;SSO;OAUTH;API_KEY',
    client_type                 VARCHAR(30)                             NULL COMMENT 'WEB;ANDROID;IOS;API;DEVICE',
    client_id                   VARCHAR(150)                            NULL COMMENT 'Client ID',
    session_sid                 VARCHAR(32)                             NULL COMMENT 'Session序號',
    result                      VARCHAR(20)                         NOT NULL COMMENT 'SUCCESS成功;FAIL失敗;BLOCKED封鎖',
    fail_reason_code            VARCHAR(100)                            NULL COMMENT '失敗原因代碼',
    fail_reason                 VARCHAR(500)                            NULL COMMENT '失敗原因',
    ip_address                  VARCHAR(50)                             NULL COMMENT 'IP位址',
    country_code                VARCHAR(10)                             NULL COMMENT '國家代碼',
    device_id                   VARCHAR(200)                            NULL COMMENT '裝置識別碼',
    user_agent                  VARCHAR(1000)                           NULL COMMENT '裝置資訊',
    risk_score                  DECIMAL(8,2)                            NULL COMMENT '風險分數',
    risk_flags                  JSON                                    NULL COMMENT '風險標記',
    INDEX idx_ile_user_sid (user_sid),
    INDEX idx_ile_account_snapshot (account_snapshot),
    INDEX idx_ile_event_code (event_code),
    INDEX idx_ile_auth_method (authentication_method),
    INDEX idx_ile_client_id (client_id),
    INDEX idx_ile_session_sid (session_sid),
    INDEX idx_ile_result (result),
    INDEX idx_ile_ip_address (ip_address),
    INDEX idx_ile_create_date (create_date),
    INDEX idx_ile_risk_score (risk_score)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='登入與驗證事件';

-- =========================================================
-- 10. 使用者群組
-- =========================================================

CREATE TABLE IF NOT EXISTS idn_group (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '使用者群組序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date                 DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    group_code                  VARCHAR(100)                        NOT NULL COMMENT '群組代碼',
    group_name                  VARCHAR(200)                        NOT NULL COMMENT '群組名稱',
    group_type                  VARCHAR(30)                         NOT NULL DEFAULT 'CUSTOM' COMMENT 'CUSTOM自訂;DEPARTMENT部門;PROJECT專案;SUPPLIER供應商;CONTRACTOR承包商',
    reference_sid               VARCHAR(32)                             NULL COMMENT '對應外部主資料序號',
    group_status                VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    avalible                    VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                      TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ig_group_code UNIQUE (group_code),
    INDEX idx_ig_group_type (group_type),
    INDEX idx_ig_reference_sid (reference_sid),
    INDEX idx_ig_status (group_status),
    INDEX idx_ig_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='使用者群組';

CREATE TABLE IF NOT EXISTS idn_group_member (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '群組成員序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    group_nid                   BIGINT UNSIGNED                     NOT NULL COMMENT '群組流水號',
    user_nid                    BIGINT UNSIGNED                     NOT NULL COMMENT '帳號流水號',
    member_role                 VARCHAR(30)                         NOT NULL DEFAULT 'MEMBER' COMMENT 'OWNER擁有者;MANAGER管理者;MEMBER成員',
    start_date                  DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '加入時間',
    end_date                    DATETIME                                NULL COMMENT '離開時間',
    member_status               VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;EXPIRED過期;REMOVED移除',
    CONSTRAINT fk_igm_group
        FOREIGN KEY (group_nid) REFERENCES idn_group(nid),
    CONSTRAINT fk_igm_user
        FOREIGN KEY (user_nid) REFERENCES idn_user(nid),
    CONSTRAINT uk_igm_group_user UNIQUE (group_nid, user_nid),
    INDEX idx_igm_group_nid (group_nid),
    INDEX idx_igm_user_nid (user_nid),
    INDEX idx_igm_member_role (member_role),
    INDEX idx_igm_status (member_status),
    CHECK (end_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='使用者群組成員';

CREATE TABLE IF NOT EXISTS idn_group_role (
    nid                         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                         VARCHAR(32)                         NOT NULL UNIQUE COMMENT '群組角色序號',
    create_date                 DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    group_nid                   BIGINT UNSIGNED                     NOT NULL COMMENT '群組流水號',
    role_nid                    BIGINT UNSIGNED                     NOT NULL COMMENT '角色流水號',
    start_date                  DATETIME                                NULL COMMENT '生效時間',
    end_date                    DATETIME                                NULL COMMENT '失效時間',
    CONSTRAINT fk_igr_group
        FOREIGN KEY (group_nid) REFERENCES idn_group(nid),
    CONSTRAINT fk_igr_role
        FOREIGN KEY (role_nid) REFERENCES idn_role(nid),
    CONSTRAINT uk_igr_group_role UNIQUE (group_nid, role_nid),
    INDEX idx_igr_group_nid (group_nid),
    INDEX idx_igr_role_nid (role_nid),
    INDEX idx_igr_date (start_date, end_date),
    CHECK (end_date IS NULL OR start_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='使用者群組角色';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
