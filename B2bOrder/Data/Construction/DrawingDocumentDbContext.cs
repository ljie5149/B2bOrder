using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.Construction.Models;

namespace B2bOrder.Resources.Construction
{
    public class DrawingDocumentDbContext : DbContext
    {
        public DrawingDocumentDbContext(DbContextOptions<DrawingDocumentDbContext> options) : base(options)
        {
        }

        #region DbSet Configurations - 資料集設定

        public DbSet<DmsDrawingMaster> DmsDrawingMasters { get; set; }
        public DbSet<DmsDrawingApprovalLog> DmsDrawingApprovalLogs { get; set; }
        public DbSet<DmsDocumentSubmittal> DmsDocumentSubmittals { get; set; }
        public DbSet<DmsDrawingTransmittal> DmsDrawingTransmittals { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Index Configurations - 唯一索引設定

            // 01. dms_drawing_master
            modelBuilder.Entity<DmsDrawingMaster>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<DmsDrawingMaster>()
                .HasIndex(e => new { e.ProjectSid, e.DrawingNumber, e.RevisionCode })
                .IsUnique()
                .HasDatabaseName("uk_ddm_project_drawing_rev");

            // 02. dms_drawing_approval_log
            modelBuilder.Entity<DmsDrawingApprovalLog>()
                .HasIndex(e => e.Sid).IsUnique();

            // 03. dms_document_submittal
            modelBuilder.Entity<DmsDocumentSubmittal>()
                .HasIndex(e => e.Sid).IsUnique();
            modelBuilder.Entity<DmsDocumentSubmittal>()
                .HasIndex(e => new { e.ProjectSid, e.SubmittalNo })
                .IsUnique()
                .HasDatabaseName("uk_dds_project_number");

            // 04. dms_drawing_transmittal
            modelBuilder.Entity<DmsDrawingTransmittal>()
                .HasIndex(e => e.Sid).IsUnique();

            #endregion

            #region Composite Index Configurations - 複合索引與一般索引設定

            // 01. dms_drawing_master
            modelBuilder.Entity<DmsDrawingMaster>()
                .HasIndex(e => e.CompanySid);
            modelBuilder.Entity<DmsDrawingMaster>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<DmsDrawingMaster>()
                .HasIndex(e => e.Discipline);
            modelBuilder.Entity<DmsDrawingMaster>()
                .HasIndex(e => e.IsCurrentVersion);
            modelBuilder.Entity<DmsDrawingMaster>()
                .HasIndex(e => e.ApprovalStatus);
            modelBuilder.Entity<DmsDrawingMaster>()
                .HasIndex(e => e.Avalible);

            // 02. dms_drawing_approval_log
            modelBuilder.Entity<DmsDrawingApprovalLog>()
                .HasIndex(e => e.DrawingSid);
            modelBuilder.Entity<DmsDrawingApprovalLog>()
                .HasIndex(e => e.ReviewerUserSid);

            // 03. dms_document_submittal
            modelBuilder.Entity<DmsDocumentSubmittal>()
                .HasIndex(e => e.ProjectSid);
            modelBuilder.Entity<DmsDocumentSubmittal>()
                .HasIndex(e => e.VendorSid);
            modelBuilder.Entity<DmsDocumentSubmittal>()
                .HasIndex(e => e.SubmittalStatus);
            modelBuilder.Entity<DmsDocumentSubmittal>()
                .HasIndex(e => e.Avalible);

            // 04. dms_drawing_transmittal
            modelBuilder.Entity<DmsDrawingTransmittal>()
                .HasIndex(e => e.DrawingSid);
            modelBuilder.Entity<DmsDrawingTransmittal>()
                .HasIndex(e => e.RecipientVendorSid);
            modelBuilder.Entity<DmsDrawingTransmittal>()
                .HasIndex(e => e.TransmittalStatus);

            #endregion

            #region Default Value Configurations - 預設值設定

            // 01. dms_drawing_master
            modelBuilder.Entity<DmsDrawingMaster>()
                .Property(e => e.DrawingType).HasDefaultValue("SHOP_DRAWING");
            modelBuilder.Entity<DmsDrawingMaster>()
                .Property(e => e.RevisionCode).HasDefaultValue("Rev.0");
            modelBuilder.Entity<DmsDrawingMaster>()
                .Property(e => e.IsCurrentVersion).HasDefaultValue("Y");
            modelBuilder.Entity<DmsDrawingMaster>()
                .Property(e => e.ApprovalStatus).HasDefaultValue("UNDER_REVIEW");
            modelBuilder.Entity<DmsDrawingMaster>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<DmsDrawingMaster>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 03. dms_document_submittal
            modelBuilder.Entity<DmsDocumentSubmittal>()
                .Property(e => e.SubmittalType).HasDefaultValue("MATERIAL_SAMPLE");
            modelBuilder.Entity<DmsDocumentSubmittal>()
                .Property(e => e.SubmittalStatus).HasDefaultValue("SUBMITTED");
            modelBuilder.Entity<DmsDocumentSubmittal>()
                .Property(e => e.VersionNo).HasDefaultValue(0UL);
            modelBuilder.Entity<DmsDocumentSubmittal>()
                .Property(e => e.Avalible).HasDefaultValue("Y");

            // 04. dms_drawing_transmittal
            modelBuilder.Entity<DmsDrawingTransmittal>()
                .Property(e => e.CopiesIssued).HasDefaultValue(1);
            modelBuilder.Entity<DmsDrawingTransmittal>()
                .Property(e => e.IssuePurpose).HasDefaultValue("FOR_CONSTRUCTION");
            modelBuilder.Entity<DmsDrawingTransmittal>()
                .Property(e => e.TransmittalStatus).HasDefaultValue("ISSUED");

            #endregion
        }
    }
}