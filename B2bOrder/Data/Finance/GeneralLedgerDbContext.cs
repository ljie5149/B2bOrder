using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Finance.Models;

namespace B2bOrder.Resources.Finance.Data
{
    #region 總帳資料庫上下文 (General Ledger DB Context)
    public class GeneralLedgerDbContext : DbContext
    {
        public GeneralLedgerDbContext(DbContextOptions<GeneralLedgerDbContext> options)
            : base(options)
        {
        }

        #endregion

        #region 資料表對應集 (DbSets)
        /// <summary>
        /// 會計年度曆 (Fiscal Calendars)
        /// </summary>
        public DbSet<FiscalCalendarModel> FiscalCalendars { get; set; } = null!;

        /// <summary>
        /// 會計期間 (Accounting Periods)
        /// </summary>
        public DbSet<AccountingPeriodModel> AccountingPeriods { get; set; } = null!;

        /// <summary>
        /// 會計科目表 (Charts of Accounts)
        /// </summary>
        public DbSet<ChartOfAccountModel> ChartOfAccounts { get; set; } = null!;

        /// <summary>
        /// 會計科目 (Accounts)
        /// </summary>
        public DbSet<AccountModel> Accounts { get; set; } = null!;

        /// <summary>
        /// 正式會計傳票 (Journals)
        /// </summary>
        public DbSet<JournalModel> Journals { get; set; } = null!;

        /// <summary>
        /// 正式會計分錄 (Journal Lines)
        /// </summary>
        public DbSet<JournalLineModel> JournalLines { get; set; } = null!;
        #endregion

        #region 模型建立設定 (Model Creating Configuration)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 會計年度曆索引
            modelBuilder.Entity<FiscalCalendarModel>()
                .HasIndex(e => e.CalendarCode)
                .IsUnique()
                .HasDatabaseName("uk_gfc_code");

            modelBuilder.Entity<FiscalCalendarModel>()
                .HasIndex(e => e.CalendarStatus)
                .HasDatabaseName("idx_gfc_status");

            // 會計期間索引
            modelBuilder.Entity<AccountingPeriodModel>()
                .HasIndex(e => new { e.FiscalCalendarSid, e.FiscalYear, e.PeriodNo })
                .IsUnique()
                .HasDatabaseName("uk_gap_period");

            // 會計科目表索引
            modelBuilder.Entity<ChartOfAccountModel>()
                .HasIndex(e => e.ChartCode)
                .IsUnique()
                .HasDatabaseName("uk_gcoa_code");

            // 會計科目索引
            modelBuilder.Entity<AccountModel>()
                .HasIndex(e => new { e.ChartOfAccountSid, e.AccountCode })
                .IsUnique()
                .HasDatabaseName("uk_ga_code");

            // 傳票索引
            modelBuilder.Entity<JournalModel>()
                .HasIndex(e => e.JournalNo)
                .IsUnique()
                .HasDatabaseName("uk_gj_no");

            modelBuilder.Entity<JournalModel>()
                .HasIndex(e => e.IdempotencyKey)
                .IsUnique()
                .HasDatabaseName("uk_gj_idempotency");

            // 分錄與傳票之外鍵關聯
            modelBuilder.Entity<JournalLineModel>()
                .HasOne<JournalModel>()
                .WithMany()
                .HasForeignKey(l => l.JournalNid)
                .HasConstraintName("fk_gjl_journal");

            modelBuilder.Entity<JournalLineModel>()
                .HasIndex(e => new { e.JournalNid, e.LineNo })
                .IsUnique()
                .HasDatabaseName("uk_gjl_line");
        }
        #endregion
    }
}