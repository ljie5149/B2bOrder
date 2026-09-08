using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.ShareCore.eCommerce.Models
{
    #region Core Item & Variant Module - 核心物料與變體模組

    [Table("pim_item")]
    public class PimItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("item_no")]
        public string ItemNo { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("item_type")]
        public string ItemType { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("item_name")]
        public string ItemName { get; set; }

        [MaxLength(200)]
        [Column("short_name")]
        public string ShortName { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("category_sid")]
        public string CategorySid { get; set; }

        [MaxLength(32)]
        [Column("brand_sid")]
        public string BrandSid { get; set; }

        [MaxLength(32)]
        [Column("manufacturer_party_sid")]
        public string ManufacturerPartySid { get; set; }

        [MaxLength(32)]
        [Column("preferred_supplier_sid")]
        public string PreferredSupplierSid { get; set; }

        [MaxLength(32)]
        [Column("base_unit_sid")]
        public string BaseUnitSid { get; set; }

        [MaxLength(32)]
        [Column("tax_sid")]
        public string TaxSid { get; set; }

        [MaxLength(32)]
        [Column("origin_country_sid")]
        public string OriginCountrySid { get; set; }

        [MaxLength(150)]
        [Column("model_no")]
        public string ModelNo { get; set; }

        [MaxLength(150)]
        [Column("manufacturer_no")]
        public string ManufacturerNo { get; set; }

        [MaxLength(100)]
        [Column("primary_barcode")]
        public string PrimaryBarcode { get; set; }

        [Column("serial_control")]
        public bool SerialControl { get; set; }

        [Column("batch_control")]
        public bool BatchControl { get; set; }

        [Column("shelf_life_days")]
        public int? ShelfLifeDays { get; set; }

        [Column("warranty_months")]
        public int? WarrantyMonths { get; set; }

        [Column("hazardous_mark")]
        public bool HazardousMark { get; set; }

        [Column("restricted_mark")]
        public bool RestrictedMark { get; set; }

        [Column("virtual_mark")]
        public bool VirtualMark { get; set; }

        [Column("reusable_mark")]
        public bool ReusableMark { get; set; }

        [Column("asset_mark")]
        public bool AssetMark { get; set; }

        [Column("content_completeness")]
        public decimal ContentCompleteness { get; set; }

        [Column("current_version")]
        public int CurrentVersion { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("item_status")]
        public string ItemStatus { get; set; }

        [MaxLength(32)]
        [Column("owner_user_sid")]
        public string OwnerUserSid { get; set; }

        [MaxLength(32)]
        [Column("mdm_golden_sid")]
        public string MdmGoldenSid { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }

        #region Navigation Properties - 導覽屬性
        public virtual ICollection<PimItemTranslation> Translations { get; set; } = new List<PimItemTranslation>();
        public virtual ICollection<PimVariant> Variants { get; set; } = new List<PimVariant>();
        public virtual ICollection<PimIdentifier> Identifiers { get; set; } = new List<PimIdentifier>();
        public virtual ICollection<PimAttributeValue> AttributeValues { get; set; } = new List<PimAttributeValue>();
        public virtual ICollection<PimMedia> MediaList { get; set; } = new List<PimMedia>();
        public virtual ICollection<PimCertification> Certifications { get; set; } = new List<PimCertification>();
        public virtual ICollection<PimItemParty> ItemParties { get; set; } = new List<PimItemParty>();
        public virtual ICollection<PimItemRelation> ItemRelations { get; set; } = new List<PimItemRelation>();
        public virtual ICollection<PimApplicationScope> ApplicationScopes { get; set; } = new List<PimApplicationScope>();
        public virtual ICollection<PimStructure> Structures { get; set; } = new List<PimStructure>();
        public virtual ICollection<PimItemVersion> ItemVersions { get; set; } = new List<PimItemVersion>();
        public virtual ICollection<PimTargetItem> TargetItems { get; set; } = new List<PimTargetItem>();
        #endregion
    }

    [Table("pim_item_translation")]
    public class PimItemTranslation
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("item_nid")]
        public ulong ItemNid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("language_sid")]
        public string LanguageSid { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("item_name")]
        public string ItemName { get; set; }

        [MaxLength(200)]
        [Column("short_name")]
        public string ShortName { get; set; }

        [MaxLength(500)]
        [Column("subtitle")]
        public string Subtitle { get; set; }

        [Column("short_description")]
        public string ShortDescription { get; set; }

        [Column("full_description")]
        public string FullDescription { get; set; }

        [Column("usage_instruction")]
        public string UsageInstruction { get; set; }

        [Column("warning_text")]
        public string WarningText { get; set; }

        [Column("warranty_text")]
        public string WarrantyText { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("translation_status")]
        public string TranslationStatus { get; set; }

        [MaxLength(32)]
        [Column("translator_user_sid")]
        public string TranslatorUserSid { get; set; }

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(ItemNid))]
        public virtual PimItem Item { get; set; }
        #endregion
    }

    [Table("pim_variant")]
    public class PimVariant
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("item_nid")]
        public ulong ItemNid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("variant_no")]
        public string VariantNo { get; set; }

        [MaxLength(500)]
        [Column("variant_name")]
        public string VariantName { get; set; }

        [MaxLength(100)]
        [Column("barcode")]
        public string Barcode { get; set; }

        [MaxLength(150)]
        [Column("manufacturer_variant_no")]
        public string ManufacturerVariantNo { get; set; }

        [MaxLength(150)]
        [Column("model_no")]
        public string ModelNo { get; set; }

        [Column("weight")]
        public decimal? Weight { get; set; }

        [MaxLength(32)]
        [Column("weight_unit_sid")]
        public string WeightUnitSid { get; set; }

        [Column("length")]
        public decimal? Length { get; set; }

        [Column("width")]
        public decimal? Width { get; set; }

        [Column("height")]
        public decimal? Height { get; set; }

        [MaxLength(32)]
        [Column("dimension_unit_sid")]
        public string DimensionUnitSid { get; set; }

        [Column("volume")]
        public decimal? Volume { get; set; }

        [Column("minimum_order_qty")]
        public decimal MinimumOrderQty { get; set; }

        [Column("order_multiple_qty")]
        public decimal OrderMultipleQty { get; set; }

        [Column("default_mark")]
        public bool DefaultMark { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("variant_status")]
        public string VariantStatus { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(ItemNid))]
        public virtual PimItem Item { get; set; }

        public virtual ICollection<PimIdentifier> Identifiers { get; set; } = new List<PimIdentifier>();
        public virtual ICollection<PimVariantSpecification> VariantSpecifications { get; set; } = new List<PimVariantSpecification>();
        public virtual ICollection<PimAttributeValue> AttributeValues { get; set; } = new List<PimAttributeValue>();
        public virtual ICollection<PimMedia> MediaList { get; set; } = new List<PimMedia>();
        public virtual ICollection<PimCertification> Certifications { get; set; } = new List<PimCertification>();
        public virtual ICollection<PimItemParty> ItemParties { get; set; } = new List<PimItemParty>();
        #endregion
    }

    [Table("pim_identifier")]
    public class PimIdentifier
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("item_nid")]
        public ulong ItemNid { get; set; }

        [Column("variant_nid")]
        public ulong? VariantNid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("identifier_type")]
        public string IdentifierType { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("identifier_value")]
        public string IdentifierValue { get; set; }

        [MaxLength(32)]
        [Column("issuer_party_sid")]
        public string IssuerPartySid { get; set; }

        [Column("primary_mark")]
        public bool PrimaryMark { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("identifier_status")]
        public string IdentifierStatus { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(ItemNid))]
        public virtual PimItem Item { get; set; }

        [ForeignKey(nameof(VariantNid))]
        public virtual PimVariant Variant { get; set; }
        #endregion
    }

    #endregion

    #region Specification & Options Module - 規格與選配模組

    [Table("pim_specification")]
    public class PimSpecification
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("specification_code")]
        public string SpecificationCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("specification_name")]
        public string SpecificationName { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("specification_category")]
        public string SpecificationCategory { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("display_type")]
        public string DisplayType { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("selection_mode")]
        public string SelectionMode { get; set; }

        [Column("required_mark")]
        public bool RequiredMark { get; set; }

        [Column("sort_no")]
        public int SortNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }

        #region Navigation Properties - 導覽屬性
        public virtual ICollection<PimSpecificationValue> SpecificationValues { get; set; } = new List<PimSpecificationValue>();
        public virtual ICollection<PimVariantSpecification> VariantSpecifications { get; set; } = new List<PimVariantSpecification>();
        #endregion
    }

    [Table("pim_specification_value")]
    public class PimSpecificationValue
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("specification_nid")]
        public ulong SpecificationNid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("value_code")]
        public string ValueCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("value_name")]
        public string ValueName { get; set; }

        [MaxLength(500)]
        [Column("display_value")]
        public string DisplayValue { get; set; }

        [Column("numeric_value")]
        public decimal? NumericValue { get; set; }

        [MaxLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; }

        [MaxLength(20)]
        [Column("color_code")]
        public string ColorCode { get; set; }

        [MaxLength(32)]
        [Column("image_file_sid")]
        public string ImageFileSid { get; set; }

        [Column("sort_no")]
        public int SortNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(SpecificationNid))]
        public virtual PimSpecification Specification { get; set; }

        public virtual ICollection<PimVariantSpecification> VariantSpecifications { get; set; } = new List<PimVariantSpecification>();
        #endregion
    }

    [Table("pim_variant_specification")]
    public class PimVariantSpecification
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("variant_nid")]
        public ulong VariantNid { get; set; }

        [Column("specification_nid")]
        public ulong SpecificationNid { get; set; }

        [Column("specification_value_nid")]
        public ulong SpecificationValueNid { get; set; }

        [Column("sort_no")]
        public int SortNo { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(VariantNid))]
        public virtual PimVariant Variant { get; set; }

        [ForeignKey(nameof(SpecificationNid))]
        public virtual PimSpecification Specification { get; set; }

        [ForeignKey(nameof(SpecificationValueNid))]
        public virtual PimSpecificationValue SpecificationValue { get; set; }
        #endregion
    }

    #endregion

    #region Dynamic Attribute Module - 動態擴充屬性模組

    [Table("pim_attribute_group")]
    public class PimAttributeGroup
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("group_code")]
        public string GroupCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("group_name")]
        public string GroupName { get; set; }

        [MaxLength(32)]
        [Column("category_sid")]
        public string CategorySid { get; set; }

        [MaxLength(30)]
        [Column("item_type")]
        public string ItemType { get; set; }

        [Column("sort_no")]
        public int SortNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }

        #region Navigation Properties - 導覽屬性
        public virtual ICollection<PimAttribute> Attributes { get; set; } = new List<PimAttribute>();
        #endregion
    }

    [Table("pim_attribute")]
    public class PimAttribute
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("attribute_group_nid")]
        public ulong? AttributeGroupNid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("attribute_code")]
        public string AttributeCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("attribute_name")]
        public string AttributeName { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("data_type")]
        public string DataType { get; set; }

        [MaxLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; }

        [Column("required_mark")]
        public bool RequiredMark { get; set; }

        [Column("searchable_mark")]
        public bool SearchableMark { get; set; }

        [Column("filterable_mark")]
        public bool FilterableMark { get; set; }

        [Column("comparable_mark")]
        public bool ComparableMark { get; set; }

        [Column("variant_mark")]
        public bool VariantMark { get; set; }

        [Column("technical_mark")]
        public bool TechnicalMark { get; set; }

        [Column("compliance_mark")]
        public bool ComplianceMark { get; set; }

        [Column("validation_rule")]
        public string ValidationRule { get; set; }

        [Column("default_value")]
        public string DefaultValue { get; set; }

        [Column("sort_no")]
        public int SortNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(AttributeGroupNid))]
        public virtual PimAttributeGroup AttributeGroup { get; set; }

        public virtual ICollection<PimAttributeOption> AttributeOptions { get; set; } = new List<PimAttributeOption>();
        public virtual ICollection<PimAttributeValue> AttributeValues { get; set; } = new List<PimAttributeValue>();
        #endregion
    }

    [Table("pim_attribute_option")]
    public class PimAttributeOption
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("attribute_nid")]
        public ulong AttributeNid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("option_code")]
        public string OptionCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("option_name")]
        public string OptionName { get; set; }

        [Column("sort_no")]
        public int SortNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(AttributeNid))]
        public virtual PimAttribute Attribute { get; set; }
        #endregion
    }

    [Table("pim_attribute_value")]
    public class PimAttributeValue
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("item_nid")]
        public ulong ItemNid { get; set; }

        [Column("variant_nid")]
        public ulong? VariantNid { get; set; }

        [Column("attribute_nid")]
        public ulong AttributeNid { get; set; }

        [Column("value_text")]
        public string ValueText { get; set; }

        [Column("value_number")]
        public decimal? ValueNumber { get; set; }

        [Column("value_boolean")]
        public bool? ValueBoolean { get; set; }

        [Column("value_date", TypeName = "date")]
        public DateTime? ValueDate { get; set; }

        [MaxLength(32)]
        [Column("option_sid")]
        public string OptionSid { get; set; }

        [Column("multi_value_json")]
        public string MultiValueJson { get; set; }

        [MaxLength(32)]
        [Column("language_sid")]
        public string LanguageSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("source_type")]
        public string SourceType { get; set; }

        [Column("confidence_score")]
        public decimal? ConfidenceScore { get; set; }

        [Column("approved_mark")]
        public bool ApprovedMark { get; set; }

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(ItemNid))]
        public virtual PimItem Item { get; set; }

        [ForeignKey(nameof(VariantNid))]
        public virtual PimVariant Variant { get; set; }

        [ForeignKey(nameof(AttributeNid))]
        public virtual PimAttribute Attribute { get; set; }
        #endregion
    }

    #endregion

    #region Media Resources & Certification Module - 多媒體資源與認證檢驗模組

    [Table("pim_media")]
    public class PimMedia
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("item_nid")]
        public ulong ItemNid { get; set; }

        [Column("variant_nid")]
        public ulong? VariantNid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("media_type")]
        public string MediaType { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("file_sid")]
        public string FileSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("usage_type")]
        public string UsageType { get; set; }

        [MaxLength(32)]
        [Column("language_sid")]
        public string LanguageSid { get; set; }

        [MaxLength(500)]
        [Column("alt_text")]
        public string AltText { get; set; }

        [MaxLength(500)]
        [Column("title")]
        public string Title { get; set; }

        [Column("sort_no")]
        public int SortNo { get; set; }

        [Column("primary_mark")]
        public bool PrimaryMark { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("media_status")]
        public string MediaStatus { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(ItemNid))]
        public virtual PimItem Item { get; set; }

        [ForeignKey(nameof(VariantNid))]
        public virtual PimVariant Variant { get; set; }
        #endregion
    }

    [Table("pim_certification")]
    public class PimCertification
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("item_nid")]
        public ulong ItemNid { get; set; }

        [Column("variant_nid")]
        public ulong? VariantNid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("certification_type")]
        public string CertificationType { get; set; }

        [MaxLength(100)]
        [Column("certification_code")]
        public string CertificationCode { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("certification_name")]
        public string CertificationName { get; set; }

        [MaxLength(32)]
        [Column("issuing_party_sid")]
        public string IssuingPartySid { get; set; }

        [MaxLength(150)]
        [Column("certificate_no")]
        public string CertificateNo { get; set; }

        [Column("issue_date", TypeName = "date")]
        public DateTime? IssueDate { get; set; }

        [Column("expiry_date", TypeName = "date")]
        public DateTime? ExpiryDate { get; set; }

        [MaxLength(32)]
        [Column("file_sid")]
        public string FileSid { get; set; }

        [Column("verified_mark")]
        public bool VerifiedMark { get; set; }

        [MaxLength(32)]
        [Column("verified_user_sid")]
        public string VerifiedUserSid { get; set; }

        [Column("verified_date")]
        public DateTime? VerifiedDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("certification_status")]
        public string CertificationStatus { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(ItemNid))]
        public virtual PimItem Item { get; set; }

        [ForeignKey(nameof(VariantNid))]
        public virtual PimVariant Variant { get; set; }
        #endregion
    }

    #endregion

    #region Relationships & Application Scope Module - 關係關聯與套用範圍模組

    [Table("pim_item_party")]
    public class PimItemParty
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("item_nid")]
        public ulong ItemNid { get; set; }

        [Column("variant_nid")]
        public ulong? VariantNid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("party_sid")]
        public string PartySid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("relationship_type")]
        public string RelationshipType { get; set; }

        [MaxLength(150)]
        [Column("party_item_no")]
        public string PartyItemNo { get; set; }

        [Column("preferred_mark")]
        public bool PreferredMark { get; set; }

        [Column("approved_mark")]
        public bool ApprovedMark { get; set; }

        [Column("lead_time_days")]
        public int LeadTimeDays { get; set; }

        [Column("minimum_order_qty")]
        public decimal? MinimumOrderQty { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("relation_status")]
        public string RelationStatus { get; set; }

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string WorkflowInstanceSid { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(ItemNid))]
        public virtual PimItem Item { get; set; }

        [ForeignKey(nameof(VariantNid))]
        public virtual PimVariant Variant { get; set; }
        #endregion
    }

    [Table("pim_item_relation")]
    public class PimItemRelation
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("item_nid")]
        public ulong ItemNid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("related_item_sid")]
        public string RelatedItemSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("relation_type")]
        public string RelationType { get; set; }

        [Column("priority")]
        public int Priority { get; set; }

        [MaxLength(20)]
        [Column("equivalence_level")]
        public string EquivalenceLevel { get; set; }

        [Column("condition_json")]
        public string ConditionJson { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("approved_mark")]
        public bool ApprovedMark { get; set; }

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string WorkflowInstanceSid { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(ItemNid))]
        public virtual PimItem Item { get; set; }
        #endregion
    }

    [Table("pim_application_scope")]
    public class PimApplicationScope
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("item_nid")]
        public ulong ItemNid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("scope_type")]
        public string ScopeType { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("scope_sid")]
        public string ScopeSid { get; set; }

        [Column("allowed_mark")]
        public bool AllowedMark { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("approval_status")]
        public string ApprovalStatus { get; set; }

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string WorkflowInstanceSid { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(ItemNid))]
        public virtual PimItem Item { get; set; }
        #endregion
    }

    #endregion

    #region Structure & Versioning Module - 組合結構與版本管控模組

    [Table("pim_structure")]
    public class PimStructure
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("item_nid")]
        public ulong ItemNid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("structure_type")]
        public string StructureType { get; set; }

        [Column("structure_version")]
        public int StructureVersion { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("inventory_policy")]
        public string InventoryPolicy { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("pricing_policy")]
        public string PricingPolicy { get; set; }

        [Column("effective_start_date")]
        public DateTime? EffectiveStartDate { get; set; }

        [Column("effective_end_date")]
        public DateTime? EffectiveEndDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("structure_status")]
        public string StructureStatus { get; set; }

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string WorkflowInstanceSid { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(ItemNid))]
        public virtual PimItem Item { get; set; }

        public virtual ICollection<PimStructureComponent> Components { get; set; } = new List<PimStructureComponent>();
        #endregion
    }

    [Table("pim_structure_component")]
    public class PimStructureComponent
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("structure_nid")]
        public ulong StructureNid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("component_item_sid")]
        public string ComponentItemSid { get; set; }

        [MaxLength(32)]
        [Column("component_variant_sid")]
        public string ComponentVariantSid { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [MaxLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; }

        [Column("waste_rate")]
        public decimal WasteRate { get; set; }

        [Column("required_mark")]
        public bool RequiredMark { get; set; }

        [MaxLength(100)]
        [Column("selection_group")]
        public string SelectionGroup { get; set; }

        [Column("minimum_select_qty")]
        public decimal? MinimumSelectQty { get; set; }

        [Column("maximum_select_qty")]
        public decimal? MaximumSelectQty { get; set; }

        [Column("sort_no")]
        public int SortNo { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(StructureNid))]
        public virtual PimStructure Structure { get; set; }
        #endregion
    }

    [Table("pim_item_version")]
    public class PimItemVersion
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("item_nid")]
        public ulong ItemNid { get; set; }

        [Column("version_no")]
        public int VersionNo { get; set; }

        [MaxLength(200)]
        [Column("version_name")]
        public string VersionName { get; set; }

        [Required]
        [Column("snapshot_data")]
        public string SnapshotData { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("change_type")]
        public string ChangeType { get; set; }

        [MaxLength(1000)]
        [Column("change_summary")]
        public string ChangeSummary { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("version_status")]
        public string VersionStatus { get; set; }

        [MaxLength(32)]
        [Column("create_user_sid")]
        public string CreateUserSid { get; set; }

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        [Column("published_date")]
        public DateTime? PublishedDate { get; set; }

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string WorkflowInstanceSid { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(ItemNid))]
        public virtual PimItem Item { get; set; }
        #endregion
    }

    #endregion

    #region Publishing & Integration Module - 發布與第三方同步模組

    [Table("pim_publish_target")]
    public class PimPublishTarget
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(80)]
        [Column("target_code")]
        public string TargetCode { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("target_name")]
        public string TargetName { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("target_type")]
        public string TargetType { get; set; }

        [MaxLength(80)]
        [Column("service_code")]
        public string ServiceCode { get; set; }

        [MaxLength(32)]
        [Column("default_language_sid")]
        public string DefaultLanguageSid { get; set; }

        [Column("required_fields")]
        public string RequiredFields { get; set; }

        [Column("validation_rules")]
        public string ValidationRules { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("target_status")]
        public string TargetStatus { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }

        #region Navigation Properties - 導覽屬性
        public virtual ICollection<PimTargetItem> TargetItems { get; set; } = new List<PimTargetItem>();
        #endregion
    }

    [Table("pim_target_item")]
    public class PimTargetItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("publish_target_nid")]
        public ulong PublishTargetNid { get; set; }

        [Column("item_nid")]
        public ulong ItemNid { get; set; }

        [MaxLength(200)]
        [Column("external_item_id")]
        public string ExternalItemId { get; set; }

        [MaxLength(200)]
        [Column("external_item_no")]
        public string ExternalItemNo { get; set; }

        [Column("published_version_no")]
        public int? PublishedVersionNo { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("desired_status")]
        public string DesiredStatus { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("actual_status")]
        public string ActualStatus { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("publish_status")]
        public string PublishStatus { get; set; }

        [Column("scheduled_publish_date")]
        public DateTime? ScheduledPublishDate { get; set; }

        [Column("last_publish_date")]
        public DateTime? LastPublishDate { get; set; }

        [Column("last_sync_date")]
        public DateTime? LastSyncDate { get; set; }

        [Column("validation_result")]
        public string ValidationResult { get; set; }

        [Column("last_error")]
        public string LastError { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(PublishTargetNid))]
        public virtual PimPublishTarget PublishTarget { get; set; }

        [ForeignKey(nameof(ItemNid))]
        public virtual PimItem Item { get; set; }

        public virtual ICollection<PimPublishJob> PublishJobs { get; set; } = new List<PimPublishJob>();
        #endregion
    }

    [Table("pim_publish_job")]
    public class PimPublishJob
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("job_no")]
        public string JobNo { get; set; }

        [Column("target_item_nid")]
        public ulong TargetItemNid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("operation_type")]
        public string OperationType { get; set; }

        [Column("item_version_no")]
        public int ItemVersionNo { get; set; }

        [Column("request_payload")]
        public string RequestPayload { get; set; }

        [Column("response_payload")]
        public string ResponsePayload { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("publish_status")]
        public string PublishStatus { get; set; }

        [Column("retry_count")]
        public int RetryCount { get; set; }

        [Column("max_retry_count")]
        public int MaxRetryCount { get; set; }

        [Column("next_retry_date")]
        public DateTime? NextRetryDate { get; set; }

        [MaxLength(200)]
        [Column("external_reference_no")]
        public string ExternalReferenceNo { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [MaxLength(100)]
        [Column("error_code")]
        public string ErrorCode { get; set; }

        [Column("error_message")]
        public string ErrorMessage { get; set; }

        [MaxLength(100)]
        [Column("correlation_id")]
        public string CorrelationId { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(TargetItemNid))]
        public virtual PimTargetItem TargetItem { get; set; }
        #endregion
    }

    #endregion

    #region Data Import Module - 資料批次匯入模組

    [Table("pim_import_job")]
    public class PimImportJob
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("job_no")]
        public string JobNo { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("import_type")]
        public string ImportType { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("source_type")]
        public string SourceType { get; set; }

        [MaxLength(32)]
        [Column("source_file_sid")]
        public string SourceFileSid { get; set; }

        [MaxLength(32)]
        [Column("source_party_sid")]
        public string SourcePartySid { get; set; }

        [Column("mapping_config")]
        public string MappingConfig { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("import_status")]
        public string ImportStatus { get; set; }

        [Column("total_count")]
        public int TotalCount { get; set; }

        [Column("success_count")]
        public int SuccessCount { get; set; }

        [Column("fail_count")]
        public int FailCount { get; set; }

        [Column("skip_count")]
        public int SkipCount { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [MaxLength(32)]
        [Column("create_user_sid")]
        public string CreateUserSid { get; set; }

        [Column("error_message")]
        public string ErrorMessage { get; set; }

        #region Navigation Properties - 導覽屬性
        public virtual ICollection<PimImportError> ImportErrors { get; set; } = new List<PimImportError>();
        #endregion
    }

    [Table("pim_import_error")]
    public class PimImportError
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("import_job_nid")]
        public ulong ImportJobNid { get; set; }

        [Column("row_no")]
        public int? RowNo { get; set; }

        [MaxLength(200)]
        [Column("source_identifier")]
        public string SourceIdentifier { get; set; }

        [MaxLength(100)]
        [Column("item_no")]
        public string ItemNo { get; set; }

        [MaxLength(100)]
        [Column("variant_no")]
        public string VariantNo { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("error_code")]
        public string ErrorCode { get; set; }

        [MaxLength(200)]
        [Column("error_field")]
        public string ErrorField { get; set; }

        [Required]
        [Column("error_message")]
        public string ErrorMessage { get; set; }

        [Column("source_data")]
        public string SourceData { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("error_status")]
        public string ErrorStatus { get; set; }

        [MaxLength(32)]
        [Column("resolved_user_sid")]
        public string ResolvedUserSid { get; set; }

        [Column("resolved_date")]
        public DateTime? ResolvedDate { get; set; }

        #region Navigation Properties - 導覽屬性
        [ForeignKey(nameof(ImportJobNid))]
        public virtual PimImportJob ImportJob { get; set; }
        #endregion
    }

    #endregion

    #region Data Quality Audit Module - 資料品質稽核模組

    [Table("pim_quality_rule")]
    public class PimQualityRule
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("rule_code")]
        public string RuleCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("rule_name")]
        public string RuleName { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("applies_to")]
        public string AppliesTo { get; set; }

        [MaxLength(30)]
        [Column("item_type")]
        public string ItemType { get; set; }

        [MaxLength(32)]
        [Column("category_sid")]
        public string CategorySid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("severity")]
        public string Severity { get; set; }

        [Required]
        [Column("rule_expression")]
        public string RuleExpression { get; set; }

        [Column("rule_config")]
        public string RuleConfig { get; set; }

        [Column("score_weight")]
        public decimal ScoreWeight { get; set; }

        [Column("blocking_mark")]
        public bool BlockingMark { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("rule_status")]
        public string RuleStatus { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    [Table("pim_quality_result")]
    public class PimQualityResult
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("item_sid")]
        public string ItemSid { get; set; }

        [MaxLength(32)]
        [Column("variant_sid")]
        public string VariantSid { get; set; }

        [MaxLength(32)]
        [Column("target_sid")]
        public string TargetSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("quality_rule_sid")]
        public string QualityRuleSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("check_result")]
        public string CheckResult { get; set; }

        [Column("score")]
        public decimal Score { get; set; }

        [MaxLength(1000)]
        [Column("message")]
        public string Message { get; set; }

        [Column("details_json")]
        public string DetailsJson { get; set; }

        [Column("resolved")]
        public bool Resolved { get; set; }

        [MaxLength(32)]
        [Column("resolved_user_sid")]
        public string ResolvedUserSid { get; set; }

        [Column("resolved_date")]
        public DateTime? ResolvedDate { get; set; }
    }

    #endregion
}