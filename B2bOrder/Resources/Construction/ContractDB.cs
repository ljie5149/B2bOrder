namespace B2bOrder.Resources.Construction
{
    /// <summary>
    /// ContractDB V1 Shared Core Schema (建築商 / 營造 ERP 專用)
    /// 設計目標：
    /// 1. 採購發包與工程合約主檔 (Contract Master)：管理總合約、下包合約、採購合約金額、付款機制與簽署狀態
    /// 2. 合約付款與估驗條件 (Payment Terms & Milestones)：訂定估驗計價週期、保留金扣留比例 (% )、預付款抵扣機制
    /// 3. 合約變更與補充協議 (Contract Addendum / Change Order Agreement)：管理具 legal binding 之補充條款與金額/工期變更
    /// 4. 保證金與保證書管理 (Bonds & Guarantee Management)：追蹤履約保證金、預付款保證金、保固保證金與退還時程
    /// 5. 保固期與售後維修承諾 (Warranty Terms & Warranty Certificate)：紀錄各工項保固年限 (如結構保固 15 年, 防水保固 5 年)
    /// 6. 整合串接 ProjectDB (建案與WBS)、BudgetCostDB (預算承諾) 與 VendorDB (廠商)
    /// 7. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 8. avalible：Y可用；W停用；D刪除
    /// 9. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class ContractDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 工程與採購合約主檔 (Contract Master)
-- =========================================================

CREATE TABLE IF NOT EXISTS ctr_contract_master (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '合約序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司/建設公司序號',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    vendor_sid              VARCHAR(32)                         NOT NULL COMMENT '乙方/承包廠商序號 (VendorDB SID)',
    contract_number         VARCHAR(100)                        NOT NULL COMMENT '合約編號 (例: CTR-2026-ST01)',
    contract_title          VARCHAR(250)                        NOT NULL COMMENT '合約名稱 (例: 晴空豪景主體結構工程總承包合約)',
    contract_type           VARCHAR(30)                         NOT NULL DEFAULT 'SUBCONTRACT' COMMENT '合約類別: SUBCONTRACT工程外包, MATERIAL採購, SERVICE勞務/顧問, DESIGN設計/建築師',
    tax_type                VARCHAR(20)                         NOT NULL DEFAULT 'TAXABLE' COMMENT '計稅方式: TAXABLE應稅(5%), ZERO_TAX零稅率, TAX_EXEMPT免稅',
    original_contract_amount DECIMAL(16,2)                      NOT NULL DEFAULT 0.00 COMMENT '原始簽約未稅金額',
    original_tax_amount     DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '原始簽約稅額',
    original_total_amount   DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '原始簽約含稅總額',
    current_contract_amount DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '變更後最新合約含稅金額',
    sign_date               DATE                                    NULL COMMENT '簽約日期',
    effective_start_date    DATE                                    NULL COMMENT '合約生效/開工日期',
    effective_end_date      DATE                                    NULL COMMENT '預計完工/履約結束日期',
    retention_rate          DECIMAL(5,2)                        NOT NULL DEFAULT 5.00 COMMENT '工程保留金扣留比例 (例: 5.00 代表 5%)',
    advance_payment_amount  DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '預付款金額 (如有)',
    liquidated_damages_rate DECIMAL(6,4)                        NOT NULL DEFAULT 0.0010 COMMENT '逾期違約金比例 (例: 0.0010 代表每日千分之一)',
    contract_status         VARCHAR(20)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;REVIEWING審核中;SIGNED已簽約;EXECUTING履約中;COMPLETED已竣工結案;TERMINATED已終止',
    legal_reviewed_by_sid   VARCHAR(32)                             NULL COMMENT '法務審核人員序號',
    legal_reviewed_at       DATETIME                                NULL COMMENT '法務審核時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註/特殊條款說明',
    CONSTRAINT uk_ccm_company_number UNIQUE (company_sid, contract_number),
    INDEX idx_ccm_project_sid (project_sid),
    INDEX idx_ccm_vendor_sid (vendor_sid),
    INDEX idx_ccm_status (contract_status),
    INDEX idx_ccm_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程與採購合約主檔';

-- =========================================================
-- 02. 合約估驗與付款條件 (Contract Payment Terms & Milestones)
-- =========================================================

CREATE TABLE IF NOT EXISTS ctr_payment_term (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '付款條件序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    contract_sid            VARCHAR(32)                         NOT NULL COMMENT '對應 ctr_contract_master.sid',
    payment_stage_code      VARCHAR(50)                         NOT NULL COMMENT '階段代碼 (例: ADVANCE預付款, MONTHLY_VALUATION每月估驗, MILESTONE階段里程碑, FINAL_SETTLEMENT結案款)',
    stage_name              VARCHAR(150)                        NOT NULL COMMENT '付款階段說明 (例: 10F結構體完成估驗, 竣工驗收合格款)',
    percentage_rate         DECIMAL(5,2)                        NOT NULL DEFAULT 0.00 COMMENT '付款佔總合約比例 (%)',
    milestone_wbs_sid       VARCHAR(32)                             NULL COMMENT '關聯 WBS 工項序號',
    cash_ratio              DECIMAL(5,2)                        NOT NULL DEFAULT 100.00 COMMENT '現金支付比例 (%)',
    ticket_ratio            DECIMAL(5,2)                        NOT NULL DEFAULT 0.00 COMMENT '商業票據/支票支付比例 (%)',
    ticket_days             INT                                 NOT NULL DEFAULT 0 COMMENT '票據兌現天數 (例: 60天期票)',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_cpt_contract_sid (contract_sid),
    INDEX idx_cpt_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='合約付款條件與里程碑設定檔';

-- =========================================================
-- 03. 合約補充協議與變更單 (Contract Addendum / Amendment)
-- =========================================================

CREATE TABLE IF NOT EXISTS ctr_contract_amendment (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '補充協議序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '簽署/建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    contract_sid            VARCHAR(32)                         NOT NULL COMMENT '對應 ctr_contract_master.sid',
    variation_order_sid     VARCHAR(32)                             NULL COMMENT '關聯 ProjectDB prj_variation_order.sid (追加減單)',
    amendment_number        VARCHAR(50)                         NOT NULL COMMENT '補充協議單號 (例: AMD-CTR-2026-01)',
    amendment_title         VARCHAR(200)                        NOT NULL COMMENT '變更/補充協議主題',
    change_type             VARCHAR(30)                         NOT NULL DEFAULT 'AMOUNT_AND_SCHEDULE' COMMENT '變更類型: AMOUNT金額變更, SCHEDULE工期展延, SCOPE工項範疇, OTHER其他條款',
    change_amount           DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '追加減含稅金額 (正數追加, 負數追減)',
    revised_total_amount    DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '協議簽署後新總合約金額',
    extended_days           INT                                 NOT NULL DEFAULT 0 COMMENT '工期展延天數',
    revised_end_date        DATE                                    NULL COMMENT '調整後完工日期',
    amendment_status        VARCHAR(20)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;EXECUTED已簽署生效;CANCELLED作廢',
    sign_date               DATE                                    NULL COMMENT '雙方協議簽署日期',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '補充條款文字內容說明',
    CONSTRAINT uk_cca_contract_number UNIQUE (contract_sid, amendment_number),
    INDEX idx_cca_contract_sid (contract_sid),
    INDEX idx_cca_vo_sid (variation_order_sid),
    INDEX idx_cca_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='合約補充協議與變更條款檔';

-- =========================================================
-- 04. 履約與保固保證金管理 (Bonds & Guarantees)
-- =========================================================

CREATE TABLE IF NOT EXISTS ctr_bond_guarantee (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '保證金序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    contract_sid            VARCHAR(32)                         NOT NULL COMMENT '合約序號',
    vendor_sid              VARCHAR(32)                         NOT NULL COMMENT '廠商序號',
    bond_type               VARCHAR(30)                         NOT NULL DEFAULT 'PERFORMANCE' COMMENT '保證類別: PERFORMANCE履約保證, ADVANCE_PAYMENT預付款保證, WARRANTY保固保證',
    bond_mode               VARCHAR(20)                         NOT NULL DEFAULT 'BANK_GUARANTEE' COMMENT '繳納方式: CASH現金, BANK_GUARANTEE銀行履約保證函, CHECK商業票據, CERTIFICATE本票',
    guarantee_bank_name     VARCHAR(100)                            NULL COMMENT '開狀/保證銀行名稱',
    bond_number             VARCHAR(100)                            NULL COMMENT '保證函/本票/單據號碼',
    bond_amount             DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '保證金額',
    effective_start_date    DATE                                NOT NULL COMMENT '保證起始日',
    effective_end_date      DATE                                NOT NULL COMMENT '保證到期日',
    bond_status             VARCHAR(20)                         NOT NULL DEFAULT 'HELD' COMMENT 'HELD保管中;RELEASED已退還/解保;CONFISCATED已沒收;EXPIRED已逾期',
    release_date            DATE                                    NULL COMMENT '實際退還/解保日期',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_cbg_contract_sid (contract_sid),
    INDEX idx_cbg_vendor_sid (vendor_sid),
    INDEX idx_cbg_type_status (bond_type, bond_status),
    INDEX idx_cbg_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='合約保證金與保證函管理檔';

-- =========================================================
-- 05. 工程保固條款與保固期 (Warranty Terms)
-- =========================================================

CREATE TABLE IF NOT EXISTS ctr_warranty_term (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '保固條款序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    contract_sid            VARCHAR(32)                         NOT NULL COMMENT '合約序號',
    item_scope_name         VARCHAR(150)                        NOT NULL COMMENT '保固項目範疇 (例: 主體結構, 防水工程, 門窗設備)',
    warranty_months         INT                                 NOT NULL DEFAULT 12 COMMENT '保固期限 (月數，例: 60 代表 5 年)',
    warranty_start_date     DATE                                    NULL COMMENT '保固起始日 (通常為總驗收/交屋日)',
    warranty_end_date       DATE                                    NULL COMMENT '保固到期日',
    warranty_status         VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING未到期生效;IN_WARRANTY保固中;EXPIRED已過保固',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註/保固維修涵蓋範圍說明',
    INDEX idx_cwt_contract_sid (contract_sid),
    INDEX idx_cwt_status (warranty_status),
    INDEX idx_cwt_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程保固條款檔';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}