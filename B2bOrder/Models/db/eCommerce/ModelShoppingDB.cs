using System;
using System.Collections.Generic;

namespace B2bOrder.Resources.eCommerce.Models
{
    // =========================================================
    // 01. 前台通路與商店設定 (Channel and Storefront Settings)
    // =========================================================

    #region 前台通路 (Channel / shp_channel)
    public class ShpChannel
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ChannelCode { get; set; } = null!;
        public string ChannelName { get; set; } = null!;
        public string ChannelType { get; set; } = null!;
        public string? CompanySid { get; set; }
        public string? BusinessUnitSid { get; set; }
        public string DefaultCurrencySid { get; set; } = null!;
        public string DefaultLanguageSid { get; set; } = null!;
        public string? DefaultPriceListSid { get; set; }
        public bool AnonymousCheckout { get; set; }
        public bool RegistrationRequired { get; set; }
        public string ChannelStatus { get; set; } = null!;
        public string Avalible { get; set; } = null!;
        public string? Remark { get; set; }
    }
    #endregion

    #region 前台商店 (Storefront / shp_storefront)
    public class ShpStorefront
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string StorefrontCode { get; set; } = null!;
        public string StorefrontName { get; set; } = null!;
        public string ChannelSid { get; set; } = null!;
        public string? OwnerPartySid { get; set; }
        public string? CompanySid { get; set; }
        public string? BusinessUnitSid { get; set; }
        public string? WarehouseSid { get; set; }
        public string? PriceListSid { get; set; }
        public string? DomainName { get; set; }
        public string? LogoFileSid { get; set; }
        public string? ThemeConfig { get; set; } // JSON
        public string? SeoConfig { get; set; } // JSON
        public string StorefrontStatus { get; set; } = null!;
        public string Avalible { get; set; } = null!;
        public string? Remark { get; set; }
    }
    #endregion


    // =========================================================
    // 02. 前台商品上架 (Product Listings)
    // =========================================================

    #region 前台商品上架資料 (Listing / shp_listing)
    public class ShpListing
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ListingNo { get; set; } = null!;
        public string StorefrontSid { get; set; } = null!;
        public string ItemSid { get; set; } = null!;
        public string? DefaultVariantSid { get; set; }
        public string ListingTitle { get; set; } = null!;
        public string? Subtitle { get; set; }
        public string Slug { get; set; } = null!;
        public string? CategorySid { get; set; }
        public string? BrandSid { get; set; }
        public string? PriceListSid { get; set; }
        public string SalesMode { get; set; } = null!;
        public bool VisibleWithoutLogin { get; set; }
        public bool PurchasableWithoutLogin { get; set; }
        public decimal MinimumOrderQty { get; set; }
        public decimal? MaximumOrderQty { get; set; }
        public decimal OrderMultipleQty { get; set; }
        public DateTime? PreorderStartDate { get; set; }
        public DateTime? PreorderEndDate { get; set; }
        public DateTime? PublishStartDate { get; set; }
        public DateTime? PublishEndDate { get; set; }
        public string ListingStatus { get; set; } = null!;
        public string? ApprovedUserSid { get; set; }
        public string? WorkflowInstanceSid { get; set; }
        public ulong VersionNo { get; set; }
        public string Avalible { get; set; } = null!;
        public string? Remark { get; set; }

        public virtual ICollection<ShpListingVariant> ListingVariants { get; set; } = new HashSet<ShpListingVariant>();
    }
    #endregion

    #region 前台商品變體上架 (Listing Variant / shp_listing_variant)
    public class ShpListingVariant
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong ListingNid { get; set; }
        public string VariantSid { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? PriceListSid { get; set; }
        public string? WarehouseSid { get; set; }
        public int SortNo { get; set; }
        public bool VisibleMark { get; set; }
        public bool PurchasableMark { get; set; }
        public string VariantStatus { get; set; } = null!;

        public virtual ShpListing Listing { get; set; } = null!;
    }
    #endregion


    // =========================================================
    // 03. 購物車 (Shopping Cart)
    // =========================================================

    #region 購物車 (Cart / shp_cart)
    public class ShpCart
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CartNo { get; set; } = null!;
        public string StorefrontSid { get; set; } = null!;
        public string ChannelSid { get; set; } = null!;
        public string? MemberPartySid { get; set; }
        public string? UserSid { get; set; }
        public string? GuestTokenHash { get; set; }
        public string? SessionId { get; set; }
        public string CurrencySid { get; set; } = null!;
        public string? PriceLevelSid { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalQty { get; set; }
        public decimal SubtotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PromotionAmount { get; set; }
        public decimal EstimatedTaxAmount { get; set; }
        public decimal EstimatedTotalAmount { get; set; }
        public DateTime? PriceExpiryDate { get; set; }
        public DateTime? CartExpiryDate { get; set; }
        public string CartStatus { get; set; } = null!;
        public ulong VersionNo { get; set; }

        public virtual ICollection<ShpCartItem> CartItems { get; set; } = new HashSet<ShpCartItem>();
    }
    #endregion

    #region 購物車明細 (Cart Item / shp_cart_item)
    public class ShpCartItem
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong CartNid { get; set; }
        public int LineNo { get; set; }
        public string ListingSid { get; set; } = null!;
        public string ItemSid { get; set; } = null!;
        public string? VariantSid { get; set; }
        public decimal Quantity { get; set; }
        public string UnitSid { get; set; } = null!;
        public decimal ListPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PromotionAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineAmount { get; set; }
        public string? PricingResultSid { get; set; }
        public decimal? InventoryAvailableQty { get; set; }
        public string? ReservationSid { get; set; }
        public string? ItemSnapshot { get; set; } // JSON
        public string? PriceSnapshot { get; set; } // JSON
        public string? SelectedOptions { get; set; } // JSON
        public string ItemStatus { get; set; } = null!;

        public virtual ShpCart Cart { get; set; } = null!;
    }
    #endregion


    // =========================================================
    // 04. 結帳工作階段 (Checkout Session)
    // =========================================================

    #region 購物平台結帳工作階段 (Checkout Session / shp_checkout_session)
    public class ShpCheckoutSession
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CheckoutNo { get; set; } = null!;
        public string CartSid { get; set; } = null!;
        public string StorefrontSid { get; set; } = null!;
        public string? MemberPartySid { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestMobile { get; set; }
        public string CurrencySid { get; set; } = null!;
        public string? PricingRequestSid { get; set; }
        public string? ShippingMethodSid { get; set; }
        public string? PaymentMethodSid { get; set; }
        public string? BillingAddressSid { get; set; }
        public string? ShippingAddressSid { get; set; }
        public decimal SubtotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PromotionAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal FreightAmount { get; set; }
        public decimal ServiceAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool InventoryValidated { get; set; }
        public bool PriceValidated { get; set; }
        public bool AddressValidated { get; set; }
        public string? PaymentRequestSid { get; set; }
        public string? SalesOrderSid { get; set; }
        public DateTime CheckoutExpiryDate { get; set; }
        public string CheckoutStatus { get; set; } = null!;
        public string IdempotencyKey { get; set; } = null!;
        public string? CorrelationId { get; set; }
        public string? ErrorMessage { get; set; }

        public virtual ICollection<ShpCheckoutAddress> CheckoutAddresses { get; set; } = new HashSet<ShpCheckoutAddress>();
    }
    #endregion

    #region 結帳地址快照 (Checkout Address / shp_checkout_address)
    public class ShpCheckoutAddress
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong CheckoutSessionNid { get; set; }
        public string AddressType { get; set; } = null!;
        public string? SourceAddressSid { get; set; }
        public string RecipientName { get; set; } = null!;
        public string? RecipientPhone { get; set; }
        public string CountrySid { get; set; } = null!;
        public string? RegionLevel1Sid { get; set; }
        public string? RegionLevel2Sid { get; set; }
        public string? RegionLevel3Sid { get; set; }
        public string? PostalCode { get; set; }
        public string AddressLine1 { get; set; } = null!;
        public string? AddressLine2 { get; set; }
        public string? DeliveryNote { get; set; }

        public virtual ShpCheckoutSession CheckoutSession { get; set; } = null!;
    }
    #endregion


    // =========================================================
    // 05. 收藏與購物清單 (Wishlist)
    // =========================================================

    #region 會員收藏與購物清單 (Wishlist / shp_wishlist)
    public class ShpWishlist
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string WishlistNo { get; set; } = null!;
        public string MemberPartySid { get; set; } = null!;
        public string StorefrontSid { get; set; } = null!;
        public string WishlistName { get; set; } = null!;
        public bool PublicMark { get; set; }
        public string WishlistStatus { get; set; } = null!;

        public virtual ICollection<ShpWishlistItem> WishlistItems { get; set; } = new HashSet<ShpWishlistItem>();
    }
    #endregion

    #region 收藏清單明細 (Wishlist Item / shp_wishlist_item)
    public class ShpWishlistItem
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong WishlistNid { get; set; }
        public string ListingSid { get; set; } = null!;
        public string ItemSid { get; set; } = null!;
        public string? VariantSid { get; set; }
        public decimal? TargetPrice { get; set; }
        public bool PriceAlertEnabled { get; set; }
        public bool StockAlertEnabled { get; set; }
        public string? Note { get; set; }
        public string ItemStatus { get; set; } = null!;

        public virtual ShpWishlist Wishlist { get; set; } = null!;
    }
    #endregion


    // =========================================================
    // 06. 商品瀏覽與行為 (Customer Behavior and Tracking)
    // =========================================================

    #region 商品瀏覽紀錄 (View History / shp_view_history)
    public class ShpViewHistory
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string StorefrontSid { get; set; } = null!;
        public string? MemberPartySid { get; set; }
        public string? UserSid { get; set; }
        public string? GuestTokenHash { get; set; }
        public string? SessionId { get; set; }
        public string ListingSid { get; set; } = null!;
        public string ItemSid { get; set; } = null!;
        public string? VariantSid { get; set; }
        public string? SourcePage { get; set; }
        public string? ReferrerUrl { get; set; }
        public string? SearchKeyword { get; set; }
        public string? DeviceType { get; set; }
        public string? IpAddress { get; set; }
        public int? ViewDurationSeconds { get; set; }
    }
    #endregion

    #region 前台搜尋紀錄 (Search History / shp_search_history)
    public class ShpSearchHistory
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string StorefrontSid { get; set; } = null!;
        public string? MemberPartySid { get; set; }
        public string? GuestTokenHash { get; set; }
        public string? SessionId { get; set; }
        public string Keyword { get; set; } = null!;
        public string? FilterData { get; set; } // JSON
        public string? SortCode { get; set; }
        public int ResultCount { get; set; }
        public string? ClickedListingSid { get; set; }
        public string? DeviceType { get; set; }
    }
    #endregion

    #region 顧客前台行為事件 (Customer Event / shp_customer_event)
    public class ShpCustomerEvent
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string StorefrontSid { get; set; } = null!;
        public string? MemberPartySid { get; set; }
        public string? GuestTokenHash { get; set; }
        public string? SessionId { get; set; }
        public string EventCode { get; set; } = null!;
        public string? EntityType { get; set; }
        public string? EntitySid { get; set; }
        public string? EventData { get; set; } // JSON
        public string? SourcePage { get; set; }
        public string? CampaignCode { get; set; }
        public string? CorrelationId { get; set; }
    }
    #endregion


    // =========================================================
    // 07. 商品評論與評分 (Product Reviews and Ratings)
    // =========================================================

    #region 商品評論與評分 (Review / shp_review)
    public class ShpReview
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ReviewNo { get; set; } = null!;
        public string StorefrontSid { get; set; } = null!;
        public string ListingSid { get; set; } = null!;
        public string ItemSid { get; set; } = null!;
        public string? VariantSid { get; set; }
        public string MemberPartySid { get; set; } = null!;
        public string? SalesOrderSid { get; set; }
        public string? SalesOrderItemSid { get; set; }
        public bool VerifiedPurchase { get; set; }
        public decimal Rating { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public decimal? QualityRating { get; set; }
        public decimal? ValueRating { get; set; }
        public decimal? DeliveryRating { get; set; }
        public bool AnonymousMark { get; set; }
        public int HelpfulCount { get; set; }
        public int ReportCount { get; set; }
        public string ReviewStatus { get; set; } = null!;
        public string? ModerationUserSid { get; set; }
        public DateTime? ModerationDate { get; set; }
        public string? ModerationNote { get; set; }
        public string Avalible { get; set; } = null!;

        public virtual ICollection<ShpReviewMedia> ReviewMedias { get; set; } = new HashSet<ShpReviewMedia>();
        public virtual ICollection<ShpReviewReaction> ReviewReactions { get; set; } = new HashSet<ShpReviewReaction>();
        public virtual ICollection<ShpReviewReply> ReviewReplies { get; set; } = new HashSet<ShpReviewReply>();
    }
    #endregion

    #region 商品評論圖片與影片 (Review Media / shp_review_media)
    public class ShpReviewMedia
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong ReviewNid { get; set; }
        public string MediaType { get; set; } = null!;
        public string FileSid { get; set; } = null!;
        public int SortNo { get; set; }
        public string MediaStatus { get; set; } = null!;

        public virtual ShpReview Review { get; set; } = null!;
    }
    #endregion

    #region 評論有幫助與檢舉 (Review Reaction / shp_review_reaction)
    public class ShpReviewReaction
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong ReviewNid { get; set; }
        public string MemberPartySid { get; set; } = null!;
        public string ReactionType { get; set; } = null!;
        public string? ReportReasonCode { get; set; }
        public string? ReportDescription { get; set; }
        public string ReactionStatus { get; set; } = null!;

        public virtual ShpReview Review { get; set; } = null!;
    }
    #endregion

    #region 商店與客服評論回覆 (Review Reply / shp_review_reply)
    public class ShpReviewReply
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong ReviewNid { get; set; }
        public string ReplyType { get; set; } = null!;
        public string? ReplyUserSid { get; set; }
        public string? ReplyPartySid { get; set; }
        public string Content { get; set; } = null!;
        public string ReplyStatus { get; set; } = null!;

        public virtual ShpReview Review { get; set; } = null!;
    }
    #endregion


    // =========================================================
    // 08. 商品問答 (Product Q&A)
    // =========================================================

    #region 商品問答問題 (Question / shp_question)
    public class ShpQuestion
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string QuestionNo { get; set; } = null!;
        public string StorefrontSid { get; set; } = null!;
        public string ListingSid { get; set; } = null!;
        public string ItemSid { get; set; } = null!;
        public string? VariantSid { get; set; }
        public string? MemberPartySid { get; set; }
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string QuestionContent { get; set; } = null!;
        public bool PublicMark { get; set; }
        public int AnswerCount { get; set; }
        public string QuestionStatus { get; set; } = null!;
        public string? ModerationUserSid { get; set; }

        public virtual ICollection<ShpAnswer> Answers { get; set; } = new HashSet<ShpAnswer>();
    }
    #endregion

    #region 商品問答回答 (Answer / shp_answer)
    public class ShpAnswer
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong QuestionNid { get; set; }
        public string AnswerType { get; set; } = null!;
        public string? AnswerUserSid { get; set; }
        public string? AnswerPartySid { get; set; }
        public string AnswerContent { get; set; } = null!;
        public bool OfficialMark { get; set; }
        public bool AcceptedMark { get; set; }
        public int HelpfulCount { get; set; }
        public string AnswerStatus { get; set; } = null!;

        public virtual ShpQuestion Question { get; set; } = null!;
    }
    #endregion


    // =========================================================
    // 09. 到貨與到價通知 (Alert Subscriptions)
    // =========================================================

    #region 到貨、降價與預購提醒訂閱 (Alert Subscription / shp_alert_subscription)
    public class ShpAlertSubscription
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string StorefrontSid { get; set; } = null!;
        public string ListingSid { get; set; } = null!;
        public string ItemSid { get; set; } = null!;
        public string? VariantSid { get; set; }
        public string? MemberPartySid { get; set; }
        public string ContactType { get; set; } = null!;
        public string? ContactValueHash { get; set; }
        public string? ContactValueEncrypted { get; set; }
        public string AlertType { get; set; } = null!;
        public decimal? TargetPrice { get; set; }
        public DateTime? LastNotifiedDate { get; set; }
        public int NotifyCount { get; set; }
        public string SubscriptionStatus { get; set; } = null!;
        public DateTime? ExpiryDate { get; set; }
    }
    #endregion


    // =========================================================
    // 10. 棄單與轉換 (Abandoned Carts and Conversions)
    // =========================================================

    #region 購物車與結帳棄單 (Abandoned Cart / shp_abandoned_cart)
    public class ShpAbandonedCart
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CartSid { get; set; } = null!;
        public string? CheckoutSid { get; set; }
        public string? MemberPartySid { get; set; }
        public string? GuestEmailHash { get; set; }
        public string AbandonedStage { get; set; } = null!;
        public DateTime AbandonedDate { get; set; }
        public decimal CartAmount { get; set; }
        public string RecoveryStatus { get; set; } = null!;
        public string? RecoveryCampaignSid { get; set; }
        public string? RecoveredOrderSid { get; set; }
        public DateTime? LastContactDate { get; set; }
    }
    #endregion

    #region 購物訂單轉換與行銷歸因 (Conversion Attribution / shp_conversion_attribution)
    public class ShpConversionAttribution
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string SalesOrderSid { get; set; } = null!;
        public string StorefrontSid { get; set; } = null!;
        public string? MemberPartySid { get; set; }
        public string? FirstTouchSource { get; set; }
        public string? FirstTouchCampaign { get; set; }
        public string? LastTouchSource { get; set; }
        public string? LastTouchCampaign { get; set; }
        public string? CouponCode { get; set; }
        public string? ReferralPartySid { get; set; }
        public string? AffiliatePartySid { get; set; }
        public string AttributionModel { get; set; } = null!;
        public string? AttributionData { get; set; } // JSON
    }
    #endregion


    // =========================================================
    // 11. 狀態歷程與領域事件 (Status History and Domain Events)
    // =========================================================

    #region 購物平台狀態歷程 (Status History / shp_status_history)
    public class ShpStatusHistory
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

    #region 購物平台領域事件 (Event / shp_event)
    public class ShpEvent
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string EntityType { get; set; } = null!;
        public string EntitySid { get; set; } = null!;
        public string EventCode { get; set; } = null!;
        public int EventVersion { get; set; }
        public string? EventData { get; set; } // JSON
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