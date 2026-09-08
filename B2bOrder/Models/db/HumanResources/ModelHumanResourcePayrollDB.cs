using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.HumanResource
{
    #region Employee / 員工主檔
    [Table("hr_employee")]
    public class Employee
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("employee_no")]
        public string EmployeeNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("party_sid")]
        public string PartySid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [MaxLength(32)]
        [Column("business_unit_sid")]
        public string? BusinessUnitSid { get; set; }

        [MaxLength(32)]
        [Column("department_sid")]
        public string? DepartmentSid { get; set; }

        [MaxLength(32)]
        [Column("position_sid")]
        public string? PositionSid { get; set; }

        [MaxLength(32)]
        [Column("manager_employee_sid")]
        public string? ManagerEmployeeSid { get; set; }

        [Column("hire_date")]
        public LocalDate HireDate { get; set; } // Can use DateOnly if .NET 6+

        [Column("probation_end_date")]
        public LocalDate? ProbationEndDate { get; set; }

        [Column("termination_date")]
        public LocalDate? TerminationDate { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("employment_type")]
        public string EmploymentType { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("payroll_type")]
        public string PayrollType { get; set; } = "MONTHLY";

        [Required]
        [MaxLength(32)]
        [Column("base_currency_sid")]
        public string BaseCurrencySid { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("employee_status")]
        public string EmployeeStatus { get; set; } = "ACTIVE";

        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }
    }
    #endregion

    #region Position / 職務與職等
    [Table("hr_position")]
    public class Position
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("position_code")]
        public string PositionCode { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("position_name")]
        public string PositionName { get; set; } = null!;

        [MaxLength(100)]
        [Column("job_family")]
        public string? JobFamily { get; set; }

        [MaxLength(50)]
        [Column("job_level")]
        public string? JobLevel { get; set; }

        [Column("management_mark")]
        public sbyte ManagementMark { get; set; }

        [MaxLength(32)]
        [Column("default_cost_center_sid")]
        public string? DefaultCostCenterSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("position_status")]
        public string PositionStatus { get; set; } = "ACTIVE";

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark")]
        public string? Remark { get; set; }
    }
    #endregion

    #region Employment Contract / 員工任用與薪資合約
    [Table("hr_employment_contract")]
    public class EmploymentContract
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("contract_no")]
        public string ContractNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("contract_type")]
        public string ContractType { get; set; } = null!;

        [Column("start_date")]
        public LocalDate StartDate { get; set; }

        [Column("end_date")]
        public LocalDate? EndDate { get; set; }

        [Column("monthly_salary", TypeName = "decimal(20,4)")]
        public decimal MonthlySalary { get; set; }

        [Column("hourly_rate", TypeName = "decimal(20,6)")]
        public decimal HourlyRate { get; set; }

        [Column("overtime_eligible")]
        public sbyte OvertimeEligible { get; set; }

        [Column("standard_hours_per_week", TypeName = "decimal(10,4)")]
        public decimal StandardHoursPerWeek { get; set; } = 40;

        [Column("probation_months")]
        public int ProbationMonths { get; set; }

        [MaxLength(32)]
        [Column("contract_file_sid")]
        public string? ContractFileSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("contract_status")]
        public string ContractStatus { get; set; } = "ACTIVE";

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }
    }
    #endregion

    #region Shift / 班別
    [Table("hr_shift")]
    public class Shift
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("shift_code")]
        public string ShiftCode { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("shift_name")]
        public string ShiftName { get; set; } = null!;

        [Column("start_time")]
        public TimeSpan StartTime { get; set; }

        [Column("end_time")]
        public TimeSpan EndTime { get; set; }

        [Column("break_minutes")]
        public int BreakMinutes { get; set; } = 60;

        [Column("standard_work_minutes")]
        public int StandardWorkMinutes { get; set; }

        [Column("cross_day_mark")]
        public sbyte CrossDayMark { get; set; }

        [Column("night_shift_mark")]
        public sbyte NightShiftMark { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("shift_status")]
        public string ShiftStatus { get; set; } = "ACTIVE";

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";
    }
    #endregion

    #region Schedule / 員工排班
    [Table("hr_schedule")]
    public class Schedule
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [Column("schedule_date")]
        public LocalDate ScheduleDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("shift_sid")]
        public string ShiftSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [MaxLength(32)]
        [Column("department_sid")]
        public string? DepartmentSid { get; set; }

        [MaxLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [MaxLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [Column("scheduled_start")]
        public DateTime ScheduledStart { get; set; }

        [Column("scheduled_end")]
        public DateTime ScheduledEnd { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("schedule_status")]
        public string ScheduleStatus { get; set; } = "PLANNED";
    }
    #endregion

    #region Attendance Record / 員工出勤與打卡紀錄
    [Table("hr_attendance_record")]
    public class AttendanceRecord
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [Column("attendance_date")]
        public LocalDate AttendanceDate { get; set; }

        [MaxLength(32)]
        [Column("schedule_sid")]
        public string? ScheduleSid { get; set; }

        [Column("clock_in_time")]
        public DateTime? ClockInTime { get; set; }

        [Column("clock_out_time")]
        public DateTime? ClockOutTime { get; set; }

        [Column("work_minutes")]
        public int WorkMinutes { get; set; }

        [Column("late_minutes")]
        public int LateMinutes { get; set; }

        [Column("early_leave_minutes")]
        public int EarlyLeaveMinutes { get; set; }

        [Column("absence_minutes")]
        public int AbsenceMinutes { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("source_type")]
        public string SourceType { get; set; } = "DEVICE";

        [MaxLength(32)]
        [Column("device_sid")]
        public string? DeviceSid { get; set; }

        [Column("latitude", TypeName = "decimal(10,7)")]
        public decimal? Latitude { get; set; }

        [Column("longitude", TypeName = "decimal(10,7)")]
        public decimal? Longitude { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("attendance_status")]
        public string AttendanceStatus { get; set; } = "NORMAL";
    }
    #endregion

    #region Attendance Adjustment / 補卡與出勤調整
    [Table("hr_attendance_adjustment")]
    public class AttendanceAdjustment
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("adjustment_no")]
        public string AdjustmentNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [MaxLength(32)]
        [Column("attendance_record_sid")]
        public string? AttendanceRecordSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("adjustment_type")]
        public string AdjustmentType { get; set; } = null!;

        [Column("original_data", TypeName = "json")]
        public string? OriginalData { get; set; }

        [Required]
        [Column("adjusted_data", TypeName = "json")]
        public string AdjustedData { get; set; } = null!;

        [Required]
        [Column("reason")]
        public string Reason { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("adjustment_status")]
        public string AdjustmentStatus { get; set; } = "DRAFT";

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }
    }
    #endregion

    #region Leave Type / 假別
    [Table("hr_leave_type")]
    public class LeaveType
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("leave_code")]
        public string LeaveCode { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("leave_name")]
        public string LeaveName { get; set; } = null!;

        [Column("paid_mark")]
        public sbyte PaidMark { get; set; } = 1;

        [Column("annual_quota_days", TypeName = "decimal(10,4)")]
        public decimal? AnnualQuotaDays { get; set; }

        [Column("carry_forward_allowed")]
        public sbyte CarryForwardAllowed { get; set; }

        [Column("carry_forward_limit_days", TypeName = "decimal(10,4)")]
        public decimal? CarryForwardLimitDays { get; set; }

        [Column("proof_required")]
        public sbyte ProofRequired { get; set; }

        [Column("minimum_unit_minutes")]
        public int MinimumUnitMinutes { get; set; } = 60;

        [Required]
        [MaxLength(20)]
        [Column("leave_status")]
        public string LeaveStatus { get; set; } = "ACTIVE";

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";
    }
    #endregion

    #region Leave Balance / 員工假別餘額
    [Table("hr_leave_balance")]
    public class LeaveBalance
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("leave_type_sid")]
        public string LeaveTypeSid { get; set; } = null!;

        [Column("leave_year")]
        public int LeaveYear { get; set; }

        [Column("granted_minutes")]
        public int GrantedMinutes { get; set; }

        [Column("carried_minutes")]
        public int CarriedMinutes { get; set; }

        [Column("used_minutes")]
        public int UsedMinutes { get; set; }

        [Column("pending_minutes")]
        public int PendingMinutes { get; set; }

        [Column("expired_minutes")]
        public int ExpiredMinutes { get; set; }

        [Column("remaining_minutes")]
        public int RemainingMinutes { get; set; }
    }
    #endregion

    #region Leave Request / 員工請假申請
    [Table("hr_leave_request")]
    public class LeaveRequest
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("request_no")]
        public string RequestNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("leave_type_sid")]
        public string LeaveTypeSid { get; set; } = null!;

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("leave_minutes")]
        public int LeaveMinutes { get; set; }

        [Column("reason")]
        public string? Reason { get; set; }

        [MaxLength(32)]
        [Column("proof_file_sid")]
        public string? ProofFileSid { get; set; }

        [MaxLength(32)]
        [Column("delegate_employee_sid")]
        public string? DelegateEmployeeSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("request_status")]
        public string RequestStatus { get; set; } = "DRAFT";

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }
    }
    #endregion

    #region Overtime Request / 加班申請
    [Table("hr_overtime_request")]
    public class OvertimeRequest
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("request_no")]
        public string RequestNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [Column("overtime_date")]
        public LocalDate OvertimeDate { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("requested_minutes")]
        public int RequestedMinutes { get; set; }

        [Column("approved_minutes")]
        public int ApprovedMinutes { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("overtime_type")]
        public string OvertimeType { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("compensation_type")]
        public string CompensationType { get; set; } = "PAY";

        [MaxLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [MaxLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [Required]
        [Column("reason")]
        public string Reason { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("request_status")]
        public string RequestStatus { get; set; } = "DRAFT";

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }
    }
    #endregion

    #region Timesheet / 員工工時表
    [Table("hr_timesheet")]
    public class Timesheet
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("timesheet_no")]
        public string TimesheetNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [Column("period_start_date")]
        public LocalDate PeriodStartDate { get; set; }

        [Column("period_end_date")]
        public LocalDate PeriodEndDate { get; set; }

        [Column("total_work_minutes")]
        public int TotalWorkMinutes { get; set; }

        [Column("billable_minutes")]
        public int BillableMinutes { get; set; }

        [Column("overtime_minutes")]
        public int OvertimeMinutes { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("timesheet_status")]
        public string TimesheetStatus { get; set; } = "DRAFT";

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }
    }
    #endregion

    #region Timesheet Item / 專案與工地工時明細
    [Table("hr_timesheet_item")]
    public class TimesheetItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("timesheet_nid")]
        public ulong TimesheetNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Column("work_date")]
        public LocalDate WorkDate { get; set; }

        [MaxLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [MaxLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [MaxLength(32)]
        [Column("cost_code_sid")]
        public string? CostCodeSid { get; set; }

        [MaxLength(100)]
        [Column("activity_code")]
        public string? ActivityCode { get; set; }

        [MaxLength(1000)]
        [Column("description")]
        public string? Description { get; set; }

        [Column("work_minutes")]
        public int WorkMinutes { get; set; }

        [Column("overtime_minutes")]
        public int OvertimeMinutes { get; set; }

        [Column("billable_mark")]
        public sbyte BillableMark { get; set; }

        [Column("labor_cost_amount", TypeName = "decimal(20,4)")]
        public decimal LaborCostAmount { get; set; }

        [Column("billing_amount", TypeName = "decimal(20,4)")]
        public decimal BillingAmount { get; set; }
    }
    #endregion

    #region Payroll Calendar / 薪資週期與發薪日
    [Table("hr_payroll_calendar")]
    public class PayrollCalendar
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("calendar_code")]
        public string CalendarCode { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("calendar_name")]
        public string CalendarName { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("pay_frequency")]
        public string PayFrequency { get; set; } = null!;

        [Column("cutoff_day")]
        public int? CutoffDay { get; set; }

        [Column("payment_day")]
        public int? PaymentDay { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("calendar_status")]
        public string CalendarStatus { get; set; } = "ACTIVE";

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";
    }
    #endregion

    #region Payroll Period / 薪資計算期間
    [Table("hr_payroll_period")]
    public class PayrollPeriod
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("payroll_calendar_sid")]
        public string PayrollCalendarSid { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("period_no")]
        public string PeriodNo { get; set; } = null!;

        [Column("period_start_date")]
        public LocalDate PeriodStartDate { get; set; }

        [Column("period_end_date")]
        public LocalDate PeriodEndDate { get; set; }

        [Column("cutoff_date")]
        public LocalDate CutoffDate { get; set; }

        [Column("payment_date")]
        public LocalDate PaymentDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("period_status")]
        public string PeriodStatus { get; set; } = "OPEN";
    }
    #endregion

    #region Pay Component / 薪資應發、應扣與雇主負擔項目
    [Table("hr_pay_component")]
    public class PayComponent
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("component_code")]
        public string ComponentCode { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("component_name")]
        public string ComponentName { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("component_type")]
        public string ComponentType { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("calculation_method")]
        public string CalculationMethod { get; set; } = null!;

        [Column("taxable_mark")]
        public sbyte TaxableMark { get; set; } = 1;

        [Column("insurable_mark")]
        public sbyte InsurableMark { get; set; } = 1;

        [Column("overtime_mark")]
        public sbyte OvertimeMark { get; set; }

        [Column("pension_mark")]
        public sbyte PensionMark { get; set; }

        [MaxLength(32)]
        [Column("gl_account_sid")]
        public string? GlAccountSid { get; set; }

        [MaxLength(32)]
        [Column("offset_account_sid")]
        public string? OffsetAccountSid { get; set; }

        [Column("formula_expression")]
        public string? FormulaExpression { get; set; }

        [Column("display_order")]
        public int DisplayOrder { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("component_status")]
        public string ComponentStatus { get; set; } = "ACTIVE";

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";
    }
    #endregion

    #region Employee Pay Component / 員工固定薪資、津貼與扣款設定
    [Table("hr_employee_pay_component")]
    public class EmployeePayComponent
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("pay_component_sid")]
        public string PayComponentSid { get; set; } = null!;

        [Column("effective_start_date")]
        public LocalDate EffectiveStartDate { get; set; }

        [Column("effective_end_date")]
        public LocalDate? EffectiveEndDate { get; set; }

        [Column("fixed_amount", TypeName = "decimal(20,4)")]
        public decimal? FixedAmount { get; set; }

        [Column("rate_value", TypeName = "decimal(12,6)")]
        public decimal? RateValue { get; set; }

        [Column("quantity_value", TypeName = "decimal(20,6)")]
        public decimal? QuantityValue { get; set; }

        [Column("override_formula")]
        public string? OverrideFormula { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("component_status")]
        public string ComponentStatus { get; set; } = "ACTIVE";
    }
    #endregion

    #region Payroll Run / 薪資計算批次
    [Table("hr_payroll_run")]
    public class PayrollRun
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("payroll_run_no")]
        public string PayrollRunNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("payroll_period_sid")]
        public string PayrollPeriodSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("run_type")]
        public string RunType { get; set; } = null!;

        [Column("employee_count")]
        public int EmployeeCount { get; set; }

        [Column("gross_amount", TypeName = "decimal(20,4)")]
        public decimal GrossAmount { get; set; }

        [Column("deduction_amount", TypeName = "decimal(20,4)")]
        public decimal DeductionAmount { get; set; }

        [Column("net_amount", TypeName = "decimal(20,4)")]
        public decimal NetAmount { get; set; }

        [Column("employer_cost_amount", TypeName = "decimal(20,4)")]
        public decimal EmployerCostAmount { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("run_status")]
        public string RunStatus { get; set; } = "DRAFT";

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }

        [MaxLength(32)]
        [Column("approved_user_sid")]
        public string? ApprovedUserSid { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        [MaxLength(32)]
        [Column("payment_batch_sid")]
        public string? PaymentBatchSid { get; set; }

        [MaxLength(32)]
        [Column("journal_request_sid")]
        public string? JournalRequestSid { get; set; }
    }
    #endregion

    #region Payslip / 員工薪資單
    [Table("hr_payslip")]
    public class Payslip
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("payslip_no")]
        public string PayslipNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("payroll_run_sid")]
        public string PayrollRunSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [Column("gross_amount", TypeName = "decimal(20,4)")]
        public decimal GrossAmount { get; set; }

        [Column("deduction_amount", TypeName = "decimal(20,4)")]
        public decimal DeductionAmount { get; set; }

        [Column("net_amount", TypeName = "decimal(20,4)")]
        public decimal NetAmount { get; set; }

        [Column("employer_cost_amount", TypeName = "decimal(20,4)")]
        public decimal EmployerCostAmount { get; set; }

        [Column("paid_date")]
        public LocalDate? PaidDate { get; set; }

        [MaxLength(32)]
        [Column("bank_account_sid")]
        public string? BankAccountSid { get; set; }

        [MaxLength(32)]
        [Column("payment_sid")]
        public string? PaymentSid { get; set; }

        [MaxLength(32)]
        [Column("payslip_file_sid")]
        public string? PayslipFileSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("payslip_status")]
        public string PayslipStatus { get; set; } = "CALCULATED";
    }
    #endregion

    #region Payslip Item / 薪資應發、應扣與人工成本明細
    [Table("hr_payslip_item")]
    public class PayslipItem
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("payslip_nid")]
        public ulong PayslipNid { get; set; }

        [Column("line_no")]
        public int LineNo { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("pay_component_sid")]
        public string PayComponentSid { get; set; } = null!;

        [Column("quantity", TypeName = "decimal(20,6)")]
        public decimal? Quantity { get; set; }

        [Column("rate", TypeName = "decimal(20,6)")]
        public decimal? Rate { get; set; }

        [Column("amount", TypeName = "decimal(20,4)")]
        public decimal Amount { get; set; }

        [Column("base_amount", TypeName = "decimal(20,4)")]
        public decimal? BaseAmount { get; set; }

        [MaxLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [MaxLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [MaxLength(32)]
        [Column("cost_center_sid")]
        public string? CostCenterSid { get; set; }

        [MaxLength(32)]
        [Column("journal_line_sid")]
        public string? JournalLineSid { get; set; }
    }
    #endregion

    #region Bonus Plan / 獎金方案
    [Table("hr_bonus_plan")]
    public class BonusPlan
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("plan_code")]
        public string PlanCode { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("plan_name")]
        public string PlanName { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("bonus_type")]
        public string BonusType { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("calculation_method")]
        public string CalculationMethod { get; set; } = null!;

        [Column("formula_expression")]
        public string? FormulaExpression { get; set; }

        [Column("taxable_mark")]
        public sbyte TaxableMark { get; set; } = 1;

        [Column("approval_required")]
        public sbyte ApprovalRequired { get; set; } = 1;

        [Required]
        [MaxLength(20)]
        [Column("plan_status")]
        public string PlanStatus { get; set; } = "ACTIVE";

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";
    }
    #endregion

    #region Bonus Award / 員工獎金核發
    [Table("hr_bonus_award")]
    public class BonusAward
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("award_no")]
        public string AwardNo { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("bonus_plan_sid")]
        public string BonusPlanSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [MaxLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [Column("performance_period_start")]
        public LocalDate? PerformancePeriodStart { get; set; }

        [Column("performance_period_end")]
        public LocalDate? PerformancePeriodEnd { get; set; }

        [Column("base_amount", TypeName = "decimal(20,4)")]
        public decimal BaseAmount { get; set; }

        [Column("award_amount", TypeName = "decimal(20,4)")]
        public decimal AwardAmount { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("currency_sid")]
        public string CurrencySid { get; set; } = null!;

        [MaxLength(32)]
        [Column("payroll_run_sid")]
        public string? PayrollRunSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("award_status")]
        public string AwardStatus { get; set; } = "DRAFT";

        [MaxLength(32)]
        [Column("workflow_instance_sid")]
        public string? WorkflowInstanceSid { get; set; }
    }
    #endregion

    #region Insurance Enrollment / 勞健保、就保、勞退與團保投保
    [Table("hr_insurance_enrollment")]
    public class InsuranceEnrollment
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("insurance_type")]
        public string InsuranceType { get; set; } = null!;

        [MaxLength(32)]
        [Column("provider_party_sid")]
        public string? ProviderPartySid { get; set; }

        [MaxLength(100)]
        [Column("enrollment_no")]
        public string? EnrollmentNo { get; set; }

        [Column("enrollment_date")]
        public LocalDate EnrollmentDate { get; set; }

        [Column("termination_date")]
        public LocalDate? TerminationDate { get; set; }

        [Column("insured_salary", TypeName = "decimal(20,4)")]
        public decimal InsuredSalary { get; set; }

        [Column("employee_rate", TypeName = "decimal(12,6)")]
        public decimal EmployeeRate { get; set; }

        [Column("employer_rate", TypeName = "decimal(12,6)")]
        public decimal EmployerRate { get; set; }

        [Column("government_rate", TypeName = "decimal(12,6)")]
        public decimal GovernmentRate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("enrollment_status")]
        public string EnrollmentStatus { get; set; } = "ACTIVE";
    }
    #endregion

    #region Tax Withholding / 薪資所得稅扣繳設定
    [Table("hr_tax_withholding")]
    public class TaxWithholding
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [Column("tax_year")]
        public int TaxYear { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("withholding_method")]
        public string WithholdingMethod { get; set; } = null!;

        [Column("withholding_rate", TypeName = "decimal(12,6)")]
        public decimal WithholdingRate { get; set; }

        [Column("fixed_amount", TypeName = "decimal(20,4)")]
        public decimal FixedAmount { get; set; }

        [Column("dependent_count")]
        public int DependentCount { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("resident_status")]
        public string ResidentStatus { get; set; } = "RESIDENT";

        [Column("effective_start_date")]
        public LocalDate EffectiveStartDate { get; set; }

        [Column("effective_end_date")]
        public LocalDate? EffectiveEndDate { get; set; }
    }
    #endregion

    #region Labor Cost Allocation / 專案、工地與WBS人工成本分攤
    [Table("hr_labor_cost_allocation")]
    public class LaborCostAllocation
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(32)]
        [Column("payroll_run_sid")]
        public string PayrollRunSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("employee_sid")]
        public string EmployeeSid { get; set; } = null!;

        [MaxLength(32)]
        [Column("project_sid")]
        public string? ProjectSid { get; set; }

        [MaxLength(32)]
        [Column("site_sid")]
        public string? SiteSid { get; set; }

        [MaxLength(32)]
        [Column("wbs_sid")]
        public string? WbsSid { get; set; }

        [MaxLength(32)]
        [Column("cost_code_sid")]
        public string? CostCodeSid { get; set; }

        [MaxLength(32)]
        [Column("cost_center_sid")]
        public string? CostCenterSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("allocation_basis")]
        public string AllocationBasis { get; set; } = null!;

        [Column("allocation_percent", TypeName = "decimal(8,4)")]
        public decimal AllocationPercent { get; set; }

        [Column("labor_cost_amount", TypeName = "decimal(20,4)")]
        public decimal LaborCostAmount { get; set; }

        [Column("employer_cost_amount", TypeName = "decimal(20,4)")]
        public decimal EmployerCostAmount { get; set; }

        [Column("total_cost_amount", TypeName = "decimal(20,4)")]
        public decimal TotalCostAmount { get; set; }

        [MaxLength(32)]
        [Column("project_actual_cost_sid")]
        public string? ProjectActualCostSid { get; set; }

        [MaxLength(32)]
        [Column("journal_line_sid")]
        public string? JournalLineSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("allocation_status")]
        public string AllocationStatus { get; set; } = "PENDING";
    }
    #endregion

    #region Status History / 人資與薪資狀態歷程
    [Table("hr_status_history")]
    public class StatusHistory
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(30)]
        [Column("entity_type")]
        public string EntityType { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("entity_sid")]
        public string EntitySid { get; set; } = null!;

        [MaxLength(30)]
        [Column("old_status")]
        public string? OldStatus { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("new_status")]
        public string NewStatus { get; set; } = null!;

        [MaxLength(100)]
        [Column("event_code")]
        public string? EventCode { get; set; }

        [MaxLength(32)]
        [Column("operator_user_sid")]
        public string? OperatorUserSid { get; set; }

        [Column("reason")]
        public string? Reason { get; set; }

        [MaxLength(100)]
        [Column("correlation_id")]
        public string? CorrelationId { get; set; }
    }
    #endregion

    #region Event / 人資與薪資領域事件
    [Table("hr_event")]
    public class HrEvent
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(30)]
        [Column("entity_type")]
        public string EntityType { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("entity_sid")]
        public string EntitySid { get; set; } = null!;

        [Required]
        [MaxLength(120)]
        [Column("event_code")]
        public string EventCode { get; set; } = null!;

        [Column("event_version")]
        public int EventVersion { get; set; } = 1;

        [Column("event_data", TypeName = "json")]
        public string? EventData { get; set; }

        [MaxLength(100)]
        [Column("source_event_id")]
        public string? SourceEventId { get; set; }

        [MaxLength(100)]
        [Column("correlation_id")]
        public string? CorrelationId { get; set; }

        [MaxLength(100)]
        [Column("causation_id")]
        public string? CausationId { get; set; }

        [MaxLength(32)]
        [Column("outbox_event_sid")]
        public string? OutboxEventSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("process_status")]
        public string ProcessStatus { get; set; } = "PENDING";

        [Column("processed_date")]
        public DateTime? ProcessedDate { get; set; }

        [Column("error_message")]
        public string? ErrorMessage { get; set; }
    }
    #endregion
}