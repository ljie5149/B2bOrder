using System.ComponentModel.DataAnnotations;

namespace B2bOrder.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "會員編號或電子郵件")]
        [StringLength(100)]
        public string AccountValue { get; set; } = string.Empty;
    }
}