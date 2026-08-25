namespace B2bOrder.Models
{
    public class LogActionViewModel
    {
        public int Nid { get; set; }

        public DateTime CreateDate { get; set; }

        public string? UserAccount { get; set; }

        public string? UserName { get; set; }

        public string? ModuleName { get; set; }

        public string? ActionName { get; set; }

        public string ActionType { get; set; } = "";

        public string? TargetSid { get; set; }

        public string? TargetMid { get; set; }

        public string? Ip { get; set; }

        public string Result { get; set; } = "";

        public string? PageName { get; set; }

        public string? Description { get; set; }

        public string? MoreDescription { get; set; }

        public string? UserAgent { get; set; }
    }
}
