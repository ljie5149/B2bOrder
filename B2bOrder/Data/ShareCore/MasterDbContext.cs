using B2bOrder.Models.Master;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Data
{
    public class MasterDbContext : DbContext
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }

        // 國家、行政區、地址
        public DbSet<MstCountry> Countries { get; set; } = null!;
        public DbSet<MstRegion> Regions { get; set; } = null!;
        public DbSet<MasterDB> Addresses { get; set; } = null!;

        // 語系、幣別、匯率
        public DbSet<MstLanguage> Languages { get; set; } = null!;
        public DbSet<MstCurrency> Currencies { get; set; } = null!;
        public DbSet<MstExchangeRate> ExchangeRates { get; set; } = null!;

        // 公司、據點、組織
        public DbSet<MstCompany> Companies { get; set; } = null!;
        public DbSet<MstBusinessUnit> BusinessUnits { get; set; } = null!;
        public DbSet<MstDepartment> Departments { get; set; } = null!;
        public DbSet<MstPosition> Positions { get; set; } = null!;

        // 成本與專案
        public DbSet<MstCostCenter> CostCenters { get; set; } = null!;
        public DbSet<MstProfitCenter> ProfitCenters { get; set; } = null!;
        public DbSet<MstProject> Projects { get; set; } = null!;

        // 單位、稅別、付款條件/方式
        public DbSet<MstUnit> Units { get; set; } = null!;
        public DbSet<MstTax> Taxes { get; set; } = null!;
        public DbSet<MstPaymentTerm> PaymentTerms { get; set; } = null!;
        public DbSet<MstPaymentMethod> PaymentMethods { get; set; } = null!;

        // 倉庫與通用代碼
        public DbSet<MstWarehouse> Warehouses { get; set; } = null!;
        public DbSet<MstLocation> Locations { get; set; } = null!;
        public DbSet<MstDocumentType> DocumentTypes { get; set; } = null!;
        public DbSet<MstCodeGroup> CodeGroups { get; set; } = null!;
        public DbSet<MstCodeValue> CodeValues { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent API 設定 mappings (如表名下劃線對應)
            modelBuilder.Entity<MstCountry>().ToTable("mst_country");
            modelBuilder.Entity<MstRegion>().ToTable("mst_region");
            modelBuilder.Entity<MasterDB>().ToTable("mst_address");
            modelBuilder.Entity<MstLanguage>().ToTable("mst_language");
            modelBuilder.Entity<MstCurrency>().ToTable("mst_currency");
            modelBuilder.Entity<MstExchangeRate>().ToTable("mst_exchange_rate");
            modelBuilder.Entity<MstCompany>().ToTable("mst_company");
            modelBuilder.Entity<MstBusinessUnit>().ToTable("mst_business_unit");
            modelBuilder.Entity<MstDepartment>().ToTable("mst_department");
            modelBuilder.Entity<MstPosition>().ToTable("mst_position");
            modelBuilder.Entity<MstCostCenter>().ToTable("mst_cost_center");
            modelBuilder.Entity<MstProfitCenter>().ToTable("mst_profit_center");
            modelBuilder.Entity<MstProject>().ToTable("mst_project");
            modelBuilder.Entity<MstUnit>().ToTable("mst_unit");
            modelBuilder.Entity<MstTax>().ToTable("mst_tax");
            modelBuilder.Entity<MstPaymentTerm>().ToTable("mst_payment_term");
            modelBuilder.Entity<MstPaymentMethod>().ToTable("mst_payment_method");
            modelBuilder.Entity<MstWarehouse>().ToTable("mst_warehouse");
            modelBuilder.Entity<MstLocation>().ToTable("mst_location");
            modelBuilder.Entity<MstDocumentType>().ToTable("mst_document_type");
            modelBuilder.Entity<MstCodeGroup>().ToTable("mst_code_group");
            modelBuilder.Entity<MstCodeValue>().ToTable("mst_code_value");
        }
    }
}