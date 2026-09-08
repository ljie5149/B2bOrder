using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.ShareCore.Models;

namespace B2bOrder.Resources.ShareCore
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options) { }

        #region DbSets
        public DbSet<LogTraceSpan> LogTraceSpans { get; set; } = null!;
        public DbSet<LogSystemException> LogSystemExceptions { get; set; } = null!;
        public DbSet<LogThirdPartyApi> LogThirdPartyApis { get; set; } = null!;
        public DbSet<LogMessageQueue> LogMessageQueues { get; set; } = null!;
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region 01. 分散式鏈路追蹤日誌 (Distributed Tracing Log)
            modelBuilder.Entity<LogTraceSpan>(entity =>
            {
                entity.ToTable("log_trace_span");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.TraceId).HasColumnName("trace_id").HasMaxLength(100).IsRequired();
                entity.Property(e => e.SpanId).HasColumnName("span_id").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ParentSpanId).HasColumnName("parent_span_id").HasMaxLength(100);
                entity.Property(e => e.ServiceName).HasColumnName("service_name").HasMaxLength(50).IsRequired();
                entity.Property(e => e.OperationName).HasColumnName("operation_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.SpanKind).HasColumnName("span_kind").HasMaxLength(20).HasDefaultValue("SERVER");
                entity.Property(e => e.StartTime).HasColumnName("start_time").HasPrecision(3).IsRequired();
                entity.Property(e => e.EndTime).HasColumnName("end_time").HasPrecision(3).IsRequired();
                entity.Property(e => e.DurationMs).HasColumnName("duration_ms").HasDefaultValue(0L);
                entity.Property(e => e.HttpStatusCode).HasColumnName("http_status_code");
                entity.Property(e => e.HasError).HasColumnName("has_error").HasMaxLength(2).HasDefaultValue("N");
                entity.Property(e => e.HostIp).HasColumnName("host_ip").HasMaxLength(45);
                entity.Property(e => e.TagsJson).HasColumnName("tags_json").HasColumnType("json");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });
            #endregion

            #region 02. 應用程式系統異常日誌 (System Error & Exception Log)
            modelBuilder.Entity<LogSystemException>(entity =>
            {
                entity.ToTable("log_system_exception");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32);
                entity.Property(e => e.ServiceName).HasColumnName("service_name").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Environment).HasColumnName("environment").HasMaxLength(20).HasDefaultValue("PROD");
                entity.Property(e => e.LogLevel).HasColumnName("log_level").HasMaxLength(20).HasDefaultValue("ERROR");
                entity.Property(e => e.ExceptionType).HasColumnName("exception_type").HasMaxLength(250).IsRequired();
                entity.Property(e => e.Message).HasColumnName("message").IsRequired();
                entity.Property(e => e.StackTrace).HasColumnName("stack_trace");
                entity.Property(e => e.RequestUri).HasColumnName("request_uri").HasMaxLength(500);
                entity.Property(e => e.HttpMethod).HasColumnName("http_method").HasMaxLength(10);
                entity.Property(e => e.OperatorUserSid).HasColumnName("operator_user_sid").HasMaxLength(32);
                entity.Property(e => e.ClientIp).HasColumnName("client_ip").HasMaxLength(45);
                entity.Property(e => e.TraceId).HasColumnName("trace_id").HasMaxLength(100);
                entity.Property(e => e.IsResolved).HasColumnName("is_resolved").HasMaxLength(2).HasDefaultValue("N");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });
            #endregion

            #region 03. 第三方對接通訊日誌 (Third-Party Integration Log)
            modelBuilder.Entity<LogThirdPartyApi>(entity =>
            {
                entity.ToTable("log_third_party_api");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32);
                entity.Property(e => e.ProviderName).HasColumnName("provider_name").HasMaxLength(50).IsRequired();
                entity.Property(e => e.ApiAction).HasColumnName("api_action").HasMaxLength(100).IsRequired();
                entity.Property(e => e.BusinessKey).HasColumnName("business_key").HasMaxLength(100);
                entity.Property(e => e.RequestUrl).HasColumnName("request_url").HasMaxLength(500).IsRequired();
                entity.Property(e => e.HttpMethod).HasColumnName("http_method").HasMaxLength(10).HasDefaultValue("POST");
                entity.Property(e => e.RequestHeaderJson).HasColumnName("request_header_json").HasColumnType("json");
                entity.Property(e => e.RequestPayload).HasColumnName("request_payload");
                entity.Property(e => e.ResponseCode).HasColumnName("response_code");
                entity.Property(e => e.ResponsePayload).HasColumnName("response_payload");
                entity.Property(e => e.ExecutionTimeMs).HasColumnName("execution_time_ms").HasDefaultValue(0L);
                entity.Property(e => e.IsSuccess).HasColumnName("is_success").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
                entity.Property(e => e.TraceId).HasColumnName("trace_id").HasMaxLength(100);
                entity.Property(e => e.Remark).HasColumnName("remark");
            });
            #endregion

            #region 04. 訊息佇列與事件流日誌 (Message Queue & Event Log)
            modelBuilder.Entity<LogMessageQueue>(entity =>
            {
                entity.ToTable("log_message_queue");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32);
                entity.Property(e => e.ExchangeOrTopic).HasColumnName("exchange_or_topic").HasMaxLength(100).IsRequired();
                entity.Property(e => e.RoutingKey).HasColumnName("routing_key").HasMaxLength(100).IsRequired();
                entity.Property(e => e.QueueName).HasColumnName("queue_name").HasMaxLength(100);
                entity.Property(e => e.ActionType).HasColumnName("action_type").HasMaxLength(20).IsRequired();
                entity.Property(e => e.MessageBodyJson).HasColumnName("message_body_json").HasColumnType("json");
                entity.Property(e => e.ProcessStatus).HasColumnName("process_status").HasMaxLength(20).HasDefaultValue("SUCCESS");
                entity.Property(e => e.RetryCount).HasColumnName("retry_count").HasDefaultValue(0);
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
                entity.Property(e => e.TraceId).HasColumnName("trace_id").HasMaxLength(100);
                entity.Property(e => e.Remark).HasColumnName("remark");
            });
            #endregion
        }
    }
}