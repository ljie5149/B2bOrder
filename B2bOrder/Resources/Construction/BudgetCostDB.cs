namespace B2bOrder.Resources.Construction
{
    /// <summary>
    /// BudgetCostDB V1 Shared Core Schema (建築商 / 營造 ERP 專用)
    /// 設計目標：
    /// 1. 工程目標預算主檔 (Project Target Budget & Budget Line)：管理建案發包總預算、工項拆解預算 (Target Cost)
    /// 2. 發包合約與承諾成本 (Contract Commitments)：紀錄廠商合約金額、變更追加減 (VO)，掌控已承諾未支出成本
    /// 3. 實際工程成本與費用歸攤 (Actual Cost Allocation)：紀錄估驗計價、採購發票、規費等實際成本支出 (Actual Expense)
    /// 4. 預算挪移與流轉審核 (Budget Reallocation)：提供跨工項預算調整/挪用申請與稽核紀錄
    /// 5. 成本盈虧與預警分析 (Cost Variance & EVM)：紀錄 EAC (預估完工總成本)、CV (成本偏差) 與超支告警
    /// 6. 整合串接 ProjectDB (WBS工項)、VendorDB (廠商) 與 FinancialDB/SalesOrderDB
    /// 7. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 8. avalible：Y可用；W停用；D刪除
    /// 9. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class BudgetCostDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 建案目標預算主檔與工項預算明細 (Target Budget & Budget Lines)
-- =========================================================

CREATE TABLE IF NOT EXISTS bgt_target_budget (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '目標預算序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司/建設公司序號',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    budget_version_code     VARCHAR(50)                         NOT NULL DEFAULT 'V1.0' COMMENT '預算版本代碼 (例: V1.0-初估, V2.0-發包預算)',
    budget_title            VARCHAR(200)                        NOT NULL COMMENT '預算案名稱',
    total_target_budget     DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '目標總預算金額',
    approved_vo_budget      DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '累計核准變更追加減預算',
    revised_total_budget    DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '調整後總預算 (目標 + 追加減)',
    committed_amount        DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '累計已發包/已承諾金額',
    actual_cost_amount      DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '累計實際支出成本',
    budget_status           VARCHAR(20)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;SUBMITTED送審中;APPROVED已核准;CLOSED已結案',
    approved_by_user_sid    VARCHAR(32)                             NULL COMMENT '核准主管序號',
    approved_at             DATETIME                                NULL COMMENT '核准時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_btb_project_version UNIQUE (project_sid, budget_version_code),
    INDEX idx_btb_company_sid (company_sid),
    INDEX idx_btb_project_sid (project_sid),
    INDEX idx_btb_status (budget_status),
    INDEX idx_btb_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='建案目標預算主檔';

CREATE TABLE IF NOT EXISTS bgt_budget_item_line (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '預算工項明細序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    budget_sid              VARCHAR(32)                         NOT NULL COMMENT '對應 bgt_target_budget.sid',
    wbs_sid                 VARCHAR(32)                             NULL COMMENT 'ProjectDB WBS 任務序號 (若有對應)',
    cost_category_code      VARCHAR(50)                         NOT NULL COMMENT '成本分類代碼 (例: SUB_CONTRACT工程外包, MATERIAL材料, EQUIPMENT機具, PERMIT規費, MANAGEMENT管理費)',
    item_code               VARCHAR(50)                         NOT NULL COMMENT '預算科目/編碼 (例: 03300-混凝土工程)',
    item_name               VARCHAR(200)                        NOT NULL COMMENT '預算科目名稱',
    unit                    VARCHAR(20)                             NULL COMMENT '計量單位 (例: m3, 噸, 式)',
    unit_price              DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '預算單價',
    quantity                DECIMAL(12,4)                       NOT NULL DEFAULT 0.0000 COMMENT '預算數量',
    original_budget_amount  DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '原始工項預算 (單價*數量)',
    reallocated_amount      DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '調撥增減金額 (正數增加, 負數減少)',
    revised_budget_amount   DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '調整後工項預算',
    committed_amount        DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '該工項已發包承諾金額',
    actual_cost_amount      DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '該工項已發生實際成本',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_bil_budget_item UNIQUE (budget_sid, item_code),
    INDEX idx_bil_budget_sid (budget_sid),
    INDEX idx_bil_wbs_sid (wbs_sid),
    INDEX idx_bil_cost_category (cost_category_code),
    INDEX idx_bil_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程目標預算工項明細檔';

-- =========================================================
-- 02. 工程發包承諾成本 (Contract Commitments)
-- =========================================================

CREATE TABLE IF NOT EXISTS bgt_contract_commitment (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '承諾成本序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '簽約/建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案序號',
    budget_item_sid         VARCHAR(32)                         NOT NULL COMMENT '對應 bgt_budget_item_line.sid',
    vendor_sid              VARCHAR(32)                         NOT NULL COMMENT '承包廠商/供應商序號 (VendorDB SID)',
    contract_number         VARCHAR(100)                        NOT NULL COMMENT '工程合約/採購單編號 (例: SUB-2026-008)',
    contract_title          VARCHAR(200)                        NOT NULL COMMENT '合約/發包名稱 (例: 晴空豪景土方開挖與安全支撐工程)',
    original_contract_amount DECIMAL(16,2)                      NOT NULL DEFAULT 0.00 COMMENT '原始簽約未稅金額',
    vo_accumulated_amount   DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '累計變更追加減金額',
    current_contract_amount DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '當前合約總額 (原始 + 追加減)',
    actual_invoiced_amount  DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '累計已估驗/已開立發票金額',
    commitment_status       VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE合約執行中;COMPLETED已完工結案;TERMINATED終止合約',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_bcc_project_contract UNIQUE (project_sid, contract_number),
    INDEX idx_bcc_project_sid (project_sid),
    INDEX idx_bcc_budget_item (budget_item_sid),
    INDEX idx_bcc_vendor (vendor_sid),
    INDEX idx_bcc_status (commitment_status),
    INDEX idx_bcc_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程發包與採購承諾成本檔';

-- =========================================================
-- 03. 實際成本開支歸攤 (Actual Cost Expense Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS bgt_actual_cost_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '實際成本紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '入帳/發生日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案序號',
    budget_item_sid         VARCHAR(32)                         NOT NULL COMMENT '對應 bgt_budget_item_line.sid',
    commitment_sid          VARCHAR(32)                             NULL COMMENT '對應 bgt_contract_commitment.sid (若為合約內支出)',
    source_document_type    VARCHAR(50)                         NOT NULL COMMENT '來源單據類別: PROGRESS_PAYMENT估驗計價, PURCHASE_INVOICE進貨發票, EXPENSE_PETTY_CASH零用金, PERMIT_FEE規費',
    source_document_sid     VARCHAR(32)                         NOT NULL COMMENT '來源單據序號 (例: ProjectDB progress_payment.sid)',
    source_document_no      VARCHAR(100)                        NOT NULL COMMENT '來源單據編號',
    vendor_sid              VARCHAR(32)                             NULL COMMENT '請款廠商序號',
    cost_amount             DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '實際成本未稅金額',
    tax_amount              DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '營業稅額',
    total_cost_amount       DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '實際成本含稅金額',
    posting_date            DATE                                NOT NULL COMMENT '會計切帳日期',
    remark                  TEXT                                    NULL COMMENT '成本摘要說明',
    INDEX idx_bacl_project_sid (project_sid),
    INDEX idx_bacl_budget_item (budget_item_sid),
    INDEX idx_bacl_commitment (commitment_sid),
    INDEX idx_bacl_source_doc (source_document_type, source_document_sid),
    INDEX idx_bacl_posting_date (posting_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='實際工程成本開支與歸攤日誌檔 (Append-Only)';

-- =========================================================
-- 04. 預算挪移與流轉紀錄 (Budget Reallocation & Transfer)
-- =========================================================

CREATE TABLE IF NOT EXISTS bgt_budget_reallocation (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '預算流轉序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '申請日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案序號',
    transfer_number         VARCHAR(50)                         NOT NULL COMMENT '流轉單號 (例: BTR-202604-002)',
    from_budget_item_sid    VARCHAR(32)                         NOT NULL COMMENT '轉出預算工項序號',
    to_budget_item_sid      VARCHAR(32)                         NOT NULL COMMENT '轉入預算工項序號',
    transfer_amount         DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '挪移/調撥金額',
    reason_description      TEXT                                NOT NULL COMMENT '預算調撥原因說明',
    transfer_status         VARCHAR(20)                         NOT NULL DEFAULT 'SUBMITTED' COMMENT 'SUBMITTED送審中;APPROVED已核准;REJECTED退回;CANCELLED撤銷',
    applicant_user_sid      VARCHAR(32)                         NOT NULL COMMENT '申請人員序號',
    approved_by_user_sid    VARCHAR(32)                             NULL COMMENT '審核主管序號',
    approved_at             DATETIME                                NULL COMMENT '審核時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_bbr_project_number UNIQUE (project_sid, transfer_number),
    INDEX idx_bbr_project_sid (project_sid),
    INDEX idx_bbr_from_item (from_budget_item_sid),
    INDEX idx_bbr_to_item (to_budget_item_sid),
    INDEX idx_bbr_status (transfer_status),
    INDEX idx_bbr_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工項預算挪移與流轉審核檔';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}