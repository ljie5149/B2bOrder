using System.ComponentModel.DataAnnotations;

namespace B2bOrder.Models
{
    public class MemberCenterViewModel
    {
        public string? Sid { get; set; }

        [Display(Name = "帳號")]
        public string? Mid { get; set; }

        [Display(Name = "姓名")]
        [StringLength(200)]
        public string? Name { get; set; }

        [Display(Name = "電子郵件")]
        [EmailAddress]
        [StringLength(200)]
        public string? Email { get; set; }

        [Display(Name = "行動電話")]
        [Phone]
        [StringLength(50)]
        public string? Mobile { get; set; }

        [Display(Name = "市話")]
        [Phone]
        [StringLength(50)]
        public string? Tel { get; set; }

        [Display(Name = "地址")]
        [StringLength(500)]
        public string? Address { get; set; }

        [Display(Name = "生日")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        [Display(Name = "加入日期")]
        [DataType(DataType.DateTime)]
        public DateTime? JoinDate { get; set; }

        [Display(Name = "續約日期")]
        [DataType(DataType.DateTime)]
        public DateTime? ContinueDate { get; set; }
    }
}