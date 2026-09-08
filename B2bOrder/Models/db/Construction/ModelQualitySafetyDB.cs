using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Construction.Models
{
    #region Quality Inspection Control Module - 品質查驗紀錄模組

    [Table("qsm_quality_inspection")]
    public class QsmQualityInspection
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

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string WbsSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("vendor_sid")]
        public string VendorSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("inspection_no")]
        public string InspectionNo { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("inspection_stage")]
        public string InspectionStage { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("location_description")]
        public string LocationDescription { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("is_hold_point")]
        public string IsHoldPoint { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("inspection_result")]
        public string InspectionResult { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("inspector_user_sid")]
        public string InspectorUserSid { get; set; }

        [MaxLength(32)]
        [Column("reviewed_by_user_sid")]
        public string ReviewedByUserSid { get; set; }

        [Column("reviewed_at")]
        public DateTime? ReviewedAt { get; set; }

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

    #region Safety Violation & Penalty Order Module - 安衛違規稽核與罰款處分單模組

    [Table("qsm_safety_penalty_order")]
    public class QsmSafetyPenaltyOrder
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
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("vendor_sid")]
        public string VendorSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("penalty_no")]
        public string PenaltyNo { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("violation_category")]
        public string ViolationCategory { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("violation_location")]
        public string ViolationLocation { get; set; }

        [Required]
        [Column("violation_description")]
        public string ViolationDescription { get; set; }

        [Column("demerit_points")]
        public int DemeritPoints { get; set; }

        [Column("penalty_amount")]
        public decimal PenaltyAmount { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("is_deducted")]
        public string IsDeducted { get; set; }

        [MaxLength(32)]
        [Column("deducted_payment_sid")]
        public string DeductedPaymentSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("safety_officer_user_sid")]
        public string SafetyOfficerUserSid { get; set; }

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

    #region Worksite Incident Log Module - 工地職業災害與事故通報模組

    [Table("qsm_worksite_incident_log")]
    public class QsmWorksiteIncidentLog
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
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [MaxLength(32)]
        [Column("vendor_sid")]
        public string VendorSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("incident_no")]
        public string IncidentNo { get; set; }

        [Column("incident_time")]
        public DateTime IncidentTime { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("severity_level")]
        public string SeverityLevel { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("incident_type")]
        public string IncidentType { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("location_description")]
        public string LocationDescription { get; set; }

        [Required]
        [Column("incident_summary")]
        public string IncidentSummary { get; set; }

        [Column("injured_count")]
        public int InjuredCount { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("is_reported_to_gov")]
        public string IsReportedToGov { get; set; }

        [Column("gov_report_at")]
        public DateTime? GovReportAt { get; set; }

        [Column("corrective_action")]
        public string CorrectiveAction { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("reporter_user_sid")]
        public string ReporterUserSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("incident_status")]
        public string IncidentStatus { get; set; }

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

    #region High-Risk Work Permit Module - 高危險作業許可證模組

    [Table("qsm_high_risk_work_permit")]
    public class QsmHighRiskWorkPermit
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
        [Column("project_sid")]
        public string ProjectSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("vendor_sid")]
        public string VendorSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("permit_no")]
        public string PermitNo { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("work_type")]
        public string WorkType { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("work_location")]
        public string WorkLocation { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("safety_measures_checked")]
        public string SafetyMeasuresChecked { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("supervisor_user_sid")]
        public string SupervisorUserSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("permit_status")]
        public string PermitStatus { get; set; }

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