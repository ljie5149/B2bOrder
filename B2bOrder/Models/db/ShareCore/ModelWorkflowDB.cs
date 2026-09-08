using System;
using System.Collections.Generic;

namespace B2bOrder.Resources.ShareCore.Models
{
    #region 01. 流程定義與版本 (Workflow Definition & Version)
    public class WfDefinition
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string WorkflowCode { get; set; } = null!;
        public string WorkflowName { get; set; } = null!;
        public string WorkflowCategory { get; set; } = null!;
        public string BusinessType { get; set; } = null!;
        public string SourceServiceCode { get; set; } = null!;
        public string? Description { get; set; }
        public int CurrentVersion { get; set; } = 1;
        public string WorkflowStatus { get; set; } = "DRAFT";
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual ICollection<WfDefinitionVersion> Versions { get; set; } = new List<WfDefinitionVersion>();
        public virtual ICollection<WfFormDefinition> FormDefinitions { get; set; } = new List<WfFormDefinition>();
    }

    public class WfDefinitionVersion
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong WorkflowDefinitionNid { get; set; }
        public int VersionNo { get; set; }
        public string? VersionName { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public string? DefinitionJson { get; set; }
        public string VersionStatus { get; set; } = "DRAFT";
        public string? PublishedUserSid { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string? Remark { get; set; }

        public virtual WfDefinition WorkflowDefinition { get; set; } = null!;
        public virtual ICollection<WfNode> Nodes { get; set; } = new List<WfNode>();
        public virtual ICollection<WfTransition> Transitions { get; set; } = new List<WfTransition>();
    }
    #endregion

    #region 02. 流程節點與路由 (Workflow Node & Transition)
    public class WfNode
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong WorkflowVersionNid { get; set; }
        public string NodeCode { get; set; } = null!;
        public string NodeName { get; set; } = null!;
        public string NodeType { get; set; } = null!;
        public string? ApprovalMode { get; set; }
        public string? ApproverType { get; set; }
        public string? ApproverValue { get; set; }
        public int? TimeoutHours { get; set; }
        public string? TimeoutAction { get; set; }
        public bool AllowReturn { get; set; } = true;
        public bool AllowDelegate { get; set; } = true;
        public bool AllowAddApprover { get; set; } = false;
        public int SortNo { get; set; } = 0;
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual WfDefinitionVersion WorkflowVersion { get; set; } = null!;
        public virtual ICollection<WfNodeApprover> NodeApprovers { get; set; } = new List<WfNodeApprover>();
        public virtual ICollection<WfTransition> FromTransitions { get; set; } = new List<WfTransition>();
        public virtual ICollection<WfTransition> ToTransitions { get; set; } = new List<WfTransition>();
    }

    public class WfTransition
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong WorkflowVersionNid { get; set; }
        public ulong FromNodeNid { get; set; }
        public ulong ToNodeNid { get; set; }
        public string TransitionCode { get; set; } = null!;
        public string? TransitionName { get; set; }
        public string? ActionCode { get; set; }
        public string? ConditionExpression { get; set; }
        public string? ConditionConfig { get; set; }
        public int Priority { get; set; } = 0;
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual WfDefinitionVersion WorkflowVersion { get; set; } = null!;
        public virtual WfNode FromNode { get; set; } = null!;
        public virtual WfNode ToNode { get; set; } = null!;
    }

    public class WfNodeApprover
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong NodeNid { get; set; }
        public string ApproverType { get; set; } = null!;
        public string? ApproverSid { get; set; }
        public string? ApproverExpression { get; set; }
        public int SequenceNo { get; set; } = 1;
        public bool RequiredApproval { get; set; } = true;
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual WfNode Node { get; set; } = null!;
    }
    #endregion

    #region 03. 表單與欄位定義 (Form Definition)
    public class WfFormDefinition
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong WorkflowDefinitionNid { get; set; }
        public string FormCode { get; set; } = null!;
        public string FormName { get; set; } = null!;
        public string FormSchema { get; set; } = null!;
        public string? ValidationSchema { get; set; }
        public string? DisplaySchema { get; set; }
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual WfDefinition WorkflowDefinition { get; set; } = null!;
    }
    #endregion

    #region 04. 流程實例與實例節點 (Workflow Instance & Instance Node)
    public class WfInstance
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string InstanceNo { get; set; } = null!;
        public string WorkflowDefinitionSid { get; set; } = null!;
        public int WorkflowVersionNo { get; set; }
        public string BusinessType { get; set; } = null!;
        public string BusinessSid { get; set; } = null!;
        public string? BusinessNo { get; set; }
        public string? BusinessTitle { get; set; }
        public string ApplicantUserSid { get; set; } = null!;
        public string? ApplicantEmployeeSid { get; set; }
        public string? ApplicantDepartmentSid { get; set; }
        public string? CurrentNodeCode { get; set; }
        public string InstanceStatus { get; set; } = "DRAFT";
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? FormData { get; set; }
        public string SourceServiceCode { get; set; } = null!;
        public string? CorrelationId { get; set; }
        public string? CreateUserSid { get; set; }
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual ICollection<WfInstanceNode> InstanceNodes { get; set; } = new List<WfInstanceNode>();
        public virtual ICollection<WfTask> Tasks { get; set; } = new List<WfTask>();
        public virtual ICollection<WfActionLog> ActionLogs { get; set; } = new List<WfActionLog>();
        public virtual ICollection<WfAdditionalApprover> AdditionalApprovers { get; set; } = new List<WfAdditionalApprover>();
        public virtual ICollection<WfReminder> Reminders { get; set; } = new List<WfReminder>();
        public virtual ICollection<WfEventLog> EventLogs { get; set; } = new List<WfEventLog>();
    }

    public class WfInstanceNode
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong WorkflowInstanceNid { get; set; }
        public string NodeCode { get; set; } = null!;
        public string NodeName { get; set; } = null!;
        public string NodeType { get; set; } = null!;
        public int SequenceNo { get; set; }
        public string NodeStatus { get; set; } = "PENDING";
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? ApprovalMode { get; set; }
        public int RequiredApprovals { get; set; } = 1;
        public int ApprovedCount { get; set; } = 0;
        public int RejectedCount { get; set; } = 0;
        public string? Remark { get; set; }

        public virtual WfInstance WorkflowInstance { get; set; } = null!;
        public virtual ICollection<WfTask> Tasks { get; set; } = new List<WfTask>();
        public virtual ICollection<WfActionLog> ActionLogs { get; set; } = new List<WfActionLog>();
        public virtual ICollection<WfAdditionalApprover> AdditionalApprovers { get; set; } = new List<WfAdditionalApprover>();
    }
    #endregion

    #region 05. 簽核任務與紀錄 (Workflow Task & Action Log)
    public class WfTask
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong WorkflowInstanceNid { get; set; }
        public ulong InstanceNodeNid { get; set; }
        public string TaskNo { get; set; } = null!;
        public string AssigneeType { get; set; } = null!;
        public string AssigneeSid { get; set; } = null!;
        public string? ActualUserSid { get; set; }
        public string? DelegatedFromSid { get; set; }
        public string TaskStatus { get; set; } = "PENDING";
        public DateTime AssignedDate { get; set; }
        public DateTime? ClaimedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "NORMAL";
        public string? Remark { get; set; }

        public virtual WfInstance WorkflowInstance { get; set; } = null!;
        public virtual WfInstanceNode InstanceNode { get; set; } = null!;
    }

    public class WfActionLog
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong WorkflowInstanceNid { get; set; }
        public ulong? InstanceNodeNid { get; set; }
        public string? TaskSid { get; set; }
        public string ActionCode { get; set; } = null!;
        public string ActionUserSid { get; set; } = null!;
        public string? ActionEmployeeSid { get; set; }
        public string? FromNodeCode { get; set; }
        public string? ToNodeCode { get; set; }
        public string? ActionComment { get; set; }
        public string? AttachmentJson { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        public virtual WfInstance WorkflowInstance { get; set; } = null!;
        public virtual WfInstanceNode? InstanceNode { get; set; }
    }
    #endregion

    #region 06. 代理與加簽 (Delegate & Additional Approver)
    public class WfDelegate
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string PrincipalUserSid { get; set; } = null!;
        public string DelegateUserSid { get; set; } = null!;
        public string? WorkflowCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? DelegateReason { get; set; }
        public string DelegateStatus { get; set; } = "ACTIVE";
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }
    }

    public class WfAdditionalApprover
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong WorkflowInstanceNid { get; set; }
        public ulong InstanceNodeNid { get; set; }
        public string RequesterUserSid { get; set; } = null!;
        public string ApproverUserSid { get; set; } = null!;
        public string AddType { get; set; } = "PARALLEL";
        public string? AddReason { get; set; }
        public string? TaskSid { get; set; }
        public string AddStatus { get; set; } = "PENDING";
        public DateTime? CompletedDate { get; set; }

        public virtual WfInstance WorkflowInstance { get; set; } = null!;
        public virtual WfInstanceNode InstanceNode { get; set; } = null!;
    }
    #endregion

    #region 07. 催辦與通知 (Reminder)
    public class WfReminder
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong WorkflowInstanceNid { get; set; }
        public string TaskSid { get; set; } = null!;
        public string ReminderType { get; set; } = null!;
        public string ReceiverUserSid { get; set; } = null!;
        public int ReminderCount { get; set; } = 1;
        public string? ReminderMessage { get; set; }
        public string? NotificationSid { get; set; }
        public string SendStatus { get; set; } = "PENDING";
        public DateTime? SentDate { get; set; }

        public virtual WfInstance WorkflowInstance { get; set; } = null!;
    }
    #endregion

    #region 08. 流程事件 (Event Log)
    public class WfEventLog
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong WorkflowInstanceNid { get; set; }
        public string EventCode { get; set; } = null!;
        public string? EventName { get; set; }
        public string? EventData { get; set; }
        public string? SourceServiceCode { get; set; }
        public string? OutboxEventSid { get; set; }
        public string ProcessStatus { get; set; } = "PENDING";
        public DateTime? ProcessedDate { get; set; }
        public string? ErrorMessage { get; set; }

        public virtual WfInstance WorkflowInstance { get; set; } = null!;
    }
    #endregion
}