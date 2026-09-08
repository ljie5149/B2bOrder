using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Customer.Construction.Models;

namespace B2bOrder.Resources.Customer.Construction
{
    public class RealEstateSalesDbContext : DbContext
    {
        public RealEstateSalesDbContext(DbContextOptions<RealEstateSalesDbContext> options) : base(options)
        {
        }

        #region DbSet Configurations - 資料集設定

        public DbSet<ResPropertyListing> ResPropertyListings { get; set; }
        public DbSet<ResPropertyShowing> ResPropertyShowings { get; set; }
        public DbSet<ResPropertyOffer> ResPropertyOffers { get; set; }
        public DbSet<ResSalesContract> ResSalesContracts { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Index Configurations - 唯一索引設定

            // 01. res_property_listing
            modelBuilder.Entity<ResPropertyListing>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<ResPropertyListing>()
                .HasIndex(e => new { e.CompanySid, e.ListingCode })
                .IsUnique()
                .HasDatabaseName("uk_rpl_company_code");

            // 02. res_property_showing
            modelBuilder.Entity<ResPropertyShowing>()
                .HasIndex(e => e.Sid).IsUnique();

            // 03. res_property_offer
            modelBuilder.Entity<ResPropertyOffer>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<ResPropertyOffer>()
                .HasIndex(e => e.OfferCode)
                .IsUnique()
                .HasDatabaseName("uk_rpo_offer_code");

            // 04. res_sales_contract
            modelBuilder.Entity<ResSalesContract>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<ResSalesContract>()
                .HasIndex(e => e.ContractNo)
                .IsUnique()
                .HasDatabaseName("uk_rsc_contract_no");

            #endregion

            #region Composite Index Configurations - 複合索引與一般索引設定

            // 01. res_property_listing
            modelBuilder.Entity<ResPropertyListing>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<ResPropertyListing>()
                .HasIndex(e => e.BranchSid);
            modelBuilder.Entity<ResPropertyListing>()
                .HasIndex(e => e.AgentUserSid);
            modelBuilder.Entity<ResPropertyListing>()
                .HasIndex(e => e.OwnerCustomerSid);
            modelBuilder.Entity<ResPropertyListing>()
                .HasIndex(e => e.ListingStatus);
            modelBuilder.Entity<ResPropertyListing>()
                .HasIndex(e => new { e.City, e.District })
                .HasDatabaseName("idx_rpl_city_dist");
            modelBuilder.Entity<ResPropertyListing>()
                .HasIndex(e => e.Avalible);

            // 02. res_property_showing
            modelBuilder.Entity<ResPropertyShowing>()
                .HasIndex(e => e.PropertyListingSid);
            modelBuilder.Entity<ResPropertyShowing>()
                .HasIndex(e => e.ShowingAgentUserSid);
            modelBuilder.Entity<ResPropertyShowing>()
                .HasIndex(e => e.BuyerCustomerSid);
            modelBuilder.Entity<ResPropertyShowing>()
                .HasIndex(e => e.ShowingTime);
            modelBuilder.Entity<ResPropertyShowing>()
                .HasIndex(e => e.Avalible);

            // 03. res_property_offer
            modelBuilder.Entity<ResPropertyOffer>()
                .HasIndex(e => e.PropertyListingSid);
            modelBuilder.Entity<ResPropertyOffer>()
                .HasIndex(e => e.BuyerCustomerSid);
            modelBuilder.Entity<ResPropertyOffer>()
                .HasIndex(e => e.OfferStatus);
            modelBuilder.Entity<ResPropertyOffer>()
                .HasIndex(e => e.Avalible);

            // 04. res_sales_contract
            modelBuilder.Entity<ResSalesContract>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<ResSalesContract>()
                .HasIndex(e => e.BranchSid);
            modelBuilder.Entity<ResSalesContract>()
                .HasIndex(e => e.PropertyListingSid);
            modelBuilder.Entity<ResSalesContract>()
                .HasIndex(e => e.BuyerCustomerSid);
            modelBuilder.Entity<ResSalesContract>()
                .HasIndex(e => e.OwnerCustomerSid);
            modelBuilder.Entity<ResSalesContract>()
                .HasIndex(e => e.ContractStatus);
            modelBuilder.Entity<ResSalesContract>()
                .HasIndex(e => e.Avalible);

            #endregion

            #region Default Value Configurations - 預設值設定

            // 01. res_property_listing
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.PropertyType).HasDefaultValue("RESIDENTIAL_APARTMENT");
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.ListingDealType).HasDefaultValue("SALE");
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.AgencyContractType).HasDefaultValue("EXCLUSIVE");
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.BuildingAge).HasDefaultValue(0.0m);
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.FloorTotal).HasDefaultValue(1);
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.MainBuildingAreaPing).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.BalconyAreaPing).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.CommonAreaPing).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.ParkingAreaPing).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.TotalPing).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.TargetPrice).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.BottomPrice).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.EstimatedMonthlyRent).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.ListingStatus).HasDefaultValue("ACTIVE");
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<ResPropertyListing>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 02. res_property_showing
            modelBuilder.Entity<ResPropertyShowing>()
                .Property(e => e.BuyerFeedbackRating).HasDefaultValue(3);
            modelBuilder.Entity<ResPropertyShowing>()
                .Property(e => e.BuyerIntentLevel).HasDefaultValue("MEDIUM");
            modelBuilder.Entity<ResPropertyShowing>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<ResPropertyShowing>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 03. res_property_offer
            modelBuilder.Entity<ResPropertyOffer>()
                .Property(e => e.OfferType).HasDefaultValue("EARNEST_MONEY");
            modelBuilder.Entity<ResPropertyOffer>()
                .Property(e => e.OfferedPrice).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResPropertyOffer>()
                .Property(e => e.EarnestMoneyAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResPropertyOffer>()
                .Property(e => e.EarnestPaymentMethod).HasDefaultValue("CASH");
            modelBuilder.Entity<ResPropertyOffer>()
                .Property(e => e.OfferStatus).HasDefaultValue("ACTIVE");
            modelBuilder.Entity<ResPropertyOffer>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<ResPropertyOffer>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 04. res_sales_contract
            modelBuilder.Entity<ResSalesContract>()
                .Property(e => e.FinalDealPrice).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResSalesContract>()
                .Property(e => e.BuyerServiceFee).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResSalesContract>()
                .Property(e => e.SellerServiceFee).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResSalesContract>()
                .Property(e => e.TotalServiceFee).HasDefaultValue(0.00m);
            modelBuilder.Entity<ResSalesContract>()
                .Property(e => e.ListingAgentSplitPct).HasDefaultValue(50.00m);
            modelBuilder.Entity<ResSalesContract>()
                .Property(e => e.SellingAgentSplitPct).HasDefaultValue(50.00m);
            modelBuilder.Entity<ResSalesContract>()
                .Property(e => e.ContractStatus).HasDefaultValue("SIGNED");
            modelBuilder.Entity<ResSalesContract>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<ResSalesContract>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            #endregion
        }
    }
}