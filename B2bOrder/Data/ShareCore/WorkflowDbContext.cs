using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.ShareCore.Models;

namespace B2bOrder.Resources.ShareCore
{
    public class WorkflowDbContext : DbContext
    {
        public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options) { }

        #region DbSets
        public DbSet<WfDefinition> WfDefinitions { get; set; } = null!;
        public DbSet<WfDefinitionVersion> WfDefinitionVersions { get; set; } = null!;
        public DbSet<WfNode> WfNodes { get; set; } = null!;
        public DbSet<WfTransition> WfTransitions { get; set; } = null!;
        public DbSet<WfNodeApprover> WfNodeApprovers { get; set; } = null!;
        public DbSet<WfFormDefinition> WfFormDefinitions { get; set; } = null!;
        public DbSet<WfInstance> WfInstances { get; set; } = null!;
        public DbSet<WfInstanceNode> WfInstanceNodes { get; set; } = null!;
        public DbSet<WfTask> WfTasks { get; set; } = null!;
        public DbSet<WfActionLog> WfActionLogs { get; set; } = null!;
        public DbSet<WfDelegate> WfDelegates { get; set; } = null!;
        public DbSet<WfAdditionalApprover> WfAdditionalApprovers { get; set; } = null!;
        public DbSet<WfReminder> WfReminders { get; set; } = null!;
        public DbSet<WfEventLog> WfEventLogs { get; set; } = null!;
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region 01. 流程定義與版本 (Workflow Definition & Version)
            modelBuilder.Entity<WfDefinition>(entity =>
            {
                entity.ToTable("wf_definition");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.WorkflowCode).HasDatabaseName("uk_wfd_workflow_code").IsUnique();

                entity.HasIndex(e => e.WorkflowCategory).HasDatabaseName("idx_wfd_category");
                entity.HasIndex(e => e.BusinessType).HasDatabaseName("idx_wfd_business_type");
                entity.HasIndex(e => e.SourceServiceCode).HasDatabaseName("idx_wfd_source_service");
                entity.HasIndex(e => e.WorkflowStatus).HasDatabaseName("idx_wfd_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wfd_avalible");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.WorkflowCode).HasColumnName("workflow_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.WorkflowName).HasColumnName("workflow_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.WorkflowCategory).HasColumnName("workflow_category").HasMaxLength(50).IsRequired();
                entity.Property(e => e.BusinessType).HasColumnName("business_type").HasMaxLength(100).IsRequired();
                entity.Property(e => e.SourceServiceCode).HasColumnName("source_service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.CurrentVersion).HasColumnName("current_version").HasDefaultValue(1);
                entity.Property(e => e.WorkflowStatus).HasColumnName("workflow_status").HasMaxLength(20).HasDefaultValue("DRAFT");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });

            modelBuilder.Entity<WfDefinitionVersion>(entity =>
            {
                entity.ToTable("wf_definition_version");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.WorkflowDefinitionNid, e.VersionNo }).HasDatabaseName("uk_wfdv_definition_version").IsUnique();

                entity.HasIndex(e => e.WorkflowDefinitionNid).HasDatabaseName("idx_wfdv_definition_nid");
                entity.HasIndex(e => new { e.EffectiveStartDate, e.EffectiveEndDate }).HasDatabaseName("idx_wfdv_effective_date");
                entity.HasIndex(e => e.VersionStatus).HasDatabaseName("idx_wfdv_status");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.WorkflowDefinitionNid).HasColumnName("workflow_definition_nid").IsRequired();
                entity.Property(e => e.VersionNo).HasColumnName("version_no").IsRequired();
                entity.Property(e => e.VersionName).HasColumnName("version_name").HasMaxLength(100);
                entity.Property(e => e.EffectiveStartDate).HasColumnName("effective_start_date");
                entity.Property(e => e.EffectiveEndDate).HasColumnName("effective_end_date");
                entity.Property(e => e.DefinitionJson).HasColumnName("definition_json").HasColumnType("json");
                entity.Property(e => e.VersionStatus).HasColumnName("version_status").HasMaxLength(20).HasDefaultValue("DRAFT");
                entity.Property(e => e.PublishedUserSid).HasColumnName("published_user_sid").HasMaxLength(32);
                entity.Property(e => e.PublishedDate).HasColumnName("published_date");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.Versions)
                    .HasForeignKey(d => d.WorkflowDefinitionNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wfdv_definition");
            });
            #endregion

            #region 02. 流程節點與路由 (Workflow Node & Transition)
            modelBuilder.Entity<WfNode>(entity =>
            {
                entity.ToTable("wf_node");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.WorkflowVersionNid, e.NodeCode }).HasDatabaseName("uk_wfn_version_code").IsUnique();

                entity.HasIndex(e => e.WorkflowVersionNid).HasDatabaseName("idx_wfn_version_nid");
                entity.HasIndex(e => e.NodeType).HasDatabaseName("idx_wfn_node_type");
                entity.HasIndex(e => e.ApproverType).HasDatabaseName("idx_wfn_approver_type");
                entity.HasIndex(e => e.SortNo).HasDatabaseName("idx_wfn_sort_no");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wfn_avalible");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.WorkflowVersionNid).HasColumnName("workflow_version_nid").IsRequired();
                entity.Property(e => e.NodeCode).HasColumnName("node_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.NodeName).HasColumnName("node_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.NodeType).HasColumnName("node_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.ApprovalMode).HasColumnName("approval_mode").HasMaxLength(30);
                entity.Property(e => e.ApproverType).HasColumnName("approver_type").HasMaxLength(30);
                entity.Property(e => e.ApproverValue).HasColumnName("approver_value").HasMaxLength(500);
                entity.Property(e => e.TimeoutHours).HasColumnName("timeout_hours");
                entity.Property(e => e.TimeoutAction).HasColumnName("timeout_action").HasMaxLength(30);
                entity.Property(e => e.AllowReturn).HasColumnName("allow_return").HasColumnType("tinyint(1)").HasDefaultValue(true);
                entity.Property(e => e.AllowDelegate).HasColumnName("allow_delegate").HasColumnType("tinyint(1)").HasDefaultValue(true);
                entity.Property(e => e.AllowAddApprover).HasColumnName("allow_add_approver").HasColumnType("tinyint(1)").HasDefaultValue(false);
                entity.Property(e => e.SortNo).HasColumnName("sort_no").HasDefaultValue(0);
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasOne(d => d.WorkflowVersion)
                    .WithMany(p => p.Nodes)
                    .HasForeignKey(d => d.WorkflowVersionNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wfn_version");
            });

            modelBuilder.Entity<WfTransition>(entity =>
            {
                entity.ToTable("wf_transition");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.WorkflowVersionNid, e.TransitionCode }).HasDatabaseName("uk_wft_transition_code").IsUnique();

                entity.HasIndex(e => e.WorkflowVersionNid).HasDatabaseName("idx_wft_version_nid");
                entity.HasIndex(e => e.FromNodeNid).HasDatabaseName("idx_wft_from_node_nid");
                entity.HasIndex(e => e.ToNodeNid).HasDatabaseName("idx_wft_to_node_nid");
                entity.HasIndex(e => e.ActionCode).HasDatabaseName("idx_wft_action_code");
                entity.HasIndex(e => e.Priority).HasDatabaseName("idx_wft_priority");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wft_avalible");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.WorkflowVersionNid).HasColumnName("workflow_version_nid").IsRequired();
                entity.Property(e => e.FromNodeNid).HasColumnName("from_node_nid").IsRequired();
                entity.Property(e => e.ToNodeNid).HasColumnName("to_node_nid").IsRequired();
                entity.Property(e => e.TransitionCode).HasColumnName("transition_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.TransitionName).HasColumnName("transition_name").HasMaxLength(200);
                entity.Property(e => e.ActionCode).HasColumnName("action_code").HasMaxLength(50);
                entity.Property(e => e.ConditionExpression).HasColumnName("condition_expression");
                entity.Property(e => e.ConditionConfig).HasColumnName("condition_config").HasColumnType("json");
                entity.Property(e => e.Priority).HasColumnName("priority").HasDefaultValue(0);
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasOne(d => d.WorkflowVersion)
                    .WithMany(p => p.Transitions)
                    .HasForeignKey(d => d.WorkflowVersionNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wft_version");

                entity.HasOne(d => d.FromNode)
                    .WithMany(p => p.FromTransitions)
                    .HasForeignKey(d => d.FromNodeNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wft_from_node");

                entity.HasOne(d => d.ToNode)
                    .WithMany(p => p.ToTransitions)
                    .HasForeignKey(d => d.ToNodeNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wft_to_node");
            });

            modelBuilder.Entity<WfNodeApprover>(entity =>
            {
                entity.ToTable("wf_node_approver");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.HasIndex(e => e.NodeNid).HasDatabaseName("idx_wfna_node_nid");
                entity.HasIndex(e => new { e.ApproverType, e.ApproverSid }).HasDatabaseName("idx_wfna_approver");
                entity.HasIndex(e => e.SequenceNo).HasDatabaseName("idx_wfna_sequence_no");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wfna_avalible");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.NodeNid).HasColumnName("node_nid").IsRequired();
                entity.Property(e => e.ApproverType).HasColumnName("approver_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.ApproverSid).HasColumnName("approver_sid").HasMaxLength(32);
                entity.Property(e => e.ApproverExpression).HasColumnName("approver_expression").HasMaxLength(1000);
                entity.Property(e => e.SequenceNo).HasColumnName("sequence_no").HasDefaultValue(1);
                entity.Property(e => e.RequiredApproval).HasColumnName("required_approval").HasColumnType("tinyint(1)").HasDefaultValue(true);
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.NodeApprovers)
                    .HasForeignKey(d => d.NodeNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wfna_node");
            });
            #endregion

            #region 03. 表單與欄位定義 (Form Definition)
            modelBuilder.Entity<WfFormDefinition>(entity =>
            {
                entity.ToTable("wf_form_definition");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.WorkflowDefinitionNid, e.FormCode }).HasDatabaseName("uk_wffd_form_code").IsUnique();

                entity.HasIndex(e => e.WorkflowDefinitionNid).HasDatabaseName("idx_wffd_definition_nid");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wffd_avalible");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.WorkflowDefinitionNid).HasColumnName("workflow_definition_nid").IsRequired();
                entity.Property(e => e.FormCode).HasColumnName("form_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.FormName).HasColumnName("form_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.FormSchema).HasColumnName("form_schema").HasColumnType("json").IsRequired();
                entity.Property(e => e.ValidationSchema).HasColumnName("validation_schema").HasColumnType("json");
                entity.Property(e => e.DisplaySchema).HasColumnName("display_schema").HasColumnType("json");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.FormDefinitions)
                    .HasForeignKey(d => d.WorkflowDefinitionNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wffd_definition");
            });
            #endregion

            #region 04. 流程實例與實例節點 (Workflow Instance & Instance Node)
            modelBuilder.Entity<WfInstance>(entity =>
            {
                entity.ToTable("wf_instance");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.InstanceNo).HasDatabaseName("uk_wfi_instance_no").IsUnique();
                entity.HasIndex(e => new { e.BusinessType, e.BusinessSid }).HasDatabaseName("uk_wfi_business").IsUnique();

                entity.HasIndex(e => e.WorkflowDefinitionSid).HasDatabaseName("idx_wfi_workflow_definition_sid");
                entity.HasIndex(e => e.ApplicantUserSid).HasDatabaseName("idx_wfi_applicant_user_sid");
                entity.HasIndex(e => e.ApplicantDepartmentSid).HasDatabaseName("idx_wfi_department_sid");
                entity.HasIndex(e => e.CurrentNodeCode).HasDatabaseName("idx_wfi_current_node_code");
                entity.HasIndex(e => e.InstanceStatus).HasDatabaseName("idx_wfi_status");
                entity.HasIndex(e => e.StartDate).HasDatabaseName("idx_wfi_start_date");
                entity.HasIndex(e => e.DueDate).HasDatabaseName("idx_wfi_due_date");
                entity.HasIndex(e => e.CorrelationId).HasDatabaseName("idx_wfi_correlation_id");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wfi_avalible");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.InstanceNo).HasColumnName("instance_no").HasMaxLength(60).IsRequired();
                entity.Property(e => e.WorkflowDefinitionSid).HasColumnName("workflow_definition_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.WorkflowVersionNo).HasColumnName("workflow_version_no").IsRequired();
                entity.Property(e => e.BusinessType).HasColumnName("business_type").HasMaxLength(100).IsRequired();
                entity.Property(e => e.BusinessSid).HasColumnName("business_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.BusinessNo).HasColumnName("business_no").HasMaxLength(100);
                entity.Property(e => e.BusinessTitle).HasColumnName("business_title").HasMaxLength(500);
                entity.Property(e => e.ApplicantUserSid).HasColumnName("applicant_user_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ApplicantEmployeeSid).HasColumnName("applicant_employee_sid").HasMaxLength(32);
                entity.Property(e => e.ApplicantDepartmentSid).HasColumnName("applicant_department_sid").HasMaxLength(32);
                entity.Property(e => e.CurrentNodeCode).HasColumnName("current_node_code").HasMaxLength(100);
                entity.Property(e => e.InstanceStatus).HasColumnName("instance_status").HasMaxLength(30).HasDefaultValue("DRAFT");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.CompletedDate).HasColumnName("completed_date");
                entity.Property(e => e.DueDate).HasColumnName("due_date");
                entity.Property(e => e.FormData).HasColumnName("form_data").HasColumnType("json");
                entity.Property(e => e.SourceServiceCode).HasColumnName("source_service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.CorrelationId).HasColumnName("correlation_id").HasMaxLength(100);
                entity.Property(e => e.CreateUserSid).HasColumnName("create_user_sid").HasMaxLength(32);
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });

            modelBuilder.Entity<WfInstanceNode>(entity =>
            {
                entity.ToTable("wf_instance_node");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.WorkflowInstanceNid, e.SequenceNo }).HasDatabaseName("uk_wfin_instance_sequence").IsUnique();

                entity.HasIndex(e => e.WorkflowInstanceNid).HasDatabaseName("idx_wfin_instance_nid");
                entity.HasIndex(e => e.NodeCode).HasDatabaseName("idx_wfin_node_code");
                entity.HasIndex(e => e.NodeStatus).HasDatabaseName("idx_wfin_status");
                entity.HasIndex(e => e.DueDate).HasDatabaseName("idx_wfin_due_date");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.WorkflowInstanceNid).HasColumnName("workflow_instance_nid").IsRequired();
                entity.Property(e => e.NodeCode).HasColumnName("node_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.NodeName).HasColumnName("node_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.NodeType).HasColumnName("node_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.SequenceNo).HasColumnName("sequence_no").IsRequired();
                entity.Property(e => e.NodeStatus).HasColumnName("node_status").HasMaxLength(30).HasDefaultValue("PENDING");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.CompletedDate).HasColumnName("completed_date");
                entity.Property(e => e.DueDate).HasColumnName("due_date");
                entity.Property(e => e.ApprovalMode).HasColumnName("approval_mode").HasMaxLength(30);
                entity.Property(e => e.RequiredApprovals).HasColumnName("required_approvals").HasDefaultValue(1);
                entity.Property(e => e.ApprovedCount).HasColumnName("approved_count").HasDefaultValue(0);
                entity.Property(e => e.RejectedCount).HasColumnName("rejected_count").HasDefaultValue(0);
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasOne(d => d.WorkflowInstance)
                    .WithMany(p => p.InstanceNodes)
                    .HasForeignKey(d => d.WorkflowInstanceNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wfin_instance");
            });
            #endregion

            #region 05. 簽核任務與紀錄 (Workflow Task & Action Log)
            modelBuilder.Entity<WfTask>(entity =>
            {
                entity.ToTable("wf_task");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.TaskNo).HasDatabaseName("uk_wftask_task_no").IsUnique();

                entity.HasIndex(e => e.WorkflowInstanceNid).HasDatabaseName("idx_wftask_instance_nid");
                entity.HasIndex(e => e.InstanceNodeNid).HasDatabaseName("idx_wftask_instance_node_nid");
                entity.HasIndex(e => new { e.AssigneeType, e.AssigneeSid }).HasDatabaseName("idx_wftask_assignee");
                entity.HasIndex(e => e.ActualUserSid).HasDatabaseName("idx_wftask_actual_user_sid");
                entity.HasIndex(e => e.TaskStatus).HasDatabaseName("idx_wftask_status");
                entity.HasIndex(e => e.DueDate).HasDatabaseName("idx_wftask_due_date");
                entity.HasIndex(e => e.Priority).HasDatabaseName("idx_wftask_priority");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.WorkflowInstanceNid).HasColumnName("workflow_instance_nid").IsRequired();
                entity.Property(e => e.InstanceNodeNid).HasColumnName("instance_node_nid").IsRequired();
                entity.Property(e => e.TaskNo).HasColumnName("task_no").HasMaxLength(60).IsRequired();
                entity.Property(e => e.AssigneeType).HasColumnName("assignee_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.AssigneeSid).HasColumnName("assignee_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ActualUserSid).HasColumnName("actual_user_sid").HasMaxLength(32);
                entity.Property(e => e.DelegatedFromSid).HasColumnName("delegated_from_sid").HasMaxLength(32);
                entity.Property(e => e.TaskStatus).HasColumnName("task_status").HasMaxLength(30).HasDefaultValue("PENDING");
                entity.Property(e => e.AssignedDate).HasColumnName("assigned_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ClaimedDate).HasColumnName("claimed_date");
                entity.Property(e => e.CompletedDate).HasColumnName("completed_date");
                entity.Property(e => e.DueDate).HasColumnName("due_date");
                entity.Property(e => e.Priority).HasColumnName("priority").HasMaxLength(20).HasDefaultValue("NORMAL");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasOne(d => d.WorkflowInstance)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.WorkflowInstanceNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wftask_instance");

                entity.HasOne(d => d.InstanceNode)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.InstanceNodeNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wftask_instance_node");
            });

            modelBuilder.Entity<WfActionLog>(entity =>
            {
                entity.ToTable("wf_action_log");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.HasIndex(e => e.WorkflowInstanceNid).HasDatabaseName("idx_wfal_instance_nid");
                entity.HasIndex(e => e.InstanceNodeNid).HasDatabaseName("idx_wfal_instance_node_nid");
                entity.HasIndex(e => e.TaskSid).HasDatabaseName("idx_wfal_task_sid");
                entity.HasIndex(e => e.ActionCode).HasDatabaseName("idx_wfal_action_code");
                entity.HasIndex(e => e.ActionUserSid).HasDatabaseName("idx_wfal_action_user_sid");
                entity.HasIndex(e => e.CreateDate).HasDatabaseName("idx_wfal_create_date");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.WorkflowInstanceNid).HasColumnName("workflow_instance_nid").IsRequired();
                entity.Property(e => e.InstanceNodeNid).HasColumnName("instance_node_nid");
                entity.Property(e => e.TaskSid).HasColumnName("task_sid").HasMaxLength(32);
                entity.Property(e => e.ActionCode).HasColumnName("action_code").HasMaxLength(30).IsRequired();
                entity.Property(e => e.ActionUserSid).HasColumnName("action_user_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ActionEmployeeSid).HasColumnName("action_employee_sid").HasMaxLength(32);
                entity.Property(e => e.FromNodeCode).HasColumnName("from_node_code").HasMaxLength(100);
                entity.Property(e => e.ToNodeCode).HasColumnName("to_node_code").HasMaxLength(100);
                entity.Property(e => e.ActionComment).HasColumnName("action_comment");
                entity.Property(e => e.AttachmentJson).HasColumnName("attachment_json").HasColumnType("json");
                entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(1000);

                entity.HasOne(d => d.WorkflowInstance)
                    .WithMany(p => p.ActionLogs)
                    .HasForeignKey(d => d.WorkflowInstanceNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wfal_instance");

                entity.HasOne(d => d.InstanceNode)
                    .WithMany(p => p.ActionLogs)
                    .HasForeignKey(d => d.InstanceNodeNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wfal_instance_node");
            });
            #endregion

            #region 06. 代理與加簽 (Delegate & Additional Approver)
            modelBuilder.Entity<WfDelegate>(entity =>
            {
                entity.ToTable("wf_delegate");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.PrincipalUserSid, e.DelegateUserSid, e.WorkflowCode, e.StartDate }).HasDatabaseName("uk_wfd_delegate").IsUnique();

                entity.HasIndex(e => e.PrincipalUserSid).HasDatabaseName("idx_wfd_principal_user_sid");
                entity.HasIndex(e => e.DelegateUserSid).HasDatabaseName("idx_wfd_delegate_user_sid");
                entity.HasIndex(e => e.WorkflowCode).HasDatabaseName("idx_wfd_workflow_code");
                entity.HasIndex(e => new { e.StartDate, e.EndDate }).HasDatabaseName("idx_wfd_date");
                entity.HasIndex(e => e.DelegateStatus).HasDatabaseName("idx_wfd_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_wfd_avalible");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.PrincipalUserSid).HasColumnName("principal_user_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.DelegateUserSid).HasColumnName("delegate_user_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.WorkflowCode).HasColumnName("workflow_code").HasMaxLength(100);
                entity.Property(e => e.StartDate).HasColumnName("start_date").IsRequired();
                entity.Property(e => e.EndDate).HasColumnName("end_date").IsRequired();
                entity.Property(e => e.DelegateReason).HasColumnName("delegate_reason").HasMaxLength(500);
                entity.Property(e => e.DelegateStatus).HasColumnName("delegate_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");
            });

            modelBuilder.Entity<WfAdditionalApprover>(entity =>
            {
                entity.ToTable("wf_additional_approver");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.HasIndex(e => e.WorkflowInstanceNid).HasDatabaseName("idx_wfaa_instance_nid");
                entity.HasIndex(e => e.InstanceNodeNid).HasDatabaseName("idx_wfaa_instance_node_nid");
                entity.HasIndex(e => e.RequesterUserSid).HasDatabaseName("idx_wfaa_requester_user_sid");
                entity.HasIndex(e => e.ApproverUserSid).HasDatabaseName("idx_wfaa_approver_user_sid");
                entity.HasIndex(e => e.AddStatus).HasDatabaseName("idx_wfaa_status");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.WorkflowInstanceNid).HasColumnName("workflow_instance_nid").IsRequired();
                entity.Property(e => e.InstanceNodeNid).HasColumnName("instance_node_nid").IsRequired();
                entity.Property(e => e.RequesterUserSid).HasColumnName("requester_user_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ApproverUserSid).HasColumnName("approver_user_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.AddType).HasColumnName("add_type").HasMaxLength(20).HasDefaultValue("PARALLEL");
                entity.Property(e => e.AddReason).HasColumnName("add_reason").HasMaxLength(500);
                entity.Property(e => e.TaskSid).HasColumnName("task_sid").HasMaxLength(32);
                entity.Property(e => e.AddStatus).HasColumnName("add_status").HasMaxLength(20).HasDefaultValue("PENDING");
                entity.Property(e => e.CompletedDate).HasColumnName("completed_date");

                entity.HasOne(d => d.WorkflowInstance)
                    .WithMany(p => p.AdditionalApprovers)
                    .HasForeignKey(d => d.WorkflowInstanceNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wfaa_instance");

                entity.HasOne(d => d.InstanceNode)
                    .WithMany(p => p.AdditionalApprovers)
                    .HasForeignKey(d => d.InstanceNodeNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wfaa_instance_node");
            });
            #endregion

            #region 07. 催辦與通知 (Reminder)
            modelBuilder.Entity<WfReminder>(entity =>
            {
                entity.ToTable("wf_reminder");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.HasIndex(e => e.WorkflowInstanceNid).HasDatabaseName("idx_wfr_instance_nid");
                entity.HasIndex(e => e.TaskSid).HasDatabaseName("idx_wfr_task_sid");
                entity.HasIndex(e => e.ReceiverUserSid).HasDatabaseName("idx_wfr_receiver_user_sid");
                entity.HasIndex(e => e.SendStatus).HasDatabaseName("idx_wfr_send_status");
                entity.HasIndex(e => e.CreateDate).HasDatabaseName("idx_wfr_create_date");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.WorkflowInstanceNid).HasColumnName("workflow_instance_nid").IsRequired();
                entity.Property(e => e.TaskSid).HasColumnName("task_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ReminderType).HasColumnName("reminder_type").HasMaxLength(20).IsRequired();
                entity.Property(e => e.ReceiverUserSid).HasColumnName("receiver_user_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ReminderCount).HasColumnName("reminder_count").HasDefaultValue(1);
                entity.Property(e => e.ReminderMessage).HasColumnName("reminder_message");
                entity.Property(e => e.NotificationSid).HasColumnName("notification_sid").HasMaxLength(32);
                entity.Property(e => e.SendStatus).HasColumnName("send_status").HasMaxLength(20).HasDefaultValue("PENDING");
                entity.Property(e => e.SentDate).HasColumnName("sent_date");

                entity.HasOne(d => d.WorkflowInstance)
                    .WithMany(p => p.Reminders)
                    .HasForeignKey(d => d.WorkflowInstanceNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wfr_instance");
            });
            #endregion

            #region 08. 流程事件 (Event Log)
            modelBuilder.Entity<WfEventLog>(entity =>
            {
                entity.ToTable("wf_event_log");
                entity.HasKey(e => e.Nid);
                entity.HasIndex(e => e.Sid).IsUnique();

                entity.HasIndex(e => e.WorkflowInstanceNid).HasDatabaseName("idx_wfel_instance_nid");
                entity.HasIndex(e => e.EventCode).HasDatabaseName("idx_wfel_event_code");
                entity.HasIndex(e => e.ProcessStatus).HasDatabaseName("idx_wfel_process_status");
                entity.HasIndex(e => e.CreateDate).HasDatabaseName("idx_wfel_create_date");

                entity.Property(e => e.Nid).HasColumnName("nid").ValueGeneratedOnAdd();
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.WorkflowInstanceNid).HasColumnName("workflow_instance_nid").IsRequired();
                entity.Property(e => e.EventCode).HasColumnName("event_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.EventName).HasColumnName("event_name").HasMaxLength(200);
                entity.Property(e => e.EventData).HasColumnName("event_data").HasColumnType("json");
                entity.Property(e => e.SourceServiceCode).HasColumnName("source_service_code").HasMaxLength(80);
                entity.Property(e => e.OutboxEventSid).HasColumnName("outbox_event_sid").HasMaxLength(32);
                entity.Property(e => e.ProcessStatus).HasColumnName("process_status").HasMaxLength(20).HasDefaultValue("PENDING");
                entity.Property(e => e.ProcessedDate).HasColumnName("processed_date");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");

                entity.HasOne(d => d.WorkflowInstance)
                    .WithMany(p => p.EventLogs)
                    .HasForeignKey(d => d.WorkflowInstanceNid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wfel_instance");
            });
            #endregion
        }
    }
}