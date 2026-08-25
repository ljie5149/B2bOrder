namespace B2bOrder.Models
{
    public class PushHistoryViewModel
    {
        public int Nid { get; set; }

        public string MemberSid { get; set; } = string.Empty;

        public string MemberMid { get; set; } = string.Empty;

        public string NotifyType { get; set; } = string.Empty;

        public string PushType { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Result { get; set; } = string.Empty;

        public string? ResponseMessage { get; set; }

        public DateTime SendDate { get; set; }
    }
}
