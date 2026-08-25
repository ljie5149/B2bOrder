namespace B2bOrder.Resources.Construction
{
    /// <summary>
    /// ChangeOrderDB V1 Shared Core Schema (建築商 / 營造 ERP 專用)
    /// 設計目標：
    /// 1. 工程變更追加減主檔 (Change Order Master)：管理變更原因 (RFI/設計變更/客戶客變)、追加減金額與展延工期 (EOT)
    /// 2. 變更工項與單價明細 (Change Order Line Items)：紀錄新增工項或原工項數量增減、原單價/議定新單價與小計
    /// 3. 工期展延與評估檔 (Time Extension Analysis)：紀錄關鍵路徑影響、申請展延天數與核准天數
    /// 4. 變更簽核軌跡與連動狀態 (Change Order Workflow Log)：記錄現場監造、成本部門、專案經理與高層簽核歷程
    /// 5. 整合串接 ProjectDB (WBS)、ContractDB (合約補充協議)、BudgetCostDB (預算調整) 與 ProgressPaymentDB (估驗)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class ChangeOrderDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 工程變更追加減主檔 (Change Order Master)
-- =========================================================

CREATE TABLE IF NOT EXISTS cho_change_order_master (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '變更單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '申請日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司/建設公司序號',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    contract_sid            VARCHAR(32)                         NOT NULL COMMENT 'ContractDB 合約序號',
    vendor_sid              VARCHAR(32)                         NOT NULL COMMENT '承包廠商序號 (VendorDB SID)',
    change_order_no         VARCHAR(50)                         NOT NULL COMMENT '變更單號 (例: VO-202608-003)',
    change_title            VARCHAR(200)                        NOT NULL COMMENT '變更主題 (例: B2F機房位置調整及管道間追加工程)',
    change_reason_type      VARCHAR(30)                         NOT NULL DEFAULT 'DESIGN_CHANGE' COMMENT '變更原因類別: DESIGN_CHANGE設計變更, CLIENT_REQUEST業主/客變, SITE_CONDITION現場地質/突發狀況, RFI_CLARIFICATION圖說疑義澄清, LAWS_REGULATION法規更新',
    
    -- 金額計算
    original_contract_amount DECIMAL(16,2)                      NOT NULL DEFAULT 0.00 COMMENT '變更前合約金額 (未稅)',
    change_amount_before_tax DECIMAL(16,2)                      NOT NULL DEFAULT 0.00 COMMENT '本單追加(正數)或追減(負數)金額 (未稅)',
    tax_rate                DECIMAL(5,2)                        NOT NULL DEFAULT 5.00 COMMENT '營業稅率 (%)',
    change_tax_amount       DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本單追加減稅額',
    change_total_amount     DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本單追加減含稅總額',
    revised_contract_amount DECIMAL(16,2)                      NOT NULL DEFAULT 0.00 COMMENT '變更後新合約總金額 (含稅)',
    
    -- 工期展延 (EOT - Extension of Time)
    is_time_extension_needed VARCHAR(2)                         NOT NULL DEFAULT 'N' COMMENT '是否申請工期展延 (Y/N)',
    requested_extension_days INT                                NOT NULL DEFAULT 0 COMMENT '廠商申請展延天數',
    approved_extension_days  INT                                NOT NULL DEFAULT 0 COMMENT '核准展延天數',
    
    -- 狀態與簽核
    order_status            VARCHAR(20)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;SUBMITTED送審中;SITE_CHECKED監造覆核;COST_REVIEWED成本審核;APPROVED已核准簽署;REJECTED退回;CANCELLED作廢',
    applicant_user_sid      VARCHAR(32)                         NOT NULL COMMENT '申請人員/工程師序號',
    approved_by_user_sid    VARCHAR(32)                             NULL COMMENT '最終核准主管序號',
    approved_at             DATETIME                                NULL COMMENT '核准時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '詳細變更原因說明與背景',
    CONSTRAINT uk_ccom_contract_number UNIQUE (contract_sid, change_order_no),
    INDEX idx_ccom_company_sid (company_sid),
    INDEX idx_ccom_project_sid (project_sid),
    INDEX idx_ccom_vendor_sid (vendor_sid),
    INDEX idx_ccom_status (order_status),
    INDEX idx_ccom_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程變更追加減主檔';

-- =========================================================
-- 02. 變更工項與單價明細 (Change Order Line Items)
-- =========================================================

CREATE TABLE IF NOT EXISTS cho_change_order_line (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '變更明細序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    change_order_sid        VARCHAR(32)                         NOT NULL COMMENT '對應 cho_change_order_master.sid',
    wbs_sid                 VARCHAR(32)                             NULL COMMENT '關聯 ProjectDB WBS 任務序號',
    item_type               VARCHAR(20)                         NOT NULL DEFAULT 'REVISED_ITEM' COMMENT '工項類別: EXISTING_ITEM既有工項數量增減, NEW_ITEM新增議價工項, DELETED_ITEM取消工項',
    item_code               VARCHAR(50)                         NOT NULL COMMENT '工項編號 (例: 03300-05)',
    item_name               VARCHAR(200)                        NOT NULL COMMENT '工項名稱 (例: B2F 牆面防水層追加)',
    unit                    VARCHAR(20)                             NULL COMMENT '計量單位 (例: m2, m3, 式, 噸)',
    
    -- 數量變動
    original_quantity       DECIMAL(12,4)                       NOT NULL DEFAULT 0.0000 COMMENT '原合約數量 (若為新工項則為 0)',
    change_quantity         DECIMAL(12,4)                       NOT NULL DEFAULT 0.0000 COMMENT '追加(正數)或追減(負數)數量',
    revised_quantity        DECIMAL(12,4)                       NOT NULL DEFAULT 0.0000 COMMENT '變更後新數量',
    
    -- 單價與金額
    unit_price              DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '核定單價 (未稅)',
    is_new_unit_price       VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT '是否為新增議定單價 (Y/N)',
    subtotal_amount         DECIMAL(16,2)                       NOT NULL DEFAULT 0.00 COMMENT '本項變更小計 (單價 * 變動數量)',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '變更分析與單價分析說明',
    INDEX idx_ccol_change_order_sid (change_order_sid),
    INDEX idx_ccol_wbs_sid (wbs_sid),
    INDEX idx_ccol_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程變更工項與單價明細檔';

-- =========================================================
-- 03. 工期展延評估檔 (Time Extension Analysis - EOT)
-- =========================================================

CREATE TABLE IF NOT EXISTS cho_time_extension_analysis (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '展延評估序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    change_order_sid        VARCHAR(32)                         NOT NULL COMMENT '對應 cho_change_order_master.sid',
    impacted_wbs_sid        VARCHAR(32)                             NULL COMMENT '受影響之關鍵路徑 WBS 序號',
    impact_description      TEXT                                NOT NULL COMMENT '對關鍵路徑 (Critical Path) 之影響說明',
    delay_start_date        DATE                                    NULL COMMENT '延誤起始日期',
    delay_end_date          DATE                                    NULL COMMENT '延誤結束日期',
    applied_days            INT                                 NOT NULL DEFAULT 0 COMMENT '廠商申請展延天數',
    approved_days           INT                                 NOT NULL DEFAULT 0 COMMENT '監造與業主核准展延天數',
    revised_project_end_date DATE                                   NULL COMMENT '展延後新預計完工日期',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註/核准理由說明',
    INDEX idx_ctea_change_order_sid (change_order_sid),
    INDEX idx_ctea_wbs_sid (impacted_wbs_sid),
    INDEX idx_ctea_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程變更工期展延評估檔';

-- =========================================================
-- 04. 變更單簽核與歷程紀錄 (Change Order Approval Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS cho_approval_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '簽核紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '簽核時間',
    change_order_sid        VARCHAR(32)                         NOT NULL COMMENT '對應 cho_change_order_master.sid',
    reviewer_user_sid       VARCHAR(32)                         NOT NULL COMMENT '審核人員帳號序號',
    reviewer_role           VARCHAR(50)                         NOT NULL COMMENT '審核角色 (例: SITE_ENGINEER, PROJECT_MANAGER, COST_ENGINEER, VP)',
    review_action           VARCHAR(20)                         NOT NULL COMMENT '動作: SUBMIT送審, APPROVE同意, REJECT退回, TRANSFER轉辦',
    review_comment          TEXT                                    NULL COMMENT '審核意見與簽核批示',
    INDEX idx_cal_change_order_sid (change_order_sid),
    INDEX idx_cal_reviewer (reviewer_user_sid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程變更單簽核歷程紀錄檔 (Append-Only)';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}