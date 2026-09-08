using System;

namespace B2bOrder.Resources.ShareCore.Models
{
    #region 01. 分散式鏈路追蹤日誌 (Distributed Tracing Log)
    public class LogTraceSpan
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string TraceId { get; set; } = null!;
        public string SpanId { get; set; } = null!;
        public string? ParentSpanId { get; set; }
        public string ServiceName { get; set; } = null!;
        public string OperationName { get; set; } = null!;
        public string SpanKind { get; set; } = "SERVER";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long DurationMs { get; set; } = 0;
        public int? HttpStatusCode { get; set; }
        public string HasError { get; set; } = "N";
        public string? HostIp { get; set; }
        public string? TagsJson { get; set; }
        public string? Remark { get; set; }
    }
    #endregion

    #region 02. 應用程式系統異常日誌 (System Error & Exception Log)
    public class LogSystemException
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string? CompanySid { get; set; }
        public string ServiceName { get; set; } = null!;
        public string Environment { get; set; } = "PROD";
        public string LogLevel { get; set; } = "ERROR";
        public string ExceptionType { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? StackTrace { get; set; }
        public string? RequestUri { get; set; }
        public string? HttpMethod { get; set; }
        public string? OperatorUserSid { get; set; }
        public string? ClientIp { get; set; }
        public string? TraceId { get; set; }
        public string IsResolved { get; set; } = "N";
        public string? Remark { get; set; }
    }
    #endregion

    #region 03. 第三方對接通訊日誌 (Third-Party Integration Log)
    public class LogThirdPartyApi
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string? CompanySid { get; set; }
        public string ProviderName { get; set; } = null!;
        public string ApiAction { get; set; } = null!;
        public string? BusinessKey { get; set; }
        public string RequestUrl { get; set; } = null!;
        public string HttpMethod { get; set; } = "POST";
        public string? RequestHeaderJson { get; set; }
        public string? RequestPayload { get; set; }
        public int? ResponseCode { get; set; }
        public string? ResponsePayload { get; set; }
        public long ExecutionTimeMs { get; set; } = 0;
        public string IsSuccess { get; set; } = "Y";
        public string? ErrorMessage { get; set; }
        public string? TraceId { get; set; }
        public string? Remark { get; set; }
    }
    #endregion

    #region 04. 訊息佇列與事件流日誌 (Message Queue & Event Log)
    public class LogMessageQueue
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string? CompanySid { get; set; }
        public string ExchangeOrTopic { get; set; } = null!;
        public string RoutingKey { get; set; } = null!;
        public string? QueueName { get; set; }
        public string ActionType { get; set; } = null!;
        public string? MessageBodyJson { get; set; }
        public string ProcessStatus { get; set; } = "SUCCESS";
        public int RetryCount { get; set; } = 0;
        public string? ErrorMessage { get; set; }
        public string? TraceId { get; set; }
        public string? Remark { get; set; }
    }
    #endregion
}