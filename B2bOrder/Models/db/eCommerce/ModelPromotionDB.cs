namespace B2bOrder.Resources.eCommerce
{
    // =========================================================
    // Models / 實體模型定義
    // =========================================================

    #region 01. 促銷活動主檔 (Campaign Master & Translation)

    /// <summary>
    /// 促銷活動主檔 (Campaign Master)
    /// </summary>
    public class CampaignModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public DateTime? ModifyDate { get; set; }
       
        public string CampaignNo { get; set; } = null!;
        public string CampaignCode { get; set; } = null!;
        public string CampaignName { get; set; } = null!;
        public string CampaignType { get; set; } = null!;
        public string? CompanySid { get; set; }
       
        public string? BusinessUnitSid { get; set; }
       
        public string CurrencySid { get; set; } = null!;
        public int Priority { get; set; }
       
        public string StackMode { get; set; } = null!;
        public bool StopProcessing { get; set; }
       
        public int? UsageLimitTotal { get; set; }
       
        public int? UsageLimitPerParty { get; set; }
       
        public int UsageLimitPerOrder { get; set; }
       
        public decimal? BudgetAmount { get; set; }
       
        public decimal ConsumedBudgetAmount { get; set; }
       
        public DateTime StartDate { get; set; }
       
        public DateTime EndDate { get; set; }
       
        public string CampaignStatus { get; set; } = null!;
        public string? WorkflowInstanceSid { get; set; }
       
        public string? ApprovedUserSid { get; set; }
       
        public DateTime? ApprovedDate { get; set; }
       
        public ulong VersionNo { get; set; }
       
        public string Avalible { get; set; } = null!;
        public string? Remark { get; set; }
       

        public ICollection<CampaignTranslationModel> Translations { get; set; } = new List<CampaignTranslationModel>();
        public ICollection<RuleModel> Rules { get; set; } = new List<RuleModel>();
        public ICollection<CampaignScopeModel> Scopes { get; set; } = new List<CampaignScopeModel>();
        public ICollection<TimeScheduleModel> TimeSchedules { get; set; } = new List<TimeScheduleModel>();
    }

    /// <summary>
    /// 促銷活動多語系內容 (Campaign Translation)
    /// </summary>
    public class CampaignTranslationModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public DateTime? ModifyDate { get; set; }
       
        public ulong CampaignNid { get; set; }
       
        public string LanguageSid { get; set; } = null!;
        public string CampaignName { get; set; } = null!;
        public string? ShortDescription { get; set; }
       
        public string? FullDescription { get; set; }
       
        public string? TermsText { get; set; }
       
        public string? BannerText { get; set; }
       

        public CampaignModel Campaign { get; set; } = null!;
    }

    #endregion

    #region 02. 促銷規則與條件 (Rules, Conditions & Rewards)

    /// <summary>
    /// 促銷活動規則 (Rule)
    /// </summary>
    public class RuleModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public DateTime? ModifyDate { get; set; }
       
        public ulong CampaignNid { get; set; }
       
        public int RuleNo { get; set; }
       
        public string RuleName { get; set; } = null!;
        public string ConditionLogic { get; set; } = null!;
        public string CalculationBasis { get; set; } = null!;
        public bool RepeatableMark { get; set; }
       
        public int? MaximumRepeatCount { get; set; }
       
        public string RuleStatus { get; set; } = null!;

        public CampaignModel Campaign { get; set; } = null!;
        public ICollection<ConditionModel> Conditions { get; set; } = new List<ConditionModel>();
        public ICollection<RewardModel> Rewards { get; set; } = new List<RewardModel>();
    }

    /// <summary>
    /// 促銷條件 (Condition)
    /// </summary>
    public class ConditionModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public ulong RuleNid { get; set; }
       
        public int ConditionNo { get; set; }
       
        public string ConditionType { get; set; } = null!;
        public string OperatorCode { get; set; } = null!;
        public string ConditionValue { get; set; } = null!; // JSON format string
        public bool IncludeMark { get; set; }
       

        public RuleModel Rule { get; set; } = null!;
    }

    /// <summary>
    /// 促銷折扣、贈品與回饋 (Reward)
    /// </summary>
    public class RewardModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public DateTime? ModifyDate { get; set; }
       
        public ulong RuleNid { get; set; }
       
        public int RewardNo { get; set; }
       
        public string RewardType { get; set; } = null!;
        public decimal? RewardValue { get; set; }
       
        public decimal? MaximumDiscountAmount { get; set; }
       
        public decimal? MinimumPayAmount { get; set; }
       
        public string TargetScope { get; set; } = null!;
        public string? TargetConfig { get; set; } // JSON format string
        public string? GiftItemSid { get; set; }
       
        public string? GiftVariantSid { get; set; }
       
        public decimal? GiftQty { get; set; }
       
        public string? AddOnItemSid { get; set; }
       
        public string? AddOnVariantSid { get; set; }
       
        public decimal? AddOnPrice { get; set; }
       
        public string? PointTypeSid { get; set; }
       
        public decimal? PointAmount { get; set; }
       
        public string RewardStatus { get; set; } = null!;

        public RuleModel Rule { get; set; } = null!;
    }

    #endregion

    #region 03. 活動適用與排除範圍 (Scopes & Time Schedules)

    /// <summary>
    /// 促銷活動適用與排除範圍 (Campaign Scope)
    /// </summary>
    public class CampaignScopeModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public ulong CampaignNid { get; set; }
       
        public string ScopeType { get; set; } = null!;
        public string? ScopeSid { get; set; }
       
        public bool IncludeMark { get; set; }
       
        public int Priority { get; set; }
       

        public CampaignModel Campaign { get; set; } = null!;
    }

    /// <summary>
    /// 活動星期、日期與每日時段 (Time Schedule)
    /// </summary>
    public class TimeScheduleModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public ulong CampaignNid { get; set; }
       
        public string ScheduleType { get; set; } = null!;
        public string? WeekdayMask { get; set; }
       
        public DateOnly? SpecificDate { get; set; }
       
        public TimeOnly? StartTime { get; set; }
       
        public TimeOnly? EndTime { get; set; }
       
        public string TimezoneCode { get; set; } = null!;
        public string ScheduleStatus { get; set; } = null!;

        public CampaignModel Campaign { get; set; } = null!;
    }

    #endregion

    #region 04. 優惠券批次與優惠碼 (Coupon Batches, Coupons & Distribution)

    /// <summary>
    /// 優惠券發行批次 (Coupon Batch)
    /// </summary>
    public class CouponBatchModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public DateTime? ModifyDate { get; set; }
       
        public string BatchNo { get; set; } = null!;
        public string BatchName { get; set; } = null!;
        public string CampaignSid { get; set; } = null!;
        public string CouponType { get; set; } = null!;
        public string? CodePrefix { get; set; }
       
        public int? TotalQuantity { get; set; }
       
        public int IssuedQuantity { get; set; }
       
        public int RedeemedQuantity { get; set; }
       
        public string ValidityType { get; set; } = null!;
        public int? ValidDays { get; set; }
       
        public DateTime? StartDate { get; set; }
       
        public DateTime? EndDate { get; set; }
       
        public string BatchStatus { get; set; } = null!;
        public string Avalible { get; set; } = null!;
        public string? Remark { get; set; }
       

        public ICollection<CouponModel> Coupons { get; set; } = new List<CouponModel>();
    }

    /// <summary>
    /// 優惠券與優惠碼實體 (Coupon)
    /// </summary>
    public class CouponModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public DateTime? ModifyDate { get; set; }
       
        public string CouponBatchSid { get; set; } = null!;
        public string CouponCode { get; set; } = null!;
        public string? CouponCodeHash { get; set; }
       
        public string? OwnerPartySid { get; set; }
       
        public DateTime? IssuedDate { get; set; }
       
        public DateTime? ReceivedDate { get; set; }
       
        public DateTime? ValidFrom { get; set; }
       
        public DateTime? ValidUntil { get; set; }
       
        public int UsageLimit { get; set; }
       
        public int UsedCount { get; set; }
       
        public string CouponStatus { get; set; } = null!;
        public string? ReservedCheckoutSid { get; set; }
       
        public DateTime? ReservedUntil { get; set; }
       

        public CouponBatchModel CouponBatch { get; set; } = null!;
        public ICollection<CouponDistributionModel> Distributions { get; set; } = new List<CouponDistributionModel>();
    }

    /// <summary>
    /// 優惠券發放與領取紀錄 (Coupon Distribution)
    /// </summary>
    public class CouponDistributionModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public string CouponSid { get; set; } = null!;
        public string PartySid { get; set; } = null!;
        public string DistributionType { get; set; } = null!;
        public string? SourceSid { get; set; }
       
        public string DistributionStatus { get; set; } = null!;
        public string? DistributedUserSid { get; set; }
       
        public string? ErrorMessage { get; set; }
       

        public CouponModel Coupon { get; set; } = null!;
    }

    #endregion

    #region 05. 優惠計算請求與結果 (Calculation Requests & Results)

    /// <summary>
    /// 促銷優惠計算請求 (Calculation Request)
    /// </summary>
    public class CalculationRequestModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public string RequestNo { get; set; } = null!;
        public string RequestSource { get; set; } = null!;
        public string? ReferenceSid { get; set; }
       
        public string? PartySid { get; set; }
       
        public string? ChannelSid { get; set; }
       
        public string? StoreSid { get; set; }
       
        public string? ProjectSid { get; set; }
       
        public string CurrencySid { get; set; } = null!;
        public string? CouponCodes { get; set; } // JSON format string
        public string RequestData { get; set; } = null!; // JSON format string
        public string CalculationStatus { get; set; } = null!;
        public DateTime? CompletedDate { get; set; }
       
        public string? CorrelationId { get; set; }
       
        public string? ErrorMessage { get; set; }
       

        public ICollection<CalculationResultModel> CalculationResults { get; set; } = new List<CalculationResultModel>();
    }

    /// <summary>
    /// 促銷優惠計算結果 (Calculation Result)
    /// </summary>
    public class CalculationResultModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public ulong CalculationRequestNid { get; set; }
       
        public string CampaignSid { get; set; } = null!;
        public string RuleSid { get; set; } = null!;
        public string? CouponSid { get; set; }
       
        public string RewardType { get; set; } = null!;
        public string TargetType { get; set; } = null!;
        public string? TargetSid { get; set; }
       
        public decimal DiscountAmount { get; set; }
       
        public string? GiftItemSid { get; set; }
       
        public string? GiftVariantSid { get; set; }
       
        public decimal? GiftQty { get; set; }
       
        public decimal? PointAmount { get; set; }
       
        public string? CalculationTrace { get; set; } // JSON format string
        public string ResultStatus { get; set; } = null!;

        public CalculationRequestModel CalculationRequest { get; set; } = null!;
    }

    #endregion

    #region 06. 優惠預留、核銷與撤銷 (Reservations, Redemptions & Reversals)

    /// <summary>
    /// 結帳期間優惠與預算預留 (Redemption Reservation)
    /// </summary>
    public class RedemptionReservationModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public string CampaignSid { get; set; } = null!;
        public string? CouponSid { get; set; }
       
        public string? PartySid { get; set; }
       
        public string? CheckoutSid { get; set; }
       
        public string? OrderSid { get; set; }
       
        public decimal ReservedDiscountAmount { get; set; }
       
        public decimal ReservedBudgetAmount { get; set; }
       
        public DateTime ReservedDate { get; set; }
       
        public DateTime ExpiryDate { get; set; }
       
        public string ReservationStatus { get; set; } = null!;
    }

    /// <summary>
    /// 優惠正式核銷 (Redemption)
    /// </summary>
    public class RedemptionModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public string RedemptionNo { get; set; } = null!;
        public string CampaignSid { get; set; } = null!;
        public string? RuleSid { get; set; }
       
        public string? CouponSid { get; set; }
       
        public string? PartySid { get; set; }
       
        public string SalesOrderSid { get; set; } = null!;
        public string? SalesOrderItemSid { get; set; }
       
        public DateTime RedemptionDate { get; set; }
       
        public decimal DiscountAmount { get; set; }
       
        public decimal BudgetConsumedAmount { get; set; }
       
        public string CurrencySid { get; set; } = null!;
        public string RedemptionStatus { get; set; } = null!;
        public string? CalculationResultSid { get; set; }
       
        public string? CorrelationId { get; set; }
       

        public ICollection<RedemptionReversalModel> Reversals { get; set; } = new List<RedemptionReversalModel>();
    }

    /// <summary>
    /// 訂單取消、退貨與退款優惠撤銷 (Redemption Reversal)
    /// </summary>
    public class RedemptionReversalModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public string RedemptionSid { get; set; } = null!;
        public string ReversalType { get; set; } = null!;
        public string? ReferenceSid { get; set; }
       
        public decimal ReversalAmount { get; set; }
       
        public decimal BudgetReturnAmount { get; set; }
       
        public bool CouponRestoreMark { get; set; }
       
        public string? ReversalReason { get; set; }
       
        public string? OperatorUserSid { get; set; }
       
        public string ReversalStatus { get; set; } = null!;
        public string? CorrelationId { get; set; }
       

        public RedemptionModel Redemption { get; set; } = null!;
    }

    #endregion

    #region 07. 贈品與加價購庫存 (Reward Inventory)

    /// <summary>
    /// 活動贈品與加價購配額 (Reward Inventory)
    /// </summary>
    public class RewardInventoryModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public DateTime? ModifyDate { get; set; }
       
        public string CampaignSid { get; set; } = null!;
        public string RewardSid { get; set; } = null!;
        public string ItemSid { get; set; } = null!;
        public string? VariantSid { get; set; }
       
        public string? WarehouseSid { get; set; }
       
        public decimal? QuotaQty { get; set; }
       
        public decimal ReservedQty { get; set; }
       
        public decimal RedeemedQty { get; set; }
       
        public decimal? RemainingQty { get; set; }
       
        public string InventoryStatus { get; set; } = null!;
    }

    #endregion

    #region 08. 活動預算與成本 (Budget Ledger)

    /// <summary>
    /// 促銷活動預算流水帳 (Budget Ledger)
    /// </summary>
    public class BudgetLedgerModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public string CampaignSid { get; set; } = null!;
        public string TransactionType { get; set; } = null!;
        public string? ReferenceType { get; set; }
       
        public string? ReferenceSid { get; set; }
       
        public decimal Amount { get; set; }
       
        public decimal BalanceAfter { get; set; }
       
        public string CurrencySid { get; set; } = null!;
        public string? OperatorUserSid { get; set; }
       
        public string? CorrelationId { get; set; }
       
    }

    #endregion

    #region 09. 促銷模板與複製 (Templates)

    /// <summary>
    /// 促銷活動模板 (Template)
    /// </summary>
    public class TemplateModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public DateTime? ModifyDate { get; set; }
       
        public string TemplateCode { get; set; } = null!;
        public string TemplateName { get; set; } = null!;
        public string CampaignType { get; set; } = null!;
        public string TemplateConfig { get; set; } = null!; // JSON format string
        public string TemplateStatus { get; set; } = null!;
        public string Avalible { get; set; } = null!;
        public string? Remark { get; set; }
       
    }

    #endregion

    #region 10. 活動版本、狀態與事件 (Versions, Status Histories & Events)

    /// <summary>
    /// 促銷活動版本歷程 (Campaign Version)
    /// </summary>
    public class CampaignVersionModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public string CampaignSid { get; set; } = null!;
        public ulong VersionNo { get; set; }
       
        public string SnapshotData { get; set; } = null!; // JSON format string
        public string ChangeType { get; set; } = null!;
        public string? ChangeSummary { get; set; }
       
        public string? CreateUserSid { get; set; }
       
        public string VersionStatus { get; set; } = null!;
    }

    /// <summary>
    /// 促銷狀態歷程 (Status History)
    /// </summary>
    public class StatusHistoryModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public string EntityType { get; set; } = null!;
        public string EntitySid { get; set; } = null!;
        public string? OldStatus { get; set; }
       
        public string NewStatus { get; set; } = null!;
        public string? EventCode { get; set; }
       
        public string? OperatorUserSid { get; set; }
       
        public string? Reason { get; set; }
       
        public string? CorrelationId { get; set; }
       
    }

    /// <summary>
    /// 促銷領域事件 (Event)
    /// </summary>
    public class EventModel
    {
        public ulong Nid { get; set; }
       
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       
        public string EntityType { get; set; } = null!;
        public string EntitySid { get; set; } = null!;
        public string EventCode { get; set; } = null!;
        public int EventVersion { get; set; }
       
        public string? EventData { get; set; } // JSON format string
        public string? SourceEventId { get; set; }
       
        public string? CorrelationId { get; set; }
       
        public string? CausationId { get; set; }
       
        public string? OutboxEventSid { get; set; }
       
        public string ProcessStatus { get; set; } = null!;
        public DateTime? ProcessedDate { get; set; }
       
        public string? ErrorMessage { get; set; }
       
    }

    #endregion
}