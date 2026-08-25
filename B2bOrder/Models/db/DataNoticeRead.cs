using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.Db
{
    // 11. 系統公告
    [Table("data_notice")]
    public class DataNotice
    {
        [Key]
        [Column("nid")]
        public uint Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Required]
        [Column("publish_date")]
        public DateTime PublishDate { get; set; }

        [Required]
        [StringLength(255)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Column("is_top")]
        public sbyte IsTop { get; set; } = 0;

        [Required]
        [StringLength(10)]
        [Column("status")]
        public string Status { get; set; } = "啟用";

        [Column("start_time")]
        public DateTime? StartTime { get; set; }

        [Column("end_time")]
        public DateTime? EndTime { get; set; }

        // --- 新增欄位開始 ---

        [StringLength(50)]
        [Column("publish_unit")]
        public string? PublishUnit { get; set; } = "系統管理部";

        [StringLength(20)]
        [Column("category")]
        public string? Category { get; set; } = "一般公告";

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("click_count")]
        public uint ClickCount { get; set; } = 0;

        // --- 新增欄位結束 ---

        [StringLength(32)]
        [Column("create_user_sid")]
        public string? CreateUserSid { get; set; }

        [Required]
        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [StringLength(32)]
        [Column("modify_user_sid")]
        public string? ModifyUserSid { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        // 💡 擴充屬性：不映射至 DB 欄位，僅供 Service 運算已讀狀態並回傳前端
        [NotMapped]
        public bool IsRead { get; set; } = false;
    }
}