using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Customer.Construction.Models
{
    #region Property Listing Master Module - 房屋資產委託刊登檔模組

    [Table("res_property_listing")]
    public class ResPropertyListing
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
        [Column("branch_sid")]
        public string BranchSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("agent_user_sid")]
        public string AgentUserSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("listing_code")]
        public string ListingCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("property_name")]
        public string PropertyName { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("property_type")]
        public string PropertyType { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("listing_deal_type")]
        public string ListingDealType { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("owner_customer_sid")]
        public string OwnerCustomerSid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("owner_name")]
        public string OwnerName { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("owner_phone")]
        public string OwnerPhone { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("agency_contract_type")]
        public string AgencyContractType { get; set; }

        [Column("agency_start_date", TypeName = "date")]
        public DateTime AgencyStartDate { get; set; }

        [Column("agency_end_date", TypeName = "date")]
        public DateTime AgencyEndDate { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("city")]
        public string City { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("district")]
        public string District { get; set; }

        [Required]
        [MaxLength(250)]
        [Column("address")]
        public string Address { get; set; }

        [Column("building_age")]
        public decimal BuildingAge { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("floor_current")]
        public string FloorCurrent { get; set; }

        [Column("floor_total")]
        public int FloorTotal { get; set; }

        [Column("main_building_area_ping")]
        public decimal MainBuildingAreaPing { get; set; }

        [Column("balcony_area_ping")]
        public decimal BalconyAreaPing { get; set; }

        [Column("common_area_ping")]
        public decimal CommonAreaPing { get; set; }

        [Column("parking_area_ping")]
        public decimal ParkingAreaPing { get; set; }

        [Column("total_ping")]
        public decimal TotalPing { get; set; }

        [Column("target_price")]
        public decimal TargetPrice { get; set; }

        [Column("bottom_price")]
        public decimal BottomPrice { get; set; }

        [Column("estimated_monthly_rent")]
        public decimal EstimatedMonthlyRent { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("listing_status")]
        public string ListingStatus { get; set; }

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

    #region Property Showing Log Module - 帶看與客戶意向紀錄檔模組

    [Table("res_property_showing")]
    public class ResPropertyShowing
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
        [Column("property_listing_sid")]
        public string PropertyListingSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("showing_agent_user_sid")]
        public string ShowingAgentUserSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("buyer_customer_sid")]
        public string BuyerCustomerSid { get; set; }

        [Column("showing_time")]
        public DateTime ShowingTime { get; set; }

        [Column("buyer_feedback_rating")]
        public int BuyerFeedbackRating { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("buyer_intent_level")]
        public string BuyerIntentLevel { get; set; }

        [Column("buyer_comments")]
        public string BuyerComments { get; set; }

        [MaxLength(200)]
        [Column("follow_up_action")]
        public string FollowUpAction { get; set; }

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

    #region Offer & Earnest Money Module - 要約書與斡旋金管理檔模組

    [Table("res_property_offer")]
    public class ResPropertyOffer
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
        [Column("property_listing_sid")]
        public string PropertyListingSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("buyer_customer_sid")]
        public string BuyerCustomerSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("agent_user_sid")]
        public string AgentUserSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("offer_code")]
        public string OfferCode { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("offer_type")]
        public string OfferType { get; set; }

        [Column("offered_price")]
        public decimal OfferedPrice { get; set; }

        [Column("earnest_money_amount")]
        public decimal EarnestMoneyAmount { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("earnest_payment_method")]
        public string EarnestPaymentMethod { get; set; }

        [Column("valid_until_date")]
        public DateTime ValidUntilDate { get; set; }

        [Column("owner_signed_date")]
        public DateTime? OwnerSignedDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("offer_status")]
        public string OfferStatus { get; set; }

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

    #region Sales Contract & Commission Split Module - 成交買賣契約與傭金拆帳檔模組

    [Table("res_sales_contract")]
    public class ResSalesContract
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
        [Column("branch_sid")]
        public string BranchSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("property_listing_sid")]
        public string PropertyListingSid { get; set; }

        [MaxLength(32)]
        [Column("offer_sid")]
        public string OfferSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("contract_no")]
        public string ContractNo { get; set; }

        [Column("signing_date", TypeName = "date")]
        public DateTime SigningDate { get; set; }

        [Column("handover_date", TypeName = "date")]
        public DateTime? HandoverDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("buyer_customer_sid")]
        public string BuyerCustomerSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("owner_customer_sid")]
        public string OwnerCustomerSid { get; set; }

        [MaxLength(32)]
        [Column("scrivener_vendor_sid")]
        public string ScrivenerVendorSid { get; set; }

        [MaxLength(100)]
        [Column("escrow_bank_account")]
        public string EscrowBankAccount { get; set; }

        [Column("final_deal_price")]
        public decimal FinalDealPrice { get; set; }

        [Column("buyer_service_fee")]
        public decimal BuyerServiceFee { get; set; }

        [Column("seller_service_fee")]
        public decimal SellerServiceFee { get; set; }

        [Column("total_service_fee")]
        public decimal TotalServiceFee { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("listing_agent_user_sid")]
        public string ListingAgentUserSid { get; set; }

        [Column("listing_agent_split_pct")]
        public decimal ListingAgentSplitPct { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("selling_agent_user_sid")]
        public string SellingAgentUserSid { get; set; }

        [Column("selling_agent_split_pct")]
        public decimal SellingAgentSplitPct { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("contract_status")]
        public string ContractStatus { get; set; }

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
}