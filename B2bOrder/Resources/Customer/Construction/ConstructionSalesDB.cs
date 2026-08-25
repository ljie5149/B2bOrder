namespace B2bOrder.Resources.Customer.Construction
{
    /// <summary>
    /// ConstructionSalesDB V1 Shared Core Schema (不動產開發商 / 預售屋銷售 ERP 專用)
    /// 設計目標：
    /// 1. 戶別銷控與房屋資產主檔 (Property Unit Master)：管理建案所有戶別 (棟別/樓層/戶號/車位)、坪數、計價與銷控狀態 (待售/訂位/已簽約/已交屋)
    /// 2. 購屋訂單與買賣契約 (Sales Order & Contract Master)：紀錄訂金、簽約總價 (房/地/車位拆分)、買方客戶資料與代銷團隊
    /// 3. 分期應繳款項與工程期款期程 (Payment Installment Schedule)：自動產出訂簽金、工程款 (1~N期)、結構頂樓款、使照款、交屋款與房貸期程
    /// 4. 客戶客變申請與追加減帳單 (Customer Change Request Log)：紀錄格局變更、退建材、追加減總額，連動 DrawingDocumentDB 圖說
    /// 5. 整合串接 ProjectDB (建案)、AccountsReceivableDB (應收帳款/催繳)、DrawingDocumentDB (客變圖說) 與 CustomerDB (買方資料)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class ConstructionSalesDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 戶別銷控與房屋資產主檔 (Property Unit Master & Inventory)
-- =========================================================

CREATE TABLE IF NOT EXISTS sal_property_unit (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '戶別序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    
    building_block          VARCHAR(20)                         NOT NULL COMMENT '棟別 (例: A棟, B棟, C棟)',
    floor_level             VARCHAR(20)                         NOT NULL COMMENT '樓層 (例: 3F, 15F, B1F)',
    unit_number             VARCHAR(50)                         NOT NULL COMMENT '戶號 (例: A1, B3, 15F-2)',
    full_unit_code          VARCHAR(100)                        NOT NULL COMMENT '完整戶號代碼 (例: A-15F-1)',
    
    unit_type               VARCHAR(30)                         NOT NULL DEFAULT 'RESIDENTIAL' COMMENT '用途類別: RESIDENTIAL住宅, COMMERCIAL店面, OFFICE辦公室, PARKING車位',
    layout_type             VARCHAR(50)                             NULL COMMENT '格局房型 (例: 2房2廳1衛, 3房2廳2衛)',
    
    -- 面積坪數資訊
    main_building_area_ping DECIMAL(10,2)                       NOT NULL DEFAULT 0.00 COMMENT '主建物面積 (坪)',
    balcony_area_ping       DECIMAL(10,2)                       NOT NULL DEFAULT 0.00 COMMENT '陽台/附屬建物面積 (坪)',
    common_area_ping        DECIMAL(10,2)                       NOT NULL DEFAULT 0.00 COMMENT '共有/公設面積 (坪)',
    total_sales_area_ping   DECIMAL(10,2)                       NOT NULL DEFAULT 0.00 COMMENT '權狀總銷售坪數 (坪)',
    
    -- 計價資訊 (單位: 新台幣 TWD)
    list_price_house        DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '房屋牌價/表價 (未稅)',
    list_price_land         DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '土地牌價/表價',
    list_price_total        DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '底價/表價總價',
    
    -- 銷控狀態
    sales_status            VARCHAR(20)                         NOT NULL DEFAULT 'AVAILABLE' COMMENT '銷控狀態: AVAILABLE待售, RESERVED保留/大訂, CONTRACTED已簽約, HANDOVER已交屋, LOCKED建設公司自留',
    current_contract_sid    VARCHAR(32)                             NULL COMMENT '當前有效之買賣契約序號 (對應 sal_sales_contract.sid)',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '採光/景觀/備註事項',
    CONSTRAINT uk_spu_project_unit UNIQUE (project_sid, full_unit_code),
    INDEX idx_spu_company_sid (company_sid),
    INDEX idx_spu_project_sid (project_sid),
    INDEX idx_spu_status (sales_status),
    INDEX idx_spu_type (unit_type),
    INDEX idx_spu_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='戶別銷控與房屋資產主檔';

-- =========================================================
-- 02. 購屋訂單與正式買賣契約檔 (Sales Order & Purchase Contract)
-- =========================================================

CREATE TABLE IF NOT EXISTS sal_sales_contract (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '契約序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '下訂/開單時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    unit_sid                VARCHAR(32)                         NOT NULL COMMENT '購買戶別 (對應 sal_property_unit.sid)',
    
    contract_no             VARCHAR(50)                         NOT NULL COMMENT '契約單號 (例: CTR-202608-019)',
    order_date              DATE                                NOT NULL COMMENT '訂購日期',
    signing_date            DATE                                    NULL COMMENT '正式簽約日期',
    
    -- 買方客戶資料 (可串接 CustomerDB/CRM)
    buyer_customer_sid      VARCHAR(32)                         NOT NULL COMMENT '買方客戶序號',
    buyer_name              VARCHAR(100)                        NOT NULL COMMENT '買方姓名/公司名稱',
    buyer_id_number         VARCHAR(50)                         NOT NULL COMMENT '身分證字號/統一編號',
    buyer_phone             VARCHAR(50)                         NOT NULL COMMENT '聯絡電話',
    buyer_address           VARCHAR(200)                        NOT NULL COMMENT '通訊地址',
    
    -- 交易金額明細 (單位: TWD)
    house_agreed_price      DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '房屋約定成交價 (未稅)',
    land_agreed_price       DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '土地約定成交價',
    parking_agreed_price    DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '車位成交總價',
    total_contract_price    DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '成交總金額 (房+地+車位)',
    
    -- 銷售管道與代銷
    agency_vendor_sid       VARCHAR(32)                             NULL COMMENT '代銷公司廠商序號 (VendorDB SID)',
    sales_agent_name        VARCHAR(100)                            NULL COMMENT '負責銷售人員/案場專員姓名',
    
    contract_status         VARCHAR(20)                         NOT NULL DEFAULT 'RESERVED' COMMENT '狀態: RESERVED已付定金, SIGNED正式簽約, CANCELLED解約/退訂, COMPLETED交屋結案',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '特約條款與備註',
    CONSTRAINT uk_ssc_project_contract_no UNIQUE (project_sid, contract_no),
    INDEX idx_ssc_company_sid (company_sid),
    INDEX idx_ssc_project_sid (project_sid),
    INDEX idx_ssc_unit_sid (unit_sid),
    INDEX idx_ssc_buyer (buyer_customer_sid),
    INDEX idx_ssc_status (contract_status),
    INDEX idx_ssc_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='購屋訂單與正式買賣契約檔';

-- =========================================================
-- 03. 分期應繳款項與工程期款期程 (Payment Installment Schedule)
-- =========================================================

CREATE TABLE IF NOT EXISTS sal_payment_installment (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '分期明細序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    contract_sid            VARCHAR(32)                         NOT NULL COMMENT '對應 sal_sales_contract.sid',
    
    installment_stage       VARCHAR(50)                         NOT NULL COMMENT '期數類別: DEPOSIT小訂/大訂, SIGNING簽約金, CONSTRUCTION_01工程款第1期, STRUCTURE_TOPPING結構頂樓款, LICENSE_ISSUANCE使照核發款, MORTGAGE_LOAN銀行貸款, HANDOVER交屋保留款',
    stage_sequence          INT                                 NOT NULL DEFAULT 1 COMMENT '期序 (例: 1, 2, 3...)',
    stage_description       VARCHAR(100)                        NOT NULL COMMENT '期數說明 (例: 第1期 1F版結構完成款)',
    
    due_date                DATE                                NOT NULL COMMENT '預定應繳款日期',
    due_amount              DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '本期應繳金額 (TWD)',
    paid_amount             DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '已累計實收金額 (TWD)',
    
    payment_status          VARCHAR(20)                         NOT NULL DEFAULT 'UNPAID' COMMENT '狀態: UNPAID未繳, PARTIAL部分繳納, PAID已全額繳清, OVERDUE逾期催繳中',
    last_paid_at            DATETIME                                NULL COMMENT '最後一次款項沖銷時間',
    ar_invoice_sid          VARCHAR(32)                             NULL COMMENT '連動應收帳款 AccountsReceivableDB 憑證序號',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '催繳備註',
    INDEX idx_spi_contract_sid (contract_sid),
    INDEX idx_spi_due_date (due_date),
    INDEX idx_spi_status (payment_status),
    INDEX idx_spi_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='分期應繳款項與工程期款期程檔';

-- =========================================================
-- 04. 客戶客變申請與追加減帳單 (Customer Change Request Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS sal_customer_change_order (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '客變單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '申請時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    contract_sid            VARCHAR(32)                         NOT NULL COMMENT '對應 sal_sales_contract.sid',
    project_sid             VARCHAR(32)                         NOT NULL COMMENT 'ProjectDB 建案專案序號',
    
    change_order_no         VARCHAR(50)                         NOT NULL COMMENT '客變單號 (例: CHO-202608-003)',
    request_date            DATE                                NOT NULL COMMENT '客變提出日期',
    
    change_category         VARCHAR(50)                         NOT NULL COMMENT '客變類別: WALL_LAYOUT隔間變更, PLUMBING_ELECTRICAL水電管線, FINISHING_MATERIAL建材退換, SANITARY_WARE衛浴設備',
    change_description      TEXT                                NOT NULL COMMENT '客變詳細需求與變更說明',
    
    -- 追加減財務計算 (單位: TWD)
    add_amount              DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '客變追加金額 (+)',
    deduct_amount           DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '客變追減金額 (-)',
    net_change_amount       DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '淨追加減金額 (+為買方補繳, -為建商退款)',
    
    -- 圖說與審核
    drawing_document_sid    VARCHAR(32)                             NULL COMMENT '對應 DrawingDocumentDB 客變核定圖說序號',
    architect_approved_sid  VARCHAR(32)                             NULL COMMENT '建築師/機電技師審核序號',
    customer_signed_at      DATETIME                                NULL COMMENT '客戶客變圖說與金額確認簽章時間',
    
    status                  VARCHAR(20)                         NOT NULL DEFAULT 'SUBMITTED' COMMENT 'STATUS: SUBMITTED已申請;ESTIMATING報價中;APPROVED買方已簽認;REJECTED退件;IN_CONSTRUCTION施工中',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '現場施工施工備註',
    CONSTRAINT uk_scco_change_order_no UNIQUE (change_order_no),
    INDEX idx_scco_contract_sid (contract_sid),
    INDEX idx_scco_project_sid (project_sid),
    INDEX idx_scco_status (status),
    INDEX idx_scco_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='客戶客變申請與追加減帳單檔';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}