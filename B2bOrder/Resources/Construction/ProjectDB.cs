namespace B2bOrder.Resources.Construction
{
    /// <summary>
    /// ProjectDB V1 Shared Core Schema (建築商 / 營造 ERP 專用)
    /// 設計目標：
    /// 1. 建案與工程主檔 (Project & Site Master)：管理基地面積、建照/使照字號、開竣工日期與專案負責人
    /// 2. 工作分解結構與工程進度 (WBS & Progress Tracking)：多層級 WBS (如地基、結構、裝修)、預計/實際進度百分比
    /// 3. 工程預算與變更追加減 (Project Budget & Variation Orders)：發包總預算、追加減變更單 (VO) 與預算控制
    /// 4. 工程估驗與請款 (Progress Payment Certificates)：紀錄營造廠商期數估驗、扣留保留金、預付款扣抵與核准金額
    /// 5. 工地日誌與安衛缺失 (Daily Site Log & Safety Inspection)：施工日誌、出工數、氣候、施工缺失單 (Punch List)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class ProjectDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 建案與工程案場主檔 (Project & Site Master)
-- =========================================================

CREATE TABLE IF NOT EXISTS prj_project_master (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '建案專案序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司/建設公司序號',
    project_code            VARCHAR(50)                         NOT NULL COMMENT '建案/工程編號 (例: PRJ-2026-001)',
    project_name            VARCHAR(200)                        NOT NULL COMMENT '建案/工程名稱 (例: 遠雄晴空豪景建案)',
    project_type            VARCHAR(30)                         NOT NULL DEFAULT 'RESIDENTIAL' COMMENT '建案類型: RESIDENTIAL住宅, COMMERCIAL商業大樓, INDUSTRIAL廠房, MIXED綜合開發',
    land_area_sqm           DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '基地面積 (平方公尺)',
    total_floor_area_sqm    DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '總樓地板面積 (平方公尺)',
    building_license_no     VARCHAR(100)                            NULL COMMENT '建造執照字號',
    usage_license_no        VARCHAR(100)                            NULL COMMENT '使用執照字號',
    site_address            VARCHAR(300)                            NULL COMMENT '基地地址/地號 description',
    project_manager_sid     VARCHAR(32)                             NULL COMMENT '專案經理/PM 帳號序號',
    planned_start_date      DATE                                    NULL COMMENT '預計開工日期',
    planned_end_date        DATE                                    NULL COMMENT '預計完工日期',
    actual_start_date       DATE                                    NULL COMMENT '實際開工日期',
    actual_end_date         DATE                                    NULL COMMENT '實際完工日期',
    project_status          VARCHAR(20)                         NOT NULL DEFAULT 'PLANNING' COMMENT 'PLANNING規劃中;LICENSING執照申請;CONSTRUCTING施工中;INSPECTION驗收中;COMPLETED已完工;SUSPENDED停工',
    total_budget_amount     DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '建案總發包預算金額',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ppm_company_code UNIQUE (company_sid, project_code),
    INDEX idx_ppm_company_sid (company_sid),
    INDEX idx_ppm_status (project_status),
    INDEX idx_ppm_manager (project_manager_sid),
    INDEX idx_ppm_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='建案與工程主檔';

-- =========================================================
-- 02. 工程工作分解結構 (Work Breakdown Structure - WBS)
-- =========================================================

CREATE TABLE IF NOT EXISTS prj_wbs_task (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'WBS 任務序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '對應 prj_project_master.sid',
    parent_wbs_sid          VARCHAR(32)                             NULL COMMENT '父層 WBS 序號 (第一層則為 NULL)',
    wbs_code                VARCHAR(50)                         NOT NULL COMMENT 'WBS 編碼 (例: 1.1.2)',
    task_name               VARCHAR(200)                        NOT NULL COMMENT '工項名稱 (例: 地下室連續壁工程, 10F結構體混凝土澆置)',
    wbs_level               INT                                 NOT NULL DEFAULT 1 COMMENT 'WBS 階層深度 (1, 2, 3...)',
    sequence_no             INT                                 NOT NULL DEFAULT 0 COMMENT '同階層排序順序',
    contractor_vendor_sid   VARCHAR(32)                             NULL COMMENT '負責承包廠商序號 (VendorDB SID)',
    budget_amount           DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '該工項預算金額',
    planned_start_date      DATE                                    NULL COMMENT '預計開始日期',
    planned_end_date        DATE                                    NULL COMMENT '預計完成日期',
    actual_start_date       DATE                                    NULL COMMENT '實際開始日期',
    actual_end_date         DATE                                    NULL COMMENT '實際完成日期',
    completion_rate         DECIMAL(5,2)                        NOT NULL DEFAULT 0.00 COMMENT '完工進度百分比 (0.00% ~ 100.00%)',
    task_status             VARCHAR(20)                         NOT NULL DEFAULT 'NOT_STARTED' COMMENT 'NOT_STARTED未開始;IN_PROGRESS施工中;PAUSED停工;COMPLETED已完成',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_pwt_project_code UNIQUE (project_sid, wbs_code),
    INDEX idx_pwt_project_sid (project_sid),
    INDEX idx_pwt_parent_wbs (parent_wbs_sid),
    INDEX idx_pwt_vendor (contractor_vendor_sid),
    INDEX idx_pwt_status (task_status),
    INDEX idx_pwt_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程工作分解結構 WBS 任務檔';

-- =========================================================
-- 03. 工程變更與追加減單 (Variation Order / Change Order)
-- =========================================================

CREATE TABLE IF NOT EXISTS prj_variation_order (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '變更單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '申請日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案序號',
    wbs_sid                 VARCHAR(32)                             NULL COMMENT '關聯 WBS 工項序號',
    vo_number               VARCHAR(50)                         NOT NULL COMMENT '變更單號 (例: VO-202604-001)',
    vo_title                VARCHAR(200)                        NOT NULL COMMENT '變更主旨/工程追加減原因',
    contractor_vendor_sid   VARCHAR(32)                         NOT NULL COMMENT '承包廠商序號',
    original_amount         DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '原合約金額',
    change_amount           DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '追加(正數)或追減(負數)金額',
    revised_amount          DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '變更後新總價',
    extended_days           INT                                 NOT NULL DEFAULT 0 COMMENT '展延工期天數',
    vo_status               VARCHAR(20)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;SUBMITTED送審中;APPROVED已核准;REJECTED已退回;CANCELLED已廢棄',
    approved_by_user_sid    VARCHAR(32)                             NULL COMMENT '核准主管序號',
    approved_at             DATETIME                                NULL COMMENT '核准時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '變更詳細說明',
    CONSTRAINT uk_pvo_project_number UNIQUE (project_sid, vo_number),
    INDEX idx_pvo_project_sid (project_sid),
    INDEX idx_pvo_vendor (contractor_vendor_sid),
    INDEX idx_pvo_status (vo_status),
    INDEX idx_pvo_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程變更與追加減單檔';

-- =========================================================
-- 04. 工程計價與估驗請款 (Progress Payment Certificate)
-- =========================================================

CREATE TABLE IF NOT EXISTS prj_progress_payment (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '估驗單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '申請日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案序號',
    contractor_vendor_sid   VARCHAR(32)                         NOT NULL COMMENT '承包廠商序號',
    certificate_no          VARCHAR(50)                         NOT NULL COMMENT '估驗期數單號 (例: CERT-2026-P03)',
    period_number           INT                                 NOT NULL DEFAULT 1 COMMENT '估驗期數 (例: 第 3 期)',
    valuation_date          DATE                                NOT NULL COMMENT '估驗截止基準日',
    current_claimed_amount  DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本期申報估驗金額',
    current_approved_amount DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本期實核估驗金額',
    retention_deduction     DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本期扣留保留金 (例: 5% ~ 10%)',
    advance_deduction       DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本期扣抵預付款',
    net_payable_amount      DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本期應付淨額 (實核 - 保留金 - 扣預付)',
    payment_status          VARCHAR(20)                         NOT NULL DEFAULT 'SUBMITTED' COMMENT 'SUBMITTED送審中;INSPECTED現場監造簽核;APPROVED財務核准;PAID已撥款;REJECTED退件',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ppp_project_vendor_period UNIQUE (project_sid, contractor_vendor_sid, period_number),
    INDEX idx_ppp_project_sid (project_sid),
    INDEX idx_ppp_vendor (contractor_vendor_sid),
    INDEX idx_ppp_status (payment_status),
    INDEX idx_ppp_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程進度估驗與請款主檔';

-- =========================================================
-- 05. 工地施工日誌與安衛巡檢 (Daily Site Log & Safety)
-- =========================================================

CREATE TABLE IF NOT EXISTS prj_daily_site_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '施工日誌序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案序號',
    log_date                DATE                                NOT NULL COMMENT '施工日期',
    weather_am              VARCHAR(30)                             NULL COMMENT '上午天氣 (例: 晴天, 陰天, 豪雨)',
    weather_pm              VARCHAR(30)                             NULL COMMENT '下午天氣',
    temperature_celsius     DECIMAL(4,1)                            NULL COMMENT '平均氣溫',
    total_workers_count     INT                                 NOT NULL DEFAULT 0 COMMENT '當日出工總人數 (包含各外包廠商)',
    machinery_count         INT                                 NOT NULL DEFAULT 0 COMMENT '進場重大機具數量 (例: 吊車、挖掘機)',
    work_summary            TEXT                                NOT NULL COMMENT '當日施工概要說明',
    safety_issue_summary    TEXT                                    NULL COMMENT '工安缺失或巡檢摘要',
    site_engineer_user_sid  VARCHAR(32)                         NOT NULL COMMENT '填報工地主任/工程師帳號序號',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_pdsl_project_date UNIQUE (project_sid, log_date),
    INDEX idx_pdsl_project_sid (project_sid),
    INDEX idx_pdsl_log_date (log_date),
    INDEX idx_pdsl_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工地每日施工日誌檔';

CREATE TABLE IF NOT EXISTS prj_safety_punch_list (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '缺失單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '開單日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案序號',
    daily_log_sid           VARCHAR(32)                             NULL COMMENT '關聯施工日誌序號',
    contractor_vendor_sid   VARCHAR(32)                         NOT NULL COMMENT '權責外包廠商序號',
    issue_type              VARCHAR(30)                         NOT NULL DEFAULT 'SAFETY' COMMENT '缺失類別: SAFETY勞安缺失, QUALITY品質瑕疵, ENVIRONMENT環保缺失',
    issue_description       TEXT                                NOT NULL COMMENT '缺失內容詳細描述',
    location_description    VARCHAR(200)                            NULL COMMENT '缺失位置 (例: A棟12F樓梯間未設防墜網)',
    deadline_date           DATE                                    NULL COMMENT '要求限期改善日期',
    rectification_status    VARCHAR(20)                         NOT NULL DEFAULT 'OPEN' COMMENT 'OPEN待改善;IN_PROGRESS復查中;CLOSED已改善結案;REJECTED退回重修',
    inspector_user_sid      VARCHAR(32)                         NOT NULL COMMENT '稽核安衛人員序號',
    closed_at               DATETIME                                NULL COMMENT '結案時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_pspl_project_sid (project_sid),
    INDEX idx_pspl_vendor (contractor_vendor_sid),
    INDEX idx_pspl_status (rectification_status),
    INDEX idx_pspl_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工地安衛與品質缺失改善單 (Punch List)';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}