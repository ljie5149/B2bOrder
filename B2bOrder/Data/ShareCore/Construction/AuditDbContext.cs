using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.ShareCore.Construction.Models;

namespace B2bOrder.Resources.ShareCore.Construction
{
    public class BudgetCostDbContext : DbContext
    {
        public BudgetCostDbContext(DbContextOptions<BudgetCostDbContext> options) : base(options)
        {
        }

        #region DbSet Configurations - 資料集設定

        public DbSet<AdtDataChangeLog> AdtDataChangeLogs { get; set; }
        public DbSet<AdtOperationLog> AdtOperationLogs { get; set; }
        public DbSet<AdtSecurityEvent> AdtSecurityEvents { get; set; }
        public DbSet<AdtSensitiveAccessLog> AdtSensitiveAccessLogs { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Index Configurations - 唯一索引設定

            modelBuilder.Entity<AdtDataChangeLog>()
                .HasIndex(e => e.Sid).IsUnique();

            modelBuilder.Entity<AdtOperationLog>()
                .HasIndex(e => e.Sid).IsUnique();

            modelBuilder.Entity<AdtSecurityEvent>()
                .HasIndex(e => e.Sid).IsUnique();

            modelBuilder.Entity<AdtSensitiveAccessLog>()
                .HasIndex(e => e.Sid).IsUnique();

            #endregion

            #region Composite Index Configurations - 複合索引與一般索引設定

            // 01. adt_data_change_log
            modelBuilder.Entity<AdtDataChangeLog>()
                .HasIndex(e => new { e.ServiceName, e.TableName });
            modelBuilder.Entity<AdtDataChangeLog>()
                .HasIndex(e => e.EntitySid);
            modelBuilder.Entity<AdtDataChangeLog>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<AdtDataChangeLog>()
                .HasIndex(e => e.OperatorUserSid);
            modelBuilder.Entity<AdtDataChangeLog>()
                .HasIndex(e => e.TraceId);
            modelBuilder.Entity<AdtDataChangeLog>()
                .HasIndex(e => e.CreateDate);

            // 02. adt_operation_log
            modelBuilder.Entity<AdtOperationLog>()
                .HasIndex(e => new { e.ServiceName, e.ModuleName });
            modelBuilder.Entity<AdtOperationLog>()
                .HasIndex(e => e.OperatorUserSid);
            modelBuilder.Entity<AdtOperationLog>()
                .HasIndex(e => e.ResponseCode);
            modelBuilder.Entity<AdtOperationLog>()
                .HasIndex(e => e.TraceId);
            modelBuilder.Entity<AdtOperationLog>()
                .HasIndex(e => e.CreateDate);

            // 03. adt_security_event
            modelBuilder.Entity<AdtSecurityEvent>()
                .HasIndex(e => e.UserSid);
            modelBuilder.Entity<AdtSecurityEvent>()
                .HasIndex(e => e.AccountIdentifier);
            modelBuilder.Entity<AdtSecurityEvent>()
                .HasIndex(e => e.EventType);
            modelBuilder.Entity<AdtSecurityEvent>()
                .HasIndex(e => e.SeverityLevel);
            modelBuilder.Entity<AdtSecurityEvent>()
                .HasIndex(e => e.ClientIp);
            modelBuilder.Entity<AdtSecurityEvent>()
                .HasIndex(e => e.CreateDate);

            // 04. adt_sensitive_access_log
            modelBuilder.Entity<AdtSensitiveAccessLog>()
                .HasIndex(e => e.OperatorUserSid);
            modelBuilder.Entity<AdtSensitiveAccessLog>()
                .HasIndex(e => e.DataType);
            modelBuilder.Entity<AdtSensitiveAccessLog>()
                .HasIndex(e => e.AccessAction);
            modelBuilder.Entity<AdtSensitiveAccessLog>()
                .HasIndex(e => e.CreateDate);

            #endregion

            #region Default Value Configurations - 預設值設定

            // 02. adt_operation_log
            modelBuilder.Entity<AdtOperationLog>()
                .Property(e => e.ExecutionTimeMs).HasDefaultValue(0L);

            // 03. adt_security_event
            modelBuilder.Entity<AdtSecurityEvent>()
                .Property(e => e.SeverityLevel).HasDefaultValue("INFO");

            // 04. adt_sensitive_access_log
            modelBuilder.Entity<AdtSensitiveAccessLog>()
                .Property(e => e.AccessAction).HasDefaultValue("VIEW");
            modelBuilder.Entity<AdtSensitiveAccessLog>()
                .Property(e => e.RecordCount).HasDefaultValue(1);

            #endregion
        }
    }
}