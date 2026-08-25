namespace B2bOrder.Models
{
    public class ExpireNoticePageViewModel
    {
        public List<ExpireNoticeViewModel> Members
        {
            get;
            set;
        } = new();

        public List<PushHistoryViewModel> Logs
        {
            get;
            set;
        } = new();

        public int ExpireSoonCount { get; set; }

        public int ExpiredCount { get; set; }

        public int NotifyCount { get; set; }

        public decimal SuccessRate { get; set; }
        public int TotalCount { get; set; }
    }
}
