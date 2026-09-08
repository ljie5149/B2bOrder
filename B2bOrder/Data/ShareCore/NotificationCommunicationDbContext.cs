using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.ShareCore.Models;

namespace B2bOrder.Resources.ShareCore
{
    public class NotificationCommunicationDbContext : DbContext
    {
        public NotificationCommunicationDbContext(DbContextOptions<NotificationCommunicationDbContext> options) : base(options) { }

        #region DbSets
        public DbSet<NcChannelProvider> NcChannelProviders { get; set; } = null!;
        public DbSet<NcRetryPolicy> NcRetryPolicies { get; set; } = null!;
        public DbSet<NcTemplate> NcTemplates { get; set; } = null!;
        public DbSet<NcTemplateVersion> NcTemplateVersions { get; set; } = null!;
        public DbSet<NcRecipientEndpoint> NcRecipientEndpoints { get; set; } = null!;
        public DbSet<NcPreference> NcPreferences { get; set; } = null!;
        public DbSet<NcConsent> NcConsents { get; set; } = null!;
        public DbSet<NcNotificationRequest> NcNotificationRequests { get; set; } = null!;
        public DbSet<NcNotificationRecipient> NcNotificationRecipients { get; set; } = null!;
        public DbSet<NcMessage> NcMessages { get; set; } = null!;
        public DbSet<NcMessageAttachment> NcMessageAttachments { get; set; } = null!;
        public DbSet<NcOutboundQueue> NcOutboundQueues { get; set; } = null!;
        public DbSet<NcDeliveryAttempt> NcDeliveryAttempts { get; set; } = null!;
        public DbSet<NcDeliveryEvent> NcDeliveryEvents { get; set; } = null!;
        public DbSet<NcBounceSuppression> NcBounceSuppressions { get; set; } = null!;
        public DbSet<NcUnsubscribeToken> NcUnsubscribeTokens { get; set; } = null!;
        public DbSet<NcWebhookSubscription> NcWebhookSubscriptions { get; set; } = null!;
        public DbSet<NcWebhookDelivery> NcWebhookDeliveries { get; set; } = null!;
        public DbSet<NcDigestQueue> NcDigestQueues { get; set; } = null!;
        public DbSet<NcProviderUsageDaily> NcProviderUsageDailies { get; set; } = null!;
        public DbSet<NcKpiSnapshot> NcKpiSnapshots { get; set; } = null!;
        public DbSet<NcStatusHistory> NcStatusHistories { get; set; } = null!;
        public DbSet<NcEvent> NcEvents { get; set; } = null!;
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region 01. 通訊供應商 (Channel Provider)
            modelBuilder.Entity<NcChannelProvider>(entity =>
            {
                entity.ToTable("nc_channel_provider");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ProviderCode).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.ProviderCode).HasColumnName("provider_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ProviderName).HasColumnName("provider_name").HasMaxLength(300).IsRequired();
                entity.Property(e => e.ChannelType).HasColumnName("channel_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.ProviderType).HasColumnName("provider_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.EndpointUrl).HasColumnName("endpoint_url").HasMaxLength(1000);
                entity.Property(e => e.CredentialReferenceSid).HasColumnName("credential_reference_sid").HasMaxLength(32);
                entity.Property(e => e.SenderIdentity).HasColumnName("sender_identity").HasMaxLength(300);
                entity.Property(e => e.CallbackUrl).HasColumnName("callback_url").HasMaxLength(1000);
                entity.Property(e => e.RateLimitPerMinute).HasColumnName("rate_limit_per_minute");
                entity.Property(e => e.DailyLimit).HasColumnName("daily_limit");
                entity.Property(e => e.RetryPolicySid).HasColumnName("retry_policy_sid").HasMaxLength(32);
                entity.Property(e => e.ProviderStatus).HasColumnName("provider_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });
            #endregion

            #region 02. 重試與退避政策 (Retry Policy)
            modelBuilder.Entity<NcRetryPolicy>(entity =>
            {
                entity.ToTable("nc_retry_policy");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.PolicyCode).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.PolicyCode).HasColumnName("policy_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.PolicyName).HasColumnName("policy_name").HasMaxLength(300).IsRequired();
                entity.Property(e => e.MaxAttempts).HasColumnName("max_attempts").HasDefaultValue(3);
                entity.Property(e => e.InitialDelaySeconds).HasColumnName("initial_delay_seconds").HasDefaultValue(60);
                entity.Property(e => e.BackoffType).HasColumnName("backoff_type").HasMaxLength(20).HasDefaultValue("EXPONENTIAL");
                entity.Property(e => e.BackoffMultiplier).HasColumnName("backoff_multiplier").HasPrecision(10, 4).HasDefaultValue(2m);
                entity.Property(e => e.MaxDelaySeconds).HasColumnName("max_delay_seconds").HasDefaultValue(3600);
                entity.Property(e => e.RetryableErrorCodes).HasColumnName("retryable_error_codes").HasColumnType("json");
                entity.Property(e => e.NonRetryableErrorCodes).HasColumnName("non_retryable_error_codes").HasColumnType("json");
                entity.Property(e => e.PolicyStatus).HasColumnName("policy_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
            });
            #endregion

            #region 03. 通知與通訊範本 (Template)
            modelBuilder.Entity<NcTemplate>(entity =>
            {
                entity.ToTable("nc_template");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.TemplateCode).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.TemplateCode).HasColumnName("template_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.TemplateName).HasColumnName("template_name").HasMaxLength(300).IsRequired();
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32);
                entity.Property(e => e.ChannelType).HasColumnName("channel_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.MessageCategory).HasColumnName("message_category").HasMaxLength(50).IsRequired();
                entity.Property(e => e.LanguageCode).HasColumnName("language_code").HasMaxLength(20).HasDefaultValue("zh-TW");
                entity.Property(e => e.SubjectTemplate).HasColumnName("subject_template").HasMaxLength(500);
                entity.Property(e => e.TitleTemplate).HasColumnName("title_template").HasMaxLength(500);
                entity.Property(e => e.BodyTemplate).HasColumnName("body_template").IsRequired();
                entity.Property(e => e.HtmlTemplate).HasColumnName("html_template");
                entity.Property(e => e.PayloadTemplate).HasColumnName("payload_template").HasColumnType("json");
                entity.Property(e => e.VariableSchema).HasColumnName("variable_schema").HasColumnType("json");
                entity.Property(e => e.DefaultSenderIdentity).HasColumnName("default_sender_identity").HasMaxLength(300);
                entity.Property(e => e.ExternalTemplateId).HasColumnName("external_template_id").HasMaxLength(300);
                entity.Property(e => e.ApprovalRequired).HasColumnName("approval_required").HasDefaultValue(false);
                entity.Property(e => e.CurrentVersion).HasColumnName("current_version").HasDefaultValue(1);
                entity.Property(e => e.TemplateStatus).HasColumnName("template_status").HasMaxLength(20).HasDefaultValue("DRAFT");
                entity.Property(e => e.WorkflowInstanceSid).HasColumnName("workflow_instance_sid").HasMaxLength(32);
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
            });
            #endregion

            #region 04. 範本版本歷程 (Template Version)
            modelBuilder.Entity<NcTemplateVersion>(entity =>
            {
                entity.ToTable("nc_template_version");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.TemplateSid, e.VersionNo }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.TemplateSid).HasColumnName("template_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.VersionNo).HasColumnName("version_no").IsRequired();
                entity.Property(e => e.SubjectTemplate).HasColumnName("subject_template").HasMaxLength(500);
                entity.Property(e => e.TitleTemplate).HasColumnName("title_template").HasMaxLength(500);
                entity.Property(e => e.BodyTemplate).HasColumnName("body_template").IsRequired();
                entity.Property(e => e.HtmlTemplate).HasColumnName("html_template");
                entity.Property(e => e.PayloadTemplate).HasColumnName("payload_template").HasColumnType("json");
                entity.Property(e => e.VariableSchema).HasColumnName("variable_schema").HasColumnType("json");
                entity.Property(e => e.ChangeSummary).HasColumnName("change_summary").HasMaxLength(1000);
                entity.Property(e => e.CreatedByUserSid).HasColumnName("created_by_user_sid").HasMaxLength(32);
                entity.Property(e => e.ApprovedByUserSid).HasColumnName("approved_by_user_sid").HasMaxLength(32);
                entity.Property(e => e.ApprovedDate).HasColumnName("approved_date");
                entity.Property(e => e.VersionStatus).HasColumnName("version_status").HasMaxLength(20).HasDefaultValue("DRAFT");
            });
            #endregion

            #region 05. 收件端點 (Recipient Endpoint)
            modelBuilder.Entity<NcRecipientEndpoint>(entity =>
            {
                entity.ToTable("nc_recipient_endpoint");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.SubjectType, e.SubjectSid, e.ChannelType, e.EndpointValue }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.SubjectType).HasColumnName("subject_type").HasMaxLength(20).IsRequired();
                entity.Property(e => e.SubjectSid).HasColumnName("subject_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ChannelType).HasColumnName("channel_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.EndpointValue).HasColumnName("endpoint_value").HasMaxLength(1000).IsRequired();
                entity.Property(e => e.EndpointHash).HasColumnName("endpoint_hash").HasMaxLength(255);
                entity.Property(e => e.EndpointLabel).HasColumnName("endpoint_label").HasMaxLength(200);
                entity.Property(e => e.PrimaryMark).HasColumnName("primary_mark").HasDefaultValue(false);
                entity.Property(e => e.VerifiedMark).HasColumnName("verified_mark").HasDefaultValue(false);
                entity.Property(e => e.VerifiedDate).HasColumnName("verified_date");
                entity.Property(e => e.VerificationMethod).HasColumnName("verification_method").HasMaxLength(30);
                entity.Property(e => e.BounceCount).HasColumnName("bounce_count").HasDefaultValue(0);
                entity.Property(e => e.LastSuccessDate).HasColumnName("last_success_date");
                entity.Property(e => e.LastFailureDate).HasColumnName("last_failure_date");
                entity.Property(e => e.EndpointStatus).HasColumnName("endpoint_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
            });
            #endregion

            #region 06. 通知偏好與勿擾 (Preference)
            modelBuilder.Entity<NcPreference>(entity =>
            {
                entity.ToTable("nc_preference");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.SubjectType, e.SubjectSid, e.MessageCategory, e.ChannelType }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.SubjectType).HasColumnName("subject_type").HasMaxLength(20).IsRequired();
                entity.Property(e => e.SubjectSid).HasColumnName("subject_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.MessageCategory).HasColumnName("message_category").HasMaxLength(50).IsRequired();
                entity.Property(e => e.ChannelType).HasColumnName("channel_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.EnabledMark).HasColumnName("enabled_mark").HasDefaultValue(true);
                entity.Property(e => e.QuietStartTime).HasColumnName("quiet_start_time");
                entity.Property(e => e.QuietEndTime).HasColumnName("quiet_end_time");
                entity.Property(e => e.DigestMode).HasColumnName("digest_mode").HasMaxLength(20).HasDefaultValue("IMMEDIATE");
                entity.Property(e => e.DigestTime).HasColumnName("digest_time");
                entity.Property(e => e.MinimumPriority).HasColumnName("minimum_priority").HasMaxLength(20).HasDefaultValue("LOW");
                entity.Property(e => e.EffectiveDate).HasColumnName("effective_date").HasDefaultValueSql("(CURRENT_DATE)");
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(e => e.PreferenceStatus).HasColumnName("preference_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
            });
            #endregion

            #region 07. 通訊與行銷同意 (Consent)
            modelBuilder.Entity<NcConsent>(entity =>
            {
                entity.ToTable("nc_consent");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.SubjectType, e.SubjectSid, e.ConsentType, e.ConsentDate }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.SubjectType).HasColumnName("subject_type").HasMaxLength(20).IsRequired();
                entity.Property(e => e.SubjectSid).HasColumnName("subject_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ConsentType).HasColumnName("consent_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.ConsentStatus).HasColumnName("consent_status").HasMaxLength(20).IsRequired();
                entity.Property(e => e.ConsentDate).HasColumnName("consent_date").IsRequired();
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(e => e.SourceType).HasColumnName("source_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.SourceReferenceSid).HasColumnName("source_reference_sid").HasMaxLength(32);
                entity.Property(e => e.ConsentTextVersion).HasColumnName("consent_text_version").HasMaxLength(50);
                entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(50);
                entity.Property(e => e.EvidenceFileSid).HasColumnName("evidence_file_sid").HasMaxLength(32);
            });
            #endregion

            #region 08. 統一通知請求 (Notification Request)
            modelBuilder.Entity<NcNotificationRequest>(entity =>
            {
                entity.ToTable("nc_notification_request");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.RequestNo).IsUnique();
                entity.HasIndex(e => e.DeduplicationKey).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.RequestNo).HasColumnName("request_no").HasMaxLength(100).IsRequired();
                entity.Property(e => e.SourceSystem).HasColumnName("source_system").HasMaxLength(100).IsRequired();
                entity.Property(e => e.SourceModule).HasColumnName("source_module").HasMaxLength(100);
                entity.Property(e => e.SourceEntityType).HasColumnName("source_entity_type").HasMaxLength(50);
                entity.Property(e => e.SourceEntitySid).HasColumnName("source_entity_sid").HasMaxLength(32);
                entity.Property(e => e.EventCode).HasColumnName("event_code").HasMaxLength(120);
                entity.Property(e => e.TemplateSid).HasColumnName("template_sid").HasMaxLength(32);
                entity.Property(e => e.TemplateCode).HasColumnName("template_code").HasMaxLength(100);
                entity.Property(e => e.MessageCategory).HasColumnName("message_category").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Priority).HasColumnName("priority").HasMaxLength(20).HasDefaultValue("NORMAL");
                entity.Property(e => e.ScheduledDate).HasColumnName("scheduled_date");
                entity.Property(e => e.ExpiresDate).HasColumnName("expires_date");
                entity.Property(e => e.SenderOverride).HasColumnName("sender_override").HasColumnType("json");
                entity.Property(e => e.RecipientRule).HasColumnName("recipient_rule").HasColumnType("json");
                entity.Property(e => e.TemplateVariables).HasColumnName("template_variables").HasColumnType("json");
                entity.Property(e => e.DeduplicationKey).HasColumnName("deduplication_key").HasMaxLength(255);
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasMaxLength(100);
                entity.Property(e => e.CausationId).HasColumnName("causation_id").HasMaxLength(100);
                entity.Property(e => e.RequestStatus).HasColumnName("request_status").HasMaxLength(30).HasDefaultValue("RECEIVED");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
            });
            #endregion

            #region 09. 通知收件者 (Notification Recipient)
            modelBuilder.Entity<NcNotificationRecipient>(entity =>
            {
                entity.ToTable("nc_notification_recipient");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.NotificationRequestSid).HasColumnName("notification_request_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.RecipientType).HasColumnName("recipient_type").HasMaxLength(20).IsRequired();
                entity.Property(e => e.SubjectType).HasColumnName("subject_type").HasMaxLength(20);
                entity.Property(e => e.SubjectSid).HasColumnName("subject_sid").HasMaxLength(32);
                entity.Property(e => e.EndpointSid).HasColumnName("endpoint_sid").HasMaxLength(32);
                entity.Property(e => e.ChannelType).HasColumnName("channel_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.RecipientName).HasColumnName("recipient_name").HasMaxLength(300);
                entity.Property(e => e.EndpointValue).HasColumnName("endpoint_value").HasMaxLength(1000).IsRequired();
                entity.Property(e => e.LanguageCode).HasColumnName("language_code").HasMaxLength(20);
                entity.Property(e => e.TimezoneCode).HasColumnName("timezone_code").HasMaxLength(50);
                entity.Property(e => e.PersonalizedVariables).HasColumnName("personalized_variables").HasColumnType("json");
                entity.Property(e => e.ConsentCheckedMark).HasColumnName("consent_checked_mark").HasDefaultValue(false);
                entity.Property(e => e.PreferenceCheckedMark).HasColumnName("preference_checked_mark").HasDefaultValue(false);
                entity.Property(e => e.RecipientStatus).HasColumnName("recipient_status").HasMaxLength(20).HasDefaultValue("PENDING");
                entity.Property(e => e.SuppressionReason).HasColumnName("suppression_reason").HasMaxLength(1000);
            });
            #endregion

            #region 10. 實際通訊訊息 (Message)
            modelBuilder.Entity<NcMessage>(entity =>
            {
                entity.ToTable("nc_message");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.NotificationRequestSid, e.NotificationRecipientSid, e.ChannelType }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.NotificationRequestSid).HasColumnName("notification_request_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.NotificationRecipientSid).HasColumnName("notification_recipient_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ProviderSid).HasColumnName("provider_sid").HasMaxLength(32);
                entity.Property(e => e.TemplateSid).HasColumnName("template_sid").HasMaxLength(32);
                entity.Property(e => e.TemplateVersionNo).HasColumnName("template_version_no");
                entity.Property(e => e.ChannelType).HasColumnName("channel_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.ProviderMessageId).HasColumnName("provider_message_id").HasMaxLength(300);
                entity.Property(e => e.SenderIdentity).HasColumnName("sender_identity").HasMaxLength(300);
                entity.Property(e => e.RecipientEndpoint).HasColumnName("recipient_endpoint").HasMaxLength(1000).IsRequired();
                entity.Property(e => e.SubjectText).HasColumnName("subject_text").HasMaxLength(500);
                entity.Property(e => e.TitleText).HasColumnName("title_text").HasMaxLength(500);
                entity.Property(e => e.BodyText).HasColumnName("body_text");
                entity.Property(e => e.HtmlContent).HasColumnName("html_content");
                entity.Property(e => e.PayloadData).HasColumnName("payload_data").HasColumnType("json");
                entity.Property(e => e.ScheduledDate).HasColumnName("scheduled_date");
                entity.Property(e => e.QueuedDate).HasColumnName("queued_date");
                entity.Property(e => e.SentDate).HasColumnName("sent_date");
                entity.Property(e => e.DeliveredDate).HasColumnName("delivered_date");
                entity.Property(e => e.OpenedDate).HasColumnName("opened_date");
                entity.Property(e => e.ClickedDate).HasColumnName("clicked_date");
                entity.Property(e => e.AcknowledgedDate).HasColumnName("acknowledged_date");
                entity.Property(e => e.ExpiresDate).HasColumnName("expires_date");
                entity.Property(e => e.AttemptCount).HasColumnName("attempt_count").HasDefaultValue(0);
                entity.Property(e => e.MessageStatus).HasColumnName("message_status").HasMaxLength(30).HasDefaultValue("CREATED");
                entity.Property(e => e.LastErrorCode).HasColumnName("last_error_code").HasMaxLength(100);
                entity.Property(e => e.LastErrorMessage).HasColumnName("last_error_message");
            });
            #endregion

            #region 11. 訊息附件 (Message Attachment)
            modelBuilder.Entity<NcMessageAttachment>(entity =>
            {
                entity.ToTable("nc_message_attachment");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.MessageSid).HasColumnName("message_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.AttachmentName).HasColumnName("attachment_name").HasMaxLength(500).IsRequired();
                entity.Property(e => e.FileSid).HasColumnName("file_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.MimeType).HasColumnName("mime_type").HasMaxLength(200);
                entity.Property(e => e.FileSizeBytes).HasColumnName("file_size_bytes");
                entity.Property(e => e.InlineMark).HasColumnName("inline_mark").HasDefaultValue(false);
                entity.Property(e => e.ContentId).HasColumnName("content_id").HasMaxLength(200);
            });
            #endregion

            #region 12. 發送工作佇列 (Outbound Queue)
            modelBuilder.Entity<NcOutboundQueue>(entity =>
            {
                entity.ToTable("nc_outbound_queue");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.MessageSid).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.MessageSid).HasColumnName("message_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ProviderSid).HasColumnName("provider_sid").HasMaxLength(32);
                entity.Property(e => e.QueuePriority).HasColumnName("queue_priority").HasDefaultValue(100);
                entity.Property(e => e.AvailableDate).HasColumnName("available_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.LockedByWorker).HasColumnName("locked_by_worker").HasMaxLength(200);
                entity.Property(e => e.LockedDate).HasColumnName("locked_date");
                entity.Property(e => e.AttemptCount).HasColumnName("attempt_count").HasDefaultValue(0);
                entity.Property(e => e.MaxAttempts).HasColumnName("max_attempts").HasDefaultValue(3);
                entity.Property(e => e.NextRetryDate).HasColumnName("next_retry_date");
                entity.Property(e => e.QueueStatus).HasColumnName("queue_status").HasMaxLength(20).HasDefaultValue("READY");
                entity.Property(e => e.LastErrorCode).HasColumnName("last_error_code").HasMaxLength(100);
                entity.Property(e => e.LastErrorMessage).HasColumnName("last_error_message");
            });
            #endregion

            #region 13. 發送嘗試歷程 (Delivery Attempt)
            modelBuilder.Entity<NcDeliveryAttempt>(entity =>
            {
                entity.ToTable("nc_delivery_attempt");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.MessageSid, e.AttemptNo }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.MessageSid).HasColumnName("message_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ProviderSid).HasColumnName("provider_sid").HasMaxLength(32);
                entity.Property(e => e.AttemptNo).HasColumnName("attempt_no").IsRequired();
                entity.Property(e => e.RequestDate).HasColumnName("request_date").IsRequired();
                entity.Property(e => e.ResponseDate).HasColumnName("response_date");
                entity.Property(e => e.RequestPayload).HasColumnName("request_payload").HasColumnType("json");
                entity.Property(e => e.ResponsePayload).HasColumnName("response_payload").HasColumnType("json");
                entity.Property(e => e.HttpStatusCode).HasColumnName("http_status_code");
                entity.Property(e => e.ProviderStatusCode).HasColumnName("provider_status_code").HasMaxLength(100);
                entity.Property(e => e.ProviderMessageId).HasColumnName("provider_message_id").HasMaxLength(300);
                entity.Property(e => e.SuccessMark).HasColumnName("success_mark").HasDefaultValue(false);
                entity.Property(e => e.RetryableMark).HasColumnName("retryable_mark").HasDefaultValue(false);
                entity.Property(e => e.ErrorCode).HasColumnName("error_code").HasMaxLength(100);
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
                entity.Property(e => e.DurationMs).HasColumnName("duration_ms");
            });
            #endregion

            #region 14. 送達與回呼事件 (Delivery Event)
            modelBuilder.Entity<NcDeliveryEvent>(entity =>
            {
                entity.ToTable("nc_delivery_event");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SourceEventId).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.MessageSid).HasColumnName("message_sid").HasMaxLength(32);
                entity.Property(e => e.ProviderSid).HasColumnName("provider_sid").HasMaxLength(32);
                entity.Property(e => e.ProviderMessageId).HasColumnName("provider_message_id").HasMaxLength(300);
                entity.Property(e => e.EventType).HasColumnName("event_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.EventDate).HasColumnName("event_date").IsRequired();
                entity.Property(e => e.EndpointValue).HasColumnName("endpoint_value").HasMaxLength(1000);
                entity.Property(e => e.EventData).HasColumnName("event_data").HasColumnType("json");
                entity.Property(e => e.SourceEventId).HasColumnName("source_event_id").HasMaxLength(300);
                entity.Property(e => e.ProcessedMark).HasColumnName("processed_mark").HasDefaultValue(false);
                entity.Property(e => e.ProcessedDate).HasColumnName("processed_date");
            });
            #endregion

            #region 15. 退信與抑制清單 (Bounce Suppression)
            modelBuilder.Entity<NcBounceSuppression>(entity =>
            {
                entity.ToTable("nc_bounce_suppression");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.ChannelType, e.EndpointHash }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ChannelType).HasColumnName("channel_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.EndpointValue).HasColumnName("endpoint_value").HasMaxLength(1000).IsRequired();
                entity.Property(e => e.EndpointHash).HasColumnName("endpoint_hash").HasMaxLength(255).IsRequired();
                entity.Property(e => e.SuppressionType).HasColumnName("suppression_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.SuppressionReason).HasColumnName("suppression_reason").HasMaxLength(1000);
                entity.Property(e => e.SourceMessageSid).HasColumnName("source_message_sid").HasMaxLength(32);
                entity.Property(e => e.SourceProviderSid).HasColumnName("source_provider_sid").HasMaxLength(32);
                entity.Property(e => e.EffectiveDate).HasColumnName("effective_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(e => e.SuppressionStatus).HasColumnName("suppression_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
            });
            #endregion

            #region 16. 退訂 Token (Unsubscribe Token)
            modelBuilder.Entity<NcUnsubscribeToken>(entity =>
            {
                entity.ToTable("nc_unsubscribe_token");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.TokenHash).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.SubjectType).HasColumnName("subject_type").HasMaxLength(20).IsRequired();
                entity.Property(e => e.SubjectSid).HasColumnName("subject_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ChannelType).HasColumnName("channel_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.MessageCategory).HasColumnName("message_category").HasMaxLength(50).IsRequired();
                entity.Property(e => e.TokenHash).HasColumnName("token_hash").HasMaxLength(255).IsRequired();
                entity.Property(e => e.IssuedDate).HasColumnName("issued_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(e => e.UsedDate).HasColumnName("used_date");
                entity.Property(e => e.TokenStatus).HasColumnName("token_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
            });
            #endregion

            #region 17. Webhook 訂閱 (Webhook Subscription)
            modelBuilder.Entity<NcWebhookSubscription>(entity =>
            {
                entity.ToTable("nc_webhook_subscription");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SubscriptionNo).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.SubscriptionNo).HasColumnName("subscription_no").HasMaxLength(100).IsRequired();
                entity.Property(e => e.SubscriberName).HasColumnName("subscriber_name").HasMaxLength(300).IsRequired();
                entity.Property(e => e.SubscriberSystem).HasColumnName("subscriber_system").HasMaxLength(100).IsRequired();
                entity.Property(e => e.EndpointUrl).HasColumnName("endpoint_url").HasMaxLength(1000).IsRequired();
                entity.Property(e => e.SecretReferenceSid).HasColumnName("secret_reference_sid").HasMaxLength(32);
                entity.Property(e => e.EventPatterns).HasColumnName("event_patterns").HasColumnType("json").IsRequired();
                entity.Property(e => e.HeaderTemplate).HasColumnName("header_template").HasColumnType("json");
                entity.Property(e => e.PayloadVersion).HasColumnName("payload_version").HasMaxLength(50).HasDefaultValue("1.0");
                entity.Property(e => e.RetryPolicySid).HasColumnName("retry_policy_sid").HasMaxLength(32);
                entity.Property(e => e.TimeoutSeconds).HasColumnName("timeout_seconds").HasDefaultValue(30);
                entity.Property(e => e.EffectiveDate).HasColumnName("effective_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(e => e.SubscriptionStatus).HasColumnName("subscription_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
            });
            #endregion

            #region 18. Webhook 投遞與重試 (Webhook Delivery)
            modelBuilder.Entity<NcWebhookDelivery>(entity =>
            {
                entity.ToTable("nc_webhook_delivery");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.WebhookSubscriptionSid, e.SourceEventId }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.WebhookSubscriptionSid).HasColumnName("webhook_subscription_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.NotificationRequestSid).HasColumnName("notification_request_sid").HasMaxLength(32);
                entity.Property(e => e.SourceEventId).HasColumnName("source_event_id").HasMaxLength(100).IsRequired();
                entity.Property(e => e.EventCode).HasColumnName("event_code").HasMaxLength(120).IsRequired();
                entity.Property(e => e.PayloadData).HasColumnName("payload_data").HasColumnType("json").IsRequired();
                entity.Property(e => e.SignatureValue).HasColumnName("signature_value").HasMaxLength(500);
                entity.Property(e => e.ScheduledDate).HasColumnName("scheduled_date");
                entity.Property(e => e.SentDate).HasColumnName("sent_date");
                entity.Property(e => e.ResponseDate).HasColumnName("response_date");
                entity.Property(e => e.HttpStatusCode).HasColumnName("http_status_code");
                entity.Property(e => e.ResponseBody).HasColumnName("response_body");
                entity.Property(e => e.AttemptCount).HasColumnName("attempt_count").HasDefaultValue(0);
                entity.Property(e => e.NextRetryDate).HasColumnName("next_retry_date");
                entity.Property(e => e.DeliveryStatus).HasColumnName("delivery_status").HasMaxLength(20).HasDefaultValue("PENDING");
            });
            #endregion

            #region 19. 通知摘要佇列 (Digest Queue)
            modelBuilder.Entity<NcDigestQueue>(entity =>
            {
                entity.ToTable("nc_digest_queue");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.SubjectType, e.SubjectSid, e.ChannelType, e.DigestMode, e.DigestPeriodStart, e.DigestPeriodEnd }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.SubjectType).HasColumnName("subject_type").HasMaxLength(20).IsRequired();
                entity.Property(e => e.SubjectSid).HasColumnName("subject_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ChannelType).HasColumnName("channel_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.DigestMode).HasColumnName("digest_mode").HasMaxLength(20).IsRequired();
                entity.Property(e => e.DigestPeriodStart).HasColumnName("digest_period_start").IsRequired();
                entity.Property(e => e.DigestPeriodEnd).HasColumnName("digest_period_end").IsRequired();
                entity.Property(e => e.NotificationCount).HasColumnName("notification_count").HasDefaultValue(0);
                entity.Property(e => e.NotificationRequestSids).HasColumnName("notification_request_sids").HasColumnType("json").IsRequired();
                entity.Property(e => e.ScheduledSendDate).HasColumnName("scheduled_send_date").IsRequired();
                entity.Property(e => e.MessageSid).HasColumnName("message_sid").HasMaxLength(32);
                entity.Property(e => e.DigestStatus).HasColumnName("digest_status").HasMaxLength(20).HasDefaultValue("COLLECTING");
            });
            #endregion

            #region 20. 供應商每日用量 (Provider Usage Daily)
            modelBuilder.Entity<NcProviderUsageDaily>(entity =>
            {
                entity.ToTable("nc_provider_usage_daily");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.ProviderSid, e.UsageDate, e.ChannelType }).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ProviderSid).HasColumnName("provider_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.UsageDate).HasColumnName("usage_date").HasColumnType("date").IsRequired();
                entity.Property(e => e.ChannelType).HasColumnName("channel_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.RequestCount).HasColumnName("request_count").HasDefaultValue(0UL);
                entity.Property(e => e.SuccessCount).HasColumnName("success_count").HasDefaultValue(0UL);
                entity.Property(e => e.FailureCount).HasColumnName("failure_count").HasDefaultValue(0UL);
                entity.Property(e => e.DeliveredCount).HasColumnName("delivered_count").HasDefaultValue(0UL);
                entity.Property(e => e.OpenedCount).HasColumnName("opened_count").HasDefaultValue(0UL);
                entity.Property(e => e.ClickedCount).HasColumnName("clicked_count").HasDefaultValue(0UL);
                entity.Property(e => e.BouncedCount).HasColumnName("bounced_count").HasDefaultValue(0UL);
                entity.Property(e => e.UnsubscribedCount).HasColumnName("unsubscribed_count").HasDefaultValue(0UL);
                entity.Property(e => e.UnitCost).HasColumnName("unit_cost").HasPrecision(20, 8).HasDefaultValue(0m);
                entity.Property(e => e.TotalCost).HasColumnName("total_cost").HasPrecision(20, 4).HasDefaultValue(0m);
                entity.Property(e => e.CurrencySid).HasColumnName("currency_sid").HasMaxLength(32);
                entity.Property(e => e.AverageLatencyMs).HasColumnName("average_latency_ms").HasPrecision(20, 4).HasDefaultValue(0m);
            });
            #endregion

            #region 21. 通知通訊 KPI (KPI Snapshot)
            modelBuilder.Entity<NcKpiSnapshot>(entity =>
            {
                entity.ToTable("nc_kpi_snapshot");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SnapshotNo).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.SnapshotNo).HasColumnName("snapshot_no").HasMaxLength(100).IsRequired();
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32);
                entity.Property(e => e.ChannelType).HasColumnName("channel_type").HasMaxLength(30);
                entity.Property(e => e.PeriodStartDate).HasColumnName("period_start_date").HasColumnType("date").IsRequired();
                entity.Property(e => e.PeriodEndDate).HasColumnName("period_end_date").HasColumnType("date").IsRequired();
                entity.Property(e => e.RequestCount).HasColumnName("request_count").HasDefaultValue(0UL);
                entity.Property(e => e.MessageCount).HasColumnName("message_count").HasDefaultValue(0UL);
                entity.Property(e => e.SentCount).HasColumnName("sent_count").HasDefaultValue(0UL);
                entity.Property(e => e.DeliveredCount).HasColumnName("delivered_count").HasDefaultValue(0UL);
                entity.Property(e => e.OpenedCount).HasColumnName("opened_count").HasDefaultValue(0UL);
                entity.Property(e => e.ClickedCount).HasColumnName("clicked_count").HasDefaultValue(0UL);
                entity.Property(e => e.AcknowledgedCount).HasColumnName("acknowledged_count").HasDefaultValue(0UL);
                entity.Property(e => e.FailedCount).HasColumnName("failed_count").HasDefaultValue(0UL);
                entity.Property(e => e.BouncedCount).HasColumnName("bounced_count").HasDefaultValue(0UL);
                entity.Property(e => e.SuppressedCount).HasColumnName("suppressed_count").HasDefaultValue(0UL);
                entity.Property(e => e.UnsubscribeCount).HasColumnName("unsubscribe_count").HasDefaultValue(0UL);
                entity.Property(e => e.DeliveryRate).HasColumnName("delivery_rate").HasPrecision(8, 4).HasDefaultValue(0m);
                entity.Property(e => e.OpenRate).HasColumnName("open_rate").HasPrecision(8, 4).HasDefaultValue(0m);
                entity.Property(e => e.ClickRate).HasColumnName("click_rate").HasPrecision(8, 4).HasDefaultValue(0m);
                entity.Property(e => e.FailureRate).HasColumnName("failure_rate").HasPrecision(8, 4).HasDefaultValue(0m);
                entity.Property(e => e.AverageDeliverySeconds).HasColumnName("average_delivery_seconds").HasPrecision(20, 4).HasDefaultValue(0m);
                entity.Property(e => e.RetryCount).HasColumnName("retry_count").HasDefaultValue(0UL);
                entity.Property(e => e.DeadLetterCount).HasColumnName("dead_letter_count").HasDefaultValue(0UL);
                entity.Property(e => e.TotalCost).HasColumnName("total_cost").HasPrecision(20, 4).HasDefaultValue(0m);
                entity.Property(e => e.CurrencySid).HasColumnName("currency_sid").HasMaxLength(32);
                entity.Property(e => e.SnapshotStatus).HasColumnName("snapshot_status").HasMaxLength(20).HasDefaultValue("FINAL");
            });
            #endregion

            #region 22. 狀態異動歷程 (Status History)
            modelBuilder.Entity<NcStatusHistory>(entity =>
            {
                entity.ToTable("nc_status_history");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.EntitySid).HasColumnName("entity_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.OldStatus).HasColumnName("old_status").HasMaxLength(30);
                entity.Property(e => e.NewStatus).HasColumnName("new_status").HasMaxLength(30).IsRequired();
                entity.Property(e => e.EventCode).HasColumnName("event_code").HasMaxLength(100);
                entity.Property(e => e.OperatorUserSid).HasColumnName("operator_user_sid").HasMaxLength(32);
                entity.Property(e => e.Reason).HasColumnName("reason");
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasMaxLength(100);
            });
            #endregion

            #region 23. 領域事件 (Event)
            modelBuilder.Entity<NcEvent>(entity =>
            {
                entity.ToTable("nc_event");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SourceEventId).IsUnique();

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.EntitySid).HasColumnName("entity_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.EventCode).HasColumnName("event_code").HasMaxLength(120).IsRequired();
                entity.Property(e => e.EventVersion).HasColumnName("event_version").HasDefaultValue(1);
                entity.Property(e => e.EventData).HasColumnName("event_data").HasColumnType("json");
                entity.Property(e => e.SourceEventId).HasColumnName("source_event_id").HasMaxLength(100);
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasMaxLength(100);
                entity.Property(e => e.CausationId).HasColumnName("causation_id").HasMaxLength(100);
                entity.Property(e => e.OutboxEventSid).HasColumnName("outbox_event_sid").HasMaxLength(32);
                entity.Property(e => e.ProcessStatus).HasColumnName("process_status").HasMaxLength(20).HasDefaultValue("PENDING");
                entity.Property(e => e.ProcessedDate).HasColumnName("processed_date");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
            });
            #endregion
        }
    }
}