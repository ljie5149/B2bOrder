using System.ComponentModel.DataAnnotations;

namespace B2bOrder.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "請輸入新密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "請再次輸入新密碼")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "兩次輸入的密碼不一致")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;
    }
}