using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.Db
{
    [Table("sys_user")]
    public class SysUser
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("account")]
        public string Account { get; set; } = string.Empty;

        [Column("pwd")]
        public string Pwd { get; set; } = string.Empty;

        [Column("password_reset_token")]
        public string? PasswordResetToken { get; set; }

        [Column("password_reset_expiry")]
        public DateTime? PasswordResetExpiry { get; set; }

        [Column("member_sid")]
        public string? MemberSid { get; set; }

        [Column("register_verify_key")]
        public string? RegisterVerifyKey { get; set; }

        [Column("last_login_date")]
        public DateTime? LastLoginDate { get; set; }

        // 💡 新增：最後全部標為已讀的時間
        [Column("last_read_all_notices_time")]
        public DateTime? LastReadAllNoticesTime { get; set; }

        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }

        [ForeignKey("MemberSid")]
        public virtual DataMember? Member { get; set; }
    }
}