using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Construction.Models
{
    #region Daily Site Log Module - 工地施工日誌主檔與明細模組

    [Table("pex_daily_site_log")]
    public class PexDailySiteLog
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

        [Column("total_machinery_count")]
        public int TotalMachineryCount { get; set; }

        [Required]
        [Column("work_summary")]
        public string WorkSummary { get; set; }

        [Column("safety_health_summary")]
        public string SafetyHealthSummary { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("site_manager_user_sid")]
        public string SiteManagerUserSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("log_status")]
        public string LogStatus { get; set; }

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

    [Table("pex_daily_labor_entry")]
    public class PexDailyLaborEntry
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
        [Column("daily_log_sid")]
        public string DailyLogSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("vendor_sid")]
        public string VendorSid { get; set; }

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string WbsSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("trade_type")]
        public string TradeType { get; set; }

        [Column("worker_count")]
        public int WorkerCount { get; set; }

        [MaxLength(200)]
        [Column("work_location")]
        public string WorkLocation { get; set; }

        [Column("work_description")]
        public string WorkDescription { get; set; }
    }

    #endregion

    #region Punch List Module - 施工與安衛缺失改善單模組

    [Table("pex_defect_punch_list")]
    public class PexDefectPunchList
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

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string WbsSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("defect_no")]
        public string DefectNo { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("defect_category")]
        public string DefectCategory { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("severity_level")]
        public string SeverityLevel { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("location_description")]
        public string LocationDescription { get; set; }

        [Required]
        [Column("defect_description")]
        public string DefectDescription { get; set; }

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

        [MaxLength(32)]
        [Column("closed_by_user_sid")]
        public string ClosedByUserSid { get; set; }

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

    #region Material & Machinery Inspection Module - 材料與機具進場抽驗紀錄模組

    [Table("pex_material_machinery_inspection")]
    public class PexMaterialMachineryInspection
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
        [MaxLength(20)]
        [Column("entry_type")]
        public string EntryType { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("item_name")]
        public string ItemName { get; set; }

        [MaxLength(200)]
        [Column("specification")]
        public string Specification { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [MaxLength(20)]
        [Column("unit")]
        public string Unit { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("inspection_result")]
        public string InspectionResult { get; set; }

        [MaxLength(100)]
        [Column("test_report_number")]
        public string TestReportNumber { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("inspector_user_sid")]
        public string InspectorUserSid { get; set; }

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

    #region Request for Information Module - 施工疑義澄清單 RFI 模組

    [Table("pex_request_for_information")]
    public class PexRequestForInformation
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
        [Column("rfi_number")]
        public string RfiNumber { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("rfi_subject")]
        public string RfiSubject { get; set; }

        [MaxLength(100)]
        [Column("drawing_reference_no")]
        public string DrawingReferenceNo { get; set; }

        [Required]
        [Column("question_description")]
        public string QuestionDescription { get; set; }

        [Column("proposed_solution")]
        public string ProposedSolution { get; set; }

        [Column("response_description")]
        public string ResponseDescription { get; set; }

        [MaxLength(100)]
        [Column("responded_by_name")]
        public string RespondedByName { get; set; }

        [Column("responded_at")]
        public DateTime? RespondedAt { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("is_cost_impact")]
        public string IsCostImpact { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("is_schedule_impact")]
        public string IsScheduleImpact { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("rfi_status")]
        public string RfiStatus { get; set; }

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