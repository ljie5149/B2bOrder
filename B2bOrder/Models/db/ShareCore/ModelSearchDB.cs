using System;
using System.Collections.Generic;

namespace B2bOrder.Resources.ShareCore.Models
{
    #region 01. 搜尋索引與同步狀態管理 (Index Task & Sync Registry)
    public class SchIndexRegistry
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CompanySid { get; set; } = null!;
        public string IndexName { get; set; } = null!;
        public string EntityType { get; set; } = null!;
        public string EntitySid { get; set; } = null!;
        public string ActionType { get; set; } = "UPSERT";
        public string SyncStatus { get; set; } = "PENDING";
        public int RetryCount { get; set; } = 0;
        public string? ErrorMessage { get; set; }
        public DateTime? LastSyncedAt { get; set; }
        public ulong VersionNo { get; set; } = 0;
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }
    }
    #endregion

    #region 02. 同義詞與搜尋字典 (Synonyms & Dictionaries)
    public class SchSynonymGroup
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CompanySid { get; set; } = null!;
        public string GroupName { get; set; } = null!;
        public string SynonymType { get; set; } = "EQUIVALENT";
        public string SynonymWords { get; set; } = null!;
        public string SynonymStatus { get; set; } = "ACTIVE";
        public ulong VersionNo { get; set; } = 0;
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }
    }

    public class SchCustomDictionary
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CompanySid { get; set; } = null!;
        public string Word { get; set; } = null!;
        public string WordType { get; set; } = "CUSTOM_WORD";
        public int Frequency { get; set; } = 1000;
        public string DictStatus { get; set; } = "ACTIVE";
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }
    }
    #endregion

    #region 03. 搜尋熱詞與建議詞 (Hot Keywords & Auto-Complete)
    public class SchHotKeyword
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CompanySid { get; set; } = null!;
        public string Keyword { get; set; } = null!;
        public string? DisplayTitle { get; set; }
        public int SearchCount { get; set; } = 0;
        public int PriorityScore { get; set; } = 0;
        public string IsPinned { get; set; } = "N";
        public string KeywordStatus { get; set; } = "ACTIVE";
        public ulong VersionNo { get; set; } = 0;
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }
    }
    #endregion

    #region 04. 搜尋軌跡與數據分析 (Search Analytics & Click Tracking)
    public class SchSearchLog
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string CompanySid { get; set; } = null!;
        public string? CustomerSid { get; set; }
        public string? SessionId { get; set; }
        public string SearchKeyword { get; set; } = null!;
        public string? AppliedFiltersJson { get; set; }
        public int ResultCount { get; set; } = 0;
        public string IsZeroResult { get; set; } = "N";
        public string? ClientIp { get; set; }
        public string? UserAgent { get; set; }
        public string? Remark { get; set; }

        public virtual ICollection<SchClickTracking> ClickTrackings { get; set; } = new List<SchClickTracking>();
    }

    public class SchClickTracking
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong SearchLogNid { get; set; }
        public string EntityType { get; set; } = "PRODUCT";
        public string EntitySid { get; set; } = null!;
        public int ClickPosition { get; set; }
        public string ConvertedToCart { get; set; } = "N";
        public string ConvertedToOrder { get; set; } = "N";
        public string? Remark { get; set; }

        public virtual SchSearchLog SearchLog { get; set; } = null!;
    }
    #endregion
}