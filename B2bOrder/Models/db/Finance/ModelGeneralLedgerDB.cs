using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Finance.Models
{
    #region 會計年度曆 (Fiscal Calendar)
    [Table("gl_fiscal_calendar")]
    public class FiscalCalendarModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; } // 流水號

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!; // 會計年度曆序號

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now; // 建立日期

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; } // 修改日期

        [Required]
        [MaxLength(100)]
        [Column("calendar_code")]
        public string CalendarCode { get; set; } = null!; // 年度曆代碼

        [Required]
        [MaxLength(200)]
        [Column("calendar_name")]
        public string CalendarName { get; set; } = null!; // 年度曆名稱

        [Column("fiscal_year_start_month")]
        public int FiscalYearStartMonth { get; set; } = 1; // 年度起始月份

        [Column("period_count")]
        public int PeriodCount { get; set; } = 12; // 一般期間數

        [Column("adjustment_period_count")]
        public int AdjustmentPeriodCount { get; set; } = 1; // 調整期間數

        [Required]
        [MaxLength(20)]
        [Column("calendar_type")]
        public string CalendarType { get; set; } = "MONTHLY"; // 曆別類型 (MONTHLY;FOUR_FOUR_FIVE;CUSTOM)

        [Required]
        [MaxLength(20)]
        [Column("calendar_status")]
        public string CalendarStatus { get; set; } = "ACTIVE"; // 曆別狀態 (ACTIVE;INACTIVE)

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y"; // 可用狀態 (Y可用;D刪除;W停用)

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; } // 備註
    }
    #endregion

    #region 會計期間 (Accounting Period)
    [Table("gl_accounting_period")]
    public class AccountingPeriodModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; } // 流水號

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!; // 會計期間序號

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now; // 建立日期

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; } // 修改日期

        [Required]
        [MaxLength(32)]
        [Column("fiscal_calendar_sid")]
        public string FiscalCalendarSid { get; set; } = null!; // 年度曆序號

        [Column("fiscal_year")]
        public int FiscalYear { get; set; } // 會計年度

        [Column("period_no")]
        public int PeriodNo { get; set; } // 期間序號

        [Required]
        [MaxLength(100)]
        [Column("period_name")]
        public string PeriodName { get; set; } = null!; // 期間名稱

        [Required]
        [MaxLength(20)]
        [Column("period_type")]
        public string PeriodType { get; set; } = "NORMAL"; // 期間類型 (NORMAL;ADJUSTMENT;OPENING;CLOSING)

        [Column("start_date", TypeName = "DATE")]
        public DateTime StartDate { get; set; } // 開始日

        [Column("end_date", TypeName = "DATE")]
        public DateTime EndDate { get; set; } // 結束日

        [Required]
        [MaxLength(20)]
        [Column("posting_status")]
        public string PostingStatus { get; set; } = "OPEN"; // 過帳狀態 (FUTURE;OPEN;SOFT_CLOSED;HARD_CLOSED;REOPENED)

        [Column("opened_date")]
        public DateTime? OpenedDate { get; set; } // 開帳時間

        [Column("closed_date")]
        public DateTime? ClosedDate { get; set; } // 關帳時間

        [MaxLength(32)]
        [Column("closed_user_sid")]
        public string? ClosedUserSid { get; set; } // 關帳人員序號

        [Column("reopen_reason", TypeName = "TEXT")]
        public string? ReopenReason { get; set; } // 重開原因
    }
    #endregion

    #region 會計科目表 (Chart of Account)
    [Table("gl_chart_of_account")]
    public class ChartOfAccountModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; } // 流水號

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!; // 科目表序號

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now; // 建立日期

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; } // 修改日期

        [Required]
        [MaxLength(100)]
        [Column("chart_code")]
        public string ChartCode { get; set; } = null!; // 科目表代碼

        [Required]
        [MaxLength(200)]
        [Column("chart_name")]
        public string ChartName { get; set; } = null!; // 科目表名稱

        [MaxLength(32)]
        [Column("country_sid")]
        public string? CountrySid { get; set; } // 國家序號

        [Required]
        [MaxLength(30)]
        [Column("accounting_standard")]
        public string AccountingStandard { get; set; } = "IFRS"; // 會計準則 (IFRS;LOCAL_GAAP;TAX_BASIS;MANAGEMENT)

        [Required]
        [MaxLength(20)]
        [Column("chart_status")]
        public string ChartStatus { get; set; } = "ACTIVE"; // 科目表狀態 (ACTIVE;INACTIVE)

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y"; // 可用狀態 (Y可用;D刪除;W停用)

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; } // 備註
    }
    #endregion

    #region 會計科目 (Account)
    [Table("gl_account")]
    public class AccountModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; } // 流水號

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!; // 會計科目序號

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now; // 建立日期

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; } // 修改日期

        [Required]
        [MaxLength(32)]
        [Column("chart_of_account_sid")]
        public string ChartOfAccountSid { get; set; } = null!; // 科目表序號

        [Required]
        [MaxLength(100)]
        [Column("account_code")]
        public string AccountCode { get; set; } = null!; // 科目代碼

        [Required]
        [MaxLength(300)]
        [Column("account_name")]
        public string AccountName { get; set; } = null!; // 科目名稱

        [MaxLength(32)]
        [Column("parent_account_sid")]
        public string? ParentAccountSid { get; set; } // 上層科目序號

        [Column("account_level")]
        public int AccountLevel { get; set; } = 1; // 科目層級

        [Required]
        [MaxLength(30)]
        [Column("account_class")]
        public string AccountClass { get; set; } = null!; // 科目大類 (ASSET;LIABILITY;EQUITY;REVENUE;EXPENSE;OTHER)

        [Required]
        [MaxLength(50)]
        [Column("account_type")]
        public string AccountType { get; set; } = null!; // 科目類型

        [Required]
        [MaxLength(10)]
        [Column("normal_balance")]
        public string NormalBalance { get; set; } = null!; // 正常餘額方向 (DEBIT;CREDIT)

        [Column("posting_allowed")]
        public bool PostingAllowed { get; set; } = true; // 是否可直接過帳

        [Column("reconciliation_required")]
        public bool ReconciliationRequired { get; set; } = false; // 是否需清帳

        [Column("party_required")]
        public bool PartyRequired { get; set; } = false; // Party是否必填

        [Column("project_required")]
        public bool ProjectRequired { get; set; } = false; // 專案是否必填

        [Column("cost_center_required")]
        public bool CostCenterRequired { get; set; } = false; // 成本中心是否必填

        [Column("effective_start_date", TypeName = "DATE")]
        public DateTime EffectiveStartDate { get; set; } // 生效日

        [Column("effective_end_date", TypeName = "DATE")]
        public DateTime? EffectiveEndDate { get; set; } // 失效日

        [Required]
        [MaxLength(20)]
        [Column("account_status")]
        public string AccountStatus { get; set; } = "ACTIVE"; // 科目狀態

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y"; // 可用狀態

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; } // 備註
    }
    #endregion

    #region 正式會計傳票 (Journal)
    [Table("gl_journal")]
    public class JournalModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; } // 流水號

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!; // 正式傳票序號

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now; // 建立日期

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; } // 修改日期

        [Required]
        [MaxLength(100)]
        [Column("journal_no")]
        public string JournalNo { get; set; } = null!; // 傳票編號

        [MaxLength(32)]
        [Column("journal_batch_sid")]
        public string? JournalBatchSid { get; set; } // 傳票批次序號

        [Required]
        [MaxLength(32)]
        [Column("ledger_sid")]
        public string LedgerSid { get; set; } = null!; // 帳簿序號

        [Required]
        [MaxLength(32)]
        [Column("accounting_period_sid")]
        public string AccountingPeriodSid { get; set; } = null!; // 會計期間序號

        [Required]
        [MaxLength(30)]
        [Column("journal_type")]
        public string JournalType { get; set; } = null!; // 傳票類型

        [Required]
        [MaxLength(30)]
        [Column("source_module")]
        public string SourceModule { get; set; } = null!; // 來源模組

        [Required]
        [MaxLength(50)]
        [Column("source_type")]
        public string SourceType { get; set; } = null!; // 來源資料類型

        [Required]
        [MaxLength(32)]
        [Column("source_sid")]
        public string SourceSid { get; set; } = null!; // 來源資料序號

        [MaxLength(32)]
        [Column("accounting_request_sid")]
        public string? AccountingRequestSid { get; set; } // AccountingDB傳票請求序號

        [Column("journal_date", TypeName = "DATE")]
        public DateTime JournalDate { get; set; } // 傳票日期

        [Column("posting_date", TypeName = "DATE")]
        public DateTime PostingDate { get; set; } // 過帳日期

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!; // 交易幣別

        [Column("exchange_rate", TypeName = "DECIMAL(20,10)]")]
        public decimal ExchangeRate { get; set; } = 1; // 匯率

        [Column("total_debit_amount", TypeName = "DECIMAL(20,4)]")]
        public decimal TotalDebitAmount { get; set; } = 0; // 借方總額

        [Column("total_credit_amount", TypeName = "DECIMAL(20,4)]")]
        public decimal TotalCreditAmount { get; set; } = 0; // 貸方總額

        [MaxLength(1000)]
        [Column("description")]
        public string? Description { get; set; } // 摘要

        [MaxLength(32)]
        [Column("reversal_journal_sid")]
        public string? ReversalJournalSid { get; set; } // 沖回傳票序號

        [Column("reverse_date", TypeName = "DATE")]
        public DateTime? ReverseDate { get; set; } // 預定沖回日

        [Column("auto_reverse_mark")]
        public bool AutoReverseMark { get; set; } = false; // 是否自動沖回

        [Required]
        [MaxLength(30)]
        [Column("journal_status")]
        public string JournalStatus { get; set; } = "DRAFT"; // 傳票狀態

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; } // 簽核流程序號

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; } // 核准人員

        [MaxLength(32)]
        [Column("posted_user_sid")]
        public string? PostedUserSid { get; set; } // 過帳人員

        [Column("posted_date")]
        public DateTime? PostedDate { get; set; } // 實際過帳時間

        [Required]
        [MaxLength(200)]
        [Column("idempotency_key")]
        public string IdempotencyKey { get; set; } = null!; // 冪等Key

        [MaxLength(100)]
        [Column("correlation_id")]
        public string? CorrelationId { get; set; } // 跨服務關聯ID

        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0; // 樂觀鎖版本

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y"; // 可用狀態

        [Column("remark", TypeName = "TEXT")]
        public string? Remark { get; set; } // 備註
    }
    #endregion

    #region 正式會計分錄 (Journal Line)
    [Table("gl_journal_line")]
    public class JournalLineModel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; } // 流水號

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!; // 傳票分錄序號

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now; // 建立日期

        [Column("journal_nid")]
        public ulong JournalNid { get; set; } // 傳票流水號

        [Column("line_no")]
        public int LineNo { get; set; } // 分錄行號

        [Required]
        [MaxLength(32)]
        [Column("account_sid")]
        public string AccountSid { get; set; } = null!; // 會計科目序號

        [Column("debit_amount", TypeName = "DECIMAL(20,4)]")]
        public decimal DebitAmount { get; set; } = 0; // 交易幣借方

        [Column("credit_amount", TypeName = "DECIMAL(20,4)]")]
        public decimal CreditAmount { get; set; } = 0; // 交易幣貸方

        [Column("base_debit_amount", TypeName = "DECIMAL(20,4)]")]
        public decimal BaseDebitAmount { get; set; } = 0; // 本位幣借方

        [Column("base_credit_amount", TypeName = "DECIMAL(20,4)]")]
        public decimal BaseCreditAmount { get; set; } = 0; // 本位幣貸方

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!; // 交易幣別

        [Column("exchange_rate", TypeName = "DECIMAL(20,10)]")]
        public decimal ExchangeRate { get; set; } = 1; // 匯率

        [MaxLength(1000)]
        [Column("description")]
        public string? Description { get; set; } // 分錄摘要

        [MaxLength(32)]
        [Column("party_sid")]
        public string? PartySid { get; set; } // 往來Party

        [MaxLength(32)]
        [Column("company_sid")]
        public string? CompanySid { get; set; } // 公司

        [MaxLength(32)]
        [Column("business_unit_sid")]
        public string? BusinessUnitSid { get; set; } // 營運單位

        [MaxLength(32)]
        [Column("department_sid")]
        public string? DepartmentSid { get; set; } // 部門

        [MaxLength(32)]
        [Column("cost_center_sid")]
        public string? CostCenterSid { get; set; } // 成本中心

        [MaxLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; } // 專案

        [MaxLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; } // 工地

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; } // WBS

        [MaxLength(32)]
        [Column("contract_sid")]
        public string? ContractSid { get; set; } // 合約

        [MaxLength(32)]
        [Column("item_sid")]
        public string? ItemSid { get; set; } // Item

        [MaxLength(32)]
        [Column("tax_sid")]
        public string? TaxSid { get; set; } // 稅別

        [MaxLength(32)]
        [Column("bank_account_sid")]
        public string? BankAccountSid { get; set; } // 銀行帳戶

        [MaxLength(50)]
        [Column("reference_type")]
        public string? ReferenceType { get; set; } // 來源明細類型

        [MaxLength(32)]
        [Column("reference_sid")]
        public string? ReferenceSid { get; set; } // 來源明細序號

        [Required]
        [MaxLength(20)]
        [Column("reconciliation_status")]
        public string ReconciliationStatus { get; set; } = "NOT_REQUIRED"; // 清帳狀態

        [Column("cleared_date", TypeName = "DATE")]
        public DateTime? ClearedDate { get; set; } // 清帳日期
    }
    #endregion
}