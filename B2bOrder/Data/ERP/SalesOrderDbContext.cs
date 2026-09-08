// SalesOrderDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Resources.eCommerce
{
    /// <summary>
    /// SalesOrderDB 銷售訂單系統資料庫上下文 (Sales Order Database Context)
    /// </summary>
    public class SalesOrderDbContext : DbContext
    {
        public SalesOrderDbContext(DbContextOptions<SalesOrderDbContext> options) : base(options)
        {
        }

        // 01. 銷售類型與政策 (Sales Types & Policies)
        public DbSet<SalOrderType> SalOrderTypes { get; set; } = null!;
        public DbSet<SalPolicy> SalPolicies { get; set; } = null!;

        // 02. 報價 (Quotations)
        public DbSet<SalQuote> SalQuotes { get; set; } = null!;
        public DbSet<SalQuoteItem> SalQuoteItems { get; set; } = null!;

        // 03. 銷售單 (Sales Orders)
        public DbSet<SalOrder> SalOrders { get; set; } = null!;
        public DbSet<SalOrderItem> SalOrderItems { get; set; } = null!;

        // 04. 交期與履約需求 (Delivery Schedules & Fulfillment Requests)
        public DbSet<SalDeliverySchedule> SalDeliverySchedules { get; set; } = null!;
        public DbSet<SalFulfillmentRequest> SalFulfillmentRequests { get; set; } = null!;
        public DbSet<SalFulfillmentRequestItem> SalFulfillmentRequestItems { get; set; } = null!;

        // 05. 訂單地址與聯絡快照 (Order Addresses & Contacts Snapshots)
        public DbSet<SalOrderAddress> SalOrderAddresses { get; set; } = null!;
        public DbSet<SalOrderContact> SalOrderContacts { get; set; } = null!;

        // 06. 付款與應收串接 (Payments & Receivables Integration)
        public DbSet<SalPaymentRequest> SalPaymentRequests { get; set; } = null!;
        public DbSet<SalReceivableRequest> SalReceivableRequests { get; set; } = null!;

        // 07. 發票串接 (Invoices Integration)
        public DbSet<SalInvoiceRequest> SalInvoiceRequests { get; set; } = null!;
        public DbSet<SalInvoiceRequestItem> SalInvoiceRequestItems { get; set; } = null!;

        // 08. 訂單變更與取消 (Order Changes & Cancellations)
        public DbSet<SalOrderChange> SalOrderChanges { get; set; } = null!;
        public DbSet<SalCancellation> SalCancellations { get; set; } = null!;
        public DbSet<SalCancellationItem> SalCancellationItems { get; set; } = null!;

        // 09. 訂單註記與附件 (Order Notes & Attachments)
        public DbSet<SalOrderNote> SalOrderNotes { get; set; } = null!;
        public DbSet<SalOrderAttachment> SalOrderAttachments { get; set; } = null!;

        // 10. 狀態歷程與事件 (Status Histories & Domain Events)
        public DbSet<SalStatusHistory> SalStatusHistories { get; set; } = null!;
        public DbSet<SalEvent> SalEvents { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 01. sal_order_type
            modelBuilder.Entity<SalOrderType>(entity =>
            {
                entity.ToTable("sal_order_type", tb => tb.HasComment("銷售單類型"));
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("銷售類型序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").HasComment("修改日期");
                entity.Property(e => e.OrderTypeCode).HasColumnName("order_type_code").HasComment("銷售類型代碼");
                entity.Property(e => e.OrderTypeName).HasColumnName("order_type_name").HasComment("銷售類型名稱");
                entity.Property(e => e.OrderCategory).HasColumnName("order_category").HasComment("RETAIL零售;B2B企業;PROJECT專案;CONTRACT合約;SERVICE服務;INTERNAL內部;RETURN退貨");
                entity.Property(e => e.InventoryEffect).HasColumnName("inventory_effect").HasComment("是否影響庫存");
                entity.Property(e => e.ApprovalRequired).HasColumnName("approval_required").HasComment("是否需要簽核");
                entity.Property(e => e.CreditCheckRequired).HasColumnName("credit_check_required").HasComment("是否需信用檢查");
                entity.Property(e => e.ContractRequired).HasColumnName("contract_required").HasComment("是否需合約");
                entity.Property(e => e.FulfillmentRequired).HasColumnName("fulfillment_required").HasComment("是否需履約");
                entity.Property(e => e.InvoiceRequired).HasColumnName("invoice_required").HasComment("是否需發票");
                entity.Property(e => e.OrderTypeStatus).HasColumnName("order_type_status").HasComment("ACTIVE啟用;INACTIVE停用");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasComment("Y可用;D刪除;W停用");
                entity.Property(e => e.Remark).HasColumnName("remark").HasComment("備註");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.OrderTypeCode).IsUnique().HasDatabaseName("uk_sot_order_type_code");
                entity.HasIndex(e => e.OrderCategory).HasDatabaseName("idx_sot_category");
                entity.HasIndex(e => e.ApprovalRequired).HasDatabaseName("idx_sot_approval_required");
                entity.HasIndex(e => e.CreditCheckRequired).HasDatabaseName("idx_sot_credit_check_required");
                entity.HasIndex(e => e.OrderTypeStatus).HasDatabaseName("idx_sot_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_sot_avalible");
            });

            // 01. sal_policy
            modelBuilder.Entity<SalPolicy>(entity =>
            {
                entity.ToTable("sal_policy", tb => {
                    tb.HasComment("銷售政策");
                    tb.HasCheckConstraint("CK_sal_policy_over_delivery_rate", "over_delivery_rate >= 0");
                    tb.HasCheckConstraint("CK_sal_policy_cancellation_cutoff_min", "cancellation_cutoff_min >= 0");
                    tb.HasCheckConstraint("CK_sal_policy_price_lock_minutes", "price_lock_minutes >= 0");
                    tb.HasCheckConstraint("CK_sal_policy_stock_lock_minutes", "stock_lock_minutes >= 0");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("銷售政策序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").HasComment("修改日期");
                entity.Property(e => e.PolicyCode).HasColumnName("policy_code").HasComment("政策代碼");
                entity.Property(e => e.PolicyName).HasColumnName("policy_name").HasComment("政策名稱");
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasComment("MasterDB公司序號");
                entity.Property(e => e.OrderTypeSid).HasColumnName("order_type_sid").HasComment("銷售類型序號");
                entity.Property(e => e.AllowPartialDelivery).HasColumnName("allow_partial_delivery").HasComment("是否允許分批交貨");
                entity.Property(e => e.AllowBackorder).HasColumnName("allow_backorder").HasComment("是否允許缺貨待補");
                entity.Property(e => e.AllowOverDelivery).HasColumnName("allow_over_delivery").HasComment("是否允許超交");
                entity.Property(e => e.OverDeliveryRate).HasColumnName("over_delivery_rate").HasColumnType("decimal(8,4)").HasComment("允許超交率");
                entity.Property(e => e.CancellationAllowed).HasColumnName("cancellation_allowed").HasComment("是否允許取消");
                entity.Property(e => e.CancellationCutoffMin).HasColumnName("cancellation_cutoff_min").HasComment("取消截止分鐘");
                entity.Property(e => e.PriceLockMinutes).HasColumnName("price_lock_minutes").HasComment("價格鎖定分鐘");
                entity.Property(e => e.StockLockMinutes).HasColumnName("stock_lock_minutes").HasComment("庫存鎖定分鐘");
                entity.Property(e => e.CreditHoldEnabled).HasColumnName("credit_hold_enabled").HasComment("是否啟用信用凍結");
                entity.Property(e => e.PolicyStatus).HasColumnName("policy_status").HasComment("ACTIVE啟用;INACTIVE停用");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasComment("Y可用;D刪除;W停用");
                entity.Property(e => e.Remark).HasColumnName("remark").HasComment("備註");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.PolicyCode).IsUnique().HasDatabaseName("uk_sp_policy_code");
                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_sp_company_sid");
                entity.HasIndex(e => e.OrderTypeSid).HasDatabaseName("idx_sp_order_type_sid");
                entity.HasIndex(e => e.PolicyStatus).HasDatabaseName("idx_sp_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_sp_avalible");
            });

            // 02. sal_quote
            modelBuilder.Entity<SalQuote>(entity =>
            {
                entity.ToTable("sal_quote", tb => {
                    tb.HasComment("銷售報價單");
                    tb.HasCheckConstraint("CK_sal_quote_exchange_rate", "exchange_rate > 0");
                    tb.HasCheckConstraint("CK_sal_quote_subtotal_amount", "subtotal_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_quote_discount_amount", "discount_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_quote_tax_amount", "tax_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_quote_freight_amount", "freight_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_quote_other_amount", "other_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_quote_total_amount", "total_amount >= 0");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("報價單序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").HasComment("修改日期");
                entity.Property(e => e.QuoteNo).HasColumnName("quote_no").HasComment("報價單號");
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasComment("公司序號");
                entity.Property(e => e.BusinessUnitSid).HasColumnName("business_unit_sid").HasComment("營運單位序號");
                entity.Property(e => e.DepartmentSid).HasColumnName("department_sid").HasComment("部門序號");
                entity.Property(e => e.OrderTypeSid).HasColumnName("order_type_sid").HasComment("銷售類型序號");
                entity.Property(e => e.CustomerPartySid).HasColumnName("customer_party_sid").HasComment("PartyDB客戶序號");
                entity.Property(e => e.ContactPartySid).HasColumnName("contact_party_sid").HasComment("聯絡人Party序號");
                entity.Property(e => e.ProjectSid).HasColumnName("project_sid").HasComment("專案序號");
                entity.Property(e => e.SiteSid).HasColumnName("site_sid").HasComment("工地序號");
                entity.Property(e => e.ContractSid).HasColumnName("contract_sid").HasComment("合約序號");
                entity.Property(e => e.OpportunitySid).HasColumnName("opportunity_sid").HasComment("CRMDB商機序號");
                entity.Property(e => e.SalesOwnerUserSid).HasColumnName("sales_owner_user_sid").HasComment("負責業務帳號序號");
                entity.Property(e => e.QuoteDate).HasColumnName("quote_date").HasComment("報價日期");
                entity.Property(e => e.ValidUntil).HasColumnName("valid_until").HasComment("有效期限");
                entity.Property(e => e.CurrencySid).HasColumnName("currency_sid").HasComment("幣別序號");
                entity.Property(e => e.ExchangeRate).HasColumnName("exchange_rate").HasColumnType("decimal(20,10)").HasComment("匯率");
                entity.Property(e => e.PriceListSid).HasColumnName("price_list_sid").HasComment("PricingDB價格清單序號");
                entity.Property(e => e.AgreementSid).HasColumnName("agreement_sid").HasComment("PricingDB價格協議序號");
                entity.Property(e => e.SubtotalAmount).HasColumnName("subtotal_amount").HasColumnType("decimal(20,4)").HasComment("未稅小計");
                entity.Property(e => e.DiscountAmount).HasColumnName("discount_amount").HasColumnType("decimal(20,4)").HasComment("折扣金額");
                entity.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(20,4)").HasComment("稅額");
                entity.Property(e => e.FreightAmount).HasColumnName("freight_amount").HasColumnType("decimal(20,4)").HasComment("運費");
                entity.Property(e => e.OtherAmount).HasColumnName("other_amount").HasColumnType("decimal(20,4)").HasComment("其他費用");
                entity.Property(e => e.TotalAmount).HasColumnName("total_amount").HasColumnType("decimal(20,4)").HasComment("報價總額");
                entity.Property(e => e.PaymentTermSid).HasColumnName("payment_term_sid").HasComment("付款條件序號");
                entity.Property(e => e.DeliveryTermCode).HasColumnName("delivery_term_code").HasComment("交貨條件");
                entity.Property(e => e.QuoteStatus).HasColumnName("quote_status").HasComment("DRAFT草稿;SUBMITTED已送出;REVIEW待審;APPROVED核准;SENT已送客戶;ACCEPTED接受;REJECTED拒絕;EXPIRED過期;CONVERTED已轉單;CANCELLED取消");
                entity.Property(e => e.WorkflowInstanceSid).HasColumnName("workflow_instance_sid").HasComment("WorkflowDB流程實例序號");
                entity.Property(e => e.ApprovedUserSid).HasColumnName("approved_user_sid").HasComment("核准人員序號");
                entity.Property(e => e.ApprovedDate).HasColumnName("approved_date").HasComment("核准時間");
                entity.Property(e => e.AcceptedDate).HasColumnName("accepted_date").HasComment("客戶接受時間");
                entity.Property(e => e.VersionNo).HasColumnName("version_no").HasComment("樂觀鎖版本");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasComment("Y可用;D刪除;W停用");
                entity.Property(e => e.Remark).HasColumnName("remark").HasComment("備註");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.QuoteNo).IsUnique().HasDatabaseName("uk_sq_quote_no");
                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_sq_company_sid");
                entity.HasIndex(e => e.OrderTypeSid).HasDatabaseName("idx_sq_order_type_sid");
                entity.HasIndex(e => e.CustomerPartySid).HasDatabaseName("idx_sq_customer_party_sid");
                entity.HasIndex(e => e.ProjectSid).HasDatabaseName("idx_sq_project_sid");
                entity.HasIndex(e => e.ContractSid).HasDatabaseName("idx_sq_contract_sid");
                entity.HasIndex(e => e.OpportunitySid).HasDatabaseName("idx_sq_opportunity_sid");
                entity.HasIndex(e => e.SalesOwnerUserSid).HasDatabaseName("idx_sq_sales_owner_user_sid");
                entity.HasIndex(e => e.QuoteDate).HasDatabaseName("idx_sq_quote_date");
                entity.HasIndex(e => e.ValidUntil).HasDatabaseName("idx_sq_valid_until");
                entity.HasIndex(e => e.QuoteStatus).HasDatabaseName("idx_sq_status");
                entity.HasIndex(e => e.WorkflowInstanceSid).HasDatabaseName("idx_sq_workflow_instance_sid");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_sq_avalible");
            });

            // 02. sal_quote_item
            modelBuilder.Entity<SalQuoteItem>(entity =>
            {
                entity.ToTable("sal_quote_item", tb => {
                    tb.HasComment("銷售報價明細");
                    tb.HasCheckConstraint("CK_sal_quote_item_line_no", "line_no > 0");
                    tb.HasCheckConstraint("CK_sal_quote_item_quantity", "quantity > 0");
                    tb.HasCheckConstraint("CK_sal_quote_item_list_price", "list_price >= 0");
                    tb.HasCheckConstraint("CK_sal_quote_item_unit_price", "unit_price >= 0");
                    tb.HasCheckConstraint("CK_sal_quote_item_discount_rate", "discount_rate >= 0");
                    tb.HasCheckConstraint("CK_sal_quote_item_discount_amount", "discount_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_quote_item_tax_amount", "tax_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_quote_item_line_amount", "line_amount >= 0");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("報價明細序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").HasComment("修改日期");
                entity.Property(e => e.QuoteNid).HasColumnName("quote_nid").HasComment("報價單流水號");
                entity.Property(e => e.LineNo).HasColumnName("line_no").HasComment("明細行號");
                entity.Property(e => e.ItemSid).HasColumnName("item_sid").HasComment("PIM/MIMDB Item序號");
                entity.Property(e => e.VariantSid).HasColumnName("variant_sid").HasComment("PIM/MIMDB變體序號");
                entity.Property(e => e.ItemDescription).HasColumnName("item_description").HasComment("品名或服務說明");
                entity.Property(e => e.SpecificationText).HasColumnName("specification_text").HasComment("規格說明");
                entity.Property(e => e.Quantity).HasColumnName("quantity").HasColumnType("decimal(20,6)").HasComment("數量");
                entity.Property(e => e.UnitSid).HasColumnName("unit_sid").HasComment("單位序號");
                entity.Property(e => e.ListPrice).HasColumnName("list_price").HasColumnType("decimal(20,6)").HasComment("牌價");
                entity.Property(e => e.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(20,6)").HasComment("報價單價");
                entity.Property(e => e.DiscountRate).HasColumnName("discount_rate").HasColumnType("decimal(8,4)").HasComment("折扣率");
                entity.Property(e => e.DiscountAmount).HasColumnName("discount_amount").HasColumnType("decimal(20,4)").HasComment("折扣金額");
                entity.Property(e => e.TaxSid).HasColumnName("tax_sid").HasComment("稅別序號");
                entity.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(20,4)").HasComment("稅額");
                entity.Property(e => e.LineAmount).HasColumnName("line_amount").HasColumnType("decimal(20,4)").HasComment("明細總額");
                entity.Property(e => e.RequestedDeliveryDate).HasColumnName("requested_delivery_date").HasComment("客戶需求交期");
                entity.Property(e => e.ProjectSid).HasColumnName("project_sid").HasComment("專案序號");
                entity.Property(e => e.SiteSid).HasColumnName("site_sid").HasComment("工地序號");
                entity.Property(e => e.WbsSid).HasColumnName("wbs_sid").HasComment("WBS序號");
                entity.Property(e => e.ContractItemSid).HasColumnName("contract_item_sid").HasComment("合約明細序號");
                entity.Property(e => e.PricingResultSid).HasColumnName("pricing_result_sid").HasComment("PricingDB計價結果序號");
                entity.Property(e => e.PriceSnapshot).HasColumnName("price_snapshot").HasColumnType("json").HasComment("價格計算快照");
                entity.Property(e => e.ItemStatus).HasColumnName("item_status").HasComment("OPEN有效;CANCELLED取消;CONVERTED已轉單");
                entity.Property(e => e.Remark).HasColumnName("remark").HasComment("備註");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.QuoteNid, e.LineNo }).IsUnique().HasDatabaseName("uk_sqi_quote_line");
                entity.HasIndex(e => e.QuoteNid).HasDatabaseName("idx_sqi_quote_nid");
                entity.HasIndex(e => e.ItemSid).HasDatabaseName("idx_sqi_item_sid");
                entity.HasIndex(e => e.VariantSid).HasDatabaseName("idx_sqi_variant_sid");
                entity.HasIndex(e => e.ProjectSid).HasDatabaseName("idx_sqi_project_sid");
                entity.HasIndex(e => e.WbsSid).HasDatabaseName("idx_sqi_wbs_sid");
                entity.HasIndex(e => e.ContractItemSid).HasDatabaseName("idx_sqi_contract_item_sid");
                entity.HasIndex(e => e.PricingResultSid).HasDatabaseName("idx_sqi_pricing_result_sid");
                entity.HasIndex(e => e.ItemStatus).HasDatabaseName("idx_sqi_status");

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.QuoteItems)
                    .HasForeignKey(d => d.QuoteNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_sqi_quote");
            });

            // 03. sal_order
            modelBuilder.Entity<SalOrder>(entity =>
            {
                entity.ToTable("sal_order", tb => {
                    tb.HasComment("共用銷售訂單");
                    tb.HasCheckConstraint("CK_sal_order_exchange_rate", "exchange_rate > 0");
                    tb.HasCheckConstraint("CK_sal_order_subtotal_amount", "subtotal_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_discount_amount", "discount_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_promotion_amount", "promotion_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_tax_amount", "tax_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_freight_amount", "freight_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_service_amount", "service_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_other_amount", "other_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_total_amount", "total_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_paid_amount", "paid_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_refunded_amount", "refunded_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_refunded_vs_paid", "refunded_amount <= paid_amount");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("銷售單序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").HasComment("修改日期");
                entity.Property(e => e.SalesOrderNo).HasColumnName("sales_order_no").HasComment("銷售單號");
                entity.Property(e => e.ExternalOrderNo).HasColumnName("external_order_no").HasComment("外部訂單號");
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasComment("公司序號");
                entity.Property(e => e.BusinessUnitSid).HasColumnName("business_unit_sid").HasComment("營運單位序號");
                entity.Property(e => e.DepartmentSid).HasColumnName("department_sid").HasComment("部門序號");
                entity.Property(e => e.OrderTypeSid).HasColumnName("order_type_sid").HasComment("銷售類型序號");
                entity.Property(e => e.CustomerPartySid).HasColumnName("customer_party_sid").HasComment("客戶Party序號");
                entity.Property(e => e.MemberPartySid).HasColumnName("member_party_sid").HasComment("會員Party序號");
                entity.Property(e => e.ContactPartySid).HasColumnName("contact_party_sid").HasComment("聯絡人Party序號");
                entity.Property(e => e.QuoteSid).HasColumnName("quote_sid").HasComment("來源報價單序號");
                entity.Property(e => e.ChannelSid).HasColumnName("channel_sid").HasComment("通路序號");
                entity.Property(e => e.StoreSid).HasColumnName("store_sid").HasComment("商店序號");
                entity.Property(e => e.ProjectSid).HasColumnName("project_sid").HasComment("專案序號");
                entity.Property(e => e.SiteSid).HasColumnName("site_sid").HasComment("工地序號");
                entity.Property(e => e.ContractSid).HasColumnName("contract_sid").HasComment("合約序號");
                entity.Property(e => e.SalesOwnerUserSid).HasColumnName("sales_owner_user_sid").HasComment("負責業務帳號序號");
                entity.Property(e => e.OrderDate).HasColumnName("order_date").HasComment("下單時間");
                entity.Property(e => e.RequestedDeliveryDate).HasColumnName("requested_delivery_date").HasComment("客戶需求交期");
                entity.Property(e => e.CurrencySid).HasColumnName("currency_sid").HasComment("幣別序號");
                entity.Property(e => e.ExchangeRate).HasColumnName("exchange_rate").HasColumnType("decimal(20,10)").HasComment("匯率");
                entity.Property(e => e.PriceListSid).HasColumnName("price_list_sid").HasComment("價格清單序號");
                entity.Property(e => e.PriceLevelSid).HasColumnName("price_level_sid").HasComment("價格層級序號");
                entity.Property(e => e.AgreementSid).HasColumnName("agreement_sid").HasComment("價格協議序號");
                entity.Property(e => e.SubtotalAmount).HasColumnName("subtotal_amount").HasColumnType("decimal(20,4)").HasComment("未稅小計");
                entity.Property(e => e.DiscountAmount).HasColumnName("discount_amount").HasColumnType("decimal(20,4)").HasComment("折扣金額");
                entity.Property(e => e.PromotionAmount).HasColumnName("promotion_amount").HasColumnType("decimal(20,4)").HasComment("活動優惠金額");
                entity.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(20,4)").HasComment("稅額");
                entity.Property(e => e.FreightAmount).HasColumnName("freight_amount").HasColumnType("decimal(20,4)").HasComment("運費");
                entity.Property(e => e.ServiceAmount).HasColumnName("service_amount").HasColumnType("decimal(20,4)").HasComment("服務費");
                entity.Property(e => e.OtherAmount).HasColumnName("other_amount").HasColumnType("decimal(20,4)").HasComment("其他費用");
                entity.Property(e => e.TotalAmount).HasColumnName("total_amount").HasColumnType("decimal(20,4)").HasComment("訂單總額");
                entity.Property(e => e.PaidAmount).HasColumnName("paid_amount").HasColumnType("decimal(20,4)").HasComment("已付款金額");
                entity.Property(e => e.RefundedAmount).HasColumnName("refunded_amount").HasColumnType("decimal(20,4)").HasComment("已退款金額");
                entity.Property(e => e.PaymentTermSid).HasColumnName("payment_term_sid").HasComment("付款條件序號");
                entity.Property(e => e.PaymentMethodSid).HasColumnName("payment_method_sid").HasComment("付款方式序號");
                entity.Property(e => e.BillingAddressSid).HasColumnName("billing_address_sid").HasComment("帳單地址序號");
                entity.Property(e => e.ShippingAddressSid).HasColumnName("shipping_address_sid").HasComment("配送地址序號");
                entity.Property(e => e.InvoiceTitle).HasColumnName("invoice_title").HasComment("發票抬頭");
                entity.Property(e => e.InvoiceTaxNo).HasColumnName("invoice_tax_no").HasComment("發票統編");
                entity.Property(e => e.SourceType).HasColumnName("source_type").HasComment("SHOPPING商城;B2B;QUOTE報價;CRM商機;PROJECT專案;CONTRACT合約;API;MANUAL人工");
                entity.Property(e => e.SourceSid).HasColumnName("source_sid").HasComment("來源資料序號");
                entity.Property(e => e.CreditCheckStatus).HasColumnName("credit_check_status").HasComment("NOT_REQUIRED不需要;PENDING待檢;PASS通過;HOLD凍結;FAIL失敗");
                entity.Property(e => e.PaymentStatus).HasColumnName("payment_status").HasComment("UNPAID未付;PARTIAL部分付款;PAID已付;REFUNDING退款中;PARTIAL_REFUNDED部分退款;REFUNDED已退款");
                entity.Property(e => e.FulfillmentStatus).HasColumnName("fulfillment_status").HasComment("UNFULFILLED未履約;RESERVED已預留;PARTIAL部分履約;FULFILLED已完成;CANCELLED取消");
                entity.Property(e => e.InvoiceStatus).HasColumnName("invoice_status").HasComment("NOT_ISSUED未開;PARTIAL部分開立;ISSUED已開;VOID作廢;ALLOWANCE折讓");
                entity.Property(e => e.OrderStatus).HasColumnName("order_status").HasComment("DRAFT草稿;PENDING_PAYMENT待付款;SUBMITTED已送出;REVIEW待審;APPROVED核准;CONFIRMED已確認;PROCESSING處理中;PARTIAL_FULFILLED部分履約;COMPLETED完成;ON_HOLD暫停;CANCELLED取消;CLOSED結案");
                entity.Property(e => e.WorkflowInstanceSid).HasColumnName("workflow_instance_sid").HasComment("WorkflowDB流程實例序號");
                entity.Property(e => e.ApprovedUserSid).HasColumnName("approved_user_sid").HasComment("核准人員序號");
                entity.Property(e => e.ApprovedDate).HasColumnName("approved_date").HasComment("核准時間");
                entity.Property(e => e.ConfirmedDate).HasColumnName("confirmed_date").HasComment("確認時間");
                entity.Property(e => e.CompletedDate).HasColumnName("completed_date").HasComment("完成時間");
                entity.Property(e => e.CancelReasonCode).HasColumnName("cancel_reason_code").HasComment("取消原因代碼");
                entity.Property(e => e.CancelReason).HasColumnName("cancel_reason").HasComment("取消原因");
                entity.Property(e => e.VersionNo).HasColumnName("version_no").HasComment("樂觀鎖版本");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasComment("Y可用;D刪除;W停用");
                entity.Property(e => e.Remark).HasColumnName("remark").HasComment("備註");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SalesOrderNo).IsUnique().HasDatabaseName("uk_so_sales_order_no");
                entity.HasIndex(e => e.ExternalOrderNo).HasDatabaseName("idx_so_external_order_no");
                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_so_company_sid");
                entity.HasIndex(e => e.OrderTypeSid).HasDatabaseName("idx_so_order_type_sid");
                entity.HasIndex(e => e.CustomerPartySid).HasDatabaseName("idx_so_customer_party_sid");
                entity.HasIndex(e => e.MemberPartySid).HasDatabaseName("idx_so_member_party_sid");
                entity.HasIndex(e => e.QuoteSid).HasDatabaseName("idx_so_quote_sid");
                entity.HasIndex(e => e.ChannelSid).HasDatabaseName("idx_so_channel_sid");
                entity.HasIndex(e => e.StoreSid).HasDatabaseName("idx_so_store_sid");
                entity.HasIndex(e => e.ProjectSid).HasDatabaseName("idx_so_project_sid");
                entity.HasIndex(e => e.SiteSid).HasDatabaseName("idx_so_site_sid");
                entity.HasIndex(e => e.ContractSid).HasDatabaseName("idx_so_contract_sid");
                entity.HasIndex(e => e.OrderDate).HasDatabaseName("idx_so_order_date");
                entity.HasIndex(e => e.RequestedDeliveryDate).HasDatabaseName("idx_so_requested_delivery_date");
                entity.HasIndex(e => e.PaymentStatus).HasDatabaseName("idx_so_payment_status");
                entity.HasIndex(e => e.FulfillmentStatus).HasDatabaseName("idx_so_fulfillment_status");
                entity.HasIndex(e => e.InvoiceStatus).HasDatabaseName("idx_so_invoice_status");
                entity.HasIndex(e => e.OrderStatus).HasDatabaseName("idx_so_order_status");
                entity.HasIndex(e => e.WorkflowInstanceSid).HasDatabaseName("idx_so_workflow_instance_sid");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_so_avalible");
            });

            // 03. sal_order_item
            modelBuilder.Entity<SalOrderItem>(entity =>
            {
                entity.ToTable("sal_order_item", tb => {
                    tb.HasComment("共用銷售訂單明細");
                    tb.HasCheckConstraint("CK_sal_order_item_line_no", "line_no > 0");
                    tb.HasCheckConstraint("CK_sal_order_item_quantity", "quantity > 0");
                    tb.HasCheckConstraint("CK_sal_order_item_list_price", "list_price >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_base_price", "base_price >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_unit_price", "unit_price >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_discount_rate", "discount_rate >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_discount_amount", "discount_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_promotion_amount", "promotion_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_tax_rate", "tax_rate >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_tax_amount", "tax_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_line_amount", "line_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_reserved_qty", "reserved_qty >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_fulfilled_qty", "fulfilled_qty >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_invoiced_qty", "invoiced_qty >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_returned_qty", "returned_qty >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_cancelled_qty", "cancelled_qty >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_backorder_qty", "backorder_qty >= 0");
                    tb.HasCheckConstraint("CK_sal_order_item_fulfilled_cancelled_qty", "fulfilled_qty + cancelled_qty <= quantity");
                    tb.HasCheckConstraint("CK_sal_order_item_returned_vs_fulfilled", "returned_qty <= fulfilled_qty");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("銷售單明細序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").HasComment("修改日期");
                entity.Property(e => e.SalesOrderNid).HasColumnName("sales_order_nid").HasComment("銷售單流水號");
                entity.Property(e => e.LineNo).HasColumnName("line_no").HasComment("明細行號");
                entity.Property(e => e.QuoteItemSid).HasColumnName("quote_item_sid").HasComment("來源報價明細序號");
                entity.Property(e => e.ItemSid).HasColumnName("item_sid").HasComment("Item序號");
                entity.Property(e => e.VariantSid).HasColumnName("variant_sid").HasComment("變體序號");
                entity.Property(e => e.ItemType).HasColumnName("item_type").HasComment("PRODUCT商品;MATERIAL材料;EQUIPMENT設備;SERVICE服務;BUNDLE組合;WORK_ITEM工項;OTHER");
                entity.Property(e => e.ItemDescription).HasColumnName("item_description").HasComment("品名或服務說明");
                entity.Property(e => e.SpecificationText).HasColumnName("specification_text").HasComment("規格說明");
                entity.Property(e => e.Quantity).HasColumnName("quantity").HasColumnType("decimal(20,6)").HasComment("訂購數量");
                entity.Property(e => e.UnitSid).HasColumnName("unit_sid").HasComment("單位序號");
                entity.Property(e => e.ListPrice).HasColumnName("list_price").HasColumnType("decimal(20,6)").HasComment("牌價");
                entity.Property(e => e.BasePrice).HasColumnName("base_price").HasColumnType("decimal(20,6)").HasComment("基礎價格");
                entity.Property(e => e.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(20,6)").HasComment("成交單價");
                entity.Property(e => e.DiscountRate).HasColumnName("discount_rate").HasColumnType("decimal(8,4)").HasComment("折扣率");
                entity.Property(e => e.DiscountAmount).HasColumnName("discount_amount").HasColumnType("decimal(20,4)").HasComment("折扣金額");
                entity.Property(e => e.PromotionAmount).HasColumnName("promotion_amount").HasColumnType("decimal(20,4)").HasComment("活動優惠金額");
                entity.Property(e => e.TaxSid).HasColumnName("tax_sid").HasComment("稅別序號");
                entity.Property(e => e.TaxRate).HasColumnName("tax_rate").HasColumnType("decimal(8,4)").HasComment("稅率快照");
                entity.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(20,4)").HasComment("稅額");
                entity.Property(e => e.LineAmount).HasColumnName("line_amount").HasColumnType("decimal(20,4)").HasComment("明細總額");
                entity.Property(e => e.RequestedDeliveryDate).HasColumnName("requested_delivery_date").HasComment("需求交期");
                entity.Property(e => e.WarehouseSid).HasColumnName("warehouse_sid").HasComment("履約倉庫序號");
                entity.Property(e => e.ProjectSid).HasColumnName("project_sid").HasComment("專案序號");
                entity.Property(e => e.SiteSid).HasColumnName("site_sid").HasComment("工地序號");
                entity.Property(e => e.WbsSid).HasColumnName("wbs_sid").HasComment("WBS序號");
                entity.Property(e => e.ContractItemSid).HasColumnName("contract_item_sid").HasComment("合約明細序號");
                entity.Property(e => e.PricingResultSid).HasColumnName("pricing_result_sid").HasComment("PricingDB計價結果序號");
                entity.Property(e => e.PriceSnapshot).HasColumnName("price_snapshot").HasColumnType("json").HasComment("價格快照");
                entity.Property(e => e.ItemSnapshot).HasColumnName("item_snapshot").HasColumnType("json").HasComment("Item名稱、規格與屬性快照");
                entity.Property(e => e.ReservedQty).HasColumnName("reserved_qty").HasColumnType("decimal(20,6)").HasComment("已預留數量");
                entity.Property(e => e.FulfilledQty).HasColumnName("fulfilled_qty").HasColumnType("decimal(20,6)").HasComment("已履約數量");
                entity.Property(e => e.InvoicedQty).HasColumnName("invoiced_qty").HasColumnType("decimal(20,6)").HasComment("已開票數量");
                entity.Property(e => e.ReturnedQty).HasColumnName("returned_qty").HasColumnType("decimal(20,6)").HasComment("已退貨數量");
                entity.Property(e => e.CancelledQty).HasColumnName("cancelled_qty").HasColumnType("decimal(20,6)").HasComment("取消數量");
                entity.Property(e => e.BackorderQty).HasColumnName("backorder_qty").HasColumnType("decimal(20,6)").HasComment("缺貨待補數量");
                entity.Property(e => e.ItemStatus).HasColumnName("item_status").HasComment("OPEN待處理;RESERVED已預留;PARTIAL部分履約;FULFILLED已履約;BACKORDER缺貨待補;RETURNED已退貨;CANCELLED取消;CLOSED結束");
                entity.Property(e => e.Remark).HasColumnName("remark").HasComment("備註");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.SalesOrderNid, e.LineNo }).IsUnique().HasDatabaseName("uk_soi_order_line");
                entity.HasIndex(e => e.SalesOrderNid).HasDatabaseName("idx_soi_order_nid");
                entity.HasIndex(e => e.QuoteItemSid).HasDatabaseName("idx_soi_quote_item_sid");
                entity.HasIndex(e => e.ItemSid).HasDatabaseName("idx_soi_item_sid");
                entity.HasIndex(e => e.VariantSid).HasDatabaseName("idx_soi_variant_sid");
                entity.HasIndex(e => e.ItemType).HasDatabaseName("idx_soi_item_type");
                entity.HasIndex(e => e.WarehouseSid).HasDatabaseName("idx_soi_warehouse_sid");
                entity.HasIndex(e => e.ProjectSid).HasDatabaseName("idx_soi_project_sid");
                entity.HasIndex(e => e.SiteSid).HasDatabaseName("idx_soi_site_sid");
                entity.HasIndex(e => e.WbsSid).HasDatabaseName("idx_soi_wbs_sid");
                entity.HasIndex(e => e.ContractItemSid).HasDatabaseName("idx_soi_contract_item_sid");
                entity.HasIndex(e => e.PricingResultSid).HasDatabaseName("idx_soi_pricing_result_sid");
                entity.HasIndex(e => e.RequestedDeliveryDate).HasDatabaseName("idx_soi_requested_delivery_date");
                entity.HasIndex(e => e.ItemStatus).HasDatabaseName("idx_soi_status");

                entity.HasOne(d => d.SalesOrder)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.SalesOrderNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_soi_order");
            });

            // 04. sal_delivery_schedule
            modelBuilder.Entity<SalDeliverySchedule>(entity =>
            {
                entity.ToTable("sal_delivery_schedule", tb => {
                    tb.HasComment("銷售交期與履約排程");
                    tb.HasCheckConstraint("CK_sal_delivery_schedule_schedule_no", "schedule_no > 0");
                    tb.HasCheckConstraint("CK_sal_delivery_schedule_scheduled_qty", "scheduled_qty > 0");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("交期排程序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").HasComment("修改日期");
                entity.Property(e => e.SalesOrderItemNid).HasColumnName("sales_order_item_nid").HasComment("銷售明細流水號");
                entity.Property(e => e.ScheduleNo).HasColumnName("schedule_no").HasComment("排程序號");
                entity.Property(e => e.ScheduledQty).HasColumnName("scheduled_qty").HasColumnType("decimal(20,6)").HasComment("排程數量");
                entity.Property(e => e.PlannedDeliveryDate).HasColumnName("planned_delivery_date").HasComment("計畫交貨日");
                entity.Property(e => e.ConfirmedDeliveryDate).HasColumnName("confirmed_delivery_date").HasComment("確認交貨日");
                entity.Property(e => e.ActualDeliveryDate).HasColumnName("actual_delivery_date").HasComment("實際交貨日");
                entity.Property(e => e.WarehouseSid).HasColumnName("warehouse_sid").HasComment("履約倉庫序號");
                entity.Property(e => e.ShippingAddressSid).HasColumnName("shipping_address_sid").HasComment("配送地址序號");
                entity.Property(e => e.FulfillmentMethod).HasColumnName("fulfillment_method").HasComment("DELIVERY配送;PICKUP自取;DIGITAL數位;SERVICE服務;PROJECT_SITE工地交付");
                entity.Property(e => e.ScheduleStatus).HasColumnName("schedule_status").HasComment("PLANNED計畫;CONFIRMED確認;RESERVED已預留;PARTIAL部分履約;FULFILLED完成;DELAYED延遲;CANCELLED取消");
                entity.Property(e => e.DelayReason).HasColumnName("delay_reason").HasComment("延遲原因");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.SalesOrderItemNid, e.ScheduleNo }).IsUnique().HasDatabaseName("uk_sds_item_schedule");
                entity.HasIndex(e => e.SalesOrderItemNid).HasDatabaseName("idx_sds_order_item_nid");
                entity.HasIndex(e => e.PlannedDeliveryDate).HasDatabaseName("idx_sds_planned_delivery_date");
                entity.HasIndex(e => e.ConfirmedDeliveryDate).HasDatabaseName("idx_sds_confirmed_delivery_date");
                entity.HasIndex(e => e.WarehouseSid).HasDatabaseName("idx_sds_warehouse_sid");
                entity.HasIndex(e => e.FulfillmentMethod).HasDatabaseName("idx_sds_fulfillment_method");
                entity.HasIndex(e => e.ScheduleStatus).HasDatabaseName("idx_sds_status");

                entity.HasOne(d => d.SalesOrderItem)
                    .WithMany(p => p.DeliverySchedules)
                    .HasForeignKey(d => d.SalesOrderItemNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_sds_order_item");
            });

            // 04. sal_fulfillment_request
            modelBuilder.Entity<SalFulfillmentRequest>(entity =>
            {
                entity.ToTable("sal_fulfillment_request", tb => tb.HasComment("銷售訂單履約申請"));
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("履約申請序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").HasComment("修改日期");
                entity.Property(e => e.RequestNo).HasColumnName("request_no").HasComment("履約申請編號");
                entity.Property(e => e.SalesOrderSid).HasColumnName("sales_order_sid").HasComment("銷售單序號");
                entity.Property(e => e.FulfillmentType).HasColumnName("fulfillment_type").HasComment("SHIPMENT出貨;PICKUP自取;DIGITAL數位;SERVICE服務;PROJECT_DELIVERY專案交付;INSTALLATION安裝");
                entity.Property(e => e.WarehouseSid).HasColumnName("warehouse_sid").HasComment("履約倉庫序號");
                entity.Property(e => e.ProjectSid).HasColumnName("project_sid").HasComment("專案序號");
                entity.Property(e => e.SiteSid).HasColumnName("site_sid").HasComment("工地序號");
                entity.Property(e => e.ShippingAddressSid).HasColumnName("shipping_address_sid").HasComment("配送地址序號");
                entity.Property(e => e.RequestedDate).HasColumnName("requested_date").HasComment("申請時間");
                entity.Property(e => e.RequiredDate).HasColumnName("required_date").HasComment("要求完成時間");
                entity.Property(e => e.ReservationSid).HasColumnName("reservation_sid").HasComment("InventoryDB庫存預留序號");
                entity.Property(e => e.FulfillmentOrderSid).HasColumnName("fulfillment_order_sid").HasComment("FulfillmentDB履約單序號");
                entity.Property(e => e.RequestStatus).HasColumnName("request_status").HasComment("PENDING待處理;RESERVED已預留;ACCEPTED已接受;PROCESSING處理中;PARTIAL部分完成;COMPLETED完成;FAILED失敗;CANCELLED取消");
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasComment("跨服務關聯識別碼");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message").HasComment("錯誤訊息");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.RequestNo).IsUnique().HasDatabaseName("uk_sfr_request_no");
                entity.HasIndex(e => e.SalesOrderSid).HasDatabaseName("idx_sfr_sales_order_sid");
                entity.HasIndex(e => e.FulfillmentType).HasDatabaseName("idx_sfr_fulfillment_type");
                entity.HasIndex(e => e.WarehouseSid).HasDatabaseName("idx_sfr_warehouse_sid");
                entity.HasIndex(e => e.ProjectSid).HasDatabaseName("idx_sfr_project_sid");
                entity.HasIndex(e => e.SiteSid).HasDatabaseName("idx_sfr_site_sid");
                entity.HasIndex(e => e.ReservationSid).HasDatabaseName("idx_sfr_reservation_sid");
                entity.HasIndex(e => e.FulfillmentOrderSid).HasDatabaseName("idx_sfr_fulfillment_order_sid");
                entity.HasIndex(e => e.RequestStatus).HasDatabaseName("idx_sfr_status");
                entity.HasIndex(e => e.CorrelationId).HasDatabaseName("idx_sfr_correlation_id");
            });

            // 04. sal_fulfillment_request_item
            modelBuilder.Entity<SalFulfillmentRequestItem>(entity =>
            {
                entity.ToTable("sal_fulfillment_request_item", tb => {
                    tb.HasComment("銷售履約申請明細");
                    tb.HasCheckConstraint("CK_sal_fulfillment_request_item_line_no", "line_no > 0");
                    tb.HasCheckConstraint("CK_sal_fulfillment_request_item_request_qty", "request_qty > 0");
                    tb.HasCheckConstraint("CK_sal_fulfillment_request_item_fulfilled_qty", "fulfilled_qty >= 0");
                    tb.HasCheckConstraint("CK_sal_fulfillment_request_item_fulfilled_vs_request", "fulfilled_qty <= request_qty");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("履約申請明細序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.FulfillmentRequestNid).HasColumnName("fulfillment_request_nid").HasComment("履約申請流水號");
                entity.Property(e => e.LineNo).HasColumnName("line_no").HasComment("明細行號");
                entity.Property(e => e.SalesOrderItemSid).HasColumnName("sales_order_item_sid").HasComment("銷售明細序號");
                entity.Property(e => e.DeliveryScheduleSid).HasColumnName("delivery_schedule_sid").HasComment("交期排程序號");
                entity.Property(e => e.RequestQty).HasColumnName("request_qty").HasColumnType("decimal(20,6)").HasComment("履約數量");
                entity.Property(e => e.FulfilledQty).HasColumnName("fulfilled_qty").HasColumnType("decimal(20,6)").HasComment("已履約數量");
                entity.Property(e => e.UnitSid).HasColumnName("unit_sid").HasComment("單位序號");
                entity.Property(e => e.ItemStatus).HasColumnName("item_status").HasComment("PENDING待處理;RESERVED已預留;PROCESSING處理中;PARTIAL部分完成;COMPLETED完成;FAILED失敗;CANCELLED取消");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.FulfillmentRequestNid, e.LineNo }).IsUnique().HasDatabaseName("uk_sfri_request_line");
                entity.HasIndex(e => e.FulfillmentRequestNid).HasDatabaseName("idx_sfri_request_nid");
                entity.HasIndex(e => e.SalesOrderItemSid).HasDatabaseName("idx_sfri_sales_order_item_sid");
                entity.HasIndex(e => e.DeliveryScheduleSid).HasDatabaseName("idx_sfri_delivery_schedule_sid");
                entity.HasIndex(e => e.ItemStatus).HasDatabaseName("idx_sfri_status");

                entity.HasOne(d => d.FulfillmentRequest)
                    .WithMany(p => p.RequestItems)
                    .HasForeignKey(d => d.FulfillmentRequestNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_sfri_request");
            });

            // 05. sal_order_address
            modelBuilder.Entity<SalOrderAddress>(entity =>
            {
                entity.ToTable("sal_order_address", tb => tb.HasComment("訂單地址快照"));
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("訂單地址序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.SalesOrderNid).HasColumnName("sales_order_nid").HasComment("銷售單流水號");
                entity.Property(e => e.AddressType).HasColumnName("address_type").HasComment("BILLING帳單;SHIPPING配送;CONTACT聯絡;PROJECT_SITE工地");
                entity.Property(e => e.SourceAddressSid).HasColumnName("source_address_sid").HasComment("PartyDB或MasterDB來源地址序號");
                entity.Property(e => e.RecipientName).HasColumnName("recipient_name").HasComment("收件人");
                entity.Property(e => e.RecipientPhone).HasColumnName("recipient_phone").HasComment("收件電話");
                entity.Property(e => e.CountrySid).HasColumnName("country_sid").HasComment("國家序號");
                entity.Property(e => e.RegionLevel1Sid).HasColumnName("region_level1_sid").HasComment("第一層行政區序號");
                entity.Property(e => e.RegionLevel2Sid).HasColumnName("region_level2_sid").HasComment("第二層行政區序號");
                entity.Property(e => e.RegionLevel3Sid).HasColumnName("region_level3_sid").HasComment("第三層行政區序號");
                entity.Property(e => e.PostalCode).HasColumnName("postal_code").HasComment("郵遞區號");
                entity.Property(e => e.AddressLine1).HasColumnName("address_line1").HasComment("主要地址");
                entity.Property(e => e.AddressLine2).HasColumnName("address_line2").HasComment("補充地址");
                entity.Property(e => e.Latitude).HasColumnName("latitude").HasColumnType("decimal(12,8)").HasComment("緯度");
                entity.Property(e => e.Longitude).HasColumnName("longitude").HasColumnType("decimal(12,8)").HasComment("經度");
                entity.Property(e => e.DeliveryNote).HasColumnName("delivery_note").HasComment("配送備註");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.SalesOrderNid, e.AddressType }).IsUnique().HasDatabaseName("uk_soa_order_address_type");
                entity.HasIndex(e => e.SalesOrderNid).HasDatabaseName("idx_soa_order_nid");
                entity.HasIndex(e => e.AddressType).HasDatabaseName("idx_soa_address_type");
                entity.HasIndex(e => e.SourceAddressSid).HasDatabaseName("idx_soa_source_address_sid");
                entity.HasIndex(e => e.CountrySid).HasDatabaseName("idx_soa_country_sid");

                entity.HasOne(d => d.SalesOrder)
                    .WithMany(p => p.OrderAddresses)
                    .HasForeignKey(d => d.SalesOrderNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_soa_order");
            });

            // 05. sal_order_contact
            modelBuilder.Entity<SalOrderContact>(entity =>
            {
                entity.ToTable("sal_order_contact", tb => tb.HasComment("訂單聯絡人快照"));
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("訂單聯絡人序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.SalesOrderNid).HasColumnName("sales_order_nid").HasComment("銷售單流水號");
                entity.Property(e => e.ContactType).HasColumnName("contact_type").HasComment("ORDER下單;BILLING帳務;SHIPPING配送;PROJECT專案;EMERGENCY緊急");
                entity.Property(e => e.SourcePartySid).HasColumnName("source_party_sid").HasComment("PartyDB來源Party序號");
                entity.Property(e => e.ContactName).HasColumnName("contact_name").HasComment("聯絡人姓名");
                entity.Property(e => e.ContactEmail).HasColumnName("contact_email").HasComment("Email");
                entity.Property(e => e.ContactMobile).HasColumnName("contact_mobile").HasComment("手機");
                entity.Property(e => e.ContactPhone).HasColumnName("contact_phone").HasComment("電話");
                entity.Property(e => e.DepartmentName).HasColumnName("department_name").HasComment("部門名稱快照");
                entity.Property(e => e.JobTitle).HasColumnName("job_title").HasComment("職稱快照");
                entity.Property(e => e.PrimaryMark).HasColumnName("primary_mark").HasComment("是否主要聯絡人");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.SalesOrderNid, e.ContactType, e.ContactName }).IsUnique().HasDatabaseName("uk_soc_order_contact");
                entity.HasIndex(e => e.SalesOrderNid).HasDatabaseName("idx_soc_order_nid");
                entity.HasIndex(e => e.ContactType).HasDatabaseName("idx_soc_contact_type");
                entity.HasIndex(e => e.SourcePartySid).HasDatabaseName("idx_soc_source_party_sid");
                entity.HasIndex(e => e.PrimaryMark).HasDatabaseName("idx_soc_primary_mark");

                entity.HasOne(d => d.SalesOrder)
                    .WithMany(p => p.OrderContacts)
                    .HasForeignKey(d => d.SalesOrderNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_soc_order");
            });

            // 06. sal_payment_request
            modelBuilder.Entity<SalPaymentRequest>(entity =>
            {
                entity.ToTable("sal_payment_request", tb => {
                    tb.HasComment("銷售訂單付款請求");
                    tb.HasCheckConstraint("CK_sal_payment_request_requested_amount", "requested_amount > 0");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("付款請求序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").HasComment("修改日期");
                entity.Property(e => e.PaymentRequestNo).HasColumnName("payment_request_no").HasComment("付款請求編號");
                entity.Property(e => e.SalesOrderSid).HasColumnName("sales_order_sid").HasComment("銷售單序號");
                entity.Property(e => e.PaymentMethodSid).HasColumnName("payment_method_sid").HasComment("付款方式序號");
                entity.Property(e => e.CurrencySid).HasColumnName("currency_sid").HasComment("幣別序號");
                entity.Property(e => e.RequestedAmount).HasColumnName("requested_amount").HasColumnType("decimal(20,4)").HasComment("請求付款金額");
                entity.Property(e => e.DueDate).HasColumnName("due_date").HasComment("付款期限");
                entity.Property(e => e.ExternalPaymentSid).HasColumnName("external_payment_sid").HasComment("PaymentDB付款單序號");
                entity.Property(e => e.VirtualAccountNo).HasColumnName("virtual_account_no").HasComment("虛擬帳號");
                entity.Property(e => e.PaymentUrl).HasColumnName("payment_url").HasComment("付款網址");
                entity.Property(e => e.RequestStatus).HasColumnName("request_status").HasComment("PENDING待付款;PROCESSING處理中;PAID已付款;FAILED失敗;EXPIRED過期;CANCELLED取消");
                entity.Property(e => e.PaidDate).HasColumnName("paid_date").HasComment("付款完成時間");
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasComment("跨服務關聯識別碼");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message").HasComment("錯誤訊息");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.PaymentRequestNo).IsUnique().HasDatabaseName("uk_spr_payment_request_no");
                entity.HasIndex(e => e.SalesOrderSid).HasDatabaseName("idx_spr_sales_order_sid");
                entity.HasIndex(e => e.PaymentMethodSid).HasDatabaseName("idx_spr_payment_method_sid");
                entity.HasIndex(e => e.ExternalPaymentSid).HasDatabaseName("idx_spr_external_payment_sid");
                entity.HasIndex(e => e.DueDate).HasDatabaseName("idx_spr_due_date");
                entity.HasIndex(e => e.RequestStatus).HasDatabaseName("idx_spr_status");
                entity.HasIndex(e => e.CorrelationId).HasDatabaseName("idx_spr_correlation_id");
            });

            // 06. sal_receivable_request
            modelBuilder.Entity<SalReceivableRequest>(entity =>
            {
                entity.ToTable("sal_receivable_request", tb => {
                    tb.HasComment("銷售訂單應收帳款建立請求");
                    tb.HasCheckConstraint("CK_sal_receivable_request_receivable_amount", "receivable_amount > 0");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("應收建立請求序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.SalesOrderSid).HasColumnName("sales_order_sid").HasComment("銷售單序號");
                entity.Property(e => e.RequestType).HasColumnName("request_type").HasComment("DEPOSIT訂金;MILESTONE里程碑;DELIVERY交貨;INVOICE開票;FINAL尾款;ADJUSTMENT調整");
                entity.Property(e => e.ReferenceType).HasColumnName("reference_type").HasComment("來源類型");
                entity.Property(e => e.ReferenceSid).HasColumnName("reference_sid").HasComment("來源資料序號");
                entity.Property(e => e.CurrencySid).HasColumnName("currency_sid").HasComment("幣別序號");
                entity.Property(e => e.ReceivableAmount).HasColumnName("receivable_amount").HasColumnType("decimal(20,4)").HasComment("應收金額");
                entity.Property(e => e.DueDate).HasColumnName("due_date").HasComment("到期日");
                entity.Property(e => e.AccountingReceivableSid).HasColumnName("accounting_receivable_sid").HasComment("AccountingDB應收單序號");
                entity.Property(e => e.RequestStatus).HasColumnName("request_status").HasComment("PENDING待處理;PROCESSING處理中;SUCCESS成功;FAILED失敗;CANCELLED取消");
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasComment("跨服務關聯識別碼");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message").HasComment("錯誤訊息");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SalesOrderSid).HasDatabaseName("idx_srr_sales_order_sid");
                entity.HasIndex(e => e.RequestType).HasDatabaseName("idx_srr_request_type");
                entity.HasIndex(e => new { e.ReferenceType, e.ReferenceSid }).HasDatabaseName("idx_srr_reference");
                entity.HasIndex(e => e.AccountingReceivableSid).HasDatabaseName("idx_srr_accounting_receivable_sid");
                entity.HasIndex(e => e.RequestStatus).HasDatabaseName("idx_srr_status");
                entity.HasIndex(e => e.CorrelationId).HasDatabaseName("idx_srr_correlation_id");
            });

            // 07. sal_invoice_request
            modelBuilder.Entity<SalInvoiceRequest>(entity =>
            {
                entity.ToTable("sal_invoice_request", tb => {
                    tb.HasComment("銷售訂單發票建立請求");
                    tb.HasCheckConstraint("CK_sal_invoice_request_invoice_amount", "invoice_amount > 0");
                    tb.HasCheckConstraint("CK_sal_invoice_request_tax_amount", "tax_amount >= 0");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("發票建立請求序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").HasComment("修改日期");
                entity.Property(e => e.InvoiceRequestNo).HasColumnName("invoice_request_no").HasComment("發票請求編號");
                entity.Property(e => e.SalesOrderSid).HasColumnName("sales_order_sid").HasComment("銷售單序號");
                entity.Property(e => e.RequestType).HasColumnName("request_type").HasComment("FULL全額;PARTIAL部分;DEPOSIT訂金;MILESTONE里程碑;FINAL尾款");
                entity.Property(e => e.InvoiceDate).HasColumnName("invoice_date").HasComment("預計開票日");
                entity.Property(e => e.InvoiceTitle).HasColumnName("invoice_title").HasComment("發票抬頭");
                entity.Property(e => e.InvoiceTaxNo).HasColumnName("invoice_tax_no").HasComment("統一編號");
                entity.Property(e => e.CarrierType).HasColumnName("carrier_type").HasComment("MOBILE手機條碼;CERT自然人憑證;DONATE捐贈;MEMBER會員載具;NONE無");
                entity.Property(e => e.CarrierNo).HasColumnName("carrier_no").HasComment("載具號碼");
                entity.Property(e => e.DonationCode).HasColumnName("donation_code").HasComment("捐贈碼");
                entity.Property(e => e.CurrencySid).HasColumnName("currency_sid").HasComment("幣別序號");
                entity.Property(e => e.InvoiceAmount).HasColumnName("invoice_amount").HasColumnType("decimal(20,4)").HasComment("開票金額");
                entity.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(20,4)").HasComment("稅額");
                entity.Property(e => e.InvoiceTaxSid).HasColumnName("invoice_tax_sid").HasComment("InvoiceTaxDB發票序號");
                entity.Property(e => e.RequestStatus).HasColumnName("request_status").HasComment("PENDING待處理;PROCESSING處理中;ISSUED已開立;FAILED失敗;CANCELLED取消");
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasComment("跨服務關聯識別碼");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message").HasComment("錯誤訊息");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.InvoiceRequestNo).IsUnique().HasDatabaseName("uk_sinv_invoice_request_no");
                entity.HasIndex(e => e.SalesOrderSid).HasDatabaseName("idx_sinv_sales_order_sid");
                entity.HasIndex(e => e.RequestType).HasDatabaseName("idx_sinv_request_type");
                entity.HasIndex(e => e.InvoiceDate).HasDatabaseName("idx_sinv_invoice_date");
                entity.HasIndex(e => e.InvoiceTaxSid).HasDatabaseName("idx_sinv_invoice_tax_sid");
                entity.HasIndex(e => e.RequestStatus).HasDatabaseName("idx_sinv_status");
                entity.HasIndex(e => e.CorrelationId).HasDatabaseName("idx_sinv_correlation_id");
            });

            // 07. sal_invoice_request_item
            modelBuilder.Entity<SalInvoiceRequestItem>(entity =>
            {
                entity.ToTable("sal_invoice_request_item", tb => {
                    tb.HasComment("銷售訂單發票請求明細");
                    tb.HasCheckConstraint("CK_sal_invoice_request_item_line_no", "line_no > 0");
                    tb.HasCheckConstraint("CK_sal_invoice_request_item_invoice_qty", "invoice_qty > 0");
                    tb.HasCheckConstraint("CK_sal_invoice_request_item_unit_price", "unit_price >= 0");
                    tb.HasCheckConstraint("CK_sal_invoice_request_item_tax_rate", "tax_rate >= 0");
                    tb.HasCheckConstraint("CK_sal_invoice_request_item_tax_amount", "tax_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_invoice_request_item_line_amount", "line_amount >= 0");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("發票請求明細序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.InvoiceRequestNid).HasColumnName("invoice_request_nid").HasComment("發票請求流水號");
                entity.Property(e => e.LineNo).HasColumnName("line_no").HasComment("明細行號");
                entity.Property(e => e.SalesOrderItemSid).HasColumnName("sales_order_item_sid").HasComment("銷售單明細序號");
                entity.Property(e => e.InvoiceQty).HasColumnName("invoice_qty").HasColumnType("decimal(20,6)").HasComment("開票數量");
                entity.Property(e => e.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(20,6)").HasComment("開票單價");
                entity.Property(e => e.TaxRate).HasColumnName("tax_rate").HasColumnType("decimal(8,4)").HasComment("稅率");
                entity.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(20,4)").HasComment("稅額");
                entity.Property(e => e.LineAmount).HasColumnName("line_amount").HasColumnType("decimal(20,4)").HasComment("開票金額");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.InvoiceRequestNid, e.LineNo }).IsUnique().HasDatabaseName("uk_sinvi_request_line");
                entity.HasIndex(e => e.InvoiceRequestNid).HasDatabaseName("idx_sinvi_request_nid");
                entity.HasIndex(e => e.SalesOrderItemSid).HasDatabaseName("idx_sinvi_sales_order_item_sid");

                entity.HasOne(d => d.InvoiceRequest)
                    .WithMany(p => p.InvoiceRequestItems)
                    .HasForeignKey(d => d.InvoiceRequestNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_sinvi_request");
            });

            // 08. sal_order_change
            modelBuilder.Entity<SalOrderChange>(entity =>
            {
                entity.ToTable("sal_order_change", tb => tb.HasComment("銷售訂單變更申請"));
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("銷售單變更序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").HasComment("修改日期");
                entity.Property(e => e.ChangeNo).HasColumnName("change_no").HasComment("變更單號");
                entity.Property(e => e.SalesOrderSid).HasColumnName("sales_order_sid").HasComment("銷售單序號");
                entity.Property(e => e.ChangeType).HasColumnName("change_type").HasComment("QUANTITY數量;PRICE價格;DELIVERY交期;ADDRESS地址;ITEM品項;PAYMENT付款;CANCEL取消;OTHER");
                entity.Property(e => e.BeforeData).HasColumnName("before_data").HasColumnType("json").HasComment("變更前資料");
                entity.Property(e => e.RequestedData).HasColumnName("requested_data").HasColumnType("json").HasComment("申請變更資料");
                entity.Property(e => e.ChangedFields).HasColumnName("changed_fields").HasColumnType("json").HasComment("變更欄位");
                entity.Property(e => e.AmountDifference).HasColumnName("amount_difference").HasColumnType("decimal(20,4)").HasComment("金額差異");
                entity.Property(e => e.ChangeReason).HasColumnName("change_reason").HasComment("變更原因");
                entity.Property(e => e.RequesterType).HasColumnName("requester_type").HasComment("CUSTOMER客戶;USER內部人員;SYSTEM系統");
                entity.Property(e => e.RequesterSid).HasColumnName("requester_sid").HasComment("申請對象序號");
                entity.Property(e => e.ChangeStatus).HasColumnName("change_status").HasComment("DRAFT草稿;SUBMITTED已送出;REVIEW待審;APPROVED核准;REJECTED駁回;APPLIED已套用;CANCELLED取消");
                entity.Property(e => e.WorkflowInstanceSid).HasColumnName("workflow_instance_sid").HasComment("WorkflowDB流程實例序號");
                entity.Property(e => e.ApprovedUserSid).HasColumnName("approved_user_sid").HasComment("核准人員序號");
                entity.Property(e => e.ApprovedDate).HasColumnName("approved_date").HasComment("核准時間");
                entity.Property(e => e.AppliedDate).HasColumnName("applied_date").HasComment("套用時間");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ChangeNo).IsUnique().HasDatabaseName("uk_socg_change_no");
                entity.HasIndex(e => e.SalesOrderSid).HasDatabaseName("idx_socg_sales_order_sid");
                entity.HasIndex(e => e.ChangeType).HasDatabaseName("idx_socg_change_type");
                entity.HasIndex(e => new { e.RequesterType, e.RequesterSid }).HasDatabaseName("idx_socg_requester");
                entity.HasIndex(e => e.ChangeStatus).HasDatabaseName("idx_socg_status");
                entity.HasIndex(e => e.WorkflowInstanceSid).HasDatabaseName("idx_socg_workflow_instance_sid");
            });

            // 08. sal_cancellation
            modelBuilder.Entity<SalCancellation>(entity =>
            {
                entity.ToTable("sal_cancellation", tb => tb.HasComment("銷售訂單取消流程"));
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("訂單取消序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").HasComment("修改日期");
                entity.Property(e => e.CancellationNo).HasColumnName("cancellation_no").HasComment("取消申請編號");
                entity.Property(e => e.SalesOrderSid).HasColumnName("sales_order_sid").HasComment("銷售單序號");
                entity.Property(e => e.CancellationType).HasColumnName("cancellation_type").HasComment("FULL整單;PARTIAL部分");
                entity.Property(e => e.ReasonCode).HasColumnName("reason_code").HasComment("取消原因代碼");
                entity.Property(e => e.Reason).HasColumnName("reason").HasComment("取消原因");
                entity.Property(e => e.RequestedByType).HasColumnName("requested_by_type").HasComment("CUSTOMER客戶;USER內部人員;SYSTEM系統");
                entity.Property(e => e.RequestedBySid).HasColumnName("requested_by_sid").HasComment("申請對象序號");
                entity.Property(e => e.RefundRequired).HasColumnName("refund_required").HasComment("是否需要退款");
                entity.Property(e => e.RestockRequired).HasColumnName("restock_required").HasComment("是否需要回補庫存");
                entity.Property(e => e.CancellationStatus).HasColumnName("cancellation_status").HasComment("PENDING待處理;REVIEW待審;APPROVED核准;REJECTED駁回;PROCESSING處理中;COMPLETED完成;FAILED失敗");
                entity.Property(e => e.WorkflowInstanceSid).HasColumnName("workflow_instance_sid").HasComment("WorkflowDB流程實例序號");
                entity.Property(e => e.CompletedDate).HasColumnName("completed_date").HasComment("完成時間");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message").HasComment("錯誤訊息");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.CancellationNo).IsUnique().HasDatabaseName("uk_scan_cancellation_no");
                entity.HasIndex(e => e.SalesOrderSid).HasDatabaseName("idx_scan_sales_order_sid");
                entity.HasIndex(e => e.CancellationType).HasDatabaseName("idx_scan_cancellation_type");
                entity.HasIndex(e => new { e.RequestedByType, e.RequestedBySid }).HasDatabaseName("idx_scan_requested_by");
                entity.HasIndex(e => e.CancellationStatus).HasDatabaseName("idx_scan_status");
                entity.HasIndex(e => e.WorkflowInstanceSid).HasDatabaseName("idx_scan_workflow_instance_sid");
            });

            // 08. sal_cancellation_item
            modelBuilder.Entity<SalCancellationItem>(entity =>
            {
                entity.ToTable("sal_cancellation_item", tb => {
                    tb.HasComment("銷售訂單取消明細");
                    tb.HasCheckConstraint("CK_sal_cancellation_item_cancel_qty", "cancel_qty > 0");
                    tb.HasCheckConstraint("CK_sal_cancellation_item_cancel_amount", "cancel_amount >= 0");
                    tb.HasCheckConstraint("CK_sal_cancellation_item_fulfilled_qty_snapshot", "fulfilled_qty_snapshot >= 0");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("訂單取消明細序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.CancellationNid).HasColumnName("cancellation_nid").HasComment("取消主檔流水號");
                entity.Property(e => e.SalesOrderItemSid).HasColumnName("sales_order_item_sid").HasComment("銷售明細序號");
                entity.Property(e => e.CancelQty).HasColumnName("cancel_qty").HasColumnType("decimal(20,6)").HasComment("取消數量");
                entity.Property(e => e.CancelAmount).HasColumnName("cancel_amount").HasColumnType("decimal(20,4)").HasComment("取消金額");
                entity.Property(e => e.FulfilledQtySnapshot).HasColumnName("fulfilled_qty_snapshot").HasColumnType("decimal(20,6)").HasComment("取消時已履約數量");
                entity.Property(e => e.ItemStatus).HasColumnName("item_status").HasComment("PENDING待處理;APPROVED核准;REJECTED駁回;COMPLETED完成");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.CancellationNid, e.SalesOrderItemSid }).IsUnique().HasDatabaseName("uk_scani_cancel_item");
                entity.HasIndex(e => e.CancellationNid).HasDatabaseName("idx_scani_cancellation_nid");
                entity.HasIndex(e => e.SalesOrderItemSid).HasDatabaseName("idx_scani_sales_order_item_sid");
                entity.HasIndex(e => e.ItemStatus).HasDatabaseName("idx_scani_status");

                entity.HasOne(d => d.Cancellation)
                    .WithMany(p => p.CancellationItems)
                    .HasForeignKey(d => d.CancellationNid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_scani_cancellation");
            });

            // 09. sal_order_note
            modelBuilder.Entity<SalOrderNote>(entity =>
            {
                entity.ToTable("sal_order_note", tb => tb.HasComment("銷售訂單註記"));
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("訂單註記序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.SalesOrderSid).HasColumnName("sales_order_sid").HasComment("銷售單序號");
                entity.Property(e => e.NoteType).HasColumnName("note_type").HasComment("CUSTOMER客戶;INTERNAL內部;WAREHOUSE倉庫;PAYMENT付款;DELIVERY配送;PROJECT專案;SYSTEM系統");
                entity.Property(e => e.NoteContent).HasColumnName("note_content").HasComment("註記內容");
                entity.Property(e => e.Visibility).HasColumnName("visibility").HasComment("INTERNAL內部;CUSTOMER客戶可見;PARTNER合作夥伴可見");
                entity.Property(e => e.ImportantMark).HasColumnName("important_mark").HasComment("是否重要");
                entity.Property(e => e.CreateUserSid).HasColumnName("create_user_sid").HasComment("建立人員序號");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SalesOrderSid).HasDatabaseName("idx_son_sales_order_sid");
                entity.HasIndex(e => e.NoteType).HasDatabaseName("idx_son_note_type");
                entity.HasIndex(e => e.Visibility).HasDatabaseName("idx_son_visibility");
                entity.HasIndex(e => e.ImportantMark).HasDatabaseName("idx_son_important_mark");
                entity.HasIndex(e => e.CreateDate).HasDatabaseName("idx_son_create_date");
            });

            // 09. sal_order_attachment
            modelBuilder.Entity<SalOrderAttachment>(entity =>
            {
                entity.ToTable("sal_order_attachment", tb => {
                    tb.HasComment("銷售訂單附件");
                    tb.HasCheckConstraint("CK_sal_order_attachment_version_no", "version_no > 0");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("訂單附件序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.SalesOrderSid).HasColumnName("sales_order_sid").HasComment("銷售單序號");
                entity.Property(e => e.AttachmentType).HasColumnName("attachment_type").HasComment("CONTRACT合約;PURCHASE_ORDER客戶採購單;DRAWING圖面;SPEC規格;APPROVAL核准;OTHER");
                entity.Property(e => e.FileSid).HasColumnName("file_sid").HasComment("FileDB檔案序號");
                entity.Property(e => e.FileName).HasColumnName("file_name").HasComment("檔名快照");
                entity.Property(e => e.VersionNo).HasColumnName("version_no").HasComment("附件版本");
                entity.Property(e => e.CustomerVisible).HasColumnName("customer_visible").HasComment("是否客戶可見");
                entity.Property(e => e.CreateUserSid).HasColumnName("create_user_sid").HasComment("建立人員序號");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SalesOrderSid).HasDatabaseName("idx_soa2_sales_order_sid");
                entity.HasIndex(e => e.AttachmentType).HasDatabaseName("idx_soa2_attachment_type");
                entity.HasIndex(e => e.FileSid).HasDatabaseName("idx_soa2_file_sid");
                entity.HasIndex(e => e.CustomerVisible).HasDatabaseName("idx_soa2_customer_visible");
            });

            // 10. sal_status_history
            modelBuilder.Entity<SalStatusHistory>(entity =>
            {
                entity.ToTable("sal_status_history", tb => tb.HasComment("銷售狀態歷程"));
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("銷售狀態歷程序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("異動時間");
                entity.Property(e => e.EntityType).HasColumnName("entity_type").HasComment("QUOTE報價;ORDER訂單;ITEM明細;FULFILLMENT履約;PAYMENT付款;INVOICE發票;CHANGE變更;CANCELLATION取消");
                entity.Property(e => e.EntitySid).HasColumnName("entity_sid").HasComment("銷售實體序號");
                entity.Property(e => e.OldStatus).HasColumnName("old_status").HasComment("原狀態");
                entity.Property(e => e.NewStatus).HasColumnName("new_status").HasComment("新狀態");
                entity.Property(e => e.EventCode).HasColumnName("event_code").HasComment("觸發事件代碼");
                entity.Property(e => e.SourceServiceCode).HasColumnName("source_service_code").HasComment("來源服務代碼");
                entity.Property(e => e.SourceReferenceSid).HasColumnName("source_reference_sid").HasComment("來源資料序號");
                entity.Property(e => e.OperatorUserSid).HasColumnName("operator_user_sid").HasComment("操作人員序號");
                entity.Property(e => e.ReasonCode).HasColumnName("reason_code").HasComment("原因代碼");
                entity.Property(e => e.Reason).HasColumnName("reason").HasComment("原因說明");
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasComment("跨服務關聯識別碼");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.EntityType, e.EntitySid }).HasDatabaseName("idx_ssh_entity");
                entity.HasIndex(e => e.NewStatus).HasDatabaseName("idx_ssh_new_status");
                entity.HasIndex(e => e.EventCode).HasDatabaseName("idx_ssh_event_code");
                entity.HasIndex(e => e.OperatorUserSid).HasDatabaseName("idx_ssh_operator_user_sid");
                entity.HasIndex(e => e.CorrelationId).HasDatabaseName("idx_ssh_correlation_id");
                entity.HasIndex(e => e.CreateDate).HasDatabaseName("idx_ssh_create_date");
            });

            // 10. sal_event
            modelBuilder.Entity<SalEvent>(entity =>
            {
                entity.ToTable("sal_event", tb => {
                    tb.HasComment("銷售領域事件");
                    tb.HasCheckConstraint("CK_sal_event_event_version", "event_version > 0");
                });
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid").HasComment("流水號");
                entity.Property(e => e.Sid).HasColumnName("sid").HasComment("銷售事件序號");
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasComment("建立日期");
                entity.Property(e => e.EntityType).HasColumnName("entity_type").HasComment("QUOTE;ORDER;ITEM;FULFILLMENT;PAYMENT;INVOICE;CHANGE;CANCELLATION");
                entity.Property(e => e.EntitySid).HasColumnName("entity_sid").HasComment("實體序號");
                entity.Property(e => e.EventCode).HasColumnName("event_code").HasComment("事件代碼");
                entity.Property(e => e.EventVersion).HasColumnName("event_version").HasComment("事件版本");
                entity.Property(e => e.EventData).HasColumnName("event_data").HasColumnType("json").HasComment("事件內容");
                entity.Property(e => e.SourceEventId).HasColumnName("source_event_id").HasComment("來源事件ID");
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasComment("關聯識別碼");
                entity.Property(e => e.CausationId).HasColumnName("causation_id").HasComment("因果事件ID");
                entity.Property(e => e.OutboxEventSid).HasColumnName("outbox_event_sid").HasComment("IntegrationDB Outbox事件序號");
                entity.Property(e => e.ProcessStatus).HasColumnName("process_status").HasComment("PENDING待處理;SUCCESS成功;FAILED失敗;IGNORED忽略");
                entity.Property(e => e.ProcessedDate).HasColumnName("processed_date").HasComment("處理時間");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message").HasComment("錯誤訊息");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SourceEventId).IsUnique().HasDatabaseName("uk_se_source_event_id");
                entity.HasIndex(e => new { e.EntityType, e.EntitySid }).HasDatabaseName("idx_se_entity");
                entity.HasIndex(e => e.EventCode).HasDatabaseName("idx_se_event_code");
                entity.HasIndex(e => e.CorrelationId).HasDatabaseName("idx_se_correlation_id");
                entity.HasIndex(e => e.OutboxEventSid).HasDatabaseName("idx_se_outbox_event_sid");
                entity.HasIndex(e => e.ProcessStatus).HasDatabaseName("idx_se_status");
                entity.HasIndex(e => e.CreateDate).HasDatabaseName("idx_se_create_date");
            });
        }
    }
}