namespace B2bOrder.Resources.Construction
{
    /// <summary>
    /// QualitySafetyDB V1 Shared Core Schema (建築商 / 營造 ERP 專用)
    /// 設計目標：
    /// 1. 品質查驗與三級品管 (Quality Inspection Control)：紀錄隱蔽工程與分項工程品質查驗點 (Hold Points) 與檢驗結果
    /// 2. 安衛巡查與違規罰款 (Safety Violation & Penalty Log)：管理工地安衛違規事項、扣點與罰款單 (扣連估驗計價)
    /// 3. 工安事故與檢討處置 (Worksite Incident & Accident Report)：紀錄職災/事故等級、緊急處置、通報紀錄與改善對策 (CAPA)
    /// 4. 特殊作業進場許可與證照 (High-Risk Work Permits & Certifications)：追蹤吊掛、局限空間、高空作業許可證與人員證照
    /// 5. 整合串接 ProjectDB (建案)、ProjectExecutionDB (施工日誌)、VendorDB (外包商) 與 ProgressPaymentDB (計價扣罰)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class QualitySafetyDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 品質查驗紀錄檔 (Quality Inspection Record)
-- =========================================================

CREATE TABLE IF NOT EXISTS qsm_quality_inspection (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '品質查驗序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間/查驗時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    wbs_sid                 VARCHAR(32)                             NULL COMMENT '對應 ProjectDB WBS 任務序號',
    vendor_sid              VARCHAR(32)                         NOT NULL COMMENT '施工外包廠商序號 (VendorDB SID)',
    
    inspection_no           VARCHAR(50)                         NOT NULL COMMENT '查驗單號 (例: QIN-202608-005)',
    inspection_stage        VARCHAR(50)                         NOT NULL COMMENT '工程階段/品管類別 (例: REINFORCEMENT鋼筋工程, FORM_WORK模板工程, WATERPROOF防水工程)',
    location_description    VARCHAR(200)                        NOT NULL COMMENT '查驗精確位置 (例: A棟3F牆筋綁紮與預埋管)',
    is_hold_point           VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT '是否為隱蔽工程/停工檢查點 (Hold Point Y/N)',
    
    inspection_result       VARCHAR(20)                         NOT NULL DEFAULT 'PASS' COMMENT '結果: PASS合格, FAIL不合格限期改善, CONDITIONAL_PASS條件合格',
    inspector_user_sid      VARCHAR(32)                         NOT NULL COMMENT '品管人員/監造工程師帳號序號',
    reviewed_by_user_sid    VARCHAR(32)                             NULL COMMENT '品管主管覆核序號',
    reviewed_at             DATETIME                                NULL COMMENT '覆核時間',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '查驗意見與注意事項',
    CONSTRAINT uk_qqi_project_number UNIQUE (project_sid, inspection_no),
    INDEX idx_qqi_company_sid (company_sid),
    INDEX idx_qqi_project_sid (project_sid),
    INDEX idx_qqi_vendor_sid (vendor_sid),
    INDEX idx_qqi_result (inspection_result),
    INDEX idx_qqi_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='品質查驗與三級品管紀錄檔';

-- =========================================================
-- 02. 安衛違規稽核與罰款處分單 (Safety Violation & Penalty Order)
-- =========================================================

CREATE TABLE IF NOT EXISTS qsm_safety_penalty_order (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '罰單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '開單時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案專案序號',
    vendor_sid              VARCHAR(32)                         NOT NULL COMMENT '受罰外包廠商序號',
    penalty_no              VARCHAR(50)                         NOT NULL COMMENT '罰單單號 (例: SAF-202608-019)',
    
    violation_category      VARCHAR(50)                         NOT NULL COMMENT '違規類別 (例: HEIGHT_WORK高空未繫安全帶, ELECTRICAL用電安全, SCAFFOLD施工架未設護欄, PPE未戴安全帽)',
    violation_location      VARCHAR(200)                        NOT NULL COMMENT '違規地點 (例: B棟頂樓施工架)',
    violation_description   TEXT                                NOT NULL COMMENT '違規事實詳細說明',
    
    -- 懲處與扣款
    demerit_points          INT                                 NOT NULL DEFAULT 0 COMMENT '安衛扣點數',
    penalty_amount          DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '違規罰款金額 (直接連動 ProgressPayment 計價扣款)',
    is_deducted             VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT '是否已於估驗計價中執行扣款 (Y/N)',
    deducted_payment_sid    VARCHAR(32)                             NULL COMMENT '關聯之 ProgressPaymentDB 估驗單序號',
    
    safety_officer_user_sid VARCHAR(32)                         NOT NULL COMMENT '開單安衛人員帳號序號',
    status                  VARCHAR(20)                         NOT NULL DEFAULT 'ISSUED' COMMENT 'ISSUED已開單;APPEAL申訴中;CONFIRMED裁罰確定;CANCELLED撤銷',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '廠商改善說明或備註',
    CONSTRAINT uk_qspo_project_number UNIQUE (project_sid, penalty_no),
    INDEX idx_qspo_project_sid (project_sid),
    INDEX idx_qspo_vendor_sid (vendor_sid),
    INDEX idx_qspo_is_deducted (is_deducted),
    INDEX idx_qspo_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='安衛違規稽核與罰款處分單檔';

-- =========================================================
-- 03. 工地職業災害與事故通報檔 (Worksite Incident Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS qsm_worksite_incident_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '事故通報序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '通報時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案專案序號',
    vendor_sid              VARCHAR(32)                             NULL COMMENT '涉及外包廠商序號',
    incident_no             VARCHAR(50)                         NOT NULL COMMENT '事故編號 (例: INC-202608-001)',
    incident_time           DATETIME                            NOT NULL COMMENT '事故發生時間',
    
    severity_level          VARCHAR(20)                         NOT NULL DEFAULT 'MINOR' COMMENT '事故等級: NEAR_MISS虛驚事件, MINOR輕傷/人為疏失, SEVERE重傷/失能, FATAL死亡/重大職災',
    incident_type           VARCHAR(50)                         NOT NULL COMMENT '事故類型 (例: FALL墜落, STRUCTURAL倒塌, ELECTRIC_SHOCK觸電, OBJECT_FALL墜落物)',
    location_description    VARCHAR(200)                        NOT NULL COMMENT '發生地點',
    incident_summary        TEXT                                NOT NULL COMMENT '事故經過與現場狀況描述',
    injured_count           INT                                 NOT NULL DEFAULT 0 COMMENT '受傷人數',
    
    -- 通報與處置
    is_reported_to_gov      VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT '是否通報勞動檢查機構 (Y/N)',
    gov_report_at           DATETIME                                NULL COMMENT '通報主管機關時間',
    corrective_action       TEXT                                    NULL COMMENT '現場緊急處置與預防再發措施 (CAPA)',
    
    reporter_user_sid       VARCHAR(32)                         NOT NULL COMMENT '通報人員帳號序號',
    incident_status         VARCHAR(20)                         NOT NULL DEFAULT 'INVESTIGATING' COMMENT 'INVESTIGATING調查中;CAPA_PENDING改善對策中;CLOSED結案',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_qwil_project_number UNIQUE (project_sid, incident_no),
    INDEX idx_qwil_project_sid (project_sid),
    INDEX idx_qwil_vendor_sid (vendor_sid),
    INDEX idx_qwil_severity (severity_level),
    INDEX idx_qwil_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工地職業災害與事故通報檔';

-- =========================================================
-- 04. 高危險作業許可與進場證照檔 (High-Risk Work Permit & License)
-- =========================================================

CREATE TABLE IF NOT EXISTS qsm_high_risk_work_permit (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '作業許可序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '申請時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案專案序號',
    vendor_sid              VARCHAR(32)                         NOT NULL COMMENT '申請廠商序號',
    permit_no               VARCHAR(50)                         NOT NULL COMMENT '許可證號 (例: PER-202608-042)',
    
    work_type               VARCHAR(50)                         NOT NULL COMMENT '高危險作業類別: HOT_WORK動火作業, CONFINED_SPACE局限空間, CRANE_LIFTING重型吊裝, SCAFFOLD_ERECT高空架設',
    work_location           VARCHAR(200)                        NOT NULL COMMENT '施工作業區域',
    start_time              DATETIME                            NOT NULL COMMENT '許可作業開始時間',
    end_time                DATETIME                            NOT NULL COMMENT '許可作業結束時間',
    
    safety_measures_checked VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT '安全防護措施是否完全確認 (Y/N)',
    supervisor_user_sid     VARCHAR(32)                         NOT NULL COMMENT '現場安全監督員帳號序號',
    permit_status           VARCHAR(20)                         NOT NULL DEFAULT 'APPROVED' COMMENT 'SUBMITTED送審;APPROVED已核准;EXPIRED已過期;REVOKED強制撤銷',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_qhrwp_project_number UNIQUE (project_sid, permit_no),
    INDEX idx_qhrwp_project_sid (project_sid),
    INDEX idx_qhrwp_vendor_sid (vendor_sid),
    INDEX idx_qhrwp_status (permit_status),
    INDEX idx_qhrwp_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='高危險作業許可證檔';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}