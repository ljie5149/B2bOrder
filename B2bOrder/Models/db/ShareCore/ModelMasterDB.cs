using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.Master
{
    #region 國家、行政區、地址 (Geography & Address)

    [Table("mst_country")]
    public class MstCountry
    {
        [Key]
        [Column("country_id")]
        public long CountryId { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("iso2")]
        public string Iso2 { get; set; } = null!;

        [Required]
        [MaxLength(3)]
        [Column("iso3")]
        public string Iso3 { get; set; } = null!;

        [MaxLength(3)]
        [Column("numeric_code")]
        public string? NumericCode { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name_en")]
        public string NameEn { get; set; } = null!;

        [MaxLength(100)]
        [Column("name_local")]
        public string? NameLocal { get; set; }

        [MaxLength(10)]
        [Column("phone_code")]
        public string? PhoneCode { get; set; }

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_region")]
    public class MstRegion
    {
        [Key]
        [Column("region_id")]
        public long RegionId { get; set; }

        [Column("country_id")]
        public long CountryId { get; set; }

        [Column("parent_id")]
        public long? ParentId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("region_code")]
        public string RegionCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("region_name")]
        public string RegionName { get; set; } = null!;

        [MaxLength(20)]
        [Column("region_type")]
        public string? RegionType { get; set; }

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// 對應 DbContext 中的 DbSet<MasterDB> Addresses 與 mst_address 資料表
    /// </summary>
    [Table("mst_address")]
    public class MasterDB
    {
        [Key]
        [Column("address_id")]
        public long AddressId { get; set; }

        [Column("country_id")]
        public long CountryId { get; set; }

        [Column("region_id")]
        public long? RegionId { get; set; }

        [MaxLength(20)]
        [Column("postal_code")]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        [Column("city")]
        public string? City { get; set; }

        [MaxLength(100)]
        [Column("district")]
        public string? District { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("address_line1")]
        public string AddressLine1 { get; set; } = null!;

        [MaxLength(255)]
        [Column("address_line2")]
        public string? AddressLine2 { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    #endregion

    #region 語系、幣別、匯率 (Language, Currency & Rates)

    [Table("mst_language")]
    public class MstLanguage
    {
        [Key]
        [Column("language_id")]
        public long LanguageId { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("lang_code")]
        public string LangCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("name_en")]
        public string NameEn { get; set; } = null!;

        [MaxLength(100)]
        [Column("name_local")]
        public string? NameLocal { get; set; }

        [Column("is_default")]
        public bool IsDefault { get; set; } = false;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_currency")]
    public class MstCurrency
    {
        [Key]
        [Column("currency_id")]
        public long CurrencyId { get; set; }

        [Required]
        [MaxLength(3)]
        [Column("currency_code")]
        public string CurrencyCode { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("currency_name")]
        public string CurrencyName { get; set; } = null!;

        [MaxLength(5)]
        [Column("symbol")]
        public string? Symbol { get; set; }

        [Column("decimal_places")]
        public byte DecimalPlaces { get; set; } = 2;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_exchange_rate")]
    public class MstExchangeRate
    {
        [Key]
        [Column("rate_id")]
        public long RateId { get; set; }

        [Required]
        [MaxLength(3)]
        [Column("from_currency")]
        public string FromCurrency { get; set; } = null!;

        [Required]
        [MaxLength(3)]
        [Column("to_currency")]
        public string ToCurrency { get; set; } = null!;

        [Column("rate", TypeName = "decimal(18,6)")]
        public decimal Rate { get; set; }

        [Column("effective_date", TypeName = "date")]
        public DateTime EffectiveDate { get; set; }

        [MaxLength(30)]
        [Column("source")]
        public string? Source { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    #endregion

    #region 公司、據點、組織 (Company & Organization)

    [Table("mst_company")]
    public class MstCompany
    {
        [Key]
        [Column("company_id")]
        public long CompanyId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("company_code")]
        public string CompanyCode { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        [Column("company_name")]
        public string CompanyName { get; set; } = null!;

        [MaxLength(50)]
        [Column("tax_id")]
        public string? TaxId { get; set; }

        [Column("country_id")]
        public long? CountryId { get; set; }

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_business_unit")]
    public class MstBusinessUnit
    {
        [Key]
        [Column("bu_id")]
        public long BuId { get; set; }

        [Column("company_id")]
        public long CompanyId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("bu_code")]
        public string BuCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("bu_name")]
        public string BuName { get; set; } = null!;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_department")]
    public class MstDepartment
    {
        [Key]
        [Column("dept_id")]
        public long DeptId { get; set; }

        [Column("company_id")]
        public long CompanyId { get; set; }

        [Column("parent_id")]
        public long? ParentId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("dept_code")]
        public string DeptCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("dept_name")]
        public string DeptName { get; set; } = null!;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_position")]
    public class MstPosition
    {
        [Key]
        [Column("position_id")]
        public long PositionId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("position_code")]
        public string PositionCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("position_name")]
        public string PositionName { get; set; } = null!;

        [Column("level")]
        public int Level { get; set; } = 1;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    #endregion

    #region 成本與專案 (Accounting & Projects)

    [Table("mst_cost_center")]
    public class MstCostCenter
    {
        [Key]
        [Column("cost_center_id")]
        public long CostCenterId { get; set; }

        [Column("company_id")]
        public long CompanyId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("code")]
        public string Code { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_profit_center")]
    public class MstProfitCenter
    {
        [Key]
        [Column("profit_center_id")]
        public long ProfitCenterId { get; set; }

        [Column("company_id")]
        public long CompanyId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("code")]
        public string Code { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_project")]
    public class MstProject
    {
        [Key]
        [Column("project_id")]
        public long ProjectId { get; set; }

        [Column("company_id")]
        public long CompanyId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("project_code")]
        public string ProjectCode { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        [Column("project_name")]
        public string ProjectName { get; set; } = null!;

        [Column("start_date", TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date", TypeName = "date")]
        public DateTime? EndDate { get; set; }

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    #endregion

    #region 單位、稅別、付款條件/方式 (Trading & Custom Settings)

    [Table("mst_unit")]
    public class MstUnit
    {
        [Key]
        [Column("unit_id")]
        public long UnitId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("unit_code")]
        public string UnitCode { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("unit_name")]
        public string UnitName { get; set; } = null!;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_tax")]
    public class MstTax
    {
        [Key]
        [Column("tax_id")]
        public long TaxId { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("tax_code")]
        public string TaxCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("tax_name")]
        public string TaxName { get; set; } = null!;

        [Column("rate", TypeName = "decimal(5,2)")]
        public decimal Rate { get; set; }

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_payment_term")]
    public class MstPaymentTerm
    {
        [Key]
        [Column("term_id")]
        public long TermId { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("term_code")]
        public string TermCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("term_name")]
        public string TermName { get; set; } = null!;

        [Column("due_days")]
        public int DueDays { get; set; } = 0;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_payment_method")]
    public class MstPaymentMethod
    {
        [Key]
        [Column("method_id")]
        public long MethodId { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("method_code")]
        public string MethodCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("method_name")]
        public string MethodName { get; set; } = null!;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    #endregion

    #region 倉庫與通用代碼 (Warehousing, Document Types & Common Codes)

    [Table("mst_warehouse")]
    public class MstWarehouse
    {
        [Key]
        [Column("warehouse_id")]
        public long WarehouseId { get; set; }

        [Column("company_id")]
        public long CompanyId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("warehouse_code")]
        public string WarehouseCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("warehouse_name")]
        public string WarehouseName { get; set; } = null!;

        [Column("address_id")]
        public long? AddressId { get; set; }

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_location")]
    public class MstLocation
    {
        [Key]
        [Column("location_id")]
        public long LocationId { get; set; }

        [Column("warehouse_id")]
        public long WarehouseId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("location_code")]
        public string LocationCode { get; set; } = null!;

        [MaxLength(100)]
        [Column("location_name")]
        public string? LocationName { get; set; }

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_document_type")]
    public class MstDocumentType
    {
        [Key]
        [Column("doc_type_id")]
        public long DocTypeId { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("doc_type_code")]
        public string DocTypeCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("doc_type_name")]
        public string DocTypeName { get; set; } = null!;

        [MaxLength(50)]
        [Column("module")]
        public string? Module { get; set; }

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_code_group")]
    public class MstCodeGroup
    {
        [Key]
        [Column("group_id")]
        public long GroupId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("group_code")]
        public string GroupCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("group_name")]
        public string GroupName { get; set; } = null!;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("mst_code_value")]
    public class MstCodeValue
    {
        [Key]
        [Column("value_id")]
        public long ValueId { get; set; }

        [Column("group_id")]
        public long GroupId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("code_value")]
        public string CodeValue { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("code_label")]
        public string CodeLabel { get; set; } = null!;

        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [Column("status")]
        public sbyte Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    #endregion
}