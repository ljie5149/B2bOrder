namespace B2bOrder.Resources.ShareCore
{
    /// <summary>
    /// MDMDB V2 / Master Data Governance Schema
    /// 設計目標：
    /// 1. 同時支援 Shopping Platform 與 Construction ERP
    /// 2. 管理主資料來源、Golden Record、重複判定、合併、版本、品質與核准
    /// 3. 僅保存治理資料，不取代 MasterDB、PIM/MIMDB、SupplierDB、CustomerDB 等業務主檔
    /// 4. 跨資料庫關聯一律使用 sid，不建立跨資料庫 Foreign Key
    /// 5. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class MDMDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 主資料領域與來源
-- =========================================================

CREATE TABLE IF NOT EXISTS mdm_domain (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '主資料領域序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    domain_code             VARCHAR(100)                        NOT NULL COMMENT '主資料領域代碼',
    domain_name             VARCHAR(200)                        NOT NULL COMMENT '主資料領域名稱',
    entity_type             VARCHAR(100)                        NOT NULL COMMENT 'COMPANY;CUSTOMER;SUPPLIER;MATERIAL;PRODUCT;PROJECT;WAREHOUSE;OTHER',
    owner_service_code      VARCHAR(80)                         NOT NULL COMMENT '主責服務代碼',
    golden_source_strategy  VARCHAR(30)                         NOT NULL DEFAULT 'SURVIVORSHIP' COMMENT 'SURVIVORSHIP存續規則;PRIORITY來源優先;MANUAL人工;LATEST最新',
    approval_required       TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否需核准',
    version_controlled      TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否版本控管',
    domain_status           VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;PAUSED暫停;DISABLED停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_md_domain_code UNIQUE (domain_code),
    INDEX idx_md_entity_type (entity_type),
    INDEX idx_md_owner_service_code (owner_service_code),
    INDEX idx_md_status (domain_status),
    INDEX idx_md_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料治理領域';

CREATE TABLE IF NOT EXISTS mdm_source_system (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '來源系統序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    source_code             VARCHAR(100)                        NOT NULL COMMENT '來源系統代碼',
    source_name             VARCHAR(200)                        NOT NULL COMMENT '來源系統名稱',
    source_type             VARCHAR(30)                         NOT NULL COMMENT 'INTERNAL內部;EXTERNAL外部;FILE檔案;API;MANUAL人工',
    service_code            VARCHAR(80)                             NULL COMMENT '服務代碼',
    source_priority         INT                                 NOT NULL DEFAULT 100 COMMENT '來源優先順序，數字越小越高',
    trust_score             DECIMAL(8,4)                        NOT NULL DEFAULT 1 COMMENT '來源可信度0至1',
    source_status           VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;PAUSED暫停;DISABLED停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_mss_source_code UNIQUE (source_code),
    INDEX idx_mss_source_type (source_type),
    INDEX idx_mss_service_code (service_code),
    INDEX idx_mss_priority (source_priority),
    INDEX idx_mss_status (source_status),
    INDEX idx_mss_avalible (avalible),
    CHECK (source_priority >= 0),
    CHECK (trust_score BETWEEN 0 AND 1)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料來源系統';

CREATE TABLE IF NOT EXISTS mdm_domain_source (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '領域來源設定序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    domain_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '主資料領域流水號',
    source_system_nid       BIGINT UNSIGNED                     NOT NULL COMMENT '來源系統流水號',
    source_entity_name      VARCHAR(200)                        NOT NULL COMMENT '來源實體名稱',
    source_key_field        VARCHAR(200)                        NOT NULL COMMENT '來源主鍵欄位',
    source_priority         INT                                 NOT NULL DEFAULT 100 COMMENT '此領域下來源優先順序',
    authoritative_mark      TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否為權威來源',
    inbound_enabled         TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否允許匯入',
    outbound_enabled        TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否允許回寫',
    mapping_config          JSON                                    NULL COMMENT '欄位對應設定',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_mds_domain
        FOREIGN KEY (domain_nid) REFERENCES mdm_domain(nid),
    CONSTRAINT fk_mds_source_system
        FOREIGN KEY (source_system_nid) REFERENCES mdm_source_system(nid),
    CONSTRAINT uk_mds_domain_source UNIQUE (domain_nid, source_system_nid, source_entity_name),
    INDEX idx_mds_domain_nid (domain_nid),
    INDEX idx_mds_source_system_nid (source_system_nid),
    INDEX idx_mds_authoritative_mark (authoritative_mark),
    INDEX idx_mds_avalible (avalible),
    CHECK (source_priority >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料領域來源設定';

-- =========================================================
-- 02. 主資料紀錄與Golden Record
-- =========================================================

CREATE TABLE IF NOT EXISTS mdm_record (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '主資料治理紀錄序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    domain_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '主資料領域流水號',
    source_system_nid       BIGINT UNSIGNED                     NOT NULL COMMENT '來源系統流水號',
    source_record_id        VARCHAR(300)                        NOT NULL COMMENT '來源系統資料識別碼',
    business_key            VARCHAR(500)                            NULL COMMENT '業務唯一鍵',
    entity_sid              VARCHAR(32)                             NULL COMMENT '對應業務系統資料序號',
    record_data             JSON                                NOT NULL COMMENT '來源主資料內容',
    normalized_data         JSON                                    NULL COMMENT '標準化後資料',
    record_hash             VARCHAR(128)                            NULL COMMENT '標準化資料雜湊',
    record_status           VARCHAR(30)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;PENDING待處理;MERGED已合併;REJECTED拒絕;DELETED刪除',
    golden_record_sid       VARCHAR(32)                             NULL COMMENT '對應Golden Record序號',
    first_seen_date         DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '首次發現時間',
    last_seen_date          DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '最後發現時間',
    source_modified_date    DATETIME                                NULL COMMENT '來源資料修改時間',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 1 COMMENT '來源紀錄版本',
    CONSTRAINT fk_mr_domain
        FOREIGN KEY (domain_nid) REFERENCES mdm_domain(nid),
    CONSTRAINT fk_mr_source_system
        FOREIGN KEY (source_system_nid) REFERENCES mdm_source_system(nid),
    CONSTRAINT uk_mr_source_record UNIQUE (domain_nid, source_system_nid, source_record_id),
    INDEX idx_mr_domain_nid (domain_nid),
    INDEX idx_mr_source_system_nid (source_system_nid),
    INDEX idx_mr_business_key (business_key(191)),
    INDEX idx_mr_entity_sid (entity_sid),
    INDEX idx_mr_record_hash (record_hash),
    INDEX idx_mr_status (record_status),
    INDEX idx_mr_golden_record_sid (golden_record_sid),
    INDEX idx_mr_last_seen_date (last_seen_date),
    CHECK (version_no > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='來源主資料治理紀錄';

CREATE TABLE IF NOT EXISTS mdm_golden_record (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Golden Record序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    domain_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '主資料領域流水號',
    golden_code             VARCHAR(150)                        NOT NULL COMMENT 'Golden Record代碼',
    business_key            VARCHAR(500)                            NULL COMMENT '業務唯一鍵',
    entity_sid              VARCHAR(32)                             NULL COMMENT '對應正式業務資料序號',
    golden_data             JSON                                NOT NULL COMMENT 'Golden Record內容',
    golden_hash             VARCHAR(128)                            NULL COMMENT 'Golden Record雜湊',
    survivorship_summary    JSON                                    NULL COMMENT '欄位存續來源摘要',
    quality_score           DECIMAL(8,4)                        NOT NULL DEFAULT 0 COMMENT '資料品質分數0至100',
    completeness_score      DECIMAL(8,4)                        NOT NULL DEFAULT 0 COMMENT '完整度0至100',
    golden_status           VARCHAR(30)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;REVIEW待審;APPROVED核准;PUBLISHED已發布;MERGED已合併;RETIRED停用',
    current_version         BIGINT UNSIGNED                     NOT NULL DEFAULT 1 COMMENT '目前版本',
    steward_user_sid        VARCHAR(32)                             NULL COMMENT '資料管理人員序號',
    approved_user_sid       VARCHAR(32)                             NULL COMMENT '核准人員序號',
    approved_date           DATETIME                                NULL COMMENT '核准時間',
    published_date          DATETIME                                NULL COMMENT '發布時間',
    workflow_instance_sid   VARCHAR(32)                             NULL COMMENT 'WorkflowDB流程實例序號',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_mgr_domain
        FOREIGN KEY (domain_nid) REFERENCES mdm_domain(nid),
    CONSTRAINT uk_mgr_domain_code UNIQUE (domain_nid, golden_code),
    INDEX idx_mgr_business_key (business_key(191)),
    INDEX idx_mgr_entity_sid (entity_sid),
    INDEX idx_mgr_golden_hash (golden_hash),
    INDEX idx_mgr_status (golden_status),
    INDEX idx_mgr_steward_user_sid (steward_user_sid),
    INDEX idx_mgr_workflow_instance_sid (workflow_instance_sid),
    INDEX idx_mgr_avalible (avalible),
    CHECK (quality_score BETWEEN 0 AND 100),
    CHECK (completeness_score BETWEEN 0 AND 100),
    CHECK (current_version > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料Golden Record';

CREATE TABLE IF NOT EXISTS mdm_golden_record_source (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Golden Record來源關聯序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    golden_record_nid       BIGINT UNSIGNED                     NOT NULL COMMENT 'Golden Record流水號',
    mdm_record_nid          BIGINT UNSIGNED                     NOT NULL COMMENT '來源紀錄流水號',
    source_role             VARCHAR(30)                         NOT NULL DEFAULT 'CONTRIBUTOR' COMMENT 'PRIMARY主要;CONTRIBUTOR貢獻;REFERENCE參考;REJECTED排除',
    contribution_score      DECIMAL(8,4)                        NOT NULL DEFAULT 0 COMMENT '欄位貢獻比例0至1',
    active_mark             TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否有效',
    CONSTRAINT fk_mgrs_golden_record
        FOREIGN KEY (golden_record_nid) REFERENCES mdm_golden_record(nid),
    CONSTRAINT fk_mgrs_mdm_record
        FOREIGN KEY (mdm_record_nid) REFERENCES mdm_record(nid),
    CONSTRAINT uk_mgrs_golden_source UNIQUE (golden_record_nid, mdm_record_nid),
    INDEX idx_mgrs_golden_record_nid (golden_record_nid),
    INDEX idx_mgrs_mdm_record_nid (mdm_record_nid),
    INDEX idx_mgrs_source_role (source_role),
    INDEX idx_mgrs_active_mark (active_mark),
    CHECK (contribution_score BETWEEN 0 AND 1)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Golden Record來源關聯';

-- =========================================================
-- 03. 重複判定與合併
-- =========================================================

CREATE TABLE IF NOT EXISTS mdm_match_rule (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '比對規則序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    domain_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '主資料領域流水號',
    rule_code               VARCHAR(100)                        NOT NULL COMMENT '比對規則代碼',
    rule_name               VARCHAR(200)                        NOT NULL COMMENT '比對規則名稱',
    match_type              VARCHAR(30)                         NOT NULL COMMENT 'EXACT精確;FUZZY模糊;COMPOSITE複合;CUSTOM自訂',
    field_config            JSON                                NOT NULL COMMENT '比對欄位與權重',
    threshold_score         DECIMAL(8,4)                        NOT NULL DEFAULT 0.8 COMMENT '判定門檻0至1',
    auto_merge_threshold    DECIMAL(8,4)                            NULL COMMENT '自動合併門檻0至1',
    blocking_rule           JSON                                    NULL COMMENT '阻擋條件',
    rule_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;PAUSED暫停;DISABLED停用',
    priority                INT                                 NOT NULL DEFAULT 0 COMMENT '優先順序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_mmr_domain
        FOREIGN KEY (domain_nid) REFERENCES mdm_domain(nid),
    CONSTRAINT uk_mmr_domain_rule UNIQUE (domain_nid, rule_code),
    INDEX idx_mmr_domain_nid (domain_nid),
    INDEX idx_mmr_match_type (match_type),
    INDEX idx_mmr_status (rule_status),
    INDEX idx_mmr_priority (priority),
    INDEX idx_mmr_avalible (avalible),
    CHECK (threshold_score BETWEEN 0 AND 1),
    CHECK (auto_merge_threshold IS NULL OR auto_merge_threshold BETWEEN 0 AND 1)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料重複比對規則';

CREATE TABLE IF NOT EXISTS mdm_match_candidate (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '重複候選序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    domain_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '主資料領域流水號',
    left_record_sid         VARCHAR(32)                         NOT NULL COMMENT '左側紀錄序號',
    right_record_sid        VARCHAR(32)                         NOT NULL COMMENT '右側紀錄序號',
    match_rule_sid          VARCHAR(32)                             NULL COMMENT '使用比對規則序號',
    match_score             DECIMAL(8,4)                        NOT NULL COMMENT '比對分數0至1',
    field_scores            JSON                                    NULL COMMENT '欄位分數明細',
    candidate_status        VARCHAR(30)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待判定;MATCH確認重複;NOT_MATCH非重複;AUTO_MERGED自動合併;IGNORED忽略',
    reviewed_user_sid       VARCHAR(32)                             NULL COMMENT '判定人員序號',
    reviewed_date           DATETIME                                NULL COMMENT '判定時間',
    review_note             TEXT                                    NULL COMMENT '判定說明',
    CONSTRAINT uk_mmc_pair UNIQUE (domain_nid, left_record_sid, right_record_sid),
    INDEX idx_mmc_domain_nid (domain_nid),
    INDEX idx_mmc_left_record_sid (left_record_sid),
    INDEX idx_mmc_right_record_sid (right_record_sid),
    INDEX idx_mmc_match_rule_sid (match_rule_sid),
    INDEX idx_mmc_match_score (match_score),
    INDEX idx_mmc_status (candidate_status),
    CHECK (left_record_sid <> right_record_sid),
    CHECK (match_score BETWEEN 0 AND 1)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料重複候選';

CREATE TABLE IF NOT EXISTS mdm_merge_job (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '主資料合併工作序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    merge_no                VARCHAR(100)                        NOT NULL COMMENT '合併工作編號',
    domain_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '主資料領域流水號',
    survivor_golden_sid     VARCHAR(32)                         NOT NULL COMMENT '保留Golden Record序號',
    merged_golden_sid       VARCHAR(32)                             NULL COMMENT '被合併Golden Record序號',
    merged_record_sids      JSON                                NOT NULL COMMENT '合併來源紀錄清單',
    merge_strategy          VARCHAR(30)                         NOT NULL COMMENT 'AUTO自動;MANUAL人工;RULE規則',
    survivorship_result     JSON                                NOT NULL COMMENT '欄位存續結果',
    merge_status            VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;PROCESSING處理中;SUCCESS成功;FAILED失敗;REVERSED已撤銷',
    requested_user_sid      VARCHAR(32)                             NULL COMMENT '申請人員序號',
    approved_user_sid       VARCHAR(32)                             NULL COMMENT '核准人員序號',
    workflow_instance_sid   VARCHAR(32)                             NULL COMMENT 'WorkflowDB流程實例序號',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    error_message           LONGTEXT                                NULL COMMENT '錯誤訊息',
    CONSTRAINT fk_mmj_domain
        FOREIGN KEY (domain_nid) REFERENCES mdm_domain(nid),
    CONSTRAINT uk_mmj_merge_no UNIQUE (merge_no),
    INDEX idx_mmj_domain_nid (domain_nid),
    INDEX idx_mmj_survivor_golden_sid (survivor_golden_sid),
    INDEX idx_mmj_merged_golden_sid (merged_golden_sid),
    INDEX idx_mmj_status (merge_status),
    INDEX idx_mmj_workflow_instance_sid (workflow_instance_sid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料合併工作';

CREATE TABLE IF NOT EXISTS mdm_merge_history (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '主資料合併歷程序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    merge_job_sid           VARCHAR(32)                         NOT NULL COMMENT '合併工作序號',
    domain_sid              VARCHAR(32)                         NOT NULL COMMENT '主資料領域序號',
    survivor_golden_sid     VARCHAR(32)                         NOT NULL COMMENT '保留Golden Record序號',
    merged_golden_sid       VARCHAR(32)                             NULL COMMENT '被合併Golden Record序號',
    before_snapshot         JSON                                NOT NULL COMMENT '合併前快照',
    after_snapshot          JSON                                NOT NULL COMMENT '合併後快照',
    field_resolution        JSON                                    NULL COMMENT '欄位解決明細',
    reversible_mark         TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否可撤銷',
    reversed_date           DATETIME                                NULL COMMENT '撤銷時間',
    reversed_user_sid       VARCHAR(32)                             NULL COMMENT '撤銷人員序號',
    reverse_reason          TEXT                                    NULL COMMENT '撤銷原因',
    INDEX idx_mmh_merge_job_sid (merge_job_sid),
    INDEX idx_mmh_domain_sid (domain_sid),
    INDEX idx_mmh_survivor_golden_sid (survivor_golden_sid),
    INDEX idx_mmh_merged_golden_sid (merged_golden_sid),
    INDEX idx_mmh_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料合併歷程';

-- =========================================================
-- 04. 欄位存續與標準化
-- =========================================================

CREATE TABLE IF NOT EXISTS mdm_survivorship_rule (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '欄位存續規則序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    domain_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '主資料領域流水號',
    field_name              VARCHAR(200)                        NOT NULL COMMENT '欄位名稱',
    strategy_type           VARCHAR(30)                         NOT NULL COMMENT 'SOURCE_PRIORITY來源優先;MOST_RECENT最新;MOST_COMPLETE最完整;HIGHEST_TRUST最高可信;MANUAL人工;CUSTOM自訂',
    source_priority_json    JSON                                    NULL COMMENT '欄位來源優先順序',
    custom_expression       TEXT                                    NULL COMMENT '自訂運算式',
    null_handling           VARCHAR(20)                         NOT NULL DEFAULT 'IGNORE' COMMENT 'IGNORE忽略空值;ALLOW允許空值;REPLACE取代',
    priority                INT                                 NOT NULL DEFAULT 0 COMMENT '規則優先順序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_msr_domain
        FOREIGN KEY (domain_nid) REFERENCES mdm_domain(nid),
    CONSTRAINT uk_msr_domain_field UNIQUE (domain_nid, field_name),
    INDEX idx_msr_domain_nid (domain_nid),
    INDEX idx_msr_strategy_type (strategy_type),
    INDEX idx_msr_priority (priority),
    INDEX idx_msr_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Golden Record欄位存續規則';

CREATE TABLE IF NOT EXISTS mdm_normalization_rule (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '標準化規則序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    domain_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '主資料領域流水號',
    field_name              VARCHAR(200)                        NOT NULL COMMENT '欄位名稱',
    rule_code               VARCHAR(100)                        NOT NULL COMMENT '標準化規則代碼',
    normalization_type      VARCHAR(30)                         NOT NULL COMMENT 'TRIM去空白;UPPERCASE大寫;LOWERCASE小寫;PHONE電話;ADDRESS地址;TAX_NO統編;UNIT單位;CUSTOM自訂',
    config_json             JSON                                    NULL COMMENT '標準化設定',
    priority                INT                                 NOT NULL DEFAULT 0 COMMENT '執行順序',
    rule_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;PAUSED暫停;DISABLED停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_mnr_domain
        FOREIGN KEY (domain_nid) REFERENCES mdm_domain(nid),
    CONSTRAINT uk_mnr_domain_rule UNIQUE (domain_nid, field_name, rule_code),
    INDEX idx_mnr_domain_nid (domain_nid),
    INDEX idx_mnr_normalization_type (normalization_type),
    INDEX idx_mnr_priority (priority),
    INDEX idx_mnr_status (rule_status),
    INDEX idx_mnr_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料標準化規則';

-- =========================================================
-- 05. 代碼映射
-- =========================================================

CREATE TABLE IF NOT EXISTS mdm_code_mapping (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '跨系統代碼映射序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    domain_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '主資料領域流水號',
    source_system_nid       BIGINT UNSIGNED                     NOT NULL COMMENT '來源系統流水號',
    target_system_nid       BIGINT UNSIGNED                     NOT NULL COMMENT '目標系統流水號',
    mapping_type            VARCHAR(50)                         NOT NULL COMMENT 'ENTITY實體;CATEGORY分類;UNIT單位;STATUS狀態;ATTRIBUTE屬性;OTHER',
    source_code             VARCHAR(500)                        NOT NULL COMMENT '來源代碼',
    source_name             VARCHAR(500)                            NULL COMMENT '來源名稱',
    target_code             VARCHAR(500)                        NOT NULL COMMENT '目標代碼',
    target_name             VARCHAR(500)                            NULL COMMENT '目標名稱',
    golden_record_sid       VARCHAR(32)                             NULL COMMENT '對應Golden Record序號',
    transform_rule          JSON                                    NULL COMMENT '轉換規則',
    mapping_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;INVALID無效;PENDING待確認;DISABLED停用',
    start_date              DATETIME                                NULL COMMENT '生效時間',
    end_date                DATETIME                                NULL COMMENT '失效時間',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_mcm_domain
        FOREIGN KEY (domain_nid) REFERENCES mdm_domain(nid),
    CONSTRAINT fk_mcm_source_system
        FOREIGN KEY (source_system_nid) REFERENCES mdm_source_system(nid),
    CONSTRAINT fk_mcm_target_system
        FOREIGN KEY (target_system_nid) REFERENCES mdm_source_system(nid),
    CONSTRAINT uk_mcm_mapping UNIQUE (domain_nid, source_system_nid, target_system_nid, mapping_type, source_code(191)),
    INDEX idx_mcm_domain_nid (domain_nid),
    INDEX idx_mcm_source_system_nid (source_system_nid),
    INDEX idx_mcm_target_system_nid (target_system_nid),
    INDEX idx_mcm_mapping_type (mapping_type),
    INDEX idx_mcm_target_code (target_code(191)),
    INDEX idx_mcm_golden_record_sid (golden_record_sid),
    INDEX idx_mcm_status (mapping_status),
    INDEX idx_mcm_avalible (avalible),
    CHECK (source_system_nid <> target_system_nid),
    CHECK (end_date IS NULL OR start_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='跨系統主資料代碼映射';

-- =========================================================
-- 06. 資料品質
-- =========================================================

CREATE TABLE IF NOT EXISTS mdm_quality_rule (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '資料品質規則序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    domain_nid              BIGINT UNSIGNED                     NOT NULL COMMENT '主資料領域流水號',
    rule_code               VARCHAR(100)                        NOT NULL COMMENT '品質規則代碼',
    rule_name               VARCHAR(200)                        NOT NULL COMMENT '品質規則名稱',
    quality_dimension       VARCHAR(30)                         NOT NULL COMMENT 'COMPLETENESS完整;VALIDITY有效;UNIQUENESS唯一;CONSISTENCY一致;ACCURACY正確;TIMELINESS及時',
    severity                VARCHAR(20)                         NOT NULL DEFAULT 'WARNING' COMMENT 'INFO資訊;WARNING警告;ERROR錯誤;CRITICAL嚴重',
    field_name              VARCHAR(200)                            NULL COMMENT '檢查欄位',
    rule_expression         TEXT                                NOT NULL COMMENT '規則運算式',
    rule_config             JSON                                    NULL COMMENT '規則設定',
    score_weight            DECIMAL(8,4)                        NOT NULL DEFAULT 1 COMMENT '計分權重',
    blocking_mark           TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否阻止發布',
    auto_fix_enabled        TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否允許自動修正',
    auto_fix_config         JSON                                    NULL COMMENT '自動修正設定',
    rule_status             VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;PAUSED暫停;DISABLED停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_mqr_domain
        FOREIGN KEY (domain_nid) REFERENCES mdm_domain(nid),
    CONSTRAINT uk_mqr_domain_rule UNIQUE (domain_nid, rule_code),
    INDEX idx_mqr_domain_nid (domain_nid),
    INDEX idx_mqr_quality_dimension (quality_dimension),
    INDEX idx_mqr_severity (severity),
    INDEX idx_mqr_blocking_mark (blocking_mark),
    INDEX idx_mqr_status (rule_status),
    INDEX idx_mqr_avalible (avalible),
    CHECK (score_weight >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料品質規則';

CREATE TABLE IF NOT EXISTS mdm_quality_result (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '資料品質檢查結果序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    domain_sid              VARCHAR(32)                         NOT NULL COMMENT '主資料領域序號',
    golden_record_sid       VARCHAR(32)                             NULL COMMENT 'Golden Record序號',
    mdm_record_sid          VARCHAR(32)                             NULL COMMENT '來源紀錄序號',
    quality_rule_sid        VARCHAR(32)                         NOT NULL COMMENT '品質規則序號',
    check_result            VARCHAR(20)                         NOT NULL COMMENT 'PASS通過;WARNING警告;FAIL失敗',
    score                   DECIMAL(8,4)                        NOT NULL DEFAULT 0 COMMENT '本規則得分',
    message                 VARCHAR(1000)                           NULL COMMENT '檢查訊息',
    details_json            JSON                                    NULL COMMENT '檢查明細',
    auto_fixed              TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否已自動修正',
    resolved                TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否已解決',
    resolved_user_sid       VARCHAR(32)                             NULL COMMENT '處理人員序號',
    resolved_date           DATETIME                                NULL COMMENT '解決時間',
    INDEX idx_mqres_domain_sid (domain_sid),
    INDEX idx_mqres_golden_record_sid (golden_record_sid),
    INDEX idx_mqres_mdm_record_sid (mdm_record_sid),
    INDEX idx_mqres_quality_rule_sid (quality_rule_sid),
    INDEX idx_mqres_check_result (check_result),
    INDEX idx_mqres_resolved (resolved),
    INDEX idx_mqres_create_date (create_date),
    CHECK (score >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料品質檢查結果';

-- =========================================================
-- 07. 版本與變更申請
-- =========================================================

CREATE TABLE IF NOT EXISTS mdm_golden_version (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT 'Golden Record版本序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    golden_record_nid       BIGINT UNSIGNED                     NOT NULL COMMENT 'Golden Record流水號',
    version_no              BIGINT UNSIGNED                     NOT NULL COMMENT '版本號',
    snapshot_data           JSON                                NOT NULL COMMENT '版本快照',
    source_summary          JSON                                    NULL COMMENT '來源摘要',
    change_type             VARCHAR(30)                         NOT NULL COMMENT 'CREATE新增;UPDATE修改;MERGE合併;SPLIT拆分;ROLLBACK回滾;PUBLISH發布',
    change_summary          VARCHAR(1000)                           NULL COMMENT '異動摘要',
    create_user_sid         VARCHAR(32)                             NULL COMMENT '建立人員序號',
    version_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;SUPERSEDED已取代;ARCHIVED封存;ROLLED_BACK已回滾',
    CONSTRAINT fk_mgv_golden_record
        FOREIGN KEY (golden_record_nid) REFERENCES mdm_golden_record(nid),
    CONSTRAINT uk_mgv_golden_version UNIQUE (golden_record_nid, version_no),
    INDEX idx_mgv_golden_record_nid (golden_record_nid),
    INDEX idx_mgv_change_type (change_type),
    INDEX idx_mgv_status (version_status),
    INDEX idx_mgv_create_date (create_date),
    CHECK (version_no > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Golden Record版本歷程';

CREATE TABLE IF NOT EXISTS mdm_change_request (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '主資料變更申請序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    request_no              VARCHAR(100)                        NOT NULL COMMENT '變更申請編號',
    domain_sid              VARCHAR(32)                         NOT NULL COMMENT '主資料領域序號',
    golden_record_sid       VARCHAR(32)                             NULL COMMENT 'Golden Record序號',
    request_type            VARCHAR(30)                         NOT NULL COMMENT 'CREATE新增;UPDATE修改;MERGE合併;SPLIT拆分;RETIRE停用;RESTORE還原',
    before_data             JSON                                    NULL COMMENT '異動前資料',
    requested_data          JSON                                NOT NULL COMMENT '申請變更資料',
    changed_fields          JSON                                    NULL COMMENT '異動欄位',
    request_reason          TEXT                                NOT NULL COMMENT '申請原因',
    requester_user_sid      VARCHAR(32)                         NOT NULL COMMENT '申請人員序號',
    request_status          VARCHAR(30)                         NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT草稿;SUBMITTED已送出;REVIEW待審;APPROVED核准;REJECTED駁回;APPLIED已套用;CANCELLED取消',
    workflow_instance_sid   VARCHAR(32)                             NULL COMMENT 'WorkflowDB流程實例序號',
    approved_user_sid       VARCHAR(32)                             NULL COMMENT '核准人員序號',
    approved_date           DATETIME                                NULL COMMENT '核准時間',
    applied_date            DATETIME                                NULL COMMENT '套用時間',
    reject_reason           TEXT                                    NULL COMMENT '駁回原因',
    CONSTRAINT uk_mcr_request_no UNIQUE (request_no),
    INDEX idx_mcr_domain_sid (domain_sid),
    INDEX idx_mcr_golden_record_sid (golden_record_sid),
    INDEX idx_mcr_request_type (request_type),
    INDEX idx_mcr_requester_user_sid (requester_user_sid),
    INDEX idx_mcr_status (request_status),
    INDEX idx_mcr_workflow_instance_sid (workflow_instance_sid),
    INDEX idx_mcr_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料變更申請';

-- =========================================================
-- 08. 發布與同步
-- =========================================================

CREATE TABLE IF NOT EXISTS mdm_publish_job (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '主資料發布工作序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    job_no                  VARCHAR(100)                        NOT NULL COMMENT '發布工作編號',
    domain_sid              VARCHAR(32)                         NOT NULL COMMENT '主資料領域序號',
    golden_record_sid       VARCHAR(32)                         NOT NULL COMMENT 'Golden Record序號',
    golden_version_no       BIGINT UNSIGNED                     NOT NULL COMMENT '發布版本號',
    target_system_sid       VARCHAR(32)                         NOT NULL COMMENT '目標系統序號',
    operation_type          VARCHAR(30)                         NOT NULL COMMENT 'CREATE新增;UPDATE更新;RETIRE停用;DELETE刪除;SYNC同步',
    request_payload         JSON                                    NULL COMMENT '送出內容',
    response_payload        JSON                                    NULL COMMENT '回應內容',
    publish_status          VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;PROCESSING處理中;SUCCESS成功;FAILED失敗;RETRY重試;DEAD死信',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '重試次數',
    max_retry_count         INT                                 NOT NULL DEFAULT 5 COMMENT '最大重試次數',
    next_retry_date         DATETIME                                NULL COMMENT '下次重試時間',
    external_reference_no   VARCHAR(200)                            NULL COMMENT '目標系統回應編號',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    error_message           LONGTEXT                                NULL COMMENT '錯誤訊息',
    correlation_id          VARCHAR(100)                            NULL COMMENT '關聯識別碼',
    CONSTRAINT uk_mpj_job_no UNIQUE (job_no),
    INDEX idx_mpj_domain_sid (domain_sid),
    INDEX idx_mpj_golden_record_sid (golden_record_sid),
    INDEX idx_mpj_target_system_sid (target_system_sid),
    INDEX idx_mpj_status (publish_status),
    INDEX idx_mpj_next_retry_date (next_retry_date),
    INDEX idx_mpj_correlation_id (correlation_id),
    CHECK (golden_version_no > 0),
    CHECK (retry_count >= 0),
    CHECK (max_retry_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Golden Record發布工作';

CREATE TABLE IF NOT EXISTS mdm_sync_checkpoint (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '主資料同步檢查點序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    domain_sid              VARCHAR(32)                         NOT NULL COMMENT '主資料領域序號',
    source_system_sid       VARCHAR(32)                         NOT NULL COMMENT '來源系統序號',
    checkpoint_type         VARCHAR(30)                         NOT NULL COMMENT 'TIMESTAMP時間;SEQUENCE序號;CURSOR游標;VERSION版本',
    checkpoint_value        VARCHAR(1000)                       NOT NULL COMMENT '檢查點值',
    last_success_date       DATETIME                                NULL COMMENT '最後成功時間',
    last_record_count       INT                                 NOT NULL DEFAULT 0 COMMENT '最後同步筆數',
    version_no              BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    CONSTRAINT uk_msc_domain_source UNIQUE (domain_sid, source_system_sid),
    INDEX idx_msc_domain_sid (domain_sid),
    INDEX idx_msc_source_system_sid (source_system_sid),
    INDEX idx_msc_last_success_date (last_success_date),
    CHECK (last_record_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料同步檢查點';

-- =========================================================
-- 09. 資料管理責任
-- =========================================================

CREATE TABLE IF NOT EXISTS mdm_steward_assignment (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '資料管理責任序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    domain_sid              VARCHAR(32)                         NOT NULL COMMENT '主資料領域序號',
    scope_type              VARCHAR(30)                         NOT NULL COMMENT 'DOMAIN整體;CATEGORY分類;REGION區域;COMPANY公司;PROJECT專案;CUSTOM自訂',
    scope_sid               VARCHAR(32)                             NULL COMMENT '責任範圍序號',
    steward_user_sid        VARCHAR(32)                         NOT NULL COMMENT '資料管理人員序號',
    backup_user_sid         VARCHAR(32)                             NULL COMMENT '代理資料管理人員序號',
    responsibility_type     VARCHAR(30)                         NOT NULL DEFAULT 'OWNER' COMMENT 'OWNER負責人;REVIEWER審查人;APPROVER核准人;QUALITY資料品質',
    start_date              DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '生效時間',
    end_date                DATETIME                                NULL COMMENT '失效時間',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_msa_assignment UNIQUE (domain_sid, scope_type, scope_sid, steward_user_sid, responsibility_type),
    INDEX idx_msa_domain_sid (domain_sid),
    INDEX idx_msa_scope (scope_type, scope_sid),
    INDEX idx_msa_steward_user_sid (steward_user_sid),
    INDEX idx_msa_backup_user_sid (backup_user_sid),
    INDEX idx_msa_responsibility_type (responsibility_type),
    INDEX idx_msa_effective_date (start_date, end_date),
    INDEX idx_msa_avalible (avalible),
    CHECK (end_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='主資料管理責任分派';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
