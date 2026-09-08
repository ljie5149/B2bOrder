using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Customer.Models
{
    #region Subscription Plan Module - 訂閱方案與計費週期定價

    [Table("sub_plan")]
    public class SubPlan
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
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("plan_code")]
        public string PlanCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("plan_name")]
        public string PlanName { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("plan_status")]
        public string PlanStatus { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    [Table("sub_plan_pricing")]
    public class SubPlanPricing
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

        [Column("plan_nid")]
        public ulong PlanNid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("billing_cycle_unit")]
        public string BillingCycleUnit { get; set; }

        [Column("billing_cycle_interval")]
        public int BillingCycleInterval { get; set; }

        [Column("price_amount")]
        public decimal PriceAmount { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("currency_code")]
        public string CurrencyCode { get; set; }

        [Column("trial_period_days")]
        public int TrialPeriodDays { get; set; }

        [Column("setup_fee_amount")]
        public decimal SetupFeeAmount { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("pricing_status")]
        public string PricingStatus { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Subscription Contract Module - 會員訂閱合約主檔

    [Table("sub_subscription")]
    public class SubSubscription
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
        [Column("subscription_no")]
        public string SubscriptionNo { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("customer_sid")]
        public string CustomerSid { get; set; }

        [Column("plan_nid")]
        public ulong PlanNid { get; set; }

        [Column("plan_pricing_nid")]
        public ulong PlanPricingNid { get; set; }

        [MaxLength(32)]
        [Column("payment_method_sid")]
        public string PaymentMethodSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("subscription_status")]
        public string SubscriptionStatus { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("trial_end_date")]
        public DateTime? TrialEndDate { get; set; }

        [Column("current_period_start")]
        public DateTime CurrentPeriodStart { get; set; }

        [Column("current_period_end")]
        public DateTime CurrentPeriodEnd { get; set; }

        [Column("next_billing_date")]
        public DateTime? NextBillingDate { get; set; }

        [Column("canceled_at")]
        public DateTime? CanceledAt { get; set; }

        [Column("ended_at")]
        public DateTime? EndedAt { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("auto_renew")]
        public string AutoRenew { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Subscription Invoice & Payment Attempt Module - 週期扣款帳單與扣款嘗試

    [Table("sub_invoice")]
    public class SubInvoice
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
        [Column("invoice_no")]
        public string InvoiceNo { get; set; }

        [Column("subscription_nid")]
        public ulong SubscriptionNid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("customer_sid")]
        public string CustomerSid { get; set; }

        [Column("period_start")]
        public DateTime PeriodStart { get; set; }

        [Column("period_end")]
        public DateTime PeriodEnd { get; set; }

        [Column("subtotal_amount")]
        public decimal SubtotalAmount { get; set; }

        [Column("discount_amount")]
        public decimal DiscountAmount { get; set; }

        [Column("tax_amount")]
        public decimal TaxAmount { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("currency_code")]
        public string CurrencyCode { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("invoice_status")]
        public string InvoiceStatus { get; set; }

        [Column("due_date")]
        public DateTime DueDate { get; set; }

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }

        [MaxLength(32)]
        [Column("sales_order_sid")]
        public string SalesOrderSid { get; set; }

        [MaxLength(32)]
        [Column("payment_transaction_sid")]
        public string PaymentTransactionSid { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    [Table("sub_payment_attempt")]
    public class SubPaymentAttempt
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

        [Column("invoice_nid")]
        public ulong InvoiceNid { get; set; }

        [Column("attempt_number")]
        public int AttemptNumber { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("payment_gateway_code")]
        public string PaymentGatewayCode { get; set; }

        [MaxLength(32)]
        [Column("payment_transaction_sid")]
        public string PaymentTransactionSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("attempt_status")]
        public string AttemptStatus { get; set; }

        [MaxLength(100)]
        [Column("failure_code")]
        public string FailureCode { get; set; }

        [Column("failure_message")]
        public string FailureMessage { get; set; }

        [Column("next_retry_date")]
        public DateTime? NextRetryDate { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Subscription Lifecycle Log Module - 訂閱生命週期歷程紀錄

    [Table("sub_subscription_log")]
    public class SubSubscriptionLog
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

        [Column("subscription_nid")]
        public ulong SubscriptionNid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("event_type")]
        public string EventType { get; set; }

        [MaxLength(30)]
        [Column("previous_status")]
        public string PreviousStatus { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("new_status")]
        public string NewStatus { get; set; }

        [Column("previous_pricing_nid")]
        public ulong? PreviousPricingNid { get; set; }

        [Column("new_pricing_nid")]
        public ulong? NewPricingNid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("operator_type")]
        public string OperatorType { get; set; }

        [MaxLength(32)]
        [Column("operator_user_sid")]
        public string OperatorUserSid { get; set; }

        [MaxLength(100)]
        [Column("reason_code")]
        public string ReasonCode { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion
}