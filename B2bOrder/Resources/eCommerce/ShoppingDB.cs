namespace B2bOrder.Resources.eCommerce
{
    /// <summary>
    /// ShoppingDB V2 Schema
    /// 設計目標：
    /// 1. 僅管理購物平台前台專屬功能，不重複保存正式銷售訂單
    /// 2. 購物車與結帳成功後，轉交 SalesOrderDB 建立正式訂單
    /// 3. 商品與變體由 PIM/MIMDB 提供；價格由 PricingDB 提供
    /// 4. 會員與Party由 PartyDB 提供；登入帳號由 IdentityDB 提供
    /// 5. 庫存可用量由 InventoryDB 提供；付款由 PaymentDB 提供
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class ShoppingDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 前台通路與商店設定
-- =========================================================

CREATE TABLE IF NOT EXISTS shp_channel (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '通路序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    channel_code            VARCHAR(100)                        NOT NULL COMMENT '通路代碼',
    channel_name            VARCHAR(200)                        NOT NULL COMMENT '通路名稱',
    channel_type            VARCHAR(30)                         NOT NULL COMMENT 'WEB網站;APP;MINI_APP小程式;MARKETPLACE外部平台;POS門市;B2B_PORTAL企業入口',
    company_sid             VARCHAR(32)                             NULL COMMENT 'MasterDB公司序號',
    business_unit_sid       VARCHAR(32)                             NULL COMMENT 'MasterDB營運單位序號',
    default_currency_sid    VARCHAR(32)                         NOT NULL COMMENT 'MasterDB預設幣別序號',
    default_language_sid    VARCHAR(32)                         NOT NULL COMMENT 'MasterDB預設語系序號',
    default_price_list_sid  VARCHAR(32)                             NULL COMMENT 'PricingDB預設價格清單序號',
    anonymous_checkout     TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否允許訪客結帳',
    registration_required   TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否強制註冊',
    channel_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;MAINTENANCE維護;INACTIVE停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_sc_channel_code UNIQUE (channel_code),
    INDEX idx_sc_channel_type (channel_type),
    INDEX idx_sc_company_sid (company_sid),
    INDEX idx_sc_business_unit_sid (business_unit_sid),
    INDEX idx_sc_default_price_list_sid (default_price_list_sid),
    INDEX idx_sc_status (channel_status),
    INDEX idx_sc_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='購物平台銷售通路';

CREATE TABLE IF NOT EXISTS shp_storefront (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '前台商店序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    storefront_code         VARCHAR(100)                        NOT NULL COMMENT '商店代碼',
    storefront_name         VARCHAR(200)                        NOT NULL COMMENT '商店名稱',
    channel_sid             VARCHAR(32)                         NOT NULL COMMENT '通路序號',
    owner_party_sid         VARCHAR(32)                             NULL COMMENT 'PartyDB商店擁有者序號',
    company_sid             VARCHAR(32)                             NULL COMMENT 'MasterDB公司序號',
    business_unit_sid       VARCHAR(32)                             NULL COMMENT 'MasterDB營運單位序號',
    warehouse_sid           VARCHAR(32)                             NULL COMMENT 'MasterDB預設履約倉序號',
    price_list_sid          VARCHAR(32)                             NULL COMMENT 'PricingDB商店價格清單序號',
    domain_name             VARCHAR(300)                            NULL COMMENT '自訂網域',
    logo_file_sid           VARCHAR(32)                             NULL COMMENT 'FileDB Logo檔案序號',
    theme_config            JSON                                    NULL COMMENT '前台主題設定',
    seo_config              JSON                                    NULL COMMENT 'SEO設定',
    storefront_status       VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'DRAFT草稿;ACTIVE啟用;MAINTENANCE維護;INACTIVE停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ss_storefront_code UNIQUE (storefront_code),
    UNIQUE KEY uk_ss_domain_name (domain_name),
    INDEX idx_ss_channel_sid (channel_sid),
    INDEX idx_ss_owner_party_sid (owner_party_sid),
    INDEX idx_ss_company_sid (company_sid),
    INDEX idx_ss_warehouse_sid (warehouse_sid),
    INDEX idx_ss_price_list_sid (price_list_sid),
    INDEX idx_ss_status (storefront_status),
    INDEX idx_ss_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='購物平台前台商店';

-- =========================================================
-- 02. 前台商品上架
-- =========================================================

CREATE TABLE IF NOT EXISTS shp_listing (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '商品上架序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    listing_no              VARCHAR(100)                        NOT NULL COMMENT '上架編號',
    storefront_sid          VARCHAR(32)                         NOT NULL COMMENT '前台商店序號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'PIM/MIMDB Item序號',
    default_variant_sid     VARCHAR(32)                             NULL COMMENT 'PIM/MIMDB預設變體序號',
    listing_title           VARCHAR(500)                        NOT NULL COMMENT '前台商品標題',
    subtitle                VARCHAR(500)                            NULL COMMENT '副標題',
    slug                    VARCHAR(500)                        NOT NULL COMMENT '商品網址Slug',
    category_sid            VARCHAR(32)                             NULL COMMENT '前台商品分類序號',
    brand_sid               VARCHAR(32)                             NULL COMMENT 'MasterDB品牌序號',
    price_list_sid          VARCHAR(32)                             NULL COMMENT 'PricingDB價格清單序號',
    sales_mode              VARCHAR(30)                         NOT NULL DEFAULT 'NORMAL' COMMENT 'NORMAL一般;PREORDER預購;QUOTE詢價;SUBSCRIPTION訂閱;DIGITAL數位;SERVICE服務',
    visible_without_login   TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '未登入是否可見',
    purchasable_without_login TINYINT(1)                        NOT NULL DEFAULT 1 COMMENT '未登入是否可購買',
    minimum_order_qty       DECIMAL(20,6)                       NOT NULL DEFAULT 1 COMMENT '最低購買數量',
    maximum_order_qty       DECIMAL(20,6)                           NULL COMMENT '最高購買數量',
    order_multiple_qty      DECIMAL(20,6)                       NOT NULL DEFAULT 1 COMMENT '購買倍數',
    preorder_start_date     DATETIME                                NULL COMMENT '預購開始時間',
    preorder_end_date       DATETIME                                NULL COMMENT '預購結束時間',
    publish_start_date      DATETIME                                NULL COMMENT '上架開始時間',
    publish_end_date        DATETIME                                NULL COMMENT '上架結束時間',
    listing_status          VARCHAR(30)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;REVIEW待審;PUBLISHED上架;HIDDEN隱藏;SOLD_OUT售完;SUSPENDED暫停;ARCHIVED封存',
    approved_user_sid       VARCHAR(32)                             NULL COMMENT '核准人員序號',
    workflow_instance_sid   VARCHAR(32)                             NULL COMMENT 'WorkflowDB流程實例序號',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_sl_listing_no UNIQUE (listing_no),
    CONSTRAINT uk_sl_storefront_slug UNIQUE (storefront_sid, slug),
    CONSTRAINT uk_sl_storefront_item UNIQUE (storefront_sid, item_sid),
    INDEX idx_sl_storefront_sid (storefront_sid),
    INDEX idx_sl_item_sid (item_sid),
    INDEX idx_sl_default_variant_sid (default_variant_sid),
    INDEX idx_sl_category_sid (category_sid),
    INDEX idx_sl_brand_sid (brand_sid),
    INDEX idx_sl_price_list_sid (price_list_sid),
    INDEX idx_sl_sales_mode (sales_mode),
    INDEX idx_sl_publish_date (publish_start_date, publish_end_date),
    INDEX idx_sl_status (listing_status),
    INDEX idx_sl_workflow_instance_sid (workflow_instance_sid),
    INDEX idx_sl_avalible (avalible),
    FULLTEXT INDEX ftx_sl_title (listing_title, subtitle),
    CHECK (minimum_order_qty > 0),
    CHECK (maximum_order_qty IS NULL OR maximum_order_qty >= minimum_order_qty),
    CHECK (order_multiple_qty > 0),
    CHECK (preorder_end_date IS NULL OR preorder_start_date IS NULL OR preorder_end_date >= preorder_start_date),
    CHECK (publish_end_date IS NULL OR publish_start_date IS NULL OR publish_end_date >= publish_start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='前台商品上架資料';

CREATE TABLE IF NOT EXISTS shp_listing_variant (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '上架變體序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    listing_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '商品上架流水號',
    variant_sid             VARCHAR(32)                         NOT NULL COMMENT 'PIM/MIMDB變體序號',
    display_name            VARCHAR(500)                            NULL COMMENT '前台變體名稱',
    price_list_sid          VARCHAR(32)                             NULL COMMENT 'PricingDB價格清單序號',
    warehouse_sid           VARCHAR(32)                             NULL COMMENT '預設履約倉序號',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    visible_mark            TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否顯示',
    purchasable_mark        TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否可購買',
    variant_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;HIDDEN隱藏;SOLD_OUT售完;INACTIVE停用',
    CONSTRAINT fk_slv_listing
        FOREIGN KEY (listing_nid) REFERENCES shp_listing(nid),
    CONSTRAINT uk_slv_listing_variant UNIQUE (listing_nid, variant_sid),
    INDEX idx_slv_listing_nid (listing_nid),
    INDEX idx_slv_variant_sid (variant_sid),
    INDEX idx_slv_price_list_sid (price_list_sid),
    INDEX idx_slv_warehouse_sid (warehouse_sid),
    INDEX idx_slv_visible_mark (visible_mark),
    INDEX idx_slv_status (variant_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='前台商品變體上架';

-- =========================================================
-- 03. 購物車
-- =========================================================

CREATE TABLE IF NOT EXISTS shp_cart (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '購物車序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    cart_no                 VARCHAR(100)                        NOT NULL COMMENT '購物車編號',
    storefront_sid          VARCHAR(32)                         NOT NULL COMMENT '前台商店序號',
    channel_sid             VARCHAR(32)                         NOT NULL COMMENT '通路序號',
    member_party_sid        VARCHAR(32)                             NULL COMMENT 'PartyDB會員Party序號',
    user_sid                VARCHAR(32)                             NULL COMMENT 'IdentityDB帳號序號',
    guest_token_hash        VARCHAR(255)                            NULL COMMENT '訪客Token雜湊',
    session_id              VARCHAR(200)                            NULL COMMENT '前台Session ID',
    currency_sid            VARCHAR(32)                         NOT NULL COMMENT '幣別序號',
    price_level_sid         VARCHAR(32)                             NULL COMMENT 'PricingDB價格層級序號',
    item_count              INT                                 NOT NULL DEFAULT 0 COMMENT '商品項目數',
    total_qty               DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '商品總數量',
    subtotal_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '購物車小計',
    discount_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '折扣金額',
    promotion_amount        DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '活動優惠金額',
    estimated_tax_amount    DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '預估稅額',
    estimated_total_amount  DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '預估總額',
    price_expiry_date       DATETIME                                NULL COMMENT '價格有效期限',
    cart_expiry_date        DATETIME                                NULL COMMENT '購物車到期時間',
    cart_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE使用中;MERGED已合併;CHECKED_OUT已結帳;ABANDONED已放棄;EXPIRED過期',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    CONSTRAINT uk_scart_cart_no UNIQUE (cart_no),
    INDEX idx_scart_storefront_sid (storefront_sid),
    INDEX idx_scart_channel_sid (channel_sid),
    INDEX idx_scart_member_party_sid (member_party_sid),
    INDEX idx_scart_user_sid (user_sid),
    INDEX idx_scart_guest_token_hash (guest_token_hash),
    INDEX idx_scart_session_id (session_id),
    INDEX idx_scart_cart_expiry_date (cart_expiry_date),
    INDEX idx_scart_status (cart_status),
    CHECK (item_count >= 0),
    CHECK (total_qty >= 0),
    CHECK (subtotal_amount >= 0),
    CHECK (discount_amount >= 0),
    CHECK (promotion_amount >= 0),
    CHECK (estimated_tax_amount >= 0),
    CHECK (estimated_total_amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='購物車';

CREATE TABLE IF NOT EXISTS shp_cart_item (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '購物車明細序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    cart_nid                BIGINT UNSIGNED                     NOT NULL COMMENT '購物車流水號',
    line_no                 INT                                 NOT NULL COMMENT '明細行號',
    listing_sid             VARCHAR(32)                         NOT NULL COMMENT 'ShoppingDB上架序號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'PIM/MIMDB Item序號',
    variant_sid             VARCHAR(32)                             NULL COMMENT 'PIM/MIMDB變體序號',
    quantity                DECIMAL(20,6)                       NOT NULL COMMENT '數量',
    unit_sid                VARCHAR(32)                         NOT NULL COMMENT '單位序號',
    list_price              DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '牌價快照',
    unit_price              DECIMAL(20,6)                       NOT NULL DEFAULT 0 COMMENT '單價快照',
    discount_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '折扣金額',
    promotion_amount        DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '優惠金額',
    tax_amount              DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '稅額',
    line_amount             DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '明細金額',
    pricing_result_sid      VARCHAR(32)                             NULL COMMENT 'PricingDB計價結果序號',
    inventory_available_qty DECIMAL(20,6)                           NULL COMMENT '加入或更新時可用庫存快照',
    reservation_sid         VARCHAR(32)                             NULL COMMENT 'InventoryDB暫時預留序號',
    item_snapshot           JSON                                    NULL COMMENT '商品名稱與規格快照',
    price_snapshot          JSON                                    NULL COMMENT '價格計算快照',
    selected_options        JSON                                    NULL COMMENT '加購、客製化或服務選項',
    item_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;INVALID商品失效;OUT_OF_STOCK缺貨;PRICE_CHANGED價格變更;REMOVED移除',
    CONSTRAINT fk_sci_cart
        FOREIGN KEY (cart_nid) REFERENCES shp_cart(nid),
    CONSTRAINT uk_sci_cart_line UNIQUE (cart_nid, line_no),
    CONSTRAINT uk_sci_cart_variant UNIQUE (cart_nid, listing_sid, variant_sid),
    INDEX idx_sci_cart_nid (cart_nid),
    INDEX idx_sci_listing_sid (listing_sid),
    INDEX idx_sci_item_sid (item_sid),
    INDEX idx_sci_variant_sid (variant_sid),
    INDEX idx_sci_pricing_result_sid (pricing_result_sid),
    INDEX idx_sci_reservation_sid (reservation_sid),
    INDEX idx_sci_status (item_status),
    CHECK (line_no > 0),
    CHECK (quantity > 0),
    CHECK (list_price >= 0),
    CHECK (unit_price >= 0),
    CHECK (discount_amount >= 0),
    CHECK (promotion_amount >= 0),
    CHECK (tax_amount >= 0),
    CHECK (line_amount >= 0),
    CHECK (inventory_available_qty IS NULL OR inventory_available_qty >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='購物車明細';

-- =========================================================
-- 04. 結帳工作階段
-- =========================================================

CREATE TABLE IF NOT EXISTS shp_checkout_session (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '結帳工作階段序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    checkout_no             VARCHAR(100)                        NOT NULL COMMENT '結帳編號',
    cart_sid                VARCHAR(32)                         NOT NULL COMMENT '購物車序號',
    storefront_sid          VARCHAR(32)                         NOT NULL COMMENT '前台商店序號',
    member_party_sid        VARCHAR(32)                             NULL COMMENT '會員Party序號',
    guest_email             VARCHAR(200)                            NULL COMMENT '訪客Email',
    guest_mobile            VARCHAR(50)                             NULL COMMENT '訪客手機',
    currency_sid            VARCHAR(32)                         NOT NULL COMMENT '幣別序號',
    pricing_request_sid     VARCHAR(32)                             NULL COMMENT 'PricingDB計價請求序號',
    shipping_method_sid     VARCHAR(32)                             NULL COMMENT 'FulfillmentDB或MasterDB配送方式序號',
    payment_method_sid      VARCHAR(32)                             NULL COMMENT 'MasterDB付款方式序號',
    billing_address_sid     VARCHAR(32)                             NULL COMMENT 'PartyDB地址序號',
    shipping_address_sid    VARCHAR(32)                             NULL COMMENT 'PartyDB地址序號',
    subtotal_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '未稅小計',
    discount_amount         DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '折扣金額',
    promotion_amount        DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '活動優惠金額',
    tax_amount              DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '稅額',
    freight_amount          DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '運費',
    service_amount          DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '服務費',
    total_amount            DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '結帳總額',
    inventory_validated     TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否已檢核庫存',
    price_validated         TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否已檢核價格',
    address_validated       TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否已檢核地址',
    payment_request_sid     VARCHAR(32)                             NULL COMMENT 'PaymentDB付款請求序號',
    sales_order_sid         VARCHAR(32)                             NULL COMMENT 'SalesOrderDB正式訂單序號',
    checkout_expiry_date    DATETIME                            NOT NULL COMMENT '結帳到期時間',
    checkout_status         VARCHAR(30)                         NOT NULL DEFAULT 'CREATED' COMMENT 'CREATED已建立;VALIDATING驗證中;READY可付款;PAYMENT_PENDING待付款;PAID已付款;ORDER_CREATED已建單;FAILED失敗;EXPIRED過期;CANCELLED取消',
    idempotency_key         VARCHAR(200)                        NOT NULL COMMENT '結帳冪等Key',
    correlation_id          VARCHAR(100)                            NULL COMMENT '跨服務關聯識別碼',
    error_message           LONGTEXT                                NULL COMMENT '錯誤訊息',
    CONSTRAINT uk_scs_checkout_no UNIQUE (checkout_no),
    CONSTRAINT uk_scs_idempotency_key UNIQUE (idempotency_key),
    INDEX idx_scs_cart_sid (cart_sid),
    INDEX idx_scs_storefront_sid (storefront_sid),
    INDEX idx_scs_member_party_sid (member_party_sid),
    INDEX idx_scs_payment_request_sid (payment_request_sid),
    INDEX idx_scs_sales_order_sid (sales_order_sid),
    INDEX idx_scs_checkout_expiry_date (checkout_expiry_date),
    INDEX idx_scs_status (checkout_status),
    INDEX idx_scs_correlation_id (correlation_id),
    CHECK (subtotal_amount >= 0),
    CHECK (discount_amount >= 0),
    CHECK (promotion_amount >= 0),
    CHECK (tax_amount >= 0),
    CHECK (freight_amount >= 0),
    CHECK (service_amount >= 0),
    CHECK (total_amount >= 0),
    CHECK (checkout_expiry_date > create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='購物平台結帳工作階段';

CREATE TABLE IF NOT EXISTS shp_checkout_address (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '結帳地址序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    checkout_session_nid    BIGINT UNSIGNED                     NOT NULL COMMENT '結帳工作階段流水號',
    address_type            VARCHAR(20)                         NOT NULL COMMENT 'BILLING帳單;SHIPPING配送',
    source_address_sid      VARCHAR(32)                             NULL COMMENT 'PartyDB來源地址序號',
    recipient_name          VARCHAR(200)                        NOT NULL COMMENT '收件人',
    recipient_phone         VARCHAR(50)                             NULL COMMENT '收件電話',
    country_sid             VARCHAR(32)                         NOT NULL COMMENT '國家序號',
    region_level1_sid       VARCHAR(32)                             NULL COMMENT '第一層行政區序號',
    region_level2_sid       VARCHAR(32)                             NULL COMMENT '第二層行政區序號',
    region_level3_sid       VARCHAR(32)                             NULL COMMENT '第三層行政區序號',
    postal_code             VARCHAR(20)                             NULL COMMENT '郵遞區號',
    address_line1           VARCHAR(500)                        NOT NULL COMMENT '主要地址',
    address_line2           VARCHAR(500)                            NULL COMMENT '補充地址',
    delivery_note           VARCHAR(1000)                           NULL COMMENT '配送備註',
    CONSTRAINT fk_sca_checkout
        FOREIGN KEY (checkout_session_nid) REFERENCES shp_checkout_session(nid),
    CONSTRAINT uk_sca_checkout_type UNIQUE (checkout_session_nid, address_type),
    INDEX idx_sca_checkout_session_nid (checkout_session_nid),
    INDEX idx_sca_source_address_sid (source_address_sid),
    INDEX idx_sca_country_sid (country_sid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='結帳地址快照';

-- =========================================================
-- 05. 收藏與購物清單
-- =========================================================

CREATE TABLE IF NOT EXISTS shp_wishlist (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '收藏清單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    wishlist_no             VARCHAR(100)                        NOT NULL COMMENT '收藏清單編號',
    member_party_sid        VARCHAR(32)                         NOT NULL COMMENT '會員Party序號',
    storefront_sid          VARCHAR(32)                         NOT NULL COMMENT '前台商店序號',
    wishlist_name           VARCHAR(200)                        NOT NULL DEFAULT '我的收藏' COMMENT '清單名稱',
    public_mark             TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否公開',
    wishlist_status         VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE使用中;ARCHIVED封存;DELETED刪除',
    CONSTRAINT uk_sw_wishlist_no UNIQUE (wishlist_no),
    INDEX idx_sw_member_party_sid (member_party_sid),
    INDEX idx_sw_storefront_sid (storefront_sid),
    INDEX idx_sw_public_mark (public_mark),
    INDEX idx_sw_status (wishlist_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='會員收藏與購物清單';

CREATE TABLE IF NOT EXISTS shp_wishlist_item (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '收藏明細序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    wishlist_nid            BIGINT UNSIGNED                     NOT NULL COMMENT '收藏清單流水號',
    listing_sid             VARCHAR(32)                         NOT NULL COMMENT '商品上架序號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'Item序號',
    variant_sid             VARCHAR(32)                             NULL COMMENT '變體序號',
    target_price            DECIMAL(20,6)                           NULL COMMENT '到價提醒目標價格',
    price_alert_enabled     TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否啟用到價提醒',
    stock_alert_enabled     TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否啟用到貨提醒',
    note                    VARCHAR(1000)                           NULL COMMENT '會員備註',
    item_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;PURCHASED已購買;REMOVED移除',
    CONSTRAINT fk_swi_wishlist
        FOREIGN KEY (wishlist_nid) REFERENCES shp_wishlist(nid),
    CONSTRAINT uk_swi_wishlist_listing_variant UNIQUE (wishlist_nid, listing_sid, variant_sid),
    INDEX idx_swi_wishlist_nid (wishlist_nid),
    INDEX idx_swi_listing_sid (listing_sid),
    INDEX idx_swi_item_sid (item_sid),
    INDEX idx_swi_variant_sid (variant_sid),
    INDEX idx_swi_price_alert_enabled (price_alert_enabled),
    INDEX idx_swi_stock_alert_enabled (stock_alert_enabled),
    INDEX idx_swi_status (item_status),
    CHECK (target_price IS NULL OR target_price >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='收藏清單明細';

-- =========================================================
-- 06. 商品瀏覽與行為
-- =========================================================

CREATE TABLE IF NOT EXISTS shp_view_history (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '瀏覽紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '瀏覽時間',
    storefront_sid          VARCHAR(32)                         NOT NULL COMMENT '前台商店序號',
    member_party_sid        VARCHAR(32)                             NULL COMMENT '會員Party序號',
    user_sid                VARCHAR(32)                             NULL COMMENT '帳號序號',
    guest_token_hash        VARCHAR(255)                            NULL COMMENT '訪客Token雜湊',
    session_id              VARCHAR(200)                            NULL COMMENT 'Session ID',
    listing_sid             VARCHAR(32)                         NOT NULL COMMENT '商品上架序號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'Item序號',
    variant_sid             VARCHAR(32)                             NULL COMMENT '變體序號',
    source_page             VARCHAR(100)                            NULL COMMENT '來源頁面',
    referrer_url            VARCHAR(1000)                           NULL COMMENT '來源網址',
    search_keyword          VARCHAR(500)                            NULL COMMENT '來源搜尋關鍵字',
    device_type             VARCHAR(30)                             NULL COMMENT 'WEB;ANDROID;IOS;TABLET;OTHER',
    ip_address              VARCHAR(50)                             NULL COMMENT 'IP位址',
    view_duration_seconds   INT                                     NULL COMMENT '停留秒數',
    INDEX idx_svh_storefront_sid (storefront_sid),
    INDEX idx_svh_member_party_sid (member_party_sid),
    INDEX idx_svh_guest_token_hash (guest_token_hash),
    INDEX idx_svh_session_id (session_id),
    INDEX idx_svh_listing_sid (listing_sid),
    INDEX idx_svh_item_sid (item_sid),
    INDEX idx_svh_variant_sid (variant_sid),
    INDEX idx_svh_create_date (create_date),
    CHECK (view_duration_seconds IS NULL OR view_duration_seconds >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商品瀏覽紀錄';

CREATE TABLE IF NOT EXISTS shp_search_history (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '搜尋紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '搜尋時間',
    storefront_sid          VARCHAR(32)                         NOT NULL COMMENT '前台商店序號',
    member_party_sid        VARCHAR(32)                             NULL COMMENT '會員Party序號',
    guest_token_hash        VARCHAR(255)                            NULL COMMENT '訪客Token雜湊',
    session_id              VARCHAR(200)                            NULL COMMENT 'Session ID',
    keyword                 VARCHAR(500)                        NOT NULL COMMENT '搜尋關鍵字',
    filter_data             JSON                                    NULL COMMENT '篩選條件',
    sort_code               VARCHAR(50)                             NULL COMMENT '排序代碼',
    result_count            INT                                 NOT NULL DEFAULT 0 COMMENT '結果筆數',
    clicked_listing_sid     VARCHAR(32)                             NULL COMMENT '點擊商品上架序號',
    device_type             VARCHAR(30)                             NULL COMMENT '裝置類型',
    INDEX idx_ssh_storefront_sid (storefront_sid),
    INDEX idx_ssh_member_party_sid (member_party_sid),
    INDEX idx_ssh_guest_token_hash (guest_token_hash),
    INDEX idx_ssh_session_id (session_id),
    INDEX idx_ssh_keyword (keyword(191)),
    INDEX idx_ssh_clicked_listing_sid (clicked_listing_sid),
    INDEX idx_ssh_create_date (create_date),
    CHECK (result_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='前台搜尋紀錄';

CREATE TABLE IF NOT EXISTS shp_customer_event (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '顧客行為事件序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '事件時間',
    storefront_sid          VARCHAR(32)                         NOT NULL COMMENT '前台商店序號',
    member_party_sid        VARCHAR(32)                             NULL COMMENT '會員Party序號',
    guest_token_hash        VARCHAR(255)                            NULL COMMENT '訪客Token雜湊',
    session_id              VARCHAR(200)                            NULL COMMENT 'Session ID',
    event_code              VARCHAR(100)                        NOT NULL COMMENT 'VIEW_ITEM;ADD_CART;REMOVE_CART;BEGIN_CHECKOUT;ADD_WISHLIST;SEARCH;SHARE;REVIEW;QUESTION',
    entity_type             VARCHAR(30)                             NULL COMMENT 'LISTING;ITEM;VARIANT;CART;CHECKOUT;ORDER;REVIEW',
    entity_sid              VARCHAR(32)                             NULL COMMENT '事件實體序號',
    event_data              JSON                                    NULL COMMENT '事件資料',
    source_page             VARCHAR(200)                            NULL COMMENT '來源頁面',
    campaign_code           VARCHAR(100)                            NULL COMMENT '行銷活動代碼',
    correlation_id          VARCHAR(100)                            NULL COMMENT '跨服務關聯識別碼',
    INDEX idx_sce_storefront_sid (storefront_sid),
    INDEX idx_sce_member_party_sid (member_party_sid),
    INDEX idx_sce_guest_token_hash (guest_token_hash),
    INDEX idx_sce_session_id (session_id),
    INDEX idx_sce_event_code (event_code),
    INDEX idx_sce_entity (entity_type, entity_sid),
    INDEX idx_sce_campaign_code (campaign_code),
    INDEX idx_sce_correlation_id (correlation_id),
    INDEX idx_sce_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='顧客前台行為事件';

-- =========================================================
-- 07. 商品評論與評分
-- =========================================================

CREATE TABLE IF NOT EXISTS shp_review (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '商品評論序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    review_no               VARCHAR(100)                        NOT NULL COMMENT '評論編號',
    storefront_sid          VARCHAR(32)                         NOT NULL COMMENT '前台商店序號',
    listing_sid             VARCHAR(32)                         NOT NULL COMMENT '商品上架序號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'Item序號',
    variant_sid             VARCHAR(32)                             NULL COMMENT '變體序號',
    member_party_sid        VARCHAR(32)                         NOT NULL COMMENT '評論會員Party序號',
    sales_order_sid         VARCHAR(32)                             NULL COMMENT 'SalesOrderDB訂單序號',
    sales_order_item_sid    VARCHAR(32)                             NULL COMMENT 'SalesOrderDB訂單明細序號',
    verified_purchase       TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否已驗證購買',
    rating                  DECIMAL(3,2)                        NOT NULL COMMENT '評分',
    title                   VARCHAR(300)                            NULL COMMENT '評論標題',
    content                 TEXT                                    NULL COMMENT '評論內容',
    quality_rating          DECIMAL(3,2)                            NULL COMMENT '品質評分',
    value_rating            DECIMAL(3,2)                            NULL COMMENT '性價比評分',
    delivery_rating         DECIMAL(3,2)                            NULL COMMENT '配送評分',
    anonymous_mark          TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否匿名',
    helpful_count           INT                                 NOT NULL DEFAULT 0 COMMENT '有幫助數量',
    report_count            INT                                 NOT NULL DEFAULT 0 COMMENT '檢舉數量',
    review_status           VARCHAR(30)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待審;PUBLISHED已發布;HIDDEN隱藏;REJECTED拒絕;DELETED刪除',
    moderation_user_sid     VARCHAR(32)                             NULL COMMENT '審核人員序號',
    moderation_date         DATETIME                                NULL COMMENT '審核時間',
    moderation_note         TEXT                                    NULL COMMENT '審核說明',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    CONSTRAINT uk_sr_review_no UNIQUE (review_no),
    CONSTRAINT uk_sr_order_item_member UNIQUE (sales_order_item_sid, member_party_sid),
    INDEX idx_sr_storefront_sid (storefront_sid),
    INDEX idx_sr_listing_sid (listing_sid),
    INDEX idx_sr_item_sid (item_sid),
    INDEX idx_sr_variant_sid (variant_sid),
    INDEX idx_sr_member_party_sid (member_party_sid),
    INDEX idx_sr_sales_order_sid (sales_order_sid),
    INDEX idx_sr_verified_purchase (verified_purchase),
    INDEX idx_sr_rating (rating),
    INDEX idx_sr_status (review_status),
    INDEX idx_sr_avalible (avalible),
    FULLTEXT INDEX ftx_sr_content (title, content),
    CHECK (rating BETWEEN 1 AND 5),
    CHECK (quality_rating IS NULL OR quality_rating BETWEEN 1 AND 5),
    CHECK (value_rating IS NULL OR value_rating BETWEEN 1 AND 5),
    CHECK (delivery_rating IS NULL OR delivery_rating BETWEEN 1 AND 5),
    CHECK (helpful_count >= 0),
    CHECK (report_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商品評論與評分';

CREATE TABLE IF NOT EXISTS shp_review_media (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '評論媒體序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    review_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '評論流水號',
    media_type              VARCHAR(20)                         NOT NULL COMMENT 'IMAGE圖片;VIDEO影片',
    file_sid                VARCHAR(32)                         NOT NULL COMMENT 'FileDB檔案序號',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    media_status            VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;HIDDEN隱藏;DELETED刪除',
    CONSTRAINT fk_srm_review
        FOREIGN KEY (review_nid) REFERENCES shp_review(nid),
    CONSTRAINT uk_srm_review_file UNIQUE (review_nid, file_sid),
    INDEX idx_srm_review_nid (review_nid),
    INDEX idx_srm_file_sid (file_sid),
    INDEX idx_srm_status (media_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商品評論圖片與影片';

CREATE TABLE IF NOT EXISTS shp_review_reaction (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '評論反應序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    review_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '評論流水號',
    member_party_sid        VARCHAR(32)                         NOT NULL COMMENT '會員Party序號',
    reaction_type           VARCHAR(20)                         NOT NULL COMMENT 'HELPFUL有幫助;NOT_HELPFUL沒幫助;REPORT檢舉',
    report_reason_code      VARCHAR(50)                             NULL COMMENT '檢舉原因代碼',
    report_description      TEXT                                    NULL COMMENT '檢舉說明',
    reaction_status         VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;REVOKED撤回;RESOLVED已處理',
    CONSTRAINT fk_srr_review
        FOREIGN KEY (review_nid) REFERENCES shp_review(nid),
    CONSTRAINT uk_srr_review_member_type UNIQUE (review_nid, member_party_sid, reaction_type),
    INDEX idx_srr_review_nid (review_nid),
    INDEX idx_srr_member_party_sid (member_party_sid),
    INDEX idx_srr_reaction_type (reaction_type),
    INDEX idx_srr_status (reaction_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='評論有幫助與檢舉';

CREATE TABLE IF NOT EXISTS shp_review_reply (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '評論回覆序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    review_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '評論流水號',
    reply_type              VARCHAR(20)                         NOT NULL COMMENT 'STORE商店;CUSTOMER客服;SYSTEM系統',
    reply_user_sid          VARCHAR(32)                             NULL COMMENT '回覆帳號序號',
    reply_party_sid         VARCHAR(32)                             NULL COMMENT '回覆Party序號',
    content                 TEXT                                NOT NULL COMMENT '回覆內容',
    reply_status            VARCHAR(20)                         NOT NULL DEFAULT 'PUBLISHED' COMMENT 'PENDING待審;PUBLISHED已發布;HIDDEN隱藏;DELETED刪除',
    CONSTRAINT fk_srrep_review
        FOREIGN KEY (review_nid) REFERENCES shp_review(nid),
    INDEX idx_srrep_review_nid (review_nid),
    INDEX idx_srrep_reply_type (reply_type),
    INDEX idx_srrep_reply_user_sid (reply_user_sid),
    INDEX idx_srrep_status (reply_status),
    INDEX idx_srrep_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商店與客服評論回覆';

-- =========================================================
-- 08. 商品問答
-- =========================================================

CREATE TABLE IF NOT EXISTS shp_question (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '商品問題序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    question_no             VARCHAR(100)                        NOT NULL COMMENT '問題編號',
    storefront_sid          VARCHAR(32)                         NOT NULL COMMENT '前台商店序號',
    listing_sid             VARCHAR(32)                         NOT NULL COMMENT '商品上架序號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'Item序號',
    variant_sid             VARCHAR(32)                             NULL COMMENT '變體序號',
    member_party_sid        VARCHAR(32)                             NULL COMMENT '提問會員Party序號',
    guest_name              VARCHAR(200)                            NULL COMMENT '訪客名稱',
    guest_email             VARCHAR(200)                            NULL COMMENT '訪客Email',
    question_content        TEXT                                NOT NULL COMMENT '問題內容',
    public_mark             TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否公開',
    answer_count            INT                                 NOT NULL DEFAULT 0 COMMENT '回答數量',
    question_status         VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待審;PUBLISHED已發布;ANSWERED已回答;HIDDEN隱藏;REJECTED拒絕;CLOSED關閉',
    moderation_user_sid     VARCHAR(32)                             NULL COMMENT '審核人員序號',
    CONSTRAINT uk_sqst_question_no UNIQUE (question_no),
    INDEX idx_sqst_storefront_sid (storefront_sid),
    INDEX idx_sqst_listing_sid (listing_sid),
    INDEX idx_sqst_item_sid (item_sid),
    INDEX idx_sqst_variant_sid (variant_sid),
    INDEX idx_sqst_member_party_sid (member_party_sid),
    INDEX idx_sqst_public_mark (public_mark),
    INDEX idx_sqst_status (question_status),
    FULLTEXT INDEX ftx_sqst_content (question_content),
    CHECK (answer_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商品問答問題';

CREATE TABLE IF NOT EXISTS shp_answer (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '商品回答序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    question_nid            BIGINT UNSIGNED                     NOT NULL COMMENT '問題流水號',
    answer_type             VARCHAR(20)                         NOT NULL COMMENT 'STORE商店;CUSTOMER會員;SUPPLIER供應商;SYSTEM系統',
    answer_user_sid         VARCHAR(32)                             NULL COMMENT '回答帳號序號',
    answer_party_sid        VARCHAR(32)                             NULL COMMENT '回答Party序號',
    answer_content          TEXT                                NOT NULL COMMENT '回答內容',
    official_mark           TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否官方回答',
    accepted_mark           TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否被提問者採納',
    helpful_count           INT                                 NOT NULL DEFAULT 0 COMMENT '有幫助數量',
    answer_status           VARCHAR(20)                         NOT NULL DEFAULT 'PUBLISHED' COMMENT 'PENDING待審;PUBLISHED已發布;HIDDEN隱藏;REJECTED拒絕;DELETED刪除',
    CONSTRAINT fk_sa_question
        FOREIGN KEY (question_nid) REFERENCES shp_question(nid),
    INDEX idx_sa_question_nid (question_nid),
    INDEX idx_sa_answer_type (answer_type),
    INDEX idx_sa_answer_user_sid (answer_user_sid),
    INDEX idx_sa_answer_party_sid (answer_party_sid),
    INDEX idx_sa_official_mark (official_mark),
    INDEX idx_sa_accepted_mark (accepted_mark),
    INDEX idx_sa_status (answer_status),
    CHECK (helpful_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商品問答回答';

-- =========================================================
-- 09. 到貨與到價通知
-- =========================================================

CREATE TABLE IF NOT EXISTS shp_alert_subscription (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '提醒訂閱序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    storefront_sid          VARCHAR(32)                         NOT NULL COMMENT '前台商店序號',
    listing_sid             VARCHAR(32)                         NOT NULL COMMENT '商品上架序號',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'Item序號',
    variant_sid             VARCHAR(32)                             NULL COMMENT '變體序號',
    member_party_sid        VARCHAR(32)                             NULL COMMENT '會員Party序號',
    contact_type            VARCHAR(20)                         NOT NULL COMMENT 'EMAIL;SMS;PUSH;LINE',
    contact_value_hash      VARCHAR(128)                            NULL COMMENT '聯絡值雜湊',
    contact_value_encrypted LONGTEXT                                NULL COMMENT '加密後聯絡值',
    alert_type              VARCHAR(20)                         NOT NULL COMMENT 'BACK_IN_STOCK到貨;PRICE_DROP降價;TARGET_PRICE到價;PREORDER預購開始',
    target_price            DECIMAL(20,6)                           NULL COMMENT '目標價格',
    last_notified_date      DATETIME                                NULL COMMENT '最後通知時間',
    notify_count            INT                                 NOT NULL DEFAULT 0 COMMENT '通知次數',
    subscription_status     VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;NOTIFIED已通知;UNSUBSCRIBED取消;EXPIRED過期',
    expiry_date             DATETIME                                NULL COMMENT '到期時間',
    CONSTRAINT uk_sas_subscription UNIQUE (storefront_sid, listing_sid, variant_sid, member_party_sid, contact_type, alert_type),
    INDEX idx_sas_storefront_sid (storefront_sid),
    INDEX idx_sas_listing_sid (listing_sid),
    INDEX idx_sas_item_sid (item_sid),
    INDEX idx_sas_variant_sid (variant_sid),
    INDEX idx_sas_member_party_sid (member_party_sid),
    INDEX idx_sas_alert_type (alert_type),
    INDEX idx_sas_status (subscription_status),
    INDEX idx_sas_expiry_date (expiry_date),
    CHECK (target_price IS NULL OR target_price >= 0),
    CHECK (notify_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='到貨、降價與預購提醒訂閱';

-- =========================================================
-- 10. 棄單與轉換
-- =========================================================

CREATE TABLE IF NOT EXISTS shp_abandoned_cart (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '棄單序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    cart_sid                VARCHAR(32)                         NOT NULL COMMENT '購物車序號',
    checkout_sid            VARCHAR(32)                             NULL COMMENT '結帳工作階段序號',
    member_party_sid        VARCHAR(32)                             NULL COMMENT '會員Party序號',
    guest_email_hash        VARCHAR(128)                            NULL COMMENT '訪客Email雜湊',
    abandoned_stage         VARCHAR(30)                         NOT NULL COMMENT 'CART購物車;ADDRESS地址;SHIPPING配送;PAYMENT付款;CONFIRM確認',
    abandoned_date          DATETIME                            NOT NULL COMMENT '放棄時間',
    cart_amount             DECIMAL(20,4)                       NOT NULL DEFAULT 0 COMMENT '放棄時購物車金額',
    recovery_status         VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;CONTACTED已聯絡;RECOVERED已挽回;EXPIRED過期;IGNORED忽略',
    recovery_campaign_sid   VARCHAR(32)                             NULL COMMENT 'CampaignDB挽回活動序號',
    recovered_order_sid     VARCHAR(32)                             NULL COMMENT 'SalesOrderDB挽回後訂單序號',
    last_contact_date       DATETIME                                NULL COMMENT '最後聯絡時間',
    INDEX idx_sac_cart_sid (cart_sid),
    INDEX idx_sac_checkout_sid (checkout_sid),
    INDEX idx_sac_member_party_sid (member_party_sid),
    INDEX idx_sac_abandoned_stage (abandoned_stage),
    INDEX idx_sac_abandoned_date (abandoned_date),
    INDEX idx_sac_recovery_status (recovery_status),
    INDEX idx_sac_recovered_order_sid (recovered_order_sid),
    CHECK (cart_amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='購物車與結帳棄單';

CREATE TABLE IF NOT EXISTS shp_conversion_attribution (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '轉換歸因序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    sales_order_sid         VARCHAR(32)                         NOT NULL COMMENT 'SalesOrderDB正式訂單序號',
    storefront_sid          VARCHAR(32)                         NOT NULL COMMENT '前台商店序號',
    member_party_sid        VARCHAR(32)                             NULL COMMENT '會員Party序號',
    first_touch_source      VARCHAR(100)                            NULL COMMENT '首次來源',
    first_touch_campaign    VARCHAR(100)                            NULL COMMENT '首次活動',
    last_touch_source       VARCHAR(100)                            NULL COMMENT '最後來源',
    last_touch_campaign     VARCHAR(100)                            NULL COMMENT '最後活動',
    coupon_code             VARCHAR(100)                            NULL COMMENT '使用優惠碼',
    referral_party_sid      VARCHAR(32)                             NULL COMMENT '推薦人Party序號',
    affiliate_party_sid     VARCHAR(32)                             NULL COMMENT '聯盟夥伴Party序號',
    attribution_model       VARCHAR(30)                         NOT NULL DEFAULT 'LAST_TOUCH' COMMENT 'FIRST_TOUCH首次;LAST_TOUCH最後;LINEAR線性;CUSTOM自訂',
    attribution_data        JSON                                    NULL COMMENT '完整歸因資料',
    CONSTRAINT uk_sca2_sales_order_sid UNIQUE (sales_order_sid),
    INDEX idx_sca2_storefront_sid (storefront_sid),
    INDEX idx_sca2_member_party_sid (member_party_sid),
    INDEX idx_sca2_first_touch_source (first_touch_source),
    INDEX idx_sca2_last_touch_source (last_touch_source),
    INDEX idx_sca2_referral_party_sid (referral_party_sid),
    INDEX idx_sca2_affiliate_party_sid (affiliate_party_sid),
    INDEX idx_sca2_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='購物訂單轉換與行銷歸因';

-- =========================================================
-- 11. 狀態歷程與領域事件
-- =========================================================

CREATE TABLE IF NOT EXISTS shp_status_history (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '購物平台狀態歷程序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '異動時間',
    entity_type             VARCHAR(30)                         NOT NULL COMMENT 'LISTING;CART;CART_ITEM;CHECKOUT;WISHLIST;REVIEW;QUESTION;ANSWER;ALERT;ABANDONED_CART',
    entity_sid              VARCHAR(32)                         NOT NULL COMMENT '實體序號',
    old_status              VARCHAR(30)                             NULL COMMENT '原狀態',
    new_status              VARCHAR(30)                         NOT NULL COMMENT '新狀態',
    event_code              VARCHAR(100)                            NULL COMMENT '觸發事件代碼',
    operator_user_sid       VARCHAR(32)                             NULL COMMENT '操作人員序號',
    reason                  TEXT                                    NULL COMMENT '原因說明',
    correlation_id          VARCHAR(100)                            NULL COMMENT '跨服務關聯識別碼',
    INDEX idx_ssh2_entity (entity_type, entity_sid),
    INDEX idx_ssh2_new_status (new_status),
    INDEX idx_ssh2_event_code (event_code),
    INDEX idx_ssh2_operator_user_sid (operator_user_sid),
    INDEX idx_ssh2_correlation_id (correlation_id),
    INDEX idx_ssh2_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='購物平台狀態歷程';

CREATE TABLE IF NOT EXISTS shp_event (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '購物平台事件序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    entity_type             VARCHAR(30)                         NOT NULL COMMENT 'LISTING;CART;CHECKOUT;WISHLIST;REVIEW;QUESTION;ALERT;ABANDONED_CART',
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
    UNIQUE KEY uk_sev_source_event_id (source_event_id),
    INDEX idx_sev_entity (entity_type, entity_sid),
    INDEX idx_sev_event_code (event_code),
    INDEX idx_sev_correlation_id (correlation_id),
    INDEX idx_sev_outbox_event_sid (outbox_event_sid),
    INDEX idx_sev_status (process_status),
    INDEX idx_sev_create_date (create_date),
    CHECK (event_version > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='購物平台領域事件';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
