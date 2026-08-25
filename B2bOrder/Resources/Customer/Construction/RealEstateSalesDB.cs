namespace B2bOrder.Resources.Customer.Construction
{
    /// <summary>
    /// RealEstateSalesDB V1 Shared Core Schema (房屋仲介 / 成屋買賣租賃 ERP 專用)
    /// 設計目標：
    /// 1. 房屋資產委託刊登檔 (Property Listing Master)：管理賣方/屋主委託之成屋、中古屋、土地物件，含屋況描述、權狀坪數與委託價格。
    /// 2. 帶看與客戶意向紀錄檔 (Property Showing & Feedback)：紀錄經紀人帶看行程、客戶意向評估與屋況反饋。
    /// 3. 要約書與斡旋金管理檔 (Offer & Earnest Money)：紀錄買方要約出價、斡旋金收取/轉訂金狀態與屋主簽認承諾。
    /// 4. 成交買賣契約與傭金拆帳檔 (Real Estate Sales Contract & Commission Split)：紀錄買賣成交總價、雙方服務費、經紀人/店東與加盟總部之傭金拆帳比例。
    /// 5. 整合串接 CustomerDB (買方/賣方客戶)、VendorDB (地政士/代書)、AccountsReceivableDB (服務費應收沖銷) 與 BranchDB (門市據點)。
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class RealEstateSalesDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 房屋資產委託刊登檔 (Property Listing Master)
-- =========================================================

CREATE TABLE IF NOT EXISTS res_property_listing (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '物件刊登序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司/總部序號',
    branch_sid              VARCHAR(32)                         NOT NULL COMMENT '門市/加盟店序號',
    agent_user_sid          VARCHAR(32)                         NOT NULL COMMENT '開發經紀人/專案經紀人序號',
    
    listing_code            VARCHAR(50)                         NOT NULL COMMENT '物件編號 (例: A-202608-001)',
    property_name           VARCHAR(200)                        NOT NULL COMMENT '物件名稱/案名 (例: 遠雄新世代高樓三房車位)',
    property_type           VARCHAR(30)                         NOT NULL DEFAULT 'RESIDENTIAL_APARTMENT' COMMENT '物件類別: RESIDENTIAL_APARTMENT電梯大樓, RESIDENTIAL_HOUSE大樓/別墅, COMMERCIAL_STORE店面, OFFICE辦公室, LAND土地, FACTORY廠房',
    listing_deal_type       VARCHAR(20)                         NOT NULL DEFAULT 'SALE' COMMENT '交易類型: SALE出售, RENT出租, BOTH可售可租',
    
    -- 屋主/委託人資訊
    owner_customer_sid      VARCHAR(32)                         NOT NULL COMMENT '屋主客戶序號 (對應 CustomerDB)',
    owner_name              VARCHAR(100)                        NOT NULL COMMENT '屋主姓名',
    owner_phone             VARCHAR(50)                         NOT NULL COMMENT '屋主聯絡電話',
    agency_contract_type    VARCHAR(20)                         NOT NULL DEFAULT 'EXCLUSIVE' COMMENT '委託類型: EXCLUSIVE專任委託, GENERAL一般委託',
    agency_start_date       DATE                                NOT NULL COMMENT '委託起日',
    agency_end_date         DATE                                NOT NULL COMMENT '委託迄日',
    
    -- 地址與權狀面積 (單位: 坪)
    city                    VARCHAR(50)                         NOT NULL COMMENT '縣市 (例: 台北市)',
    district                VARCHAR(50)                         NOT NULL COMMENT '鄉鎮市區 (例: 信義區)',
    address                 VARCHAR(250)                        NOT NULL COMMENT '完整地址',
    building_age            DECIMAL(5,1)                        NOT NULL DEFAULT 0.0 COMMENT '屋齡 (年)',
    floor_current           VARCHAR(20)                         NOT NULL COMMENT '樓層 (例: 8F, 整棟)',
    floor_total             INT                                 NOT NULL DEFAULT 1 COMMENT '總樓層數',
    
    main_building_area_ping DECIMAL(10,2)                       NOT NULL DEFAULT 0.00 COMMENT '主建物面積 (坪)',
    balcony_area_ping       DECIMAL(10,2)                       NOT NULL DEFAULT 0.00 COMMENT '附屬建物/陽台 (坪)',
    common_area_ping        DECIMAL(10,2)                       NOT NULL DEFAULT 0.00 COMMENT '公共設施面積 (坪)',
    parking_area_ping       DECIMAL(10,2)                       NOT NULL DEFAULT 0.00 COMMENT '車位坪數 (坪)',
    total_ping              DECIMAL(10,2)                       NOT NULL DEFAULT 0.00 COMMENT '權狀總坪數 (坪)',
    
    -- 委託價格 (單位: TWD)
    target_price            DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '委託底價/售價 (TWD)',
    bottom_price            DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '成交底價 (內部保密)',
    estimated_monthly_rent  DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '若出租時之月租金 (TWD)',
    
    -- 物件狀態
    listing_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT '狀態: ACTIVE委託銷售中, OFFERING斡旋中, DEAL_CLOSED已成交, EXPIRED委託到期, TERMINATED解約/撤刊',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '屋況特色與備註說明',
    CONSTRAINT uk_rpl_company_code UNIQUE (company_sid, listing_code),
    INDEX idx_rpl_company_sid (company_sid),
    INDEX idx_rpl_branch_sid (branch_sid),
    INDEX idx_rpl_agent_user_sid (agent_user_sid),
    INDEX idx_rpl_owner (owner_customer_sid),
    INDEX idx_rpl_status (listing_status),
    INDEX idx_rpl_city_dist (city, district),
    INDEX idx_rpl_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='房屋資產委託刊登檔';

-- =========================================================
-- 02. 帶看與客戶意向紀錄檔 (Property Showing Log)
-- =========================================================

CREATE TABLE IF NOT EXISTS res_property_showing (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '帶看紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    property_listing_sid    VARCHAR(32)                         NOT NULL COMMENT '對應 res_property_listing.sid',
    showing_agent_user_sid  VARCHAR(32)                         NOT NULL COMMENT '帶看經紀人序號',
    buyer_customer_sid      VARCHAR(32)                         NOT NULL COMMENT '帶看買方客戶序號 (對應 CustomerDB)',
    
    showing_time            DATETIME                            NOT NULL COMMENT '帶看時間',
    buyer_feedback_rating   INT                                 NOT NULL DEFAULT 3 COMMENT '買方滿意度評分 (1~5分)',
    buyer_intent_level      VARCHAR(20)                         NOT NULL DEFAULT 'MEDIUM' COMMENT '買方意向: HIGH高(打算付斡旋), MEDIUM中(考慮中), LOW低(不喜歡/價位太高)',
    buyer_comments          TEXT                                    NULL COMMENT '買方反饋意見 (例: 採光好但廁所無窗)',
    follow_up_action        VARCHAR(200)                            NULL COMMENT '後續追蹤動作 (例: 補送謄本資料/再約二次帶看)',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '帶看備註',
    INDEX idx_rps_listing_sid (property_listing_sid),
    INDEX idx_rps_agent_sid (showing_agent_user_sid),
    INDEX idx_rps_buyer_sid (buyer_customer_sid),
    INDEX idx_rps_showing_time (showing_time),
    INDEX idx_rps_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='帶看與客戶意向紀錄檔';

-- =========================================================
-- 03. 要約書與斡旋金管理檔 (Offer & Earnest Money)
-- =========================================================

CREATE TABLE IF NOT EXISTS res_property_offer (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '要約/斡旋序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '下斡旋時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    property_listing_sid    VARCHAR(32)                         NOT NULL COMMENT '對應 res_property_listing.sid',
    buyer_customer_sid      VARCHAR(32)                         NOT NULL COMMENT '買方客戶序號 (對應 CustomerDB)',
    agent_user_sid          VARCHAR(32)                         NOT NULL COMMENT '承辦經紀人序號',
    
    offer_code              VARCHAR(50)                         NOT NULL COMMENT '要約/斡旋單號 (例: OFF-202608-005)',
    offer_type              VARCHAR(20)                         NOT NULL DEFAULT 'EARNEST_MONEY' COMMENT '出價形式: EARNEST_MONEY斡旋金, WRITTEN_OFFER內政部要約書',
    
    offered_price           DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '買方出價總額 (TWD)',
    earnest_money_amount    DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '斡旋金/保證金金額 (TWD)',
    earnest_payment_method  VARCHAR(30)                         NOT NULL DEFAULT 'CASH' COMMENT '斡旋金支付方式: CASH現金, TICKET支票, TRANSFER銀行轉帳',
    
    valid_until_date        DATETIME                            NOT NULL COMMENT '要約有效期限',
    owner_signed_date       DATETIME                                NULL COMMENT '屋主同意簽認時間 (轉為定金時間)',
    
    offer_status            VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT '狀態: ACTIVE斡旋中, ACCEPTED屋主同意成交, REJECTED屋主拒絕, REFUNDED轉款退回, CANCELLED買方撤回',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '出價附帶條件 (例: 需含冷氣家電)',
    CONSTRAINT uk_rpo_offer_code UNIQUE (offer_code),
    INDEX idx_rpo_listing_sid (property_listing_sid),
    INDEX idx_rpo_buyer_sid (buyer_customer_sid),
    INDEX idx_rpo_status (offer_status),
    INDEX idx_rpo_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='要約書與斡旋金管理檔';

-- =========================================================
-- 04. 成交買賣契約與傭金拆帳檔 (Sales Contract & Commission Split)
-- =========================================================

CREATE TABLE IF NOT EXISTS res_sales_contract (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '買賣契約序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '簽約時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    branch_sid              VARCHAR(32)                         NOT NULL COMMENT '簽約門市序號',
    property_listing_sid    VARCHAR(32)                         NOT NULL COMMENT '對應 res_property_listing.sid',
    offer_sid               VARCHAR(32)                             NULL COMMENT '對應 res_property_offer.sid',
    
    contract_no             VARCHAR(50)                         NOT NULL COMMENT '成屋買賣契約單號 (例: RES-202608-088)',
    signing_date            DATE                                NOT NULL COMMENT '正式簽約日期',
    handover_date           DATE                                    NULL COMMENT '預定交屋日期',
    
    -- 雙方與地政士
    buyer_customer_sid      VARCHAR(32)                         NOT NULL COMMENT '買方客戶序號',
    owner_customer_sid      VARCHAR(32)                         NOT NULL COMMENT '賣方客戶序號',
    scrivener_vendor_sid    VARCHAR(32)                             NULL COMMENT '地政士/代書廠商序號 (對應 VendorDB)',
    escrow_bank_account     VARCHAR(100)                            NULL COMMENT '履約保證履保專戶帳號',
    
    -- 最終成交與服務費 (TWD)
    final_deal_price        DECIMAL(14,2)                       NOT NULL DEFAULT 0.00 COMMENT '最終成交總總價 (TWD)',
    buyer_service_fee       DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '買方服務費 (最高2%)',
    seller_service_fee      DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '賣方服務費 (最高4%)',
    total_service_fee       DECIMAL(12,2)                       NOT NULL DEFAULT 0.00 COMMENT '總服務費/總佣金 (買+賣)',
    
    -- 經紀人拆帳分潤 (Commission Split)
    listing_agent_user_sid  VARCHAR(32)                         NOT NULL COMMENT '開發經紀人序號',
    listing_agent_split_pct DECIMAL(5,2)                        NOT NULL DEFAULT 50.00 COMMENT '開發獎金拆帳比例 (%)',
    selling_agent_user_sid  VARCHAR(32)                         NOT NULL COMMENT '銷售經紀人序號',
    selling_agent_split_pct DECIMAL(5,2)                        NOT NULL DEFAULT 50.00 COMMENT '銷售獎金拆帳比例 (%)',
    
    contract_status         VARCHAR(20)                         NOT NULL DEFAULT 'SIGNED' COMMENT '狀態: SIGNED已簽約履保中, ESCROW_COMPLETED履保結案, HANDOVER_COMPLETED已交屋結案, TERMINATED簽約後解約',
    ar_invoice_sid          VARCHAR(32)                             NULL COMMENT '連動應收帳款 AccountsReceivableDB 服務費憑證序號',
    
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '特約事項與代書叮嚀',
    CONSTRAINT uk_rsc_contract_no UNIQUE (contract_no),
    INDEX idx_rsc_company_sid (company_sid),
    INDEX idx_rsc_branch_sid (branch_sid),
    INDEX idx_rsc_listing_sid (property_listing_sid),
    INDEX idx_rsc_buyer (buyer_customer_sid),
    INDEX idx_rsc_owner (owner_customer_sid),
    INDEX idx_rsc_status (contract_status),
    INDEX idx_rsc_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='成交買賣契約與傭金拆帳檔';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}