namespace B2bOrder.Resources.Construction
{
    /// <summary>
    /// ProgressPaymentDB V1 Shared Core Schema (建築商 / 營造 ERP 專用)
    /// 設計目標：
    /// 1. 工程估驗請款主檔 (Progress Payment Valuation Master)：管理廠商各期估驗、應付金額、保留金扣留與審核狀態
    /// 2. 估驗工項數量與金額明細 (Valuation Line Items)：紀錄各 WBS / 工項本期申報與實核數量、累計進度 %
    /// 3. 工程代扣與扣罰款明細 (Backcharge & Deductions)：處理現場代購材料、缺失扣款、逾期罰款等點收扣款
    /// 4. 工程保留金退還與釋放 (Retention Release Request)：管理竣工驗收合格後保留金的退還申請與核撥
    /// 5. 整合串接 ProjectDB (WBS進度)、ContractDB (合約與扣款條件)、BudgetCostDB (實際成本歸攤) 與 FinancialDB (付款發票)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class ProgressPaymentDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 工程估驗請款主檔 (Progress Payment Valuation Master)
-- =========================================================

CREATE TABLE IF NOT EXISTS pgp_payment_valuation (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '估驗單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '申請日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司/建設公司序號',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    contract_sid            VARCHAR(32)                         NOT NULL COMMENT 'ContractDB 合約序號',
    vendor_sid              VARCHAR(32)                         NOT NULL COMMENT '承包廠商序號 (VendorDB SID)',
    valuation_no            VARCHAR(50)                         NOT NULL COMMENT '估驗單號 (例: VAL-202608-005)',
    period_number           INT                                 NOT NULL DEFAULT 1 COMMENT '估驗期數 (例: 第 5 期)',
    cutoff_date             DATE                                NOT NULL COMMENT '本期估驗截止基準日',
    
    -- 金額計算欄位 (未稅)
    accumulated_previous_amount DECIMAL(16,2)                   NOT NULL DEFAULT 0.00 COMMENT '前期累計已核准估驗金額',
    current_claimed_amount      DECIMAL(16,2)                   NOT NULL DEFAULT 0.00 COMMENT '本期廠商申報估驗金額',
    current_approved_amount     DECIMAL(16,2)                   NOT NULL DEFAULT 0.00 COMMENT '本期監造實核估驗金額',
    accumulated_approved_amount DECIMAL(16,2)                   NOT NULL DEFAULT 0.00 COMMENT '累計至本期實核估驗總額',
    
    -- 扣款與調整金額
    retention_rate              DECIMAL(5,2)                        NOT NULL DEFAULT 5.00 COMMENT '本期保留金扣留比例 (%)',
    current_retention_deduction DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本期扣留保留金金額',
    current_advance_deduction   DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本期扣抵預付款金額',
    current_backcharge_deduction DECIMAL(16,2)                      NOT NULL DEFAULT 0.00 COMMENT '本期點收代扣/罰款金額',
    price_escalation_amount     DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本期物價指數調整款 (可為正負數)',
    
    -- 應付與稅額計算
    net_approved_before_tax     DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本期未稅應付淨額 (實核 - 保留金 - 扣預付 - 代扣 + 物調)',
    tax_rate                    DECIMAL(5,2)                        NOT NULL DEFAULT 5.00 COMMENT '營業稅率 (%)',
    tax_amount                  DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本期營業稅額',
    total_payable_amount        DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本期含稅應付總金額',
    
    -- 發票與付款狀態
    invoice_number              VARCHAR(100)                            NULL COMMENT '廠商開立統一發票號碼',
    invoice_date                DATE                                    NULL COMMENT '發票開立日期',
    valuation_status            VARCHAR(20)                         NOT NULL DEFAULT 'SUBMITTED' COMMENT 'SUBMITTED送審中;SITE_CHECKED監造審驗完成;APPROVED總公司審核通過;PAID已撥款;REJECTED退件',
    approved_by_user_sid        VARCHAR(32)                             NULL COMMENT '核准主管序號',
    approved_at                 DATETIME                                NULL COMMENT '核准時間',
    version_no                  BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                    VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                      TEXT                                    NULL COMMENT '估驗備註說明',
    CONSTRAINT uk_ppv_contract_period UNIQUE (contract_sid, period_number),
    INDEX idx_ppv_company_sid (company_sid),
    INDEX idx_ppv_project_sid (project_sid),
    INDEX idx_ppv_vendor_sid (vendor_sid),
    INDEX idx_ppv_status (valuation_status),
    INDEX idx_ppv_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程估驗請款主檔';

-- =========================================================
-- 02. 工程估驗工項明細 (Valuation Line Items)
-- =========================================================

CREATE TABLE IF NOT EXISTS pgp_valuation_item_line (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '估驗明細序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    valuation_sid           VARCHAR(32)                         NOT NULL COMMENT '對應 pgp_payment_valuation.sid',
    contract_item_sid       VARCHAR(32)                             NULL COMMENT '關聯 ContractDB / BudgetCostDB 工項序號',
    wbs_sid                 VARCHAR(32)                             NULL COMMENT '關聯 ProjectDB WBS 任務序號',
    item_code               VARCHAR(50)                         NOT NULL COMMENT '工項編號 (例: 03300-01)',
    item_name               VARCHAR(200)                        NOT NULL COMMENT '工項名稱 (例: 5F 結構體混凝土澆置)',
    unit                    VARCHAR(20)                             NULL COMMENT '單位 (例: m3, 式)',
    unit_price              DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '合約單價',
    contract_quantity       DECIMAL(12,4)                       NOT NULL DEFAULT 0.0000 COMMENT '合約總數量',
    
    -- 數量與進度%
    previous_quantity       DECIMAL(12,4)                       NOT NULL DEFAULT 0.0000 COMMENT '前期累計完成數量',
    current_claimed_qty     DECIMAL(12,4)                       NOT NULL DEFAULT 0.0000 COMMENT '本期申報數量',
    current_approved_qty    DECIMAL(12,4)                       NOT NULL DEFAULT 0.0000 COMMENT '本期實核數量',
    accumulated_approved_qty DECIMAL(12,4)                      NOT NULL DEFAULT 0.0000 COMMENT '累計實核數量',
    completion_rate         DECIMAL(5,2)                        NOT NULL DEFAULT 0.00 COMMENT '累計完成進度百分比 (%)',
    
    -- 金額計算
    current_approved_amount DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本期實核未稅金額 (單價 * 本期實核數量)',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_pvil_valuation_sid (valuation_sid),
    INDEX idx_pvil_wbs_sid (wbs_sid),
    INDEX idx_pvil_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程估驗工項數量金額明細檔';

-- =========================================================
-- 03. 工程代扣與扣罰款明細 (Backcharge & Deductions)
-- =========================================================

CREATE TABLE IF NOT EXISTS pgp_backcharge_line (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '代扣款序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '開單日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    valuation_sid           VARCHAR(32)                         NOT NULL COMMENT '對應 pgp_payment_valuation.sid',
    deduction_type          VARCHAR(30)                         NOT NULL DEFAULT 'SAFETY_PENALTY' COMMENT '扣款類別: SAFETY_PENALTY安衛缺失罰款, MATERIAL_PURCHASE工地代購材料, CLEANING_FEE清潔維護費, DAMAGE_COMPLIANCE損壞賠償',
    deduction_title         VARCHAR(200)                        NOT NULL COMMENT '扣款主旨/原因',
    deduction_amount        DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '扣款金額 (未稅)',
    supporting_document_no  VARCHAR(100)                            NULL COMMENT '佐證單據編號 (例: 工地缺失單號 PunchList SID)',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '扣款詳細說明',
    INDEX idx_pbl_valuation_sid (valuation_sid),
    INDEX idx_pbl_type (deduction_type),
    INDEX idx_pbl_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程代扣與缺失罰款明細檔';

-- =========================================================
-- 04. 工程保留金退還與釋放 (Retention Money Release)
-- =========================================================

CREATE TABLE IF NOT EXISTS pgp_retention_release (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '保留金釋放序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '申請日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案序號',
    contract_sid            VARCHAR(32)                         NOT NULL COMMENT '合約序號',
    vendor_sid              VARCHAR(32)                         NOT NULL COMMENT '廠商序號',
    release_number          VARCHAR(50)                         NOT NULL COMMENT '退還單號 (例: RET-2026-001)',
    total_retained_amount   DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '該合約累計扣留保留金總額',
    release_stage           VARCHAR(30)                         NOT NULL DEFAULT 'FINAL_INSPECTION' COMMENT '退還階段: PRACTICAL_COMPLETION竣工驗收(退一半), FINAL_INSPECTION總驗收合格(退尾款), WARRANTY_EXPIRED保固期滿',
    requested_release_amount DECIMAL(16,2)                      NOT NULL DEFAULT 0.00 COMMENT '本期申請退還金額',
    approved_release_amount  DECIMAL(16,2)                      NOT NULL DEFAULT 0.00 COMMENT '核准退還金額',
    release_status          VARCHAR(20)                         NOT NULL DEFAULT 'SUBMITTED' COMMENT 'SUBMITTED送審中;APPROVED已核准;PAID已撥款;REJECTED退回',
    approved_by_user_sid    VARCHAR(32)                             NULL COMMENT '核准主管序號',
    approved_at             DATETIME                                NULL COMMENT '核准時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_prr_contract_release UNIQUE (contract_sid, release_number),
    INDEX idx_prr_company_sid (company_sid),
    INDEX idx_prr_project_sid (project_sid),
    INDEX idx_prr_vendor_sid (vendor_sid),
    INDEX idx_prr_status (release_status),
    INDEX idx_prr_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程保留金退還申請與釋放檔';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}