using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.ShareCore.Models;

namespace B2bOrder.Resources.ShareCore
{
    public class SchedulerDbContext : DbContext
    {
        public SchedulerDbContext(DbContextOptions<SchedulerDbContext> options) : base(options) { }

        #region DbSets
        public DbSet<SchJobDefinition> SchJobDefinitions { get; set; } = null!;
        public DbSet<SchJobLock> SchJobLocks { get; set; } = null!;
        public DbSet<SchExecutionLog> SchExecutionLogs { get; set; } = null!;
        public DbSet<SchDelayedTask> SchDelayedTasks { get; set; } = null!;
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region 01. 分散式排程任務主檔 (Cron Job Master Definition)
            modelBuilder.Entity<SchJobDefinition>(entity =>
            {
                entity.ToTable("sch_job_definition");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.CompanySid, e.JobCode }).HasDatabaseName("uk_sjd_company_job").IsUnique();

                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_sjd_company_sid");
                entity.HasIndex(e => e.JobGroup).HasDatabaseName("idx_sjd_job_group");
                entity.HasIndex(e => e.JobStatus).HasDatabaseName("idx_sjd_status");
                entity.HasIndex(e => e.NextFireTime).HasDatabaseName("idx_sjd_next_fire");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_sjd_avalible");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.JobCode).HasColumnName("job_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.JobGroup).HasColumnName("job_group").HasMaxLength(50).HasDefaultValue("DEFAULT");
                entity.Property(e => e.JobName).HasColumnName("job_name").HasMaxLength(150).IsRequired();
                entity.Property(e => e.CronExpression).HasColumnName("cron_expression").HasMaxLength(100).IsRequired();
                entity.Property(e => e.TargetService).HasColumnName("target_service").HasMaxLength(50).IsRequired();
                entity.Property(e => e.TargetEndpoint).HasColumnName("target_endpoint").HasMaxLength(255).IsRequired();
                entity.Property(e => e.HttpMethod).HasColumnName("http_method").HasMaxLength(10).HasDefaultValue("POST");
                entity.Property(e => e.PayloadJson).HasColumnName("payload_json").HasColumnType("json");
                entity.Property(e => e.ConcurrentAllowed).HasColumnName("concurrent_allowed").HasMaxLength(2).HasDefaultValue("N");
                entity.Property(e => e.MaxRetryCount).HasColumnName("max_retry_count").HasDefaultValue(3);
                entity.Property(e => e.TimeoutSeconds).HasColumnName("timeout_seconds").HasDefaultValue(3600);
                entity.Property(e => e.JobStatus).HasColumnName("job_status").HasMaxLength(20).HasDefaultValue("PAUSED");
                entity.Property(e => e.LastExecutedAt).HasColumnName("last_executed_at");
                entity.Property(e => e.NextFireTime).HasColumnName("next_fire_time");
                entity.Property(e => e.VersionNo).HasColumnName("version_no").HasDefaultValue(0UL);
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });
            #endregion

            #region 02. 分散式任務執行鎖 (Distributed Job Lock)
            modelBuilder.Entity<SchJobLock>(entity =>
            {
                entity.ToTable("sch_job_lock");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.JobCode).IsUnique();

                entity.HasIndex(e => e.LockExpiredAt).HasDatabaseName("idx_sjl_expired_at");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.JobCode).HasColumnName("job_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.LockedByNode).HasColumnName("locked_by_node").HasMaxLength(100).IsRequired();
                entity.Property(e => e.LockedAt).HasColumnName("locked_at").IsRequired();
                entity.Property(e => e.LockExpiredAt).HasColumnName("lock_expired_at").IsRequired();
                entity.Property(e => e.VersionNo).HasColumnName("version_no").HasDefaultValue(0UL);
            });
            #endregion

            #region 03. 排程執行歷程日誌 (Job Execution History)
            modelBuilder.Entity<SchExecutionLog>(entity =>
            {
                entity.ToTable("sch_execution_log");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_sel_company_sid");
                entity.HasIndex(e => e.JobCode).HasDatabaseName("idx_sel_job_code");
                entity.HasIndex(e => e.ExecutionStatus).HasDatabaseName("idx_sel_status");
                entity.HasIndex(e => e.StartTime).HasDatabaseName("idx_sel_start_time");
                entity.HasIndex(e => e.CreateDate).HasDatabaseName("idx_sel_create_date");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.JobCode).HasColumnName("job_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ExecutedByNode).HasColumnName("executed_by_node").HasMaxLength(100).IsRequired();
                entity.Property(e => e.StartTime).HasColumnName("start_time").IsRequired();
                entity.Property(e => e.EndTime).HasColumnName("end_time");
                entity.Property(e => e.ExecutionTimeMs).HasColumnName("execution_time_ms");
                entity.Property(e => e.ExecutionStatus).HasColumnName("execution_status").HasMaxLength(20).HasDefaultValue("RUNNING");
                entity.Property(e => e.RetryCount).HasColumnName("retry_count").HasDefaultValue(0);
                entity.Property(e => e.AffectedRows).HasColumnName("affected_rows").HasDefaultValue(0);
                entity.Property(e => e.ResultMessage).HasColumnName("result_message");
                entity.Property(e => e.ErrorStackTrace).HasColumnName("error_stack_trace");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });
            #endregion

            #region 04. 延遲與一次性異步任務 (Delayed & One-off Tasks)
            modelBuilder.Entity<SchDelayedTask>(entity =>
            {
                entity.ToTable("sch_delayed_task");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.CompanySid, e.TaskType, e.BusinessKey }).HasDatabaseName("uk_sdt_task_business").IsUnique();

                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_sdt_company_sid");
                entity.HasIndex(e => new { e.ScheduledExecuteTime, e.TaskStatus }).HasDatabaseName("idx_sdt_execute_time");
                entity.HasIndex(e => e.TaskStatus).HasDatabaseName("idx_sdt_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_sdt_avalible");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.TaskType).HasColumnName("task_type").HasMaxLength(50).IsRequired();
                entity.Property(e => e.BusinessKey).HasColumnName("business_key").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ScheduledExecuteTime).HasColumnName("scheduled_execute_time").IsRequired();
                entity.Property(e => e.TargetService).HasColumnName("target_service").HasMaxLength(50).IsRequired();
                entity.Property(e => e.TargetEndpoint).HasColumnName("target_endpoint").HasMaxLength(255).IsRequired();
                entity.Property(e => e.PayloadJson).HasColumnName("payload_json").HasColumnType("json");
                entity.Property(e => e.TaskStatus).HasColumnName("task_status").HasMaxLength(20).HasDefaultValue("WAITING");
                entity.Property(e => e.RetryCount).HasColumnName("retry_count").HasDefaultValue(0);
                entity.Property(e => e.MaxRetryCount).HasColumnName("max_retry_count").HasDefaultValue(3);
                entity.Property(e => e.ExecutedAt).HasColumnName("executed_at");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
                entity.Property(e => e.VersionNo).HasColumnName("version_no").HasDefaultValue(0UL);
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });
            #endregion
        }
    }
}