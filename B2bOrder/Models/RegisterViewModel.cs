using B2bOrder.Models.Common;
using B2bOrder.Models.Db;
using System.ComponentModel.DataAnnotations;

namespace B2bOrder.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "請輸入會員編號")]
        [Display(Name = "會員編號")]
        public string Mid { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "請再次輸入密碼")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "兩次輸入的密碼不一致")]
        [Display(Name = "確認密碼")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "請選擇會員角色")]
        public string Role { get; set; } = MemberRoles.Regular;

        [Required(ErrorMessage = "請填寫加入日期")]
        [Display(Name = "加入日期")]
        [DataType(DataType.Date)]
        public DateTime? JoinDate { get; set; }

        [Required(ErrorMessage = "請輸入姓名")]
        [Display(Name = "姓名")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入手機號碼")]
        [Phone(ErrorMessage = "請輸入正確的手機號碼")]
        [Display(Name = "手機")]
        public string Mobile { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "請輸入正確的電子信箱")]
        [Display(Name = "電子信箱")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "上級 MID")]
        public string? ParentMid { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "請同意隱私權政策與使用條款")]
        public bool AgreeTerms { get; set; } = false;

        public string? AdminVerifyCode { get; set; } // 新增此屬性以修正 CS0117
        public string? IdNumber { get; set; } // 若有用到 IdNumber 也一併補上

        [DataType(DataType.Date)]
        [Display(Name = "續約日期")]
        public DateTime? ContinueDate { get; set; }

        public bool isApi { get; set; } = false;
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "帳號")]
        public string Mid { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "記住我")]
        public bool RememberMe { get; set; } = false;

        public bool isApi { get; set; } = false;
    }
    public class CreateMemberViewModel
    {
        [Required(ErrorMessage = "請輸入會員編號")]
        public string Mid { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入姓名")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "請選擇會員角色")]
        public string Role { get; set; } = MemberRoles.Regular;

        [Required(ErrorMessage = "請填寫加入日期")]
        [DataType(DataType.Date)]
        public DateTime? JoinDate { get; set; }

        [Required(ErrorMessage = "請填寫續約日期")]
        [DataType(DataType.Date)]
        public DateTime? ContinueDate { get; set; }

        public DateTime? RealContinueDate { get; set; }

        //[Phone(ErrorMessage = "請輸入正確的手機號碼")]
        [Display(Name = "手機")]
        public string Mobile { get; set; } = string.Empty;

        //[Required(ErrorMessage = "請輸入Email")]
        //[EmailAddress]
        [Display(Name = "電子信箱")]
        public string Email { get; set; } = string.Empty;

        public string? IdNumber { get; set; }

        // 顯示用
        public string ParentMid { get; set; } = string.Empty;

        // 寫入用
        public string ParentSid { get; set; } = string.Empty;
    }
    public class EditMemberViewModel
    {
        public string Sid { get; set; } = string.Empty;

        //[Required(ErrorMessage = "請輸入會員編號")]
        public string Mid { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入姓名")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "請選擇會員角色")]
        public string Role { get; set; } = MemberRoles.Regular;

        [Required(ErrorMessage = "請填寫加入日期")]
        [DataType(DataType.Date)]
        public DateTime? JoinDate { get; set; }

        [Required(ErrorMessage = "請填寫續約日期")]
        [DataType(DataType.Date)]
        public DateTime? ContinueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? RealContinueDate { get; set; }

        //[Required(ErrorMessage = "請輸入手機")]
        [Display(Name = "手機")]
        public string Mobile { get; set; } = string.Empty;

        //[Required(ErrorMessage = "請輸入Email")]
        //[EmailAddress]
        [Display(Name = "電子信箱")]
        public string Email { get; set; } = string.Empty;

        public string? IdNumber { get; set; }

        public string ParentMid { get; set; } = string.Empty;
    }
    public class ViewMemberViewModel
    {
        public string Sid { get; set; } = string.Empty;

        //[Required(ErrorMessage = "請輸入會員編號")]
        public string Mid { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入姓名")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "請選擇會員角色")]
        public string Role { get; set; } = MemberRoles.Regular;

        [Required(ErrorMessage = "請填寫加入日期")]
        [DataType(DataType.Date)]
        public DateTime? JoinDate { get; set; }

        [Required(ErrorMessage = "請填寫續約日期")]
        [DataType(DataType.Date)]
        public DateTime?ContinueDate { get; set; }

        //[Required(ErrorMessage = "請輸入手機")]
        [Display(Name = "手機")]
        public string Mobile { get; set; } = string.Empty;

        //[Required(ErrorMessage = "請輸入Email")]
        //[EmailAddress]
        [Display(Name = "電子信箱")]
        public string Email { get; set; } = string.Empty;

        public string? IdNumber { get; set; }

        public string ParentMid { get; set; } = string.Empty;
    }
    public class MemberListViewModel
    {
        public List<DataMember> Members { get; set; }
            = new();

        public EditMemberViewModel EditMember { get; set; }
            = new();
    }
    public class MemberModalViewModel
    {
        public CreateMemberViewModel CreateMember { get; set; }
            = new();

        public EditMemberViewModel EditMember { get; set; }
            = new();

        public ViewMemberViewModel ViewMember { get; set; }
            = new();
    }
}