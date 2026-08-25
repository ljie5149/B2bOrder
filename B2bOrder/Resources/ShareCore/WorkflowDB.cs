namespace B2bOrder.Resources.ShareCore
{
    /// <summary>
    /// WorkflowDB Core Schema
    /// 規範：
    /// 1. nid：資料庫內部主鍵與外鍵
    /// 2. sid：Web / APP / API 對外識別碼
    /// 3. avalible：Y可用；W停用；D刪除
    /// 4. IdentityDB、MasterDB、ShoppingDB、PurchaseDB、StoreDB 等跨資料庫參照使用 sid
    /// 5. 支援流程定義、版本、節點、條件、簽核實例、簽核紀錄與代理人
    /// 6. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class WorkflowDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 流程定義
-- =========================================================

CREATE TABLE IF NOT EXISTS wf_definition (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '流程定義序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    workflow_code           VARCHAR(100)                        NOT NULL COMMENT '流程代碼',
    workflow_name           VARCHAR(200)                        NOT NULL COMMENT '流程名稱',
    workflow_category       VARCHAR(50)                         NOT NULL COMMENT 'PURCHASE採購;REFUND退款;PRODUCT商品;STORE商店;GENERAL一般簽呈',
    business_type           VARCHAR(100)                        NOT NULL COMMENT '適用業務類型',
    source_service_code     VARCHAR(80)                         NOT NULL COMMENT '來源服務代碼',
    description             TEXT                                    NULL COMMENT '流程說明',
    current_version         INT                                 NOT NULL DEFAULT 1 COMMENT '目前版本',
    workflow_status         VARCHAR(20)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;ACTIVE啟用;INACTIVE停用;ARCHIVED封存',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_wfd_workflow_code UNIQUE (workflow_code),
    INDEX idx_wfd_category (workflow_category),
    INDEX idx_wfd_business_type (business_type),
    INDEX idx_wfd_source_service (source_service_code),
    INDEX idx_wfd_status (workflow_status),
    INDEX idx_wfd_avalible (avalible),
    CHECK (current_version > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='簽核流程定義';

CREATE TABLE IF NOT EXISTS wf_definition_version (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '流程版本序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    workflow_definition_nid BIGINT UNSIGNED                     NOT NULL COMMENT '流程定義流水號',
    version_no              INT                                 NOT NULL COMMENT '版本號',
    version_name            VARCHAR(100)                            NULL COMMENT '版本名稱',
    effective_start_date    DATETIME                                NULL COMMENT '生效開始時間',
    effective_end_date      DATETIME                                NULL COMMENT '生效結束時間',
    definition_json         JSON                                    NULL COMMENT '完整流程定義JSON',
    version_status          VARCHAR(20)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;PUBLISHED已發布;EXPIRED失效',
    published_user_sid      VARCHAR(32)                             NULL COMMENT '發布人員序號',
    published_date          DATETIME                                NULL COMMENT '發布時間',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_wfdv_definition
        FOREIGN KEY (workflow_definition_nid) REFERENCES wf_definition(nid),
    CONSTRAINT uk_wfdv_definition_version UNIQUE (workflow_definition_nid, version_no),
    INDEX idx_wfdv_definition_nid (workflow_definition_nid),
    INDEX idx_wfdv_effective_date (effective_start_date, effective_end_date),
    INDEX idx_wfdv_status (version_status),
    CHECK (version_no > 0),
    CHECK (effective_end_date IS NULL OR effective_start_date IS NULL OR effective_end_date >= effective_start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='簽核流程版本';

-- =========================================================
-- 02. 流程節點與路由
-- =========================================================

CREATE TABLE IF NOT EXISTS wf_node (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '流程節點序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    workflow_version_nid    BIGINT UNSIGNED                     NOT NULL COMMENT '流程版本流水號',
    node_code               VARCHAR(100)                        NOT NULL COMMENT '節點代碼',
    node_name               VARCHAR(200)                        NOT NULL COMMENT '節點名稱',
    node_type               VARCHAR(30)                         NOT NULL COMMENT 'START開始;APPROVAL簽核;CONDITION條件;PARALLEL並行;NOTIFY通知;END結束',
    approval_mode           VARCHAR(30)                             NULL COMMENT 'ANY任一;ALL全數;MAJORITY多數;SEQUENTIAL依序',
    approver_type           VARCHAR(30)                             NULL COMMENT 'USER指定人;ROLE角色;DEPARTMENT部門;MANAGER主管;FORM_FIELD表單欄位',
    approver_value          VARCHAR(500)                            NULL COMMENT '簽核人設定值',
    timeout_hours           INT                                     NULL COMMENT '逾時小時數',
    timeout_action          VARCHAR(30)                             NULL COMMENT 'REMIND提醒;ESCALATE升級;AUTO_APPROVE自動核准;AUTO_REJECT自動駁回',
    allow_return            TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否允許退回',
    allow_delegate          TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否允許代理',
    allow_add_approver      TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否允許加簽',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_wfn_version
        FOREIGN KEY (workflow_version_nid) REFERENCES wf_definition_version(nid),
    CONSTRAINT uk_wfn_version_code UNIQUE (workflow_version_nid, node_code),
    INDEX idx_wfn_version_nid (workflow_version_nid),
    INDEX idx_wfn_node_type (node_type),
    INDEX idx_wfn_approver_type (approver_type),
    INDEX idx_wfn_sort_no (sort_no),
    INDEX idx_wfn_avalible (avalible),
    CHECK (timeout_hours IS NULL OR timeout_hours > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='簽核流程節點';

CREATE TABLE IF NOT EXISTS wf_transition (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '流程路由序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    workflow_version_nid    BIGINT UNSIGNED                     NOT NULL COMMENT '流程版本流水號',
    from_node_nid           BIGINT UNSIGNED                     NOT NULL COMMENT '來源節點流水號',
    to_node_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '目標節點流水號',
    transition_code         VARCHAR(100)                        NOT NULL COMMENT '路由代碼',
    transition_name         VARCHAR(200)                            NULL COMMENT '路由名稱',
    action_code             VARCHAR(50)                             NULL COMMENT 'SUBMIT送出;APPROVE核准;REJECT駁回;RETURN退回;TIMEOUT逾時',
    condition_expression    TEXT                                    NULL COMMENT '條件運算式',
    condition_config        JSON                                    NULL COMMENT '條件設定JSON',
    priority                INT                                 NOT NULL DEFAULT 0 COMMENT '路由優先順序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_wft_version
        FOREIGN KEY (workflow_version_nid) REFERENCES wf_definition_version(nid),
    CONSTRAINT fk_wft_from_node
        FOREIGN KEY (from_node_nid) REFERENCES wf_node(nid),
    CONSTRAINT fk_wft_to_node
        FOREIGN KEY (to_node_nid) REFERENCES wf_node(nid),
    CONSTRAINT uk_wft_transition_code UNIQUE (workflow_version_nid, transition_code),
    INDEX idx_wft_version_nid (workflow_version_nid),
    INDEX idx_wft_from_node_nid (from_node_nid),
    INDEX idx_wft_to_node_nid (to_node_nid),
    INDEX idx_wft_action_code (action_code),
    INDEX idx_wft_priority (priority),
    INDEX idx_wft_avalible (avalible),
    CHECK (from_node_nid <> to_node_nid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='簽核流程路由';

CREATE TABLE IF NOT EXISTS wf_node_approver (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '節點簽核人設定序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    node_nid                BIGINT UNSIGNED                     NOT NULL COMMENT '流程節點流水號',
    approver_type           VARCHAR(30)                         NOT NULL COMMENT 'USER;ROLE;DEPARTMENT;POSITION;MANAGER;FORM_FIELD',
    approver_sid            VARCHAR(32)                             NULL COMMENT '指定對象序號',
    approver_expression     VARCHAR(1000)                           NULL COMMENT '動態簽核人運算式',
    sequence_no             INT                                 NOT NULL DEFAULT 1 COMMENT '簽核順序',
    required_approval       TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否必須簽核',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_wfna_node
        FOREIGN KEY (node_nid) REFERENCES wf_node(nid),
    INDEX idx_wfna_node_nid (node_nid),
    INDEX idx_wfna_approver (approver_type, approver_sid),
    INDEX idx_wfna_sequence_no (sequence_no),
    INDEX idx_wfna_avalible (avalible),
    CHECK (sequence_no > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='流程節點簽核人設定';

-- =========================================================
-- 03. 表單與欄位定義
-- =========================================================

CREATE TABLE IF NOT EXISTS wf_form_definition (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '流程表單定義序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    workflow_definition_nid BIGINT UNSIGNED                     NOT NULL COMMENT '流程定義流水號',
    form_code               VARCHAR(100)                        NOT NULL COMMENT '表單代碼',
    form_name               VARCHAR(200)                        NOT NULL COMMENT '表單名稱',
    form_schema             JSON                                NOT NULL COMMENT '表單Schema',
    validation_schema       JSON                                    NULL COMMENT '驗證規則Schema',
    display_schema          JSON                                    NULL COMMENT '畫面顯示Schema',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_wffd_definition
        FOREIGN KEY (workflow_definition_nid) REFERENCES wf_definition(nid),
    CONSTRAINT uk_wffd_form_code UNIQUE (workflow_definition_nid, form_code),
    INDEX idx_wffd_definition_nid (workflow_definition_nid),
    INDEX idx_wffd_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='簽核流程表單定義';

-- =========================================================
-- 04. 流程實例
-- =========================================================

CREATE TABLE IF NOT EXISTS wf_instance (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '流程實例序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    instance_no             VARCHAR(60)                         NOT NULL COMMENT '流程實例編號',
    workflow_definition_sid VARCHAR(32)                         NOT NULL COMMENT '流程定義序號',
    workflow_version_no     INT                                 NOT NULL COMMENT '流程版本號',
    business_type           VARCHAR(100)                        NOT NULL COMMENT '業務類型',
    business_sid            VARCHAR(32)                         NOT NULL COMMENT '業務資料序號',
    business_no             VARCHAR(100)                            NULL COMMENT '業務單號',
    business_title          VARCHAR(500)                            NULL COMMENT '業務標題',
    applicant_user_sid      VARCHAR(32)                         NOT NULL COMMENT '申請人帳號序號',
    applicant_employee_sid  VARCHAR(32)                             NULL COMMENT '申請人員工序號',
    applicant_department_sid VARCHAR(32)                            NULL COMMENT '申請部門序號',
    current_node_code       VARCHAR(100)                            NULL COMMENT '目前節點代碼',
    instance_status         VARCHAR(30)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;RUNNING簽核中;APPROVED核准;REJECTED駁回;RETURNED退回;CANCELLED取消;COMPLETED完成',
    start_date              DATETIME                                NULL COMMENT '送簽時間',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    due_date                DATETIME                                NULL COMMENT '預計完成時間',
    form_data               JSON                                    NULL COMMENT '簽呈表單資料',
    source_service_code     VARCHAR(80)                         NOT NULL COMMENT '來源服務代碼',
    correlation_id          VARCHAR(100)                            NULL COMMENT '跨服務關聯識別碼',
    create_user_sid         VARCHAR(32)                             NULL COMMENT '建立人員序號',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_wfi_instance_no UNIQUE (instance_no),
    CONSTRAINT uk_wfi_business UNIQUE (business_type, business_sid),
    INDEX idx_wfi_workflow_definition_sid (workflow_definition_sid),
    INDEX idx_wfi_business (business_type, business_sid),
    INDEX idx_wfi_applicant_user_sid (applicant_user_sid),
    INDEX idx_wfi_department_sid (applicant_department_sid),
    INDEX idx_wfi_current_node_code (current_node_code),
    INDEX idx_wfi_status (instance_status),
    INDEX idx_wfi_start_date (start_date),
    INDEX idx_wfi_due_date (due_date),
    INDEX idx_wfi_correlation_id (correlation_id),
    INDEX idx_wfi_avalible (avalible),
    CHECK (workflow_version_no > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='簽核流程實例';

CREATE TABLE IF NOT EXISTS wf_instance_node (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '流程實例節點序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    workflow_instance_nid   BIGINT UNSIGNED                     NOT NULL COMMENT '流程實例流水號',
    node_code               VARCHAR(100)                        NOT NULL COMMENT '節點代碼快照',
    node_name               VARCHAR(200)                        NOT NULL COMMENT '節點名稱快照',
    node_type               VARCHAR(30)                         NOT NULL COMMENT '節點類型',
    sequence_no             INT                                 NOT NULL COMMENT '執行順序',
    node_status             VARCHAR(30)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;ACTIVE處理中;APPROVED核准;REJECTED駁回;RETURNED退回;SKIPPED略過;CANCELLED取消',
    start_date              DATETIME                                NULL COMMENT '開始時間',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    due_date                DATETIME                                NULL COMMENT '節點期限',
    approval_mode           VARCHAR(30)                             NULL COMMENT 'ANY;ALL;MAJORITY;SEQUENTIAL',
    required_approvals      INT                                 NOT NULL DEFAULT 1 COMMENT '所需核准數',
    approved_count          INT                                 NOT NULL DEFAULT 0 COMMENT '已核准數',
    rejected_count          INT                                 NOT NULL DEFAULT 0 COMMENT '已駁回數',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_wfin_instance
        FOREIGN KEY (workflow_instance_nid) REFERENCES wf_instance(nid),
    CONSTRAINT uk_wfin_instance_sequence UNIQUE (workflow_instance_nid, sequence_no),
    INDEX idx_wfin_instance_nid (workflow_instance_nid),
    INDEX idx_wfin_node_code (node_code),
    INDEX idx_wfin_status (node_status),
    INDEX idx_wfin_due_date (due_date),
    CHECK (sequence_no > 0),
    CHECK (required_approvals > 0),
    CHECK (approved_count >= 0),
    CHECK (rejected_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='流程實例節點';

-- =========================================================
-- 05. 簽核任務與紀錄
-- =========================================================

CREATE TABLE IF NOT EXISTS wf_task (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '簽核任務序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    workflow_instance_nid   BIGINT UNSIGNED                     NOT NULL COMMENT '流程實例流水號',
    instance_node_nid       BIGINT UNSIGNED                     NOT NULL COMMENT '流程實例節點流水號',
    task_no                 VARCHAR(60)                         NOT NULL COMMENT '簽核任務編號',
    assignee_type           VARCHAR(30)                         NOT NULL COMMENT 'USER帳號;ROLE角色;DEPARTMENT部門;POSITION職務',
    assignee_sid            VARCHAR(32)                         NOT NULL COMMENT '被指派對象序號',
    actual_user_sid         VARCHAR(32)                             NULL COMMENT '實際處理帳號序號',
    delegated_from_sid      VARCHAR(32)                             NULL COMMENT '代理來源帳號序號',
    task_status             VARCHAR(30)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待辦;CLAIMED已認領;APPROVED核准;REJECTED駁回;RETURNED退回;DELEGATED已代理;CANCELLED取消;EXPIRED逾期',
    assigned_date           DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '指派時間',
    claimed_date            DATETIME                                NULL COMMENT '認領時間',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    due_date                DATETIME                                NULL COMMENT '處理期限',
    priority                VARCHAR(20)                         NOT NULL DEFAULT 'NORMAL' COMMENT 'LOW低;NORMAL一般;HIGH高;URGENT緊急',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_wftask_instance
        FOREIGN KEY (workflow_instance_nid) REFERENCES wf_instance(nid),
    CONSTRAINT fk_wftask_instance_node
        FOREIGN KEY (instance_node_nid) REFERENCES wf_instance_node(nid),
    CONSTRAINT uk_wftask_task_no UNIQUE (task_no),
    INDEX idx_wftask_instance_nid (workflow_instance_nid),
    INDEX idx_wftask_instance_node_nid (instance_node_nid),
    INDEX idx_wftask_assignee (assignee_type, assignee_sid),
    INDEX idx_wftask_actual_user_sid (actual_user_sid),
    INDEX idx_wftask_status (task_status),
    INDEX idx_wftask_due_date (due_date),
    INDEX idx_wftask_priority (priority)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='簽核待辦任務';

CREATE TABLE IF NOT EXISTS wf_action_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '簽核動作紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '動作時間',
    workflow_instance_nid   BIGINT UNSIGNED                     NOT NULL COMMENT '流程實例流水號',
    instance_node_nid       BIGINT UNSIGNED                         NULL COMMENT '流程實例節點流水號',
    task_sid                VARCHAR(32)                             NULL COMMENT '簽核任務序號',
    action_code             VARCHAR(30)                         NOT NULL COMMENT 'SUBMIT送出;APPROVE核准;REJECT駁回;RETURN退回;CANCEL取消;DELEGATE代理;ADD_APPROVER加簽;WITHDRAW撤回',
    action_user_sid         VARCHAR(32)                         NOT NULL COMMENT '操作帳號序號',
    action_employee_sid     VARCHAR(32)                             NULL COMMENT '操作員工序號',
    from_node_code          VARCHAR(100)                            NULL COMMENT '來源節點代碼',
    to_node_code            VARCHAR(100)                            NULL COMMENT '目標節點代碼',
    action_comment          TEXT                                    NULL COMMENT '簽核意見',
    attachment_json         JSON                                    NULL COMMENT '附件清單',
    ip_address              VARCHAR(50)                             NULL COMMENT 'IP位址',
    user_agent              VARCHAR(1000)                           NULL COMMENT '瀏覽器資訊',
    CONSTRAINT fk_wfal_instance
        FOREIGN KEY (workflow_instance_nid) REFERENCES wf_instance(nid),
    CONSTRAINT fk_wfal_instance_node
        FOREIGN KEY (instance_node_nid) REFERENCES wf_instance_node(nid),
    INDEX idx_wfal_instance_nid (workflow_instance_nid),
    INDEX idx_wfal_instance_node_nid (instance_node_nid),
    INDEX idx_wfal_task_sid (task_sid),
    INDEX idx_wfal_action_code (action_code),
    INDEX idx_wfal_action_user_sid (action_user_sid),
    INDEX idx_wfal_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='簽核動作紀錄';

-- =========================================================
-- 06. 代理與加簽
-- =========================================================

CREATE TABLE IF NOT EXISTS wf_delegate (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '簽核代理序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    principal_user_sid      VARCHAR(32)                         NOT NULL COMMENT '原簽核人帳號序號',
    delegate_user_sid       VARCHAR(32)                         NOT NULL COMMENT '代理人帳號序號',
    workflow_code           VARCHAR(100)                            NULL COMMENT '限定流程代碼，NULL代表全部',
    start_date              DATETIME                            NOT NULL COMMENT '代理開始時間',
    end_date                DATETIME                            NOT NULL COMMENT '代理結束時間',
    delegate_reason         VARCHAR(500)                            NULL COMMENT '代理原因',
    delegate_status         VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;CANCELLED取消;EXPIRED過期',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_wfd_delegate UNIQUE (principal_user_sid, delegate_user_sid, workflow_code, start_date),
    INDEX idx_wfd_principal_user_sid (principal_user_sid),
    INDEX idx_wfd_delegate_user_sid (delegate_user_sid),
    INDEX idx_wfd_workflow_code (workflow_code),
    INDEX idx_wfd_date (start_date, end_date),
    INDEX idx_wfd_status (delegate_status),
    INDEX idx_wfd_avalible (avalible),
    CHECK (principal_user_sid <> delegate_user_sid),
    CHECK (end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='簽核代理設定';

CREATE TABLE IF NOT EXISTS wf_additional_approver (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '加簽紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    workflow_instance_nid   BIGINT UNSIGNED                     NOT NULL COMMENT '流程實例流水號',
    instance_node_nid       BIGINT UNSIGNED                     NOT NULL COMMENT '流程實例節點流水號',
    requester_user_sid      VARCHAR(32)                         NOT NULL COMMENT '加簽申請人帳號序號',
    approver_user_sid       VARCHAR(32)                         NOT NULL COMMENT '被加簽人帳號序號',
    add_type                VARCHAR(20)                         NOT NULL DEFAULT 'PARALLEL' COMMENT 'PARALLEL並簽;SEQUENTIAL順簽;CONSULT會簽',
    add_reason              VARCHAR(500)                            NULL COMMENT '加簽原因',
    task_sid                VARCHAR(32)                             NULL COMMENT '產生的簽核任務序號',
    add_status              VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;APPROVED核准;REJECTED駁回;CANCELLED取消',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    CONSTRAINT fk_wfaa_instance
        FOREIGN KEY (workflow_instance_nid) REFERENCES wf_instance(nid),
    CONSTRAINT fk_wfaa_instance_node
        FOREIGN KEY (instance_node_nid) REFERENCES wf_instance_node(nid),
    INDEX idx_wfaa_instance_nid (workflow_instance_nid),
    INDEX idx_wfaa_instance_node_nid (instance_node_nid),
    INDEX idx_wfaa_requester_user_sid (requester_user_sid),
    INDEX idx_wfaa_approver_user_sid (approver_user_sid),
    INDEX idx_wfaa_status (add_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='流程加簽紀錄';

-- =========================================================
-- 07. 催辦與通知
-- =========================================================

CREATE TABLE IF NOT EXISTS wf_reminder (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '催辦紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    workflow_instance_nid   BIGINT UNSIGNED                     NOT NULL COMMENT '流程實例流水號',
    task_sid                VARCHAR(32)                         NOT NULL COMMENT '簽核任務序號',
    reminder_type           VARCHAR(20)                         NOT NULL COMMENT 'AUTO系統;MANUAL人工;ESCALATION升級',
    receiver_user_sid       VARCHAR(32)                         NOT NULL COMMENT '接收人帳號序號',
    reminder_count          INT                                 NOT NULL DEFAULT 1 COMMENT '催辦次數',
    reminder_message        TEXT                                    NULL COMMENT '催辦訊息',
    notification_sid        VARCHAR(32)                             NULL COMMENT 'NotificationDB通知序號',
    send_status             VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待送;SUCCESS成功;FAILED失敗',
    sent_date               DATETIME                                NULL COMMENT '發送時間',
    CONSTRAINT fk_wfr_instance
        FOREIGN KEY (workflow_instance_nid) REFERENCES wf_instance(nid),
    INDEX idx_wfr_instance_nid (workflow_instance_nid),
    INDEX idx_wfr_task_sid (task_sid),
    INDEX idx_wfr_receiver_user_sid (receiver_user_sid),
    INDEX idx_wfr_send_status (send_status),
    INDEX idx_wfr_create_date (create_date),
    CHECK (reminder_count > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='簽核催辦紀錄';

-- =========================================================
-- 08. 流程事件
-- =========================================================

CREATE TABLE IF NOT EXISTS wf_event_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '流程事件序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    workflow_instance_nid   BIGINT UNSIGNED                     NOT NULL COMMENT '流程實例流水號',
    event_code              VARCHAR(100)                        NOT NULL COMMENT '事件代碼',
    event_name              VARCHAR(200)                            NULL COMMENT '事件名稱',
    event_data              JSON                                    NULL COMMENT '事件資料',
    source_service_code     VARCHAR(80)                             NULL COMMENT '來源服務代碼',
    outbox_event_sid        VARCHAR(32)                             NULL COMMENT 'IntegrationDB Outbox事件序號',
    process_status          VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;SUCCESS成功;FAILED失敗',
    processed_date          DATETIME                                NULL COMMENT '處理時間',
    error_message           TEXT                                    NULL COMMENT '錯誤訊息',
    CONSTRAINT fk_wfel_instance
        FOREIGN KEY (workflow_instance_nid) REFERENCES wf_instance(nid),
    INDEX idx_wfel_instance_nid (workflow_instance_nid),
    INDEX idx_wfel_event_code (event_code),
    INDEX idx_wfel_process_status (process_status),
    INDEX idx_wfel_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='簽核流程事件紀錄';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
