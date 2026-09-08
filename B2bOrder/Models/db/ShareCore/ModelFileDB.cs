using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.File
{
    #region 01. 儲存供應商與儲存空間

    [Table("fil_storage_provider")]
    public class FilStorageProvider
    {
        [Key]
        [Column("nid")]
        public uint Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("provider_code")]
        public string ProviderCode { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        [Column("provider_name")]
        public string ProviderName { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("provider_type")]
        public string ProviderType { get; set; } = null!; // LOCAL, S3, AZURE_BLOB, GCS, MINIO, FTP

        [MaxLength(500)]
        [Column("endpoint_url")]
        public string? EndpointUrl { get; set; }

        [Column("credential_encrypted", TypeName = "longtext")]
        public string? CredentialEncrypted { get; set; }

        [Column("config_json", TypeName = "json")]
        public string? ConfigJson { get; set; }

        [MaxLength(200)]
        [Column("default_bucket")]
        public string? DefaultBucket { get; set; }

        [MaxLength(500)]
        [Column("public_base_url")]
        public string? PublicBaseUrl { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("provider_status")]
        public string ProviderStatus { get; set; } = "ACTIVE"; // ACTIVE, PAUSED, DISABLED

        [Column("priority")]
        public int Priority { get; set; } = 0;

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }

    [Table("fil_storage_space")]
    public class FilStorageSpace
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("provider_nid")]
        public uint ProviderNid { get; set; }

        [Required]
        [MaxLength(80)]
        [Column("space_code")]
        public string SpaceCode { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        [Column("space_name")]
        public string SpaceName { get; set; } = null!;

        [MaxLength(200)]
        [Column("bucket_name")]
        public string? BucketName { get; set; }

        [MaxLength(500)]
        [Column("base_path")]
        public string? BasePath { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("access_type")]
        public string AccessType { get; set; } = "PRIVATE"; // PRIVATE, PUBLIC, SIGNED_URL

        [Column("max_file_size")]
        public ulong? MaxFileSize { get; set; }

        [Column("total_quota")]
        public ulong? TotalQuota { get; set; }

        [Column("used_size")]
        public ulong UsedSize { get; set; } = 0;

        [Column("allowed_extensions", TypeName = "json")]
        public string? AllowedExtensions { get; set; }

        [Column("allowed_mime_types", TypeName = "json")]
        public string? AllowedMimeTypes { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("space_status")]
        public string SpaceStatus { get; set; } = "ACTIVE"; // ACTIVE, READ_ONLY, DISABLED

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }

        [ForeignKey(nameof(ProviderNid))]
        public virtual FilStorageProvider? StorageProvider { get; set; }
    }

    #endregion

    #region 02. 資料夾

    [Table("fil_folder")]
    public class FilFolder
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("storage_space_nid")]
        public ulong StorageSpaceNid { get; set; }

        [Column("parent_nid")]
        public ulong? ParentNid { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("folder_code")]
        public string FolderCode { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        [Column("folder_name")]
        public string FolderName { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        [Column("physical_path")]
        public string PhysicalPath { get; set; } = null!;

        [MaxLength(2000)]
        [Column("tree_path")]
        public string? TreePath { get; set; }

        [Column("folder_level")]
        public int FolderLevel { get; set; } = 1;

        [MaxLength(30)]
        [Column("owner_type")]
        public string? OwnerType { get; set; } // SYSTEM, USER, STORE, SERVICE

        [MaxLength(32)]
        [Column("owner_sid")]
        public string? OwnerSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("access_type")]
        public string AccessType { get; set; } = "PRIVATE"; // PRIVATE, PUBLIC, INHERIT

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }

        [ForeignKey(nameof(StorageSpaceNid))]
        public virtual FilStorageSpace? StorageSpace { get; set; }

        [ForeignKey(nameof(ParentNid))]
        public virtual FilFolder? ParentFolder { get; set; }
    }

    #endregion

    #region 03. 檔案主檔與版本

    [Table("fil_file")]
    public class FilFile
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("folder_nid")]
        public ulong? FolderNid { get; set; }

        [Column("storage_space_nid")]
        public ulong StorageSpaceNid { get; set; }

        [Required]
        [MaxLength(80)]
        [Column("file_no")]
        public string FileNo { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        [Column("original_name")]
        public string OriginalName { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        [Column("stored_name")]
        public string StoredName { get; set; } = null!;

        [MaxLength(30)]
        [Column("file_extension")]
        public string? FileExtension { get; set; }

        [MaxLength(150)]
        [Column("mime_type")]
        public string? MimeType { get; set; }

        [Column("file_size")]
        public ulong FileSize { get; set; } = 0;

        [Required]
        [MaxLength(2000)]
        [Column("storage_path")]
        public string StoragePath { get; set; } = null!;

        [MaxLength(2000)]
        [Column("public_url")]
        public string? PublicUrl { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("checksum_type")]
        public string ChecksumType { get; set; } = "SHA256";

        [Required]
        [MaxLength(128)]
        [Column("checksum_value")]
        public string ChecksumValue { get; set; } = null!;

        [Column("current_version")]
        public int CurrentVersion { get; set; } = 1;

        [MaxLength(50)]
        [Column("file_category")]
        public string? FileCategory { get; set; } // IMAGE, DOCUMENT, VIDEO, AUDIO, ARCHIVE, OTHER

        [Required]
        [MaxLength(30)]
        [Column("upload_source")]
        public string UploadSource { get; set; } = "USER"; // USER, SYSTEM, IMPORT, API

        [MaxLength(32)]
        [Column("uploader_sid")]
        public string? UploaderSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("access_type")]
        public string AccessType { get; set; } = "PRIVATE";

        [Required]
        [MaxLength(20)]
        [Column("virus_scan_status")]
        public string VirusScanStatus { get; set; } = "PENDING"; // PENDING, CLEAN, INFECTED, FAILED

        [Required]
        [MaxLength(20)]
        [Column("process_status")]
        public string ProcessStatus { get; set; } = "READY"; // UPLOADING, READY, PROCESSING, FAILED, DELETED

        [Column("retention_until")]
        public DateTime? RetentionUntil { get; set; }

        [Column("legal_hold")]
        public bool LegalHold { get; set; } = false;

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }

        [ForeignKey(nameof(FolderNid))]
        public virtual FilFolder? Folder { get; set; }

        [ForeignKey(nameof(StorageSpaceNid))]
        public virtual FilStorageSpace? StorageSpace { get; set; }
    }

    [Table("fil_file_version")]
    public class FilFileVersion
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("file_nid")]
        public ulong FileNid { get; set; }

        [Column("version_no")]
        public int VersionNo { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("stored_name")]
        public string StoredName { get; set; } = null!;

        [Required]
        [MaxLength(2000)]
        [Column("storage_path")]
        public string StoragePath { get; set; } = null!;

        [Column("file_size")]
        public ulong FileSize { get; set; } = 0;

        [Required]
        [MaxLength(128)]
        [Column("checksum_value")]
        public string ChecksumValue { get; set; } = null!;

        [MaxLength(150)]
        [Column("mime_type")]
        public string? MimeType { get; set; }

        [MaxLength(32)]
        [Column("uploader_sid")]
        public string? UploaderSid { get; set; }

        [MaxLength(500)]
        [Column("change_reason")]
        public string? ChangeReason { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("virus_scan_status")]
        public string VirusScanStatus { get; set; } = "PENDING";

        [Required]
        [MaxLength(20)]
        [Column("version_status")]
        public string VersionStatus { get; set; } = "ACTIVE"; // ACTIVE, ARCHIVED, DELETED

        [ForeignKey(nameof(FileNid))]
        public virtual FilFile? File { get; set; }
    }

    #endregion

    #region 04. 業務附件關聯

    [Table("fil_reference")]
    public class FilReference
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("file_nid")]
        public ulong FileNid { get; set; }

        [Required]
        [MaxLength(80)]
        [Column("reference_service")]
        public string ReferenceService { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("reference_type")]
        public string ReferenceType { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("reference_sid")]
        public string ReferenceSid { get; set; } = null!;

        [MaxLength(32)]
        [Column("reference_item_sid")]
        public string? ReferenceItemSid { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("attachment_type")]
        public string AttachmentType { get; set; } = "ATTACHMENT"; // ATTACHMENT, COVER, IMAGE, CONTRACT, REPORT

        [MaxLength(500)]
        [Column("title")]
        public string? Title { get; set; }

        [Column("description", TypeName = "text")]
        public string? Description { get; set; }

        [Column("sort_no")]
        public int SortNo { get; set; } = 0;

        [Column("is_primary")]
        public bool IsPrimary { get; set; } = false;

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [ForeignKey(nameof(FileNid))]
        public virtual FilFile? File { get; set; }
    }

    #endregion

    #region 05. 縮圖與衍生檔

    [Table("fil_derivative")]
    public class FilDerivative
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("file_nid")]
        public ulong FileNid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("derivative_type")]
        public string DerivativeType { get; set; } = null!; // THUMBNAIL, PREVIEW, WATERMARK, TRANSCODE, PDF_PREVIEW

        [Column("width")]
        public int? Width { get; set; }

        [Column("height")]
        public int? Height { get; set; }

        [MaxLength(150)]
        [Column("mime_type")]
        public string? MimeType { get; set; }

        [Column("file_size")]
        public ulong FileSize { get; set; } = 0;

        [Required]
        [MaxLength(2000)]
        [Column("storage_path")]
        public string StoragePath { get; set; } = null!;

        [MaxLength(2000)]
        [Column("public_url")]
        public string? PublicUrl { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("process_status")]
        public string ProcessStatus { get; set; } = "PENDING"; // PENDING, PROCESSING, SUCCESS, FAILED

        [Column("error_message", TypeName = "text")]
        public string? ErrorMessage { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [ForeignKey(nameof(FileNid))]
        public virtual FilFile? File { get; set; }
    }

    [Table("fil_processing_job")]
    public class FilProcessingJob
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("file_nid")]
        public ulong FileNid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("job_type")]
        public string JobType { get; set; } = null!; // VIRUS_SCAN, THUMBNAIL, OCR, TRANSCODE, METADATA

        [Column("job_config", TypeName = "json")]
        public string? JobConfig { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("job_status")]
        public string JobStatus { get; set; } = "PENDING"; // PENDING, PROCESSING, SUCCESS, FAILED, DEAD

        [Column("retry_count")]
        public int RetryCount { get; set; } = 0;

        [Column("max_retry_count")]
        public int MaxRetryCount { get; set; } = 3;

        [Column("next_retry_date")]
        public DateTime? NextRetryDate { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("completed_date")]
        public DateTime? CompletedDate { get; set; }

        [Column("result_data", TypeName = "json")]
        public string? ResultData { get; set; }

        [Column("error_message", TypeName = "longtext")]
        public string? ErrorMessage { get; set; }

        [ForeignKey(nameof(FileNid))]
        public virtual FilFile? File { get; set; }
    }

    #endregion

    #region 06. 檔案權限與分享

    [Table("fil_permission")]
    public class FilPermission
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(20)]
        [Column("target_type")]
        public string TargetType { get; set; } = null!; // FILE, FOLDER

        [Required]
        [MaxLength(32)]
        [Column("target_sid")]
        public string TargetSid { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("principal_type")]
        public string PrincipalType { get; set; } = null!; // USER, ROLE, STORE, SERVICE, PUBLIC

        [MaxLength(32)]
        [Column("principal_sid")]
        public string? PrincipalSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("permission_type")]
        public string PermissionType { get; set; } = null!; // READ, WRITE, DELETE, SHARE, OWNER

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [MaxLength(32)]
        [Column("granted_user_sid")]
        public string? GrantedUserSid { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }

    [Table("fil_share_link")]
    public class FilShareLink
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("file_nid")]
        public ulong? FileNid { get; set; }

        [Column("folder_nid")]
        public ulong? FolderNid { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("share_token_hash")]
        public string ShareTokenHash { get; set; } = null!;

        [MaxLength(255)]
        [Column("password_hash")]
        public string? PasswordHash { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Column("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        [Column("max_download_count")]
        public int? MaxDownloadCount { get; set; }

        [Column("download_count")]
        public int DownloadCount { get; set; } = 0;

        [Column("allow_preview")]
        public bool AllowPreview { get; set; } = true;

        [Column("allow_download")]
        public bool AllowDownload { get; set; } = true;

        [Required]
        [MaxLength(20)]
        [Column("share_status")]
        public string ShareStatus { get; set; } = "ACTIVE"; // ACTIVE, EXPIRED, REVOKED

        [MaxLength(32)]
        [Column("create_user_sid")]
        public string? CreateUserSid { get; set; }

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }

        [ForeignKey(nameof(FileNid))]
        public virtual FilFile? File { get; set; }

        [ForeignKey(nameof(FolderNid))]
        public virtual FilFolder? Folder { get; set; }
    }

    [Table("fil_access_log")]
    public class FilAccessLog
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("access_date")]
        public DateTime AccessDate { get; set; } = DateTime.UtcNow;

        [MaxLength(32)]
        [Column("file_sid")]
        public string? FileSid { get; set; }

        [MaxLength(32)]
        [Column("folder_sid")]
        public string? FolderSid { get; set; }

        [MaxLength(32)]
        [Column("share_link_sid")]
        public string? ShareLinkSid { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("actor_type")]
        public string ActorType { get; set; } = null!; // USER, SERVICE, ANONYMOUS

        [MaxLength(32)]
        [Column("actor_sid")]
        public string? ActorSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("action_type")]
        public string ActionType { get; set; } = null!; // VIEW, DOWNLOAD, UPLOAD, UPDATE, DELETE, SHARE

        [Required]
        [MaxLength(20)]
        [Column("result")]
        public string Result { get; set; } = "SUCCESS"; // SUCCESS, DENIED, FAILED

        [MaxLength(50)]
        [Column("ip_address")]
        public string? IpAddress { get; set; }

        [MaxLength(1000)]
        [Column("user_agent")]
        public string? UserAgent { get; set; }

        [MaxLength(100)]
        [Column("request_id")]
        public string? RequestId { get; set; }

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }

    #endregion

    #region 07. 分段上傳

    [Table("fil_upload_session")]
    public class FilUploadSession
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("upload_id")]
        public string UploadId { get; set; } = null!;

        [Column("storage_space_nid")]
        public ulong StorageSpaceNid { get; set; }

        [Column("folder_nid")]
        public ulong? FolderNid { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("original_name")]
        public string OriginalName { get; set; } = null!;

        [MaxLength(150)]
        [Column("mime_type")]
        public string? MimeType { get; set; }

        [Column("total_size")]
        public ulong TotalSize { get; set; }

        [Column("chunk_size")]
        public ulong ChunkSize { get; set; }

        [Column("total_chunks")]
        public int TotalChunks { get; set; }

        [Column("uploaded_chunks")]
        public int UploadedChunks { get; set; } = 0;

        [MaxLength(32)]
        [Column("uploader_sid")]
        public string? UploaderSid { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("upload_status")]
        public string UploadStatus { get; set; } = "UPLOADING"; // UPLOADING, COMPLETING, SUCCESS, FAILED, CANCELLED, EXPIRED

        [Column("expiry_date")]
        public DateTime ExpiryDate { get; set; }

        [MaxLength(32)]
        [Column("completed_file_sid")]
        public string? CompletedFileSid { get; set; }

        [Column("error_message", TypeName = "text")]
        public string? ErrorMessage { get; set; }

        [ForeignKey(nameof(StorageSpaceNid))]
        public virtual FilStorageSpace? StorageSpace { get; set; }

        [ForeignKey(nameof(FolderNid))]
        public virtual FilFolder? Folder { get; set; }
    }

    [Table("fil_upload_chunk")]
    public class FilUploadChunk
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Column("upload_session_nid")]
        public ulong UploadSessionNid { get; set; }

        [Column("chunk_no")]
        public int ChunkNo { get; set; }

        [Column("chunk_size")]
        public ulong ChunkSize { get; set; }

        [MaxLength(128)]
        [Column("checksum_value")]
        public string? ChecksumValue { get; set; }

        [Required]
        [MaxLength(2000)]
        [Column("storage_path")]
        public string StoragePath { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("chunk_status")]
        public string ChunkStatus { get; set; } = "UPLOADED"; // UPLOADING, UPLOADED, VERIFIED, FAILED

        [ForeignKey(nameof(UploadSessionNid))]
        public virtual FilUploadSession? UploadSession { get; set; }
    }

    #endregion
}