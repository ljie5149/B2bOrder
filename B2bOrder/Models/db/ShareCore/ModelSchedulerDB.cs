using System;

namespace B2bOrder.Resources.ShareCore.Models
{
    #region 01. 分散式排程任務主檔 (Cron Job Master Definition)
    public class SchJobDefinition
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CompanySid { get; set; } = null!;
        public string JobCode { get; set; } = null!;
        public string JobGroup { get; set; } = "DEFAULT";
        public string JobName { get; set; } = null!;
        public string CronExpression { get; set; } = null!;
        public string TargetService { get; set; } = null!;
        public string TargetEndpoint { get; set; } = null!;
        public string HttpMethod { get; set; } = "POST";
        public string? PayloadJson { get; set; }
        public string ConcurrentAllowed { get; set; } = "N";
        public int MaxRetryCount { get; set; } = 3;
        public int TimeoutSeconds { get; set; } = 3600;
        public string JobStatus { get; set; } = "PAUSED";
        public DateTime? LastExecutedAt { get; set; }
        public DateTime? NextFireTime { get; set; }
        public ulong VersionNo { get; set; } = 0;
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }
    }
    #endregion

    #region 02. 分散式任務執行鎖 (Distributed Job Lock)
    public class SchJobLock
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string JobCode { get; set; } = null!;
        public string LockedByNode { get; set; } = null!;
        public DateTime LockedAt { get; set; }
        public DateTime LockExpiredAt { get; set; }
        public ulong VersionNo { get; set; } = 0;
    }
    #endregion

    #region 03. 排程執行歷程日誌 (Job Execution History)
    public class SchExecutionLog
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string CompanySid { get; set; } = null!;
        public string JobCode { get; set; } = null!;
        public string ExecutedByNode { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public long? ExecutionTimeMs { get; set; }
        public string ExecutionStatus { get; set; } = "RUNNING";
        public int RetryCount { get; set; } = 0;
        public int AffectedRows { get; set; } = 0;
        public string? ResultMessage { get; set; }
        public string? ErrorStackTrace { get; set; }
        public string? Remark { get; set; }
    }
    #endregion

    #region 04. 延遲與一次性異步任務 (Delayed & One-off Tasks)
    public class SchDelayedTask
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CompanySid { get; set; } = null!;
        public string TaskType { get; set; } = null!;
        public string BusinessKey { get; set; } = null!;
        public DateTime ScheduledExecuteTime { get; set; }
        public string TargetService { get; set; } = null!;
        public string TargetEndpoint { get; set; } = null!;
        public string? PayloadJson { get; set; }
        public string TaskStatus { get; set; } = "WAITING";
        public int RetryCount { get; set; } = 0;
        public int MaxRetryCount { get; set; } = 3;
        public DateTime? ExecutedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public ulong VersionNo { get; set; } = 0;
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }
    }
    #endregion
}