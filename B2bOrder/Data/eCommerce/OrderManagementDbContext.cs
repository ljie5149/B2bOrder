using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Resources.eCommerce.Models
{
    public class OmsDbContext : DbContext
    {
        public OmsDbContext(DbContextOptions<OmsDbContext> options) : base(options)
        {
        }

        public DbSet<OmsChannel> OmsChannels { get; set; } = null!;
        public DbSet<OmsChannelAccount> OmsChannelAccounts { get; set; } = null!;
        public DbSet<OmsOrderImport> OmsOrderImports { get; set; } = null!;
        public DbSet<OmsOrder> OmsOrders { get; set; } = null!;
        public DbSet<OmsOrderItem> OmsOrderItems { get; set; } = null!;
        public DbSet<OmsOrderAddress> OmsOrderAddresses { get; set; } = null!;
        public DbSet<OmsFulfillmentOrder> OmsFulfillmentOrders { get; set; } = null!;
        public DbSet<OmsFulfillmentOrderItem> OmsFulfillmentOrderItems { get; set; } = null!;
        public DbSet<OmsRoutingRule> OmsRoutingRules { get; set; } = null!;
        public DbSet<OmsRoutingLog> OmsRoutingLogs { get; set; } = null!;
        public DbSet<OmsOrderStatusHistory> OmsOrderStatusHistories { get; set; } = null!;
        public DbSet<OmsStatusMapping> OmsStatusMappings { get; set; } = null!;
        public DbSet<OmsStatusSyncJob> OmsStatusSyncJobs { get; set; } = null!;
        public DbSet<OmsCancelRequest> OmsCancelRequests { get; set; } = null!;
        public DbSet<OmsCancelItem> OmsCancelItems { get; set; } = null!;
        public DbSet<OmsException> OmsExceptions { get; set; } = null!;
        public DbSet<OmsExceptionAction> OmsExceptionActions { get; set; } = null!;
        public DbSet<OmsOrderEvent> OmsOrderEvents { get; set; } = null!;
        public DbSet<OmsSlaMonitor> OmsSlaMonitors { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 01. 銷售通路 (Sales Channel)
            modelBuilder.Entity<OmsChannel>(entity =>
            {
                entity.ToTable("oms_channel");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ChannelCode).IsUnique();
            });

            modelBuilder.Entity<OmsChannelAccount>(entity =>
            {
                entity.ToTable("oms_channel_account");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.ChannelNid, e.AccountCode }).IsUnique();
                entity.HasOne(e => e.Channel)
                      .WithMany(c => c.ChannelAccounts)
                      .HasForeignKey(e => e.ChannelNid);
            });

            // 02. 外部訂單接收 (External Order Import)
            modelBuilder.Entity<OmsOrderImport>(entity =>
            {
                entity.ToTable("oms_order_import");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ImportNo).IsUnique();
                entity.HasIndex(e => new { e.ChannelAccountNid, e.IdempotencyKey }).IsUnique();
                entity.HasIndex(e => new { e.ChannelAccountNid, e.ExternalOrderNo, e.ExternalOrderVersion }).IsUnique();
                entity.HasOne(e => e.ChannelAccount)
                      .WithMany(a => a.OrderImports)
                      .HasForeignKey(e => e.ChannelAccountNid);
            });

            // 03. OMS訂單主檔與明細 (OMS Order Master & Details)
            modelBuilder.Entity<OmsOrder>(entity =>
            {
                entity.ToTable("oms_order");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.OmsOrderNo).IsUnique();
                entity.HasIndex(e => new { e.ChannelAccountSid, e.ExternalOrderNo }).IsUnique();
            });

            modelBuilder.Entity<OmsOrderItem>(entity =>
            {
                entity.ToTable("oms_order_item");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.OmsOrderNid, e.LineNo }).IsUnique();
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(e => e.OmsOrderNid);
            });

            modelBuilder.Entity<OmsOrderAddress>(entity =>
            {
                entity.ToTable("oms_order_address");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.OmsOrderNid, e.AddressType }).IsUnique();
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderAddresses)
                      .HasForeignKey(e => e.OmsOrderNid);
            });

            // 04. 拆單與履約訂單 (Fulfillment Orders)
            modelBuilder.Entity<OmsFulfillmentOrder>(entity =>
            {
                entity.ToTable("oms_fulfillment_order");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.FulfillmentOrderNo).IsUnique();
                entity.HasIndex(e => new { e.OmsOrderNid, e.SplitGroupNo }).IsUnique();
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.FulfillmentOrders)
                      .HasForeignKey(e => e.OmsOrderNid);
            });

            modelBuilder.Entity<OmsFulfillmentOrderItem>(entity =>
            {
                entity.ToTable("oms_fulfillment_order_item");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.FulfillmentOrderNid, e.OmsOrderItemNid }).IsUnique();
                entity.HasOne(e => e.FulfillmentOrder)
                      .WithMany(f => f.FulfillmentOrderItems)
                      .HasForeignKey(e => e.FulfillmentOrderNid);
                entity.HasOne(e => e.OrderItem)
                      .WithMany(i => i.FulfillmentOrderItems)
                      .HasForeignKey(e => e.OmsOrderItemNid);
            });

            // 05. 訂單路由 (Routing Rules & Logs)
            modelBuilder.Entity<OmsRoutingRule>(entity =>
            {
                entity.ToTable("oms_routing_rule");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.RuleCode).IsUnique();
            });

            modelBuilder.Entity<OmsRoutingLog>(entity =>
            {
                entity.ToTable("oms_routing_log");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
            });

            // 06. 狀態歷程與同步 (Status History & Sync)
            modelBuilder.Entity<OmsOrderStatusHistory>(entity =>
            {
                entity.ToTable("oms_order_status_history");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.StatusHistories)
                      .HasForeignKey(e => e.OmsOrderNid);
            });

            modelBuilder.Entity<OmsStatusMapping>(entity =>
            {
                entity.ToTable("oms_status_mapping");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.ChannelSid, e.StatusType, e.ExternalStatus, e.Direction }).IsUnique();
            });

            modelBuilder.Entity<OmsStatusSyncJob>(entity =>
            {
                entity.ToTable("oms_status_sync_job");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.JobNo).IsUnique();
            });

            // 07. 訂單取消 (Cancel Requests)
            modelBuilder.Entity<OmsCancelRequest>(entity =>
            {
                entity.ToTable("oms_cancel_request");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.CancelNo).IsUnique();
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.CancelRequests)
                      .HasForeignKey(e => e.OmsOrderNid);
            });

            modelBuilder.Entity<OmsCancelItem>(entity =>
            {
                entity.ToTable("oms_cancel_item");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.CancelRequestNid, e.OmsOrderItemNid }).IsUnique();
                entity.HasOne(e => e.CancelRequest)
                      .WithMany(c => c.CancelItems)
                      .HasForeignKey(e => e.CancelRequestNid);
                entity.HasOne(e => e.OrderItem)
                      .WithMany(i => i.CancelItems)
                      .HasForeignKey(e => e.OmsOrderItemNid);
            });

            // 08. 訂單例外 (Exceptions)
            modelBuilder.Entity<OmsException>(entity =>
            {
                entity.ToTable("oms_exception");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ExceptionNo).IsUnique();
            });

            modelBuilder.Entity<OmsExceptionAction>(entity =>
            {
                entity.ToTable("oms_exception_action");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasOne(e => e.Exception)
                      .WithMany(e => e.ExceptionActions)
                      .HasForeignKey(e => e.ExceptionNid);
            });

            // 09. 事件與監控 (Events & SLA)
            modelBuilder.Entity<OmsOrderEvent>(entity =>
            {
                entity.ToTable("oms_order_event");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.SourceServiceCode, e.SourceEventId }).IsUnique();
            });

            modelBuilder.Entity<OmsSlaMonitor>(entity =>
            {
                entity.ToTable("oms_sla_monitor");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
            });
        }
    }
}