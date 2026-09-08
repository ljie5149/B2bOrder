// AccountingModels.cs
using System;
using System.Collections.Generic;

namespace B2bOrder.Resources.Finance
{
    #region 01. Financial Document Types and Policies (財務單據類型與政策)

    public class AccDocumentType
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string DocumentTypeCode { get; set; } = null!;
        public string DocumentTypeName { get; set; } = null!;
        public string DocumentCategory { get; set; } = null!;
        public string SourceModule { get; set; } = null!;
        public bool ApprovalRequired { get; set; }
        public bool PostingRequired { get; set; }
        public string DocumentStatus { get; set; } = null!;
        public string Avalible { get; set; } = null!;
        public string? Remark { get; set; }
    }

    public class AccPolicy
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string PolicyCode { get; set; } = null!;
        public string PolicyName { get; set; } = null!;
        public string? CompanySid { get; set; }
        public string? BusinessUnitSid { get; set; }
        public string DefaultCurrencySid { get; set; } = null!;
        public string ExchangeRateType { get; set; } = null!;
        public decimal ToleranceAmount { get; set; }
        public decimal WriteoffLimitAmount { get; set; }
        public bool AutoPostEnabled { get; set; }
        public bool AutoMatchEnabled { get; set; }
        public string PolicyStatus { get; set; } = null!;
        public string Avalible { get; set; } = null!;
        public string? Remark { get; set; }
    }

    #endregion

    #region 02. Accounts Receivable (應收帳款)

    public class AccReceivable
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ReceivableNo { get; set; } = null!;
        public string DocumentTypeSid { get; set; } = null!;
        public string CompanySid { get; set; } = null!;
        public string? BusinessUnitSid { get; set; }
        public string CustomerPartySid { get; set; } = null!;
        public string? SalesOrderSid { get; set; }
        public string? InvoiceSid { get; set; }
        public string? ContractSid { get; set; }
        public string? ProjectSid { get; set; }
        public string? SiteSid { get; set; }
        public string? WbsSid { get; set; }
        public string? CostCenterSid { get; set; }
        public DateOnly TransactionDate { get; set; }
        public DateOnly? PostingDate { get; set; }
        public DateOnly DueDate { get; set; }
        public string CurrencySid { get; set; } = null!;
        public decimal ExchangeRate { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal WriteoffAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string ReceivableStatus { get; set; } = null!;
        public string? AccountingPeriodSid { get; set; }
        public string? JournalRequestSid { get; set; }
        public string? WorkflowInstanceSid { get; set; }
        public string? CorrelationId { get; set; }
        public ulong VersionNo { get; set; }
        public string Avalible { get; set; } = null!;
        public string? Remark { get; set; }

        public virtual ICollection<AccReceivableItem> ReceivableItems { get; set; } = new List<AccReceivableItem>();
    }

    public class AccReceivableItem
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong ReceivableNid { get; set; }
        public int LineNo { get; set; }
        public string ReferenceType { get; set; } = null!;
        public string ReferenceSid { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Quantity { get; set; }
        public string? UnitSid { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public string? ProjectSid { get; set; }
        public string? WbsSid { get; set; }
        public string? RevenueAccountSid { get; set; }

        public virtual AccReceivable Receivable { get; set; } = null!;
    }

    #endregion

    #region 03. Accounts Payable (應付帳款)

    public class AccPayable
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string PayableNo { get; set; } = null!;
        public string DocumentTypeSid { get; set; } = null!;
        public string CompanySid { get; set; } = null!;
        public string? BusinessUnitSid { get; set; }
        public string SupplierPartySid { get; set; } = null!;
        public string? PurchaseOrderSid { get; set; }
        public string? ReceiptSid { get; set; }
        public string? InvoiceSid { get; set; }
        public string? ContractSid { get; set; }
        public string? ProjectSid { get; set; }
        public string? SiteSid { get; set; }
        public string? WbsSid { get; set; }
        public string? CostCenterSid { get; set; }
        public DateOnly TransactionDate { get; set; }
        public DateOnly? PostingDate { get; set; }
        public DateOnly DueDate { get; set; }
        public string CurrencySid { get; set; } = null!;
        public decimal ExchangeRate { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal WriteoffAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string PayableStatus { get; set; } = null!;
        public bool PaymentHoldMark { get; set; }
        public string? HoldReason { get; set; }
        public string? AccountingPeriodSid { get; set; }
        public string? JournalRequestSid { get; set; }
        public string? WorkflowInstanceSid { get; set; }
        public string? CorrelationId { get; set; }
        public ulong VersionNo { get; set; }
        public string Avalible { get; set; } = null!;
        public string? Remark { get; set; }

        public virtual ICollection<AccPayableItem> PayableItems { get; set; } = new List<AccPayableItem>();
    }

    public class AccPayableItem
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong PayableNid { get; set; }
        public int LineNo { get; set; }
        public string ReferenceType { get; set; } = null!;
        public string ReferenceSid { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Quantity { get; set; }
        public string? UnitSid { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public string? ProjectSid { get; set; }
        public string? WbsSid { get; set; }
        public string? ExpenseAccountSid { get; set; }

        public virtual AccPayable Payable { get; set; } = null!;
    }

    #endregion

    #region 04. Receipts (收款)

    public class AccReceipt
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ReceiptNo { get; set; } = null!;
        public string CompanySid { get; set; } = null!;
        public string? CustomerPartySid { get; set; }
        public string? PayerPartySid { get; set; }
        public string? PaymentRequestSid { get; set; }
        public string? PaymentTransactionSid { get; set; }
        public string? BankAccountSid { get; set; }
        public string ReceiptMethod { get; set; } = null!;
        public DateTime ReceiptDate { get; set; }
        public DateOnly? ValueDate { get; set; }
        public string CurrencySid { get; set; } = null!;
        public decimal ExchangeRate { get; set; }
        public decimal ReceiptAmount { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal UnappliedAmount { get; set; }
        public string ReceiptStatus { get; set; } = null!;
        public string? JournalRequestSid { get; set; }
        public string? CorrelationId { get; set; }
        public string Avalible { get; set; } = null!;
        public string? Remark { get; set; }
    }

    public class AccReceiptApplication
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string ReceiptSid { get; set; } = null!;
        public string ReceivableSid { get; set; } = null!;
        public decimal AppliedAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal WriteoffAmount { get; set; }
        public decimal ExchangeGainLoss { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string ApplicationStatus { get; set; } = null!;
        public string? ReversalSid { get; set; }
    }

    #endregion

    #region 05. Payments (付款)

    public class AccPayment
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string PaymentNo { get; set; } = null!;
        public string CompanySid { get; set; } = null!;
        public string? SupplierPartySid { get; set; }
        public string? PayeePartySid { get; set; }
        public string? BankAccountSid { get; set; }
        public string? PayeeBankAccountSid { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public DateTime PaymentDate { get; set; }
        public DateOnly? ValueDate { get; set; }
        public string CurrencySid { get; set; } = null!;
        public decimal ExchangeRate { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal UnappliedAmount { get; set; }
        public string PaymentStatus { get; set; } = null!;
        public string? WorkflowInstanceSid { get; set; }
        public string? ApprovedUserSid { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? JournalRequestSid { get; set; }
        public string? CorrelationId { get; set; }
        public string Avalible { get; set; } = null!;
        public string? Remark { get; set; }
    }

    public class AccPaymentApplication
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string PaymentSid { get; set; } = null!;
        public string PayableSid { get; set; } = null!;
        public decimal AppliedAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal WriteoffAmount { get; set; }
        public decimal ExchangeGainLoss { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string ApplicationStatus { get; set; } = null!;
        public string? ReversalSid { get; set; }
    }

    #endregion

    #region 06. Advances and Prepayments (預收與預付)

    public class AccAdvanceReceipt
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string AdvanceNo { get; set; } = null!;
        public string CompanySid { get; set; } = null!;
        public string CustomerPartySid { get; set; } = null!;
        public string? ContractSid { get; set; }
        public string? ProjectSid { get; set; }
        public string SourceType { get; set; } = null!;
        public string? SourceSid { get; set; }
        public string? ReceiptSid { get; set; }
        public string CurrencySid { get; set; } = null!;
        public decimal AdvanceAmount { get; set; }
        public decimal AppliedAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string AdvanceStatus { get; set; } = null!;
        public string? JournalRequestSid { get; set; }
    }

    public class AccPrepayment
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string PrepaymentNo { get; set; } = null!;
        public string CompanySid { get; set; } = null!;
        public string SupplierPartySid { get; set; } = null!;
        public string? ContractSid { get; set; }
        public string? ProjectSid { get; set; }
        public string SourceType { get; set; } = null!;
        public string? SourceSid { get; set; }
        public string? PaymentSid { get; set; }
        public string CurrencySid { get; set; } = null!;
        public decimal PrepaymentAmount { get; set; }
        public decimal AppliedAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string PrepaymentStatus { get; set; } = null!;
        public string? JournalRequestSid { get; set; }
    }

    #endregion

    #region 07. Expenses and Reimbursements (費用與報支)

    public class AccExpense
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ExpenseNo { get; set; } = null!;
        public string CompanySid { get; set; } = null!;
        public string? DepartmentSid { get; set; }
        public string? EmployeePartySid { get; set; }
        public string? SupplierPartySid { get; set; }
        public string? ProjectSid { get; set; }
        public string? SiteSid { get; set; }
        public string? WbsSid { get; set; }
        public string? CostCenterSid { get; set; }
        public DateOnly ExpenseDate { get; set; }
        public string CurrencySid { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public decimal ReimbursableAmount { get; set; }
        public string? PayableSid { get; set; }
        public string ExpenseStatus { get; set; } = null!;
        public string? WorkflowInstanceSid { get; set; }
        public string? ApprovedUserSid { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Avalible { get; set; } = null!;
        public string? Remark { get; set; }

        public virtual ICollection<AccExpenseItem> ExpenseItems { get; set; } = new List<AccExpenseItem>();
    }

    public class AccExpenseItem
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong ExpenseNid { get; set; }
        public int LineNo { get; set; }
        public string ExpenseCategorySid { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateOnly ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? TaxSid { get; set; }
        public string? InvoiceSid { get; set; }
        public string? ProjectSid { get; set; }
        public string? WbsSid { get; set; }
        public string? CostCenterSid { get; set; }
        public string? ExpenseAccountSid { get; set; }
        public string? FileSid { get; set; }

        public virtual AccExpense Expense { get; set; } = null!;
    }

    #endregion

    #region 08. Credits, Debits, and Notes (折讓、貸項與借項)

    public class AccCreditDebitNote
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string NoteNo { get; set; } = null!;
        public string NoteType { get; set; } = null!;
        public string CompanySid { get; set; } = null!;
        public string PartySid { get; set; } = null!;
        public string? ReceivableSid { get; set; }
        public string? PayableSid { get; set; }
        public string? ReturnCaseSid { get; set; }
        public string? SupplierClaimSid { get; set; }
        public string? InvoiceAllowanceSid { get; set; }
        public DateOnly NoteDate { get; set; }
        public string CurrencySid { get; set; } = null!;
        public decimal NoteAmount { get; set; }
        public string ReasonCode { get; set; } = null!;
        public string? Reason { get; set; }
        public string NoteStatus { get; set; } = null!;
        public string? WorkflowInstanceSid { get; set; }
        public string? JournalRequestSid { get; set; }
    }

    #endregion

    #region 09. Reconciliations and Applications (對帳、沖帳與調整)

    public class AccReconciliation
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ReconciliationNo { get; set; } = null!;
        public string ReconciliationType { get; set; } = null!;
        public string CompanySid { get; set; } = null!;
        public string? PartySid { get; set; }
        public string? BankAccountSid { get; set; }
        public DateOnly PeriodStartDate { get; set; }
        public DateOnly PeriodEndDate { get; set; }
        public string CurrencySid { get; set; } = null!;
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal MatchedAmount { get; set; }
        public decimal DifferenceAmount { get; set; }
        public string? StatementFileSid { get; set; }
        public string ReconciliationStatus { get; set; } = null!;
        public string? ResponsibleUserSid { get; set; }
        public DateTime? CompletedDate { get; set; }

        public virtual ICollection<AccReconciliationItem> ReconciliationItems { get; set; } = new List<AccReconciliationItem>();
    }

    public class AccReconciliationItem
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong ReconciliationNid { get; set; }
        public int LineNo { get; set; }
        public string? ExternalReferenceNo { get; set; }
        public string? InternalType { get; set; }
        public string? InternalSid { get; set; }
        public DateOnly? TransactionDate { get; set; }
        public decimal ExternalAmount { get; set; }
        public decimal InternalAmount { get; set; }
        public decimal DifferenceAmount { get; set; }
        public string MatchStatus { get; set; } = null!;
        public string? ResolutionNote { get; set; }

        public virtual AccReconciliation Reconciliation { get; set; } = null!;
    }

    #endregion

    #region 10. Financial Journal Interfaces (財務傳票介面)

    public class AccJournalRequest
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string JournalRequestNo { get; set; } = null!;
        public string CompanySid { get; set; } = null!;
        public string SourceModule { get; set; } = null!;
        public string SourceType { get; set; } = null!;
        public string SourceSid { get; set; } = null!;
        public DateOnly PostingDate { get; set; }
        public string? AccountingPeriodSid { get; set; }
        public string CurrencySid { get; set; } = null!;
        public decimal TotalDebitAmount { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public string RequestStatus { get; set; } = null!;
        public string? GeneralLedgerJournalSid { get; set; }
        public string IdempotencyKey { get; set; } = null!;
        public string? CorrelationId { get; set; }
        public DateTime? PostedDate { get; set; }
        public string? ErrorMessage { get; set; }

        public virtual ICollection<AccJournalRequestLine> JournalRequestLines { get; set; } = new List<AccJournalRequestLine>();
    }

    public class AccJournalRequestLine
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong JournalRequestNid { get; set; }
        public int LineNo { get; set; }
        public string AccountSid { get; set; } = null!;
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string? Description { get; set; }
        public string? PartySid { get; set; }
        public string? ProjectSid { get; set; }
        public string? SiteSid { get; set; }
        public string? WbsSid { get; set; }
        public string? CostCenterSid { get; set; }
        public string? DepartmentSid { get; set; }
        public string? TaxSid { get; set; }
        public string? ReferenceType { get; set; }
        public string? ReferenceSid { get; set; }

        public virtual AccJournalRequest JournalRequest { get; set; } = null!;
    }

    #endregion

    #region 11. Aging and Financial Snapshots (帳齡與財務快照)

    public class AccAgingSnapshot
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateOnly SnapshotDate { get; set; }
        public string LedgerType { get; set; } = null!;
        public string CompanySid { get; set; } = null!;
        public string PartySid { get; set; } = null!;
        public string CurrencySid { get; set; } = null!;
        public decimal CurrentAmount { get; set; }
        public decimal Days130Amount { get; set; }
        public decimal Days3160Amount { get; set; }
        public decimal Days6190Amount { get; set; }
        public decimal Days91180Amount { get; set; }
        public decimal DaysOver180Amount { get; set; }
        public decimal TotalOutstanding { get; set; }
    }

    #endregion

    #region 12. Status Histories and Events (狀態歷程與事件)

    public class AccStatusHistory
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string EntityType { get; set; } = null!;
        public string EntitySid { get; set; } = null!;
        public string? OldStatus { get; set; }
        public string NewStatus { get; set; } = null!;
        public string? EventCode { get; set; }
        public string? OperatorUserSid { get; set; }
        public string? Reason { get; set; }
        public string? CorrelationId { get; set; }
    }

    public class AccEvent
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string EntityType { get; set; } = null!;
        public string EntitySid { get; set; } = null!;
        public string EventCode { get; set; } = null!;
        public int EventVersion { get; set; }
        public string? EventData { get; set; } // Mapping JSON column as string
        public string? SourceEventId { get; set; }
        public string? CorrelationId { get; set; }
        public string? CausationId { get; set; }
        public string? OutboxEventSid { get; set; }
        public string ProcessStatus { get; set; } = null!;
        public DateTime? ProcessedDate { get; set; }
        public string? ErrorMessage { get; set; }
    }

    #endregion
}