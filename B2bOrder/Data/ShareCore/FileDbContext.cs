using B2bOrder.Models.File;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Data
{
    public class FileDbContext : DbContext
    {
        public FileDbContext(DbContextOptions<FileDbContext> options) : base(options) { }

        // 01. 儲存供應商與儲存空間
        public DbSet<FilStorageProvider> StorageProviders { get; set; } = null!;
        public DbSet<FilStorageSpace> StorageSpaces { get; set; } = null!;

        // 02. 資料夾
        public DbSet<FilFolder> Folders { get; set; } = null!;

        // 03. 檔案主檔與版本
        public DbSet<FilFile> Files { get; set; } = null!;
        public DbSet<FilFileVersion> FileVersions { get; set; } = null!;

        // 04. 業務附件關聯
        public DbSet<FilReference> References { get; set; } = null!;

        // 05. 縮圖與衍生檔
        public DbSet<FilDerivative> Derivatives { get; set; } = null!;
        public DbSet<FilProcessingJob> ProcessingJobs { get; set; } = null!;

        // 06. 檔案權限與分享
        public DbSet<FilPermission> Permissions { get; set; } = null!;
        public DbSet<FilShareLink> ShareLinks { get; set; } = null!;
        public DbSet<FilAccessLog> AccessLogs { get; set; } = null!;

        // 07. 分段上傳
        public DbSet<FilUploadSession> UploadSessions { get; set; } = null!;
        public DbSet<FilUploadChunk> UploadChunks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 01. Storage Provider
            modelBuilder.Entity<FilStorageProvider>(entity =>
            {
                entity.ToTable("fil_storage_provider");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ProviderCode).IsUnique().HasDatabaseName("uk_fsp_provider_code");
                entity.HasIndex(e => e.ProviderType).HasDatabaseName("idx_fsp_provider_type");
                entity.HasIndex(e => e.ProviderStatus).HasDatabaseName("idx_fsp_status");
                entity.HasIndex(e => e.Priority).HasDatabaseName("idx_fsp_priority");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_fsp_avalible");
            });

            // Storage Space
            modelBuilder.Entity<FilStorageSpace>(entity =>
            {
                entity.ToTable("fil_storage_space");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SpaceCode).IsUnique().HasDatabaseName("uk_fss_space_code");
                entity.HasIndex(e => e.ProviderNid).HasDatabaseName("idx_fss_provider_nid");
                entity.HasIndex(e => e.AccessType).HasDatabaseName("idx_fss_access_type");
                entity.HasIndex(e => e.SpaceStatus).HasDatabaseName("idx_fss_status");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_fss_avalible");

                entity.HasOne(d => d.StorageProvider)
                      .WithMany()
                      .HasForeignKey(d => d.ProviderNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_fss_provider");
            });

            // 02. Folder
            modelBuilder.Entity<FilFolder>(entity =>
            {
                entity.ToTable("fil_folder");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.StorageSpaceNid, e.PhysicalPath }).HasDatabaseName("uk_ff_space_path");
                entity.HasIndex(e => e.StorageSpaceNid).HasDatabaseName("idx_ff_storage_space_nid");
                entity.HasIndex(e => e.ParentNid).HasDatabaseName("idx_ff_parent_nid");
                entity.HasIndex(e => new { e.OwnerType, e.OwnerSid }).HasDatabaseName("idx_ff_owner");
                entity.HasIndex(e => e.AccessType).HasDatabaseName("idx_ff_access_type");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_ff_avalible");

                entity.HasOne(d => d.StorageSpace)
                      .WithMany()
                      .HasForeignKey(d => d.StorageSpaceNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_ff_storage_space");

                entity.HasOne(d => d.ParentFolder)
                      .WithMany()
                      .HasForeignKey(d => d.ParentNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_ff_parent");
            });

            // 03. File
            modelBuilder.Entity<FilFile>(entity =>
            {
                entity.ToTable("fil_file");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.FileNo).IsUnique().HasDatabaseName("uk_fff_file_no");
                entity.HasIndex(e => e.FolderNid).HasDatabaseName("idx_fff_folder_nid");
                entity.HasIndex(e => e.StorageSpaceNid).HasDatabaseName("idx_fff_storage_space_nid");
                entity.HasIndex(e => new { e.ChecksumType, e.ChecksumValue }).HasDatabaseName("idx_fff_checksum");
                entity.HasIndex(e => e.FileCategory).HasDatabaseName("idx_fff_file_category");
                entity.HasIndex(e => e.UploaderSid).HasDatabaseName("idx_fff_uploader_sid");
                entity.HasIndex(e => e.VirusScanStatus).HasDatabaseName("idx_fff_virus_scan_status");
                entity.HasIndex(e => e.ProcessStatus).HasDatabaseName("idx_fff_process_status");
                entity.HasIndex(e => e.RetentionUntil).HasDatabaseName("idx_fff_retention_until");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_fff_avalible");

                entity.HasOne(d => d.Folder)
                      .WithMany()
                      .HasForeignKey(d => d.FolderNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_fff_folder");

                entity.HasOne(d => d.StorageSpace)
                      .WithMany()
                      .HasForeignKey(d => d.StorageSpaceNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_fff_storage_space");
            });

            // File Version
            modelBuilder.Entity<FilFileVersion>(entity =>
            {
                entity.ToTable("fil_file_version");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.FileNid, e.VersionNo }).IsUnique().HasDatabaseName("uk_ffv_file_version");
                entity.HasIndex(e => e.FileNid).HasDatabaseName("idx_ffv_file_nid");
                entity.HasIndex(e => e.VersionNo).HasDatabaseName("idx_ffv_version_no");
                entity.HasIndex(e => e.VirusScanStatus).HasDatabaseName("idx_ffv_virus_scan_status");
                entity.HasIndex(e => e.VersionStatus).HasDatabaseName("idx_ffv_status");

                entity.HasOne(d => d.File)
                      .WithMany()
                      .HasForeignKey(d => d.FileNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_ffv_file");
            });

            // 04. Reference
            modelBuilder.Entity<FilReference>(entity =>
            {
                entity.ToTable("fil_reference");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.FileNid, e.ReferenceService, e.ReferenceType, e.ReferenceSid, e.ReferenceItemSid })
                      .IsUnique()
                      .HasDatabaseName("uk_fr_reference");
                entity.HasIndex(e => e.FileNid).HasDatabaseName("idx_fr_file_nid");
                entity.HasIndex(e => new { e.ReferenceService, e.ReferenceType, e.ReferenceSid }).HasDatabaseName("idx_fr_reference");
                entity.HasIndex(e => e.ReferenceItemSid).HasDatabaseName("idx_fr_reference_item_sid");
                entity.HasIndex(e => e.AttachmentType).HasDatabaseName("idx_fr_attachment_type");
                entity.HasIndex(e => e.IsPrimary).HasDatabaseName("idx_fr_primary");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_fr_avalible");

                entity.HasOne(d => d.File)
                      .WithMany()
                      .HasForeignKey(d => d.FileNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_fr_file");
            });

            // 05. Derivative & Processing Job
            modelBuilder.Entity<FilDerivative>(entity =>
            {
                entity.ToTable("fil_derivative");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.FileNid).HasDatabaseName("idx_fd_file_nid");
                entity.HasIndex(e => e.DerivativeType).HasDatabaseName("idx_fd_derivative_type");
                entity.HasIndex(e => new { e.Width, e.Height }).HasDatabaseName("idx_fd_dimension");
                entity.HasIndex(e => e.ProcessStatus).HasDatabaseName("idx_fd_status");

                entity.HasOne(d => d.File)
                      .WithMany()
                      .HasForeignKey(d => d.FileNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_fd_file");
            });

            modelBuilder.Entity<FilProcessingJob>(entity =>
            {
                entity.ToTable("fil_processing_job");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.FileNid).HasDatabaseName("idx_fpj_file_nid");
                entity.HasIndex(e => e.JobType).HasDatabaseName("idx_fpj_job_type");
                entity.HasIndex(e => e.JobStatus).HasDatabaseName("idx_fpj_status");
                entity.HasIndex(e => e.NextRetryDate).HasDatabaseName("idx_fpj_next_retry_date");

                entity.HasOne(d => d.File)
                      .WithMany()
                      .HasForeignKey(d => d.FileNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_fpj_file");
            });

            // 06. Permission, Share & Access Log
            modelBuilder.Entity<FilPermission>(entity =>
            {
                entity.ToTable("fil_permission");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.TargetType, e.TargetSid, e.PrincipalType, e.PrincipalSid, e.PermissionType })
                      .IsUnique()
                      .HasDatabaseName("uk_fp_permission");
                entity.HasIndex(e => new { e.TargetType, e.TargetSid }).HasDatabaseName("idx_fp_target");
                entity.HasIndex(e => new { e.PrincipalType, e.PrincipalSid }).HasDatabaseName("idx_fp_principal");
                entity.HasIndex(e => e.PermissionType).HasDatabaseName("idx_fp_permission_type");
                entity.HasIndex(e => new { e.StartDate, e.EndDate }).HasDatabaseName("idx_fp_date");
                entity.HasIndex(e => e.Avalible).HasDatabaseName("idx_fp_avalible");
            });

            modelBuilder.Entity<FilShareLink>(entity =>
            {
                entity.ToTable("fil_share_link");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ShareTokenHash).IsUnique().HasDatabaseName("uk_fsl_token_hash");
                entity.HasIndex(e => e.FileNid).HasDatabaseName("idx_fsl_file_nid");
                entity.HasIndex(e => e.FolderNid).HasDatabaseName("idx_fsl_folder_nid");
                entity.HasIndex(e => e.ExpiryDate).HasDatabaseName("idx_fsl_expiry_date");
                entity.HasIndex(e => e.ShareStatus).HasDatabaseName("idx_fsl_status");

                entity.HasOne(d => d.File)
                      .WithMany()
                      .HasForeignKey(d => d.FileNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_fsl_file");

                entity.HasOne(d => d.Folder)
                      .WithMany()
                      .HasForeignKey(d => d.FolderNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_fsl_folder");
            });

            modelBuilder.Entity<FilAccessLog>(entity =>
            {
                entity.ToTable("fil_access_log");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.FileSid).HasDatabaseName("idx_fal_file_sid");
                entity.HasIndex(e => e.FolderSid).HasDatabaseName("idx_fal_folder_sid");
                entity.HasIndex(e => e.ShareLinkSid).HasDatabaseName("idx_fal_share_link_sid");
                entity.HasIndex(e => new { e.ActorType, e.ActorSid }).HasDatabaseName("idx_fal_actor");
                entity.HasIndex(e => e.ActionType).HasDatabaseName("idx_fal_action_type");
                entity.HasIndex(e => e.AccessDate).HasDatabaseName("idx_fal_access_date");
                entity.HasIndex(e => e.Result).HasDatabaseName("idx_fal_result");
                entity.HasIndex(e => e.RequestId).HasDatabaseName("idx_fal_request_id");
            });

            // 07. Upload Session & Chunk
            modelBuilder.Entity<FilUploadSession>(entity =>
            {
                entity.ToTable("fil_upload_session");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.UploadId).IsUnique().HasDatabaseName("uk_fus_upload_id");
                entity.HasIndex(e => e.StorageSpaceNid).HasDatabaseName("idx_fus_storage_space_nid");
                entity.HasIndex(e => e.FolderNid).HasDatabaseName("idx_fus_folder_nid");
                entity.HasIndex(e => e.UploaderSid).HasDatabaseName("idx_fus_uploader_sid");
                entity.HasIndex(e => e.UploadStatus).HasDatabaseName("idx_fus_status");
                entity.HasIndex(e => e.ExpiryDate).HasDatabaseName("idx_fus_expiry_date");

                entity.HasOne(d => d.StorageSpace)
                      .WithMany()
                      .HasForeignKey(d => d.StorageSpaceNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_fus_storage_space");

                entity.HasOne(d => d.Folder)
                      .WithMany()
                      .HasForeignKey(d => d.FolderNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_fus_folder");
            });

            modelBuilder.Entity<FilUploadChunk>(entity =>
            {
                entity.ToTable("fil_upload_chunk");
                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.UploadSessionNid, e.ChunkNo }).IsUnique().HasDatabaseName("uk_fuc_session_chunk");
                entity.HasIndex(e => e.UploadSessionNid).HasDatabaseName("idx_fuc_upload_session_nid");
                entity.HasIndex(e => e.ChunkStatus).HasDatabaseName("idx_fuc_status");

                entity.HasOne(d => d.UploadSession)
                      .WithMany()
                      .HasForeignKey(d => d.UploadSessionNid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("fk_fuc_upload_session");
            });
        }
    }
}