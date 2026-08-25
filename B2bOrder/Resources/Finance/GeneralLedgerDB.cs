namespace B2bOrder.Resources.Finance
{
    /// <summary>
    /// GeneralLedgerDB V2 Shared Core Schema
    /// 支援 Shopping、B2B Sales、Construction ERP、多公司、多帳簿、多幣別、專案/WBS維度。
    /// AccountingDB 產生傳票請求；GeneralLedgerDB 負責正式過帳、餘額、關帳與報表。
    /// nid：資料庫內部主鍵；sid：跨服務/API識別碼；avalible：Y可用、W停用、D刪除。
    /// MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class GeneralLedgerDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

CREATE TABLE IF NOT EXISTS gl_fiscal_calendar (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '會計年度曆序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    calendar_code VARCHAR(100) NOT NULL COMMENT '年度曆代碼',
    calendar_name VARCHAR(200) NOT NULL COMMENT '年度曆名稱',
    fiscal_year_start_month INT NOT NULL DEFAULT 1 COMMENT '年度起始月份',
    period_count INT NOT NULL DEFAULT 12 COMMENT '一般期間數',
    adjustment_period_count INT NOT NULL DEFAULT 1 COMMENT '調整期間數',
    calendar_type VARCHAR(20) NOT NULL DEFAULT 'MONTHLY' COMMENT 'MONTHLY;FOUR_FOUR_FIVE;CUSTOM',
    calendar_status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE;INACTIVE',
    avalible VARCHAR(2) NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark TEXT NULL COMMENT '備註',
    UNIQUE KEY uk_gfc_code (calendar_code),
    INDEX idx_gfc_status (calendar_status),
    CHECK (fiscal_year_start_month BETWEEN 1 AND 12),
    CHECK (period_count > 0),
    CHECK (adjustment_period_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='會計年度曆';

CREATE TABLE IF NOT EXISTS gl_accounting_period (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '會計期間序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    fiscal_calendar_sid VARCHAR(32) NOT NULL COMMENT '年度曆序號',
    fiscal_year INT NOT NULL COMMENT '會計年度',
    period_no INT NOT NULL COMMENT '期間序號',
    period_name VARCHAR(100) NOT NULL COMMENT '期間名稱',
    period_type VARCHAR(20) NOT NULL DEFAULT 'NORMAL' COMMENT 'NORMAL;ADJUSTMENT;OPENING;CLOSING',
    start_date DATE NOT NULL COMMENT '開始日',
    end_date DATE NOT NULL COMMENT '結束日',
    posting_status VARCHAR(20) NOT NULL DEFAULT 'OPEN' COMMENT 'FUTURE;OPEN;SOFT_CLOSED;HARD_CLOSED;REOPENED',
    opened_date DATETIME NULL COMMENT '開帳時間',
    closed_date DATETIME NULL COMMENT '關帳時間',
    closed_user_sid VARCHAR(32) NULL COMMENT '關帳人員序號',
    reopen_reason TEXT NULL COMMENT '重開原因',
    UNIQUE KEY uk_gap_period (fiscal_calendar_sid,fiscal_year,period_no),
    INDEX idx_gap_dates (start_date,end_date),
    INDEX idx_gap_status (posting_status),
    CHECK (fiscal_year > 0), CHECK (period_no > 0), CHECK (end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='會計期間';

CREATE TABLE IF NOT EXISTS gl_chart_of_account (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '科目表序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    chart_code VARCHAR(100) NOT NULL COMMENT '科目表代碼',
    chart_name VARCHAR(200) NOT NULL COMMENT '科目表名稱',
    country_sid VARCHAR(32) NULL COMMENT '國家序號',
    accounting_standard VARCHAR(30) NOT NULL DEFAULT 'IFRS' COMMENT 'IFRS;LOCAL_GAAP;TAX_BASIS;MANAGEMENT',
    chart_status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE;INACTIVE',
    avalible VARCHAR(2) NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark TEXT NULL COMMENT '備註',
    UNIQUE KEY uk_gcoa_code (chart_code),
    INDEX idx_gcoa_standard (accounting_standard),
    INDEX idx_gcoa_status (chart_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='會計科目表';

CREATE TABLE IF NOT EXISTS gl_account (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '會計科目序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    chart_of_account_sid VARCHAR(32) NOT NULL COMMENT '科目表序號',
    account_code VARCHAR(100) NOT NULL COMMENT '科目代碼',
    account_name VARCHAR(300) NOT NULL COMMENT '科目名稱',
    parent_account_sid VARCHAR(32) NULL COMMENT '上層科目序號',
    account_level INT NOT NULL DEFAULT 1 COMMENT '科目層級',
    account_class VARCHAR(30) NOT NULL COMMENT 'ASSET;LIABILITY;EQUITY;REVENUE;EXPENSE;OTHER',
    account_type VARCHAR(50) NOT NULL COMMENT 'CASH;BANK;AR;AP;INVENTORY;FIXED_ASSET;TAX;REVENUE;COGS;EXPENSE;OTHER',
    normal_balance VARCHAR(10) NOT NULL COMMENT 'DEBIT;CREDIT',
    posting_allowed TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否可直接過帳',
    reconciliation_required TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否需清帳',
    party_required TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Party是否必填',
    project_required TINYINT(1) NOT NULL DEFAULT 0 COMMENT '專案是否必填',
    cost_center_required TINYINT(1) NOT NULL DEFAULT 0 COMMENT '成本中心是否必填',
    effective_start_date DATE NOT NULL COMMENT '生效日',
    effective_end_date DATE NULL COMMENT '失效日',
    account_status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE;BLOCKED;INACTIVE',
    avalible VARCHAR(2) NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark TEXT NULL COMMENT '備註',
    UNIQUE KEY uk_ga_code (chart_of_account_sid,account_code),
    INDEX idx_ga_parent (parent_account_sid),
    INDEX idx_ga_class (account_class),
    INDEX idx_ga_type (account_type),
    INDEX idx_ga_status (account_status),
    CHECK (account_level > 0),
    CHECK (effective_end_date IS NULL OR effective_end_date >= effective_start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='會計科目';

CREATE TABLE IF NOT EXISTS gl_ledger (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '帳簿序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    ledger_code VARCHAR(100) NOT NULL COMMENT '帳簿代碼',
    ledger_name VARCHAR(200) NOT NULL COMMENT '帳簿名稱',
    company_sid VARCHAR(32) NOT NULL COMMENT '公司序號',
    ledger_type VARCHAR(30) NOT NULL COMMENT 'PRIMARY;TAX;MANAGEMENT;CONSOLIDATION;PROJECT',
    accounting_standard VARCHAR(30) NOT NULL DEFAULT 'IFRS' COMMENT '會計準則',
    base_currency_sid VARCHAR(32) NOT NULL COMMENT '本位幣序號',
    fiscal_calendar_sid VARCHAR(32) NOT NULL COMMENT '年度曆序號',
    chart_of_account_sid VARCHAR(32) NOT NULL COMMENT '科目表序號',
    retained_earnings_account_sid VARCHAR(32) NULL COMMENT '保留盈餘科目',
    rounding_account_sid VARCHAR(32) NULL COMMENT '尾差科目',
    exchange_gain_account_sid VARCHAR(32) NULL COMMENT '匯兌收益科目',
    exchange_loss_account_sid VARCHAR(32) NULL COMMENT '匯兌損失科目',
    auto_post_enabled TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否自動過帳',
    ledger_status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE;CLOSED;INACTIVE',
    avalible VARCHAR(2) NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark TEXT NULL COMMENT '備註',
    UNIQUE KEY uk_gl_code (ledger_code),
    INDEX idx_gl_company (company_sid),
    INDEX idx_gl_type (ledger_type),
    INDEX idx_gl_status (ledger_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='總帳帳簿';

CREATE TABLE IF NOT EXISTS gl_account_mapping (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '科目映射序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    ledger_sid VARCHAR(32) NOT NULL COMMENT '帳簿序號',
    source_module VARCHAR(30) NOT NULL COMMENT 'SALES;PURCHASE;INVENTORY;PAYMENT;EXPENSE;TAX;PROJECT',
    transaction_type VARCHAR(100) NOT NULL COMMENT '交易類型代碼',
    debit_account_sid VARCHAR(32) NULL COMMENT '借方科目',
    credit_account_sid VARCHAR(32) NULL COMMENT '貸方科目',
    tax_account_sid VARCHAR(32) NULL COMMENT '稅額科目',
    dimension_rule JSON NULL COMMENT '維度規則',
    priority INT NOT NULL DEFAULT 100 COMMENT '優先順序',
    effective_start_date DATE NOT NULL COMMENT '生效日',
    effective_end_date DATE NULL COMMENT '失效日',
    mapping_status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE;INACTIVE',
    UNIQUE KEY uk_gam_rule (ledger_sid,source_module,transaction_type,priority,effective_start_date),
    INDEX idx_gam_module (source_module),
    INDEX idx_gam_type (transaction_type),
    CHECK (priority >= 0), CHECK (effective_end_date IS NULL OR effective_end_date >= effective_start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='交易科目映射';

CREATE TABLE IF NOT EXISTS gl_journal_batch (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '傳票批次序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    batch_no VARCHAR(100) NOT NULL COMMENT '批次編號',
    ledger_sid VARCHAR(32) NOT NULL COMMENT '帳簿序號',
    accounting_period_sid VARCHAR(32) NOT NULL COMMENT '會計期間序號',
    batch_type VARCHAR(30) NOT NULL COMMENT 'AUTO;MANUAL;IMPORT;CLOSING;REVERSAL;ADJUSTMENT',
    source_module VARCHAR(30) NULL COMMENT '來源模組',
    journal_count INT NOT NULL DEFAULT 0 COMMENT '傳票數量',
    total_debit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '借方總額',
    total_credit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '貸方總額',
    batch_status VARCHAR(30) NOT NULL DEFAULT 'OPEN' COMMENT 'OPEN;VALIDATING;READY;POSTING;POSTED;FAILED;CANCELLED',
    created_user_sid VARCHAR(32) NULL COMMENT '建立人員',
    posted_user_sid VARCHAR(32) NULL COMMENT '過帳人員',
    posted_date DATETIME NULL COMMENT '過帳時間',
    error_message LONGTEXT NULL COMMENT '錯誤訊息',
    UNIQUE KEY uk_gjb_no (batch_no),
    INDEX idx_gjb_ledger (ledger_sid),
    INDEX idx_gjb_period (accounting_period_sid),
    INDEX idx_gjb_status (batch_status),
    CHECK (journal_count >= 0), CHECK (total_debit_amount >= 0), CHECK (total_credit_amount >= 0),
    CHECK (total_debit_amount = total_credit_amount)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='傳票批次';

CREATE TABLE IF NOT EXISTS gl_journal (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '正式傳票序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    journal_no VARCHAR(100) NOT NULL COMMENT '傳票編號',
    journal_batch_sid VARCHAR(32) NULL COMMENT '傳票批次序號',
    ledger_sid VARCHAR(32) NOT NULL COMMENT '帳簿序號',
    accounting_period_sid VARCHAR(32) NOT NULL COMMENT '會計期間序號',
    journal_type VARCHAR(30) NOT NULL COMMENT 'STANDARD;ADJUSTMENT;REVERSAL;OPENING;CLOSING;INTERCOMPANY;ALLOCATE',
    source_module VARCHAR(30) NOT NULL COMMENT '來源模組',
    source_type VARCHAR(50) NOT NULL COMMENT '來源資料類型',
    source_sid VARCHAR(32) NOT NULL COMMENT '來源資料序號',
    accounting_request_sid VARCHAR(32) NULL COMMENT 'AccountingDB傳票請求序號',
    journal_date DATE NOT NULL COMMENT '傳票日期',
    posting_date DATE NOT NULL COMMENT '過帳日期',
    currency_sid VARCHAR(32) NOT NULL COMMENT '交易幣別',
    exchange_rate DECIMAL(20,10) NOT NULL DEFAULT 1 COMMENT '匯率',
    total_debit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '借方總額',
    total_credit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '貸方總額',
    description VARCHAR(1000) NULL COMMENT '摘要',
    reversal_journal_sid VARCHAR(32) NULL COMMENT '沖回傳票序號',
    reverse_date DATE NULL COMMENT '預定沖回日',
    auto_reverse_mark TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否自動沖回',
    journal_status VARCHAR(30) NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT;VALIDATING;APPROVED;POSTED;REVERSED;FAILED;CANCELLED',
    workflow_instance_sid VARCHAR(32) NULL COMMENT '簽核流程序號',
    approved_user_sid VARCHAR(32) NULL COMMENT '核准人員',
    posted_user_sid VARCHAR(32) NULL COMMENT '過帳人員',
    posted_date DATETIME NULL COMMENT '實際過帳時間',
    idempotency_key VARCHAR(200) NOT NULL COMMENT '冪等Key',
    correlation_id VARCHAR(100) NULL COMMENT '跨服務關聯ID',
    version_no BIGINT UNSIGNED NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    avalible VARCHAR(2) NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark TEXT NULL COMMENT '備註',
    UNIQUE KEY uk_gj_no (journal_no),
    UNIQUE KEY uk_gj_idempotency (idempotency_key),
    INDEX idx_gj_ledger (ledger_sid), INDEX idx_gj_period (accounting_period_sid),
    INDEX idx_gj_source (source_module,source_type,source_sid),
    INDEX idx_gj_posting_date (posting_date), INDEX idx_gj_status (journal_status),
    CHECK (exchange_rate > 0), CHECK (total_debit_amount >= 0), CHECK (total_credit_amount >= 0),
    CHECK (total_debit_amount = total_credit_amount)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='正式會計傳票';

CREATE TABLE IF NOT EXISTS gl_journal_line (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '傳票分錄序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    journal_nid BIGINT UNSIGNED NOT NULL COMMENT '傳票流水號',
    line_no INT NOT NULL COMMENT '分錄行號',
    account_sid VARCHAR(32) NOT NULL COMMENT '會計科目序號',
    debit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '交易幣借方',
    credit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '交易幣貸方',
    base_debit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '本位幣借方',
    base_credit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '本位幣貸方',
    currency_sid VARCHAR(32) NOT NULL COMMENT '交易幣別',
    exchange_rate DECIMAL(20,10) NOT NULL DEFAULT 1 COMMENT '匯率',
    description VARCHAR(1000) NULL COMMENT '分錄摘要',
    party_sid VARCHAR(32) NULL COMMENT '往來Party',
    company_sid VARCHAR(32) NULL COMMENT '公司',
    business_unit_sid VARCHAR(32) NULL COMMENT '營運單位',
    department_sid VARCHAR(32) NULL COMMENT '部門',
    cost_center_sid VARCHAR(32) NULL COMMENT '成本中心',
    project_sid VARCHAR(32) NULL COMMENT '專案',
    site_sid VARCHAR(32) NULL COMMENT '工地',
    wbs_sid VARCHAR(32) NULL COMMENT 'WBS',
    contract_sid VARCHAR(32) NULL COMMENT '合約',
    item_sid VARCHAR(32) NULL COMMENT 'Item',
    tax_sid VARCHAR(32) NULL COMMENT '稅別',
    bank_account_sid VARCHAR(32) NULL COMMENT '銀行帳戶',
    reference_type VARCHAR(50) NULL COMMENT '來源明細類型',
    reference_sid VARCHAR(32) NULL COMMENT '來源明細序號',
    reconciliation_status VARCHAR(20) NOT NULL DEFAULT 'NOT_REQUIRED' COMMENT 'NOT_REQUIRED;OPEN;PARTIAL;CLEARED',
    cleared_date DATE NULL COMMENT '清帳日期',
    CONSTRAINT fk_gjl_journal FOREIGN KEY (journal_nid) REFERENCES gl_journal(nid),
    UNIQUE KEY uk_gjl_line (journal_nid,line_no),
    INDEX idx_gjl_account (account_sid), INDEX idx_gjl_party (party_sid),
    INDEX idx_gjl_project (project_sid), INDEX idx_gjl_wbs (wbs_sid),
    INDEX idx_gjl_cost_center (cost_center_sid), INDEX idx_gjl_reference (reference_type,reference_sid),
    CHECK (line_no > 0), CHECK (debit_amount >= 0), CHECK (credit_amount >= 0),
    CHECK (base_debit_amount >= 0), CHECK (base_credit_amount >= 0), CHECK (exchange_rate > 0),
    CHECK ((debit_amount > 0 AND credit_amount = 0) OR (credit_amount > 0 AND debit_amount = 0)),
    CHECK ((base_debit_amount > 0 AND base_credit_amount = 0) OR (base_credit_amount > 0 AND base_debit_amount = 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='正式會計分錄';

CREATE TABLE IF NOT EXISTS gl_journal_validation (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '傳票驗證序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    journal_sid VARCHAR(32) NOT NULL COMMENT '傳票序號',
    validation_type VARCHAR(30) NOT NULL COMMENT 'BALANCE;PERIOD;ACCOUNT;DIMENSION;CURRENCY;DUPLICATE;POLICY',
    validation_result VARCHAR(20) NOT NULL COMMENT 'PASS;WARNING;FAIL',
    validation_code VARCHAR(100) NULL COMMENT '驗證代碼',
    validation_message VARCHAR(2000) NULL COMMENT '驗證訊息',
    line_sid VARCHAR(32) NULL COMMENT '分錄序號',
    validation_data JSON NULL COMMENT '驗證資料',
    INDEX idx_gjv_journal (journal_sid), INDEX idx_gjv_type (validation_type), INDEX idx_gjv_result (validation_result)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='傳票驗證紀錄';

CREATE TABLE IF NOT EXISTS gl_posting_lock (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '過帳鎖序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    ledger_sid VARCHAR(32) NOT NULL COMMENT '帳簿序號',
    accounting_period_sid VARCHAR(32) NOT NULL COMMENT '會計期間序號',
    lock_scope VARCHAR(30) NOT NULL COMMENT 'ALL;MODULE;ACCOUNT;USER',
    scope_value VARCHAR(100) NULL COMMENT '範圍值',
    lock_reason TEXT NOT NULL COMMENT '鎖定原因',
    locked_user_sid VARCHAR(32) NULL COMMENT '鎖定人員',
    lock_start_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '開始時間',
    lock_end_date DATETIME NULL COMMENT '結束時間',
    lock_status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE;RELEASED;EXPIRED',
    UNIQUE KEY uk_gpl_lock (ledger_sid,accounting_period_sid,lock_scope,scope_value,lock_status),
    INDEX idx_gpl_status (lock_status),
    CHECK (lock_end_date IS NULL OR lock_end_date >= lock_start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='期間與模組過帳鎖';

CREATE TABLE IF NOT EXISTS gl_account_balance (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '科目餘額序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    ledger_sid VARCHAR(32) NOT NULL COMMENT '帳簿序號',
    accounting_period_sid VARCHAR(32) NOT NULL COMMENT '會計期間序號',
    account_sid VARCHAR(32) NOT NULL COMMENT '會計科目序號',
    currency_sid VARCHAR(32) NOT NULL COMMENT '幣別序號',
    opening_debit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期初借方',
    opening_credit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期初貸方',
    period_debit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '本期借方',
    period_credit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '本期貸方',
    closing_debit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期末借方',
    closing_credit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期末貸方',
    transaction_count BIGINT UNSIGNED NOT NULL DEFAULT 0 COMMENT '交易筆數',
    last_posting_date DATETIME NULL COMMENT '最後過帳時間',
    version_no BIGINT UNSIGNED NOT NULL DEFAULT 0 COMMENT '樂觀鎖版本',
    UNIQUE KEY uk_gab_balance (ledger_sid,accounting_period_sid,account_sid,currency_sid),
    INDEX idx_gab_account (account_sid), INDEX idx_gab_period (accounting_period_sid),
    CHECK (opening_debit_amount >= 0), CHECK (opening_credit_amount >= 0),
    CHECK (period_debit_amount >= 0), CHECK (period_credit_amount >= 0),
    CHECK (closing_debit_amount >= 0), CHECK (closing_credit_amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='科目期間餘額';

CREATE TABLE IF NOT EXISTS gl_dimension_balance (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '維度餘額序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    ledger_sid VARCHAR(32) NOT NULL COMMENT '帳簿序號',
    accounting_period_sid VARCHAR(32) NOT NULL COMMENT '期間序號',
    account_sid VARCHAR(32) NOT NULL COMMENT '科目序號',
    dimension_type VARCHAR(30) NOT NULL COMMENT 'PARTY;PROJECT;SITE;WBS;COST_CENTER;DEPARTMENT;BUSINESS_UNIT;CONTRACT;ITEM;BANK_ACCOUNT',
    dimension_sid VARCHAR(32) NOT NULL COMMENT '維度序號',
    currency_sid VARCHAR(32) NOT NULL COMMENT '幣別序號',
    opening_balance DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期初淨額',
    period_debit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '本期借方',
    period_credit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '本期貸方',
    closing_balance DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期末淨額',
    transaction_count BIGINT UNSIGNED NOT NULL DEFAULT 0 COMMENT '交易筆數',
    UNIQUE KEY uk_gdb_balance (ledger_sid,accounting_period_sid,account_sid,dimension_type,dimension_sid,currency_sid),
    INDEX idx_gdb_dimension (dimension_type,dimension_sid),
    CHECK (period_debit_amount >= 0), CHECK (period_credit_amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='專案、工地、WBS等維度餘額';

CREATE TABLE IF NOT EXISTS gl_open_item (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '未清項目序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    ledger_sid VARCHAR(32) NOT NULL COMMENT '帳簿序號',
    account_sid VARCHAR(32) NOT NULL COMMENT '清帳科目序號',
    journal_line_sid VARCHAR(32) NOT NULL COMMENT '來源分錄序號',
    party_sid VARCHAR(32) NULL COMMENT 'Party序號',
    document_type VARCHAR(30) NULL COMMENT 'RECEIVABLE;PAYABLE;BANK;ADVANCE;OTHER',
    document_sid VARCHAR(32) NULL COMMENT '來源單據序號',
    document_no VARCHAR(100) NULL COMMENT '來源單號',
    transaction_date DATE NOT NULL COMMENT '交易日期',
    due_date DATE NULL COMMENT '到期日',
    currency_sid VARCHAR(32) NOT NULL COMMENT '幣別',
    original_amount DECIMAL(20,4) NOT NULL COMMENT '原始金額',
    cleared_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '已清金額',
    outstanding_amount DECIMAL(20,4) NOT NULL COMMENT '未清金額',
    debit_credit VARCHAR(10) NOT NULL COMMENT 'DEBIT;CREDIT',
    clearing_status VARCHAR(20) NOT NULL DEFAULT 'OPEN' COMMENT 'OPEN;PARTIAL;CLEARED;WRITTEN_OFF;REVERSED',
    cleared_date DATE NULL COMMENT '清帳日期',
    INDEX idx_goi_account (account_sid), INDEX idx_goi_party (party_sid),
    INDEX idx_goi_document (document_type,document_sid), INDEX idx_goi_status (clearing_status),
    CHECK (original_amount > 0), CHECK (cleared_amount >= 0), CHECK (outstanding_amount >= 0),
    CHECK (cleared_amount + outstanding_amount <= original_amount)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='未清項目';

CREATE TABLE IF NOT EXISTS gl_clearing (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '清帳序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    clearing_no VARCHAR(100) NOT NULL COMMENT '清帳編號',
    ledger_sid VARCHAR(32) NOT NULL COMMENT '帳簿序號',
    account_sid VARCHAR(32) NOT NULL COMMENT '清帳科目',
    party_sid VARCHAR(32) NULL COMMENT 'Party序號',
    clearing_date DATE NOT NULL COMMENT '清帳日期',
    currency_sid VARCHAR(32) NOT NULL COMMENT '幣別',
    cleared_amount DECIMAL(20,4) NOT NULL COMMENT '清帳金額',
    difference_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '差額',
    difference_account_sid VARCHAR(32) NULL COMMENT '差額科目',
    clearing_journal_sid VARCHAR(32) NULL COMMENT '清帳傳票',
    clearing_type VARCHAR(20) NOT NULL COMMENT 'AUTO;MANUAL;WRITE_OFF;NETTING',
    clearing_status VARCHAR(20) NOT NULL DEFAULT 'COMPLETED' COMMENT 'DRAFT;COMPLETED;REVERSED',
    operator_user_sid VARCHAR(32) NULL COMMENT '操作人員',
    UNIQUE KEY uk_gc_no (clearing_no),
    INDEX idx_gc_account (account_sid), INDEX idx_gc_party (party_sid), INDEX idx_gc_status (clearing_status),
    CHECK (cleared_amount > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='清帳主檔';

CREATE TABLE IF NOT EXISTS gl_clearing_item (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '清帳明細序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    clearing_nid BIGINT UNSIGNED NOT NULL COMMENT '清帳流水號',
    line_no INT NOT NULL COMMENT '行號',
    open_item_sid VARCHAR(32) NOT NULL COMMENT '未清項目序號',
    applied_amount DECIMAL(20,4) NOT NULL COMMENT '清帳金額',
    exchange_gain_loss DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '匯兌損益',
    CONSTRAINT fk_gci_clearing FOREIGN KEY (clearing_nid) REFERENCES gl_clearing(nid),
    UNIQUE KEY uk_gci_line (clearing_nid,line_no),
    INDEX idx_gci_open_item (open_item_sid),
    CHECK (line_no > 0), CHECK (applied_amount > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='清帳明細';

CREATE TABLE IF NOT EXISTS gl_allocation_rule (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '分攤規則序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    rule_code VARCHAR(100) NOT NULL COMMENT '規則代碼',
    rule_name VARCHAR(200) NOT NULL COMMENT '規則名稱',
    ledger_sid VARCHAR(32) NOT NULL COMMENT '帳簿序號',
    source_account_sid VARCHAR(32) NOT NULL COMMENT '來源科目',
    allocation_basis VARCHAR(30) NOT NULL COMMENT 'FIXED_PERCENT;REVENUE;HEADCOUNT;AREA;QUANTITY;COST;CUSTOM',
    source_dimension_type VARCHAR(30) NULL COMMENT '來源維度類型',
    source_dimension_sid VARCHAR(32) NULL COMMENT '來源維度序號',
    target_config JSON NOT NULL COMMENT '目標與比例設定',
    offset_account_sid VARCHAR(32) NULL COMMENT '沖抵科目',
    effective_start_date DATE NOT NULL COMMENT '生效日',
    effective_end_date DATE NULL COMMENT '失效日',
    rule_status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE;INACTIVE',
    avalible VARCHAR(2) NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    UNIQUE KEY uk_gar_code (rule_code),
    INDEX idx_gar_ledger (ledger_sid), INDEX idx_gar_basis (allocation_basis),
    CHECK (effective_end_date IS NULL OR effective_end_date >= effective_start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='費用與成本分攤規則';

CREATE TABLE IF NOT EXISTS gl_allocation_run (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '分攤執行序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    allocation_no VARCHAR(100) NOT NULL COMMENT '執行編號',
    allocation_rule_sid VARCHAR(32) NOT NULL COMMENT '規則序號',
    accounting_period_sid VARCHAR(32) NOT NULL COMMENT '會計期間',
    source_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '來源金額',
    allocated_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '已分攤金額',
    journal_sid VARCHAR(32) NULL COMMENT '分攤傳票序號',
    run_status VARCHAR(20) NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING;CALCULATING;READY;POSTED;FAILED;REVERSED',
    executed_user_sid VARCHAR(32) NULL COMMENT '執行人員',
    completed_date DATETIME NULL COMMENT '完成時間',
    error_message LONGTEXT NULL COMMENT '錯誤訊息',
    UNIQUE KEY uk_garun_no (allocation_no),
    INDEX idx_garun_rule (allocation_rule_sid), INDEX idx_garun_period (accounting_period_sid), INDEX idx_garun_status (run_status),
    CHECK (source_amount >= 0), CHECK (allocated_amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='分攤執行';

CREATE TABLE IF NOT EXISTS gl_close_process (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '關帳流程序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    close_no VARCHAR(100) NOT NULL COMMENT '關帳編號',
    ledger_sid VARCHAR(32) NOT NULL COMMENT '帳簿序號',
    accounting_period_sid VARCHAR(32) NOT NULL COMMENT '會計期間',
    close_type VARCHAR(20) NOT NULL COMMENT 'MONTH;QUARTER;YEAR;SOFT;HARD',
    close_status VARCHAR(30) NOT NULL DEFAULT 'PLANNED' COMMENT 'PLANNED;VALIDATING;PROCESSING;WAITING_APPROVAL;COMPLETED;FAILED;REOPENED;CANCELLED',
    checklist_total INT NOT NULL DEFAULT 0 COMMENT '檢核總數',
    checklist_completed INT NOT NULL DEFAULT 0 COMMENT '完成數',
    initiated_user_sid VARCHAR(32) NULL COMMENT '發起人',
    approved_user_sid VARCHAR(32) NULL COMMENT '核准人',
    workflow_instance_sid VARCHAR(32) NULL COMMENT '簽核流程',
    started_date DATETIME NULL COMMENT '開始時間',
    completed_date DATETIME NULL COMMENT '完成時間',
    reopen_reason TEXT NULL COMMENT '重開原因',
    UNIQUE KEY uk_gcp_no (close_no),
    UNIQUE KEY uk_gcp_period_type (ledger_sid,accounting_period_sid,close_type),
    INDEX idx_gcp_status (close_status),
    CHECK (checklist_total >= 0), CHECK (checklist_completed >= 0), CHECK (checklist_completed <= checklist_total)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='月結季結年結流程';

CREATE TABLE IF NOT EXISTS gl_close_checklist (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '關帳檢核序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    close_process_nid BIGINT UNSIGNED NOT NULL COMMENT '關帳流程流水號',
    step_no INT NOT NULL COMMENT '步驟序號',
    step_code VARCHAR(100) NOT NULL COMMENT '步驟代碼',
    step_name VARCHAR(300) NOT NULL COMMENT '步驟名稱',
    module_code VARCHAR(30) NULL COMMENT 'SALES;PURCHASE;INVENTORY;PAYMENT;TAX;AR;AP;PROJECT;GL',
    required_mark TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否必須',
    auto_check_mark TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否自動檢核',
    check_result VARCHAR(20) NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING;PASS;WARNING;FAIL;WAIVED',
    result_message TEXT NULL COMMENT '結果訊息',
    responsible_user_sid VARCHAR(32) NULL COMMENT '負責人',
    completed_date DATETIME NULL COMMENT '完成時間',
    waived_user_sid VARCHAR(32) NULL COMMENT '豁免人',
    waived_reason TEXT NULL COMMENT '豁免原因',
    CONSTRAINT fk_gcc_close FOREIGN KEY (close_process_nid) REFERENCES gl_close_process(nid),
    UNIQUE KEY uk_gcc_step (close_process_nid,step_no),
    INDEX idx_gcc_module (module_code), INDEX idx_gcc_result (check_result),
    CHECK (step_no > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='關帳檢核清單';

CREATE TABLE IF NOT EXISTS gl_closing_entry (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '期末結轉序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    close_process_sid VARCHAR(32) NOT NULL COMMENT '關帳流程序號',
    closing_type VARCHAR(30) NOT NULL COMMENT 'INCOME_STATEMENT;FX_REVALUATION;INVENTORY_ADJUSTMENT;ACCRUAL;DEFERRED;DEPRECIATION;OTHER',
    source_account_sid VARCHAR(32) NULL COMMENT '來源科目',
    target_account_sid VARCHAR(32) NULL COMMENT '目標科目',
    amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '結轉金額',
    journal_sid VARCHAR(32) NULL COMMENT '結轉傳票',
    reversal_required TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否次期沖回',
    reversal_date DATE NULL COMMENT '沖回日期',
    entry_status VARCHAR(20) NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING;POSTED;REVERSED;FAILED',
    INDEX idx_gce_close (close_process_sid), INDEX idx_gce_type (closing_type), INDEX idx_gce_status (entry_status),
    CHECK (amount >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='期末調整與結轉';

CREATE TABLE IF NOT EXISTS gl_trial_balance_snapshot (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '試算表快照序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    snapshot_no VARCHAR(100) NOT NULL COMMENT '快照編號',
    ledger_sid VARCHAR(32) NOT NULL COMMENT '帳簿序號',
    accounting_period_sid VARCHAR(32) NOT NULL COMMENT '會計期間',
    snapshot_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '快照時間',
    dimension_filter JSON NULL COMMENT '維度篩選',
    total_opening_debit DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期初借方總額',
    total_opening_credit DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期初貸方總額',
    total_period_debit DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '本期借方總額',
    total_period_credit DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '本期貸方總額',
    total_closing_debit DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期末借方總額',
    total_closing_credit DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期末貸方總額',
    balanced_mark TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否平衡',
    generated_user_sid VARCHAR(32) NULL COMMENT '產生人員',
    UNIQUE KEY uk_gtbs_no (snapshot_no),
    INDEX idx_gtbs_ledger (ledger_sid), INDEX idx_gtbs_period (accounting_period_sid), INDEX idx_gtbs_balanced (balanced_mark)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='試算表快照';

CREATE TABLE IF NOT EXISTS gl_trial_balance_item (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '試算表明細序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    trial_balance_snapshot_nid BIGINT UNSIGNED NOT NULL COMMENT '試算表快照流水號',
    line_no INT NOT NULL COMMENT '行號',
    account_sid VARCHAR(32) NOT NULL COMMENT '科目序號',
    opening_debit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期初借方',
    opening_credit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期初貸方',
    period_debit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '本期借方',
    period_credit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '本期貸方',
    closing_debit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期末借方',
    closing_credit_amount DECIMAL(20,4) NOT NULL DEFAULT 0 COMMENT '期末貸方',
    CONSTRAINT fk_gtbi_snapshot FOREIGN KEY (trial_balance_snapshot_nid) REFERENCES gl_trial_balance_snapshot(nid),
    UNIQUE KEY uk_gtbi_line (trial_balance_snapshot_nid,line_no),
    INDEX idx_gtbi_account (account_sid),
    CHECK (line_no > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='試算表明細';

CREATE TABLE IF NOT EXISTS gl_financial_statement (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '財務報表序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    statement_no VARCHAR(100) NOT NULL COMMENT '報表編號',
    statement_type VARCHAR(30) NOT NULL COMMENT 'BALANCE_SHEET;INCOME_STATEMENT;CASH_FLOW;EQUITY;PROJECT_PNL',
    ledger_sid VARCHAR(32) NOT NULL COMMENT '帳簿序號',
    period_start_sid VARCHAR(32) NOT NULL COMMENT '起始期間',
    period_end_sid VARCHAR(32) NOT NULL COMMENT '結束期間',
    comparison_period_start_sid VARCHAR(32) NULL COMMENT '比較期起始',
    comparison_period_end_sid VARCHAR(32) NULL COMMENT '比較期結束',
    currency_sid VARCHAR(32) NOT NULL COMMENT '報表幣別',
    dimension_filter JSON NULL COMMENT '維度篩選',
    statement_data JSON NOT NULL COMMENT '完整報表快照',
    generated_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '產生時間',
    generated_user_sid VARCHAR(32) NULL COMMENT '產生人員',
    statement_status VARCHAR(20) NOT NULL DEFAULT 'DRAFT' COMMENT 'DRAFT;FINAL;ARCHIVED',
    UNIQUE KEY uk_gfs_no (statement_no),
    INDEX idx_gfs_type (statement_type), INDEX idx_gfs_ledger (ledger_sid), INDEX idx_gfs_status (statement_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='財務報表快照';

CREATE TABLE IF NOT EXISTS gl_intercompany_transaction (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '關係企業交易序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    transaction_no VARCHAR(100) NOT NULL COMMENT '交易編號',
    source_company_sid VARCHAR(32) NOT NULL COMMENT '來源公司',
    target_company_sid VARCHAR(32) NOT NULL COMMENT '目標公司',
    source_journal_sid VARCHAR(32) NULL COMMENT '來源公司傳票',
    target_journal_sid VARCHAR(32) NULL COMMENT '目標公司傳票',
    transaction_type VARCHAR(30) NOT NULL COMMENT 'SALE;PURCHASE;SERVICE;LOAN;FEE;TRANSFER;OTHER',
    transaction_date DATE NOT NULL COMMENT '交易日期',
    currency_sid VARCHAR(32) NOT NULL COMMENT '幣別',
    transaction_amount DECIMAL(20,4) NOT NULL COMMENT '交易金額',
    matching_status VARCHAR(20) NOT NULL DEFAULT 'UNMATCHED' COMMENT 'UNMATCHED;PARTIAL;MATCHED;DIFFERENCE',
    elimination_required TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否需合併沖銷',
    elimination_journal_sid VARCHAR(32) NULL COMMENT '沖銷傳票',
    UNIQUE KEY uk_gict_no (transaction_no),
    INDEX idx_gict_source_company (source_company_sid), INDEX idx_gict_target_company (target_company_sid),
    INDEX idx_gict_status (matching_status),
    CHECK (source_company_sid <> target_company_sid), CHECK (transaction_amount > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='關係企業往來與合併沖銷';

CREATE TABLE IF NOT EXISTS gl_audit_log (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '總帳稽核序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '操作時間',
    entity_type VARCHAR(30) NOT NULL COMMENT 'LEDGER;PERIOD;ACCOUNT;JOURNAL;JOURNAL_LINE;BALANCE;CLEARING;CLOSE;STATEMENT',
    entity_sid VARCHAR(32) NOT NULL COMMENT '實體序號',
    action_type VARCHAR(30) NOT NULL COMMENT 'CREATE;UPDATE;APPROVE;POST;REVERSE;CLOSE;REOPEN;EXPORT;VIEW',
    before_data JSON NULL COMMENT '異動前資料',
    after_data JSON NULL COMMENT '異動後資料',
    operator_user_sid VARCHAR(32) NULL COMMENT '操作人員',
    ip_address VARCHAR(50) NULL COMMENT 'IP',
    correlation_id VARCHAR(100) NULL COMMENT '關聯ID',
    INDEX idx_gal_entity (entity_type,entity_sid), INDEX idx_gal_action (action_type), INDEX idx_gal_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='總帳稽核紀錄';

CREATE TABLE IF NOT EXISTS gl_status_history (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '總帳狀態歷程序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '異動時間',
    entity_type VARCHAR(30) NOT NULL COMMENT 'LEDGER;PERIOD;ACCOUNT;JOURNAL_BATCH;JOURNAL;OPEN_ITEM;CLEARING;ALLOCATION;CLOSE;STATEMENT',
    entity_sid VARCHAR(32) NOT NULL COMMENT '實體序號',
    old_status VARCHAR(30) NULL COMMENT '原狀態',
    new_status VARCHAR(30) NOT NULL COMMENT '新狀態',
    event_code VARCHAR(100) NULL COMMENT '事件代碼',
    operator_user_sid VARCHAR(32) NULL COMMENT '操作人員',
    reason TEXT NULL COMMENT '原因',
    correlation_id VARCHAR(100) NULL COMMENT '關聯ID',
    INDEX idx_gsh_entity (entity_type,entity_sid), INDEX idx_gsh_new_status (new_status), INDEX idx_gsh_create_date (create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='總帳狀態歷程';

CREATE TABLE IF NOT EXISTS gl_event (
    nid BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid VARCHAR(32) NOT NULL UNIQUE COMMENT '總帳事件序號',
    create_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    entity_type VARCHAR(30) NOT NULL COMMENT 'LEDGER;PERIOD;JOURNAL;BALANCE;CLEARING;CLOSE;STATEMENT',
    entity_sid VARCHAR(32) NOT NULL COMMENT '實體序號',
    event_code VARCHAR(120) NOT NULL COMMENT '事件代碼',
    event_version INT NOT NULL DEFAULT 1 COMMENT '事件版本',
    event_data JSON NULL COMMENT '事件內容',
    source_event_id VARCHAR(100) NULL COMMENT '來源事件ID',
    correlation_id VARCHAR(100) NULL COMMENT '關聯ID',
    causation_id VARCHAR(100) NULL COMMENT '因果事件ID',
    outbox_event_sid VARCHAR(32) NULL COMMENT 'IntegrationDB Outbox序號',
    process_status VARCHAR(20) NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING;SUCCESS;FAILED;IGNORED',
    processed_date DATETIME NULL COMMENT '處理時間',
    error_message TEXT NULL COMMENT '錯誤訊息',
    UNIQUE KEY uk_ge_source_event (source_event_id),
    INDEX idx_ge_entity (entity_type,entity_sid), INDEX idx_ge_event_code (event_code), INDEX idx_ge_status (process_status),
    CHECK (event_version > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='總帳領域事件';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
