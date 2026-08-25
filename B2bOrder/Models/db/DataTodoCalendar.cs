using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.Db
{
    [Table("data_todo_calendar")]
    public class DataTodoCalendar
    {
        [Key]
        [Column("nid")]
        public uint Nid { get; set; }

        [Required]
        [Column("sid")]
        [StringLength(32)]
        public string Sid { get; set; } = null!; // 💡 對齊實體欄位：sid

        [Required]
        [Column("member_sid")]
        [StringLength(32)]
        public string MemberSid { get; set; } = null!; // 💡 對齊實體欄位：member_sid

        [Required]
        [Column("title")]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [Column("category")]
        [StringLength(50)]
        public string Category { get; set; } = null!;

        [Required]
        [Column("class_name")]
        [StringLength(50)]
        public string ClassName { get; set; } = null!;

        [Column("start_date")]
        public DateTime? StartDate { get; set; } // 對齊 start_date

        [Column("end_date")]
        public DateTime? EndDate { get; set; } // 對齊 end_date

        [Column("is_all_day")]
        public bool IsAllDay { get; set; } // 對齊 is_all_day

        [Column("is_scheduled")]
        public bool IsScheduled { get; set; } // 對齊 is_scheduled

        [Column("is_completed")]
        public bool IsCompleted { get; set; } // 對齊 is_completed

        [Column("create_date")]
        public DateTime CreateDate { get; set; } // 對齊 create_date

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; } = null!; // 💡 對齊實體欄位：modify_date

        [Column("create_sid")]
        [StringLength(32)]
        public string? CreateSid { get; set; }

        [Required]
        [Column("avalible")]
        [StringLength(2)]
        public string Avalible { get; set; } = "Y"; // 💡 對齊實體欄位：avalible
    }
}