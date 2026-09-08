using B2bOrder.Resources.ShareCore.Models;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Resources.ShareCore
{
    public class IntegrationDbContext : DbContext
    {
        public IntegrationDbContext(DbContextOptions<IntegrationDbContext> options) : base(options) { }

        #region DbSets
        public DbSet<IntServiceRegistry> IntServiceRegistries { get; set; } = null!;
        public DbSet<IntEventDefinition> IntEventDefinitions { get; set; } = null!;
        public DbSet<IntOutboxEvent> IntOutboxEvents { get; set; } = null!;
        public DbSet<IntOutboxDelivery> IntOutboxDeliveries { get; set; } = null!;
        public DbSet<IntInboxEvent> IntInboxEvents { get; set; } = null!;
        public DbSet<IntEventHandlerLog> IntEventHandlerLogs { get; set; } = null!;
        public DbSet<IntDeadLetter> IntDeadLetters { get; set; } = null!;
        public DbSet<IntIdempotencyKey> IntIdempotencyKeys { get; set; } = null!;
        public DbSet<IntApiCallLog> IntApiCallLogs { get; set; } = null!;
        public DbSet<IntSyncDefinition> IntSyncDefinitions { get; set; } = null!;
        public DbSet<IntSyncCheckpoint> IntSyncCheckpoints { get; set; } = null!;
        public DbSet<IntSyncJob> IntSyncJobs { get; set; } = null!;
        public DbSet<IntSyncError> IntSyncErrors { get; set; } = null!;
        public DbSet<IntSagaInstance> IntSagaInstances { get; set; } = null!;
        public DbSet<IntSagaStep> IntSagaSteps { get; set; } = null!;
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region 01. 服務與事件定義
            modelBuilder.Entity<IntServiceRegistry>(entity =>
            {
                entity.ToTable("int_service_registry");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ServiceCode).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.ServiceCode).HasColumnName("service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.ServiceName).HasColumnName("service_name").HasMaxLength(150).IsRequired();
                entity.Property(e => e.ServiceUrl).HasColumnName("service_url").HasMaxLength(500);
                entity.Property(e => e.HealthCheckUrl).HasColumnName("health_check_url").HasMaxLength(500);
                entity.Property(e => e.ServiceVersion).HasColumnName("service_version").HasMaxLength(50);
                entity.Property(e => e.TimeoutSeconds).HasColumnName("timeout_seconds").HasDefaultValue(30);
                entity.Property(e => e.RetryCount).HasColumnName("retry_count").HasDefaultValue(3);
                entity.Property(e => e.ServiceStatus).HasColumnName("service_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });

            modelBuilder.Entity<IntEventDefinition>(entity =>
            {
                entity.ToTable("int_event_definition");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.EventCode).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.EventCode).HasColumnName("event_code").HasMaxLength(150).IsRequired();
                entity.Property(e => e.EventName).HasColumnName("event_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.AggregateType).HasColumnName("aggregate_type").HasMaxLength(100).IsRequired();
                entity.Property(e => e.SourceServiceCode).HasColumnName("source_service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.SchemaVersion).HasColumnName("schema_version").HasMaxLength(20).HasDefaultValue("1.0");
                entity.Property(e => e.PayloadSchema).HasColumnName("payload_schema");
                entity.Property(e => e.RetentionDays).HasColumnName("retention_days").HasDefaultValue(90);
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });
            #endregion

            #region 02. Transactional Outbox
            modelBuilder.Entity<IntOutboxEvent>(entity =>
            {
                entity.ToTable("int_outbox_event");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.EventId).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EventId).HasColumnName("event_id").HasMaxLength(36).IsRequired();
                entity.Property(e => e.EventCode).HasColumnName("event_code").HasMaxLength(150).IsRequired();
                entity.Property(e => e.AggregateType).HasColumnName("aggregate_type").HasMaxLength(100).IsRequired();
                entity.Property(e => e.AggregateSid).HasColumnName("aggregate_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.SourceServiceCode).HasColumnName("source_service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasMaxLength(100);
                entity.Property(e => e.CausationId).HasColumnName("causation_id").HasMaxLength(100);
                entity.Property(e => e.TraceId).HasColumnName("trace_id").HasMaxLength(100);
                entity.Property(e => e.Payload).HasColumnName("payload").IsRequired();
                entity.Property(e => e.Headers).HasColumnName("headers");
                entity.Property(e => e.EventVersion).HasColumnName("event_version").HasDefaultValue(1);
                entity.Property(e => e.OccurredDate).HasColumnName("occurred_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.PublishStatus).HasColumnName("publish_status").HasMaxLength(20).HasDefaultValue("PENDING");
                entity.Property(e => e.RetryCount).HasColumnName("retry_count").HasDefaultValue(0);
                entity.Property(e => e.MaxRetryCount).HasColumnName("max_retry_count").HasDefaultValue(10);
                entity.Property(e => e.NextRetryDate).HasColumnName("next_retry_date");
                entity.Property(e => e.PublishedDate).HasColumnName("published_date");
                entity.Property(e => e.LockToken).HasColumnName("lock_token").HasMaxLength(100);
                entity.Property(e => e.LockExpiryDate).HasColumnName("lock_expiry_date");
                entity.Property(e => e.LastError).HasColumnName("last_error");
            });

            modelBuilder.Entity<IntOutboxDelivery>(entity =>
            {
                entity.ToTable("int_outbox_delivery");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.OutboxEventNid, e.TargetServiceCode }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.OutboxEventNid).HasColumnName("outbox_event_nid").IsRequired();
                entity.Property(e => e.TargetServiceCode).HasColumnName("target_service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.EndpointUrl).HasColumnName("endpoint_url").HasMaxLength(500);
                entity.Property(e => e.DeliveryStatus).HasColumnName("delivery_status").HasMaxLength(20).HasDefaultValue("PENDING");
                entity.Property(e => e.RetryCount).HasColumnName("retry_count").HasDefaultValue(0);
                entity.Property(e => e.MaxRetryCount).HasColumnName("max_retry_count").HasDefaultValue(10);
                entity.Property(e => e.NextRetryDate).HasColumnName("next_retry_date");
                entity.Property(e => e.RequestData).HasColumnName("request_data");
                entity.Property(e => e.ResponseStatusCode).HasColumnName("response_status_code");
                entity.Property(e => e.ResponseData).HasColumnName("response_data");
                entity.Property(e => e.DeliveredDate).HasColumnName("delivered_date");
                entity.Property(e => e.LastError).HasColumnName("last_error");

                entity.HasOne(d => d.OutboxEvent)
                    .WithMany(p => p.OutboxDeliveries)
                    .HasForeignKey(d => d.OutboxEventNid)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region 03. Inbox與事件消費
            modelBuilder.Entity<IntInboxEvent>(entity =>
            {
                entity.ToTable("int_inbox_event");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.TargetServiceCode, e.EventId }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EventId).HasColumnName("event_id").HasMaxLength(36).IsRequired();
                entity.Property(e => e.EventCode).HasColumnName("event_code").HasMaxLength(150).IsRequired();
                entity.Property(e => e.SourceServiceCode).HasColumnName("source_service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.TargetServiceCode).HasColumnName("target_service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.AggregateType).HasColumnName("aggregate_type").HasMaxLength(100).IsRequired();
                entity.Property(e => e.AggregateSid).HasColumnName("aggregate_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasMaxLength(100);
                entity.Property(e => e.TraceId).HasColumnName("trace_id").HasMaxLength(100);
                entity.Property(e => e.Payload).HasColumnName("payload").IsRequired();
                entity.Property(e => e.Headers).HasColumnName("headers");
                entity.Property(e => e.ReceivedDate).HasColumnName("received_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ProcessStatus).HasColumnName("process_status").HasMaxLength(20).HasDefaultValue("PENDING");
                entity.Property(e => e.RetryCount).HasColumnName("retry_count").HasDefaultValue(0);
                entity.Property(e => e.MaxRetryCount).HasColumnName("max_retry_count").HasDefaultValue(10);
                entity.Property(e => e.NextRetryDate).HasColumnName("next_retry_date");
                entity.Property(e => e.ProcessedDate).HasColumnName("processed_date");
                entity.Property(e => e.LockToken).HasColumnName("lock_token").HasMaxLength(100);
                entity.Property(e => e.LockExpiryDate).HasColumnName("lock_expiry_date");
                entity.Property(e => e.LastError).HasColumnName("last_error");
            });

            modelBuilder.Entity<IntEventHandlerLog>(entity =>
            {
                entity.ToTable("int_event_handler_log");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.InboxEventNid).HasColumnName("inbox_event_nid").IsRequired();
                entity.Property(e => e.HandlerName).HasColumnName("handler_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.StartDate).HasColumnName("start_date").IsRequired();
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.ElapsedMs).HasColumnName("elapsed_ms").HasDefaultValue(0);
                entity.Property(e => e.HandlerStatus).HasColumnName("handler_status").HasMaxLength(20).IsRequired();
                entity.Property(e => e.ResultData).HasColumnName("result_data");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");

                entity.HasOne(d => d.InboxEvent)
                    .WithMany(p => p.EventHandlerLogs)
                    .HasForeignKey(d => d.InboxEventNid)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region 04. Dead Letter
            modelBuilder.Entity<IntDeadLetter>(entity =>
            {
                entity.ToTable("int_dead_letter");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.SourceType).HasColumnName("source_type").HasMaxLength(20).IsRequired();
                entity.Property(e => e.SourceSid).HasColumnName("source_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.EventCode).HasColumnName("event_code").HasMaxLength(150);
                entity.Property(e => e.ServiceCode).HasColumnName("service_code").HasMaxLength(80);
                entity.Property(e => e.Payload).HasColumnName("payload");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message").IsRequired();
                entity.Property(e => e.RetryCount).HasColumnName("retry_count").HasDefaultValue(0);
                entity.Property(e => e.DeadStatus).HasColumnName("dead_status").HasMaxLength(20).HasDefaultValue("OPEN");
                entity.Property(e => e.ResolvedUserSid).HasColumnName("resolved_user_sid").HasMaxLength(32);
                entity.Property(e => e.ResolvedDate).HasColumnName("resolved_date");
                entity.Property(e => e.ResolutionNote).HasColumnName("resolution_note");
            });
            #endregion

            #region 05. API冪等與跨服務呼叫
            modelBuilder.Entity<IntIdempotencyKey>(entity =>
            {
                entity.ToTable("int_idempotency_key");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.ServiceCode, e.IdempotencyKey }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IdempotencyKey).HasColumnName("idempotency_key").HasMaxLength(150).IsRequired();
                entity.Property(e => e.ServiceCode).HasColumnName("service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.OperationCode).HasColumnName("operation_code").HasMaxLength(150).IsRequired();
                entity.Property(e => e.RequestHash).HasColumnName("request_hash").HasMaxLength(128).IsRequired();
                entity.Property(e => e.RequestData).HasColumnName("request_data");
                entity.Property(e => e.ResponseStatusCode).HasColumnName("response_status_code");
                entity.Property(e => e.ResponseData).HasColumnName("response_data");
                entity.Property(e => e.ProcessStatus).HasColumnName("process_status").HasMaxLength(20).HasDefaultValue("PROCESSING");
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date").IsRequired();
                entity.Property(e => e.CompletedDate).HasColumnName("completed_date");
            });

            modelBuilder.Entity<IntApiCallLog>(entity =>
            {
                entity.ToTable("int_api_call_log");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.RequestId).HasColumnName("request_id").HasMaxLength(100).IsRequired();
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasMaxLength(100);
                entity.Property(e => e.TraceId).HasColumnName("trace_id").HasMaxLength(100);
                entity.Property(e => e.SourceServiceCode).HasColumnName("source_service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.TargetServiceCode).HasColumnName("target_service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.Method).HasColumnName("method").HasMaxLength(10).IsRequired();
                entity.Property(e => e.EndpointUrl).HasColumnName("endpoint_url").HasMaxLength(1000).IsRequired();
                entity.Property(e => e.RequestHeaders).HasColumnName("request_headers");
                entity.Property(e => e.RequestBody).HasColumnName("request_body");
                entity.Property(e => e.ResponseStatusCode).HasColumnName("response_status_code");
                entity.Property(e => e.ResponseBody).HasColumnName("response_body");
                entity.Property(e => e.ElapsedMs).HasColumnName("elapsed_ms").HasDefaultValue(0);
                entity.Property(e => e.CallStatus).HasColumnName("call_status").HasMaxLength(20).IsRequired();
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
            });
            #endregion

            #region 06. 資料同步
            modelBuilder.Entity<IntSyncDefinition>(entity =>
            {
                entity.ToTable("int_sync_definition");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SyncCode).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.SyncCode).HasColumnName("sync_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.SyncName).HasColumnName("sync_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.SourceServiceCode).HasColumnName("source_service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.TargetServiceCode).HasColumnName("target_service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(100).IsRequired();
                entity.Property(e => e.SyncMode).HasColumnName("sync_mode").HasMaxLength(30).HasDefaultValue("INCREMENTAL");
                entity.Property(e => e.ScheduleExpression).HasColumnName("schedule_expression").HasMaxLength(100);
                entity.Property(e => e.BatchSize).HasColumnName("batch_size").HasDefaultValue(500);
                entity.Property(e => e.MaxRetryCount).HasColumnName("max_retry_count").HasDefaultValue(5);
                entity.Property(e => e.SyncStatus).HasColumnName("sync_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });

            modelBuilder.Entity<IntSyncCheckpoint>(entity =>
            {
                entity.ToTable("int_sync_checkpoint");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SyncDefinitionNid).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.SyncDefinitionNid).HasColumnName("sync_definition_nid").IsRequired();
                entity.Property(e => e.CheckpointType).HasColumnName("checkpoint_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.CheckpointValue).HasColumnName("checkpoint_value").HasMaxLength(1000).IsRequired();
                entity.Property(e => e.LastSuccessDate).HasColumnName("last_success_date");
                entity.Property(e => e.LastRecordCount).HasColumnName("last_record_count").HasDefaultValue(0);
                entity.Property(e => e.VersionNo).HasColumnName("version_no").HasDefaultValue(0UL);

                entity.HasOne(d => d.SyncDefinition)
                    .WithOne(p => p.SyncCheckpoint)
                    .HasForeignKey<IntSyncCheckpoint>(d => d.SyncDefinitionNid)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<IntSyncJob>(entity =>
            {
                entity.ToTable("int_sync_job");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.JobNo).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.SyncDefinitionNid).HasColumnName("sync_definition_nid").IsRequired();
                entity.Property(e => e.JobNo).HasColumnName("job_no").HasMaxLength(80).IsRequired();
                entity.Property(e => e.StartDate).HasColumnName("start_date").IsRequired();
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.CheckpointBefore).HasColumnName("checkpoint_before").HasMaxLength(1000);
                entity.Property(e => e.CheckpointAfter).HasColumnName("checkpoint_after").HasMaxLength(1000);
                entity.Property(e => e.ReadCount).HasColumnName("read_count").HasDefaultValue(0);
                entity.Property(e => e.SuccessCount).HasColumnName("success_count").HasDefaultValue(0);
                entity.Property(e => e.FailCount).HasColumnName("fail_count").HasDefaultValue(0);
                entity.Property(e => e.SkipCount).HasColumnName("skip_count").HasDefaultValue(0);
                entity.Property(e => e.JobStatus).HasColumnName("job_status").HasMaxLength(20).HasDefaultValue("RUNNING");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");

                entity.HasOne(d => d.SyncDefinition)
                    .WithMany(p => p.SyncJobs)
                    .HasForeignKey(d => d.SyncDefinitionNid)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<IntSyncError>(entity =>
            {
                entity.ToTable("int_sync_error");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.SyncJobNid).HasColumnName("sync_job_nid").IsRequired();
                entity.Property(e => e.SourceRecordSid).HasColumnName("source_record_sid").HasMaxLength(32);
                entity.Property(e => e.TargetRecordSid).HasColumnName("target_record_sid").HasMaxLength(32);
                entity.Property(e => e.OperationType).HasColumnName("operation_type").HasMaxLength(20).IsRequired();
                entity.Property(e => e.SourceData).HasColumnName("source_data");
                entity.Property(e => e.ErrorCode).HasColumnName("error_code").HasMaxLength(100);
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message").IsRequired();
                entity.Property(e => e.RetryCount).HasColumnName("retry_count").HasDefaultValue(0);
                entity.Property(e => e.ErrorStatus).HasColumnName("error_status").HasMaxLength(20).HasDefaultValue("OPEN");
                entity.Property(e => e.ResolvedDate).HasColumnName("resolved_date");

                entity.HasOne(d => d.SyncJob)
                    .WithMany(p => p.SyncErrors)
                    .HasForeignKey(d => d.SyncJobNid)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region 07. Saga流程協調
            modelBuilder.Entity<IntSagaInstance>(entity =>
            {
                entity.ToTable("int_saga_instance");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.CorrelationId).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.SagaType).HasColumnName("saga_type").HasMaxLength(100).IsRequired();
                entity.Property(e => e.BusinessSid).HasColumnName("business_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasMaxLength(100).IsRequired();
                entity.Property(e => e.CurrentStep).HasColumnName("current_step").HasMaxLength(100);
                entity.Property(e => e.SagaStatus).HasColumnName("saga_status").HasMaxLength(20).HasDefaultValue("RUNNING");
                entity.Property(e => e.StateData).HasColumnName("state_data");
                entity.Property(e => e.StartedDate).HasColumnName("started_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.CompletedDate).HasColumnName("completed_date");
                entity.Property(e => e.LastError).HasColumnName("last_error");
            });

            modelBuilder.Entity<IntSagaStep>(entity =>
            {
                entity.ToTable("int_saga_step");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.SagaInstanceNid, e.StepNo }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.SagaInstanceNid).HasColumnName("saga_instance_nid").IsRequired();
                entity.Property(e => e.StepNo).HasColumnName("step_no").IsRequired();
                entity.Property(e => e.StepCode).HasColumnName("step_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ServiceCode).HasColumnName("service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.CommandName).HasColumnName("command_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.CompensationCommand).HasColumnName("compensation_command").HasMaxLength(200);
                entity.Property(e => e.RequestData).HasColumnName("request_data");
                entity.Property(e => e.ResponseData).HasColumnName("response_data");
                entity.Property(e => e.StepStatus).HasColumnName("step_status").HasMaxLength(20).HasDefaultValue("PENDING");
                entity.Property(e => e.RetryCount).HasColumnName("retry_count").HasDefaultValue(0);
                entity.Property(e => e.StartedDate).HasColumnName("started_date");
                entity.Property(e => e.CompletedDate).HasColumnName("completed_date");
                entity.Property(e => e.LastError).HasColumnName("last_error");

                entity.HasOne(d => d.SagaInstance)
                    .WithMany(p => p.SagaSteps)
                    .HasForeignKey(d => d.SagaInstanceNid)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion
        }
    }
}