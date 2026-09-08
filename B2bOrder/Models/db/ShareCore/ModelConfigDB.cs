using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.Config
{
    #region 01. 分散式系統組態主檔 (System Configurations)

    [Table("cfg_system_config")]
    public class CfgSystemConfig
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("env_code")]
        public string EnvCode { get; set; } = "PROD";

        [Required]
        [MaxLength(50)]
        [Column("service_name")]
        public string ServiceName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("config_group")]
        public string ConfigGroup { get; set; } = "DEFAULT";

        [Required]
        [MaxLength(100)]
        [Column("config_key")]
        public string ConfigKey { get; set; } = null!;

        [Required]
        [Column("config_value", TypeName = "text")]
        public string ConfigValue { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("value_type")]
        public string ValueType { get; set; } = "STRING";

        [Required]
        [MaxLength(2)]
        [Column("is_encrypted")]
        public string IsEncrypted { get; set; } = "N";

        [Required]
        [MaxLength(2)]
        [Column("is_dynamic")]
        public string IsDynamic { get; set; } = "Y";

        [Required]
        [MaxLength(20)]
        [Column("config_status")]
        public string ConfigStatus { get; set; } = "ACTIVE";

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }

    #endregion

    #region 02. 功能開關與灰度發布 (Feature Flags & Canary Toggles)

    [Table("cfg_feature_flag")]
    public class CfgFeatureFlag
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("feature_code")]
        public string FeatureCode { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        [Column("feature_name")]
        public string FeatureName { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("toggle_strategy")]
        public string ToggleStrategy { get; set; } = "BOOLEAN";

        [Required]
        [MaxLength(2)]
        [Column("is_enabled")]
        public string IsEnabled { get; set; } = "N";

        [Column("percentage_rollout")]
        public int PercentageRollout { get; set; } = 0;

        [Column("whitelist_rules_json", TypeName = "json")]
        public string? WhitelistRulesJson { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }

    #endregion

    #region 03. 通用字典與代碼對照 (System Dictionaries & Lookups)

    [Table("cfg_dictionary_category")]
    public class CfgDictionaryCategory
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("category_code")]
        public string CategoryCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("category_name")]
        public string CategoryName { get; set; } = null!;

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }

    [Table("cfg_dictionary_item")]
    public class CfgDictionaryItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("category_sid")]
        public string CategorySid { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("item_code")]
        public string ItemCode { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("item_value")]
        public string ItemValue { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        [Column("item_locale")]
        public string ItemLocale { get; set; } = "zh-TW";

        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [Required]
        [MaxLength(2)]
        [Column("is_default")]
        public string IsDefault { get; set; } = "N";

        [Column("extra_attribute_json", TypeName = "json")]
        public string? ExtraAttributeJson { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }

    #endregion

    #region 04. 組態異動歷程與快照 (Config History & Snapshots)

    [Table("cfg_change_history")]
    public class CfgChangeHistory
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("config_type")]
        public string ConfigType { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("target_sid")]
        public string TargetSid { get; set; } = null!;

        [Column("before_value_json", TypeName = "json")]
        public string? BeforeValueJson { get; set; }

        [Required]
        [Column("after_value_json", TypeName = "json")]
        public string AfterValueJson { get; set; } = null!;

        [MaxLength(32)]
        [Column("operator_user_sid")]
        public string? OperatorUserSid { get; set; }

        [MaxLength(100)]
        [Column("operator_name")]
        public string? OperatorName { get; set; }

        [MaxLength(500)]
        [Column("change_reason")]
        public string? ChangeReason { get; set; }

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }

    #endregion
}