using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.eCommerce.Models;

namespace B2bOrder.Resources.eCommerce
{
    /// <summary>
    /// ShoppingDB V2 資料庫內容物件 (Database Context)
    /// </summary>
    public class ShoppingDbContext : DbContext
    {
        public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : base(options)
        {
        }

        #region 01. 前台通路與商店設定 (Channel and Storefront)
        public DbSet<ShpChannel> ShpChannels { get; set; } = null!;
        public DbSet<ShpStorefront> ShpStorefronts { get; set; } = null!;
        #endregion

        #region 02. 前台商品上架 (Product Listings)
        public DbSet<ShpListing> ShpListings { get; set; } = null!;
        public DbSet<ShpListingVariant> ShpListingVariants { get; set; } = null!;
        #endregion

        #region 03. 購物車 (Shopping Cart)
        public DbSet<ShpCart> ShpCarts { get; set; } = null!;
        public DbSet<ShpCartItem> ShpCartItems { get; set; } = null!;
        #endregion

        #region 04. 結帳工作階段 (Checkout Session)
        public DbSet<ShpCheckoutSession> ShpCheckoutSessions { get; set; } = null!;
        public DbSet<ShpCheckoutAddress> ShpCheckoutAddresses { get; set; } = null!;
        #endregion

        #region 05. 收藏與購物清單 (Wishlist)
        public DbSet<ShpWishlist> ShpWishlists { get; set; } = null!;
        public DbSet<ShpWishlistItem> ShpWishlistItems { get; set; } = null!;
        #endregion

        #region 06. 商品瀏覽與行為 (Customer Behavior and Tracking)
        public DbSet<ShpViewHistory> ShpViewHistories { get; set; } = null!;
        public DbSet<ShpSearchHistory> ShpSearchHistories { get; set; } = null!;
        public DbSet<ShpCustomerEvent> ShpCustomerEvents { get; set; } = null!;
        #endregion

        #region 07. 商品評論與評分 (Product Reviews and Ratings)
        public DbSet<ShpReview> ShpReviews { get; set; } = null!;
        public DbSet<ShpReviewMedia> ShpReviewMedias { get; set; } = null!;
        public DbSet<ShpReviewReaction> ShpReviewReactions { get; set; } = null!;
        public DbSet<ShpReviewReply> ShpReviewReplies { get; set; } = null!;
        #endregion

        #region 08. 商品問答 (Product Q&A)
        public DbSet<ShpQuestion> ShpQuestions { get; set; } = null!;
        public DbSet<ShpAnswer> ShpAnswers { get; set; } = null!;
        #endregion

        #region 09. 到貨與到價通知 (Alert Subscriptions)
        public DbSet<ShpAlertSubscription> ShpAlertSubscriptions { get; set; } = null!;
        #endregion

        #region 10. 棄單與轉換 (Abandoned Carts and Conversions)
        public DbSet<ShpAbandonedCart> ShpAbandonedCarts { get; set; } = null!;
        public DbSet<ShpConversionAttribution> ShpConversionAttributions { get; set; } = null!;
        #endregion

        #region 11. 狀態歷程與領域事件 (Status History and Domain Events)
        public DbSet<ShpStatusHistory> ShpStatusHistories { get; set; } = null!;
        public DbSet<ShpEvent> ShpEvents { get; set; } = null!;
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 01. 前台通路與商店設定
            modelBuilder.Entity<ShpChannel>(entity =>
            {
                entity.ToTable("shp_channel");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.ChannelCode).HasColumnName("channel_code");
                entity.Property(e => e.ChannelName).HasColumnName("channel_name");
                entity.Property(e => e.ChannelType).HasColumnName("channel_type");
                entity.Property(e => e.CompanySid).HasColumnName("company_sid");
                entity.Property(e => e.BusinessUnitSid).HasColumnName("business_unit_sid");
                entity.Property(e => e.DefaultCurrencySid).HasColumnName("default_currency_sid");
                entity.Property(e => e.DefaultLanguageSid).HasColumnName("default_language_sid");
                entity.Property(e => e.DefaultPriceListSid).HasColumnName("default_price_list_sid");
                entity.Property(e => e.AnonymousCheckout).HasColumnName("anonymous_checkout");
                entity.Property(e => e.RegistrationRequired).HasColumnName("registration_required");
                entity.Property(e => e.ChannelStatus).HasColumnName("channel_status");
                entity.Property(e => e.Avalible).HasColumnName("avalible");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });

            modelBuilder.Entity<ShpStorefront>(entity =>
            {
                entity.ToTable("shp_storefront");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.StorefrontCode).HasColumnName("storefront_code");
                entity.Property(e => e.StorefrontName).HasColumnName("storefront_name");
                entity.Property(e => e.ChannelSid).HasColumnName("channel_sid");
                entity.Property(e => e.OwnerPartySid).HasColumnName("owner_party_sid");
                entity.Property(e => e.CompanySid).HasColumnName("company_sid");
                entity.Property(e => e.BusinessUnitSid).HasColumnName("business_unit_sid");
                entity.Property(e => e.WarehouseSid).HasColumnName("warehouse_sid");
                entity.Property(e => e.PriceListSid).HasColumnName("price_list_sid");
                entity.Property(e => e.DomainName).HasColumnName("domain_name");
                entity.Property(e => e.LogoFileSid).HasColumnName("logo_file_sid");
                entity.Property(e => e.ThemeConfig).HasColumnName("theme_config");
                entity.Property(e => e.SeoConfig).HasColumnName("seo_config");
                entity.Property(e => e.StorefrontStatus).HasColumnName("storefront_status");
                entity.Property(e => e.Avalible).HasColumnName("avalible");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });

            // 02. 前台商品上架
            modelBuilder.Entity<ShpListing>(entity =>
            {
                entity.ToTable("shp_listing");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.ListingNo).HasColumnName("listing_no");
                entity.Property(e => e.StorefrontSid).HasColumnName("storefront_sid");
                entity.Property(e => e.ItemSid).HasColumnName("item_sid");
                entity.Property(e => e.DefaultVariantSid).HasColumnName("default_variant_sid");
                entity.Property(e => e.ListingTitle).HasColumnName("listing_title");
                entity.Property(e => e.Subtitle).HasColumnName("subtitle");
                entity.Property(e => e.Slug).HasColumnName("slug");
                entity.Property(e => e.CategorySid).HasColumnName("category_sid");
                entity.Property(e => e.BrandSid).HasColumnName("brand_sid");
                entity.Property(e => e.PriceListSid).HasColumnName("price_list_sid");
                entity.Property(e => e.SalesMode).HasColumnName("sales_mode");
                entity.Property(e => e.VisibleWithoutLogin).HasColumnName("visible_without_login");
                entity.Property(e => e.PurchasableWithoutLogin).HasColumnName("purchasable_without_login");
                entity.Property(e => e.MinimumOrderQty).HasColumnName("minimum_order_qty");
                entity.Property(e => e.MaximumOrderQty).HasColumnName("maximum_order_qty");
                entity.Property(e => e.OrderMultipleQty).HasColumnName("order_multiple_qty");
                entity.Property(e => e.PreorderStartDate).HasColumnName("preorder_start_date");
                entity.Property(e => e.PreorderEndDate).HasColumnName("preorder_end_date");
                entity.Property(e => e.PublishStartDate).HasColumnName("publish_start_date");
                entity.Property(e => e.PublishEndDate).HasColumnName("publish_end_date");
                entity.Property(e => e.ListingStatus).HasColumnName("listing_status");
                entity.Property(e => e.ApprovedUserSid).HasColumnName("approved_user_sid");
                entity.Property(e => e.WorkflowInstanceSid).HasColumnName("workflow_instance_sid");
                entity.Property(e => e.VersionNo).HasColumnName("version_no");
                entity.Property(e => e.Avalible).HasColumnName("avalible");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });

            modelBuilder.Entity<ShpListingVariant>(entity =>
            {
                entity.ToTable("shp_listing_variant");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.ListingNid).HasColumnName("listing_nid");
                entity.Property(e => e.VariantSid).HasColumnName("variant_sid");
                entity.Property(e => e.DisplayName).HasColumnName("display_name");
                entity.Property(e => e.PriceListSid).HasColumnName("price_list_sid");
                entity.Property(e => e.WarehouseSid).HasColumnName("warehouse_sid");
                entity.Property(e => e.SortNo).HasColumnName("sort_no");
                entity.Property(e => e.VisibleMark).HasColumnName("visible_mark");
                entity.Property(e => e.PurchasableMark).HasColumnName("purchasable_mark");
                entity.Property(e => e.VariantStatus).HasColumnName("variant_status");

                entity.HasOne(d => d.Listing)
                    .WithMany(p => p.ListingVariants)
                    .HasForeignKey(d => d.ListingNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_slv_listing");
            });

            // 03. 購物車
            modelBuilder.Entity<ShpCart>(entity =>
            {
                entity.ToTable("shp_cart");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.CartNo).HasColumnName("cart_no");
                entity.Property(e => e.StorefrontSid).HasColumnName("storefront_sid");
                entity.Property(e => e.ChannelSid).HasColumnName("channel_sid");
                entity.Property(e => e.MemberPartySid).HasColumnName("member_party_sid");
                entity.Property(e => e.UserSid).HasColumnName("user_sid");
                entity.Property(e => e.GuestTokenHash).HasColumnName("guest_token_hash");
                entity.Property(e => e.SessionId).HasColumnName("session_id");
                entity.Property(e => e.CurrencySid).HasColumnName("currency_sid");
                entity.Property(e => e.PriceLevelSid).HasColumnName("price_level_sid");
                entity.Property(e => e.ItemCount).HasColumnName("item_count");
                entity.Property(e => e.TotalQty).HasColumnName("total_qty");
                entity.Property(e => e.SubtotalAmount).HasColumnName("subtotal_amount");
                entity.Property(e => e.DiscountAmount).HasColumnName("discount_amount");
                entity.Property(e => e.PromotionAmount).HasColumnName("promotion_amount");
                entity.Property(e => e.EstimatedTaxAmount).HasColumnName("estimated_tax_amount");
                entity.Property(e => e.EstimatedTotalAmount).HasColumnName("estimated_total_amount");
                entity.Property(e => e.PriceExpiryDate).HasColumnName("price_expiry_date");
                entity.Property(e => e.CartExpiryDate).HasColumnName("cart_expiry_date");
                entity.Property(e => e.CartStatus).HasColumnName("cart_status");
                entity.Property(e => e.VersionNo).HasColumnName("version_no");
            });

            modelBuilder.Entity<ShpCartItem>(entity =>
            {
                entity.ToTable("shp_cart_item");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.CartNid).HasColumnName("cart_nid");
                entity.Property(e => e.LineNo).HasColumnName("line_no");
                entity.Property(e => e.ListingSid).HasColumnName("listing_sid");
                entity.Property(e => e.ItemSid).HasColumnName("item_sid");
                entity.Property(e => e.VariantSid).HasColumnName("variant_sid");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.UnitSid).HasColumnName("unit_sid");
                entity.Property(e => e.ListPrice).HasColumnName("list_price");
                entity.Property(e => e.UnitPrice).HasColumnName("unit_price");
                entity.Property(e => e.DiscountAmount).HasColumnName("discount_amount");
                entity.Property(e => e.PromotionAmount).HasColumnName("promotion_amount");
                entity.Property(e => e.TaxAmount).HasColumnName("tax_amount");
                entity.Property(e => e.LineAmount).HasColumnName("line_amount");
                entity.Property(e => e.PricingResultSid).HasColumnName("pricing_result_sid");
                entity.Property(e => e.InventoryAvailableQty).HasColumnName("inventory_available_qty");
                entity.Property(e => e.ReservationSid).HasColumnName("reservation_sid");
                entity.Property(e => e.ItemSnapshot).HasColumnName("item_snapshot");
                entity.Property(e => e.PriceSnapshot).HasColumnName("price_snapshot");
                entity.Property(e => e.SelectedOptions).HasColumnName("selected_options");
                entity.Property(e => e.ItemStatus).HasColumnName("item_status");

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CartNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_sci_cart");
            });

            // 04. 結帳工作階段
            modelBuilder.Entity<ShpCheckoutSession>(entity =>
            {
                entity.ToTable("shp_checkout_session");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.CheckoutNo).HasColumnName("checkout_no");
                entity.Property(e => e.CartSid).HasColumnName("cart_sid");
                entity.Property(e => e.StorefrontSid).HasColumnName("storefront_sid");
                entity.Property(e => e.MemberPartySid).HasColumnName("member_party_sid");
                entity.Property(e => e.GuestEmail).HasColumnName("guest_email");
                entity.Property(e => e.GuestMobile).HasColumnName("guest_mobile");
                entity.Property(e => e.CurrencySid).HasColumnName("currency_sid");
                entity.Property(e => e.PricingRequestSid).HasColumnName("pricing_request_sid");
                entity.Property(e => e.ShippingMethodSid).HasColumnName("shipping_method_sid");
                entity.Property(e => e.PaymentMethodSid).HasColumnName("payment_method_sid");
                entity.Property(e => e.BillingAddressSid).HasColumnName("billing_address_sid");
                entity.Property(e => e.ShippingAddressSid).HasColumnName("shipping_address_sid");
                entity.Property(e => e.SubtotalAmount).HasColumnName("subtotal_amount");
                entity.Property(e => e.DiscountAmount).HasColumnName("discount_amount");
                entity.Property(e => e.PromotionAmount).HasColumnName("promotion_amount");
                entity.Property(e => e.TaxAmount).HasColumnName("tax_amount");
                entity.Property(e => e.FreightAmount).HasColumnName("freight_amount");
                entity.Property(e => e.ServiceAmount).HasColumnName("service_amount");
                entity.Property(e => e.TotalAmount).HasColumnName("total_amount");
                entity.Property(e => e.InventoryValidated).HasColumnName("inventory_validated");
                entity.Property(e => e.PriceValidated).HasColumnName("price_validated");
                entity.Property(e => e.AddressValidated).HasColumnName("address_validated");
                entity.Property(e => e.PaymentRequestSid).HasColumnName("payment_request_sid");
                entity.Property(e => e.SalesOrderSid).HasColumnName("sales_order_sid");
                entity.Property(e => e.CheckoutExpiryDate).HasColumnName("checkout_expiry_date");
                entity.Property(e => e.CheckoutStatus).HasColumnName("checkout_status");
                entity.Property(e => e.IdempotencyKey).HasColumnName("idempotency_key");
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
            });

            modelBuilder.Entity<ShpCheckoutAddress>(entity =>
            {
                entity.ToTable("shp_checkout_address");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.CheckoutSessionNid).HasColumnName("checkout_session_nid");
                entity.Property(e => e.AddressType).HasColumnName("address_type");
                entity.Property(e => e.SourceAddressSid).HasColumnName("source_address_sid");
                entity.Property(e => e.RecipientName).HasColumnName("recipient_name");
                entity.Property(e => e.RecipientPhone).HasColumnName("recipient_phone");
                entity.Property(e => e.CountrySid).HasColumnName("country_sid");
                entity.Property(e => e.RegionLevel1Sid).HasColumnName("region_level1_sid");
                entity.Property(e => e.RegionLevel2Sid).HasColumnName("region_level2_sid");
                entity.Property(e => e.RegionLevel3Sid).HasColumnName("region_level3_sid");
                entity.Property(e => e.PostalCode).HasColumnName("postal_code");
                entity.Property(e => e.AddressLine1).HasColumnName("address_line1");
                entity.Property(e => e.AddressLine2).HasColumnName("address_line2");
                entity.Property(e => e.DeliveryNote).HasColumnName("delivery_note");

                entity.HasOne(d => d.CheckoutSession)
                    .WithMany(p => p.CheckoutAddresses)
                    .HasForeignKey(d => d.CheckoutSessionNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_sca_checkout");
            });

            // 05. 收藏與購物清單
            modelBuilder.Entity<ShpWishlist>(entity =>
            {
                entity.ToTable("shp_wishlist");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.WishlistNo).HasColumnName("wishlist_no");
                entity.Property(e => e.MemberPartySid).HasColumnName("member_party_sid");
                entity.Property(e => e.StorefrontSid).HasColumnName("storefront_sid");
                entity.Property(e => e.WishlistName).HasColumnName("wishlist_name");
                entity.Property(e => e.PublicMark).HasColumnName("public_mark");
                entity.Property(e => e.WishlistStatus).HasColumnName("wishlist_status");
            });

            modelBuilder.Entity<ShpWishlistItem>(entity =>
            {
                entity.ToTable("shp_wishlist_item");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.WishlistNid).HasColumnName("wishlist_nid");
                entity.Property(e => e.ListingSid).HasColumnName("listing_sid");
                entity.Property(e => e.ItemSid).HasColumnName("item_sid");
                entity.Property(e => e.VariantSid).HasColumnName("variant_sid");
                entity.Property(e => e.TargetPrice).HasColumnName("target_price");
                entity.Property(e => e.PriceAlertEnabled).HasColumnName("price_alert_enabled");
                entity.Property(e => e.StockAlertEnabled).HasColumnName("stock_alert_enabled");
                entity.Property(e => e.Note).HasColumnName("note");
                entity.Property(e => e.ItemStatus).HasColumnName("item_status");

                entity.HasOne(d => d.Wishlist)
                    .WithMany(p => p.WishlistItems)
                    .HasForeignKey(d => d.WishlistNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_swi_wishlist");
            });

            // 06. 商品瀏覽與行為
            modelBuilder.Entity<ShpViewHistory>(entity =>
            {
                entity.ToTable("shp_view_history");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.StorefrontSid).HasColumnName("storefront_sid");
                entity.Property(e => e.MemberPartySid).HasColumnName("member_party_sid");
                entity.Property(e => e.UserSid).HasColumnName("user_sid");
                entity.Property(e => e.GuestTokenHash).HasColumnName("guest_token_hash");
                entity.Property(e => e.SessionId).HasColumnName("session_id");
                entity.Property(e => e.ListingSid).HasColumnName("listing_sid");
                entity.Property(e => e.ItemSid).HasColumnName("item_sid");
                entity.Property(e => e.VariantSid).HasColumnName("variant_sid");
                entity.Property(e => e.SourcePage).HasColumnName("source_page");
                entity.Property(e => e.ReferrerUrl).HasColumnName("referrer_url");
                entity.Property(e => e.SearchKeyword).HasColumnName("search_keyword");
                entity.Property(e => e.DeviceType).HasColumnName("device_type");
                entity.Property(e => e.IpAddress).HasColumnName("ip_address");
                entity.Property(e => e.ViewDurationSeconds).HasColumnName("view_duration_seconds");
            });

            modelBuilder.Entity<ShpSearchHistory>(entity =>
            {
                entity.ToTable("shp_search_history");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.StorefrontSid).HasColumnName("storefront_sid");
                entity.Property(e => e.MemberPartySid).HasColumnName("member_party_sid");
                entity.Property(e => e.GuestTokenHash).HasColumnName("guest_token_hash");
                entity.Property(e => e.SessionId).HasColumnName("session_id");
                entity.Property(e => e.Keyword).HasColumnName("keyword");
                entity.Property(e => e.FilterData).HasColumnName("filter_data");
                entity.Property(e => e.SortCode).HasColumnName("sort_code");
                entity.Property(e => e.ResultCount).HasColumnName("result_count");
                entity.Property(e => e.ClickedListingSid).HasColumnName("clicked_listing_sid");
                entity.Property(e => e.DeviceType).HasColumnName("device_type");
            });

            modelBuilder.Entity<ShpCustomerEvent>(entity =>
            {
                entity.ToTable("shp_customer_event");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.StorefrontSid).HasColumnName("storefront_sid");
                entity.Property(e => e.MemberPartySid).HasColumnName("member_party_sid");
                entity.Property(e => e.GuestTokenHash).HasColumnName("guest_token_hash");
                entity.Property(e => e.SessionId).HasColumnName("session_id");
                entity.Property(e => e.EventCode).HasColumnName("event_code");
                entity.Property(e => e.EntityType).HasColumnName("entity_type");
                entity.Property(e => e.EntitySid).HasColumnName("entity_sid");
                entity.Property(e => e.EventData).HasColumnName("event_data");
                entity.Property(e => e.SourcePage).HasColumnName("source_page");
                entity.Property(e => e.CampaignCode).HasColumnName("campaign_code");
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id");
            });

            // 07. 商品評論與評分
            modelBuilder.Entity<ShpReview>(entity =>
            {
                entity.ToTable("shp_review");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.ReviewNo).HasColumnName("review_no");
                entity.Property(e => e.StorefrontSid).HasColumnName("storefront_sid");
                entity.Property(e => e.ListingSid).HasColumnName("listing_sid");
                entity.Property(e => e.ItemSid).HasColumnName("item_sid");
                entity.Property(e => e.VariantSid).HasColumnName("variant_sid");
                entity.Property(e => e.MemberPartySid).HasColumnName("member_party_sid");
                entity.Property(e => e.SalesOrderSid).HasColumnName("sales_order_sid");
                entity.Property(e => e.SalesOrderItemSid).HasColumnName("sales_order_item_sid");
                entity.Property(e => e.VerifiedPurchase).HasColumnName("verified_purchase");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.QualityRating).HasColumnName("quality_rating");
                entity.Property(e => e.ValueRating).HasColumnName("value_rating");
                entity.Property(e => e.DeliveryRating).HasColumnName("delivery_rating");
                entity.Property(e => e.AnonymousMark).HasColumnName("anonymous_mark");
                entity.Property(e => e.HelpfulCount).HasColumnName("helpful_count");
                entity.Property(e => e.ReportCount).HasColumnName("report_count");
                entity.Property(e => e.ReviewStatus).HasColumnName("review_status");
                entity.Property(e => e.ModerationUserSid).HasColumnName("moderation_user_sid");
                entity.Property(e => e.ModerationDate).HasColumnName("moderation_date");
                entity.Property(e => e.ModerationNote).HasColumnName("moderation_note");
                entity.Property(e => e.Avalible).HasColumnName("avalible");
            });

            modelBuilder.Entity<ShpReviewMedia>(entity =>
            {
                entity.ToTable("shp_review_media");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ReviewNid).HasColumnName("review_nid");
                entity.Property(e => e.MediaType).HasColumnName("media_type");
                entity.Property(e => e.FileSid).HasColumnName("file_sid");
                entity.Property(e => e.SortNo).HasColumnName("sort_no");
                entity.Property(e => e.MediaStatus).HasColumnName("media_status");

                entity.HasOne(d => d.Review)
                    .WithMany(p => p.ReviewMedias)
                    .HasForeignKey(d => d.ReviewNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_srm_review");
            });

            modelBuilder.Entity<ShpReviewReaction>(entity =>
            {
                entity.ToTable("shp_review_reaction");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ReviewNid).HasColumnName("review_nid");
                entity.Property(e => e.MemberPartySid).HasColumnName("member_party_sid");
                entity.Property(e => e.ReactionType).HasColumnName("reaction_type");
                entity.Property(e => e.ReportReasonCode).HasColumnName("report_reason_code");
                entity.Property(e => e.ReportDescription).HasColumnName("report_description");
                entity.Property(e => e.ReactionStatus).HasColumnName("reaction_status");

                entity.HasOne(d => d.Review)
                    .WithMany(p => p.ReviewReactions)
                    .HasForeignKey(d => d.ReviewNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_srr_review");
            });

            modelBuilder.Entity<ShpReviewReply>(entity =>
            {
                entity.ToTable("shp_review_reply");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.ReviewNid).HasColumnName("review_nid");
                entity.Property(e => e.ReplyType).HasColumnName("reply_type");
                entity.Property(e => e.ReplyUserSid).HasColumnName("reply_user_sid");
                entity.Property(e => e.ReplyPartySid).HasColumnName("reply_party_sid");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.ReplyStatus).HasColumnName("reply_status");

                entity.HasOne(d => d.Review)
                    .WithMany(p => p.ReviewReplies)
                    .HasForeignKey(d => d.ReviewNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_srrep_review");
            });

            // 08. 商品問答
            modelBuilder.Entity<ShpQuestion>(entity =>
            {
                entity.ToTable("shp_question");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.QuestionNo).HasColumnName("question_no");
                entity.Property(e => e.StorefrontSid).HasColumnName("storefront_sid");
                entity.Property(e => e.ListingSid).HasColumnName("listing_sid");
                entity.Property(e => e.ItemSid).HasColumnName("item_sid");
                entity.Property(e => e.VariantSid).HasColumnName("variant_sid");
                entity.Property(e => e.MemberPartySid).HasColumnName("member_party_sid");
                entity.Property(e => e.GuestName).HasColumnName("guest_name");
                entity.Property(e => e.GuestEmail).HasColumnName("guest_email");
                entity.Property(e => e.QuestionContent).HasColumnName("question_content");
                entity.Property(e => e.PublicMark).HasColumnName("public_mark");
                entity.Property(e => e.AnswerCount).HasColumnName("answer_count");
                entity.Property(e => e.QuestionStatus).HasColumnName("question_status");
                entity.Property(e => e.ModerationUserSid).HasColumnName("moderation_user_sid");
            });

            modelBuilder.Entity<ShpAnswer>(entity =>
            {
                entity.ToTable("shp_answer");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.QuestionNid).HasColumnName("question_nid");
                entity.Property(e => e.AnswerType).HasColumnName("answer_type");
                entity.Property(e => e.AnswerUserSid).HasColumnName("answer_user_sid");
                entity.Property(e => e.AnswerPartySid).HasColumnName("answer_party_sid");
                entity.Property(e => e.AnswerContent).HasColumnName("answer_content");
                entity.Property(e => e.OfficialMark).HasColumnName("official_mark");
                entity.Property(e => e.AcceptedMark).HasColumnName("accepted_mark");
                entity.Property(e => e.HelpfulCount).HasColumnName("helpful_count");
                entity.Property(e => e.AnswerStatus).HasColumnName("answer_status");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.QuestionNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_sa_question");
            });

            // 09. 到貨與到價通知
            modelBuilder.Entity<ShpAlertSubscription>(entity =>
            {
                entity.ToTable("shp_alert_subscription");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.StorefrontSid).HasColumnName("storefront_sid");
                entity.Property(e => e.ListingSid).HasColumnName("listing_sid");
                entity.Property(e => e.ItemSid).HasColumnName("item_sid");
                entity.Property(e => e.VariantSid).HasColumnName("variant_sid");
                entity.Property(e => e.MemberPartySid).HasColumnName("member_party_sid");
                entity.Property(e => e.ContactType).HasColumnName("contact_type");
                entity.Property(e => e.ContactValueHash).HasColumnName("contact_value_hash");
                entity.Property(e => e.ContactValueEncrypted).HasColumnName("contact_value_encrypted");
                entity.Property(e => e.AlertType).HasColumnName("alert_type");
                entity.Property(e => e.TargetPrice).HasColumnName("target_price");
                entity.Property(e => e.LastNotifiedDate).HasColumnName("last_notified_date");
                entity.Property(e => e.NotifyCount).HasColumnName("notify_count");
                entity.Property(e => e.SubscriptionStatus).HasColumnName("subscription_status");
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            });

            // 10. 棄單與轉換
            modelBuilder.Entity<ShpAbandonedCart>(entity =>
            {
                entity.ToTable("shp_abandoned_cart");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.CartSid).HasColumnName("cart_sid");
                entity.Property(e => e.CheckoutSid).HasColumnName("checkout_sid");
                entity.Property(e => e.MemberPartySid).HasColumnName("member_party_sid");
                entity.Property(e => e.GuestEmailHash).HasColumnName("guest_email_hash");
                entity.Property(e => e.AbandonedStage).HasColumnName("abandoned_stage");
                entity.Property(e => e.AbandonedDate).HasColumnName("abandoned_date");
                entity.Property(e => e.CartAmount).HasColumnName("cart_amount");
                entity.Property(e => e.RecoveryStatus).HasColumnName("recovery_status");
                entity.Property(e => e.RecoveryCampaignSid).HasColumnName("recovery_campaign_sid");
                entity.Property(e => e.RecoveredOrderSid).HasColumnName("recovered_order_sid");
                entity.Property(e => e.LastContactDate).HasColumnName("last_contact_date");
            });

            modelBuilder.Entity<ShpConversionAttribution>(entity =>
            {
                entity.ToTable("shp_conversion_attribution");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => _e.Sid).HasColumnName("sid"); // Note: check variable spelling below in actual file if needed, standard is e.Sid
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.SalesOrderSid).HasColumnName("sales_order_sid");
                entity.Property(e => e.StorefrontSid).HasColumnName("storefront_sid");
                entity.Property(e => e.MemberPartySid).HasColumnName("member_party_sid");
                entity.Property(e => e.FirstTouchSource).HasColumnName("first_touch_source");
                entity.Property(e => e.FirstTouchCampaign).HasColumnName("first_touch_campaign");
                entity.Property(e => e.LastTouchSource).HasColumnName("last_touch_source");
                entity.Property(e => e.LastTouchCampaign).HasColumnName("last_touch_campaign");
                entity.Property(e => e.CouponCode).HasColumnName("coupon_code");
                entity.Property(e => e.ReferralPartySid).HasColumnName("referral_party_sid");
                entity.Property(e => e.AffiliatePartySid).HasColumnName("affiliate_party_sid");
                entity.Property(e => e.AttributionModel).HasColumnName("attribution_model");
                entity.Property(e => e.AttributionData).HasColumnName("attribution_data");
            });

            // 11. 狀態歷程與領域事件
            modelBuilder.Entity<ShpStatusHistory>(entity =>
            {
                entity.ToTable("shp_status_history");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.EntityType).HasColumnName("entity_type");
                entity.Property(e => e.EntitySid).HasColumnName("entity_sid");
                entity.Property(e => e.OldStatus).HasColumnName("old_status");
                entity.Property(e => e.NewStatus).HasColumnName("new_status");
                entity.Property(e => e.EventCode).HasColumnName("event_code");
                entity.Property(e => e.OperatorUserSid).HasColumnName("operator_user_sid");
                entity.Property(e => e.Reason).HasColumnName("reason");
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id");
            });

            modelBuilder.Entity<ShpEvent>(entity =>
            {
                entity.ToTable("shp_event");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.EntityType).HasColumnName("entity_type");
                entity.Property(e => e.EntitySid).HasColumnName("entity_sid");
                entity.Property(e => e.EventCode).HasColumnName("event_code");
                entity.Property(e => e.EventVersion).HasColumnName("event_version");
                entity.Property(e => e.EventData).HasColumnName("event_data");
                entity.Property(e => e.SourceEventId).HasColumnName("source_event_id");
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id");
                entity.Property(e => e.CausationId).HasColumnName("causation_id");
                entity.Property(e => e.OutboxEventSid).HasColumnName("outbox_event_sid");
                entity.Property(e => e.ProcessStatus).HasColumnName("process_status");
                entity.Property(e => e.ProcessedDate).HasColumnName("processed_date");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
            });
        }
    }
}