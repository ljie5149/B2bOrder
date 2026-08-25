namespace B2bOrder.Models
{
    public class TodoDto
    {
        public string Id { get; set; } = null!;        // 對應 data_todo_calendar.sid

        public string Title { get; set; } = null!;     // 事項名稱

        public string Category { get; set; } = null!;  // work, life, personal

        public string ClassName { get; set; } = null!; // bg-work-event, bg-life-event 等

        // 💡 優化：由 string 改為 DateTime?，省去手動自訂格式轉換的功夫
        public DateTime? Start { get; set; }           // 行事曆開始時間

        public DateTime? End { get; set; }             // 行事曆結束時間

        public bool AllDay { get; set; }               // 是否全天 (對應 is_all_day)

        public bool IsDone { get; set; }               // 是否已完成 (對應 is_completed)

        // 💡 擴充：建議補上此欄位，方便前端判斷該事項是在「待辦側欄」還是「行事曆主體」
        public bool IsScheduled { get; set; }          // 是否已排程 (對應 is_scheduled)
    }
}