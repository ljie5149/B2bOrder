namespace B2bOrder.Resources.eCommerce
{
    /// <summary>
    /// PromotionDB V2 Schema
    /// 設計目標：
    /// 1. 支援購物平台促銷活動與未來 B2B / ERP 專案優惠
    /// 2. PricingDB 管理基礎價格、會員價、客戶價與協議價；PromotionDB 管理額外優惠
    /// 3. 支援滿額、滿件、折扣、折價、贈品、加價購、組合價、免運與優惠碼
    /// 4. 支援會員等級、Party、商品、分類、品牌、通路、商店、區域與專案範圍
    /// 5. 正式訂單優惠快照由 SalesOrderDB 保存
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class PromotionDatabaseSqlV2
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 促銷活動主檔
-- =========================================================

CREATE TABLE IF NOT EXISTS prm_campaign (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '促銷活動序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    campaign_no             VARCHAR(100)                        NOT NULL COMMENT '活動編號',
    campaign_code           VARCHAR(100)                        NOT NULL COMMENT '活動代碼',
    campaign_name           VARCHAR(300)                        NOT NULL COMMENT '活動名稱',
    campaign_type           VARCHAR(30)                         NOT NULL COMMENT 'AUTO自動;COUPON優惠券;CODE優惠碼;MEMBER會員;FLASH限時;BUNDLE組合;GIFT贈品;ADD_ON加價購;SHIPPING免運;PROJECT專案',
    company_sid             VARCHAR(32)                             NULL COMMENT 'MasterDB公司序號',
    business_unit_sid       VARCHAR(32)                             NULL COMMENT 'MasterDB營運單位序號',
    currency_sid            VARCHAR(32)                         NOT NULL COMMENT 'MasterDB幣別序號',
    priority                INT                                 NOT NULL DEFAULT 100 COMMENT '活動優先順序，數字越小越高',
    stack_mode              VARCHAR(20)                         NOT NULL DEFAULT 'EXCLUSIVE' COMMENT 'EXCLUSIVE互斥;STACK可疊加;BEST最佳優惠;PRIORITY依優先序',
    stop_processing         TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '命中後是否停止後續活動',
    usage_limit_total       INT                                     NULL COMMENT '活動總使用次數上限',
    usage_limit_per_party   INT                                     NULL COMMENT '每Party使用上限',
    usage_limit_per_order   INT                                 NOT NULL DEFAULT 1 COMMENT '每訂單使用上限',
    budget_amount           DECIMAL(20,4)                           NULL COMMENT '活動預算',
    consumed_budget_amount  DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '已使用預算',
    start_date              DATETIME                            NOT NULL COMMENT '活動開始時間',
    end_date                DATETIME                            NOT NULL COMMENT '活動結束時間',
    campaign_status         VARCHAR(30)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;REVIEW待審;APPROVED核准;SCHEDULED排程;ACTIVE進行中;PAUSED暫停;EXPIRED結束;CANCELLED取消',
    workflow_instance_sid   VARCHAR(32)                             NULL COMMENT 'WorkflowDB流程實例序號',
    approved_user_sid       VARCHAR(32)                             NULL COMMENT '核准人員序號',
    approved_date           DATETIME                                NULL COMMENT '核准時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_pc_campaign_no UNIQUE (campaign_no),
    CONSTRAINT uk_pc_campaign_code UNIQUE (campaign_code),
    INDEX idx_pc_campaign_type (campaign_type),
    INDEX idx_pc_company_sid (company_sid),
    INDEX idx_pc_business_unit_sid (business_unit_sid),
    INDEX idx_pc_priority (priority),
    INDEX idx_pc_stack_mode (stack_mode),
    INDEX idx_pc_effective_date (start_date, end_date),
    INDEX idx_pc_status (campaign_status),
    INDEX idx_pc_workflow_instance_sid (workflow_instance_sid),
    INDEX idx_pc_avalible (avalible),
    CHECK (priority >= 0),
    CHECK (usage_limit_total IS NULL OR usage_limit_total > 0),
    CHECK (usage_limit_per_party IS NULL OR usage_limit_per_party > 0),
    CHECK (usage_limit_per_order > 0),
    CHECK (budget_amount IS NULL OR budget_amount >= 0),
    CHECK (consumed_budget_amount >= 0),
    CHECK (end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='促銷活動主檔';

CREATE TABLE IF NOT EXISTS prm_campaign_translation (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '活動多語系序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    campaign_nid            BIGINT UNSIGNED                     NOT NULL COMMENT '活動流水號',
    language_sid            VARCHAR(32)                         NOT NULL COMMENT 'MasterDB語系序號',
    campaign_name           VARCHAR(300)                        NOT NULL COMMENT '活動名稱',
    short_description       VARCHAR(1000)                           NULL COMMENT '活動簡介',
    full_description        LONGTEXT                                NULL COMMENT '完整活動說明',
    terms_text              LONGTEXT                                NULL COMMENT '活動條款',
    banner_text             VARCHAR(500)                            NULL COMMENT 'Banner文案',
    CONSTRAINT fk_pct_campaign
        FOREIGN KEY (campaign_nid) REFERENCES prm_campaign(nid),
    CONSTRAINT uk_pct_campaign_language UNIQUE (campaign_nid, language_sid),
    INDEX idx_pct_campaign_nid (campaign_nid),
    INDEX idx_pct_language_sid (language_sid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='促銷活動多語系內容';

-- =========================================================
-- 02. 促銷規則與條件
-- =========================================================

CREATE TABLE IF NOT EXISTS prm_rule (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '促銷規則序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    campaign_nid            BIGINT UNSIGNED                     NOT NULL COMMENT '活動流水號',
    rule_no                 INT                                 NOT NULL COMMENT '規則序號',
    rule_name               VARCHAR(300)                        NOT NULL COMMENT '規則名稱',
    condition_logic         VARCHAR(10)                         NOT NULL DEFAULT 'AND' COMMENT 'AND全部符合;OR任一符合',
    calculation_basis       VARCHAR(30)                         NOT NULL DEFAULT 'ELIGIBLE_ITEMS' COMMENT 'ORDER訂單;ELIGIBLE_ITEMS符合商品;CHEAPEST最低價商品;HIGHEST最高價商品;SHIPPING運費',
    repeatable_mark         TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否可重複計算',
    maximum_repeat_count    INT                                     NULL COMMENT '最大重複次數',
    rule_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    CONSTRAINT fk_pr_campaign
        FOREIGN KEY (campaign_nid) REFERENCES prm_campaign(nid),
    CONSTRAINT uk_pr_campaign_rule_no UNIQUE (campaign_nid, rule_no),
    INDEX idx_pr_campaign_nid (campaign_nid),
    INDEX idx_pr_condition_logic (condition_logic),
    INDEX idx_pr_calculation_basis (calculation_basis),
    INDEX idx_pr_status (rule_status),
    CHECK (rule_no > 0),
    CHECK (maximum_repeat_count IS NULL OR maximum_repeat_count > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='促銷活動規則';

CREATE TABLE IF NOT EXISTS prm_condition (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '促銷條件序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    rule_nid                BIGINT UNSIGNED                     NOT NULL COMMENT '促銷規則流水號',
    condition_no            INT                                 NOT NULL COMMENT '條件序號',
    condition_type          VARCHAR(40)                         NOT NULL COMMENT 'MIN_AMOUNT滿額;MIN_QTY滿件;ITEM商品;CATEGORY分類;BRAND品牌;PARTY對象;MEMBER_LEVEL會員等級;PRICE_LEVEL價格層級;CHANNEL通路;STORE商店;REGION區域;PAYMENT付款;SHIPPING配送;TIME時間;PROJECT專案;CONTRACT合約;FIRST_ORDER首購',
    operator_code           VARCHAR(20)                         NOT NULL COMMENT 'EQ;NE;GT;GE;LT;LE;IN;NOT_IN;BETWEEN;EXISTS',
    condition_value         JSON                                NOT NULL COMMENT '條件值',
    include_mark            TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '1包含;0排除',
    CONSTRAINT fk_pcond_rule
        FOREIGN KEY (rule_nid) REFERENCES prm_rule(nid),
    CONSTRAINT uk_pcond_rule_condition_no UNIQUE (rule_nid, condition_no),
    INDEX idx_pcond_rule_nid (rule_nid),
    INDEX idx_pcond_condition_type (condition_type),
    INDEX idx_pcond_operator_code (operator_code),
    INDEX idx_pcond_include_mark (include_mark),
    CHECK (condition_no > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='促銷條件';

CREATE TABLE IF NOT EXISTS prm_reward (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '促銷回饋序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    rule_nid                BIGINT UNSIGNED                     NOT NULL COMMENT '促銷規則流水號',
    reward_no               INT                                 NOT NULL COMMENT '回饋序號',
    reward_type             VARCHAR(30)                         NOT NULL COMMENT 'PERCENT_DISCOUNT百分比折扣;AMOUNT_DISCOUNT固定折價;FIXED_PRICE固定價;GIFT贈品;ADD_ON_PRICE加價購;FREE_SHIPPING免運;POINTS點數;CREDIT購物金',
    reward_value            DECIMAL(20,6)                           NULL COMMENT '回饋值',
    maximum_discount_amount DECIMAL(20,4)                           NULL COMMENT '最高折扣金額',
    minimum_pay_amount      DECIMAL(20,4)                           NULL COMMENT '最低應付金額',
    target_scope            VARCHAR(30)                         NOT NULL DEFAULT 'ELIGIBLE_ITEMS' COMMENT 'ORDER整單;ELIGIBLE_ITEMS符合商品;CHEAPEST最低價;HIGHEST最高價;SPECIFIC_ITEM指定商品;SHIPPING運費',
    target_config           JSON                                    NULL COMMENT '回饋目標設定',
    gift_item_sid           VARCHAR(32)                             NULL COMMENT 'PIM/MIMDB贈品Item序號',
    gift_variant_sid        VARCHAR(32)                             NULL COMMENT 'PIM/MIMDB贈品變體序號',
    gift_qty                DECIMAL(20,6)                           NULL COMMENT '贈品數量',
    add_on_item_sid         VARCHAR(32)                             NULL COMMENT '加價購Item序號',
    add_on_variant_sid      VARCHAR(32)                             NULL COMMENT '加價購變體序號',
    add_on_price            DECIMAL(20,6)                           NULL COMMENT '加價購價格',
    point_type_sid          VARCHAR(32)                             NULL COMMENT 'LoyaltyDB點數類型序號',
    point_amount            DECIMAL(20,4)                           NULL COMMENT '贈送點數',
    reward_status           VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    CONSTRAINT fk_prew_rule
        FOREIGN KEY (rule_nid) REFERENCES prm_rule(nid),
    CONSTRAINT uk_prew_rule_reward_no UNIQUE (rule_nid, reward_no),
    INDEX idx_prew_rule_nid (rule_nid),
    INDEX idx_prew_reward_type (reward_type),
    INDEX idx_prew_target_scope (target_scope),
    INDEX idx_prew_gift_item_sid (gift_item_sid),
    INDEX idx_prew_add_on_item_sid (add_on_item_sid),
    INDEX idx_prew_status (reward_status),
    CHECK (reward_no > 0),
    CHECK (reward_value IS NULL OR reward_value >= 0),
    CHECK (maximum_discount_amount IS NULL OR maximum_discount_amount >= 0),
    CHECK (minimum_pay_amount IS NULL OR minimum_pay_amount >= 0),
    CHECK (gift_qty IS NULL OR gift_qty > 0),
    CHECK (add_on_price IS NULL OR add_on_price >= 0),
    CHECK (point_amount IS NULL OR point_amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='促銷折扣、贈品與回饋';

-- =========================================================
-- 03. 活動適用與排除範圍
-- =========================================================

CREATE TABLE IF NOT EXISTS prm_campaign_scope (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '活動範圍序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    campaign_nid            BIGINT UNSIGNED                     NOT NULL COMMENT '活動流水號',
    scope_type              VARCHAR(30)                         NOT NULL COMMENT 'ITEM商品;VARIANT變體;CATEGORY分類;BRAND品牌;PARTY對象;PARTY_CATEGORY對象分類;MEMBER_LEVEL會員等級;PRICE_LEVEL價格層級;CHANNEL通路;STORE商店;REGION區域;PROJECT專案;CONTRACT合約;ALL全部',
    scope_sid               VARCHAR(32)                             NULL COMMENT '範圍序號，ALL可為NULL',
    include_mark            TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '1適用;0排除',
    priority                INT                                 NOT NULL DEFAULT 0 COMMENT '範圍優先順序',
    CONSTRAINT fk_pcs_campaign
        FOREIGN KEY (campaign_nid) REFERENCES prm_campaign(nid),
    CONSTRAINT uk_pcs_campaign_scope UNIQUE (campaign_nid, scope_type, scope_sid, include_mark),
    INDEX idx_pcs_campaign_nid (campaign_nid),
    INDEX idx_pcs_scope (scope_type, scope_sid),
    INDEX idx_pcs_include_mark (include_mark),
    INDEX idx_pcs_priority (priority),
    CHECK (priority >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='促銷活動適用與排除範圍';

CREATE TABLE IF NOT EXISTS prm_time_schedule (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '活動時段序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    campaign_nid            BIGINT UNSIGNED                     NOT NULL COMMENT '活動流水號',
    schedule_type           VARCHAR(20)                         NOT NULL COMMENT 'DATE_RANGE日期;WEEKDAY星期;DAILY_TIME每日時段;SPECIFIC_DATE指定日期',
    weekday_mask            VARCHAR(20)                             NULL COMMENT '星期遮罩，例如MON,TUE',
    specific_date           DATE                                    NULL COMMENT '指定日期',
    start_time              TIME                                    NULL COMMENT '每日開始時間',
    end_time                TIME                                    NULL COMMENT '每日結束時間',
    timezone_code           VARCHAR(100)                        NOT NULL DEFAULT 'Asia/Taipei' COMMENT '時區',
    schedule_status         VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    CONSTRAINT fk_pts_campaign
        FOREIGN KEY (campaign_nid) REFERENCES prm_campaign(nid),
    INDEX idx_pts_campaign_nid (campaign_nid),
    INDEX idx_pts_schedule_type (schedule_type),
    INDEX idx_pts_specific_date (specific_date),
    INDEX idx_pts_status (schedule_status),
    CHECK (end_time IS NULL OR start_time IS NULL OR end_time >= start_time)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='活動星期、日期與每日時段';

-- =========================================================
-- 04. 優惠券批次與優惠碼
-- =========================================================

CREATE TABLE IF NOT EXISTS prm_coupon_batch (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '優惠券批次序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    batch_no                VARCHAR(100)                        NOT NULL COMMENT '批次編號',
    batch_name              VARCHAR(300)                        NOT NULL COMMENT '批次名稱',
    campaign_sid            VARCHAR(32)                         NOT NULL COMMENT '促銷活動序號',
    coupon_type             VARCHAR(20)                         NOT NULL COMMENT 'PUBLIC_CODE公開碼;UNIQUE_CODE唯一碼;ACCOUNT_BOUND帳號綁定;AUTO_ISSUE自動發放',
    code_prefix             VARCHAR(30)                             NULL COMMENT '優惠碼前綴',
    total_quantity          INT                                     NULL COMMENT '總發行量',
    issued_quantity         INT                                 NOT NULL DEFAULT 0 COMMENT '已發行量',
    redeemed_quantity       INT                                 NOT NULL DEFAULT 0 COMMENT '已核銷量',
    validity_type           VARCHAR(20)                         NOT NULL DEFAULT 'FIXED_DATE' COMMENT 'FIXED_DATE固定日期;AFTER_RECEIVE領取後天數;AFTER_ISSUE發行後天數',
    valid_days              INT                                     NULL COMMENT '有效天數',
    start_date              DATETIME                                NULL COMMENT '固定生效時間',
    end_date                DATETIME                                NULL COMMENT '固定失效時間',
    batch_status            VARCHAR(20)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;ACTIVE發行中;PAUSED暫停;CLOSED結束;CANCELLED取消',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_pcb_batch_no UNIQUE (batch_no),
    INDEX idx_pcb_campaign_sid (campaign_sid),
    INDEX idx_pcb_coupon_type (coupon_type),
    INDEX idx_pcb_status (batch_status),
    INDEX idx_pcb_avalible (avalible),
    CHECK (total_quantity IS NULL OR total_quantity > 0),
    CHECK (issued_quantity >= 0),
    CHECK (redeemed_quantity >= 0),
    CHECK (total_quantity IS NULL OR issued_quantity <= total_quantity),
    CHECK (redeemed_quantity <= issued_quantity),
    CHECK (valid_days IS NULL OR valid_days > 0),
    CHECK (end_date IS NULL OR start_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='優惠券發行批次';

CREATE TABLE IF NOT EXISTS prm_coupon (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '優惠券序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    coupon_batch_sid        VARCHAR(32)                         NOT NULL COMMENT '優惠券批次序號',
    coupon_code             VARCHAR(150)                        NOT NULL COMMENT '優惠碼',
    coupon_code_hash        VARCHAR(255)                            NULL COMMENT '優惠碼雜湊',
    owner_party_sid         VARCHAR(32)                             NULL COMMENT '綁定Party序號',
    issued_date             DATETIME                                NULL COMMENT '發行時間',
    received_date           DATETIME                                NULL COMMENT '領取時間',
    valid_from              DATETIME                                NULL COMMENT '有效開始時間',
    valid_until             DATETIME                                NULL COMMENT '有效期限',
    usage_limit             INT                                 NOT NULL DEFAULT 1 COMMENT '可使用次數',
    used_count              INT                                 NOT NULL DEFAULT 0 COMMENT '已使用次數',
    coupon_status           VARCHAR(20)                         NOT NULL DEFAULT 'AVAILABLE' COMMENT 'AVAILABLE可領;ISSUED已發行;RECEIVED已領取;RESERVED已保留;USED已用完;EXPIRED過期;REVOKED撤銷',
    reserved_checkout_sid   VARCHAR(32)                             NULL COMMENT 'ShoppingDB保留結帳序號',
    reserved_until          DATETIME                                NULL COMMENT '保留到期時間',
    CONSTRAINT uk_pcou_coupon_code UNIQUE (coupon_code),
    UNIQUE KEY uk_pcou_coupon_code_hash (coupon_code_hash),
    INDEX idx_pcou_coupon_batch_sid (coupon_batch_sid),
    INDEX idx_pcou_owner_party_sid (owner_party_sid),
    INDEX idx_pcou_valid_until (valid_until),
    INDEX idx_pcou_status (coupon_status),
    INDEX idx_pcou_reserved_checkout_sid (reserved_checkout_sid),
    CHECK (usage_limit > 0),
    CHECK (used_count >= 0),
    CHECK (used_count <= usage_limit),
    CHECK (valid_until IS NULL OR valid_from IS NULL OR valid_until >= valid_from)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='優惠券與優惠碼實體';

CREATE TABLE IF NOT EXISTS prm_coupon_distribution (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '優惠券發放序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    coupon_sid              VARCHAR(32)                         NOT NULL COMMENT '優惠券序號',
    party_sid               VARCHAR(32)                         NOT NULL COMMENT 'Party序號',
    distribution_type       VARCHAR(30)                         NOT NULL COMMENT 'CLAIM主動領取;REGISTER註冊;BIRTHDAY生日;ORDER訂單;MEMBER_LEVEL會員等級;CAMPAIGN活動;ADMIN人工;API',
    source_sid              VARCHAR(32)                             NULL COMMENT '來源資料序號',
    distribution_status     VARCHAR(20)                         NOT NULL DEFAULT 'SUCCESS' COMMENT 'PENDING待處理;SUCCESS成功;FAILED失敗;REVOKED撤銷',
    distributed_user_sid    VARCHAR(32)                             NULL COMMENT '操作人員序號',
    error_message           TEXT                                    NULL COMMENT '錯誤訊息',
    CONSTRAINT uk_pcd_coupon_party UNIQUE (coupon_sid, party_sid),
    INDEX idx_pcd_coupon_sid (coupon_sid),
    INDEX idx_pcd_party_sid (party_sid),
    INDEX idx_pcd_distribution_type (distribution_type),
    INDEX idx_pcd_source_sid (source_sid),
    INDEX idx_pcd_status (distribution_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='優惠券發放與領取紀錄';

-- =========================================================
-- 05. 優惠計算請求與結果
-- =========================================================

CREATE TABLE IF NOT EXISTS prm_calculation_request (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '促銷計算請求序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    request_no              VARCHAR(100)                        NOT NULL COMMENT '計算請求編號',
    request_source          VARCHAR(30)                         NOT NULL COMMENT 'CART購物車;CHECKOUT結帳;ORDER訂單;QUOTE報價;API',
    reference_sid           VARCHAR(32)                             NULL COMMENT '來源資料序號',
    party_sid               VARCHAR(32)                             NULL COMMENT 'Party序號',
    channel_sid             VARCHAR(32)                             NULL COMMENT '通路序號',
    store_sid               VARCHAR(32)                             NULL COMMENT '商店序號',
    project_sid             VARCHAR(32)                             NULL COMMENT '專案序號',
    currency_sid            VARCHAR(32)                         NOT NULL COMMENT '幣別序號',
    coupon_codes            JSON                                    NULL COMMENT '輸入優惠碼清單',
    request_data            JSON                                NOT NULL COMMENT '訂單或購物車資料',
    calculation_status      VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待計算;PROCESSING計算中;SUCCESS成功;FAILED失敗',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    correlation_id          VARCHAR(100)                            NULL COMMENT '跨服務關聯識別碼',
    error_message           LONGTEXT                                NULL COMMENT '錯誤訊息',
    CONSTRAINT uk_pcr_request_no UNIQUE (request_no),
    INDEX idx_pcr_request_source (request_source),
    INDEX idx_pcr_reference_sid (reference_sid),
    INDEX idx_pcr_party_sid (party_sid),
    INDEX idx_pcr_channel_sid (channel_sid),
    INDEX idx_pcr_store_sid (store_sid),
    INDEX idx_pcr_project_sid (project_sid),
    INDEX idx_pcr_status (calculation_status),
    INDEX idx_pcr_correlation_id (correlation_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='促銷優惠計算請求';

CREATE TABLE IF NOT EXISTS prm_calculation_result (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '促銷計算結果序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    calculation_request_nid BIGINT UNSIGNED                     NOT NULL COMMENT '促銷計算請求流水號',
    campaign_sid            VARCHAR(32)                         NOT NULL COMMENT '命中活動序號',
    rule_sid                VARCHAR(32)                         NOT NULL COMMENT '命中規則序號',
    coupon_sid              VARCHAR(32)                             NULL COMMENT '使用優惠券序號',
    reward_type             VARCHAR(30)                         NOT NULL COMMENT '回饋類型',
    target_type             VARCHAR(30)                         NOT NULL COMMENT 'ORDER訂單;ITEM明細;SHIPPING運費;GIFT贈品;POINTS點數',
    target_sid              VARCHAR(32)                             NULL COMMENT '目標明細序號',
    discount_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '折扣金額',
    gift_item_sid           VARCHAR(32)                             NULL COMMENT '贈品Item序號',
    gift_variant_sid        VARCHAR(32)                             NULL COMMENT '贈品變體序號',
    gift_qty                DECIMAL(20,6)                           NULL COMMENT '贈品數量',
    point_amount            DECIMAL(20,4)                           NULL COMMENT '贈送點數',
    calculation_trace       JSON                                    NULL COMMENT '規則計算過程',
    result_status           VARCHAR(20)                         NOT NULL DEFAULT 'APPLIED' COMMENT 'APPLIED已套用;REJECTED未套用;REVERSED已撤銷',
    CONSTRAINT fk_pcrs_request
        FOREIGN KEY (calculation_request_nid) REFERENCES prm_calculation_request(nid),
    INDEX idx_pcrs_request_nid (calculation_request_nid),
    INDEX idx_pcrs_campaign_sid (campaign_sid),
    INDEX idx_pcrs_rule_sid (rule_sid),
    INDEX idx_pcrs_coupon_sid (coupon_sid),
    INDEX idx_pcrs_reward_type (reward_type),
    INDEX idx_pcrs_target (target_type, target_sid),
    INDEX idx_pcrs_status (result_status),
    CHECK (discount_amount >= 0),
    CHECK (gift_qty IS NULL OR gift_qty > 0),
    CHECK (point_amount IS NULL OR point_amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='促銷優惠計算結果';

-- =========================================================
-- 06. 優惠預留、核銷與撤銷
-- =========================================================

CREATE TABLE IF NOT EXISTS prm_redemption_reservation (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '優惠預留序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    campaign_sid            VARCHAR(32)                         NOT NULL COMMENT '活動序號',
    coupon_sid              VARCHAR(32)                             NULL COMMENT '優惠券序號',
    party_sid               VARCHAR(32)                             NULL COMMENT 'Party序號',
    checkout_sid            VARCHAR(32)                             NULL COMMENT 'ShoppingDB結帳序號',
    order_sid               VARCHAR(32)                             NULL COMMENT 'SalesOrderDB訂單序號',
    reserved_discount_amount DECIMAL(20,4)                      NOT NULL DEFAULT 0 COMMENT '預留折扣金額',
    reserved_budget_amount  DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '預留活動預算',
    reserved_date           DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '預留時間',
    expiry_date             DATETIME                            NOT NULL COMMENT '預留到期時間',
    reservation_status      VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;COMMITTED已核銷;RELEASED已釋放;EXPIRED過期',
    CONSTRAINT uk_prr_campaign_checkout_coupon UNIQUE (campaign_sid, checkout_sid, coupon_sid),
    INDEX idx_prr_campaign_sid (campaign_sid),
    INDEX idx_prr_coupon_sid (coupon_sid),
    INDEX idx_prr_party_sid (party_sid),
    INDEX idx_prr_checkout_sid (checkout_sid),
    INDEX idx_prr_order_sid (order_sid),
    INDEX idx_prr_expiry_date (expiry_date),
    INDEX idx_prr_status (reservation_status),
    CHECK (reserved_discount_amount >= 0),
    CHECK (reserved_budget_amount >= 0),
    CHECK (expiry_date > reserved_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='結帳期間優惠與預算預留';

CREATE TABLE IF NOT EXISTS prm_redemption (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '優惠核銷序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    redemption_no           VARCHAR(100)                        NOT NULL COMMENT '核銷編號',
    campaign_sid            VARCHAR(32)                         NOT NULL COMMENT '活動序號',
    rule_sid                VARCHAR(32)                             NULL COMMENT '規則序號',
    coupon_sid              VARCHAR(32)                             NULL COMMENT '優惠券序號',
    party_sid               VARCHAR(32)                             NULL COMMENT 'Party序號',
    sales_order_sid         VARCHAR(32)                         NOT NULL COMMENT 'SalesOrderDB訂單序號',
    sales_order_item_sid    VARCHAR(32)                             NULL COMMENT 'SalesOrderDB訂單明細序號',
    redemption_date         DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '核銷時間',
    discount_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '折扣金額',
    budget_consumed_amount  DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '活動預算使用金額',
    currency_sid            VARCHAR(32)                         NOT NULL COMMENT '幣別序號',
    redemption_status       VARCHAR(20)                         NOT NULL DEFAULT 'COMMITTED' COMMENT 'COMMITTED已核銷;PARTIAL_REVERSED部分撤銷;REVERSED已撤銷',
    calculation_result_sid  VARCHAR(32)                             NULL COMMENT '促銷計算結果序號',
    correlation_id          VARCHAR(100)                            NULL COMMENT '跨服務關聯識別碼',
    CONSTRAINT uk_pred_redemption_no UNIQUE (redemption_no),
    INDEX idx_pred_campaign_sid (campaign_sid),
    INDEX idx_pred_rule_sid (rule_sid),
    INDEX idx_pred_coupon_sid (coupon_sid),
    INDEX idx_pred_party_sid (party_sid),
    INDEX idx_pred_sales_order_sid (sales_order_sid),
    INDEX idx_pred_sales_order_item_sid (sales_order_item_sid),
    INDEX idx_pred_redemption_date (redemption_date),
    INDEX idx_pred_status (redemption_status),
    INDEX idx_pred_correlation_id (correlation_id),
    CHECK (discount_amount >= 0),
    CHECK (budget_consumed_amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='優惠正式核銷';

CREATE TABLE IF NOT EXISTS prm_redemption_reversal (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '優惠撤銷序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    redemption_sid          VARCHAR(32)                         NOT NULL COMMENT '優惠核銷序號',
    reversal_type           VARCHAR(30)                         NOT NULL COMMENT 'ORDER_CANCEL訂單取消;ITEM_CANCEL明細取消;RETURN退貨;REFUND退款;MANUAL人工',
    reference_sid           VARCHAR(32)                             NULL COMMENT '來源資料序號',
    reversal_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '撤銷折扣金額',
    budget_return_amount    DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '歸還活動預算',
    coupon_restore_mark     TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否恢復優惠券',
    reversal_reason         TEXT                                    NULL COMMENT '撤銷原因',
    operator_user_sid       VARCHAR(32)                             NULL COMMENT '操作人員序號',
    reversal_status         VARCHAR(20)                         NOT NULL DEFAULT 'SUCCESS' COMMENT 'PENDING待處理;SUCCESS成功;FAILED失敗',
    correlation_id          VARCHAR(100)                            NULL COMMENT '跨服務關聯識別碼',
    INDEX idx_prrev_redemption_sid (redemption_sid),
    INDEX idx_prrev_reversal_type (reversal_type),
    INDEX idx_prrev_reference_sid (reference_sid),
    INDEX idx_prrev_status (reversal_status),
    INDEX idx_prrev_correlation_id (correlation_id),
    CHECK (reversal_amount >= 0),
    CHECK (budget_return_amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='訂單取消、退貨與退款優惠撤銷';

-- =========================================================
-- 07. 贈品與加價購庫存
-- =========================================================

CREATE TABLE IF NOT EXISTS prm_reward_inventory (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '活動贈品庫存序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    campaign_sid            VARCHAR(32)                         NOT NULL COMMENT '活動序號',
    reward_sid              VARCHAR(32)                         NOT NULL COMMENT '回饋規則序號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT '贈品或加價購Item序號',
    variant_sid             VARCHAR(32)                             NULL COMMENT '變體序號',
    warehouse_sid           VARCHAR(32)                             NULL COMMENT 'InventoryDB或MasterDB倉庫序號',
    quota_qty               DECIMAL(20,6)                           NULL COMMENT '活動配額',
    reserved_qty            DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '已預留數量',
    redeemed_qty            DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '已核銷數量',
    remaining_qty           DECIMAL(20,6)                           NULL COMMENT '剩餘配額',
    inventory_status        VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;EXHAUSTED用罄;PAUSED暫停',
    CONSTRAINT uk_pri_campaign_reward_item UNIQUE (campaign_sid, reward_sid, item_sid, variant_sid, warehouse_sid),
    INDEX idx_pri_campaign_sid (campaign_sid),
    INDEX idx_pri_reward_sid (reward_sid),
    INDEX idx_pri_item_sid (item_sid),
    INDEX idx_pri_variant_sid (variant_sid),
    INDEX idx_pri_warehouse_sid (warehouse_sid),
    INDEX idx_pri_status (inventory_status),
    CHECK (quota_qty IS NULL OR quota_qty >= 0),
    CHECK (reserved_qty >= 0),
    CHECK (redeemed_qty >= 0),
    CHECK (remaining_qty IS NULL OR remaining_qty >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='活動贈品與加價購配額';

-- =========================================================
-- 08. 活動預算與成本
-- =========================================================

CREATE TABLE IF NOT EXISTS prm_budget_ledger (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '活動預算流水序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    campaign_sid            VARCHAR(32)                         NOT NULL COMMENT '活動序號',
    transaction_type        VARCHAR(20)                         NOT NULL COMMENT 'ALLOCATE編列;RESERVE預留;CONSUME使用;RELEASE釋放;RETURN歸還;ADJUST調整',
    reference_type          VARCHAR(30)                             NULL COMMENT 'CHECKOUT;ORDER;REDEMPTION;REVERSAL;MANUAL',
    reference_sid           VARCHAR(32)                             NULL COMMENT '來源資料序號',
    amount                  DECIMAL(20,4)                       NOT NULL COMMENT '金額',
    balance_after           DECIMAL(20,4)                       NOT NULL COMMENT '交易後餘額',
    currency_sid            VARCHAR(32)                         NOT NULL COMMENT '幣別序號',
    operator_user_sid       VARCHAR(32)                             NULL COMMENT '操作人員序號',
    correlation_id          VARCHAR(100)                            NULL COMMENT '跨服務關聯識別碼',
    INDEX idx_pbl_campaign_sid (campaign_sid),
    INDEX idx_pbl_transaction_type (transaction_type),
    INDEX idx_pbl_reference (reference_type, reference_sid),
    INDEX idx_pbl_create_date (create_date),
    INDEX idx_pbl_correlation_id (correlation_id),
    CHECK (amount >= 0),
    CHECK (balance_after >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='促銷活動預算流水帳';

-- =========================================================
-- 09. 促銷模板與複製
-- =========================================================

CREATE TABLE IF NOT EXISTS prm_template (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '促銷模板序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    template_code           VARCHAR(100)                        NOT NULL COMMENT '模板代碼',
    template_name           VARCHAR(300)                        NOT NULL COMMENT '模板名稱',
    campaign_type           VARCHAR(30)                         NOT NULL COMMENT '活動類型',
    template_config         JSON                                NOT NULL COMMENT '活動、條件與回饋模板設定',
    template_status         VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_pt_template_code UNIQUE (template_code),
    INDEX idx_pt_campaign_type (campaign_type),
    INDEX idx_pt_status (template_status),
    INDEX idx_pt_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='促銷活動模板';

-- =========================================================
-- 10. 活動版本、狀態與事件
-- =========================================================

CREATE TABLE IF NOT EXISTS prm_campaign_version (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '活動版本序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    campaign_sid            VARCHAR(32)                         NOT NULL COMMENT '活動序號',
    version_no              BIGINT UNSIGNED                     NOT NULL COMMENT '版本號',
    snapshot_data           JSON                                NOT NULL COMMENT '活動完整快照',
    change_type             VARCHAR(30)                         NOT NULL COMMENT 'CREATE新增;UPDATE修改;APPROVE核准;ACTIVATE啟用;PAUSE暫停;EXPIRE結束;ROLLBACK回滾',
    change_summary          VARCHAR(1000)                           NULL COMMENT '變更摘要',
    create_user_sid         VARCHAR(32)                             NULL COMMENT '建立人員序號',
    version_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;SUPERSEDED已取代;ARCHIVED封存;ROLLED_BACK已回滾',
    CONSTRAINT uk_pcv_campaign_version UNIQUE (campaign_sid, version_no),
    INDEX idx_pcv_campaign_sid (campaign_sid),
    INDEX idx_pcv_change_type (change_type),
    INDEX idx_pcv_status (version_status),
    INDEX idx_pcv_create_date (create_date),
    CHECK (version_no > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='促銷活動版本歷程';

CREATE TABLE IF NOT EXISTS prm_status_history (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '促銷狀態歷程序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '異動時間',
    entity_type             VARCHAR(30)                         NOT NULL COMMENT 'CAMPAIGN;COUPON_BATCH;COUPON;CALCULATION;RESERVATION;REDEMPTION;REWARD_INVENTORY',
    entity_sid              VARCHAR(32)                         NOT NULL COMMENT '實體序號',
    old_status              VARCHAR(30)                             NULL COMMENT '原狀態',
    new_status              VARCHAR(30)                         NOT NULL COMMENT '新狀態',
    event_code              VARCHAR(100)                            NULL COMMENT '觸發事件代碼',
    operator_user_sid       VARCHAR(32)                             NULL COMMENT '操作人員序號',
    reason                  TEXT                                    NULL COMMENT '原因說明',
    correlation_id          VARCHAR(100)                            NULL COMMENT '跨服務關聯識別碼',
    INDEX idx_psh_entity (entity_type, entity_sid),
    INDEX idx_psh_new_status (new_status),
    INDEX idx_psh_event_code (event_code),
    INDEX idx_psh_operator_user_sid (operator_user_sid),
    INDEX idx_psh_correlation_id (correlation_id),
    INDEX idx_psh_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='促銷狀態歷程';

CREATE TABLE IF NOT EXISTS prm_event (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '促銷事件序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    entity_type             VARCHAR(30)                         NOT NULL COMMENT 'CAMPAIGN;COUPON;CALCULATION;RESERVATION;REDEMPTION;BUDGET',
    entity_sid              VARCHAR(32)                         NOT NULL COMMENT '實體序號',
    event_code              VARCHAR(120)                        NOT NULL COMMENT '事件代碼',
    event_version           INT                                 NOT NULL DEFAULT 1 COMMENT '事件版本',
    event_data              JSON                                    NULL COMMENT '事件內容',
    source_event_id         VARCHAR(100)                            NULL COMMENT '來源事件ID',
    correlation_id          VARCHAR(100)                            NULL COMMENT '關聯識別碼',
    causation_id            VARCHAR(100)                            NULL COMMENT '因果事件ID',
    outbox_event_sid        VARCHAR(32)                             NULL COMMENT 'IntegrationDB Outbox事件序號',
    process_status          VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;SUCCESS成功;FAILED失敗;IGNORED忽略',
    processed_date          DATETIME                                NULL COMMENT '處理時間',
    error_message           TEXT                                    NULL COMMENT '錯誤訊息',
    UNIQUE KEY uk_pe_source_event_id (source_event_id),
    INDEX idx_pe_entity (entity_type, entity_sid),
    INDEX idx_pe_event_code (event_code),
    INDEX idx_pe_correlation_id (correlation_id),
    INDEX idx_pe_outbox_event_sid (outbox_event_sid),
    INDEX idx_pe_status (process_status),
    INDEX idx_pe_create_date (create_date),
    CHECK (event_version > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='促銷領域事件';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
