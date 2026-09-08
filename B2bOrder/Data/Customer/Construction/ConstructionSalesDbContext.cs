using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Customer.Construction.Models;

namespace B2bOrder.Resources.Customer.Construction
{
    public class ConstructionSalesDbContext : DbContext
    {
        public ConstructionSalesDbContext(DbContextOptions<ConstructionSalesDbContext> options) : base(options)
        {
        }

        #region DbSet Configurations - 資料集設定

        public DbSet<SalPropertyUnit> SalPropertyUnits { get; set; }
        public DbSet<SalSalesContract> SalSalesContracts { get; set; }
        public DbSet<SalPaymentInstallment> SalPaymentInstallments { get; set; }
        public DbSet<SalCustomerChangeOrder> SalCustomerChangeOrders { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Index Configurations - 唯一索引設定

            // 01. sal_property_unit
            modelBuilder.Entity<SalPropertyUnit>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<SalPropertyUnit>()
                .HasIndex(e => new { e.ProjectSid, e.FullUnitCode })
                .IsUnique()
                .HasDatabaseName("uk_spu_project_unit");

            // 02. sal_sales_contract
            modelBuilder.Entity<SalSalesContract>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<SalSalesContract>()
                .HasIndex(e => new { e.ProjectSid, e.ContractNo })
                .IsUnique()
                .HasDatabaseName("uk_ssc_project_contract_no");

            // 03. sal_payment_installment
            modelBuilder.Entity<SalPaymentInstallment>()
                .HasIndex(e => e.Sid).IsUnique();

            // 04. sal_customer_change_order
            modelBuilder.Entity<SalCustomerChangeOrder>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<SalCustomerChangeOrder>()
                .HasIndex(e => e.ChangeOrderNo)
                .IsUnique()
                .HasDatabaseName("uk_scco_change_order_no");

            #endregion

            #region Composite Index Configurations - 複合索引與一般索引設定

            // 01. sal_property_unit
            modelBuilder.Entity<SalPropertyUnit>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<SalPropertyUnit>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<SalPropertyUnit>()
                .HasIndex(e => e.SalesStatus);
            modelBuilder.Entity<SalPropertyUnit>()
                .HasIndex(e => e.UnitType);
            modelBuilder.Entity<SalPropertyUnit>()
                .HasIndex(e => e.Avalible);

            // 02. sal_sales_contract
            modelBuilder.Entity<SalSalesContract>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<SalSalesContract>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<SalSalesContract>()
                .HasIndex(e => e.UnitSid);
            modelBuilder.Entity<SalSalesContract>()
                .HasIndex(e => e.BuyerCustomerSid);
            modelBuilder.Entity<SalSalesContract>()
                .HasIndex(e => e.ContractStatus);
            modelBuilder.Entity<SalSalesContract>()
                .HasIndex(e => e.Avalible);

            // 03. sal_payment_installment
            modelBuilder.Entity<SalPaymentInstallment>()
                .HasIndex(e => e.ContractSid);
            modelBuilder.Entity<SalPaymentInstallment>()
                .HasIndex(e => e.DueDate);
            modelBuilder.Entity<SalPaymentInstallment>()
                .HasIndex(e => e.PaymentStatus);
            modelBuilder.Entity<SalPaymentInstallment>()
                .HasIndex(e => e.Avalible);

            // 04. sal_customer_change_order
            modelBuilder.Entity<SalCustomerChangeOrder>()
                .HasIndex(e => e.ContractSid);
            modelBuilder.Entity<SalCustomerChangeOrder>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<SalCustomerChangeOrder>()
                .HasIndex(e => e.Status);
            modelBuilder.Entity<SalCustomerChangeOrder>()
                .HasIndex(e => e.Avalible);

            #endregion

            #region Default Value Configurations - 預設值設定

            // 01. sal_property_unit
            modelBuilder.Entity<SalPropertyUnit>()
                .Property(e => e.UnitType).HasDefaultValue("RESIDENTIAL");
            modelBuilder.Entity<SalPropertyUnit>()
                .Property(e => e.MainBuildingAreaPing).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalPropertyUnit>()
                .Property(e => e.BalconyAreaPing).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalPropertyUnit>()
                .Property(e => e.CommonAreaPing).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalPropertyUnit>()
                .Property(e => e.TotalSalesAreaPing).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalPropertyUnit>()
                .Property(e => e.ListPriceHouse).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalPropertyUnit>()
                .Property(e => e.ListPriceLand).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalPropertyUnit>()
                .Property(e => e.ListPriceTotal).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalPropertyUnit>()
                .Property(e => e.SalesStatus).HasDefaultValue("AVAILABLE");
            modelBuilder.Entity<SalPropertyUnit>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<SalPropertyUnit>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 02. sal_sales_contract
            modelBuilder.Entity<SalSalesContract>()
                .Property(e => e.HouseAgreedPrice).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalSalesContract>()
                .Property(e => e.LandAgreedPrice).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalSalesContract>()
                .Property(e => e.ParkingAgreedPrice).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalSalesContract>()
                .Property(e => e.TotalContractPrice).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalSalesContract>()
                .Property(e => e.ContractStatus).HasDefaultValue("RESERVED");
            modelBuilder.Entity<SalSalesContract>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<SalSalesContract>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 03. sal_payment_installment
            modelBuilder.Entity<SalPaymentInstallment>()
                .Property(e => e.StageSequence).HasDefaultValue(1);
            modelBuilder.Entity<SalPaymentInstallment>()
                .Property(e => e.DueAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalPaymentInstallment>()
                .Property(e => e.PaidAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalPaymentInstallment>()
                .Property(e => e.PaymentStatus).HasDefaultValue("UNPAID");
            modelBuilder.Entity<SalPaymentInstallment>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<SalPaymentInstallment>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 04. sal_customer_change_order
            modelBuilder.Entity<SalCustomerChangeOrder>()
                .Property(e => e.AddAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalCustomerChangeOrder>()
                .Property(e => e.DeductAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalCustomerChangeOrder>()
                .Property(e => e.NetChangeAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<SalCustomerChangeOrder>()
                .Property(e => e.Status).HasDefaultValue("SUBMITTED");
            modelBuilder.Entity<SalCustomerChangeOrder>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<SalCustomerChangeOrder>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            #endregion
        }
    }
}