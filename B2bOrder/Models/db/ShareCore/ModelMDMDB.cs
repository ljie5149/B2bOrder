using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.Mdm
{
    #region 領域與來源 (Domains & Sources)

    [Table("mdm_domain")]
    public class MdmDomain
    {
        [Key]
        [Column("domain_id")]
        public long DomainId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("domain_code")]
        public string DomainCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("domain_name")]
        public string DomainName { get; set; } = null!;

        [MaxLength(255)]
        [Column("description")]
        public string? Description { get; set; }

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mdm_source_system")]
    public class MdmSourceSystem
    {
        [Key]
        [Column("source_system_id")]
        public long SourceSystemId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("system_code")]
        public string SystemCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("system_name")]
        public string SystemName { get; set; } = null!;

        [Column("trust_score")]
        public int TrustScore { get; set; } = 50;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mdm_domain_source")]
    public class MdmDomainSource
    {
        [Key]
        [Column("domain_source_id")]
        public long DomainSourceId { get; set; }

        [Column("domain_id")]
        public long DomainId { get; set; }

        [Column("source_system_id")]
        public long SourceSystemId { get; set; }

        [Column("priority")]
        public int Priority { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    #endregion

    #region Golden Record (單一真實資料來源)

    [Table("mdm_record")]
    public class MdmRecord
    {
        [Key]
        [Column("record_id")]
        public long RecordId { get; set; }

        [Column("domain_id")]
        public long DomainId { get; set; }

        [Column("source_system_id")]
        public long SourceSystemId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("source_record_id")]
        public string SourceRecordId { get; set; } = null!;

        [Column("payload", TypeName = "json")]
        public string? Payload { get; set; }

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mdm_golden_record")]
    public class MdmGoldenRecord
    {
        [Key]
        [Column("golden_id")]
        public long GoldenId { get; set; }

        [Column("domain_id")]
        public long DomainId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("golden_code")]
        public string GoldenCode { get; set; } = null!;

        [Column("merged_payload", TypeName = "json")]
        public string? MergedPayload { get; set; }

        [Column("version")]
        public int Version { get; set; } = 1;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mdm_golden_record_source")]
    public class MdmGoldenRecordSource
    {
        [Key]
        [Column("golden_source_id")]
        public long GoldenSourceId { get; set; }

        [Column("golden_id")]
        public long GoldenId { get; set; }

        [Column("record_id")]
        public long RecordId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    #endregion

    #region 比對與合併 (Matching & Merging)

    [Table("mdm_match_rule")]
    public class MdmMatchRule
    {
        [Key]
        [Column("rule_id")]
        public long RuleId { get; set; }

        [Column("domain_id")]
        public long DomainId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("rule_name")]
        public string RuleName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("match_type")]
        public string MatchType { get; set; } = null!; // Exact, Fuzzy, Rules Engine

        [Column("rule_config", TypeName = "json")]
        public string? RuleConfig { get; set; }

        [Column("threshold", TypeName = "decimal(5,2)")]
        public decimal Threshold { get; set; }

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mdm_match_candidate")]
    public class MdmMatchCandidate
    {
        [Key]
        [Column("candidate_id")]
        public long CandidateId { get; set; }

        [Column("rule_id")]
        public long RuleId { get; set; }

        [Column("source_record_id")]
        public long SourceRecordId { get; set; }

        [Column("target_record_id")]
        public long TargetRecordId { get; set; }

        [Column("match_score", TypeName = "decimal(5,2)")]
        public decimal MatchScore { get; set; }

        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mdm_merge_job")]
    public class MdmMergeJob
    {
        [Key]
        [Column("job_id")]
        public long JobId { get; set; }

        [Column("domain_id")]
        public long DomainId { get; set; }

        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed

        [Column("total_processed")]
        public int TotalProcessed { get; set; } = 0;

        [Column("started_at")]
        public DateTime? StartedAt { get; set; }

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mdm_merge_history")]
    public class MdmMergeHistory
    {
        [Key]
        [Column("history_id")]
        public long HistoryId { get; set; }

        [Column("golden_id")]
        public long GoldenId { get; set; }

        [Column("source_record_id")]
        public long SourceRecordId { get; set; }

        [MaxLength(50)]
        [Column("action_type")]
        public string ActionType { get; set; } = null!; // Merge, Unmerge

        [Column("executed_by")]
        public long? ExecutedBy { get; set; }

        [Column("executed_at")]
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    }

    #endregion

    #region 存續與標準化 (Survivorship & Normalization)

    [Table("mdm_survivorship_rule")]
    public class MdmSurvivorshipRule
    {
        [Key]
        [Column("survivorship_rule_id")]
        public long SurvivorshipRuleId { get; set; }

        [Column("domain_id")]
        public long DomainId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("target_field")]
        public string TargetField { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("strategy")]
        public string Strategy { get; set; } = null!; // MostRecent, MostTrusted, Frequency

        [Column("rule_config", TypeName = "json")]
        public string? RuleConfig { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mdm_normalization_rule")]
    public class MdmNormalizationRule
    {
        [Key]
        [Column("normalization_rule_id")]
        public long NormalizationRuleId { get; set; }

        [Column("domain_id")]
        public long DomainId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("target_field")]
        public string TargetField { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("rule_type")]
        public string RuleType { get; set; } = null!; // Trim, Upper, Regex, FormatPhone

        [MaxLength(255)]
        [Column("rule_expression")]
        public string? RuleExpression { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mdm_code_mapping")]
    public class MdmCodeMapping
    {
        [Key]
        [Column("mapping_id")]
        public long MappingId { get; set; }

        [Column("domain_id")]
        public long DomainId { get; set; }

        [Column("source_system_id")]
        public long SourceSystemId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("code_type")]
        public string CodeType { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("source_code")]
        public string SourceCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("target_code")]
        public string TargetCode { get; set; } = null!;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    #endregion

    #region 品質與變更 (Quality, Version & Change Requests)

    [Table("mdm_quality_rule")]
    public class MdmQualityRule
    {
        [Key]
        [Column("rule_id")]
        public long RuleId { get; set; }

        [Column("domain_id")]
        public long DomainId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("rule_name")]
        public string RuleName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("rule_type")]
        public string RuleType { get; set; } = null!; // Required, Regex, Range, Custom

        [Column("rule_expression", TypeName = "text")]
        public string? RuleExpression { get; set; }

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mdm_quality_result")]
    public class MdmQualityResult
    {
        [Key]
        [Column("result_id")]
        public long ResultId { get; set; }

        [Column("rule_id")]
        public long RuleId { get; set; }

        [Column("record_id")]
        public long RecordId { get; set; }

        [Column("is_passed")]
        public bool IsPassed { get; set; }

        [MaxLength(255)]
        [Column("error_message")]
        public string? ErrorMessage { get; set; }

        [Column("checked_at")]
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mdm_golden_version")]
    public class MdmGoldenVersion
    {
        [Key]
        [Column("version_id")]
        public long VersionId { get; set; }

        [Column("golden_id")]
        public long GoldenId { get; set; }

        [Column("version")]
        public int Version { get; set; }

        [Column("payload_snapshot", TypeName = "json")]
        public string? PayloadSnapshot { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mdm_change_request")]
    public class MdmChangeRequest
    {
        [Key]
        [Column("request_id")]
        public long RequestId { get; set; }

        [Column("golden_id")]
        public long GoldenId { get; set; }

        [Column("proposed_payload", TypeName = "json")]
        public string ProposedPayload { get; set; } = null!;

        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        [Column("requested_by")]
        public long RequestedBy { get; set; }

        [Column("reviewed_by")]
        public long? ReviewedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("reviewed_at")]
        public DateTime? ReviewedAt { get; set; }
    }

    [Table("mdm_publish_job")]
    public class MdmPublishJob
    {
        [Key]
        [Column("publish_job_id")]
        public long PublishJobId { get; set; }

        [Column("golden_id")]
        public long GoldenId { get; set; }

        [Column("target_system_id")]
        public long TargetSystemId { get; set; }

        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed

        [Column("retry_count")]
        public int RetryCount { get; set; } = 0;

        [Column("error_message", TypeName = "text")]
        public string? ErrorMessage { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("published_at")]
        public DateTime? PublishedAt { get; set; }
    }

    [Table("mdm_sync_checkpoint")]
    public class MdmSyncCheckpoint
    {
        [Key]
        [Column("checkpoint_id")]
        public long CheckpointId { get; set; }

        [Column("source_system_id")]
        public long SourceSystemId { get; set; }

        [Column("domain_id")]
        public long DomainId { get; set; }

        [Column("last_sync_time")]
        public DateTime LastSyncTime { get; set; }

        [MaxLength(100)]
        [Column("last_sync_offset")]
        public string? LastSyncOffset { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mdm_steward_assignment")]
    public class MdmStewardAssignment
    {
        [Key]
        [Column("assignment_id")]
        public long AssignmentId { get; set; }

        [Column("domain_id")]
        public long DomainId { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [MaxLength(50)]
        [Column("role")]
        public string Role { get; set; } = "Steward"; // Steward, Owner, Reviewer

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    #endregion
}