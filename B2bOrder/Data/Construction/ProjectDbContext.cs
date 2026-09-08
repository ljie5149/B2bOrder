using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Construction.Models
{
    #region Project Master Module - 建案與工程案場主檔模組

    [Table("prj_project_master")]
    public class PrjProjectMaster
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
        [MaxLength(50)]
        [Column("project_code")]
        public string ProjectCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("project_name")]
        public string ProjectName { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("project_type")]
        public string ProjectType { get; set; }

        [Column("land_area_sqm")]
        public decimal LandAreaSqm { get; set; }

        [Column("total_floor_area_sqm")]
        public decimal TotalFloorAreaSqm { get; set; }

        [MaxLength(100)]
        [Column("building_license_no")]
        public string BuildingLicenseNo { get; set; }

        [MaxLength(100)]
        [Column("usage_license_no")]
        public string UsageLicenseNo { get; set; }

        [MaxLength(300)]
        [Column("site_address")]
        public string SiteAddress { get; set; }

        [MaxLength(32)]
        [Column("project_manager_sid")]
        public string ProjectManagerSid { get; set; }

        [Column("planned_start_date", TypeName = "date")]
        public DateTime? PlannedStartDate { get; set; }

        [Column("planned_end_date", TypeName = "date")]
        public DateTime? PlannedEndDate { get; set; }

        [Column("actual_start_date", TypeName = "date")]
        public DateTime? ActualStartDate { get; set; }

        [Column("actual_end_date", TypeName = "date")]
        public DateTime? ActualEndDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("project_status")]
        public string ProjectStatus { get; set; }

        [Column("total_budget_amount")]
        public decimal TotalBudgetAmount { get; set; }

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

    #region Work Breakdown Structure Module - 工作分解結構 WBS 模組

    [Table("prj_wbs_task")]
    public class PrjWbsTask
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
        [Column("parent_wbs_sid")]
        public string ParentWbsSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("wbs_code")]
        public string WbsCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("task_name")]
        public string TaskName { get; set; }

        [Column("wbs_level")]
        public int WbsLevel { get; set; }

        [Column("sequence_no")]
        public int SequenceNo { get; set; }

        [MaxLength(32)]
        [Column("contractor_vendor_sid")]
        public string ContractorVendorSid { get; set; }

        [Column("budget_amount")]
        public decimal BudgetAmount { get; set; }

        [Column("planned_start_date", TypeName = "date")]
        public DateTime? PlannedStartDate { get; set; }

        [Column("planned_end_date", TypeName = "date")]
        public DateTime? PlannedEndDate { get; set; }

        [Column("actual_start_date", TypeName = "date")]
        public DateTime? ActualStartDate { get; set; }

        [Column("actual_end_date", TypeName = "date")]
        public DateTime? ActualEndDate { get; set; }

        [Column("completion_rate")]
        public decimal CompletionRate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("task_status")]
        public string TaskStatus { get; set; }

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

    #region Variation Order Module - 工程變更與追加減單模組

    [Table("prj_variation_order")]
    public class PrjVariationOrder
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
        [Column("wbs_sid")]
        public string WbsSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("vo_number")]
        public string VoNumber { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("vo_title")]
        public string VoTitle { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("contractor_vendor_sid")]
        public string ContractorVendorSid { get; set; }

        [Column("original_amount")]
        public decimal OriginalAmount { get; set; }

        [Column("change_amount")]
        public decimal ChangeAmount { get; set; }

        [Column("revised_amount")]
        public decimal RevisedAmount { get; set; }

        [Column("extended_days")]
        public int ExtendedDays { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("vo_status")]
        public string VoStatus { get; set; }

        [MaxLength(32)]
        [Column("approved_by_user_sid")]
        public string ApprovedByUserSid { get; set; }

        [Column("approved_at")]
        public DateTime? ApprovedAt { get; set; }

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

    #region Progress Payment Certificate Module - 工程進度估驗與請款模組

    [Table("prj_progress_payment")]
    public class PrjProgressPayment
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
        [Column("contractor_vendor_sid")]
        public string ContractorVendorSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("certificate_no")]
        public string CertificateNo { get; set; }

        [Column("period_number")]
        public int PeriodNumber { get; set; }

        [Column("valuation_date", TypeName = "date")]
        public DateTime ValuationDate { get; set; }

        [Column("current_claimed_amount")]
        public decimal CurrentClaimedAmount { get; set; }

        [Column("current_approved_amount")]
        public decimal CurrentApprovedAmount { get; set; }

        [Column("retention_deduction")]
        public decimal RetentionDeduction { get; set; }

        [Column("advance_deduction")]
        public decimal AdvanceDeduction { get; set; }

        [Column("net_payable_amount")]
        public decimal NetPayableAmount { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("payment_status")]
        public string PaymentStatus { get; set; }

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

    #region Daily Site Log & Safety Module - 工地施工日誌與安衛巡檢模組

    [Table("prj_daily_site_log")]
    public class PrjDailySiteLog
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

        [Column("log_date", TypeName = "date")]
        public DateTime LogDate { get; set; }

        [MaxLength(30)]
        [Column("weather_am")]
        public string WeatherAm { get; set; }

        [MaxLength(30)]
        [Column("weather_pm")]
        public string WeatherPm { get; set; }

        [Column("temperature_celsius")]
        public decimal? TemperatureCelsius { get; set; }

        [Column("total_workers_count")]
        public int TotalWorkersCount { get; set; }

        [Column("machinery_count")]
        public int MachineryCount { get; set; }

        [Required]
        [Column("work_summary")]
        public string WorkSummary { get; set; }

        [Column("safety_issue_summary")]
        public string SafetyIssueSummary { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("site_engineer_user_sid")]
        public string SiteEngineerUserSid { get; set; }

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

    [Table("prj_safety_punch_list")]
    public class PrjSafetyPunchList
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
        [Column("daily_log_sid")]
        public string DailyLogSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("contractor_vendor_sid")]
        public string ContractorVendorSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("issue_type")]
        public string IssueType { get; set; }

        [Required]
        [Column("issue_description")]
        public string IssueDescription { get; set; }

        [MaxLength(200)]
        [Column("location_description")]
        public string LocationDescription { get; set; }

        [Column("deadline_date", TypeName = "date")]
        public DateTime? DeadlineDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("rectification_status")]
        public string RectificationStatus { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("inspector_user_sid")]
        public string InspectorUserSid { get; set; }

        [Column("closed_at")]
        public DateTime? ClosedAt { get; set; }

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