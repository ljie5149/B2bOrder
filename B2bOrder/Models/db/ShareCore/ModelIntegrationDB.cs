using System;
using System.Collections.Generic;

namespace B2bOrder.Resources.ShareCore.Models
{
    #region 01. 服務與事件定義
    public class IntServiceRegistry
    {
        public uint Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ServiceCode { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public string? ServiceUrl { get; set; }
        public string? HealthCheckUrl { get; set; }
        public string? ServiceVersion { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
        public int RetryCount { get; set; } = 3;
        public string ServiceStatus { get; set; } = "ACTIVE";
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }
    }

    public class IntEventDefinition
    {
        public uint Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string EventCode { get; set; } = null!;
        public string EventName { get; set; } = null!;
        public string AggregateType { get; set; } = null!;
        public string SourceServiceCode { get; set; } = null!;
        public string SchemaVersion { get; set; } = "1.0";
        public string? PayloadSchema { get; set; }
        public int RetentionDays { get; set; } = 90;
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }
    }
    #endregion

    #region 02. Transactional Outbox
    public class IntOutboxEvent
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string EventId { get; set; } = null!;
        public string EventCode { get; set; } = null!;
        public string AggregateType { get; set; } = null!;
        public string AggregateSid { get; set; } = null!;
        public string SourceServiceCode { get; set; } = null!;
        public string? CorrelationId { get; set; }
        public string? CausationId { get; set; }
        public string? TraceId { get; set; }
        public string Payload { get; set; } = null!;
        public string? Headers { get; set; }
        public int EventVersion { get; set; } = 1;
        public DateTime OccurredDate { get; set; }
        public string PublishStatus { get; set; } = "PENDING";
        public int RetryCount { get; set; } = 0;
        public int MaxRetryCount { get; set; } = 10;
        public DateTime? NextRetryDate { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string? LockToken { get; set; }
        public DateTime? LockExpiryDate { get; set; }
        public string? LastError { get; set; }

        public virtual ICollection<IntOutboxDelivery> OutboxDeliveries { get; set; } = new List<IntOutboxDelivery>();
    }

    public class IntOutboxDelivery
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong OutboxEventNid { get; set; }
        public string TargetServiceCode { get; set; } = null!;
        public string? EndpointUrl { get; set; }
        public string DeliveryStatus { get; set; } = "PENDING";
        public int RetryCount { get; set; } = 0;
        public int MaxRetryCount { get; set; } = 10;
        public DateTime? NextRetryDate { get; set; }
        public string? RequestData { get; set; }
        public int? ResponseStatusCode { get; set; }
        public string? ResponseData { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string? LastError { get; set; }

        public virtual IntOutboxEvent OutboxEvent { get; set; } = null!;
    }
    #endregion

    #region 03. Inbox與事件消費
    public class IntInboxEvent
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string EventId { get; set; } = null!;
        public string EventCode { get; set; } = null!;
        public string SourceServiceCode { get; set; } = null!;
        public string TargetServiceCode { get; set; } = null!;
        public string AggregateType { get; set; } = null!;
        public string AggregateSid { get; set; } = null!;
        public string? CorrelationId { get; set; }
        public string? TraceId { get; set; }
        public string Payload { get; set; } = null!;
        public string? Headers { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string ProcessStatus { get; set; } = "PENDING";
        public int RetryCount { get; set; } = 0;
        public int MaxRetryCount { get; set; } = 10;
        public DateTime? NextRetryDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string? LockToken { get; set; }
        public DateTime? LockExpiryDate { get; set; }
        public string? LastError { get; set; }

        public virtual ICollection<IntEventHandlerLog> EventHandlerLogs { get; set; } = new List<IntEventHandlerLog>();
    }

    public class IntEventHandlerLog
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong InboxEventNid { get; set; }
        public string HandlerName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ElapsedMs { get; set; } = 0;
        public string HandlerStatus { get; set; } = null!;
        public string? ResultData { get; set; }
        public string? ErrorMessage { get; set; }

        public virtual IntInboxEvent InboxEvent { get; set; } = null!;
    }
    #endregion

    #region 04. Dead Letter
    public class IntDeadLetter
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string SourceType { get; set; } = null!;
        public string SourceSid { get; set; } = null!;
        public string? EventCode { get; set; }
        public string? ServiceCode { get; set; }
        public string? Payload { get; set; }
        public string ErrorMessage { get; set; } = null!;
        public int RetryCount { get; set; } = 0;
        public string DeadStatus { get; set; } = "OPEN";
        public string? ResolvedUserSid { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string? ResolutionNote { get; set; }
    }
    #endregion

    #region 05. API冪等與跨服務呼叫
    public class IntIdempotencyKey
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string IdempotencyKey { get; set; } = null!;
        public string ServiceCode { get; set; } = null!;
        public string OperationCode { get; set; } = null!;
        public string RequestHash { get; set; } = null!;
        public string? RequestData { get; set; }
        public int? ResponseStatusCode { get; set; }
        public string? ResponseData { get; set; }
        public string ProcessStatus { get; set; } = "PROCESSING";
        public DateTime ExpiryDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }

    public class IntApiCallLog
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string RequestId { get; set; } = null!;
        public string? CorrelationId { get; set; }
        public string? TraceId { get; set; }
        public string SourceServiceCode { get; set; } = null!;
        public string TargetServiceCode { get; set; } = null!;
        public string Method { get; set; } = null!;
        public string EndpointUrl { get; set; } = null!;
        public string? RequestHeaders { get; set; }
        public string? RequestBody { get; set; }
        public int? ResponseStatusCode { get; set; }
        public string? ResponseBody { get; set; }
        public int ElapsedMs { get; set; } = 0;
        public string CallStatus { get; set; } = null!;
        public string? ErrorMessage { get; set; }
    }
    #endregion

    #region 06. 資料同步
    public class IntSyncDefinition
    {
        public uint Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string SyncCode { get; set; } = null!;
        public string SyncName { get; set; } = null!;
        public string SourceServiceCode { get; set; } = null!;
        public string TargetServiceCode { get; set; } = null!;
        public string EntityType { get; set; } = null!;
        public string SyncMode { get; set; } = "INCREMENTAL";
        public string? ScheduleExpression { get; set; }
        public int BatchSize { get; set; } = 500;
        public int MaxRetryCount { get; set; } = 5;
        public string SyncStatus { get; set; } = "ACTIVE";
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual IntSyncCheckpoint? SyncCheckpoint { get; set; }
        public virtual ICollection<IntSyncJob> SyncJobs { get; set; } = new List<IntSyncJob>();
    }

    public class IntSyncCheckpoint
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public uint SyncDefinitionNid { get; set; }
        public string CheckpointType { get; set; } = null!;
        public string CheckpointValue { get; set; } = null!;
        public DateTime? LastSuccessDate { get; set; }
        public int LastRecordCount { get; set; } = 0;
        public ulong VersionNo { get; set; } = 0;

        public virtual IntSyncDefinition SyncDefinition { get; set; } = null!;
    }

    public class IntSyncJob
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public uint SyncDefinitionNid { get; set; }
        public string JobNo { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CheckpointBefore { get; set; }
        public string? CheckpointAfter { get; set; }
        public int ReadCount { get; set; } = 0;
        public int SuccessCount { get; set; } = 0;
        public int FailCount { get; set; } = 0;
        public int SkipCount { get; set; } = 0;
        public string JobStatus { get; set; } = "RUNNING";
        public string? ErrorMessage { get; set; }

        public virtual IntSyncDefinition SyncDefinition { get; set; } = null!;
        public virtual ICollection<IntSyncError> SyncErrors { get; set; } = new List<IntSyncError>();
    }

    public class IntSyncError
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong SyncJobNid { get; set; }
        public string? SourceRecordSid { get; set; }
        public string? TargetRecordSid { get; set; }
        public string OperationType { get; set; } = null!;
        public string? SourceData { get; set; }
        public string? ErrorCode { get; set; }
        public string ErrorMessage { get; set; } = null!;
        public int RetryCount { get; set; } = 0;
        public string ErrorStatus { get; set; } = "OPEN";
        public DateTime? ResolvedDate { get; set; }

        public virtual IntSyncJob SyncJob { get; set; } = null!;
    }
    #endregion

    #region 07. Saga流程協調
    public class IntSagaInstance
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string SagaType { get; set; } = null!;
        public string BusinessSid { get; set; } = null!;
        public string CorrelationId { get; set; } = null!;
        public string? CurrentStep { get; set; }
        public string SagaStatus { get; set; } = "RUNNING";
        public string? StateData { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? LastError { get; set; }

        public virtual ICollection<IntSagaStep> SagaSteps { get; set; } = new List<IntSagaStep>();
    }

    public class IntSagaStep
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong SagaInstanceNid { get; set; }
        public int StepNo { get; set; }
        public string StepCode { get; set; } = null!;
        public string ServiceCode { get; set; } = null!;
        public string CommandName { get; set; } = null!;
        public string? CompensationCommand { get; set; }
        public string? RequestData { get; set; }
        public string? ResponseData { get; set; }
        public string StepStatus { get; set; } = "PENDING";
        public int RetryCount { get; set; } = 0;
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? LastError { get; set; }

        public virtual IntSagaInstance SagaInstance { get; set; } = null!;
    }
    #endregion
}