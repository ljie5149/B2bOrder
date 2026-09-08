using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Construction.Models
{
    #region Equipment Asset Master Module - 施工機具資產主檔模組

    [Table("eqp_equipment_master")]
    public class EqpEquipmentMaster
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

        [MaxLength(32)]
        [Column("vendor_sid")]
        public string VendorSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("equipment_code")]
        public string EquipmentCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("equipment_name")]
        public string EquipmentName { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("category")]
        public string Category { get; set; }

        [MaxLength(100)]
        [Column("brand")]
        public string Brand { get; set; }

        [MaxLength(100)]
        [Column("model_number")]
        public string ModelNumber { get; set; }

        [MaxLength(100)]
        [Column("serial_number")]
        public string SerialNumber { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("ownership_type")]
        public string OwnershipType { get; set; }

        [Column("daily_rental_rate")]
        public decimal DailyRentalRate { get; set; }

        [Column("monthly_rental_rate")]
        public decimal MonthlyRentalRate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("current_status")]
        public string CurrentStatus { get; set; }

        [MaxLength(32)]
        [Column("current_project_sid")]
        public string CurrentProjectSid { get; set; }

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

    #region Equipment Dispatch Log Module - 機具派遣與進退場紀錄模組

    [Table("eqp_dispatch_log")]
    public class EqpDispatchLog
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
        [Column("equipment_sid")]
        public string EquipmentSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("dispatch_no")]
        public string DispatchNo { get; set; }

        [Column("scheduled_mobilization_date", TypeName = "date")]
        public DateTime ScheduledMobilizationDate { get; set; }

        [Column("actual_mobilization_date")]
        public DateTime? ActualMobilizationDate { get; set; }

        [Column("scheduled_demobilization_date", TypeName = "date")]
        public DateTime? ScheduledDemobilizationDate { get; set; }

        [Column("actual_demobilization_date")]
        public DateTime? ActualDemobilizationDate { get; set; }

        [MaxLength(100)]
        [Column("operator_name")]
        public string OperatorName { get; set; }

        [MaxLength(50)]
        [Column("operator_phone")]
        public string OperatorPhone { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("dispatch_status")]
        public string DispatchStatus { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("requested_by_user_sid")]
        public string RequestedByUserSid { get; set; }

        [MaxLength(32)]
        [Column("approved_by_user_sid")]
        public string ApprovedByUserSid { get; set; }

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

    #region Maintenance & Repair Log Module - 機具保養與維修履歷模組

    [Table("eqp_maintenance_log")]
    public class EqpMaintenanceLog
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
        [Column("equipment_sid")]
        public string EquipmentSid { get; set; }

        [MaxLength(32)]
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("maintenance_no")]
        public string MaintenanceNo { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("maintenance_type")]
        public string MaintenanceType { get; set; }

        [Column("failure_description")]
        public string FailureDescription { get; set; }

        [MaxLength(32)]
        [Column("repair_vendor_sid")]
        public string RepairVendorSid { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("completion_time")]
        public DateTime? CompletionTime { get; set; }

        [Column("parts_cost")]
        public decimal PartsCost { get; set; }

        [Column("labor_cost")]
        public decimal LaborCost { get; set; }

        [Column("total_cost")]
        public decimal TotalCost { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("maintenance_status")]
        public string MaintenanceStatus { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("serviced_by_user_sid")]
        public string ServicedByUserSid { get; set; }

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

    #region Safety Certifications Module - 法定安全檢驗與特種證照模組

    [Table("eqp_safety_certification")]
    public class EqpSafetyCertification
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
        [Column("equipment_sid")]
        public string EquipmentSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("cert_type")]
        public string CertType { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("gov_license_number")]
        public string GovLicenseNumber { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("inspection_agency")]
        public string InspectionAgency { get; set; }

        [Column("issue_date", TypeName = "date")]
        public DateTime IssueDate { get; set; }

        [Column("expiration_date", TypeName = "date")]
        public DateTime ExpirationDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("cert_status")]
        public string CertStatus { get; set; }

        [MaxLength(32)]
        [Column("attachment_document_sid")]
        public string AttachmentDocumentSid { get; set; }

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