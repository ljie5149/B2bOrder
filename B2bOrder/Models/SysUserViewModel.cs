namespace B2bOrder.Models
{
    public class SysUserViewModel
    {
        public string account { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string memberSid { get; set; } = string.Empty;
        public bool isApi { get; set; } = false;
        public string? adminVerify { get; set; } = null;
        public string? parentMid { get; set; } = null;
        public string? name { get; set; } = null;
        public string? email { get; set; } = null;
        public string? mobile { get; set; } = null;
        public string? idNumber { get; set; } = null;
        public string? role { get; set; } = null;
    }
}