using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.Construction.Models
{
    #region Drawing Master Module - 工程圖說主檔與版次控管模組

    [Table("dms_drawing_master")]
    public class DmsDrawingMaster
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
        [MaxLength(100)]
        [Column("drawing_number")]
        public string DrawingNumber { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("drawing_title")]
        public string DrawingTitle { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("discipline")]
        public string Discipline { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("drawing_type")]
        public string DrawingType { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("revision_code")]
        public string RevisionCode { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("is_current_version")]
        public string IsCurrentVersion { get; set; }

        [Column("effective_date", TypeName = "date")]
        public DateTime? EffectiveDate { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("file_storage_path")]
        public string FileStoragePath { get; set; }

        [MaxLength(500)]
        [Column("cad_file_path")]
        public string CadFilePath { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("approval_status")]
        public string ApprovalStatus { get; set; }

        [MaxLength(100)]
        [Column("designed_by_architect")]
        public string DesignedByArchitect { get; set; }

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

    #region Drawing Approval Log Module - 圖說簽核與審查歷程模組

    [Table("dms_drawing_approval_log")]
    public class DmsDrawingApprovalLog
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
        [Column("drawing_sid")]
        public string DrawingSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("reviewer_user_sid")]
        public string ReviewerUserSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("reviewer_role")]
        public string ReviewerRole { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("review_action")]
        public string ReviewAction { get; set; }

        [Column("review_comment")]
        public string ReviewComment { get; set; }

        [MaxLength(500)]
        [Column("markup_file_path")]
        public string MarkupFilePath { get; set; }
    }

    #endregion

    #region Document Submittal Control Module - 送審文件與計畫書管理模組

    [Table("dms_document_submittal")]
    public class DmsDocumentSubmittal
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
        [Column("submittal_no")]
        public string SubmittalNo { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("submittal_title")]
        public string SubmittalTitle { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("submittal_type")]
        public string SubmittalType { get; set; }

        [MaxLength(50)]
        [Column("specification_code")]
        public string SpecificationCode { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("file_storage_path")]
        public string FileStoragePath { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("submittal_status")]
        public string SubmittalStatus { get; set; }

        [MaxLength(32)]
        [Column("reviewer_user_sid")]
        public string ReviewerUserSid { get; set; }

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

    #region Drawing Distribution Module - 圖說分發與簽收紀錄模組

    [Table("dms_drawing_transmittal")]
    public class DmsDrawingTransmittal
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
        [Column("drawing_sid")]
        public string DrawingSid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("recipient_vendor_sid")]
        public string RecipientVendorSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("transmittal_no")]
        public string TransmittalNo { get; set; }

        [Column("copies_issued")]
        public int CopiesIssued { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("issue_purpose")]
        public string IssuePurpose { get; set; }

        [MaxLength(100)]
        [Column("recipient_name")]
        public string RecipientName { get; set; }

        [Column("signed_at")]
        public DateTime? SignedAt { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("transmittal_status")]
        public string TransmittalStatus { get; set; }
    }

    #endregion
}