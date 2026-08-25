namespace B2bOrder.Resources.Construction
{
    /// <summary>
    /// EquipmentDB V1 Shared Core Schema (建築商 / 營造 ERP 專用)
    /// 設計目標：
    /// 1. 施工機具資產主檔 (Equipment Asset Master)：管理重型設備 (塔吊/挖土機/發電機) 之產權 (自有/租賃)、規格與租率
    /// 2. 機具派遣與進退場紀錄 (Equipment Dispatch Log)：紀錄機具於各大建案/工地間的調撥、進退場時間與使用狀態
    /// 3. 機具保養與維修履歷 (Maintenance & Repair Log)：紀錄定保、故障報修、零件更換與維修費用
    /// 4. 法定安檢與特種證照 (Safety Certifications)：追蹤危險性機械設備合格證號、定期檢驗到期日與安檢狀態
    /// 5. 整合串接 ProjectDB (建案)、VendorDB (設備租賃商/維修商)、BudgetCostDB (機具成本歸攤) 與 QualitySafetyDB
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class EquipmentDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 施工機具資產主檔 (Equipment Asset Master)
-- =========================================================

CREATE TABLE IF NOT EXISTS eqp_equipment_master (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '機具設備序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '所屬公司序號',
    vendor_sid              VARCHAR(32)                             NULL COMMENT '租賃廠商/供應商序號 (若為租賃機具，對應 VendorDB SID)',
    
    equipment_code          VARCHAR(50)                         NOT NULL UNIQUE COMMENT '機具編號/車牌 (例: EQ-CRANE-001, 35-TC)',
    equipment_name          VARCHAR(200)                        NOT NULL COMMENT '機具名稱 (例: 100噸移動式吊車, 履帶式挖掘機)',
    category                VARCHAR(50)                         NOT NULL COMMENT '類別: CRANE吊車塔吊, EXCAVATOR挖土機, GENERATOR發電機, HOIST人貨升降梯, SCAFFOLD高空車',
    brand                   VARCHAR(100)                            NULL COMMENT '品牌/製造商 (例: LIEBHERR, KOMATSU)',
    model_number            VARCHAR(100)                            NULL COMMENT '型號/規格 (例: PC200-8, LTM 1100)',
    serial_number           VARCHAR(100)                            NULL COMMENT '出廠機號/引擎號碼',
    
    ownership_type          VARCHAR(20)                         NOT NULL DEFAULT 'OWNED' COMMENT '產權類型: OWNED公司自有, LEASED外租機具, SUBCONTRACTOR_PROVIDED包商自備',
    daily_rental_rate       DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '標準日租金成本/計費率 (未稅)',
    monthly_rental_rate     DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '標準月租金成本/計費率 (未稅)',
    
    current_status          VARCHAR(20)                         NOT NULL DEFAULT 'IDLE' COMMENT '狀態: IDLE閒置待命, ON_SITE工地使用中, IN_TRANSIT調撥運送中, UNDER_MAINTENANCE維修保養中, RETIRED報廢',
    current_project_sid     VARCHAR(32)                             NULL COMMENT '當前駐紮/施工之 ProjectDB 建案序號 (若在機具庫房則為 NULL)',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '設備詳細技術規格與備註',
    INDEX idx_eem_company_sid (company_sid),
    INDEX idx_eem_category (category),
    INDEX idx_eem_ownership (ownership_type),
    INDEX idx_eem_status (current_status),
    INDEX idx_eem_current_project (current_project_sid),
    INDEX idx_eem_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='施工機具資產主檔';

-- =========================================================
-- 02. 機具派遣與進退場紀錄 (Equipment Dispatch Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS eqp_dispatch_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '派遣單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '開單時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    equipment_sid           VARCHAR(32)                         NOT NULL COMMENT '對應 eqp_equipment_master.sid',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '派遣目標 ProjectDB 建案序號',
    dispatch_no             VARCHAR(50)                         NOT NULL COMMENT '派遣單號 (例: DSP-202608-015)',
    
    scheduled_mobilization_date DATE                                NOT NULL COMMENT '預計進場日期',
    actual_mobilization_date    DATETIME                                NULL COMMENT '實際進場簽收時間',
    scheduled_demobilization_date DATE                              NULL COMMENT '預計退場日期',
    actual_demobilization_date  DATETIME                                NULL COMMENT '實際退場簽收時間',
    
    operator_name           VARCHAR(100)                            NULL COMMENT '機具操作手/駕駛員姓名',
    operator_phone          VARCHAR(50)                             NULL COMMENT '操作手聯絡電話',
    
    dispatch_status         VARCHAR(20)                         NOT NULL DEFAULT 'SCHEDULED' COMMENT 'SCHEDULED已排程;ON_SITE使用中;COMPLETED已退場結案;CANCELLED取消',
    requested_by_user_sid   VARCHAR(32)                         NOT NULL COMMENT '申請工程師帳號序號',
    approved_by_user_sid    VARCHAR(32)                             NULL COMMENT '核准主管帳號序號',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '進退場注意事項',
    CONSTRAINT uk_edl_dispatch_no UNIQUE (dispatch_no),
    INDEX idx_edl_equipment_sid (equipment_sid),
    INDEX idx_edl_project_sid (project_sid),
    INDEX idx_edl_status (dispatch_status),
    INDEX idx_edl_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='機具派遣與進退場紀錄檔';

-- =========================================================
-- 03. 機具保養與維修履歷 (Equipment Maintenance & Repair Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS eqp_maintenance_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '保養維修序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '開單時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    equipment_sid           VARCHAR(32)                         NOT NULL COMMENT '對應 eqp_equipment_master.sid',
    project_sid             VARCHAR(32)                             NULL COMMENT '發生事故/保養時所在建案序號 (若在庫房則為 NULL)',
    maintenance_no          VARCHAR(50)                         NOT NULL COMMENT '單號 (例: MNT-202608-003)',
    
    maintenance_type        VARCHAR(30)                         NOT NULL DEFAULT 'PREVENTIVE' COMMENT '類別: PREVENTIVE定期保養, BREAKDOWN_REPAIR故障報修, OVERHAUL大修/組件更換',
    failure_description     TEXT                                    NULL COMMENT '故障現象或保養項目說明',
    repair_vendor_sid       VARCHAR(32)                             NULL COMMENT '委外維修廠商序號 (VendorDB SID)',
    
    start_time              DATETIME                            NOT NULL COMMENT '維修保養開始時間',
    completion_time         DATETIME                                NULL COMMENT '完工驗收時間',
    
    parts_cost              DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '零件耗材費用 (未稅)',
    labor_cost              DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '維修工資費用 (未稅)',
    total_cost              DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '維修總費用 (未稅，歸攤至建案成本)',
    
    maintenance_status      VARCHAR(20)                         NOT NULL DEFAULT 'IN_PROGRESS' COMMENT 'IN_PROGRESS處理中;WAITING_PARTS待料中;COMPLETED維修完成;CANCELLED作廢',
    serviced_by_user_sid    VARCHAR(32)                         NOT NULL COMMENT '填報/負責工程師帳號序號',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '更換零件清單與詳細備註',
    CONSTRAINT uk_eml_maintenance_no UNIQUE (maintenance_no),
    INDEX idx_eml_equipment_sid (equipment_sid),
    INDEX idx_eml_project_sid (project_sid),
    INDEX idx_eml_type (maintenance_type),
    INDEX idx_eml_status (maintenance_status),
    INDEX idx_eml_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='機具保養與維修履歷檔';

-- =========================================================
-- 04. 法定安全檢驗與特種證照 (Safety Inspection & Certifications)
-- =========================================================

CREATE TABLE IF NOT EXISTS eqp_safety_certification (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '證照/安檢序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    equipment_sid           VARCHAR(32)                         NOT NULL COMMENT '對應 eqp_equipment_master.sid',
    
    cert_type               VARCHAR(50)                         NOT NULL COMMENT '證照/安檢類型 (例: TOWER_CRANE_GOV_LICENSE塔吊合格證, BOILER_INSPECTION, HIGH_PRESSURE_VESSEL)',
    gov_license_number      VARCHAR(100)                        NOT NULL COMMENT '政府或第三方檢驗機構證號 (例: 勞檢字第1150892號)',
    inspection_agency       VARCHAR(100)                        NOT NULL COMMENT '檢驗機構名稱 (例: 中華民國工業安全衛生協會)',
    
    issue_date              DATE                                NOT NULL COMMENT '發照/檢驗合格日期',
    expiration_date         DATE                                NOT NULL COMMENT '有效期限至 (到期前警告)',
    
    cert_status             VARCHAR(20)                         NOT NULL DEFAULT 'VALID' COMMENT '狀態: VALID有效, EXPIRING_SOON即將到期, EXPIRED已過期(禁止派遣), REVOKED撤銷',
    attachment_document_sid VARCHAR(32)                             NULL COMMENT '證照電子掃描檔序號 (DMS)',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_esc_equipment_sid (equipment_sid),
    INDEX idx_esc_expiration (expiration_date),
    INDEX idx_esc_status (cert_status),
    INDEX idx_esc_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='法定安全檢驗與特種設備證照檔';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}