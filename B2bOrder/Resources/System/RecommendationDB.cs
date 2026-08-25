namespace B2bOrder.Resources.System
{
    /// <summary>
    /// RecommendationDB V1 Shared Core Schema
    /// 設計目標：
    /// 1. 使用者偏好與特徵檔 (Customer Profile & Preferences)：儲存會員喜好分類、品牌偏好度與向量特徵
    /// 2. 商品關聯與相似度 (Item Similarity & Related Products)：儲存經常一起購買 (Frequently Bought Together)、相似商品矩陣
    /// 3. 個人化推薦清單與快取 (Personalized Recommendation Cache)：儲存針對指定會員預先算好的推薦商品排位
    /// 4. 推薦規則與人工干預 (Recommendation Rules & Interventions)：提供置頂強推 (Boost/Pin)、屏蔽排除 (Block/Bury) 機制
    /// 5. 推薦成效追蹤 (Recommendation Analytics)：紀錄推薦版位的曝光 (Impression)、點擊 (Click) 與訂單轉換 (Conversion)
    /// 6. 整合與串接 CustomerDB/MemberDB (會員特徵)、PIM/MIMDB (商品主檔) 與 SalesOrderDB (轉換分析)
    /// 7. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 8. avalible：Y可用；W停用；D刪除
    /// 9. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class RecommendationDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 會員偏好與推薦特徵 (Customer Profile & Preferences)
-- =========================================================

CREATE TABLE IF NOT EXISTS rec_customer_preference (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '偏好紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    customer_sid            VARCHAR(32)                         NOT NULL COMMENT 'CustomerDB/MemberDB 會員序號',
    preferred_categories_json JSON                                  NULL COMMENT '偏好商品分類與權重 (例: [{category_sid: ""CAT123"", weight: 0.85}])',
    preferred_brands_json   JSON                                    NULL COMMENT '偏好品牌與權重 (例: [{brand_sid: ""BRD456"", weight: 0.90}])',
    price_sensitivity_level VARCHAR(20)                         NOT NULL DEFAULT 'MEDIUM' COMMENT '價格敏感度: LOW低敏/高消費;MEDIUM中等;HIGH高敏/追求CP值',
    last_model_updated_at   DATETIME                                NULL COMMENT '推薦模型上次更新本會員特徵的時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_rcp_company_customer UNIQUE (company_sid, customer_sid),
    INDEX idx_rcp_company_sid (company_sid),
    INDEX idx_rcp_customer_sid (customer_sid),
    INDEX idx_rcp_price_sensitivity (price_sensitivity_level),
    INDEX idx_rcp_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='會員推薦偏好與特徵檔';

-- =========================================================
-- 02. 商品關聯與相似度矩陣 (Item-to-Item Similarity)
-- =========================================================

CREATE TABLE IF NOT EXISTS rec_item_similarity (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '關聯序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    source_item_sid         VARCHAR(32)                         NOT NULL COMMENT '來源商品序號 (PIM Item SID)',
    target_item_sid         VARCHAR(32)                         NOT NULL COMMENT '目標/關聯商品序號 (PIM Item SID)',
    relation_type           VARCHAR(30)                         NOT NULL DEFAULT 'SIMILAR' COMMENT 'SIMILAR相似品;BOUGHT_TOGETHER經常一起購買;CROSS_SELL交叉銷售;UP_SELL升級銷售',
    similarity_score        DECIMAL(8,6)                        NOT NULL DEFAULT 0.000000 COMMENT '相似度/相關性分數 (0.000000 ~ 1.000000)',
    rank_order              INT                                 NOT NULL DEFAULT 1 COMMENT '排序權重 (1為最相關)',
    model_version           VARCHAR(50)                         NOT NULL DEFAULT 'v1.0' COMMENT '演算模型版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ris_relation UNIQUE (company_sid, source_item_sid, target_item_sid, relation_type),
    INDEX idx_ris_company_sid (company_sid),
    INDEX idx_ris_source_item (source_item_sid),
    INDEX idx_ris_target_item (target_item_sid),
    INDEX idx_ris_type_score (relation_type, similarity_score DESC),
    INDEX idx_ris_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商品關聯與相似度矩陣檔';

-- =========================================================
-- 03. 個人化推薦結果清單與快取 (Personalized Recommendations)
-- =========================================================

CREATE TABLE IF NOT EXISTS rec_personal_recommendation (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '個人推薦序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '計算生成時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    customer_sid            VARCHAR(32)                         NOT NULL COMMENT '會員序號',
    scene_code              VARCHAR(50)                         NOT NULL COMMENT '推薦場景代碼 (例: HOME_PAGE, CART_BOTTOM, CHECKOUT_POPUP, EMAIL_EDM)',
    recommended_item_sid    VARCHAR(32)                         NOT NULL COMMENT '推薦商品序號 (PIM Item SID)',
    recommend_score         DECIMAL(8,6)                        NOT NULL DEFAULT 0.000000 COMMENT '推薦預測分數',
    display_rank            INT                                 NOT NULL COMMENT '建議展示順序 (1, 2, 3...)',
    algorithm_code          VARCHAR(50)                         NOT NULL DEFAULT 'CF_HYBRID' COMMENT '使用的演算法代碼 (例: CF_HYBRID, VECTOR_SEARCH, HOT_POPULAR)',
    expired_at              DATETIME                            NOT NULL COMMENT '快取過期時間',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_rpr_scene_item UNIQUE (company_sid, customer_sid, scene_code, recommended_item_sid),
    INDEX idx_rpr_company_sid (company_sid),
    INDEX idx_rpr_customer_scene (customer_sid, scene_code, display_rank),
    INDEX idx_rpr_expired_at (expired_at),
    INDEX idx_rpr_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='個人化推薦結果與快取檔';

-- =========================================================
-- 04. 推薦干預與控制規則 (Boost, Bury & Block Rules)
-- =========================================================

CREATE TABLE IF NOT EXISTS rec_rule_setting (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '推薦規則序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    rule_name               VARCHAR(150)                        NOT NULL COMMENT '規則名稱 (例: 慶典活動主推高利潤商品)',
    scene_code              VARCHAR(50)                         NOT NULL DEFAULT 'ALL' COMMENT '適用推薦場景 (ALL代表全站適用)',
    action_type             VARCHAR(20)                         NOT NULL DEFAULT 'BOOST' COMMENT 'BOOST提權/強推;BURY降權;BLOCK黑名單/屏蔽;PIN固定位置',
    target_entity_type      VARCHAR(30)                         NOT NULL DEFAULT 'ITEM' COMMENT '目標實體: ITEM指定商品;CATEGORY指定分類;BRAND指定品牌',
    target_entity_sid       VARCHAR(32)                         NOT NULL COMMENT '目標實體序號',
    boost_weight            DECIMAL(5,2)                        NOT NULL DEFAULT 1.00 COMMENT '加權倍率 (例: 1.5 表示分數乘 1.5)',
    fixed_pin_position      INT                                     NULL COMMENT '當 action_type=PIN 時強行固定在第幾位',
    start_time              DATETIME                                NULL COMMENT '規則生效開始時間',
    end_time                DATETIME                                NULL COMMENT '規則生效結束時間',
    rule_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_rrs_company_sid (company_sid),
    INDEX idx_rrs_scene_code (scene_code),
    INDEX idx_rrs_action_type (action_type),
    INDEX idx_rrs_status (rule_status),
    INDEX idx_rrs_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='推薦系統人工干預與調控規則檔';

-- =========================================================
-- 05. 推薦成效與轉換追蹤 (Recommendation Analytics)
-- =========================================================

CREATE TABLE IF NOT EXISTS rec_impression_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '曝光紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '曝光時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    customer_sid            VARCHAR(32)                             NULL COMMENT '會員序號 (訪客則為 NULL)',
    session_id              VARCHAR(100)                            NULL COMMENT 'Session/訪客識別碼',
    scene_code              VARCHAR(50)                         NOT NULL COMMENT '推薦場景代碼',
    recommended_item_sid    VARCHAR(32)                         NOT NULL COMMENT '被推薦的商品序號',
    display_position        INT                                 NOT NULL COMMENT '展示位置/順序 (例: 第1個位置 = 1)',
    algorithm_code          VARCHAR(50)                             NULL COMMENT '計算該推薦結果之演算法代碼',
    is_clicked              VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT 'Y已點擊;N未點擊',
    click_time              DATETIME                                NULL COMMENT '點擊時間',
    converted_to_order      VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT 'Y產生下單轉換;N未轉換',
    sales_order_sid         VARCHAR(32)                             NULL COMMENT '產生轉換之 SalesOrderDB 銷售單序號',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_ril_company_sid (company_sid),
    INDEX idx_ril_scene_item (scene_code, recommended_item_sid),
    INDEX idx_ril_customer_sid (customer_sid),
    INDEX idx_ril_clicked (is_clicked),
    INDEX idx_ril_conversion (converted_to_order),
    INDEX idx_ril_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='推薦版位曝光、點擊與轉換歷程檔 (不可修改，僅供統計分析)';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}