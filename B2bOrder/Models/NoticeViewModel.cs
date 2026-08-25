using B2bOrder.Models.Db; // 確保有引用您的資料庫 Entity（如 SysModule, DataNotice）

namespace B2bOrder.Models
{
    public class NoticeViewModel
    {
        // =========================================================================
        // 💡 核心補強：公告列表渲染必備集合（對齊 NoticeController 與 List.cshtml）
        // =========================================================================
        /// <summary>
        /// 系統公告資料列表
        /// </summary>
        public List<DataNotice> DataNoties { get; set; } = new List<DataNotice>();

        // === 原有的基本資訊設定 ===
        public string SystemName { get; set; } = string.Empty;
        public string SystemTimezone { get; set; } = string.Empty;
        public string SystemLanguage { get; set; } = string.Empty;
        public string DateFormat { get; set; } = string.Empty;
        public string TimeFormat { get; set; } = string.Empty;
        public int PageSize { get; set; }

        // === 原有的登入與存取設定 ===
        public bool Enable2FA { get; set; }
        public int MinPasswordLength { get; set; }
        public int PasswordExpiryDays { get; set; }
        public int SessionTimeout { get; set; }
        public int LoginFailLockCount { get; set; }
        public int AccountLockDuration { get; set; }
        public bool AllowMultiDevice { get; set; }
        public bool EnableIpWhitelist { get; set; }

        // === 原有的公司資訊 ===
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyEmail { get; set; } = string.Empty;
        public string CompanyPhone { get; set; } = string.Empty;
        public string CompanyAddress { get; set; } = string.Empty;

        // === 其他 Tab 渲染時所需的集合屬性 ===
        public List<SysModule> Modules { get; set; } = new List<SysModule>();
        public List<NoticeEventViewModel> NoticeEvents { get; set; } = new List<NoticeEventViewModel>();
        public List<IntegrationServiceViewModel> Integrations { get; set; } = new List<IntegrationServiceViewModel>();
    }

    // === 附屬的 DTO / ViewModel ===

    public class NoticeEventViewModel
    {
        public string EventName { get; set; } = string.Empty;
        public string Script { get; set; } = string.Empty;
    }

    public class IntegrationServiceViewModel
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public string ConnectStatus { get; set; } = string.Empty;
        public DateTime? LastSyncDate { get; set; }
    }

    public class ModuleConfigViewModel
    {
        // 從資料庫查出的完整功能模組清單
        public List<SysModule> Modules { get; set; } = new List<SysModule>();

        // 用於右側統計數據欄位
        public int TotalCount => Modules.Count;
        public int EnabledCount => Modules.Count(m => m.Avalible == "Y");
        public int DisabledCount => Modules.Count(m => m.Avalible == "N");
    }
}