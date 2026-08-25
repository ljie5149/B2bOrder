using System.ComponentModel.DataAnnotations;

namespace B2bOrder.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "請輸入舊密碼。")]
        [DataType(DataType.Password)]
        [Display(Name = "舊密碼")]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "請輸入新密碼。")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼至少要 {2} 個字元。")]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "請再次輸入新密碼。")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "兩次新密碼輸入不一致。")]
        [Display(Name = "確認新密碼")]
        public string? ConfirmPassword { get; set; }
    }
}