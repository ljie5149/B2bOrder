using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Construction.Models;

namespace B2bOrder.Resources.Construction
{
    public class ContractDbContext : DbContext
    {
        public ContractDbContext(DbContextOptions<ContractDbContext> options) : base(options)
        {
        }

        #region DbSet Configurations - 資料集設定

        public DbSet<CtrContractMaster> CtrContractMasters { get; set; }
        public DbSet<CtrPaymentTerm> CtrPaymentTerms { get; set; }
        public DbSet<CtrContractAmendment> CtrContractAmendments { get; set; }
        public DbSet<CtrBondGuarantee> CtrBondGuarantees { get; set; }
        public DbSet<CtrWarrantyTerm> CtrWarrantyTerms { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Index Configurations - 唯一索引設定

            // 01. ctr_contract_master
            modelBuilder.Entity<CtrContractMaster>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<CtrContractMaster>()
                .HasIndex(e => new { e.CompanySid, e.ContractNumber })
                .IsUnique()
                .HasDatabaseName("uk_ccm_company_number");

            // 02. ctr_payment_term
            modelBuilder.Entity<CtrPaymentTerm>()
                .HasIndex(e => e.Sid).IsUnique();

            // 03. ctr_contract_amendment
            modelBuilder.Entity<CtrContractAmendment>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<CtrContractAmendment>()
                .HasIndex(e => new { e.ContractSid, e.AmendmentNumber })
                .IsUnique()
                .HasDatabaseName("uk_cca_contract_number");

            // 04. ctr_bond_guarantee
            modelBuilder.Entity<CtrBondGuarantee>()
                .HasIndex(e => e.Sid).IsUnique();

            // 05. ctr_warranty_term
            modelBuilder.Entity<CtrWarrantyTerm>()
                .HasIndex(e => e.Sid).IsUnique();

            #endregion

            #region Composite Index Configurations - 複合索引與一般索引設定

            // 01. ctr_contract_master
            modelBuilder.Entity<CtrContractMaster>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<CtrContractMaster>()
                .HasIndex(e => e.VendorSid);
            modelBuilder.Entity<CtrContractMaster>()
                .HasIndex(e => e.ContractStatus);
            modelBuilder.Entity<CtrContractMaster>()
                .HasIndex(e => e.Avalible);

            // 02. ctr_payment_term
            modelBuilder.Entity<CtrPaymentTerm>()
                .HasIndex(e => e.ContractSid);
            modelBuilder.Entity<CtrPaymentTerm>()
                .HasIndex(e => e.Avalible);

            // 03. ctr_contract_amendment
            modelBuilder.Entity<CtrContractAmendment>()
                .HasIndex(e => e.ContractSid);
            modelBuilder.Entity<CtrContractAmendment>()
                .HasIndex(e => e.VariationOrderSid);
            modelBuilder.Entity<CtrContractAmendment>()
                .HasIndex(e => e.Avalible);

            // 04. ctr_bond_guarantee
            modelBuilder.Entity<CtrBondGuarantee>()
                .HasIndex(e => e.ContractSid);
            modelBuilder.Entity<CtrBondGuarantee>()
                .HasIndex(e => e.VendorSid);
            modelBuilder.Entity<CtrBondGuarantee>()
                .HasIndex(e => new { e.BondType, e.BondStatus });
            modelBuilder.Entity<CtrBondGuarantee>()
                .HasIndex(e => e.Avalible);

            // 05. ctr_warranty_term
            modelBuilder.Entity<CtrWarrantyTerm>()
                .HasIndex(e => e.ContractSid);
            modelBuilder.Entity<CtrWarrantyTerm>()
                .HasIndex(e => e.WarrantyStatus);
            modelBuilder.Entity<CtrWarrantyTerm>()
                .HasIndex(e => e.Avalible);

            #endregion

            #region Default Value Configurations - 預設值設定

            // 01. ctr_contract_master
            modelBuilder.Entity<CtrContractMaster>()
                .Property(e => e.ContractType).HasDefaultValue("SUBCONTRACT");
            modelBuilder.Entity<CtrContractMaster>()
                .Property(e => e.TaxType).HasDefaultValue("TAXABLE");
            modelBuilder.Entity<CtrContractMaster>()
                .Property(e => e.OriginalContractAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<CtrContractMaster>()
                .Property(e => e.OriginalTaxAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<CtrContractMaster>()
                .Property(e => e.OriginalTotalAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<CtrContractMaster>()
                .Property(e => e.CurrentContractAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<CtrContractMaster>()
                .Property(e => e.RetentionRate).HasDefaultValue(5.00m);
            modelBuilder.Entity<CtrContractMaster>()
                .Property(e => e.AdvancePaymentAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<CtrContractMaster>()
                .Property(e => e.LiquidatedDamagesRate).HasDefaultValue(0.0010m);
            modelBuilder.Entity<CtrContractMaster>()
                .Property(e => e.ContractStatus).HasDefaultValue("DRAFT");
            modelBuilder.Entity<CtrContractMaster>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<CtrContractMaster>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 02. ctr_payment_term
            modelBuilder.Entity<CtrPaymentTerm>()
                .Property(e => e.PercentageRate).HasDefaultValue(0.00m);
            modelBuilder.Entity<CtrPaymentTerm>()
                .Property(e => e.CashRatio).HasDefaultValue(100.00m);
            modelBuilder.Entity<CtrPaymentTerm>()
                .Property(e => e.TicketRatio).HasDefaultValue(0.00m);
            modelBuilder.Entity<CtrPaymentTerm>()
                .Property(e => e.TicketDays).HasDefaultValue(0);
            modelBuilder.Entity<CtrPaymentTerm>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<CtrPaymentTerm>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 03. ctr_contract_amendment
            modelBuilder.Entity<CtrContractAmendment>()
                .Property(e => e.ChangeType).HasDefaultValue("AMOUNT_AND_SCHEDULE");
            modelBuilder.Entity<CtrContractAmendment>()
                .Property(e => e.ChangeAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<CtrContractAmendment>()
                .Property(e => e.RevisedTotalAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<CtrContractAmendment>()
                .Property(e => e.ExtendedDays).HasDefaultValue(0);
            modelBuilder.Entity<CtrContractAmendment>()
                .Property(e => e.AmendmentStatus).HasDefaultValue("DRAFT");
            modelBuilder.Entity<CtrContractAmendment>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<CtrContractAmendment>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 04. ctr_bond_guarantee
            modelBuilder.Entity<CtrBondGuarantee>()
                .Property(e => e.BondType).HasDefaultValue("PERFORMANCE");
            modelBuilder.Entity<CtrBondGuarantee>()
                .Property(e => e.BondMode).HasDefaultValue("BANK_GUARANTEE");
            modelBuilder.Entity<CtrBondGuarantee>()
                .Property(e => e.BondAmount).HasDefaultValue(0.00m);
            modelBuilder.Entity<CtrBondGuarantee>()
                .Property(e => e.BondStatus).HasDefaultValue("HELD");
            modelBuilder.Entity<CtrBondGuarantee>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<CtrBondGuarantee>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 05. ctr_warranty_term
            modelBuilder.Entity<CtrWarrantyTerm>()
                .Property(e => e.WarrantyMonths).HasDefaultValue(12);
            modelBuilder.Entity<CtrWarrantyTerm>()
                .Property(e => e.WarrantyStatus).HasDefaultValue("PENDING");
            modelBuilder.Entity<CtrWarrantyTerm>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<CtrWarrantyTerm>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            #endregion
        }
    }
}