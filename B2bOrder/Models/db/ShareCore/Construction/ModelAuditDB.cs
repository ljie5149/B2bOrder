using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.ShareCore.Construction.Models
{
    #region Data Change Audit Module - 實體資料變更歷程稽核模組

    [Table("adt_data_change_log")]
    public class AdtDataChangeLog
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("service_name")]
        public string ServiceName { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("table_name")]
        public string TableName { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("entity_sid")]
        public string EntitySid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("action_type")]
        public string ActionType { get; set; }

        [Column("before_data_json")]
        public string BeforeDataJson { get; set; }

        [Column("after_data_json")]
        public string AfterDataJson { get; set; }

        [Column("changed_fields_json")]
        public string ChangedFieldsJson { get; set; }

        [MaxLength(32)]
        [Column("operator_user_sid")]
        public string OperatorUserSid { get; set; }

        [MaxLength(100)]
        [Column("operator_name")]
        public string OperatorName { get; set; }

        [MaxLength(100)]
        [Column("trace_id")]
        public string TraceId { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Operation & Access Log Module - 操作行為與 API 存取稽核模組

    [Table("adt_operation_log")]
    public class AdtOperationLog
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("service_name")]
        public string ServiceName { get; set; }

        [MaxLength(100)]
        [Column("module_name")]
        public string ModuleName { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("action_title")]
        public string ActionTitle { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("http_method")]
        public string HttpMethod { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("request_uri")]
        public string RequestUri { get; set; }

        [Column("request_params_json")]
        public string RequestParamsJson { get; set; }

        [Column("response_code")]
        public int ResponseCode { get; set; }

        [Column("execution_time_ms")]
        public long ExecutionTimeMs { get; set; }

        [MaxLength(32)]
        [Column("operator_user_sid")]
        public string OperatorUserSid { get; set; }

        [MaxLength(45)]
        [Column("operator_ip")]
        public string OperatorIp { get; set; }

        [MaxLength(500)]
        [Column("user_agent")]
        public string UserAgent { get; set; }

        [MaxLength(100)]
        [Column("trace_id")]
        public string TraceId { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Authentication & Security Event Module - 身分驗證與安全事件稽核模組

    [Table("adt_security_event")]
    public class AdtSecurityEvent
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [MaxLength(32)]
        [Column("user_sid")]
        public string UserSid { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("account_identifier")]
        public string AccountIdentifier { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("event_type")]
        public string EventType { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("severity_level")]
        public string SeverityLevel { get; set; }

        [Required]
        [MaxLength(45)]
        [Column("client_ip")]
        public string ClientIp { get; set; }

        [MaxLength(100)]
        [Column("location_geo")]
        public string LocationGeo { get; set; }

        [MaxLength(500)]
        [Column("user_agent")]
        public string UserAgent { get; set; }

        [MaxLength(200)]
        [Column("failure_reason")]
        public string FailureReason { get; set; }

        [MaxLength(100)]
        [Column("trace_id")]
        public string TraceId { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Sensitive Data Access Module - 敏感個資檢視與匯出稽核模組

    [Table("adt_sensitive_access_log")]
    public class AdtSensitiveAccessLog
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("operator_user_sid")]
        public string OperatorUserSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("data_type")]
        public string DataType { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("access_action")]
        public string AccessAction { get; set; }

        [Column("target_entity_sids_json")]
        public string TargetEntitySidsJson { get; set; }

        [Column("record_count")]
        public int RecordCount { get; set; }

        [Column("query_condition_json")]
        public string QueryConditionJson { get; set; }

        [MaxLength(500)]
        [Column("justification_reason")]
        public string JustificationReason { get; set; }

        [Required]
        [MaxLength(45)]
        [Column("client_ip")]
        public string ClientIp { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion
}