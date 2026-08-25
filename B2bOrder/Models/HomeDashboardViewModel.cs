namespace B2bOrder.Models
{
    public class HomeDashboardViewModel
    {
        // 新增：供儀表板顯示的最新公告列表 (例如取前 1 到 3 筆)
        public List<ViewAnnouncementModel> LatestNotices { get; set; } = new List<ViewAnnouncementModel>();
        public string MemberName { get; set; } = "";
        public bool ShowSalesAndBonus { get; set; } = false;

        // Top summary
        public int TotalMembers { get; set; }
        public int DirectMembers { get; set; }
        public int RegularMembers { get; set; }

        // Latest members table
        public IEnumerable<LatestMemberDto> LatestMembers { get; set; } = Array.Empty<LatestMemberDto>();

        // Member growth for charts (label + count)
        public IEnumerable<MemberGrowthPoint> MemberGrowth { get; set; } = Array.Empty<MemberGrowthPoint>();

        // Sales / bonus (when available)
        public decimal SalesTotal { get; set; }
        public decimal BonusTotal { get; set; }

        public class LatestMemberDto
        {
            public string Name { get; set; } = string.Empty;
            public string Mid { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public DateTime? JoinDate { get; set; }
        }

        public class MemberGrowthPoint
        {
            public string Label { get; set; } = string.Empty;
            public int Count { get; set; }
        }

        // 用來儲存第 1 到第 5 層（含以上）的人數
        public int Level1Count { get; set; }
        public int Level2Count { get; set; }
        public int Level3Count { get; set; }
        public int Level4Count { get; set; }
        public int Level5PlusCount { get; set; }

        /// <summary>
        /// 即將到期會員數量
        /// </summary>
        public int ExpireSoonCount { get; set; }

        /// <summary>
        /// 已到期（逾期）會員數量
        /// </summary>
        public int ExpiredCount { get; set; }
    }
}