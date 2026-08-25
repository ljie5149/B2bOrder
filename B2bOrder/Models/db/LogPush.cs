using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.Db
{
    [Table("log_push")]
    public class LogPush
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Column("member_sid")]
        public string MemberSid { get; set; }
            = string.Empty;

        [Column("member_mid")]
        public string? MemberMid { get; set; }

        [Column("notify_type")]
        public string NotifyType { get; set; }
            = string.Empty;

        [Column("push_type")]
        public string PushType { get; set; }
            = string.Empty;

        [Column("title")]
        public string Title { get; set; }
            = string.Empty;

        [Column("message")]
        public string Message { get; set; }
            = string.Empty;

        [Column("send_date")]
        public DateTime SendDate { get; set; }

        [Column("result")]
        public string? Result { get; set; }

        [Column("response_message")]
        public string? ResponseMessage { get; set; }

        [Column("create_sid")]
        public string? CreateSid { get; set; }

        [ForeignKey(nameof(MemberSid))]
        public virtual DataMember? Member { get; set; }
    }
}