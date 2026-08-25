namespace B2bOrder.Resources.ShareCore.Construction
{
    /// <summary>
    /// MIMDB V1 Shared Core Schema (工程材料與庫存管理資料庫 - 由 PIMDB 調整擴充)
    /// 設計目標：
    /// 1. 工程材料主檔 (Material Master)：管理建材 (鋼筋/混凝土/板材) 之規格、CNS/PCCES 編碼與預設損耗率
    /// 2. 工地與倉儲庫存管理 (Warehouse & On-Site Inventory)：管理總庫與各大建案工地現場堆置場 (Yard) 之即時庫存
    /// 3. 材料進退場與領料異動歷程 (Material Transaction Log)：紀錄到場驗收簽收、工地領用 (指定樓層工區)、撥轉與退料
    /// 4. 現場盤點與損耗控制 (Stock Taking & Wastage Control)：監控實際材料損耗 (Actual Wastage) 與盤盈虧調整
    /// 5. 整合串接 ProjectDB (建案)、ProcurementDB (採購單 PO)、VendorDB (供應商) 與 BudgetCostDB (成本歸攤)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class MIMDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 工程材料品類與主檔 (Material Master - 由 Product Master 調整)
-- =========================================================

CREATE TABLE IF NOT EXISTS mim_material_master (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '材料序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    
    material_code           VARCHAR(50)                         NOT NULL UNIQUE COMMENT '材料編號 (例: MAT-REBAR-SD420-15, MAT-CONC-4000)',
    pcces_code              VARCHAR(50)                             NULL COMMENT '公共工程 PCCES 標準編碼 (例: 0321021000)',
    material_name           VARCHAR(200)                        NOT NULL COMMENT '材料名稱 (例: SD420W 鋼筋 #5, 4000PSI 預拌混凝土)',
    
    category                VARCHAR(50)                         NOT NULL COMMENT '大類: STRUCTURAL結構材, ARCHITECTURAL裝修材, MEP機電管線, FINISHING塗料/石材, SAFETY安衛耗材',
    sub_category            VARCHAR(50)                             NULL COMMENT '中子類 (例: REBAR鋼筋, CONCRETE混凝土, TILE磁磚, PIPE管線)',
    specification           VARCHAR(200)                            NULL COMMENT '技術規格與強度 (例: CNS 560, ASTM A615, FY=4200kgf/cm2)',
    
    base_unit               VARCHAR(20)                         NOT NULL COMMENT '基本計量單位 (例: 噸, m3, m2, 支, 捲, kg)',
    standard_unit_price     DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '預算標準單價 (未稅)',
    standard_wastage_rate   DECIMAL(5,2)                        NOT NULL DEFAULT 0.00 COMMENT '標準允許損耗率 (%) (例: 鋼筋 3.00%, 磁磚 5.00%)',
    
    is_hazmat               VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT '是否為危險化學品/易燃物 (Y/N)',
    msds_document_sid       VARCHAR(32)                             NULL COMMENT '物質安全資料表 MSDS 文件序號 (DMS)',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '材料詳細說明與儲存規範',
    INDEX idx_mmm_company_sid (company_sid),
    INDEX idx_mmm_category (category),
    INDEX idx_mmm_pcces (pcces_code),
    INDEX idx_mmm_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工程材料主檔 (Material Master)';

-- =========================================================
-- 02. 工地與倉庫庫存主檔 (Project Yard & Warehouse Inventory)
-- =========================================================

CREATE TABLE IF NOT EXISTS mim_inventory_stock (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '庫存記錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    project_sid             VARCHAR(32)                             NULL COMMENT 'ProjectDB 建案專案序號 (若為公司中央總庫則為 NULL)',
    warehouse_code          VARCHAR(50)                         NOT NULL COMMENT '倉庫/工地堆置場代碼 (例: YARD-SITE-A, WH-CENTRAL)',
    
    material_sid            VARCHAR(32)                         NOT NULL COMMENT '對應 mim_material_master.sid',
    location_bin            VARCHAR(50)                             NULL COMMENT '庫位/儲位號碼 (例: B1F-ZONE-C, A-02-15)',
    
    qty_on_hand             DECIMAL(12,4)                       NOT NULL DEFAULT 0.0000 COMMENT '現場現存數量 (On-Hand)',
    qty_allocated           DECIMAL(12,4)                       NOT NULL DEFAULT 0.0000 COMMENT '已預約/已派發未領用量',
    qty_available           DECIMAL(12,4)                       NOT NULL DEFAULT 0.0000 COMMENT '可用庫存量 (On-Hand minus Allocated)',
    
    min_safety_qty          DECIMAL(12,4)                       NOT NULL DEFAULT 0.0000 COMMENT '安全庫存下限 (低於此值自動提醒補料)',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    CONSTRAINT uk_mis_project_wh_material UNIQUE (project_sid, warehouse_code, material_sid, location_bin),
    INDEX idx_mis_company_sid (company_sid),
    INDEX idx_mis_project_sid (project_sid),
    INDEX idx_mis_material_sid (material_sid),
    INDEX idx_mis_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工地與倉庫即時庫存檔';

-- =========================================================
-- 03. 材料異動與領退料紀錄 (Material Transaction Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS mim_inventory_transaction (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '異動單號序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '異動發生時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案專案序號',
    
    material_sid            VARCHAR(32)                         NOT NULL COMMENT '對應 mim_material_master.sid',
    warehouse_code          VARCHAR(50)                         NOT NULL COMMENT '倉庫/工地堆置場代碼',
    
    txn_type                VARCHAR(30)                         NOT NULL COMMENT '異動類型: GOODS_RECEIPT進場點收, ISSUE_TO_SITE領料施工, SITE_TRANSFER工地間調撥, RETURN_TO_STOCK退庫, ADJUSTMENT盤點調整',
    ref_po_sid              VARCHAR(32)                             NULL COMMENT '關聯 ProcurementDB 採購單 PO SID (若為進場點收)',
    ref_subcontractor_sid   VARCHAR(32)                             NULL COMMENT '領料外包廠商序號 (VendorDB SID)',
    
    target_work_zone        VARCHAR(100)                            NULL COMMENT '施作工區/樓層 (例: 12F 牆柱結構, B2F 機電機房)',
    
    quantity                DECIMAL(12,4)                       NOT NULL COMMENT '異動數量 (正數為入庫/異動增加，負數為出庫/領料)',
    unit_price              DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '異動當下加權平均單價/進貨單價 (未稅)',
    total_amount            DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '異動總金額 (未稅)',
    
    operator_user_sid       VARCHAR(32)                         NOT NULL COMMENT '經辦工程師/倉管帳號序號',
    signed_by_receiver      VARCHAR(100)                            NULL COMMENT '領料簽收人/外包工頭姓名',
    
    remark                  TEXT                                    NULL COMMENT '備註說明',
    INDEX idx_mit_project_sid (project_sid),
    INDEX idx_mit_material_sid (material_sid),
    INDEX idx_mit_txn_type (txn_type),
    INDEX idx_mit_ref_po (ref_po_sid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='材料進退場與領異動明細檔 (Append-Only)';

-- =========================================================
-- 04. 工地盤點與損耗調整單 (Stock Taking & Wastage Audit)
-- =========================================================

CREATE TABLE IF NOT EXISTS mim_stock_taking (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '盤點單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '開單時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT '建案專案序號',
    warehouse_code          VARCHAR(50)                         NOT NULL COMMENT '盤點倉庫/堆置場代碼',
    audit_no                VARCHAR(50)                         NOT NULL COMMENT '盤點單號 (例: STK-202608-001)',
    
    audit_date              DATE                                NOT NULL COMMENT '盤點基準日期',
    audit_status            VARCHAR(20)                         NOT NULL DEFAULT 'DRAFT' COMMENT '狀態: DRAFT草稿, AUDITING盤點中, APPROVED已核准損耗調整, CANCELLED作廢',
    
    audited_by_user_sid     VARCHAR(32)                         NOT NULL COMMENT '盤點主導工程師序號',
    approved_by_user_sid    VARCHAR(32)                             NULL COMMENT '核准主管/工地總監序號',
    approved_at             DATETIME                                NULL COMMENT '核准時間',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '盤點異常與損耗原因說明',
    CONSTRAINT uk_mst_audit_no UNIQUE (audit_no),
    INDEX idx_mst_project_sid (project_sid),
    INDEX idx_mst_status (audit_status),
    INDEX idx_mst_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工地材料盤點與損耗控制主檔';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}