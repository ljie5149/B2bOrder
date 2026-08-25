namespace B2bOrder.Resources.ShareCore.eCommerce
{
    /// <summary>
    /// PIM/MIMDB V2 Shared Core Schema
    /// 設計目標：
    /// 1. 同時支援 Shopping Platform 商品與 Construction ERP 材料/設備主資料
    /// 2. Item 為產業中立實體，可代表商品、材料、設備、服務、組合品與標準工項
    /// 3. 價格、庫存、成本與交易資料不存於本資料庫
    /// 4. 文件實體由 FileDB 管理；本資料庫保存文件用途與業務狀態
    /// 5. 審核流程由 WorkflowDB 管理；主資料治理由 MDMDB 管理
    /// 6. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 7. avalible：Y可用；W停用；D刪除
    /// 8. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class PIMDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. Item核心主檔
-- =========================================================

CREATE TABLE IF NOT EXISTS pim_item (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Item序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    item_no                 VARCHAR(100)                        NOT NULL COMMENT 'Item編號',
    item_type               VARCHAR(30)                         NOT NULL COMMENT 'PRODUCT商品;MATERIAL材料;EQUIPMENT設備;SERVICE服務;BUNDLE組合;WORK_ITEM工項;DIGITAL數位;OTHER',
    item_name               VARCHAR(500)                        NOT NULL COMMENT '預設名稱',
    short_name              VARCHAR(200)                            NULL COMMENT '簡稱',
    category_sid            VARCHAR(32)                         NOT NULL COMMENT 'MasterDB分類序號',
    brand_sid               VARCHAR(32)                             NULL COMMENT 'MasterDB品牌序號',
    manufacturer_party_sid  VARCHAR(32)                             NULL COMMENT 'PartyDB製造商序號',
    preferred_supplier_sid  VARCHAR(32)                             NULL COMMENT 'PartyDB主要供應商序號',
    base_unit_sid           VARCHAR(32)                             NULL COMMENT 'MasterDB基本單位序號',
    tax_sid                 VARCHAR(32)                             NULL COMMENT 'MasterDB預設稅別序號',
    origin_country_sid      VARCHAR(32)                             NULL COMMENT 'MasterDB原產國序號',
    model_no                VARCHAR(150)                            NULL COMMENT '型號',
    manufacturer_no         VARCHAR(150)                            NULL COMMENT '製造商料號',
    primary_barcode         VARCHAR(100)                            NULL COMMENT '主條碼',
    serial_control          TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否序號控管',
    batch_control           TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否批號控管',
    shelf_life_days         INT                                     NULL COMMENT '保存期限天數',
    warranty_months         INT                                     NULL COMMENT '保固月數',
    hazardous_mark          TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否危險品',
    restricted_mark         TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否限制品',
    virtual_mark            TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否虛擬Item',
    reusable_mark           TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否可重複使用',
    asset_mark              TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否可轉固定資產',
    content_completeness    DECIMAL(8,2)                        NOT NULL DEFAULT 0 COMMENT '資料完整度百分比',
    current_version         INT                                 NOT NULL DEFAULT 1 COMMENT '目前版本',
    item_status             VARCHAR(30)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;REVIEW待審;APPROVED核准;ACTIVE啟用;INACTIVE停用;DISCONTINUED停用供應;ARCHIVED封存',
    owner_user_sid          VARCHAR(32)                             NULL COMMENT '資料負責人帳號序號',
    mdm_golden_sid          VARCHAR(32)                             NULL COMMENT 'MDMDB Golden Record序號',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_pi_item_no UNIQUE (item_no),
    UNIQUE KEY uk_pi_primary_barcode (primary_barcode),
    INDEX idx_pi_item_type (item_type),
    INDEX idx_pi_category_sid (category_sid),
    INDEX idx_pi_brand_sid (brand_sid),
    INDEX idx_pi_manufacturer_party_sid (manufacturer_party_sid),
    INDEX idx_pi_preferred_supplier_sid (preferred_supplier_sid),
    INDEX idx_pi_status (item_status),
    INDEX idx_pi_mdm_golden_sid (mdm_golden_sid),
    INDEX idx_pi_avalible (avalible),
    FULLTEXT INDEX ftx_pi_name (item_name, short_name),
    CHECK (shelf_life_days IS NULL OR shelf_life_days > 0),
    CHECK (warranty_months IS NULL OR warranty_months >= 0),
    CHECK (content_completeness BETWEEN 0 AND 100),
    CHECK (current_version > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='跨產業商品、材料與設備主檔';

CREATE TABLE IF NOT EXISTS pim_item_translation (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Item多語系序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    item_nid                BIGINT UNSIGNED                     NOT NULL COMMENT 'Item流水號',
    language_sid            VARCHAR(32)                         NOT NULL COMMENT 'MasterDB語系序號',
    item_name               VARCHAR(500)                        NOT NULL COMMENT '名稱',
    short_name              VARCHAR(200)                            NULL COMMENT '簡稱',
    subtitle                VARCHAR(500)                            NULL COMMENT '副標題',
    short_description       TEXT                                    NULL COMMENT '簡介',
    full_description        LONGTEXT                                NULL COMMENT '完整說明',
    usage_instruction       LONGTEXT                                NULL COMMENT '使用或施工說明',
    warning_text            LONGTEXT                                NULL COMMENT '警語',
    warranty_text           LONGTEXT                                NULL COMMENT '保固說明',
    translation_status      VARCHAR(20)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;REVIEW待審;APPROVED核准;PUBLISHED已發布',
    translator_user_sid     VARCHAR(32)                             NULL COMMENT '翻譯人員序號',
    approved_user_sid       VARCHAR(32)                             NULL COMMENT '核准人員序號',
    approved_date           DATETIME                                NULL COMMENT '核准時間',
    CONSTRAINT fk_pit_item
        FOREIGN KEY (item_nid) REFERENCES pim_item(nid),
    CONSTRAINT uk_pit_item_language UNIQUE (item_nid, language_sid),
    INDEX idx_pit_item_nid (item_nid),
    INDEX idx_pit_language_sid (language_sid),
    INDEX idx_pit_status (translation_status),
    FULLTEXT INDEX ftx_pit_search (item_name, subtitle, short_description, full_description)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item多語系內容';

-- =========================================================
-- 02. 變體、SKU與型號
-- =========================================================

CREATE TABLE IF NOT EXISTS pim_variant (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '變體序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    item_nid                BIGINT UNSIGNED                     NOT NULL COMMENT 'Item流水號',
    variant_no              VARCHAR(100)                        NOT NULL COMMENT 'SKU或變體編號',
    variant_name            VARCHAR(500)                            NULL COMMENT '變體名稱',
    barcode                 VARCHAR(100)                            NULL COMMENT '條碼',
    manufacturer_variant_no VARCHAR(150)                            NULL COMMENT '製造商變體編號',
    model_no                VARCHAR(150)                            NULL COMMENT '型號',
    weight                  DECIMAL(20,6)                           NULL COMMENT '重量',
    weight_unit_sid         VARCHAR(32)                             NULL COMMENT 'MasterDB重量單位序號',
    length                  DECIMAL(20,6)                           NULL COMMENT '長度',
    width                   DECIMAL(20,6)                           NULL COMMENT '寬度',
    height                  DECIMAL(20,6)                           NULL COMMENT '高度',
    dimension_unit_sid      VARCHAR(32)                             NULL COMMENT 'MasterDB尺寸單位序號',
    volume                  DECIMAL(20,6)                           NULL COMMENT '體積',
    minimum_order_qty       DECIMAL(20,6)                       NOT NULL DEFAULT 1 COMMENT '最小訂購量',
    order_multiple_qty      DECIMAL(20,6)                       NOT NULL DEFAULT 1 COMMENT '訂購倍數',
    default_mark            TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否預設變體',
    variant_status          VARCHAR(30)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用;DISCONTINUED停供;ARCHIVED封存',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_pv_item
        FOREIGN KEY (item_nid) REFERENCES pim_item(nid),
    CONSTRAINT uk_pv_variant_no UNIQUE (variant_no),
    UNIQUE KEY uk_pv_barcode (barcode),
    INDEX idx_pv_item_nid (item_nid),
    INDEX idx_pv_default_mark (default_mark),
    INDEX idx_pv_status (variant_status),
    INDEX idx_pv_avalible (avalible),
    CHECK (weight IS NULL OR weight >= 0),
    CHECK (length IS NULL OR length >= 0),
    CHECK (width IS NULL OR width >= 0),
    CHECK (height IS NULL OR height >= 0),
    CHECK (volume IS NULL OR volume >= 0),
    CHECK (minimum_order_qty > 0),
    CHECK (order_multiple_qty > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item SKU、材料規格型號與設備變體';

CREATE TABLE IF NOT EXISTS pim_identifier (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '識別碼序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    item_nid                BIGINT UNSIGNED                     NOT NULL COMMENT 'Item流水號',
    variant_nid             BIGINT UNSIGNED                         NULL COMMENT '變體流水號',
    identifier_type         VARCHAR(30)                         NOT NULL COMMENT 'BARCODE條碼;GTIN;UPC;EAN;ISBN;MODEL型號;MANUFACTURER製造商料號;CUSTOM自訂',
    identifier_value        VARCHAR(300)                        NOT NULL COMMENT '識別碼值',
    issuer_party_sid        VARCHAR(32)                             NULL COMMENT '核發或提供Party序號',
    primary_mark            TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否主要識別碼',
    start_date              DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '生效時間',
    end_date                DATETIME                                NULL COMMENT '失效時間',
    identifier_status       VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;EXPIRED過期;REVOKED撤銷',
    CONSTRAINT fk_pid_item
        FOREIGN KEY (item_nid) REFERENCES pim_item(nid),
    CONSTRAINT fk_pid_variant
        FOREIGN KEY (variant_nid) REFERENCES pim_variant(nid),
    CONSTRAINT uk_pid_type_value UNIQUE (identifier_type, identifier_value),
    INDEX idx_pid_item_nid (item_nid),
    INDEX idx_pid_variant_nid (variant_nid),
    INDEX idx_pid_issuer_party_sid (issuer_party_sid),
    INDEX idx_pid_primary_mark (primary_mark),
    INDEX idx_pid_status (identifier_status),
    CHECK (end_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item多重識別碼';

-- =========================================================
-- 03. 規格與規格值
-- =========================================================

CREATE TABLE IF NOT EXISTS pim_specification (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '規格序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    specification_code      VARCHAR(100)                        NOT NULL COMMENT '規格代碼',
    specification_name      VARCHAR(200)                        NOT NULL COMMENT '規格名稱',
    specification_category  VARCHAR(30)                         NOT NULL DEFAULT 'VARIANT' COMMENT 'VARIANT變體;TECHNICAL技術;DIMENSION尺寸;APPEARANCE外觀;CONSTRUCTION施工',
    display_type            VARCHAR(30)                         NOT NULL DEFAULT 'TEXT' COMMENT 'TEXT文字;COLOR色票;IMAGE圖片;SELECT下拉;BUTTON按鈕',
    selection_mode          VARCHAR(20)                         NOT NULL DEFAULT 'SINGLE' COMMENT 'SINGLE單選;MULTIPLE複選',
    required_mark           TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否必填',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ps_specification_code UNIQUE (specification_code),
    INDEX idx_ps_category (specification_category),
    INDEX idx_ps_display_type (display_type),
    INDEX idx_ps_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item規格定義';

CREATE TABLE IF NOT EXISTS pim_specification_value (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '規格值序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    specification_nid       BIGINT UNSIGNED                     NOT NULL COMMENT '規格流水號',
    value_code              VARCHAR(100)                        NOT NULL COMMENT '規格值代碼',
    value_name              VARCHAR(200)                        NOT NULL COMMENT '規格值名稱',
    display_value           VARCHAR(500)                            NULL COMMENT '顯示值',
    numeric_value           DECIMAL(30,10)                          NULL COMMENT '數字值',
    unit_sid                VARCHAR(32)                             NULL COMMENT 'MasterDB單位序號',
    color_code              VARCHAR(20)                             NULL COMMENT '色碼',
    image_file_sid          VARCHAR(32)                             NULL COMMENT 'FileDB圖片序號',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_psv_specification
        FOREIGN KEY (specification_nid) REFERENCES pim_specification(nid),
    CONSTRAINT uk_psv_spec_value UNIQUE (specification_nid, value_code),
    INDEX idx_psv_specification_nid (specification_nid),
    INDEX idx_psv_unit_sid (unit_sid),
    INDEX idx_psv_image_file_sid (image_file_sid),
    INDEX idx_psv_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item規格值';

CREATE TABLE IF NOT EXISTS pim_variant_specification (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '變體規格序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    variant_nid             BIGINT UNSIGNED                     NOT NULL COMMENT '變體流水號',
    specification_nid       BIGINT UNSIGNED                     NOT NULL COMMENT '規格流水號',
    specification_value_nid BIGINT UNSIGNED                     NOT NULL COMMENT '規格值流水號',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    CONSTRAINT fk_pvs_variant
        FOREIGN KEY (variant_nid) REFERENCES pim_variant(nid),
    CONSTRAINT fk_pvs_specification
        FOREIGN KEY (specification_nid) REFERENCES pim_specification(nid),
    CONSTRAINT fk_pvs_specification_value
        FOREIGN KEY (specification_value_nid) REFERENCES pim_specification_value(nid),
    CONSTRAINT uk_pvs_variant_spec UNIQUE (variant_nid, specification_nid),
    INDEX idx_pvs_variant_nid (variant_nid),
    INDEX idx_pvs_specification_nid (specification_nid),
    INDEX idx_pvs_specification_value_nid (specification_value_nid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='變體與規格值關聯';

-- =========================================================
-- 04. 動態屬性
-- =========================================================

CREATE TABLE IF NOT EXISTS pim_attribute_group (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '屬性群組序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    group_code              VARCHAR(100)                        NOT NULL COMMENT '群組代碼',
    group_name              VARCHAR(200)                        NOT NULL COMMENT '群組名稱',
    category_sid            VARCHAR(32)                             NULL COMMENT 'MasterDB適用分類序號',
    item_type               VARCHAR(30)                             NULL COMMENT '限定Item類型',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_pag_group_code UNIQUE (group_code),
    INDEX idx_pag_category_sid (category_sid),
    INDEX idx_pag_item_type (item_type),
    INDEX idx_pag_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item屬性群組';

CREATE TABLE IF NOT EXISTS pim_attribute (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '屬性序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    attribute_group_nid     BIGINT UNSIGNED                         NULL COMMENT '屬性群組流水號',
    attribute_code          VARCHAR(100)                        NOT NULL COMMENT '屬性代碼',
    attribute_name          VARCHAR(200)                        NOT NULL COMMENT '屬性名稱',
    data_type               VARCHAR(30)                         NOT NULL COMMENT 'TEXT文字;NUMBER數字;BOOLEAN布林;DATE日期;OPTION選項;MULTI_OPTION多選;JSON結構',
    unit_sid                VARCHAR(32)                             NULL COMMENT 'MasterDB單位序號',
    required_mark           TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否必填',
    searchable_mark         TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否可搜尋',
    filterable_mark         TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否可篩選',
    comparable_mark         TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否可比較',
    variant_mark            TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否用於變體',
    technical_mark          TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否技術屬性',
    compliance_mark         TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否法規或合規屬性',
    validation_rule         JSON                                    NULL COMMENT '驗證規則',
    default_value           LONGTEXT                                NULL COMMENT '預設值',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_pa_group
        FOREIGN KEY (attribute_group_nid) REFERENCES pim_attribute_group(nid),
    CONSTRAINT uk_pa_attribute_code UNIQUE (attribute_code),
    INDEX idx_pa_group_nid (attribute_group_nid),
    INDEX idx_pa_data_type (data_type),
    INDEX idx_pa_searchable_mark (searchable_mark),
    INDEX idx_pa_filterable_mark (filterable_mark),
    INDEX idx_pa_technical_mark (technical_mark),
    INDEX idx_pa_compliance_mark (compliance_mark),
    INDEX idx_pa_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item動態屬性';

CREATE TABLE IF NOT EXISTS pim_attribute_option (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '屬性選項序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    attribute_nid           BIGINT UNSIGNED                     NOT NULL COMMENT '屬性流水號',
    option_code             VARCHAR(100)                        NOT NULL COMMENT '選項代碼',
    option_name             VARCHAR(200)                        NOT NULL COMMENT '選項名稱',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_pao_attribute
        FOREIGN KEY (attribute_nid) REFERENCES pim_attribute(nid),
    CONSTRAINT uk_pao_attribute_option UNIQUE (attribute_nid, option_code),
    INDEX idx_pao_attribute_nid (attribute_nid),
    INDEX idx_pao_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item屬性選項';

CREATE TABLE IF NOT EXISTS pim_attribute_value (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '屬性值序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    item_nid                BIGINT UNSIGNED                     NOT NULL COMMENT 'Item流水號',
    variant_nid             BIGINT UNSIGNED                         NULL COMMENT '變體流水號，NULL代表Item層級',
    attribute_nid           BIGINT UNSIGNED                     NOT NULL COMMENT '屬性流水號',
    value_text              LONGTEXT                                NULL COMMENT '文字值',
    value_number            DECIMAL(30,10)                          NULL COMMENT '數字值',
    value_boolean           TINYINT(1)                              NULL COMMENT '布林值',
    value_date              DATE                                    NULL COMMENT '日期值',
    option_sid              VARCHAR(32)                             NULL COMMENT '單一選項序號',
    multi_value_json        JSON                                    NULL COMMENT '多選或JSON值',
    language_sid            VARCHAR(32)                             NULL COMMENT 'MasterDB語系序號',
    source_type             VARCHAR(30)                         NOT NULL DEFAULT 'MANUAL' COMMENT 'MANUAL人工;IMPORT匯入;SUPPLIER供應商;MANUFACTURER製造商;AI自動',
    confidence_score        DECIMAL(8,4)                            NULL COMMENT '自動產生信心分數',
    approved_mark           TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否已核准',
    approved_user_sid       VARCHAR(32)                             NULL COMMENT '核准人員序號',
    approved_date           DATETIME                                NULL COMMENT '核准時間',
    CONSTRAINT fk_pav_item
        FOREIGN KEY (item_nid) REFERENCES pim_item(nid),
    CONSTRAINT fk_pav_variant
        FOREIGN KEY (variant_nid) REFERENCES pim_variant(nid),
    CONSTRAINT fk_pav_attribute
        FOREIGN KEY (attribute_nid) REFERENCES pim_attribute(nid),
    CONSTRAINT uk_pav_value UNIQUE (item_nid, variant_nid, attribute_nid, language_sid),
    INDEX idx_pav_item_nid (item_nid),
    INDEX idx_pav_variant_nid (variant_nid),
    INDEX idx_pav_attribute_nid (attribute_nid),
    INDEX idx_pav_option_sid (option_sid),
    INDEX idx_pav_language_sid (language_sid),
    INDEX idx_pav_source_type (source_type),
    INDEX idx_pav_approved_mark (approved_mark),
    CHECK (confidence_score IS NULL OR confidence_score BETWEEN 0 AND 1)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item與變體屬性值';

-- =========================================================
-- 05. 文件、媒體與證書
-- =========================================================

CREATE TABLE IF NOT EXISTS pim_media (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '媒體序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    item_nid                BIGINT UNSIGNED                     NOT NULL COMMENT 'Item流水號',
    variant_nid             BIGINT UNSIGNED                         NULL COMMENT '變體流水號',
    media_type              VARCHAR(30)                         NOT NULL COMMENT 'IMAGE圖片;VIDEO影片;DOCUMENT文件;MODEL_3D 3D模型;MANUAL說明書;DRAWING圖面;CERTIFICATE證書',
    file_sid                VARCHAR(32)                         NOT NULL COMMENT 'FileDB檔案序號',
    usage_type              VARCHAR(30)                         NOT NULL COMMENT 'COVER封面;GALLERY圖庫;DETAIL詳情;SPEC規格;INSTALLATION施工;SAFETY安全;CERTIFICATE證書;MANUAL說明書',
    language_sid            VARCHAR(32)                             NULL COMMENT 'MasterDB語系序號',
    alt_text                VARCHAR(500)                            NULL COMMENT '替代文字',
    title                   VARCHAR(500)                            NULL COMMENT '媒體標題',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    primary_mark            TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否主要媒體',
    media_status            VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用;PROCESSING處理中;FAILED失敗',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_pm_item
        FOREIGN KEY (item_nid) REFERENCES pim_item(nid),
    CONSTRAINT fk_pm_variant
        FOREIGN KEY (variant_nid) REFERENCES pim_variant(nid),
    CONSTRAINT uk_pm_file_usage UNIQUE (item_nid, variant_nid, file_sid, usage_type),
    INDEX idx_pm_item_nid (item_nid),
    INDEX idx_pm_variant_nid (variant_nid),
    INDEX idx_pm_file_sid (file_sid),
    INDEX idx_pm_media_type (media_type),
    INDEX idx_pm_usage_type (usage_type),
    INDEX idx_pm_primary_mark (primary_mark),
    INDEX idx_pm_status (media_status),
    INDEX idx_pm_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item媒體、文件、圖面與證書';

CREATE TABLE IF NOT EXISTS pim_certification (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '認證序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    item_nid                BIGINT UNSIGNED                     NOT NULL COMMENT 'Item流水號',
    variant_nid             BIGINT UNSIGNED                         NULL COMMENT '變體流水號',
    certification_type      VARCHAR(50)                         NOT NULL COMMENT 'QUALITY品質;SAFETY安全;ENVIRONMENT環保;FIRE防火;ENERGY節能;MATERIAL_TEST材料試驗;OTHER',
    certification_code      VARCHAR(100)                            NULL COMMENT '認證代碼',
    certification_name      VARCHAR(300)                        NOT NULL COMMENT '認證名稱',
    issuing_party_sid       VARCHAR(32)                             NULL COMMENT '核發Party序號',
    certificate_no          VARCHAR(150)                            NULL COMMENT '證書號碼',
    issue_date              DATE                                    NULL COMMENT '核發日期',
    expiry_date             DATE                                    NULL COMMENT '到期日期',
    file_sid                VARCHAR(32)                             NULL COMMENT 'FileDB證書檔案序號',
    verified_mark           TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否已驗證',
    verified_user_sid       VARCHAR(32)                             NULL COMMENT '驗證人員序號',
    verified_date           DATETIME                                NULL COMMENT '驗證時間',
    certification_status    VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待驗;VALID有效;EXPIRED過期;REVOKED撤銷;INVALID無效',
    CONSTRAINT fk_pc_item
        FOREIGN KEY (item_nid) REFERENCES pim_item(nid),
    CONSTRAINT fk_pc_variant
        FOREIGN KEY (variant_nid) REFERENCES pim_variant(nid),
    INDEX idx_pc_item_nid (item_nid),
    INDEX idx_pc_variant_nid (variant_nid),
    INDEX idx_pc_type (certification_type),
    INDEX idx_pc_issuing_party_sid (issuing_party_sid),
    INDEX idx_pc_certificate_no (certificate_no),
    INDEX idx_pc_expiry_date (expiry_date),
    INDEX idx_pc_status (certification_status),
    CHECK (expiry_date IS NULL OR issue_date IS NULL OR expiry_date >= issue_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item認證、檢驗與合格證書';

-- =========================================================
-- 06. 供應來源與製造商對應
-- =========================================================

CREATE TABLE IF NOT EXISTS pim_item_party (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Item Party關聯序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    item_nid                BIGINT UNSIGNED                     NOT NULL COMMENT 'Item流水號',
    variant_nid             BIGINT UNSIGNED                         NULL COMMENT '變體流水號',
    party_sid               VARCHAR(32)                         NOT NULL COMMENT 'PartyDB Party序號',
    relationship_type       VARCHAR(30)                         NOT NULL COMMENT 'MANUFACTURER製造商;SUPPLIER供應商;DISTRIBUTOR經銷商;INSTALLER安裝商;SERVICE_PROVIDER服務商',
    party_item_no           VARCHAR(150)                            NULL COMMENT '對方Item編號',
    preferred_mark          TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否優先',
    approved_mark           TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否已核准',
    lead_time_days          INT                                 NOT NULL DEFAULT 0 COMMENT '交期天數',
    minimum_order_qty       DECIMAL(20,6)                           NULL COMMENT '最低訂購量',
    start_date              DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '生效時間',
    end_date                DATETIME                                NULL COMMENT '失效時間',
    relation_status         VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'PENDING待審;ACTIVE有效;SUSPENDED暫停;EXPIRED過期',
    workflow_instance_sid   VARCHAR(32)                             NULL COMMENT 'WorkflowDB流程實例序號',
    CONSTRAINT fk_pip_item
        FOREIGN KEY (item_nid) REFERENCES pim_item(nid),
    CONSTRAINT fk_pip_variant
        FOREIGN KEY (variant_nid) REFERENCES pim_variant(nid),
    CONSTRAINT uk_pip_relation UNIQUE (item_nid, variant_nid, party_sid, relationship_type),
    INDEX idx_pip_item_nid (item_nid),
    INDEX idx_pip_variant_nid (variant_nid),
    INDEX idx_pip_party_sid (party_sid),
    INDEX idx_pip_relationship_type (relationship_type),
    INDEX idx_pip_preferred_mark (preferred_mark),
    INDEX idx_pip_approved_mark (approved_mark),
    INDEX idx_pip_status (relation_status),
    CHECK (lead_time_days >= 0),
    CHECK (minimum_order_qty IS NULL OR minimum_order_qty > 0),
    CHECK (end_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item製造商、供應商與服務商關聯';

-- =========================================================
-- 07. 替代品、關聯品與適用範圍
-- =========================================================

CREATE TABLE IF NOT EXISTS pim_item_relation (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Item關聯序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    item_nid                BIGINT UNSIGNED                     NOT NULL COMMENT '來源Item流水號',
    related_item_sid        VARCHAR(32)                         NOT NULL COMMENT '關聯Item序號',
    relation_type           VARCHAR(30)                         NOT NULL COMMENT 'ACCESSORY配件;SUBSTITUTE替代品;EQUIVALENT同等品;UPSELL升級;CROSS_SELL交叉銷售;REQUIRED必搭;COMPATIBLE相容',
    priority                INT                                 NOT NULL DEFAULT 0 COMMENT '優先順序',
    equivalence_level       VARCHAR(20)                             NULL COMMENT 'EXACT完全等同;TECHNICAL技術等同;COMMERCIAL商業替代;CONDITIONAL有條件',
    condition_json          JSON                                    NULL COMMENT '適用條件',
    start_date              DATETIME                                NULL COMMENT '生效時間',
    end_date                DATETIME                                NULL COMMENT '失效時間',
    approved_mark           TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否已核准',
    workflow_instance_sid   VARCHAR(32)                             NULL COMMENT 'WorkflowDB流程實例序號',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_pir_item
        FOREIGN KEY (item_nid) REFERENCES pim_item(nid),
    CONSTRAINT uk_pir_relation UNIQUE (item_nid, related_item_sid, relation_type),
    INDEX idx_pir_item_nid (item_nid),
    INDEX idx_pir_related_item_sid (related_item_sid),
    INDEX idx_pir_relation_type (relation_type),
    INDEX idx_pir_approved_mark (approved_mark),
    INDEX idx_pir_workflow_instance_sid (workflow_instance_sid),
    INDEX idx_pir_avalible (avalible),
    CHECK (end_date IS NULL OR start_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item替代品、同等品、配件與推薦關聯';

CREATE TABLE IF NOT EXISTS pim_application_scope (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '適用範圍序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    item_nid                BIGINT UNSIGNED                     NOT NULL COMMENT 'Item流水號',
    scope_type              VARCHAR(30)                         NOT NULL COMMENT 'CHANNEL通路;STORE商店;PROJECT專案;SITE工地;WBS工項;REGION區域;CUSTOMER客戶;CUSTOM自訂',
    scope_sid               VARCHAR(32)                         NOT NULL COMMENT '適用範圍序號',
    allowed_mark            TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否允許使用',
    start_date              DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '生效時間',
    end_date                DATETIME                                NULL COMMENT '失效時間',
    approval_status         VARCHAR(20)                         NOT NULL DEFAULT 'APPROVED' COMMENT 'PENDING待審;APPROVED核准;REJECTED拒絕;EXPIRED過期',
    workflow_instance_sid   VARCHAR(32)                             NULL COMMENT 'WorkflowDB流程實例序號',
    CONSTRAINT fk_pas_item
        FOREIGN KEY (item_nid) REFERENCES pim_item(nid),
    CONSTRAINT uk_pas_item_scope UNIQUE (item_nid, scope_type, scope_sid),
    INDEX idx_pas_item_nid (item_nid),
    INDEX idx_pas_scope (scope_type, scope_sid),
    INDEX idx_pas_allowed_mark (allowed_mark),
    INDEX idx_pas_status (approval_status),
    INDEX idx_pas_workflow_instance_sid (workflow_instance_sid),
    CHECK (end_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item通路、商店、專案、工地與工項適用範圍';

-- =========================================================
-- 08. 組合與BOM
-- =========================================================

CREATE TABLE IF NOT EXISTS pim_structure (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Item結構序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    item_nid                BIGINT UNSIGNED                     NOT NULL COMMENT '父Item流水號',
    structure_type          VARCHAR(30)                         NOT NULL COMMENT 'BUNDLE銷售組合;KIT套件;BOM物料清單;ASSEMBLY組裝;WORK_PACKAGE工項組合',
    structure_version       INT                                 NOT NULL DEFAULT 1 COMMENT '結構版本',
    inventory_policy        VARCHAR(30)                         NOT NULL DEFAULT 'COMPONENT' COMMENT 'COMPONENT依組件;PARENT依父項;NONE不控庫存',
    pricing_policy          VARCHAR(30)                         NOT NULL DEFAULT 'PARENT_PRICE' COMMENT 'PARENT_PRICE父項價;SUM_COMPONENT組件加總;CUSTOM自訂',
    effective_start_date    DATETIME                                NULL COMMENT '生效開始時間',
    effective_end_date      DATETIME                                NULL COMMENT '失效時間',
    structure_status        VARCHAR(20)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;REVIEW待審;APPROVED核准;ACTIVE啟用;INACTIVE停用',
    workflow_instance_sid   VARCHAR(32)                             NULL COMMENT 'WorkflowDB流程實例序號',
    CONSTRAINT fk_pst_item
        FOREIGN KEY (item_nid) REFERENCES pim_item(nid),
    CONSTRAINT uk_pst_item_type_version UNIQUE (item_nid, structure_type, structure_version),
    INDEX idx_pst_item_nid (item_nid),
    INDEX idx_pst_structure_type (structure_type),
    INDEX idx_pst_status (structure_status),
    INDEX idx_pst_workflow_instance_sid (workflow_instance_sid),
    CHECK (structure_version > 0),
    CHECK (effective_end_date IS NULL OR effective_start_date IS NULL OR effective_end_date >= effective_start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item組合、套件與BOM結構';

CREATE TABLE IF NOT EXISTS pim_structure_component (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '結構組件序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    structure_nid           BIGINT UNSIGNED                     NOT NULL COMMENT 'Item結構流水號',
    component_item_sid      VARCHAR(32)                         NOT NULL COMMENT '組件Item序號',
    component_variant_sid   VARCHAR(32)                             NULL COMMENT '組件變體序號',
    quantity                DECIMAL(20,6)                       NOT NULL DEFAULT 1 COMMENT '組件數量',
    unit_sid                VARCHAR(32)                             NULL COMMENT 'MasterDB單位序號',
    waste_rate              DECIMAL(8,4)                        NOT NULL DEFAULT 0 COMMENT '耗損率百分比',
    required_mark           TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否必選',
    selection_group         VARCHAR(100)                            NULL COMMENT '選配群組',
    minimum_select_qty      DECIMAL(20,6)                           NULL COMMENT '最少選擇數量',
    maximum_select_qty      DECIMAL(20,6)                           NULL COMMENT '最多選擇數量',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    CONSTRAINT fk_psc_structure
        FOREIGN KEY (structure_nid) REFERENCES pim_structure(nid),
    CONSTRAINT uk_psc_component UNIQUE (structure_nid, component_item_sid, component_variant_sid),
    INDEX idx_psc_structure_nid (structure_nid),
    INDEX idx_psc_component_item_sid (component_item_sid),
    INDEX idx_psc_component_variant_sid (component_variant_sid),
    INDEX idx_psc_selection_group (selection_group),
    CHECK (quantity > 0),
    CHECK (waste_rate >= 0),
    CHECK (minimum_select_qty IS NULL OR minimum_select_qty >= 0),
    CHECK (maximum_select_qty IS NULL OR maximum_select_qty > 0),
    CHECK (maximum_select_qty IS NULL OR minimum_select_qty IS NULL OR maximum_select_qty >= minimum_select_qty)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item組合與BOM組件';

-- =========================================================
-- 09. 版本、審核與發布
-- =========================================================

CREATE TABLE IF NOT EXISTS pim_item_version (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Item版本序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    item_nid                BIGINT UNSIGNED                     NOT NULL COMMENT 'Item流水號',
    version_no              INT                                 NOT NULL COMMENT '版本號',
    version_name            VARCHAR(200)                            NULL COMMENT '版本名稱',
    snapshot_data           JSON                                NOT NULL COMMENT 'Item完整快照',
    change_type             VARCHAR(30)                         NOT NULL COMMENT 'CREATE新增;UPDATE修改;APPROVE核准;PUBLISH發布;ROLLBACK回滾;MERGE合併',
    change_summary          VARCHAR(1000)                           NULL COMMENT '變更摘要',
    version_status          VARCHAR(20)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;REVIEW待審;APPROVED核准;PUBLISHED已發布;REPLACED已取代;ROLLED_BACK已回滾',
    create_user_sid         VARCHAR(32)                             NULL COMMENT '建立人員序號',
    approved_user_sid       VARCHAR(32)                             NULL COMMENT '核准人員序號',
    approved_date           DATETIME                                NULL COMMENT '核准時間',
    published_date          DATETIME                                NULL COMMENT '發布時間',
    workflow_instance_sid   VARCHAR(32)                             NULL COMMENT 'WorkflowDB流程實例序號',
    CONSTRAINT fk_piv_item
        FOREIGN KEY (item_nid) REFERENCES pim_item(nid),
    CONSTRAINT uk_piv_item_version UNIQUE (item_nid, version_no),
    INDEX idx_piv_item_nid (item_nid),
    INDEX idx_piv_change_type (change_type),
    INDEX idx_piv_status (version_status),
    INDEX idx_piv_workflow_instance_sid (workflow_instance_sid),
    INDEX idx_piv_create_date (create_date),
    CHECK (version_no > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item內容版本';

CREATE TABLE IF NOT EXISTS pim_publish_target (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '發布目標序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    target_code             VARCHAR(80)                         NOT NULL COMMENT '發布目標代碼',
    target_name             VARCHAR(150)                        NOT NULL COMMENT '發布目標名稱',
    target_type             VARCHAR(30)                         NOT NULL COMMENT 'SHOPPING商城;APP;POS;MARKETPLACE外部平台;PROJECT專案;SITE工地;OWNER_PORTAL業主平台;INTERNAL內部',
    service_code            VARCHAR(80)                             NULL COMMENT '負責服務代碼',
    default_language_sid    VARCHAR(32)                             NULL COMMENT 'MasterDB預設語系序號',
    required_fields         JSON                                    NULL COMMENT '必填欄位',
    validation_rules        JSON                                    NULL COMMENT '驗證規則',
    target_status           VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;PAUSED暫停;DISABLED停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ppt_target_code UNIQUE (target_code),
    INDEX idx_ppt_target_type (target_type),
    INDEX idx_ppt_service_code (service_code),
    INDEX idx_ppt_status (target_status),
    INDEX idx_ppt_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item發布目標';

CREATE TABLE IF NOT EXISTS pim_target_item (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '目標Item序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    publish_target_nid      BIGINT UNSIGNED                     NOT NULL COMMENT '發布目標流水號',
    item_nid                BIGINT UNSIGNED                     NOT NULL COMMENT 'Item流水號',
    external_item_id        VARCHAR(200)                            NULL COMMENT '外部目標Item ID',
    external_item_no        VARCHAR(200)                            NULL COMMENT '外部目標Item編號',
    published_version_no    INT                                     NULL COMMENT '已發布版本號',
    desired_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE預期啟用;INACTIVE預期停用',
    actual_status           VARCHAR(20)                         NOT NULL DEFAULT 'UNKNOWN' COMMENT 'ACTIVE啟用;INACTIVE停用;UNKNOWN未知',
    publish_status          VARCHAR(30)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;READY待發布;PUBLISHING發布中;PUBLISHED已發布;UPDATE_PENDING待更新;FAILED失敗',
    scheduled_publish_date  DATETIME                                NULL COMMENT '預定發布時間',
    last_publish_date       DATETIME                                NULL COMMENT '最後發布時間',
    last_sync_date          DATETIME                                NULL COMMENT '最後同步時間',
    validation_result       JSON                                    NULL COMMENT '驗證結果',
    last_error              LONGTEXT                                NULL COMMENT '最後錯誤',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    CONSTRAINT fk_pti_target
        FOREIGN KEY (publish_target_nid) REFERENCES pim_publish_target(nid),
    CONSTRAINT fk_pti_item
        FOREIGN KEY (item_nid) REFERENCES pim_item(nid),
    CONSTRAINT uk_pti_target_item UNIQUE (publish_target_nid, item_nid),
    INDEX idx_pti_target_nid (publish_target_nid),
    INDEX idx_pti_item_nid (item_nid),
    INDEX idx_pti_external_item_id (external_item_id),
    INDEX idx_pti_publish_status (publish_status),
    INDEX idx_pti_scheduled_publish_date (scheduled_publish_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item發布目標狀態';

CREATE TABLE IF NOT EXISTS pim_publish_job (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '發布工作序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    job_no                  VARCHAR(100)                        NOT NULL COMMENT '發布工作編號',
    target_item_nid         BIGINT UNSIGNED                     NOT NULL COMMENT '目標Item流水號',
    operation_type          VARCHAR(30)                         NOT NULL COMMENT 'CREATE新增;UPDATE更新;ACTIVATE啟用;DEACTIVATE停用;DELETE刪除;SYNC同步',
    item_version_no         INT                                 NOT NULL COMMENT '發布Item版本號',
    request_payload         JSON                                    NULL COMMENT '送出內容',
    response_payload        JSON                                    NULL COMMENT '回應內容',
    publish_status          VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;PROCESSING處理中;SUCCESS成功;FAILED失敗;RETRY重試;DEAD死信',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '重試次數',
    max_retry_count         INT                                 NOT NULL DEFAULT 5 COMMENT '最大重試次數',
    next_retry_date         DATETIME                                NULL COMMENT '下次重試時間',
    external_reference_no   VARCHAR(200)                            NULL COMMENT '外部回應編號',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    error_code              VARCHAR(100)                            NULL COMMENT '錯誤代碼',
    error_message           LONGTEXT                                NULL COMMENT '錯誤訊息',
    correlation_id          VARCHAR(100)                            NULL COMMENT '跨服務關聯識別碼',
    CONSTRAINT fk_ppj_target_item
        FOREIGN KEY (target_item_nid) REFERENCES pim_target_item(nid),
    CONSTRAINT uk_ppj_job_no UNIQUE (job_no),
    INDEX idx_ppj_target_item_nid (target_item_nid),
    INDEX idx_ppj_operation_type (operation_type),
    INDEX idx_ppj_status (publish_status),
    INDEX idx_ppj_next_retry_date (next_retry_date),
    INDEX idx_ppj_correlation_id (correlation_id),
    CHECK (item_version_no > 0),
    CHECK (retry_count >= 0),
    CHECK (max_retry_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item發布工作';

-- =========================================================
-- 10. 匯入與資料品質
-- =========================================================

CREATE TABLE IF NOT EXISTS pim_import_job (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '匯入工作序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    job_no                  VARCHAR(100)                        NOT NULL COMMENT '匯入工作編號',
    import_type             VARCHAR(30)                         NOT NULL COMMENT 'ITEM;VARIANT;ATTRIBUTE;MEDIA;CERTIFICATION;TRANSLATION;FULL',
    source_type             VARCHAR(30)                         NOT NULL COMMENT 'FILE檔案;API;SUPPLIER供應商;MANUFACTURER製造商;MIGRATION移轉',
    source_file_sid         VARCHAR(32)                             NULL COMMENT 'FileDB來源檔案序號',
    source_party_sid        VARCHAR(32)                             NULL COMMENT '來源Party序號',
    mapping_config          JSON                                    NULL COMMENT '欄位對應設定',
    import_status           VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;VALIDATING驗證中;IMPORTING匯入中;SUCCESS成功;PARTIAL部分成功;FAILED失敗',
    total_count             INT                                 NOT NULL DEFAULT 0 COMMENT '總筆數',
    success_count           INT                                 NOT NULL DEFAULT 0 COMMENT '成功筆數',
    fail_count              INT                                 NOT NULL DEFAULT 0 COMMENT '失敗筆數',
    skip_count              INT                                 NOT NULL DEFAULT 0 COMMENT '略過筆數',
    start_date              DATETIME                                NULL COMMENT '開始時間',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    create_user_sid         VARCHAR(32)                             NULL COMMENT '建立人員序號',
    error_message           LONGTEXT                                NULL COMMENT '錯誤訊息',
    CONSTRAINT uk_pij_job_no UNIQUE (job_no),
    INDEX idx_pij_import_type (import_type),
    INDEX idx_pij_source_type (source_type),
    INDEX idx_pij_source_file_sid (source_file_sid),
    INDEX idx_pij_source_party_sid (source_party_sid),
    INDEX idx_pij_status (import_status),
    CHECK (total_count >= 0),
    CHECK (success_count >= 0),
    CHECK (fail_count >= 0),
    CHECK (skip_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item資料匯入工作';

CREATE TABLE IF NOT EXISTS pim_import_error (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '匯入錯誤序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    import_job_nid          BIGINT UNSIGNED                     NOT NULL COMMENT '匯入工作流水號',
    row_no                  INT                                     NULL COMMENT '來源資料列號',
    source_identifier       VARCHAR(200)                            NULL COMMENT '來源識別碼',
    item_no                 VARCHAR(100)                            NULL COMMENT 'Item編號',
    variant_no              VARCHAR(100)                            NULL COMMENT '變體編號',
    error_code              VARCHAR(100)                        NOT NULL COMMENT '錯誤代碼',
    error_field             VARCHAR(200)                            NULL COMMENT '錯誤欄位',
    error_message           LONGTEXT                            NOT NULL COMMENT '錯誤訊息',
    source_data             JSON                                    NULL COMMENT '來源資料',
    error_status            VARCHAR(20)                         NOT NULL DEFAULT 'OPEN' COMMENT 'OPEN未處理;CORRECTED已修正;IGNORED忽略;RETRIED已重試',
    resolved_user_sid       VARCHAR(32)                             NULL COMMENT '處理人員序號',
    resolved_date           DATETIME                                NULL COMMENT '處理時間',
    CONSTRAINT fk_pie_import_job
        FOREIGN KEY (import_job_nid) REFERENCES pim_import_job(nid),
    INDEX idx_pie_import_job_nid (import_job_nid),
    INDEX idx_pie_item_no (item_no),
    INDEX idx_pie_variant_no (variant_no),
    INDEX idx_pie_error_code (error_code),
    INDEX idx_pie_status (error_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item匯入錯誤';

CREATE TABLE IF NOT EXISTS pim_quality_rule (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '資料品質規則序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    rule_code               VARCHAR(100)                        NOT NULL COMMENT '規則代碼',
    rule_name               VARCHAR(200)                        NOT NULL COMMENT '規則名稱',
    applies_to              VARCHAR(30)                         NOT NULL COMMENT 'ITEM;VARIANT;TRANSLATION;MEDIA;CERTIFICATION;TARGET',
    item_type               VARCHAR(30)                             NULL COMMENT '限定Item類型',
    category_sid            VARCHAR(32)                             NULL COMMENT '限定分類序號',
    severity                VARCHAR(20)                         NOT NULL DEFAULT 'WARNING' COMMENT 'INFO資訊;WARNING警告;ERROR錯誤;CRITICAL嚴重',
    rule_expression         TEXT                                NOT NULL COMMENT '規則運算式',
    rule_config             JSON                                    NULL COMMENT '規則設定',
    score_weight            DECIMAL(8,4)                        NOT NULL DEFAULT 1 COMMENT '完整度計分權重',
    blocking_mark           TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否阻止核准或發布',
    rule_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;PAUSED暫停;DISABLED停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_pqr_rule_code UNIQUE (rule_code),
    INDEX idx_pqr_applies_to (applies_to),
    INDEX idx_pqr_item_type (item_type),
    INDEX idx_pqr_category_sid (category_sid),
    INDEX idx_pqr_severity (severity),
    INDEX idx_pqr_blocking_mark (blocking_mark),
    INDEX idx_pqr_status (rule_status),
    INDEX idx_pqr_avalible (avalible),
    CHECK (score_weight >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item資料品質規則';

CREATE TABLE IF NOT EXISTS pim_quality_result (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '品質檢查結果序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    item_sid                VARCHAR(32)                         NOT NULL COMMENT 'Item序號',
    variant_sid             VARCHAR(32)                             NULL COMMENT '變體序號',
    target_sid              VARCHAR(32)                             NULL COMMENT '發布目標序號',
    quality_rule_sid        VARCHAR(32)                         NOT NULL COMMENT '品質規則序號',
    check_result            VARCHAR(20)                         NOT NULL COMMENT 'PASS通過;WARNING警告;FAIL失敗',
    score                   DECIMAL(8,4)                        NOT NULL DEFAULT 0 COMMENT '得分',
    message                 VARCHAR(1000)                           NULL COMMENT '檢查訊息',
    details_json            JSON                                    NULL COMMENT '檢查明細',
    resolved                TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否已解決',
    resolved_user_sid       VARCHAR(32)                             NULL COMMENT '處理人員序號',
    resolved_date           DATETIME                                NULL COMMENT '處理時間',
    INDEX idx_pqres_item_sid (item_sid),
    INDEX idx_pqres_variant_sid (variant_sid),
    INDEX idx_pqres_target_sid (target_sid),
    INDEX idx_pqres_quality_rule_sid (quality_rule_sid),
    INDEX idx_pqres_check_result (check_result),
    INDEX idx_pqres_resolved (resolved),
    INDEX idx_pqres_create_date (create_date),
    CHECK (score >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Item資料品質檢查結果';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
