using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.Db
{
    // 1. 系統參數設定表 (你提供的，已補上其餘欄位屬性)
    [Table("sys_config")]
    public class SysConfig
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Required]
        [StringLength(100)]
        [Column("config_key")]
        public string ConfigKey { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("config_name")]
        public string ConfigName { get; set; } = string.Empty;

        [Column("config_value")]
        public string? ConfigValue { get; set; }

        [StringLength(20)]
        [Column("config_type")]
        public string ConfigType { get; set; } = "TEXT";

        [StringLength(50)]
        [Column("category")]
        public string? Category { get; set; }

        [Column("script")]
        public string? Script { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }
    }

    // 2. 系統功能模組設定
    [Table("sys_module")]
    public class SysModule
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Required]
        [StringLength(50)]
        [Column("module_code")]
        public string ModuleCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("module_name")]
        public string ModuleName { get; set; } = string.Empty;

        [StringLength(500)]
        [Column("module_desc")]
        public string? ModuleDesc { get; set; }

        [StringLength(100)]
        [Column("icon")]
        public string? Icon { get; set; }

        [StringLength(255)]
        [Column("route_url")]
        public string? RouteUrl { get; set; }

        [Column("sort_no")]
        public int SortNo { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }
    }

    // 3. 通知事件設定
    [Table("sys_notice_event")]
    public class SysNoticeEvent
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Required]
        [StringLength(50)]
        [Column("event_code")]
        public string EventCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("event_name")]
        public string EventName { get; set; } = string.Empty;

        [StringLength(500)]
        [Column("script")]
        public string? Script { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        // 導覽屬性：一個事件可以有多個管道設定
        public virtual ICollection<SysNoticeChannel> NoticeChannels { get; set; } = new List<SysNoticeChannel>();
    }

    // 4. 通知發送管道設定
    [Table("sys_notice_channel")]
    public class SysNoticeChannel
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Required]
        [StringLength(50)]
        [Column("event_code")]
        public string EventCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("channel_type")]
        public string ChannelType { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("receiver_type")]
        public string ReceiverType { get; set; } = string.Empty;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        // 對應外鍵的父實體導覽屬性
        public virtual SysNoticeEvent? NoticeEvent { get; set; }
    }

    // 5. 通知範本設定
    [Table("sys_notice_setting")]
    public class SysNoticeSetting
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Required]
        [StringLength(50)]
        [Column("notice_type")]
        public string NoticeType { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("channel_type")]
        public string ChannelType { get; set; } = string.Empty;

        [Column("days_before")]
        public int DaysBefore { get; set; } = 0;

        [Required]
        [StringLength(200)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }
    }

    // 6. 第三方服務整合設定
    [Table("sys_integration")]
    public class SysIntegration
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Required]
        [StringLength(50)]
        [Column("service_code")]
        public string ServiceCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("service_name")]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("service_type")]
        public string ServiceType { get; set; } = string.Empty;

        [StringLength(20)]
        [Column("connect_status")]
        public string ConnectStatus { get; set; } = "DISCONNECT";

        [Column("last_sync_date")]
        public DateTime? LastSyncDate { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }
    }

    // 7. 第三方服務參數
    [Table("sys_integration_config")]
    public class SysIntegrationConfig
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Required]
        [StringLength(50)]
        [Column("service_code")]
        public string ServiceCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("config_key")]
        public string ConfigKey { get; set; } = string.Empty;

        [Column("config_value")]
        public string? ConfigValue { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }
    }

    // 8. IP白名單
    [Table("sys_ip_whitelist")]
    public class SysIpWhitelist
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Required]
        [StringLength(100)]
        [Column("ip_address")]
        public string IpAddress { get; set; } = string.Empty;

        [StringLength(100)]
        [Column("ip_name")]
        public string? IpName { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("remark")]
        public string? Remark { get; set; }
    }

    // 9. 系統排程工作
    [Table("sys_job")]
    public class SysJob
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Required]
        [StringLength(50)]
        [Column("job_code")]
        public string JobCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("job_name")]
        public string JobName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("cron_expression")]
        public string CronExpression { get; set; } = string.Empty;

        [Column("last_run_time")]
        public DateTime? LastRunTime { get; set; }

        [Column("next_run_time")]
        public DateTime? NextRunTime { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }
    }

    // 10. 排程執行紀錄
    [Table("sys_job_log")]
    public class SysJobLog
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Required]
        [StringLength(50)]
        [Column("job_code")]
        public string JobCode { get; set; } = string.Empty;

        [Column("execute_time")]
        public DateTime ExecuteTime { get; set; }

        [StringLength(20)]
        [Column("execute_result")]
        public string? ExecuteResult { get; set; }

        [Column("execute_message")]
        public string? ExecuteMessage { get; set; }

        [Column("spend_ms")]
        public long? SpendMs { get; set; }
    }
}