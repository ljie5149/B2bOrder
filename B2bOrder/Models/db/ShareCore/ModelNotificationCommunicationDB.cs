using System;

namespace B2bOrder.Resources.ShareCore.Models
{
    #region 01. 通訊供應商 (Channel Provider)
    public class NcChannelProvider
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ProviderCode { get; set; } = null!;
        public string ProviderName { get; set; } = null!;
        public string ChannelType { get; set; } = null!;
        public string ProviderType { get; set; } = null!;
        public string? EndpointUrl { get; set; }
        public string? CredentialReferenceSid { get; set; }
        public string? SenderIdentity { get; set; }
        public string? CallbackUrl { get; set; }
        public int? RateLimitPerMinute { get; set; }
        public int? DailyLimit { get; set; }
        public string? RetryPolicySid { get; set; }
        public string ProviderStatus { get; set; } = "ACTIVE";
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }
    }
    #endregion

    #region 02. 重試與退避政策 (Retry Policy)
    public class NcRetryPolicy
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string PolicyCode { get; set; } = null!;
        public string PolicyName { get; set; } = null!;
        public int MaxAttempts { get; set; } = 3;
        public int InitialDelaySeconds { get; set; } = 60;
        public string BackoffType { get; set; } = "EXPONENTIAL";
        public decimal BackoffMultiplier { get; set; } = 2m;
        public int MaxDelaySeconds { get; set; } = 3600;
        public string? RetryableErrorCodes { get; set; }
        public string? NonRetryableErrorCodes { get; set; }
        public string PolicyStatus { get; set; } = "ACTIVE";
        public string Avalible { get; set; } = "Y";
    }
    #endregion

    #region 03. 通知與通訊範本 (Template)
    public class NcTemplate
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string TemplateCode { get; set; } = null!;
        public string TemplateName { get; set; } = null!;
        public string? CompanySid { get; set; }
        public string ChannelType { get; set; } = null!;
        public string MessageCategory { get; set; } = null!;
        public string LanguageCode { get; set; } = "zh-TW";
        public string? SubjectTemplate { get; set; }
        public string? TitleTemplate { get; set; }
        public string BodyTemplate { get; set; } = null!;
        public string? HtmlTemplate { get; set; }
        public string? PayloadTemplate { get; set; }
        public string? VariableSchema { get; set; }
        public string? DefaultSenderIdentity { get; set; }
        public string? ExternalTemplateId { get; set; }
        public bool ApprovalRequired { get; set; } = false;
        public int CurrentVersion { get; set; } = 1;
        public string TemplateStatus { get; set; } = "DRAFT";
        public string? WorkflowInstanceSid { get; set; }
        public string Avalible { get; set; } = "Y";
    }
    #endregion

    #region 04. 範本版本歷程 (Template Version)
    public class NcTemplateVersion
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string TemplateSid { get; set; } = null!;
        public int VersionNo { get; set; }
        public string? SubjectTemplate { get; set; }
        public string? TitleTemplate { get; set; }
        public string BodyTemplate { get; set; } = null!;
        public string? HtmlTemplate { get; set; }
        public string? PayloadTemplate { get; set; }
        public string? VariableSchema { get; set; }
        public string? ChangeSummary { get; set; }
        public string? CreatedByUserSid { get; set; }
        public string? ApprovedByUserSid { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string VersionStatus { get; set; } = "DRAFT";
    }
    #endregion

    #region 05. 收件端點 (Recipient Endpoint)
    public class NcRecipientEndpoint
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string SubjectType { get; set; } = null!;
        public string SubjectSid { get; set; } = null!;
        public string ChannelType { get; set; } = null!;
        public string EndpointValue { get; set; } = null!;
        public string? EndpointHash { get; set; }
        public string? EndpointLabel { get; set; }
        public bool PrimaryMark { get; set; } = false;
        public bool VerifiedMark { get; set; } = false;
        public DateTime? VerifiedDate { get; set; }
        public string? VerificationMethod { get; set; }
        public int BounceCount { get; set; } = 0;
        public DateTime? LastSuccessDate { get; set; }
        public DateTime? LastFailureDate { get; set; }
        public string EndpointStatus { get; set; } = "ACTIVE";
    }
    #endregion

    #region 06. 通知偏好與勿擾 (Preference)
    public class NcPreference
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string SubjectType { get; set; } = null!;
        public string SubjectSid { get; set; } = null!;
        public string MessageCategory { get; set; } = null!;
        public string ChannelType { get; set; } = null!;
        public bool EnabledMark { get; set; } = true;
        public TimeSpan? QuietStartTime { get; set; }
        public TimeSpan? QuietEndTime { get; set; }
        public string DigestMode { get; set; } = "IMMEDIATE";
        public TimeSpan? DigestTime { get; set; }
        public string MinimumPriority { get; set; } = "LOW";
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string PreferenceStatus { get; set; } = "ACTIVE";
    }
    #endregion

    #region 07. 通訊與行銷同意 (Consent)
    public class NcConsent
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string SubjectType { get; set; } = null!;
        public string SubjectSid { get; set; } = null!;
        public string ConsentType { get; set; } = null!;
        public string ConsentStatus { get; set; } = null!;
        public DateTime ConsentDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string SourceType { get; set; } = null!;
        public string? SourceReferenceSid { get; set; }
        public string? ConsentTextVersion { get; set; }
        public string? IpAddress { get; set; }
        public string? EvidenceFileSid { get; set; }
    }
    #endregion

    #region 08. 統一通知請求 (Notification Request)
    public class NcNotificationRequest
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string RequestNo { get; set; } = null!;
        public string SourceSystem { get; set; } = null!;
        public string? SourceModule { get; set; }
        public string? SourceEntityType { get; set; }
        public string? SourceEntitySid { get; set; }
        public string? EventCode { get; set; }
        public string? TemplateSid { get; set; }
        public string? TemplateCode { get; set; }
        public string MessageCategory { get; set; } = null!;
        public string Priority { get; set; } = "NORMAL";
        public DateTime? ScheduledDate { get; set; }
        public DateTime? ExpiresDate { get; set; }
        public string? SenderOverride { get; set; }
        public string? RecipientRule { get; set; }
        public string? TemplateVariables { get; set; }
        public string? DeduplicationKey { get; set; }
        public string? CorrelationId { get; set; }
        public string? CausationId { get; set; }
        public string RequestStatus { get; set; } = "RECEIVED";
        public string? ErrorMessage { get; set; }
    }
    #endregion

    #region 09. 通知收件者 (Notification Recipient)
    public class NcNotificationRecipient
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string NotificationRequestSid { get; set; } = null!;
        public string RecipientType { get; set; } = null!;
        public string? SubjectType { get; set; }
        public string? SubjectSid { get; set; }
        public string? EndpointSid { get; set; }
        public string ChannelType { get; set; } = null!;
        public string? RecipientName { get; set; }
        public string EndpointValue { get; set; } = null!;
        public string? LanguageCode { get; set; }
        public string? TimezoneCode { get; set; }
        public string? PersonalizedVariables { get; set; }
        public bool ConsentCheckedMark { get; set; } = false;
        public bool PreferenceCheckedMark { get; set; } = false;
        public string RecipientStatus { get; set; } = "PENDING";
        public string? SuppressionReason { get; set; }
    }
    #endregion

    #region 10. 實際通訊訊息 (Message)
    public class NcMessage
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string NotificationRequestSid { get; set; } = null!;
        public string NotificationRecipientSid { get; set; } = null!;
        public string? ProviderSid { get; set; }
        public string? TemplateSid { get; set; }
        public int? TemplateVersionNo { get; set; }
        public string ChannelType { get; set; } = null!;
        public string? ProviderMessageId { get; set; }
        public string? SenderIdentity { get; set; }
        public string RecipientEndpoint { get; set; } = null!;
        public string? SubjectText { get; set; }
        public string? TitleText { get; set; }
        public string? BodyText { get; set; }
        public string? HtmlContent { get; set; }
        public string? PayloadData { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? QueuedDate { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? OpenedDate { get; set; }
        public DateTime? ClickedDate { get; set; }
        public DateTime? AcknowledgedDate { get; set; }
        public DateTime? ExpiresDate { get; set; }
        public int AttemptCount { get; set; } = 0;
        public string MessageStatus { get; set; } = "CREATED";
        public string? LastErrorCode { get; set; }
        public string? LastErrorMessage { get; set; }
    }
    #endregion

    #region 11. 訊息附件 (Message Attachment)
    public class NcMessageAttachment
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string MessageSid { get; set; } = null!;
        public string AttachmentName { get; set; } = null!;
        public string FileSid { get; set; } = null!;
        public string? MimeType { get; set; }
        public ulong? FileSizeBytes { get; set; }
        public bool InlineMark { get; set; } = false;
        public string? ContentId { get; set; }
    }
    #endregion

    #region 12. 發送工作佇列 (Outbound Queue)
    public class NcOutboundQueue
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string MessageSid { get; set; } = null!;
        public string? ProviderSid { get; set; }
        public int QueuePriority { get; set; } = 100;
        public DateTime AvailableDate { get; set; }
        public string? LockedByWorker { get; set; }
        public DateTime? LockedDate { get; set; }
        public int AttemptCount { get; set; } = 0;
        public int MaxAttempts { get; set; } = 3;
        public DateTime? NextRetryDate { get; set; }
        public string QueueStatus { get; set; } = "READY";
        public string? LastErrorCode { get; set; }
        public string? LastErrorMessage { get; set; }
    }
    #endregion

    #region 13. 發送嘗試歷程 (Delivery Attempt)
    public class NcDeliveryAttempt
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string MessageSid { get; set; } = null!;
        public string? ProviderSid { get; set; }
        public int AttemptNo { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string? RequestPayload { get; set; }
        public string? ResponsePayload { get; set; }
        public int? HttpStatusCode { get; set; }
        public string? ProviderStatusCode { get; set; }
        public string? ProviderMessageId { get; set; }
        public bool SuccessMark { get; set; } = false;
        public bool RetryableMark { get; set; } = false;
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public int? DurationMs { get; set; }
    }
    #endregion

    #region 14. 送達與回呼事件 (Delivery Event)
    public class NcDeliveryEvent
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string? MessageSid { get; set; }
        public string? ProviderSid { get; set; }
        public string? ProviderMessageId { get; set; }
        public string EventType { get; set; } = null!;
        public DateTime EventDate { get; set; }
        public string? EndpointValue { get; set; }
        public string? EventData { get; set; }
        public string? SourceEventId { get; set; }
        public bool ProcessedMark { get; set; } = false;
        public DateTime? ProcessedDate { get; set; }
    }
    #endregion

    #region 15. 退信與抑制清單 (Bounce Suppression)
    public class NcBounceSuppression
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string ChannelType { get; set; } = null!;
        public string EndpointValue { get; set; } = null!;
        public string EndpointHash { get; set; } = null!;
        public string SuppressionType { get; set; } = null!;
        public string? SuppressionReason { get; set; }
        public string? SourceMessageSid { get; set; }
        public string? SourceProviderSid { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string SuppressionStatus { get; set; } = "ACTIVE";
    }
    #endregion

    #region 16. 退訂 Token (Unsubscribe Token)
    public class NcUnsubscribeToken
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string SubjectType { get; set; } = null!;
        public string SubjectSid { get; set; } = null!;
        public string ChannelType { get; set; } = null!;
        public string MessageCategory { get; set; } = null!;
        public string TokenHash { get; set; } = null!;
        public DateTime IssuedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? UsedDate { get; set; }
        public string TokenStatus { get; set; } = "ACTIVE";
    }
    #endregion

    #region 17. Webhook 訂閱 (Webhook Subscription)
    public class NcWebhookSubscription
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string SubscriptionNo { get; set; } = null!;
        public string SubscriberName { get; set; } = null!;
        public string SubscriberSystem { get; set; } = null!;
        public string EndpointUrl { get; set; } = null!;
        public string? SecretReferenceSid { get; set; }
        public string EventPatterns { get; set; } = null!;
        public string? HeaderTemplate { get; set; }
        public string PayloadVersion { get; set; } = "1.0";
        public string? RetryPolicySid { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string SubscriptionStatus { get; set; } = "ACTIVE";
    }
    #endregion

    #region 18. Webhook 投遞與重試 (Webhook Delivery)
    public class NcWebhookDelivery
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string WebhookSubscriptionSid { get; set; } = null!;
        public string? NotificationRequestSid { get; set; }
        public string SourceEventId { get; set; } = null!;
        public string EventCode { get; set; } = null!;
        public string PayloadData { get; set; } = null!;
        public string? SignatureValue { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? ResponseDate { get; set; }
        public int? HttpStatusCode { get; set; }
        public string? ResponseBody { get; set; }
        public int AttemptCount { get; set; } = 0;
        public DateTime? NextRetryDate { get; set; }
        public string DeliveryStatus { get; set; } = "PENDING";
    }
    #endregion

    #region 19. 通知摘要佇列 (Digest Queue)
    public class NcDigestQueue
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string SubjectType { get; set; } = null!;
        public string SubjectSid { get; set; } = null!;
        public string ChannelType { get; set; } = null!;
        public string DigestMode { get; set; } = null!;
        public DateTime DigestPeriodStart { get; set; }
        public DateTime DigestPeriodEnd { get; set; }
        public int NotificationCount { get; set; } = 0;
        public string NotificationRequestSids { get; set; } = null!;
        public DateTime ScheduledSendDate { get; set; }
        public string? MessageSid { get; set; }
        public string DigestStatus { get; set; } = "COLLECTING";
    }
    #endregion

    #region 20. 供應商每日用量 (Provider Usage Daily)
    public class NcProviderUsageDaily
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string ProviderSid { get; set; } = null!;
        public DateTime UsageDate { get; set; }
        public string ChannelType { get; set; } = null!;
        public ulong RequestCount { get; set; } = 0;
        public ulong SuccessCount { get; set; } = 0;
        public ulong FailureCount { get; set; } = 0;
        public ulong DeliveredCount { get; set; } = 0;
        public ulong OpenedCount { get; set; } = 0;
        public ulong ClickedCount { get; set; } = 0;
        public ulong BouncedCount { get; set; } = 0;
        public ulong UnsubscribedCount { get; set; } = 0;
        public decimal UnitCost { get; set; } = 0m;
        public decimal TotalCost { get; set; } = 0m;
        public string? CurrencySid { get; set; }
        public decimal AverageLatencyMs { get; set; } = 0m;
    }
    #endregion

    #region 21. 通知通訊 KPI (KPI Snapshot)
    public class NcKpiSnapshot
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string SnapshotNo { get; set; } = null!;
        public string? CompanySid { get; set; }
        public string? ChannelType { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public ulong RequestCount { get; set; } = 0;
        public ulong MessageCount { get; set; } = 0;
        public ulong SentCount { get; set; } = 0;
        public ulong DeliveredCount { get; set; } = 0;
        public ulong OpenedCount { get; set; } = 0;
        public ulong ClickedCount { get; set; } = 0;
        public ulong AcknowledgedCount { get; set; } = 0;
        public ulong FailedCount { get; set; } = 0;
        public ulong BouncedCount { get; set; } = 0;
        public ulong SuppressedCount { get; set; } = 0;
        public ulong UnsubscribeCount { get; set; } = 0;
        public decimal DeliveryRate { get; set; } = 0m;
        public decimal OpenRate { get; set; } = 0m;
        public decimal ClickRate { get; set; } = 0m;
        public decimal FailureRate { get; set; } = 0m;
        public decimal AverageDeliverySeconds { get; set; } = 0m;
        public ulong RetryCount { get; set; } = 0;
        public ulong DeadLetterCount { get; set; } = 0;
        public decimal TotalCost { get; set; } = 0m;
        public string? CurrencySid { get; set; }
        public string SnapshotStatus { get; set; } = "FINAL";
    }
    #endregion

    #region 22. 狀態異動歷程 (Status History)
    public class NcStatusHistory
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
    #endregion

    #region 23. 領域事件 (Event)
    public class NcEvent
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string EntityType { get; set; } = null!;
        public string EntitySid { get; set; } = null!;
        public string EventCode { get; set; } = null!;
        public int EventVersion { get; set; } = 1;
        public string? EventData { get; set; }
        public string? SourceEventId { get; set; }
        public string? CorrelationId { get; set; }
        public string? CausationId { get; set; }
        public string? OutboxEventSid { get; set; }
        public string ProcessStatus { get; set; } = "PENDING";
        public DateTime? ProcessedDate { get; set; }
        public string? ErrorMessage { get; set; }
    }
    #endregion
}