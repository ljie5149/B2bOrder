using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Customer.Construction.Models
{
    #region Property Unit Master & Inventory Module - 戶別銷控與房屋資產主檔模組

    [Table("sal_property_unit")]
    public class SalPropertyUnit
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
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("building_block")]
        public string BuildingBlock { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("floor_level")]
        public string FloorLevel { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("unit_number")]
        public string UnitNumber { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("full_unit_code")]
        public string FullUnitCode { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("unit_type")]
        public string UnitType { get; set; }

        [MaxLength(50)]
        [Column("layout_type")]
        public string LayoutType { get; set; }

        [Column("main_building_area_ping")]
        public decimal MainBuildingAreaPing { get; set; }

        [Column("balcony_area_ping")]
        public decimal BalconyAreaPing { get; set; }

        [Column("common_area_ping")]
        public decimal CommonAreaPing { get; set; }

        [Column("total_sales_area_ping")]
        public decimal TotalSalesAreaPing { get; set; }

        [Column("list_price_house")]
        public decimal ListPriceHouse { get; set; }

        [Column("list_price_land")]
        public decimal ListPriceLand { get; set; }

        [Column("list_price_total")]
        public decimal ListPriceTotal { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("sales_status")]
        public string SalesStatus { get; set; }

        [MaxLength(32)]
        [Column("current_contract_sid")]
        public string CurrentContractSid { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Sales Order & Purchase Contract Module - 購屋訂單與正式買賣契約模組

    [Table("sal_sales_contract")]
    public class SalSalesContract
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
        [Column("company_sid")]
        public string CompanySid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("unit_sid")]
        public string UnitSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("contract_no")]
        public string ContractNo { get; set; }

        [Column("order_date", TypeName = "date")]
        public DateTime OrderDate { get; set; }

        [Column("signing_date", TypeName = "date")]
        public DateTime? SigningDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("buyer_customer_sid")]
        public string BuyerCustomerSid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("buyer_name")]
        public string BuyerName { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("buyer_id_number")]
        public string BuyerIdNumber { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("buyer_phone")]
        public string BuyerPhone { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("buyer_address")]
        public string BuyerAddress { get; set; }

        [Column("house_agreed_price")]
        public decimal HouseAgreedPrice { get; set; }

        [Column("land_agreed_price")]
        public decimal LandAgreedPrice { get; set; }

        [Column("parking_agreed_price")]
        public decimal ParkingAgreedPrice { get; set; }

        [Column("total_contract_price")]
        public decimal TotalContractPrice { get; set; }

        [MaxLength(32)]
        [Column("agency_vendor_sid")]
        public string AgencyVendorSid { get; set; }

        [MaxLength(100)]
        [Column("sales_agent_name")]
        public string SalesAgentName { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("contract_status")]
        public string ContractStatus { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Payment Installment Schedule Module - 分期應繳款項與工程期款期程模組

    [Table("sal_payment_installment")]
    public class SalPaymentInstallment
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
        [Column("contract_sid")]
        public string ContractSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("installment_stage")]
        public string InstallmentStage { get; set; }

        [Column("stage_sequence")]
        public int StageSequence { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("stage_description")]
        public string StageDescription { get; set; }

        [Column("due_date", TypeName = "date")]
        public DateTime DueDate { get; set; }

        [Column("due_amount")]
        public decimal DueAmount { get; set; }

        [Column("paid_amount")]
        public decimal PaidAmount { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("payment_status")]
        public string PaymentStatus { get; set; }

        [Column("last_paid_at")]
        public DateTime? LastPaidAt { get; set; }

        [MaxLength(32)]
        [Column("ar_invoice_sid")]
        public string ArInvoiceSid { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion

    #region Customer Change Request Log Module - 客戶客變申請與追加減帳單模組

    [Table("sal_customer_change_order")]
    public class SalCustomerChangeOrder
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
        [Column("contract_sid")]
        public string ContractSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("change_order_no")]
        public string ChangeOrderNo { get; set; }

        [Column("request_date", TypeName = "date")]
        public DateTime RequestDate { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("change_category")]
        public string ChangeCategory { get; set; }

        [Required]
        [Column("change_description")]
        public string ChangeDescription { get; set; }

        [Column("add_amount")]
        public decimal AddAmount { get; set; }

        [Column("deduct_amount")]
        public decimal DeductAmount { get; set; }

        [Column("net_change_amount")]
        public decimal NetChangeAmount { get; set; }

        [MaxLength(32)]
        [Column("drawing_document_sid")]
        public string DrawingDocumentSid { get; set; }

        [MaxLength(32)]
        [Column("architect_approved_sid")]
        public string ArchitectApprovedSid { get; set; }

        [Column("customer_signed_at")]
        public DateTime? CustomerSignedAt { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; }

        [ConcurrencyCheck]
        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; }

        [Column("remark")]
        public string Remark { get; set; }
    }

    #endregion
}