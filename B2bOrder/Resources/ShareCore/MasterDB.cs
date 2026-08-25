namespace B2bOrder.Resources.ShareCore
{
    /// <summary>
    /// MasterDB V2 Shared Core Schema
    /// 設計目標：
    /// 1. 同時支援 Shopping Platform 與未來 Construction ERP
    /// 2. 採產業中立命名，避免使用 mall、store、project-specific 等限定語意
    /// 3. nid：資料庫內部主鍵；sid：跨服務/API 對外識別碼
    /// 4. 所有跨資料庫關聯一律使用 sid，不建立跨資料庫 Foreign Key
    /// 5. avalible：Y可用；W停用；D刪除
    /// 6. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class MasterDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 國家、行政區、地址
-- =========================================================

CREATE TABLE IF NOT EXISTS mst_country (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '國家序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    country_code            VARCHAR(10)                         NOT NULL COMMENT '國家代碼',
    country_name            VARCHAR(100)                        NOT NULL COMMENT '國家名稱',
    country_english_name    VARCHAR(150)                            NULL COMMENT '國家英文名稱',
    iso_alpha2              VARCHAR(2)                              NULL COMMENT 'ISO Alpha-2',
    iso_alpha3              VARCHAR(3)                              NULL COMMENT 'ISO Alpha-3',
    calling_code            VARCHAR(20)                             NULL COMMENT '國際電話碼',
    default_currency_sid    VARCHAR(32)                             NULL COMMENT '預設幣別序號',
    default_language_sid    VARCHAR(32)                             NULL COMMENT '預設語系序號',
    timezone_code           VARCHAR(100)                            NULL COMMENT '預設時區',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mc_country_code UNIQUE (country_code),
    UNIQUE KEY uk_mc_iso_alpha2 (iso_alpha2),
    UNIQUE KEY uk_mc_iso_alpha3 (iso_alpha3),
    INDEX idx_mc_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='國家主檔';

CREATE TABLE IF NOT EXISTS mst_region (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '行政區序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    country_sid             VARCHAR(32)                         NOT NULL COMMENT '國家序號',
    parent_sid              VARCHAR(32)                             NULL COMMENT '上層行政區序號',
    region_code             VARCHAR(50)                         NOT NULL COMMENT '行政區代碼',
    region_name             VARCHAR(150)                        NOT NULL COMMENT '行政區名稱',
    region_english_name     VARCHAR(150)                            NULL COMMENT '行政區英文名稱',
    region_type             VARCHAR(30)                         NOT NULL COMMENT 'STATE州省;CITY縣市;DISTRICT行政區;TOWNSHIP鄉鎮;VILLAGE村里',
    postal_code_pattern     VARCHAR(100)                            NULL COMMENT '郵遞區號格式',
    level_no                INT                                 NOT NULL DEFAULT 1 COMMENT '階層',
    tree_path               VARCHAR(1000)                           NULL COMMENT '樹狀路徑',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mr_country_code UNIQUE (country_sid, region_code),
    INDEX idx_mr_country_sid (country_sid),
    INDEX idx_mr_parent_sid (parent_sid),
    INDEX idx_mr_region_type (region_type),
    INDEX idx_mr_avalible (avalible),
    CHECK (level_no > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='行政區主檔';

CREATE TABLE IF NOT EXISTS mst_address (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '地址序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    owner_type              VARCHAR(50)                         NOT NULL COMMENT 'COMPANY;BRANCH;DEPARTMENT;CUSTOMER;SUPPLIER;WAREHOUSE;SITE;OTHER',
    owner_sid               VARCHAR(32)                         NOT NULL COMMENT '所屬資料序號',
    address_type            VARCHAR(30)                         NOT NULL COMMENT 'REGISTERED登記;CONTACT聯絡;BILLING帳單;SHIPPING配送;SITE現場;OTHER',
    country_sid             VARCHAR(32)                         NOT NULL COMMENT '國家序號',
    region_level1_sid       VARCHAR(32)                             NULL COMMENT '第一層行政區序號',
    region_level2_sid       VARCHAR(32)                             NULL COMMENT '第二層行政區序號',
    region_level3_sid       VARCHAR(32)                             NULL COMMENT '第三層行政區序號',
    postal_code             VARCHAR(20)                             NULL COMMENT '郵遞區號',
    address_line1           VARCHAR(500)                        NOT NULL COMMENT '主要地址',
    address_line2           VARCHAR(500)                            NULL COMMENT '補充地址',
    contact_name            VARCHAR(100)                            NULL COMMENT '聯絡人',
    contact_phone           VARCHAR(50)                             NULL COMMENT '聯絡電話',
    latitude                DECIMAL(12,8)                           NULL COMMENT '緯度',
    longitude               DECIMAL(12,8)                           NULL COMMENT '經度',
    is_default              TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否預設',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ma_owner_type UNIQUE (owner_type, owner_sid, address_type, sid),
    INDEX idx_ma_owner (owner_type, owner_sid),
    INDEX idx_ma_country_sid (country_sid),
    INDEX idx_ma_region1_sid (region_level1_sid),
    INDEX idx_ma_region2_sid (region_level2_sid),
    INDEX idx_ma_region3_sid (region_level3_sid),
    INDEX idx_ma_default (is_default),
    INDEX idx_ma_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='共用地址主檔';

-- =========================================================
-- 02. 語系、幣別、匯率
-- =========================================================

CREATE TABLE IF NOT EXISTS mst_language (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '語系序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    language_code           VARCHAR(20)                         NOT NULL COMMENT '語系代碼',
    language_name           VARCHAR(100)                        NOT NULL COMMENT '語系名稱',
    locale_code             VARCHAR(30)                         NOT NULL COMMENT 'Locale代碼',
    date_format             VARCHAR(50)                             NULL COMMENT '日期格式',
    time_format             VARCHAR(50)                             NULL COMMENT '時間格式',
    number_format           VARCHAR(50)                             NULL COMMENT '數字格式',
    rtl_mark                TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否由右至左',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_ml_language_code UNIQUE (language_code),
    CONSTRAINT uk_ml_locale_code UNIQUE (locale_code),
    INDEX idx_ml_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='語系主檔';

CREATE TABLE IF NOT EXISTS mst_currency (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '幣別序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    currency_code           VARCHAR(10)                         NOT NULL COMMENT '幣別代碼',
    currency_name           VARCHAR(100)                        NOT NULL COMMENT '幣別名稱',
    currency_symbol         VARCHAR(20)                             NULL COMMENT '幣別符號',
    decimal_places          INT                                 NOT NULL DEFAULT 2 COMMENT '小數位數',
    rounding_method         VARCHAR(20)                         NOT NULL DEFAULT 'HALF_UP' COMMENT 'HALF_UP;HALF_EVEN;UP;DOWN',
    rounding_unit           DECIMAL(20,10)                      NOT NULL DEFAULT 0.01 COMMENT '最小進位單位',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mcu_currency_code UNIQUE (currency_code),
    INDEX idx_mcu_avalible (avalible),
    CHECK (decimal_places >= 0),
    CHECK (rounding_unit > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='幣別主檔';

CREATE TABLE IF NOT EXISTS mst_exchange_rate (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '匯率序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    base_currency_sid       VARCHAR(32)                         NOT NULL COMMENT '基準幣別序號',
    quote_currency_sid      VARCHAR(32)                         NOT NULL COMMENT '報價幣別序號',
    rate_date               DATE                                NOT NULL COMMENT '匯率日期',
    rate_type               VARCHAR(30)                         NOT NULL DEFAULT 'STANDARD' COMMENT 'STANDARD標準;BUY買入;SELL賣出;CLOSING期末;CUSTOM自訂',
    exchange_rate           DECIMAL(20,10)                      NOT NULL COMMENT '匯率',
    source_code             VARCHAR(100)                            NULL COMMENT '匯率來源代碼',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mer_rate UNIQUE (base_currency_sid, quote_currency_sid, rate_date, rate_type),
    INDEX idx_mer_base_currency_sid (base_currency_sid),
    INDEX idx_mer_quote_currency_sid (quote_currency_sid),
    INDEX idx_mer_rate_date (rate_date),
    INDEX idx_mer_avalible (avalible),
    CHECK (base_currency_sid <> quote_currency_sid),
    CHECK (exchange_rate > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='匯率主檔';

-- =========================================================
-- 03. 公司、據點、組織
-- =========================================================

CREATE TABLE IF NOT EXISTS mst_company (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '公司序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_code            VARCHAR(50)                         NOT NULL COMMENT '公司代碼',
    company_name            VARCHAR(200)                        NOT NULL COMMENT '公司名稱',
    company_short_name      VARCHAR(100)                            NULL COMMENT '公司簡稱',
    tax_no                  VARCHAR(30)                             NULL COMMENT '稅籍編號或統一編號',
    registration_no         VARCHAR(100)                            NULL COMMENT '公司登記號碼',
    country_sid             VARCHAR(32)                         NOT NULL COMMENT '國家序號',
    base_currency_sid       VARCHAR(32)                         NOT NULL COMMENT '本位幣序號',
    default_language_sid    VARCHAR(32)                         NOT NULL COMMENT '預設語系序號',
    timezone_code           VARCHAR(100)                        NOT NULL DEFAULT 'Asia/Taipei' COMMENT '預設時區',
    fiscal_year_start_month INT                                 NOT NULL DEFAULT 1 COMMENT '會計年度起始月份',
    legal_representative    VARCHAR(100)                            NULL COMMENT '法定代表人',
    phone                   VARCHAR(50)                             NULL COMMENT '電話',
    email                   VARCHAR(200)                            NULL COMMENT 'Email',
    website                 VARCHAR(500)                            NULL COMMENT '網站',
    company_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE營運中;INACTIVE停用;CLOSED結束',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mco_company_code UNIQUE (company_code),
    UNIQUE KEY uk_mco_tax_no (tax_no),
    INDEX idx_mco_country_sid (country_sid),
    INDEX idx_mco_base_currency_sid (base_currency_sid),
    INDEX idx_mco_status (company_status),
    INDEX idx_mco_avalible (avalible),
    CHECK (fiscal_year_start_month BETWEEN 1 AND 12)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='公司主檔';

CREATE TABLE IF NOT EXISTS mst_business_unit (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '營運單位序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    parent_sid              VARCHAR(32)                             NULL COMMENT '上層營運單位序號',
    unit_code               VARCHAR(50)                         NOT NULL COMMENT '營運單位代碼',
    unit_name               VARCHAR(200)                        NOT NULL COMMENT '營運單位名稱',
    unit_type               VARCHAR(30)                         NOT NULL COMMENT 'HEADQUARTER總部;BRANCH分公司;DIVISION事業部;SITE據點;PROJECT_OFFICE專案辦公室;OTHER',
    manager_employee_sid    VARCHAR(32)                             NULL COMMENT '負責人員工序號',
    profit_center_sid       VARCHAR(32)                             NULL COMMENT '利潤中心序號',
    cost_center_sid         VARCHAR(32)                             NULL COMMENT '成本中心序號',
    level_no                INT                                 NOT NULL DEFAULT 1 COMMENT '階層',
    tree_path               VARCHAR(1000)                           NULL COMMENT '樹狀路徑',
    start_date              DATE                                    NULL COMMENT '啟用日期',
    end_date                DATE                                    NULL COMMENT '結束日期',
    unit_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用;CLOSED結束',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mbu_company_unit_code UNIQUE (company_sid, unit_code),
    INDEX idx_mbu_company_sid (company_sid),
    INDEX idx_mbu_parent_sid (parent_sid),
    INDEX idx_mbu_unit_type (unit_type),
    INDEX idx_mbu_manager_employee_sid (manager_employee_sid),
    INDEX idx_mbu_status (unit_status),
    INDEX idx_mbu_avalible (avalible),
    CHECK (level_no > 0),
    CHECK (end_date IS NULL OR start_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='營運單位與據點主檔';

CREATE TABLE IF NOT EXISTS mst_department (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '部門序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    business_unit_sid       VARCHAR(32)                             NULL COMMENT '營運單位序號',
    parent_sid              VARCHAR(32)                             NULL COMMENT '上層部門序號',
    department_code         VARCHAR(50)                         NOT NULL COMMENT '部門代碼',
    department_name         VARCHAR(200)                        NOT NULL COMMENT '部門名稱',
    manager_employee_sid    VARCHAR(32)                             NULL COMMENT '主管員工序號',
    cost_center_sid         VARCHAR(32)                             NULL COMMENT '成本中心序號',
    level_no                INT                                 NOT NULL DEFAULT 1 COMMENT '階層',
    tree_path               VARCHAR(1000)                           NULL COMMENT '樹狀路徑',
    department_status       VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用;CLOSED結束',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_md_company_department_code UNIQUE (company_sid, department_code),
    INDEX idx_md_company_sid (company_sid),
    INDEX idx_md_business_unit_sid (business_unit_sid),
    INDEX idx_md_parent_sid (parent_sid),
    INDEX idx_md_manager_employee_sid (manager_employee_sid),
    INDEX idx_md_status (department_status),
    INDEX idx_md_avalible (avalible),
    CHECK (level_no > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='部門主檔';

CREATE TABLE IF NOT EXISTS mst_position (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '職務序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    position_code           VARCHAR(50)                         NOT NULL COMMENT '職務代碼',
    position_name           VARCHAR(150)                        NOT NULL COMMENT '職務名稱',
    position_level          INT                                 NOT NULL DEFAULT 1 COMMENT '職務層級',
    management_mark         TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否管理職',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mp_position_code UNIQUE (position_code),
    INDEX idx_mp_position_level (position_level),
    INDEX idx_mp_management_mark (management_mark),
    INDEX idx_mp_avalible (avalible),
    CHECK (position_level > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='職務主檔';

-- =========================================================
-- 04. 成本中心、利潤中心、專案
-- =========================================================

CREATE TABLE IF NOT EXISTS mst_cost_center (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '成本中心序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    parent_sid              VARCHAR(32)                             NULL COMMENT '上層成本中心序號',
    cost_center_code        VARCHAR(50)                         NOT NULL COMMENT '成本中心代碼',
    cost_center_name        VARCHAR(200)                        NOT NULL COMMENT '成本中心名稱',
    manager_employee_sid    VARCHAR(32)                             NULL COMMENT '負責人員工序號',
    level_no                INT                                 NOT NULL DEFAULT 1 COMMENT '階層',
    tree_path               VARCHAR(1000)                           NULL COMMENT '樹狀路徑',
    start_date              DATE                                    NULL COMMENT '生效日',
    end_date                DATE                                    NULL COMMENT '失效日',
    center_status           VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用;CLOSED關閉',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mcc_company_code UNIQUE (company_sid, cost_center_code),
    INDEX idx_mcc_company_sid (company_sid),
    INDEX idx_mcc_parent_sid (parent_sid),
    INDEX idx_mcc_manager_employee_sid (manager_employee_sid),
    INDEX idx_mcc_status (center_status),
    INDEX idx_mcc_avalible (avalible),
    CHECK (level_no > 0),
    CHECK (end_date IS NULL OR start_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='成本中心主檔';

CREATE TABLE IF NOT EXISTS mst_profit_center (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '利潤中心序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    parent_sid              VARCHAR(32)                             NULL COMMENT '上層利潤中心序號',
    profit_center_code      VARCHAR(50)                         NOT NULL COMMENT '利潤中心代碼',
    profit_center_name      VARCHAR(200)                        NOT NULL COMMENT '利潤中心名稱',
    manager_employee_sid    VARCHAR(32)                             NULL COMMENT '負責人員工序號',
    level_no                INT                                 NOT NULL DEFAULT 1 COMMENT '階層',
    tree_path               VARCHAR(1000)                           NULL COMMENT '樹狀路徑',
    start_date              DATE                                    NULL COMMENT '生效日',
    end_date                DATE                                    NULL COMMENT '失效日',
    center_status           VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用;CLOSED關閉',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mpc_company_code UNIQUE (company_sid, profit_center_code),
    INDEX idx_mpc_company_sid (company_sid),
    INDEX idx_mpc_parent_sid (parent_sid),
    INDEX idx_mpc_manager_employee_sid (manager_employee_sid),
    INDEX idx_mpc_status (center_status),
    INDEX idx_mpc_avalible (avalible),
    CHECK (level_no > 0),
    CHECK (end_date IS NULL OR start_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='利潤中心主檔';

CREATE TABLE IF NOT EXISTS mst_project (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '共用專案序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    project_code            VARCHAR(80)                         NOT NULL COMMENT '專案代碼',
    project_name            VARCHAR(300)                        NOT NULL COMMENT '專案名稱',
    project_type            VARCHAR(50)                         NOT NULL COMMENT 'ECOMMERCE電商;CONSTRUCTION建築;IT資訊;MARKETING行銷;INTERNAL內部;OTHER',
    customer_sid            VARCHAR(32)                             NULL COMMENT '客戶序號',
    business_unit_sid       VARCHAR(32)                             NULL COMMENT '營運單位序號',
    department_sid          VARCHAR(32)                             NULL COMMENT '主責部門序號',
    project_manager_sid     VARCHAR(32)                             NULL COMMENT '專案經理員工序號',
    cost_center_sid         VARCHAR(32)                             NULL COMMENT '成本中心序號',
    profit_center_sid       VARCHAR(32)                             NULL COMMENT '利潤中心序號',
    start_date              DATE                                    NULL COMMENT '開始日期',
    planned_end_date        DATE                                    NULL COMMENT '預計結束日期',
    actual_end_date         DATE                                    NULL COMMENT '實際結束日期',
    base_currency_sid       VARCHAR(32)                             NULL COMMENT '專案幣別序號',
    project_status          VARCHAR(30)                         NOT NULL DEFAULT 'PLANNING' COMMENT 'PLANNING規劃;ACTIVE執行中;ON_HOLD暫停;COMPLETED完成;CANCELLED取消;CLOSED結案',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mpr_company_project_code UNIQUE (company_sid, project_code),
    INDEX idx_mpr_company_sid (company_sid),
    INDEX idx_mpr_project_type (project_type),
    INDEX idx_mpr_customer_sid (customer_sid),
    INDEX idx_mpr_business_unit_sid (business_unit_sid),
    INDEX idx_mpr_department_sid (department_sid),
    INDEX idx_mpr_project_manager_sid (project_manager_sid),
    INDEX idx_mpr_cost_center_sid (cost_center_sid),
    INDEX idx_mpr_profit_center_sid (profit_center_sid),
    INDEX idx_mpr_status (project_status),
    INDEX idx_mpr_avalible (avalible),
    CHECK (planned_end_date IS NULL OR start_date IS NULL OR planned_end_date >= start_date),
    CHECK (actual_end_date IS NULL OR start_date IS NULL OR actual_end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='跨產業共用專案主檔';

-- =========================================================
-- 05. 單位、稅別、付款條件、付款方式
-- =========================================================

CREATE TABLE IF NOT EXISTS mst_unit (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '計量單位序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    unit_code               VARCHAR(30)                         NOT NULL COMMENT '單位代碼',
    unit_name               VARCHAR(100)                        NOT NULL COMMENT '單位名稱',
    unit_category           VARCHAR(30)                         NOT NULL COMMENT 'QUANTITY數量;WEIGHT重量;LENGTH長度;AREA面積;VOLUME體積;TIME時間;OTHER',
    base_unit_sid           VARCHAR(32)                             NULL COMMENT '基準單位序號',
    conversion_rate         DECIMAL(30,10)                      NOT NULL DEFAULT 1 COMMENT '換算倍率',
    decimal_places          INT                                 NOT NULL DEFAULT 6 COMMENT '允許小數位數',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mu_unit_code UNIQUE (unit_code),
    INDEX idx_mu_unit_category (unit_category),
    INDEX idx_mu_base_unit_sid (base_unit_sid),
    INDEX idx_mu_avalible (avalible),
    CHECK (conversion_rate > 0),
    CHECK (decimal_places >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='計量單位主檔';

CREATE TABLE IF NOT EXISTS mst_tax (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '稅別序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    country_sid             VARCHAR(32)                         NOT NULL COMMENT '國家序號',
    tax_code                VARCHAR(50)                         NOT NULL COMMENT '稅別代碼',
    tax_name                VARCHAR(150)                        NOT NULL COMMENT '稅別名稱',
    tax_type                VARCHAR(30)                         NOT NULL COMMENT 'SALES銷項;PURCHASE進項;WITHHOLDING扣繳;DUTY關稅;OTHER',
    tax_rate                DECIMAL(8,4)                        NOT NULL DEFAULT 0 COMMENT '稅率百分比',
    included_in_price       TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '價格是否含稅',
    deductible_mark         TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否可扣抵',
    start_date              DATE                                    NULL COMMENT '生效日',
    end_date                DATE                                    NULL COMMENT '失效日',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mt_country_tax_code UNIQUE (country_sid, tax_code),
    INDEX idx_mt_country_sid (country_sid),
    INDEX idx_mt_tax_type (tax_type),
    INDEX idx_mt_effective_date (start_date, end_date),
    INDEX idx_mt_avalible (avalible),
    CHECK (tax_rate >= 0),
    CHECK (end_date IS NULL OR start_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='稅別主檔';

CREATE TABLE IF NOT EXISTS mst_payment_term (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '付款條件序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    payment_term_code       VARCHAR(50)                         NOT NULL COMMENT '付款條件代碼',
    payment_term_name       VARCHAR(150)                        NOT NULL COMMENT '付款條件名稱',
    term_type               VARCHAR(30)                         NOT NULL COMMENT 'IMMEDIATE即付;DAYS天數;MONTH_END月底;INSTALLMENT分期;MILESTONE里程碑',
    due_days                INT                                 NOT NULL DEFAULT 0 COMMENT '到期天數',
    installment_count       INT                                 NOT NULL DEFAULT 1 COMMENT '分期數',
    discount_days           INT                                     NULL COMMENT '折扣期限天數',
    discount_rate           DECIMAL(8,4)                            NULL COMMENT '提早付款折扣率',
    config_json             JSON                                    NULL COMMENT '進階條件設定',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mpt_payment_term_code UNIQUE (payment_term_code),
    INDEX idx_mpt_term_type (term_type),
    INDEX idx_mpt_avalible (avalible),
    CHECK (due_days >= 0),
    CHECK (installment_count > 0),
    CHECK (discount_days IS NULL OR discount_days >= 0),
    CHECK (discount_rate IS NULL OR discount_rate >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='付款條件主檔';

CREATE TABLE IF NOT EXISTS mst_payment_method (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '付款方式序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    payment_method_code     VARCHAR(50)                         NOT NULL COMMENT '付款方式代碼',
    payment_method_name     VARCHAR(150)                        NOT NULL COMMENT '付款方式名稱',
    payment_category        VARCHAR(30)                         NOT NULL COMMENT 'CASH現金;BANK_TRANSFER轉帳;CREDIT_CARD信用卡;E_WALLET電子支付;CHECK支票;VIRTUAL_ACCOUNT虛擬帳號;OTHER',
    online_mark             TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否線上支付',
    refund_supported        TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否支援退款',
    provider_required       TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否需要支付服務商',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mpm_payment_method_code UNIQUE (payment_method_code),
    INDEX idx_mpm_payment_category (payment_category),
    INDEX idx_mpm_online_mark (online_mark),
    INDEX idx_mpm_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='付款方式主檔';

-- =========================================================
-- 06. 倉庫、儲位、文件類型、通用代碼
-- =========================================================

CREATE TABLE IF NOT EXISTS mst_warehouse (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '倉庫序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    company_sid             VARCHAR(32)                         NOT NULL COMMENT '公司序號',
    business_unit_sid       VARCHAR(32)                             NULL COMMENT '營運單位序號',
    project_sid             VARCHAR(32)                             NULL COMMENT '專案序號，建築工地倉可使用',
    warehouse_code          VARCHAR(50)                         NOT NULL COMMENT '倉庫代碼',
    warehouse_name          VARCHAR(200)                        NOT NULL COMMENT '倉庫名稱',
    warehouse_type          VARCHAR(30)                         NOT NULL COMMENT 'CENTRAL中央倉;STORE店倉;SITE工地倉;TRANSIT在途倉;RETURN退貨倉;VIRTUAL虛擬倉',
    country_sid             VARCHAR(32)                             NULL COMMENT '國家序號',
    region_sid              VARCHAR(32)                             NULL COMMENT '行政區序號',
    address_text            VARCHAR(1000)                           NULL COMMENT '地址快照',
    manager_employee_sid    VARCHAR(32)                             NULL COMMENT '倉庫負責人員工序號',
    negative_stock_allowed  TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否允許負庫存',
    batch_control_default   TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否預設批號控管',
    serial_control_default  TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否預設序號控管',
    warehouse_status        VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INACTIVE停用;CLOSED關閉',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mw_company_code UNIQUE (company_sid, warehouse_code),
    INDEX idx_mw_company_sid (company_sid),
    INDEX idx_mw_business_unit_sid (business_unit_sid),
    INDEX idx_mw_project_sid (project_sid),
    INDEX idx_mw_warehouse_type (warehouse_type),
    INDEX idx_mw_manager_employee_sid (manager_employee_sid),
    INDEX idx_mw_status (warehouse_status),
    INDEX idx_mw_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='跨產業共用倉庫主檔';

CREATE TABLE IF NOT EXISTS mst_location (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '儲位序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    warehouse_sid           VARCHAR(32)                         NOT NULL COMMENT '倉庫序號',
    parent_sid              VARCHAR(32)                             NULL COMMENT '上層儲位序號',
    location_code           VARCHAR(100)                        NOT NULL COMMENT '儲位代碼',
    location_name           VARCHAR(200)                        NOT NULL COMMENT '儲位名稱',
    location_type           VARCHAR(30)                         NOT NULL COMMENT 'ZONE區域;AISLE走道;RACK貨架;BIN儲格;FLOOR樓層;AREA工區;YARD料場;CONTAINER貨櫃',
    level_no                INT                                 NOT NULL DEFAULT 1 COMMENT '階層',
    tree_path               VARCHAR(1000)                           NULL COMMENT '樹狀路徑',
    picking_allowed         TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否允許揀料',
    receiving_allowed       TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否允許收料',
    storage_allowed         TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否允許存放',
    location_status         VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;BLOCKED封鎖;INACTIVE停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mloc_warehouse_code UNIQUE (warehouse_sid, location_code),
    INDEX idx_mloc_warehouse_sid (warehouse_sid),
    INDEX idx_mloc_parent_sid (parent_sid),
    INDEX idx_mloc_location_type (location_type),
    INDEX idx_mloc_status (location_status),
    INDEX idx_mloc_avalible (avalible),
    CHECK (level_no > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='倉庫與工地共用儲位主檔';

CREATE TABLE IF NOT EXISTS mst_document_type (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '文件類型序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    document_type_code      VARCHAR(100)                        NOT NULL COMMENT '文件類型代碼',
    document_type_name      VARCHAR(200)                        NOT NULL COMMENT '文件類型名稱',
    document_category       VARCHAR(50)                         NOT NULL COMMENT 'ORDER訂單;PURCHASE採購;CONTRACT合約;INVOICE發票;DRAWING圖面;REPORT報表;CERTIFICATE證書;OTHER',
    retention_days          INT                                     NULL COMMENT '保留天數',
    approval_required       TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否需要簽核',
    version_controlled      TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否版本控管',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mdt_document_type_code UNIQUE (document_type_code),
    INDEX idx_mdt_category (document_category),
    INDEX idx_mdt_avalible (avalible),
    CHECK (retention_days IS NULL OR retention_days > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='共用文件類型主檔';

CREATE TABLE IF NOT EXISTS mst_code_group (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '代碼群組序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    group_code              VARCHAR(100)                        NOT NULL COMMENT '代碼群組代碼',
    group_name              VARCHAR(200)                        NOT NULL COMMENT '代碼群組名稱',
    system_mark             TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否系統內建',
    allow_custom_value      TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否允許自訂值',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mcg_group_code UNIQUE (group_code),
    INDEX idx_mcg_system_mark (system_mark),
    INDEX idx_mcg_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='通用代碼群組';

CREATE TABLE IF NOT EXISTS mst_code_value (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '代碼值序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    code_group_nid          BIGINT UNSIGNED                     NOT NULL COMMENT '代碼群組流水號',
    parent_sid              VARCHAR(32)                             NULL COMMENT '上層代碼值序號',
    code_value              VARCHAR(100)                        NOT NULL COMMENT '代碼值',
    code_name               VARCHAR(200)                        NOT NULL COMMENT '代碼名稱',
    language_sid            VARCHAR(32)                             NULL COMMENT '語系序號',
    extra_data              JSON                                    NULL COMMENT '延伸資料',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_mcv_code_group
        FOREIGN KEY (code_group_nid) REFERENCES mst_code_group(nid),
    CONSTRAINT uk_mcv_group_value_language UNIQUE (code_group_nid, code_value, language_sid),
    INDEX idx_mcv_code_group_nid (code_group_nid),
    INDEX idx_mcv_parent_sid (parent_sid),
    INDEX idx_mcv_language_sid (language_sid),
    INDEX idx_mcv_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='通用代碼值';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
