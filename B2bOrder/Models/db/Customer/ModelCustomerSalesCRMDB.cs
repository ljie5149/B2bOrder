using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Customer.Models
{
    #region CRM Lead Module - 潛在客戶檔

    [Table("crm_lead")]
    public class CrmLead
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
        [Column("lead_no")]
        public string LeadNo { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [MaxLength(32)]
        [Column("campaign_sid")]
        public string CampaignSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("source_type")]
        public string SourceType { get; set; }

        [MaxLength(32)]
        [Column("source_reference_sid")]
        public string SourceReferenceSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("lead_type")]
        public string LeadType { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("lead_name")]
        public string LeadName { get; set; }

        [MaxLength(100)]
        [Column("contact_phone")]
        public string ContactPhone { get; set; }

        [MaxLength(300)]
        [Column("contact_email")]
        public string ContactEmail { get; set; }

        [MaxLength(200)]
        [Column("contact_line_id")]
        public string ContactLineId { get; set; }

        [MaxLength(32)]
        [Column("interested_project_sid")]
        public string InterestedProjectSid { get; set; }

        [MaxLength(30)]
        [Column("interested_product_type")]
        public string InterestedProductType { get; set; }

        [Column("budget_min")]
        public decimal? BudgetMin { get; set; }

        [Column("budget_max")]
        public decimal? BudgetMax { get; set; }

        [Column("preferred_area_min")]
        public decimal? PreferredAreaMin { get; set; }

        [Column("preferred_area_max")]
        public decimal? PreferredAreaMax { get; set; }

        [MaxLength(500)]
        [Column("preferred_location")]
        public string PreferredLocation { get; set; }

        [MaxLength(30)]
        [Column("purchase_timeline")]
        public string PurchaseTimeline { get; set; }

        [MaxLength(32)]
        [Column("assigned_sales_party_sid")]
        public string AssignedSalesPartySid { get; set; }

        [Column("qualification_score")]
        public decimal QualificationScore { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("lead_status")]
        public string LeadStatus { get; set; }

        [MaxLength(32)]
        [Column("converted_customer_sid")]
        public string ConvertedCustomerSid { get; set; }

        [Column("converted_date")]
        public DateTime? ConvertedDate { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region CRM Customer Module - 客戶主檔與關係管理

    [Table("crm_customer")]
    public class CrmCustomer
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
        [Column("customer_no")]
        public string CustomerNo { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("party_sid")]
        public string PartySid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("customer_type")]
        public string CustomerType { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("customer_level")]
        public string CustomerLevel { get; set; }

        [MaxLength(32)]
        [Column("primary_contact_sid")]
        public string PrimaryContactSid { get; set; }

        [MaxLength(32)]
        [Column("sales_owner_party_sid")]
        public string SalesOwnerPartySid { get; set; }

        [Column("first_purchase_date", TypeName = "date")]
        public DateTime? FirstPurchaseDate { get; set; }

        [Column("total_purchase_amount")]
        public decimal TotalPurchaseAmount { get; set; }

        [Column("total_contract_count")]
        public int TotalContractCount { get; set; }

        [MaxLength(20)]
        [Column("preferred_contact_channel")]
        public string PreferredContactChannel { get; set; }

        [Column("consent_marketing")]
        public bool ConsentMarketing { get; set; }

        [Column("consent_date")]
        public DateTime? ConsentDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("customer_status")]
        public string CustomerStatus { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }
    }

    #endregion

    #region CRM Sales Project Module - 建案銷售主檔

    [Table("crm_sales_project")]
    public class CrmSalesProject
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
        [Column("sales_project_no")]
        public string SalesProjectNo { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("sales_project_name")]
        public string SalesProjectName { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [MaxLength(32)]
        [Column("property_sid")]
        public string PropertySid { get; set; }

        [MaxLength(32)]
        [Column("developer_party_sid")]
        public string DeveloperPartySid { get; set; }

        [MaxLength(32)]
        [Column("sales_agent_party_sid")]
        public string SalesAgentPartySid { get; set; }

        [MaxLength(32)]
        [Column("address_sid")]
        public string AddressSid { get; set; }

        [Column("launch_date", TypeName = "date")]
        public DateTime? LaunchDate { get; set; }

        [Column("sales_start_date", TypeName = "date")]
        public DateTime? SalesStartDate { get; set; }

        [Column("expected_completion_date", TypeName = "date")]
        public DateTime? ExpectedCompletionDate { get; set; }

        [Column("expected_handover_date", TypeName = "date")]
        public DateTime? ExpectedHandoverDate { get; set; }

        [Column("total_unit_count")]
        public int TotalUnitCount { get; set; }

        [Column("available_unit_count")]
        public int AvailableUnitCount { get; set; }

        [Column("reserved_unit_count")]
        public int ReservedUnitCount { get; set; }

        [Column("sold_unit_count")]
        public int SoldUnitCount { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("project_status")]
        public string ProjectStatus { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }
    }

    #endregion

    #region CRM Sales Unit Module - 房屋、店面、辦公室與車位銷控

    [Table("crm_sales_unit")]
    public class CrmSalesUnit
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
        [MaxLength(32)]
        [Column("sales_project_sid")]
        public string SalesProjectSid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("unit_no")]
        public string UnitNo { get; set; }

        [MaxLength(300)]
        [Column("unit_name")]
        public string UnitName { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("product_type")]
        public string ProductType { get; set; }

        [MaxLength(32)]
        [Column("building_sid")]
        public string BuildingSid { get; set; }

        [MaxLength(32)]
        [Column("floor_sid")]
        public string FloorSid { get; set; }

        [MaxLength(32)]
        [Column("space_sid")]
        public string SpaceSid { get; set; }

        [MaxLength(32)]
        [Column("parent_unit_sid")]
        public string ParentUnitSid { get; set; }

        [MaxLength(50)]
        [Column("orientation")]
        public string Orientation { get; set; }

        [MaxLength(100)]
        [Column("layout_code")]
        public string LayoutCode { get; set; }

        [Column("room_count")]
        public int? RoomCount { get; set; }

        [Column("hall_count")]
        public int? HallCount { get; set; }

        [Column("bathroom_count")]
        public int? BathroomCount { get; set; }

        [Column("balcony_count")]
        public int? BalconyCount { get; set; }

        [Column("registered_area")]
        public decimal RegisteredArea { get; set; }

        [Column("main_building_area")]
        public decimal MainBuildingArea { get; set; }

        [Column("accessory_area")]
        public decimal AccessoryArea { get; set; }

        [Column("common_area")]
        public decimal CommonArea { get; set; }

        [Column("land_area")]
        public decimal LandArea { get; set; }

        [MaxLength(32)]
        [Column("unit_sid_area")]
        public string UnitSidArea { get; set; }

        [Column("list_price")]
        public decimal ListPrice { get; set; }

        [Column("minimum_price")]
        public decimal MinimumPrice { get; set; }

        [Column("actual_sale_price")]
        public decimal? ActualSalePrice { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; }

        [Column("release_date", TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }

        [Column("reservation_expiry_date")]
        public DateTime? ReservationExpiryDate { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("unit_status")]
        public string UnitStatus { get; set; }

        [MaxLength(32)]
        [Column("reserved_customer_sid")]
        public string ReservedCustomerSid { get; set; }

        [MaxLength(32)]
        [Column("sales_contract_sid")]
        public string SalesContractSid { get; set; }
    }

    #endregion

    #region CRM Opportunity Module - 建案銷售商機與銷售漏斗

    [Table("crm_opportunity")]
    public class CrmOpportunity
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
        [Column("opportunity_no")]
        public string OpportunityNo { get; set; }

        [MaxLength(32)]
        [Column("customer_sid")]
        public string CustomerSid { get; set; }

        [MaxLength(32)]
        [Column("lead_sid")]
        public string LeadSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sales_project_sid")]
        public string SalesProjectSid { get; set; }

        [MaxLength(32)]
        [Column("sales_unit_sid")]
        public string SalesUnitSid { get; set; }

        [MaxLength(32)]
        [Column("sales_owner_party_sid")]
        public string SalesOwnerPartySid { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("opportunity_name")]
        public string OpportunityName { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("stage_code")]
        public string StageCode { get; set; }

        [Column("expected_amount")]
        public decimal ExpectedAmount { get; set; }

        [Column("probability_percent")]
        public decimal ProbabilityPercent { get; set; }

        [Column("expected_close_date", TypeName = "date")]
        public DateTime? ExpectedCloseDate { get; set; }

        [Column("actual_close_date", TypeName = "date")]
        public DateTime? ActualCloseDate { get; set; }

        [Column("competitor_information")]
        public string CompetitorInformation { get; set; }

        [Column("loss_reason")]
        public string LossReason { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("opportunity_status")]
        public string OpportunityStatus { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }
    }

    #endregion

    #region CRM Sales Contract Module - 房屋、店面、辦公室與車位銷售合約

    [Table("crm_sales_contract")]
    public class CrmSalesContract
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
        [Column("sales_contract_no")]
        public string SalesContractNo { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("customer_sid")]
        public string CustomerSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sales_project_sid")]
        public string SalesProjectSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sales_unit_sid")]
        public string SalesUnitSid { get; set; }

        [MaxLength(32)]
        [Column("quotation_sid")]
        public string QuotationSid { get; set; }

        [MaxLength(32)]
        [Column("reservation_sid")]
        public string ReservationSid { get; set; }

        [MaxLength(32)]
        [Column("contract_sid")]
        public string ContractSid { get; set; }

        [Column("signing_date", TypeName = "date")]
        public DateTime SigningDate { get; set; }

        [Column("contract_amount")]
        public decimal ContractAmount { get; set; }

        [Column("tax_amount")]
        public decimal TaxAmount { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; }

        [MaxLength(32)]
        [Column("payment_plan_sid")]
        public string PaymentPlanSid { get; set; }

        [Column("expected_handover_date", TypeName = "date")]
        public DateTime? ExpectedHandoverDate { get; set; }

        [Column("actual_handover_date", TypeName = "date")]
        public DateTime? ActualHandoverDate { get; set; }

        [Column("mortgage_required")]
        public bool MortgageRequired { get; set; }

        [Column("mortgage_amount")]
        public decimal MortgageAmount { get; set; }

        [MaxLength(32)]
        [Column("contract_file_sid")]
        public string ContractFileSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("contract_status")]
        public string ContractStatus { get; set; }

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string WorkflowInstanceSid { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }
    }

    #endregion
}