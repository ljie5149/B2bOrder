using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.Db
{
    [Table("log_action")]
    public class LogAction
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        /// <summary>
        /// 操作時間
        /// </summary>
        [Required]
        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 操作帳號序號
        /// </summary>
        [Column("user_sid")]
        [MaxLength(32)]
        public string? UserSid { get; set; }

        /// <summary>
        /// 操作帳號
        /// </summary>
        [Column("user_account")]
        [MaxLength(50)]
        public string? UserAccount { get; set; }

        /// <summary>
        /// 操作人員
        /// </summary>
        [Column("user_name")]
        [MaxLength(50)]
        public string? UserName { get; set; }

        /// <summary>
        /// 模組名稱
        /// </summary>
        [Column("module_name")]
        [MaxLength(50)]
        public string? ModuleName { get; set; }

        /// <summary>
        /// 操作名稱
        /// </summary>
        [Column("action_name")]
        [MaxLength(50)]
        public string? ActionName { get; set; }

        /// <summary>
        /// 操作類型
        /// </summary>
        [Required]
        [Column("action_type")]
        [MaxLength(50)]
        public string ActionType { get; set; } = string.Empty;

        /// <summary>
        /// 目標對象序號
        /// </summary>
        [Column("target_sid")]
        [MaxLength(32)]
        public string? TargetSid { get; set; }

        /// <summary>
        /// 操作帳號
        /// </summary>
        [Column("target_mid")]
        [MaxLength(50)]
        public string? TargetMid { get; set; }

        /// <summary>
        /// IP 位址
        /// </summary>
        [Column("ip")]
        [MaxLength(50)]
        public string? Ip { get; set; }

        /// <summary>
        /// 執行結果：成功／失敗
        /// </summary>
        [Required]
        [Column("result")]
        [MaxLength(10)]
        public string Result { get; set; } = "成功";

        /// <summary>
        /// 操作頁面
        /// </summary>
        [Column("page_name")]
        [MaxLength(100)]
        public string? PageName { get; set; }

        /// <summary>
        /// 瀏覽器資訊
        /// </summary>
        [Column("user_agent")]
        [MaxLength(255)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// 操作描述
        /// </summary>
        [Column("description")]
        public string? Description { get; set; }

        /// <summary>
        /// 更多資訊（JSON、例外訊息、異動前後資料等）
        /// </summary>
        [Column("more_description")]
        public string? MoreDescription { get; set; }
    }
}