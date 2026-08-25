namespace B2bOrder.Models
{
    public class ExpireNoticeViewModel
    {
        public string Sid { get; set; } = "";

        public string Mid { get; set; } = "";

        public string Name { get; set; } = "";

        public string Mobile { get; set; } = "";

        public string Email { get; set; } = "";

        public string FcmToken { get; set; } = "";

        public string Role { get; set; } = "";

        public DateTime? ContinueDate { get; set; }

        public int RemainDays { get; set; }

        public int HintDays { get; set; }

        public string Status { get; set; } = "";

        public DateTime? LastNotifyDate { get; set; }
        public string ParentMid { get; set; } = "";
        public string ParentName { get; set; } = "";
    }
}
