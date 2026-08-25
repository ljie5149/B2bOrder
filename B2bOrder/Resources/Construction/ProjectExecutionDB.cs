namespace B2bOrder.Resources.Construction
{
    /// <summary>
    /// ProjectExecutionDB V1 Shared Core Schema (建築商 / 營造 ERP 專用)
    /// 設計目標：
    /// 1. 施工日誌與出工管理 (Site Daily Log & Labor Entry)：紀錄每日氣候、出工數、施工區劃與現場施工概要
    /// 2. 施工品質與安衛缺失改善單 (Punch List & Defect Management)：追蹤工安與品質缺失、責任廠商、限期改善與結案歷程
    /// 3. 材料與機具進場抽驗 (Material & Equipment Inspection Log)：紀錄材料進場檢驗、試體抽驗、重大機具進場合格證
    /// 4. 施工自主檢查 (Site Quality Self-Inspection Checklist)：管理各工程階段 (如混凝土澆置前) 的品質自主檢查單
    /// 5. 施工疑義澄清單 (Request for Information - RFI)：紀錄現場與設計圖說不符之 RFI 疑義 clarification 與回覆
    /// 6. 整合串接 ProjectDB (WBS進度)、ContractDB (廠商)、VendorDB 與 ChangeOrderDB (追加減)
    /// 7. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 8. avalible：Y可用；W停用；D刪除
    /// 9. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class ProjectExecutionDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 工地施工日誌主檔 (Daily Site Log Master)
-- =========================================================

CREATE TABLE IF NOT EXISTS pex_daily_site_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '施工日誌序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    log_date                DATE                                NOT NULL COMMENT '施工日期',
    
    -- 環境與氣候
    weather_am              VARCHAR(30)                             NULL COMMENT '上午天氣 (例: 晴天, 陰天, 豪雨)',
    weather_pm              VARCHAR(30)                             NULL COMMENT '下午天氣',
    temperature_celsius     DECIMAL(4,1)                            NULL COMMENT '平均氣溫 (度)',
    
    -- 統計數據
    total_workers_count     INT                                 NOT NULL DEFAULT 0 COMMENT '當日現場出工總人數',
    total_machinery_count   INT                                 NOT NULL DEFAULT 0 COMMENT '當日進場機具總台數',
    
    -- 施工概要
    work_summary            TEXT                                NOT NULL COMMENT '當日主要施工項目與進度概要',
    safety_health_summary   TEXT                                    NULL COMMENT '勞安與衛生管理摘要',
    site_manager_user_sid   VARCHAR(32)                         NOT NULL COMMENT '填報工地主任/工程師帳號序號',
    log_status              VARCHAR(20)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;SUBMITTED送審中;APPROVED監造已審核',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_pdsl_project_date UNIQUE (project_sid, log_date),
    INDEX idx_pdsl_company_sid (company_sid),
    INDEX idx_pdsl_project_sid (project_sid),
    INDEX idx_pdsl_log_date (log_date),
    INDEX idx_pdsl_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工地每日施工日誌主檔';

-- =========================================================
-- 02. 每日出工與工種統計明細 (Daily Labor & Subcontractor Dispatch)
-- =========================================================

CREATE TABLE IF NOT EXISTS pex_daily_labor_entry (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '出工明細序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    daily_log_sid           VARCHAR(32)                         NOT NULL COMMENT '對應 pex_daily_site_log.sid',
    vendor_sid              VARCHAR(32)                         NOT NULL COMMENT '外包廠商序號 (VendorDB SID)',
    wbs_sid                 VARCHAR(32)                             NULL COMMENT '對應 ProjectDB WBS 任務序號',
    trade_type              VARCHAR(50)                         NOT NULL COMMENT '工種類別 (例: 模板工, 鋼筋工, 泥作工, 電線工)',
    worker_count            INT                                 NOT NULL DEFAULT 0 COMMENT '該工種出工人數',
    work_location           VARCHAR(200)                            NULL COMMENT '施工位置/區劃 (例: B1F 區鋼筋綁紮)',
    work_description        TEXT                                    NULL COMMENT '具體工作內容描述',
    INDEX idx_pdle_daily_log_sid (daily_log_sid),
    INDEX idx_pdle_vendor_sid (vendor_sid),
    INDEX idx_pdle_wbs_sid (wbs_sid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工地每日出工與工種明細檔';

-- =========================================================
-- 03. 施工缺失與改善單 (Punch List & Defect Inspection)
-- =========================================================

CREATE TABLE IF NOT EXISTS pex_defect_punch_list (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '缺失單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '開單日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案序號',
    daily_log_sid           VARCHAR(32)                             NULL COMMENT '關聯施工日誌序號',
    contractor_vendor_sid   VARCHAR(32)                         NOT NULL COMMENT '權責外包廠商序號',
    wbs_sid                 VARCHAR(32)                             NULL COMMENT '關聯 WBS 工項序號',
    
    defect_no               VARCHAR(50)                         NOT NULL COMMENT '缺失單號 (例: PUN-202608-012)',
    defect_category         VARCHAR(30)                         NOT NULL DEFAULT 'SAFETY' COMMENT '缺失類別: SAFETY勞安缺失, QUALITY品質瑕疵, ENVIRONMENT環保缺失, SCHEDULE進度落後',
    severity_level          VARCHAR(20)                         NOT NULL DEFAULT 'MEDIUM' COMMENT '嚴重等級: MINOR輕微, MEDIUM中度, CRITICAL嚴重/立即停工',
    location_description    VARCHAR(200)                        NOT NULL COMMENT '缺失精確位置 (例: A棟12F樓梯間未設防墜網)',
    defect_description      TEXT                                NOT NULL COMMENT '缺失詳細內容說明',
    deadline_date           DATE                                    NULL COMMENT '要求限期改善完成日期',
    
    rectification_status    VARCHAR(20)                         NOT NULL DEFAULT 'OPEN' COMMENT 'OPEN待改善;SUBMITTED已改善待覆查;CLOSED複查合格結案;REJECTED退回重修',
    inspector_user_sid      VARCHAR(32)                         NOT NULL COMMENT '稽核安衛/品質人員帳號序號',
    closed_by_user_sid      VARCHAR(32)                             NULL COMMENT '結案簽核主管序號',
    closed_at               DATETIME                                NULL COMMENT '結案時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '改善結果說明/備註',
    CONSTRAINT uk_pdpl_project_number UNIQUE (project_sid, defect_no),
    INDEX idx_pdpl_project_sid (project_sid),
    INDEX idx_pdpl_vendor_sid (contractor_vendor_sid),
    INDEX idx_pdpl_status (rectification_status),
    INDEX idx_pdpl_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='施工與安衛缺失改善單檔 (Punch List)';

-- =========================================================
-- 04. 材料與機具進場檢驗紀錄 (Material & Machinery Entry Inspection)
-- =========================================================

CREATE TABLE IF NOT EXISTS pex_material_machinery_inspection (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '進場檢驗序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '進場/抽驗日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案序號',
    vendor_sid              VARCHAR(32)                         NOT NULL COMMENT '供應商/廠商序號',
    entry_type              VARCHAR(20)                         NOT NULL DEFAULT 'MATERIAL' COMMENT '進場類別: MATERIAL材料進場, MACHINERY機具進場',
    item_name               VARCHAR(200)                        NOT NULL COMMENT '進場品項/機具名稱 (例: 4000psi 混凝土, 100噸移動式吊車)',
    specification           VARCHAR(200)                            NULL COMMENT '規格/型號 (例: SD420 鋼筋 #6)',
    quantity                DECIMAL(12,4)                       NOT NULL DEFAULT 0.0000 COMMENT '進場數量',
    unit                    VARCHAR(20)                             NULL COMMENT '單位 (例: m3, 噸, 台)',
    inspection_result       VARCHAR(20)                         NOT NULL DEFAULT 'PASS' COMMENT '檢驗結果: PASS合格, FAIL不合格退回, CONDITIONALLY_PASS條件接收',
    test_report_number      VARCHAR(100)                            NULL COMMENT '抽驗/試體報告編號',
    inspector_user_sid      VARCHAR(32)                         NOT NULL COMMENT '抽驗人員序號',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_pmmi_project_sid (project_sid),
    INDEX idx_pmmi_vendor_sid (vendor_sid),
    INDEX idx_pmmi_entry_type (entry_type),
    INDEX idx_pmmi_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='材料與機具進場抽驗紀錄檔';

-- =========================================================
-- 05. 施工疑義澄清單 (Request for Information - RFI)
-- =========================================================

CREATE TABLE IF NOT EXISTS pex_request_for_information (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'RFI 序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '提問日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案序號',
    wbs_sid                 VARCHAR(32)                             NULL COMMENT '關聯 WBS 任務序號',
    rfi_number              VARCHAR(50)                         NOT NULL COMMENT 'RFI 編號 (例: RFI-2026-009)',
    rfi_subject             VARCHAR(200)                        NOT NULL COMMENT '疑義主題 (例: B1F 管道間與結構樑干涉疑義澄清)',
    drawing_reference_no    VARCHAR(100)                            NULL COMMENT '關聯圖說編號 (例: A-102, S-301)',
    question_description    TEXT                                NOT NULL COMMENT '疑義問題詳細描述',
    proposed_solution       TEXT                                    NULL COMMENT '現場建議解決方案',
    
    -- 答覆與審核
    response_description    TEXT                                    NULL COMMENT '建築師/結構技師答覆說明',
    responded_by_name       VARCHAR(100)                            NULL COMMENT '答覆技師/單位名稱',
    responded_at            DATETIME                                NULL COMMENT '答覆時間',
    is_cost_impact          VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT '是否衍生費用變更 (Y/N，若Y後續轉 ChangeOrder)',
    is_schedule_impact      VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT '是否衍生工期影響 (Y/N)',
    rfi_status              VARCHAR(20)                         NOT NULL DEFAULT 'SUBMITTED' COMMENT 'SUBMITTED已提出;ANSWERED已答覆;CLOSED結案',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_prfi_project_number UNIQUE (project_sid, rfi_number),
    INDEX idx_prfi_project_sid (project_sid),
    INDEX idx_prfi_status (rfi_status),
    INDEX idx_prfi_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='施工疑義澄清單檔 (RFI)';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}