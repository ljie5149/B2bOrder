// TMSDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Resources.ERP
{
    public class GeneralLedgerDbContext : DbContext
    {
        public GeneralLedgerDbContext(DbContextOptions<GeneralLedgerDbContext> options) : base(options)
        {
        }

        #region 01. Carriers, Vehicles, and Drivers (承運商、車隊、車輛與司機基礎架構)
        public DbSet<TmsCarrier> TmsCarriers => Set<TmsCarrier>();
        public DbSet<TmsVehicle> TmsVehicles => Set<TmsVehicle>();
        public DbSet<TmsDriver> TmsDrivers => Set<TmsDriver>();
        #endregion

        #region 02. Dispatch Trips and Routes (派車車次 / 車程調度)
        public DbSet<TmsDispatchTrip> TmsDispatchTrips => Set<TmsDispatchTrip>();
        #endregion

        #region 03. Waybills, Items, and Stops (託運單與站點明細)
        public DbSet<TmsWaybill> TmsWaybills => Set<TmsWaybill>();
        public DbSet<TmsWaybillItem> TmsWaybillItems => Set<TmsWaybillItem>();
        public DbSet<TmsTripStop> TmsTripStops => Set<TmsTripStop>();
        #endregion

        #region 04. Tracking and Proof of Delivery (軌跡追蹤與 POD)
        public DbSet<TmsTrackingEvent> TmsTrackingEvents => Set<TmsTrackingEvent>();
        public DbSet<TmsProofOfDelivery> TmsProofOfDeliveries => Set<TmsProofOfDelivery>();
        #endregion

        #region 05. Freight Cost Settlement (運費核算與費用結算)
        public DbSet<TmsFreightSettlement> TmsFreightSettlements => Set<TmsFreightSettlement>();
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TmsCarrier 設定
            modelBuilder.Entity<TmsCarrier>(entity =>
            {
                entity.ToTable("tms_carrier");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.CarrierCode).IsUnique();
            });

            // TmsVehicle 設定
            modelBuilder.Entity<TmsVehicle>(entity =>
            {
                entity.ToTable("tms_vehicle");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.LicensePlate).IsUnique();

                entity.HasOne(d => d.Carrier)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.CarrierNid)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TmsDriver 設定
            modelBuilder.Entity<TmsDriver>(entity =>
            {
                entity.ToTable("tms_driver");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.HasOne(d => d.Carrier)
                    .WithMany(p => p.Drivers)
                    .HasForeignKey(d => d.CarrierNid)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TmsDispatchTrip 設定
            modelBuilder.Entity<TmsDispatchTrip>(entity =>
            {
                entity.ToTable("tms_dispatch_trip");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.TripNo).IsUnique();

                entity.HasOne(d => d.Carrier)
                    .WithMany(p => p.DispatchTrips)
                    .HasForeignKey(d => d.CarrierNid)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.DispatchTrips)
                    .HasForeignKey(d => d.VehicleNid)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Driver)
                    .WithMany(p => p.DispatchTrips)
                    .HasForeignKey(d => d.DriverNid)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // TmsWaybill 設定
            modelBuilder.Entity<TmsWaybill>(entity =>
            {
                entity.ToTable("tms_waybill");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.WaybillNo).IsUnique();

                entity.HasOne(d => d.DispatchTrip)
                    .WithMany(p => p.Waybills)
                    .HasForeignKey(d => d.DispatchTripNid)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // TmsWaybillItem 設定
            modelBuilder.Entity<TmsWaybillItem>(entity =>
            {
                entity.ToTable("tms_waybill_item");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.WaybillNid, e.LineNo }).IsUnique();

                entity.HasOne(d => d.Waybill)
                    .WithMany(p => p.WaybillItems)
                    .HasForeignKey(d => d.WaybillNid)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // TmsTripStop 設定
            modelBuilder.Entity<TmsTripStop>(entity =>
            {
                entity.ToTable("tms_trip_stop");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.DispatchTripNid, e.StopSequence }).IsUnique();

                entity.HasOne(d => d.DispatchTrip)
                    .WithMany(p => p.TripStops)
                    .HasForeignKey(d => d.DispatchTripNid)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Waybill)
                    .WithMany(p => p.TripStops)
                    .HasForeignKey(d => d.WaybillNid)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // TmsTrackingEvent 設定
            modelBuilder.Entity<TmsTrackingEvent>(entity =>
            {
                entity.ToTable("tms_tracking_event");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.HasOne(d => d.Waybill)
                    .WithMany(p => p.TrackingEvents)
                    .HasForeignKey(d => d.WaybillNid)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // TmsProofOfDelivery 設定
            modelBuilder.Entity<TmsProofOfDelivery>(entity =>
            {
                entity.ToTable("tms_proof_of_delivery");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.WaybillNid).IsUnique();

                entity.HasOne(d => d.Waybill)
                    .WithOne(p => p.ProofOfDelivery)
                    .HasForeignKey<TmsProofOfDelivery>(d => d.WaybillNid)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // TmsFreightSettlement 設定
            modelBuilder.Entity<TmsFreightSettlement>(entity =>
            {
                entity.ToTable("tms_freight_settlement");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SettlementNo).IsUnique();

                entity.HasOne(d => d.Carrier)
                    .WithMany(p => p.FreightSettlements)
                    .HasForeignKey(d => d.CarrierNid)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Waybill)
                    .WithMany(p => p.FreightSettlements)
                    .HasForeignKey(d => d.WaybillNid)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}