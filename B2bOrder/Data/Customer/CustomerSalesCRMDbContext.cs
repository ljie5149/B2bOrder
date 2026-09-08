using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Customer.Models;

namespace B2bOrder.Resources.Customer
{
    public class CustomerSalesCrmDbContext : DbContext
    {
        public CustomerSalesCrmDbContext(DbContextOptions<CustomerSalesCrmDbContext> options) : base(options)
        {
        }

        #region DbSet Configurations - 資料集設定

        public DbSet<CrmLead> CrmLeads { get; set; }
        public DbSet<CrmCustomer> CrmCustomers { get; set; }
        public DbSet<CrmSalesProject> CrmSalesProjects { get; set; }
        public DbSet<CrmSalesUnit> CrmSalesUnits { get; set; }
        public DbSet<CrmOpportunity> CrmOpportunities { get; set; }
        public DbSet<CrmSalesContract> CrmSalesContracts { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Index Configurations - 唯一索引設定

            modelBuilder.Entity<CrmLead>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<CrmLead>().HasIndex(e => e.LeadNo).IsUnique().HasDatabaseName("uk_cl_lead_no");

            modelBuilder.Entity<CrmCustomer>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<CrmCustomer>().HasIndex(e => e.CustomerNo).IsUnique().HasDatabaseName("uk_cc_customer_no");
            modelBuilder.Entity<CrmCustomer>().HasIndex(e => new { e.PartySid, e.CompanySid }).IsUnique().HasDatabaseName("uk_cc_party_company");

            modelBuilder.Entity<CrmSalesProject>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<CrmSalesProject>().HasIndex(e => e.SalesProjectNo).IsUnique().HasDatabaseName("uk_csp_sales_project_no");

            modelBuilder.Entity<CrmSalesUnit>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<CrmSalesUnit>().HasIndex(e => new { e.SalesProjectSid, e.UnitNo }).IsUnique().HasDatabaseName("uk_csu_project_unit_no");

            modelBuilder.Entity<CrmOpportunity>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<CrmOpportunity>().HasIndex(e => e.OpportunityNo).IsUnique().HasDatabaseName("uk_co_opportunity_no");

            modelBuilder.Entity<CrmSalesContract>().HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<CrmSalesContract>().HasIndex(e => e.SalesContractNo).IsUnique().HasDatabaseName("uk_csc_contract_no");
            modelBuilder.Entity<CrmSalesContract>().HasIndex(e => e.SalesUnitSid).IsUnique().HasDatabaseName("uk_csc_sales_unit");

            #endregion

            #region General & Composite Index Configurations - 一般與複合索引設定

            modelBuilder.Entity<CrmLead>().HasIndex(e => e.CompanySid);
            modelBuilder.Entity<CrmLead>().HasIndex(e => e.CampaignSid);
            modelBuilder.Entity<CrmLead>().HasIndex(e => e.SourceType);
            modelBuilder.Entity<CrmLead>().HasIndex(e => e.InterestedProjectSid);
            modelBuilder.Entity<CrmLead>().HasIndex(e => e.AssignedSalesPartySid);
            modelBuilder.Entity<CrmLead>().HasIndex(e => e.LeadStatus);

            modelBuilder.Entity<CrmCustomer>().HasIndex(e => e.PartySid);
            modelBuilder.Entity<CrmCustomer>().HasIndex(e => e.CompanySid);
            modelBuilder.Entity<CrmCustomer>().HasIndex(e => e.SalesOwnerPartySid);
            modelBuilder.Entity<CrmCustomer>().HasIndex(e => e.CustomerLevel);
            modelBuilder.Entity<CrmCustomer>().HasIndex(e => e.CustomerStatus);

            modelBuilder.Entity<CrmSalesProject>().HasIndex(e => e.CompanySid);
            modelBuilder.Entity<CrmSalesProject>().HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<CrmSalesProject>().HasIndex(e => e.PropertySid);
            modelBuilder.Entity<CrmSalesProject>().HasIndex(e => e.SalesAgentPartySid);
            modelBuilder.Entity<CrmSalesProject>().HasIndex(e => e.ProjectStatus);

            modelBuilder.Entity<CrmSalesUnit>().HasIndex(e => e.SalesProjectSid);
            modelBuilder.Entity<CrmSalesUnit>().HasIndex(e => e.ProductType);
            modelBuilder.Entity<CrmSalesUnit>().HasIndex(e => e.BuildingSid);
            modelBuilder.Entity<CrmSalesUnit>().HasIndex(e => e.FloorSid);
            modelBuilder.Entity<CrmSalesUnit>().HasIndex(e => e.SpaceSid);
            modelBuilder.Entity<CrmSalesUnit>().HasIndex(e => e.ParentUnitSid);
            modelBuilder.Entity<CrmSalesUnit>().HasIndex(e => e.ReservedCustomerSid);
            modelBuilder.Entity<CrmSalesUnit>().HasIndex(e => e.UnitStatus);

            modelBuilder.Entity<CrmOpportunity>().HasIndex(e => e.CustomerSid);
            modelBuilder.Entity<CrmOpportunity>().HasIndex(e => e.LeadSid);
            modelBuilder.Entity<CrmOpportunity>().HasIndex(e => e.SalesProjectSid);
            modelBuilder.Entity<CrmOpportunity>().HasIndex(e => e.SalesUnitSid);
            modelBuilder.Entity<CrmOpportunity>().HasIndex(e => e.SalesOwnerPartySid);
            modelBuilder.Entity<CrmOpportunity>().HasIndex(e => e.StageCode);
            modelBuilder.Entity<CrmOpportunity>().HasIndex(e => e.ExpectedCloseDate);
            modelBuilder.Entity<CrmOpportunity>().HasIndex(e => e.OpportunityStatus);

            modelBuilder.Entity<CrmSalesContract>().HasIndex(e => e.CustomerSid);
            modelBuilder.Entity<CrmSalesContract>().HasIndex(e => e.SalesProjectSid);
            modelBuilder.Entity<CrmSalesContract>().HasIndex(e => e.SalesUnitSid);
            modelBuilder.Entity<CrmSalesContract>().HasIndex(e => e.ContractSid);
            modelBuilder.Entity<CrmSalesContract>().HasIndex(e => e.SigningDate);
            modelBuilder.Entity<CrmSalesContract>().HasIndex(e => e.ExpectedHandoverDate);
            modelBuilder.Entity<CrmSalesContract>().HasIndex(e => e.ContractStatus);

            #endregion

            #region Default Value Configurations - 欄位預設值設定

            modelBuilder.Entity<CrmLead>().Property(e => e.QualificationScore).HasDefaultValue(0.0000m);
            modelBuilder.Entity<CrmLead>().Property(e => e.LeadStatus).HasDefaultValue("NEW");
            modelBuilder.Entity<CrmLead>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<CrmCustomer>().Property(e => e.CustomerLevel).HasDefaultValue("NORMAL");
            modelBuilder.Entity<CrmCustomer>().Property(e => e.TotalPurchaseAmount).HasDefaultValue(0.0000m);
            modelBuilder.Entity<CrmCustomer>().Property(e => e.TotalContractCount).HasDefaultValue(0);
            modelBuilder.Entity<CrmCustomer>().Property(e => e.ConsentMarketing).HasDefaultValue(false);
            modelBuilder.Entity<CrmCustomer>().Property(e => e.CustomerStatus).HasDefaultValue("ACTIVE");
            modelBuilder.Entity<CrmCustomer>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<CrmSalesProject>().Property(e => e.TotalUnitCount).HasDefaultValue(0);
            modelBuilder.Entity<CrmSalesProject>().Property(e => e.AvailableUnitCount).HasDefaultValue(0);
            modelBuilder.Entity<CrmSalesProject>().Property(e => e.ReservedUnitCount).HasDefaultValue(0);
            modelBuilder.Entity<CrmSalesProject>().Property(e => e.SoldUnitCount).HasDefaultValue(0);
            modelBuilder.Entity<CrmSalesProject>().Property(e => e.ProjectStatus).HasDefaultValue("PLANNING");
            modelBuilder.Entity<CrmSalesProject>().Property(e => e.Avalible).HasDefaultValue("Y");

            modelBuilder.Entity<CrmSalesUnit>().Property(e => e.RegisteredArea).HasDefaultValue(0.000000m);
            modelBuilder.Entity<CrmSalesUnit>().Property(e => e.MainBuildingArea).HasDefaultValue(0.000000m);
            modelBuilder.Entity<CrmSalesUnit>().Property(e => e.AccessoryArea).HasDefaultValue(0.000000m);
            modelBuilder.Entity<CrmSalesUnit>().Property(e => e.CommonArea).HasDefaultValue(0.000000m);
            modelBuilder.Entity<CrmSalesUnit>().Property(e => e.LandArea).HasDefaultValue(0.000000m);
            modelBuilder.Entity<CrmSalesUnit>().Property(e => e.ListPrice).HasDefaultValue(0.0000m);
            modelBuilder.Entity<CrmSalesUnit>().Property(e => e.MinimumPrice).HasDefaultValue(0.0000m);
            modelBuilder.Entity<CrmSalesUnit>().Property(e => e.UnitStatus).HasDefaultValue("AVAILABLE");

            modelBuilder.Entity<CrmOpportunity>().Property(e => e.ExpectedAmount).HasDefaultValue(0.0000m);
            modelBuilder.Entity<CrmOpportunity>().Property(e => e.ProbabilityPercent).HasDefaultValue(0.0000m);
            modelBuilder.Entity<CrmOpportunity>().Property(e => e.OpportunityStatus).HasDefaultValue("OPEN");
            modelBuilder.Entity<CrmOpportunity>().Property(e => e.VersionNo).HasDefaultValue(0UL);

            modelBuilder.Entity<CrmSalesContract>().Property(e => e.TaxAmount).HasDefaultValue(0.0000m);
            modelBuilder.Entity<CrmSalesContract>().Property(e => e.MortgageRequired).HasDefaultValue(false);
            modelBuilder.Entity<CrmSalesContract>().Property(e => e.MortgageAmount).HasDefaultValue(0.0000m);
            modelBuilder.Entity<CrmSalesContract>().Property(e => e.ContractStatus).HasDefaultValue("DRAFT");
            modelBuilder.Entity<CrmSalesContract>().Property(e => e.VersionNo).HasDefaultValue(0UL);

            #endregion
        }
    }
}