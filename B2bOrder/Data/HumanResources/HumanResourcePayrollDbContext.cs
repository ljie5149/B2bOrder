using Microsoft.EntityFrameworkCore;
using B2bOrder.Models.HumanResource;

namespace B2bOrder.Data
{
    #region Human Resource Payroll DB Context / 人資與薪資資料庫上下文
    public class HumanResourcePayrollDbContext : DbContext
    {
        public HumanResourcePayrollDbContext(DbContextOptions<HumanResourcePayrollDbContext> options)
            : base(options)
        {
        }

        // DbSets / 資料集
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Position> Positions { get; set; } = null!;
        public DbSet<EmploymentContract> EmploymentContracts { get; set; } = null!;
        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<Schedule> Schedules { get; set; } = null!;
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; } = null!;
        public DbSet<AttendanceAdjustment> AttendanceAdjustments { get; set; } = null!;
        public DbSet<LeaveType> LeaveTypes { get; set; } = null!;
        public DbSet<LeaveBalance> LeaveBalances { get; set; } = null!;
        public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;
        public DbSet<OvertimeRequest> OvertimeRequests { get; set; } = null!;
        public DbSet<Timesheet> Timesheets { get; set; } = null!;
        public DbSet<TimesheetItem> TimesheetItems { get; set; } = null!;
        public DbSet<PayrollCalendar> PayrollCalendars { get; set; } = null!;
        public DbSet<PayrollPeriod> PayrollPeriods { get; set; } = null!;
        public DbSet<PayComponent> PayComponents { get; set; } = null!;
        public DbSet<EmployeePayComponent> EmployeePayComponents { get; set; } = null!;
        public DbSet<PayrollRun> PayrollRuns { get; set; } = null!;
        public DbSet<Payslip> Payslips { get; set; } = null!;
        public DbSet<PayslipItem> PayslipItems { get; set; } = null!;
        public DbSet<BonusPlan> BonusPlans { get; set; } = null!;
        public DbSet<BonusAward> BonusAwards { get; set; } = null!;
        public DbSet<InsuranceEnrollment> InsuranceEnrollments { get; set; } = null!;
        public DbSet<TaxWithholding> TaxWithholdings { get; set; } = null!;
        public DbSet<LaborCostAllocation> LaborCostAllocations { get; set; } = null!;
        public DbSet<StatusHistory> StatusHistories { get; set; } = null!;
        public DbSet<HrEvent> HrEvents { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Constraints and Indexes / 唯一限制與索引設定
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeNo)
                .IsUnique();

            modelBuilder.Entity<Position>()
                .HasIndex(p => p.PositionCode)
                .IsUnique();

            modelBuilder.Entity<EmploymentContract>()
                .HasIndex(c => c.ContractNo)
                .IsUnique();

            modelBuilder.Entity<Shift>()
                .HasIndex(s => s.ShiftCode)
                .IsUnique();

            modelBuilder.Entity<Schedule>()
                .HasIndex(s => new { s.EmployeeSid, s.ScheduleDate })
                .IsUnique();

            modelBuilder.Entity<AttendanceAdjustment>()
                .HasIndex(a => a.AdjustmentNo)
                .IsUnique();

            modelBuilder.Entity<LeaveType>()
                .HasIndex(l => l.LeaveCode)
                .IsUnique();

            modelBuilder.Entity<LeaveBalance>()
                .HasIndex(b => new { b.EmployeeSid, b.LeaveTypeSid, b.LeaveYear })
                .IsUnique();

            modelBuilder.Entity<LeaveRequest>()
                .HasIndex(r => r.RequestNo)
                .IsUnique();

            modelBuilder.Entity<Timesheet>()
                .HasIndex(t => t.TimesheetNo)
                .IsUnique();

            modelBuilder.Entity<Timesheet>()
                .HasIndex(t => new { t.EmployeeSid, t.PeriodStartDate, t.PeriodEndDate })
                .IsUnique();

            modelBuilder.Entity<TimesheetItem>()
                .HasIndex(ti => new { ti.TimesheetNid, ti.LineNo })
                .IsUnique();

            modelBuilder.Entity<PayrollCalendar>()
                .HasIndex(pc => pc.CalendarCode)
                .IsUnique();

            modelBuilder.Entity<PayrollPeriod>()
                .HasIndex(pp => new { pp.PayrollCalendarSid, pp.PeriodNo })
                .IsUnique();

            modelBuilder.Entity<PayComponent>()
                .HasIndex(p => p.ComponentCode)
                .IsUnique();

            modelBuilder.Entity<EmployeePayComponent>()
                .HasIndex(ep => new { ep.EmployeeSid, ep.PayComponentSid, ep.EffectiveStartDate })
                .IsUnique();

            modelBuilder.Entity<PayrollRun>()
                .HasIndex(pr => pr.PayrollRunNo)
                .IsUnique();

            modelBuilder.Entity<Payslip>()
                .HasIndex(ps => ps.PayslipNo)
                .IsUnique();

            modelBuilder.Entity<Payslip>()
                .HasIndex(ps => new { ps.PayrollRunSid, ps.EmployeeSid })
                .IsUnique();

            modelBuilder.Entity<PayslipItem>()
                .HasIndex(psi => new { psi.PayslipNid, psi.LineNo })
                .IsUnique();

            modelBuilder.Entity<BonusPlan>()
                .HasIndex(bp => bp.PlanCode)
                .IsUnique();

            modelBuilder.Entity<BonusAward>()
                .HasIndex(ba => ba.AwardNo)
                .IsUnique();

            modelBuilder.Entity<HrEvent>()
                .HasIndex(he => he.SourceEventId)
                .IsUnique();
            #endregion

            #region Relationships / 關聯對應設定
            modelBuilder.Entity<TimesheetItem>()
                .HasOne<Timesheet>()
                .WithMany()
                .HasForeignKey(ti => ti.TimesheetNid)
                .HasPrincipalKey(t => t.Nid);

            modelBuilder.Entity<PayslipItem>()
                .HasOne<Payslip>()
                .WithMany()
                .HasForeignKey(psi => psi.PayslipNid)
                .HasPrincipalKey(p => p.Nid);
            #endregion
        }
    }
    #endregion
}