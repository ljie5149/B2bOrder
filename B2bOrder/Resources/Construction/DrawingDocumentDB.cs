namespace B2bOrder.Resources.Construction
{
    /// <summary>
    /// DrawingDocumentDB V1 Shared Core Schema (建築商 / 營造 ERP 專用)
    /// 設計目標：
    /// 1. 工程圖說主檔與版本控管 (Drawing Master & Version Control)：管理建築/結構/機電圖說之版次 (Rev)、核定狀態與生效日
    /// 2. 圖說簽核與審查歷程 (Drawing Approval Log)：記錄監造、專案經理與建築師之簽核意見 (Approve as Noted, Revise)
    /// 3. 送審文件與施工計畫書管理 (Document Submittal Control)：管理材料送審單 (Submittals)、施工計畫書與品質計畫書
    /// 4. 圖說分發與簽收紀錄 (Drawing Distribution & Transmittal)：紀錄圖說分發至外包商/工地現場之簽收紀錄，避免誤用舊圖
    /// 5. 整合串接 ProjectDB (建案)、ProjectExecutionDB (RFI)、ChangeOrderDB (變更單) 與 VendorDB
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class DrawingDocumentDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 工程圖說主檔與版次控管 (Drawing Master & Version Control)
-- =========================================================

CREATE TABLE IF NOT EXISTS dms_drawing_master (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '圖說序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    
    drawing_number          VARCHAR(100)                        NOT NULL COMMENT '圖號 (例: A-101, S-203, MEP-B1F-01)',
    drawing_title           VARCHAR(200)                        NOT NULL COMMENT '圖說名稱 (例: B1F 結構平面圖與配筋圖)',
    discipline              VARCHAR(30)                         NOT NULL COMMENT '專業類別: ARCHITECTURAL建築, STRUCTURAL結構, MEP機電, LANDSCAPE景觀, INTERIOR室內',
    drawing_type            VARCHAR(30)                         NOT NULL DEFAULT 'SHOP_DRAWING' COMMENT '圖說性質: BID_DRAWING發包圖, TENDER_DRAWING招標圖, SHOP_DRAWING施工圖, AS_BUILT竣工圖',
    
    revision_code           VARCHAR(20)                         NOT NULL DEFAULT 'Rev.0' COMMENT '圖說版次 (例: Rev.0, Rev.A, Rev.1)',
    is_current_version      VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT '是否為當前最新核定版本 (Y/N)',
    effective_date          DATE                                    NULL COMMENT '圖說生效/發布日期',
    
    file_storage_path       VARCHAR(500)                        NOT NULL COMMENT '圖說檔案儲存路徑/DWG或PDF路徑 (DMS)',
    cad_file_path           VARCHAR(500)                            NULL COMMENT '原始 CAD/BIM 模型檔案路徑',
    
    approval_status         VARCHAR(20)                         NOT NULL DEFAULT 'UNDER_REVIEW' COMMENT '狀態: UNDER_REVIEW審核中, APPROVED核准, APPROVED_AS_NOTED條件核准, REVISED_RESUBMIT退回重繪, SUPERSEDED已被新版取代',
    designed_by_architect   VARCHAR(100)                            NULL COMMENT '設計建築師/技師事務所名稱',
    approved_by_user_sid    VARCHAR(32)                             NULL COMMENT '專案簽核主管/監造序號',
    approved_at             DATETIME                                NULL COMMENT '簽核時間',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '變更摘要與說明',
    CONSTRAINT uk_ddm_project_drawing_rev UNIQUE (project_sid, drawing_number, revision_code),
    INDEX idx_ddm_company_sid (company_sid),
    INDEX idx_ddm_project_sid (project_sid),
    INDEX idx_ddm_discipline (discipline),
    INDEX idx_ddm_current_version (is_current_version),
    INDEX idx_ddm_status (approval_status),
    INDEX idx_ddm_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程圖說主檔與版次控管';

-- =========================================================
-- 02. 圖說簽核與審查歷程 (Drawing Approval Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS dms_drawing_approval_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '審查歷程序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '審查時間',
    drawing_sid             VARCHAR(32)                         NOT NULL COMMENT '對應 dms_drawing_master.sid',
    reviewer_user_sid       VARCHAR(32)                         NOT NULL COMMENT '審查人員帳號序號',
    reviewer_role           VARCHAR(50)                         NOT NULL COMMENT '角色 (例: ARCHITECT建築師, STRUCTURAL_ENGINEER結構技師, SITE_PM工地總監)',
    
    review_action           VARCHAR(30)                         NOT NULL COMMENT '動作: APPROVE核准, APPROVE_AS_NOTED修正後核准, REJECT退回, REQUEST_CLARIFICATION補件澄清',
    review_comment          TEXT                                    NULL COMMENT '審查意見與套繪註記 (Redline comments)',
    markup_file_path        VARCHAR(500)                            NULL COMMENT '圈錯/標註繪圖檔 (Redline PDF)',
    
    INDEX idx_ddal_drawing_sid (drawing_sid),
    INDEX idx_ddal_reviewer (reviewer_user_sid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='圖說簽核與審查歷程檔 (Append-Only)';

-- =========================================================
-- 03. 送審文件與計畫書管理 (Document Submittal Control)
-- =========================================================

CREATE TABLE IF NOT EXISTS dms_document_submittal (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '送審單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '送審時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    vendor_sid              VARCHAR(32)                         NOT NULL COMMENT '送審廠商序號 (VendorDB SID)',
    
    submittal_no            VARCHAR(50)                         NOT NULL COMMENT '送審單號 (例: SUB-202608-012)',
    submittal_title         VARCHAR(200)                        NOT NULL COMMENT '送審主題 (例: 10F 牆面石材材料樣品與施工計畫書送審)',
    submittal_type          VARCHAR(30)                         NOT NULL DEFAULT 'MATERIAL_SAMPLE' COMMENT '類型: METHOD_STATEMENT施工計畫書, MATERIAL_SAMPLE材料樣品/型錄, QUALITY_PLAN品質計畫書, TEST_REPORT試驗報告',
    
    specification_code      VARCHAR(50)                             NULL COMMENT '對應規範章節 (例: Section 04200)',
    file_storage_path       VARCHAR(500)                        NOT NULL COMMENT '送審文件檔案路徑 (DMS)',
    
    submittal_status        VARCHAR(20)                         NOT NULL DEFAULT 'SUBMITTED' COMMENT 'SUBMITTED送審中;APPROVED核准;APPROVED_AS_NOTED條件核准;REJECTED退件',
    reviewer_user_sid       VARCHAR(32)                             NULL COMMENT '審核主管/監造帳號序號',
    reviewed_at             DATETIME                                NULL COMMENT '審核時間',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註與審查批示',
    CONSTRAINT uk_dds_project_number UNIQUE (project_sid, submittal_no),
    INDEX idx_dds_project_sid (project_sid),
    INDEX idx_dds_vendor_sid (vendor_sid),
    INDEX idx_dds_status (submittal_status),
    INDEX idx_dds_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程送審文件與計畫書管理檔';

-- =========================================================
-- 04. 圖說分發與簽收紀錄 (Drawing Distribution / Transmittal)
-- =========================================================

CREATE TABLE IF NOT EXISTS dms_drawing_transmittal (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '分發單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '分發時間',
    drawing_sid             VARCHAR(32)                         NOT NULL COMMENT '對應 dms_drawing_master.sid',
    recipient_vendor_sid    VARCHAR(32)                         NOT NULL COMMENT '接收單位/外包廠商序號 (VendorDB SID)',
    
    transmittal_no          VARCHAR(50)                         NOT NULL COMMENT '分發單號 (例: TRN-202608-005)',
    copies_issued           INT                                 NOT NULL DEFAULT 1 COMMENT '發放份數/圖紙張數',
    issue_purpose           VARCHAR(30)                         NOT NULL DEFAULT 'FOR_CONSTRUCTION' COMMENT '發放目的: FOR_CONSTRUCTION施工用, FOR_REVIEW審查用, FOR_INFORMATION參考用',
    
    recipient_name          VARCHAR(100)                            NULL COMMENT '簽收人姓名',
    signed_at               DATETIME                                NULL COMMENT '簽收時間',
    transmittal_status      VARCHAR(20)                         NOT NULL DEFAULT 'ISSUED' COMMENT 'ISSUED已發放;ACKNOWLEDGED已簽收收執',
    
    INDEX idx_ddt_drawing_sid (drawing_sid),
    INDEX idx_ddt_vendor_sid (recipient_vendor_sid),
    INDEX idx_ddt_status (transmittal_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程圖說分發與簽收紀錄檔';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}