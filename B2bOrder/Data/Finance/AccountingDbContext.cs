// AccountingDbContext.cs
using B2bOrder.Resources.ShareCore.Models;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Resources.Finance
{
    public class AccountingDbContext : DbContext
    {
        public AccountingDbContext(DbContextOptions<AccountingDbContext> options) : base(options)
        {
        }

        #region 01. Financial Document Types and Policies (財務單據類型與政策)
        public DbSet<AccDocumentType> AccDocumentTypes => Set<AccDocumentType>();
        public DbSet<AccPolicy> AccPolicies => Set<AccPolicy>();
        #endregion

        #region 02. Accounts Receivable (應收帳款)
        public DbSet<AccReceivable> AccReceivables => Set<AccReceivable>();
        public DbSet<AccReceivableItem> AccReceivableItems => Set<AccReceivableItem>();
        #endregion

        #region 03. Accounts Payable (應付帳款)
        public DbSet<AccPayable> AccPayables => Set<AccPayable>();
        public DbSet<AccPayableItem> AccPayableItems => Set<AccPayableItem>();
        #endregion

        #region 04. Receipts (收款)
        public DbSet<AccReceipt> AccReceipts => Set<AccReceipt>();
        public DbSet<AccReceiptApplication> AccReceiptApplications => Set<AccReceiptApplication>();
        #endregion

        #region 05. Payments (付款)
        public DbSet<AccPayment> AccPayments => Set<AccPayment>();
        public DbSet<AccPaymentApplication> AccPaymentApplications => Set<AccPaymentApplication>();
        #endregion

        #region 06. Advances and Prepayments (預收與預付)
        public DbSet<AccAdvanceReceipt> AccAdvanceReceipts => Set<AccAdvanceReceipt>();
        public DbSet<AccPrepayment> AccPrepayments => Set<AccPrepayment>();
        #endregion

        #region 07. Expenses and Reimbursements (費用與報支)
        public DbSet<AccExpense> AccExpenses => Set<AccExpense>();
        public DbSet<AccExpenseItem> AccExpenseItems => Set<AccExpenseItem>();
        #endregion

        #region 08. Credits, Debits, and Notes (折讓、貸項與借項)
        public DbSet<AccCreditDebitNote> AccCreditDebitNotes => Set<AccCreditDebitNote>();
        #endregion

        #region 09. Reconciliations and Applications (對帳、沖帳與調整)
        public DbSet<AccReconciliation> AccReconciliations => Set<AccReconciliation>();
        public DbSet<AccReconciliationItem> AccReconciliationItems => Set<AccReconciliationItem>();
        #endregion

        #region 10. Financial Journal Interfaces (財務傳票介面)
        public DbSet<AccJournalRequest> AccJournalRequests => Set<AccJournalRequest>();
        public DbSet<AccJournalRequestLine> AccJournalRequestLines => Set<AccJournalRequestLine>();
        #endregion

        #region 11. Aging and Financial Snapshots (帳齡與財務快照)
        public DbSet<AccAgingSnapshot> AccAgingSnapshots => Set<AccAgingSnapshot>();
        #endregion

        #region 12. Status Histories and Events (狀態歷程與事件)
        public DbSet<AccStatusHistory> AccStatusHistories => Set<AccStatusHistory>();
        public DbSet<AccEvent> AccEvents => Set<AccEvent>();
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 01. acc_document_type
            modelBuilder.Entity<AccDocumentType>(entity =>
            {
                entity.ToTable("acc_document_type");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.DocumentTypeCode).IsUnique();
            });

            // 01. acc_policy
            modelBuilder.Entity<AccPolicy>(entity =>
            {
                entity.ToTable("acc_policy");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.PolicyCode).IsUnique();
            });

            // 02. acc_receivable
            modelBuilder.Entity<AccReceivable>(entity =>
            {
                entity.ToTable("acc_receivable");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ReceivableNo).IsUnique();
            });

            // 02. acc_receivable_item
            modelBuilder.Entity<AccReceivableItem>(entity =>
            {
                entity.ToTable("acc_receivable_item");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.ReceivableNid, e.LineNo }).IsUnique();

                entity.HasOne(d => d.Receivable)
                    .WithMany(p => p.ReceivableItems)
                    .HasForeignKey(d => d.ReceivableNid)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 03. acc_payable
            modelBuilder.Entity<AccPayable>(entity =>
            {
                entity.ToTable("acc_payable");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.PayableNo).IsUnique();
            });

            // 03. acc_payable_item
            modelBuilder.Entity<AccPayableItem>(entity =>
            {
                entity.ToTable("acc_payable_item");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.PayableNid, e.LineNo }).IsUnique();

                entity.HasOne(d => d.Payable)
                    .WithMany(p => p.PayableItems)
                    .HasForeignKey(d => d.PayableNid)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 04. acc_receipt
            modelBuilder.Entity<AccReceipt>(entity =>
            {
                entity.ToTable("acc_receipt");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ReceiptNo).IsUnique();
            });

            // 04. acc_receipt_application
            modelBuilder.Entity<AccReceiptApplication>(entity =>
            {
                entity.ToTable("acc_receipt_application");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.ReceiptSid, e.ReceivableSid }).IsUnique();
            });

            // 05. acc_payment
            modelBuilder.Entity<AccPayment>(entity =>
            {
                entity.ToTable("acc_payment");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.PaymentNo).IsUnique();
            });

            // 05. acc_payment_application
            modelBuilder.Entity<AccPaymentApplication>(entity =>
            {
                entity.ToTable("acc_payment_application");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.PaymentSid, e.PayableSid }).IsUnique();
            });

            // 06. acc_advance_receipt
            modelBuilder.Entity<AccAdvanceReceipt>(entity =>
            {
                entity.ToTable("acc_advance_receipt");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.AdvanceNo).IsUnique();
            });

            // 06. acc_prepayment
            modelBuilder.Entity<AccPrepayment>(entity =>
            {
                entity.ToTable("acc_prepayment");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.PrepaymentNo).IsUnique();
            });

            // 07. acc_expense
            modelBuilder.Entity<AccExpense>(entity =>
            {
                entity.ToTable("acc_expense");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ExpenseNo).IsUnique();
            });

            // 07. acc_expense_item
            modelBuilder.Entity<AccExpenseItem>(entity =>
            {
                entity.ToTable("acc_expense_item");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.ExpenseNid, e.LineNo }).IsUnique();

                entity.HasOne(d => d.Expense)
                    .WithMany(p => p.ExpenseItems)
                    .HasForeignKey(d => d.ExpenseNid)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 08. acc_credit_debit_note
            modelBuilder.Entity<AccCreditDebitNote>(entity =>
            {
                entity.ToTable("acc_credit_debit_note");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.NoteNo).IsUnique();
            });

            // 09. acc_reconciliation
            modelBuilder.Entity<AccReconciliation>(entity =>
            {
                entity.ToTable("acc_reconciliation");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ReconciliationNo).IsUnique();
            });

            // 09. acc_reconciliation_item
            modelBuilder.Entity<AccReconciliationItem>(entity =>
            {
                entity.ToTable("acc_reconciliation_item");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.ReconciliationNid, e.LineNo }).IsUnique();

                entity.HasOne(d => d.Reconciliation)
                    .WithMany(p => p.ReconciliationItems)
                    .HasForeignKey(d => d.ReconciliationNid)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 10. acc_journal_request
            modelBuilder.Entity<AccJournalRequest>(entity =>
            {
                entity.ToTable("acc_journal_request");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.JournalRequestNo).IsUnique();
                entity.HasIndex(e => e.IdempotencyKey).IsUnique();
            });

            // 10. acc_journal_request_line
            modelBuilder.Entity<AccJournalRequestLine>(entity =>
            {
                entity.ToTable("acc_journal_request_line");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.JournalRequestNid, e.LineNo }).IsUnique();

                entity.HasOne(d => d.JournalRequest)
                    .WithMany(p => p.JournalRequestLines)
                    .HasForeignKey(d => d.JournalRequestNid)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 11. acc_aging_snapshot
            modelBuilder.Entity<AccAgingSnapshot>(entity =>
            {
                entity.ToTable("acc_aging_snapshot");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.SnapshotDate, e.LedgerType, e.CompanySid, e.PartySid, e.CurrencySid }).IsUnique();
            });

            // 12. acc_status_history
            modelBuilder.Entity<AccStatusHistory>(entity =>
            {
                entity.ToTable("acc_status_history");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
            });

            // 12. acc_event
            modelBuilder.Entity<AccEvent>(entity =>
            {
                entity.ToTable("acc_event");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SourceEventId).IsUnique();
            });
        }
    }
}