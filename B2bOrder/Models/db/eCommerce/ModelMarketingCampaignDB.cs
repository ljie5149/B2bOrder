using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.eCommerce.Models
{
    #region Marketing Campaign Module - 行銷活動與渠道配置

    [Table("mkt_campaign")]
    public class MktCampaign
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("campaign_no")]
        public string CampaignNo { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("campaign_name")]
        public string CampaignName { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [MaxLength(32)]
        [Column("sales_project_sid")]
        public string SalesProjectSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("campaign_type")]
        public string CampaignType { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("objective_type")]
        public string ObjectiveType { get; set; }

        [Column("planned_start_date")]
        public DateTime PlannedStartDate { get; set; }

        [Column("planned_end_date")]
        public DateTime PlannedEndDate { get; set; }

        [Column("actual_start_date")]
        public DateTime? ActualStartDate { get; set; }

        [Column("actual_end_date")]
        public DateTime? ActualEndDate { get; set; }

        [Column("budget_amount")]
        public decimal BudgetAmount { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; }

        [MaxLength(32)]
        [Column("owner_party_sid")]
        public string OwnerPartySid { get; set; }

        [Column("target_audience")]
        public string TargetAudience { get; set; }

        [Column("campaign_description")]
        public string CampaignDescription { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("campaign_status")]
        public string CampaignStatus { get; set; }

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string WorkflowInstanceSid { get; set; }

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    [Table("mkt_channel")]
    public class MktChannel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("channel_code")]
        public string ChannelCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("channel_name")]
        public string ChannelName { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("channel_type")]
        public string ChannelType { get; set; }

        [MaxLength(200)]
        [Column("platform_name")]
        public string PlatformName { get; set; }

        [MaxLength(300)]
        [Column("account_reference")]
        public string AccountReference { get; set; }

        [MaxLength(32)]
        [Column("default_currency_sid")]
        public string DefaultCurrencySid { get; set; }

        [Column("tracking_enabled")]
        public bool TrackingEnabled { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("channel_status")]
        public string ChannelStatus { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }
    }

    [Table("mkt_campaign_channel")]
    public class MktCampaignChannel
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("campaign_sid")]
        public string CampaignSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("channel_sid")]
        public string ChannelSid { get; set; }

        [MaxLength(200)]
        [Column("channel_campaign_id")]
        public string ChannelCampaignId { get; set; }

        [Column("planned_budget")]
        public decimal PlannedBudget { get; set; }

        [Column("actual_spend")]
        public decimal ActualSpend { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [MaxLength(1000)]
        [Column("landing_page_url")]
        public string LandingPageUrl { get; set; }

        [MaxLength(200)]
        [Column("tracking_code")]
        public string TrackingCode { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("channel_status")]
        public string ChannelStatus { get; set; }
    }

    #endregion

    #region Marketing Promotion & Coupon Module - 促銷與優惠券管理

    [Table("mkt_promotion")]
    public class MktPromotion
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("promotion_no")]
        public string PromotionNo { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("promotion_name")]
        public string PromotionName { get; set; }

        [MaxLength(32)]
        [Column("campaign_sid")]
        public string CampaignSid { get; set; }

        [MaxLength(32)]
        [Column("sales_project_sid")]
        public string SalesProjectSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("promotion_type")]
        public string PromotionType { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("eligibility_rule")]
        public string EligibilityRule { get; set; }

        [Required]
        [Column("benefit_rule")]
        public string BenefitRule { get; set; }

        [Column("budget_limit")]
        public decimal? BudgetLimit { get; set; }

        [Column("usage_limit")]
        public int? UsageLimit { get; set; }

        [Column("per_customer_limit")]
        public int? PerCustomerLimit { get; set; }

        [Column("approval_required")]
        public bool ApprovalRequired { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("promotion_status")]
        public string PromotionStatus { get; set; }

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string WorkflowInstanceSid { get; set; }
    }

    [Table("mkt_coupon")]
    public class MktCoupon
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("coupon_code")]
        public string CouponCode { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("promotion_sid")]
        public string PromotionSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("coupon_type")]
        public string CouponType { get; set; }

        [MaxLength(32)]
        [Column("customer_sid")]
        public string CustomerSid { get; set; }

        [Column("issue_date")]
        public DateTime? IssueDate { get; set; }

        [Column("valid_start_date")]
        public DateTime ValidStartDate { get; set; }

        [Column("valid_end_date")]
        public DateTime ValidEndDate { get; set; }

        [Column("usage_limit")]
        public int UsageLimit { get; set; }

        [Column("used_count")]
        public int UsedCount { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("coupon_status")]
        public string CouponStatus { get; set; }
    }

    #endregion
}