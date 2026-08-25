using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.Db
{
    [Table("data_notice_read")]
    public class DataNoticeRead
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("notice_sid")]
        public string NoticeSid { get; set; } = string.Empty;

        [Column("member_sid")]
        public string MemberSid { get; set; } = string.Empty;

        [Column("read_time")]
        public DateTime ReadTime { get; set; } = DateTime.Now;

        // 💡 選擇性：若有需要 EF Core 導覽屬性可取消註解
        // [ForeignKey("NoticeSid")]
        // public virtual DataNotice? Notice { get; set; }

        // [ForeignKey("MemberSid")]
        // public virtual DataMember? Member { get; set; }
    }
}