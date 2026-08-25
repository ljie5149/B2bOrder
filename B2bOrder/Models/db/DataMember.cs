using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.Db
{
    [Table("data_member")]
    public class DataMember
    {
        public DataMember()
        {
            Files = new HashSet<DataMemberFile>();
            LogPushes = new HashSet<LogPush>();
        }

        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("register_status")]
        public string? RegisterStatus { get; set; }

        [Column("parent_sid")]
        public string? ParentSid { get; set; }

        [Column("mid")]
        public string Mid { get; set; } = string.Empty;

        [Column("join_date")]
        public DateTime? JoinDate { get; set; }

        [Column("continue_date")]
        public DateTime? ContinueDate { get; set; }

        [Column("real_continue_date")]
        public DateTime? RealContinueDate { get; set; }

        [Column("hint_days")]
        public int HintDays { get; set; } = 30;

        [Column("birthday")]
        public DateTime? Birthday { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("eng_name")]
        public string? EngName { get; set; }

        [Column("head_img")]
        public string? HeadImg { get; set; }

        [Column("iden")]
        public string? Iden { get; set; }

        [Column("cmp_code")]
        public string? CmpCode { get; set; }

        [Column("role")]
        public string Role { get; set; } = string.Empty;

        [Column("authorization_page")]
        public string? AuthorizationPage { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("mobile")]
        public string? Mobile { get; set; }

        [Column("tel")]
        public string? Tel { get; set; }

        [Column("fax")]
        public string? Fax { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("line_user_id")]
        public string? LineUserId { get; set; }

        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("signature_pic")]
        public string? SignaturePic { get; set; }

        [Column("advertising_id")]
        public string? AdvertisingId { get; set; }

        [Column("device_id")]
        public string? DeviceId { get; set; }

        [Column("fcm_token")]
        public string? FcmToken { get; set; }

        [Column("priority")]
        public int? Priority { get; set; }

        [Column("level_name")]
        public string? LevelName { get; set; }

        [Column("notice_enable")]
        public string? NoticeEnable { get; set; }

        [Column("last_login_date")]
        public DateTime? LastLoginDate { get; set; }

        [Column("register_source")]
        public string? RegisterSource { get; set; }

        [Column("edit_sid")]
        public string? EditSid { get; set; }

        [Column("cur_coupon")]
        public int CurCoupon { get; set; }

        [Column("cur_point")]
        public int CurPoint { get; set; }

        [Column("script")]
        public string? Script { get; set; }

        [Column("remark")]
        public string? Remark { get; set; }

        /// <summary>
        /// 重設密碼 Token（不儲存於 data_member 表）
        /// </summary>
        [NotMapped]
        public string? PasswordResetToken { get; set; }

        [InverseProperty("Member")]
        public virtual ICollection<DataMemberFile> Files { get; set; }

        [InverseProperty("Member")]
        public virtual ICollection<LogPush> LogPushes { get; set; }
    }
}