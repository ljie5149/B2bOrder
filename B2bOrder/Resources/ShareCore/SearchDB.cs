namespace B2bOrder.Resources.ShareCore
{
    /// <summary>
    /// SearchDB V1 Shared Core Schema
    /// 設計目標：
    /// 1. 搜尋索引同步主檔 (Index Queue / Sync State)：紀錄待 Sync / 重建至 Elasticsearch 的商品或內容資料狀態
    /// 2. 搜尋字典與同義詞管理 (Dictionary & Synonyms)：維護同義詞 (Synonyms)、停用詞 (Stopwords) 與自訂分詞字典
    /// 3. 搜尋熱詞與建議詞 (Hot Keywords & Auto-Complete)：紀錄熱門搜尋關鍵字、搜尋建議與搜尋提示詞
    /// 4. 搜尋軌跡與數據分析 (Search Analytics Log)：紀錄使用者搜尋行為、無結果關鍵字 (Zero Results) 與點擊轉化
    /// 5. 整合與串接 PIM/MIMDB (商品/內容資料源) 與 SalesOrderDB (搜尋點擊至轉換分析)
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class SearchDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 搜尋索引與同步狀態管理 (Index Task & Sync Registry)
-- =========================================================

CREATE TABLE IF NOT EXISTS sch_index_registry (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '索引任務序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    index_name              VARCHAR(100)                        NOT NULL COMMENT 'Elasticsearch 索引名稱 (例: idx_product_v1)',
    entity_type             VARCHAR(50)                         NOT NULL COMMENT '實體類型 (例: PRODUCT, ARTICLE, CATEGORY)',
    entity_sid              VARCHAR(32)                         NOT NULL COMMENT '對應業務實體序號 (例: PIM Item SID)',
    action_type             VARCHAR(20)                         NOT NULL DEFAULT 'UPSERT' COMMENT 'UPSERT更新或新增;DELETE刪除索引',
    sync_status             VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;PROCESSING處理中;SUCCESS同步成功;FAILED失敗',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '重試次數',
    error_message           TEXT                                    NULL COMMENT '最後一次同步失敗訊息',
    last_synced_at          DATETIME                                NULL COMMENT '最後成功同步時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_sir_entity UNIQUE (index_name, entity_type, entity_sid),
    INDEX idx_sir_company_sid (company_sid),
    INDEX idx_sir_status (sync_status),
    INDEX idx_sir_entity_lookup (entity_type, entity_sid),
    INDEX idx_sir_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='搜尋索引變更同步 Queue/紀錄檔';

-- =========================================================
-- 02. 同義詞與搜尋字典 (Synonyms & Dictionaries)
-- =========================================================

CREATE TABLE IF NOT EXISTS sch_synonym_group (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '同義詞組序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    group_name              VARCHAR(100)                        NOT NULL COMMENT '同義詞組名稱/主題',
    synonym_type            VARCHAR(20)                         NOT NULL DEFAULT 'EQUIVALENT' COMMENT 'EQUIVALENT雙向同義 (A=B=C);EXPLICIT單向同義 (A=>B)',
    synonym_words           TEXT                                NOT NULL COMMENT '同義詞清單 (以逗號分隔，例: iPhone,蘋果手機, iOS手機)',
    synonym_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_ssg_company_sid (company_sid),
    INDEX idx_ssg_status (synonym_status),
    INDEX idx_ssg_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='搜尋同義詞字典檔';

CREATE TABLE IF NOT EXISTS sch_custom_dictionary (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '自訂詞庫序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    word                    VARCHAR(100)                        NOT NULL COMMENT '自訂斷詞/關鍵字 (例: 珍珠奶茶, 氣炸鍋)',
    word_type               VARCHAR(30)                         NOT NULL DEFAULT 'CUSTOM_WORD' COMMENT 'CUSTOM_WORD自訂斷詞;STOPWORD停用詞;BOOST_WORD權重加高詞',
    frequency               INT                                 NOT NULL DEFAULT 1000 COMMENT '詞頻/權重設定 (供 Ik/Jieba 分析器參考)',
    dict_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_scd_company_word UNIQUE (company_sid, word, word_type),
    INDEX idx_scd_company_sid (company_sid),
    INDEX idx_scd_word_type (word_type),
    INDEX idx_scd_status (dict_status),
    INDEX idx_scd_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='搜尋自訂斷詞與停用詞庫檔';

-- =========================================================
-- 03. 搜尋熱詞與建議詞 (Hot Keywords & Auto-Complete)
-- =========================================================

CREATE TABLE IF NOT EXISTS sch_hot_keyword (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '熱詞序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    keyword                 VARCHAR(150)                        NOT NULL COMMENT '搜尋關鍵字',
    display_title           VARCHAR(150)                            NULL COMMENT '前端顯示名稱 (若與原關鍵字不同)',
    search_count            INT                                 NOT NULL DEFAULT 0 COMMENT '累積搜尋次數',
    priority_score          INT                                 NOT NULL DEFAULT 0 COMMENT '人工干預排位分數 (數字越大越靠前)',
    is_pinned               VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT 'Y置頂/強推;N正常',
    keyword_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE顯示;INACTIVE隱藏',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_shk_company_keyword UNIQUE (company_sid, keyword),
    INDEX idx_shk_company_sid (company_sid),
    INDEX idx_shk_score (priority_score, search_count),
    INDEX idx_shk_status (keyword_status),
    INDEX idx_shk_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='搜尋熱門關鍵字檔';

-- =========================================================
-- 04. 搜尋軌跡與數據分析 (Search Analytics & Click Tracking)
-- =========================================================

CREATE TABLE IF NOT EXISTS sch_search_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '搜尋紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '搜尋時間',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    customer_sid            VARCHAR(32)                             NULL COMMENT '會員序號 (未登入則為 NULL)',
    session_id              VARCHAR(100)                            NULL COMMENT 'Session/訪客識別碼',
    search_keyword          VARCHAR(200)                        NOT NULL COMMENT '使用者輸入之搜尋關鍵字',
    applied_filters_json    JSON                                    NULL COMMENT '套用之篩選條件 (例: 價格區間, 分類, 品牌)',
    result_count            INT                                 NOT NULL DEFAULT 0 COMMENT '搜尋結果比數',
    is_zero_result          VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT 'Y查無結果;N有結果',
    client_ip               VARCHAR(45)                             NULL COMMENT '用戶 IP 位址',
    user_agent              VARCHAR(500)                            NULL COMMENT '瀏覽器 User-Agent',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_ssl_company_sid (company_sid),
    INDEX idx_ssl_keyword (search_keyword),
    INDEX idx_ssl_customer_sid (customer_sid),
    INDEX idx_ssl_zero_result (is_zero_result),
    INDEX idx_ssl_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='搜尋行為軌跡日誌檔 (不可修改，僅供統計分析)';

CREATE TABLE IF NOT EXISTS sch_click_tracking (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '點擊紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '點擊時間',
    search_log_nid          BIGINT UNSIGNED                     NOT NULL COMMENT '搜尋紀錄流水號',
    entity_type             VARCHAR(50)                         NOT NULL DEFAULT 'PRODUCT' COMMENT '點擊實體類型 (例: PRODUCT, ARTICLE)',
    entity_sid              VARCHAR(32)                         NOT NULL COMMENT '被點擊實體序號 (例: PIM Item SID)',
    click_position          INT                                 NOT NULL COMMENT '點擊在搜尋結果列表中的名次位置 (例: 第1筆 = 1)',
    converted_to_cart       VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT 'Y加入購物車;N未加入',
    converted_to_order      VARCHAR(2)                          NOT NULL DEFAULT 'N' COMMENT 'Y產生購買轉換;N未購買',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_sct_search_log
        FOREIGN KEY (search_log_nid) REFERENCES sch_search_log(nid),
    INDEX idx_sct_search_log_nid (search_log_nid),
    INDEX idx_sct_entity (entity_type, entity_sid),
    INDEX idx_sct_position (click_position),
    INDEX idx_sct_conversion (converted_to_order)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='搜尋結果點擊與轉換追蹤檔';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}