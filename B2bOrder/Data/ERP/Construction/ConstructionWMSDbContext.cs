using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.ERP.Construction.Models;

namespace B2bOrder.Resources.ERP.Construction.Data
{
    /// <summary>
    /// ConstructionWMSDB 資料庫上下文 / Construction WMS Database Context
    /// </summary>
    public class ConstructionWMSDbContext : DbContext
    {
        public ConstructionWMSDbContext(DbContextOptions<ConstructionWMSDbContext> options) : base(options)
        {
        }

        #region DbSet 宣告 / DbSet Declarations

        /// <summary>
        /// 工地區域與現場儲位主檔 / Site Zones
        /// </summary>
        public DbSet<SiteZoneMaster> SiteZoneMasters { get; set; } = null!;

        /// <summary>
        /// 進場交貨與卸貨時段預約檔 / Delivery Appointments
        /// </summary>
        public DbSet<DeliveryAppointment> DeliveryAppointments { get; set; } = null!;

        /// <summary>
        /// 車輛過磅與現場簽收點收單 / Weighbridge Tickets
        /// </summary>
        public DbSet<WeighbridgeTicket> WeighbridgeTickets { get; set; } = null!;

        /// <summary>
        /// 跨工地材料與設備調撥主檔 / Transfer Orders
        /// </summary>
        public DbSet<TransferOrder> TransferOrders { get; set; } = null!;

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region 索引與限制條件設定 / Indexes and Constraints Configuration

            // wms_site_zone_master
            modelBuilder.Entity<SiteZoneMaster>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.ProjectSid, e.ZoneCode }).IsUnique().HasDatabaseName("uk_wszm_project_zone");
                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_wszm_company_sid");
                entity.HasIndex(e => e.ProjectSid).HasDatabaseName("idx_wszm_project_sid");
                entity.HasIndex(e => e.ZoneType).HasDatabaseName("idx_wszm_type");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wszm_avalible");
                entity.ToTable(tb => tb.HasCheckConstraint("CK_SiteZoneMaster_MaxWeight", "max_weight_capacity_kg >= 0"));
            });

            // wms_delivery_appointment
            modelBuilder.Entity<DeliveryAppointment>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.AppointmentNo).IsUnique().HasDatabaseName("uk_wda_appointment_no");
                entity.HasIndex(e => e.ProjectSid).HasDatabaseName("idx_wda_project_sid");
                entity.HasIndex(e => e.ZoneSid).HasDatabaseName("idx_wda_zone_sid");
                entity.HasIndex(e => e.ExpectedArrivalTime).HasDatabaseName("idx_wda_arrival");
                entity.HasIndex(e => e.AppointmentStatus).HasDatabaseName("idx_wda_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wda_avalible");
                entity.ToTable(tb => tb.HasCheckConstraint("CK_DeliveryAppointment_Duration", "estimated_duration_mins > 0"));
            });

            // wms_weighbridge_ticket
            modelBuilder.Entity<WeighbridgeTicket>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.TicketNo).IsUnique().HasDatabaseName("uk_wwt_ticket_no");
                entity.HasIndex(e => e.ProjectSid).HasDatabaseName("idx_wwt_project_sid");
                entity.HasIndex(e => e.AppointmentSid).HasDatabaseName("idx_wwt_appointment");
                entity.HasIndex(e => e.RefPoSid).HasDatabaseName("idx_wwt_po");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wwt_avalible");
                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_WeighbridgeTicket_Gross", "gross_weight_kg >= 0");
                    tb.HasCheckConstraint("CK_WeighbridgeTicket_Tare", "tare_weight_kg >= 0");
                    tb.HasCheckConstraint("CK_WeighbridgeTicket_Net", "net_weight_kg >= 0");
                });
            });

            // wms_transfer_order
            modelBuilder.Entity<TransferOrder>(entity =>
            {
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.TransferNo).IsUnique().HasDatabaseName("uk_wto_transfer_no");
                entity.HasIndex(e => e.CompanySid).HasDatabaseName("idx_wto_company_sid");
                entity.HasIndex(e => e.FromProjectSid).HasDatabaseName("idx_wto_from_project");
                entity.HasIndex(e => e.ToProjectSid).HasDatabaseName("idx_wto_to_project");
                entity.HasIndex(e => e.TransferStatus).HasDatabaseName("idx_wto_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wto_avalible");
            });

            #endregion
        }
    }
}