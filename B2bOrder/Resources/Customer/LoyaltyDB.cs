namespace B2bOrder.Resources.Customer
{
    /// <summary>
    /// LoyaltyDB V1 Shared Core Schema
    /// 設計目標：
    /// 1. 管理會員忠誠度等級 (Loyalty Tier)、升等規則與等級權益 (Tier Benefits)
    /// 2. 支援會員點數帳戶 (Points Account)、點數發放/扣減流水帳 (Points Ledger) 與過期機制
    /// 3. 管理優惠券/折扣券模板 (Coupon Template)、會員領取之優惠券 (Customer Coupon) 與核銷 (Redemption)
    /// 4. 提供點數兌換商品/贈品 (Reward Item & Exchange Order)
    /// 5. 整合與串接 SalesOrderDB (消費給點/折抵)、CustomerDB/MemberDB (會員資料)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class LoyaltyDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 會員等級與權益 (Loyalty Tier & Benefits)
-- =========================================================

CREATE TABLE IF NOT EXISTS lyt_tier (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '等級序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    tier_code               VARCHAR(50)                         NOT NULL COMMENT '等級代碼 (例: BRONZE, SILVER, GOLD, PLATINUM)',
    tier_name               VARCHAR(100)                        NOT NULL COMMENT '等級名稱 (例: 銅級會員、金級會員)',
    tier_level              INT                                 NOT NULL DEFAULT 1 COMMENT '等級層級/排序 (數值越大越高階)',
    min_accumulated_spend   DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '晉升所需最低累積消費額',
    min_accumulated_points  INT                                 NOT NULL DEFAULT 0 COMMENT '晉升所需最低累積點數',
    points_multiplier       DECIMAL(5,2)                        NOT NULL DEFAULT 1.00 COMMENT '點數累積加倍倍率 (例: 1.5 倍)',
    tier_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_lt_company_tier UNIQUE (company_sid, tier_code),
    INDEX idx_lt_company_sid (company_sid),
    INDEX idx_lt_tier_level (tier_level),
    INDEX idx_lt_status (tier_status),
    INDEX idx_lt_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='會員忠誠度等級檔';

CREATE TABLE IF NOT EXISTS lyt_customer_tier (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '會員等級紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    customer_sid            VARCHAR(32)                         NOT NULL COMMENT 'CustomerDB/MemberDB 會員序號',
    tier_nid                BIGINT UNSIGNED                     NOT NULL COMMENT '當前等級流水號',
    effective_date          DATETIME                            NOT NULL COMMENT '等級生效日期',
    expiration_date         DATETIME                                NULL COMMENT '等級到期日期 (NULL表示永久)',
    current_period_spend    DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '當前保級週期累積消費金額',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_lct_tier
        FOREIGN KEY (tier_nid) REFERENCES lyt_tier(nid),
    CONSTRAINT uk_lct_customer UNIQUE (customer_sid),
    INDEX idx_lct_customer_sid (customer_sid),
    INDEX idx_lct_tier_nid (tier_nid),
    INDEX idx_lct_exp_date (expiration_date),
    INDEX idx_lct_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='會員當前等級狀態檔';

-- =========================================================
-- 02. 會員點數帳戶與異動流水 (Points Account & Ledger)
-- =========================================================

CREATE TABLE IF NOT EXISTS lyt_points_account (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '點數帳戶序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    customer_sid            VARCHAR(32)                         NOT NULL COMMENT '會員序號',
    total_points            INT                                 NOT NULL DEFAULT 0 COMMENT '當前可用總點數',
    locked_points           INT                                 NOT NULL DEFAULT 0 COMMENT '鎖定/預扣中點數',
    expired_points_accum    INT                                 NOT NULL DEFAULT 0 COMMENT '歷史累計已過期點數',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    CONSTRAINT uk_lpa_company_customer UNIQUE (company_sid, customer_sid),
    INDEX idx_lpa_customer_sid (customer_sid),
    INDEX idx_lpa_company_sid (company_sid),
    INDEX idx_lpa_avalible (avalible),
    CHECK (total_points >= 0),
    CHECK (locked_points >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='會員點數總帳戶';

CREATE TABLE IF NOT EXISTS lyt_points_ledger (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '點數流水序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '異動時間',
    account_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '點數帳戶流水號',
    customer_sid            VARCHAR(32)                         NOT NULL COMMENT '會員序號',
    transaction_type        VARCHAR(30)                         NOT NULL COMMENT 'EARN消費贈點;REDEEM訂單折抵;EXCHANGE兌換贈品;EXPIRE過期扣除;ADJUST_ADD人工補點;ADJUST_SUB人工扣點;CANCEL訂單取消退點',
    points_change           INT                                 NOT NULL COMMENT '變動點數 (正數增加/負數減少)',
    points_after            INT                                 NOT NULL COMMENT '異動後帳戶總點數',
    expiration_date         DATETIME                                NULL COMMENT '該筆獲取點數之到期日',
    reference_doc_type      VARCHAR(50)                             NULL COMMENT '關聯單據類型 (例: SALES_ORDER, EXCHANGE_ORDER, MANUAL)',
    reference_doc_no        VARCHAR(100)                            NULL COMMENT '關聯單據編號',
    operator_user_sid       VARCHAR(32)                             NULL COMMENT '操作人員帳號序號 (人工調整時使用)',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_lpl_account
        FOREIGN KEY (account_nid) REFERENCES lyt_points_account(nid),
    INDEX idx_lpl_account_nid (account_nid),
    INDEX idx_lpl_customer_sid (customer_sid),
    INDEX idx_lpl_tx_type (transaction_type),
    INDEX idx_lpl_ref_doc (reference_doc_type, reference_doc_no),
    INDEX idx_lpl_exp_date (expiration_date),
    INDEX idx_lpl_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='點數異動流水帳 (不可修改，僅供稽核)';

-- =========================================================
-- 03. 優惠券 / 折扣券管理 (Coupon Template & Customer Coupons)
-- =========================================================

CREATE TABLE IF NOT EXISTS lyt_coupon_template (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '優惠券模板序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    template_code           VARCHAR(50)                         NOT NULL COMMENT '模板代碼',
    title                   VARCHAR(200)                        NOT NULL COMMENT '優惠券標題',
    discount_type           VARCHAR(30)                         NOT NULL DEFAULT 'FIXED_AMOUNT' COMMENT 'FIXED_AMOUNT定額折扣;PERCENTAGE折扣百分比;FREE_SHIPPING免運券',
    discount_value          DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '折扣值 (金額或百分比，如10.00表示10% off)',
    min_purchase_amount     DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '最低消費門檻金額',
    max_discount_amount     DECIMAL(18,4)                           NULL COMMENT '最高折扣金額上限 (百分比折抵時限制)',
    total_quantity          INT                                 NOT NULL DEFAULT -1 COMMENT '總發行數量 (-1為無限制)',
    issued_quantity         INT                                 NOT NULL DEFAULT 0 COMMENT '已發放數量',
    used_quantity           INT                                 NOT NULL DEFAULT 0 COMMENT '已核銷數量',
    validity_type           VARCHAR(20)                         NOT NULL DEFAULT 'DATE_RANGE' COMMENT 'DATE_RANGE固定日期範圍;RELATIVE_DAYS領券後相對天數',
    start_date              DATETIME                                NULL COMMENT '生效開始時間',
    end_date                DATETIME                                NULL COMMENT '生效結束時間',
    relative_days           INT                                     NULL COMMENT '領券後有效天數',
    template_status         VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用;EXPIRED已過期',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_lct_company_code UNIQUE (company_sid, template_code),
    INDEX idx_lct_company_sid (company_sid),
    INDEX idx_lct_discount_type (discount_type),
    INDEX idx_lct_status (template_status),
    INDEX idx_lct_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='優惠券模板主檔';

CREATE TABLE IF NOT EXISTS lyt_customer_coupon (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '會員優惠券序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '領取/獲取時間',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    coupon_template_nid     BIGINT UNSIGNED                     NOT NULL COMMENT '優惠券模板流水號',
    customer_sid            VARCHAR(32)                         NOT NULL COMMENT '會員序號',
    coupon_code             VARCHAR(100)                        NOT NULL COMMENT '優惠券券碼 (唯一碼)',
    start_time              DATETIME                            NOT NULL COMMENT '有效開始時間',
    end_time                DATETIME                            NOT NULL COMMENT '有效結束時間',
    coupon_status           VARCHAR(20)                         NOT NULL DEFAULT 'UNUSED' COMMENT 'UNUSED未使用;LOCKED預扣/下單中;USED已使用;EXPIRED已過期;CANCELLED已廢止',
    used_time               DATETIME                                NULL COMMENT '使用/核銷時間',
    used_sales_order_sid    VARCHAR(32)                             NULL COMMENT '核銷使用的銷售單序號',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_lcc_template
        FOREIGN KEY (coupon_template_nid) REFERENCES lyt_coupon_template(nid),
    CONSTRAINT uk_lcc_coupon_code UNIQUE (coupon_code),
    INDEX idx_lcc_template_nid (coupon_template_nid),
    INDEX idx_lcc_customer_sid (customer_sid),
    INDEX idx_lcc_status (coupon_status),
    INDEX idx_lcc_end_time (end_time),
    INDEX idx_lcc_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='會員持有優惠券明細檔';

-- =========================================================
-- 04. 點數兌換商品與兌換訂單 (Reward Items & Exchange Orders)
-- =========================================================

CREATE TABLE IF NOT EXISTS lyt_reward_item (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '兌換商品序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    item_title              VARCHAR(200)                        NOT NULL COMMENT '兌換品名稱',
    reward_type             VARCHAR(30)                         NOT NULL DEFAULT 'COUPON' COMMENT 'COUPON優惠券;PHYSICAL實體商品;DIGITAL電子序號',
    required_points         INT                                 NOT NULL COMMENT '所需兌換點數',
    additional_amount       DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '加購金額 (點數+自費)',
    target_coupon_tmpl_nid  BIGINT UNSIGNED                         NULL COMMENT '若為優惠券時對應的模板流水號',
    pim_item_sid            VARCHAR(32)                             NULL COMMENT '若為實體商品時對應 PIM/MIMDB Item序號',
    stock_qty               INT                                 NOT NULL DEFAULT 0 COMMENT '可用可兌換庫存',
    item_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE上架;INACTIVE下架',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_lri_coupon_tmpl
        FOREIGN KEY (target_coupon_tmpl_nid) REFERENCES lyt_coupon_template(nid),
    INDEX idx_lri_company_sid (company_sid),
    INDEX idx_lri_reward_type (reward_type),
    INDEX idx_lri_status (item_status),
    INDEX idx_lri_avalible (avalible),
    CHECK (required_points > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='點數兌換商品品項檔';

CREATE TABLE IF NOT EXISTS lyt_exchange_order (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '兌換單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    exchange_no             VARCHAR(100)                        NOT NULL COMMENT '兌換單號',
    customer_sid            VARCHAR(32)                         NOT NULL COMMENT '會員序號',
    reward_item_nid         BIGINT UNSIGNED                     NOT NULL COMMENT '兌換商品流水號',
    used_points             INT                                 NOT NULL COMMENT '扣除點數',
    paid_amount             DECIMAL(18,4)                       NOT NULL DEFAULT 0 COMMENT '支付現金金額',
    fulfillment_request_sid VARCHAR(32)                             NULL COMMENT '履約需求序號 (實體商品出貨時使用)',
    issued_customer_coupon_sid VARCHAR(32)                         NULL COMMENT '已發放會員優惠券序號 (兌換優惠券時使用)',
    exchange_status         VARCHAR(30)                         NOT NULL DEFAULT 'COMPLETED' COMMENT 'PENDING待處理;PROCESSING處理中;COMPLETED完成;CANCELLED取消退點',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_leo_reward_item
        FOREIGN KEY (reward_item_nid) REFERENCES lyt_reward_item(nid),
    CONSTRAINT uk_leo_exchange_no UNIQUE (exchange_no),
    INDEX idx_leo_customer_sid (customer_sid),
    INDEX idx_leo_reward_item_nid (reward_item_nid),
    INDEX idx_leo_status (exchange_status),
    INDEX idx_leo_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='點數兌換紀錄單';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
