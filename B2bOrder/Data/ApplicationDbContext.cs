using Microsoft.EntityFrameworkCore;
using B2bOrder.Models.Db;
using B2bOrder.Models;

namespace B2bOrder.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==========================================
        // 原有 Db Models (Data Tables)
        // ==========================================
        public DbSet<DataMember> DataMembers { get; set; } = null!;
        public DbSet<DataMemberFile> DataMemberFiles { get; set; } = null!;
        public DbSet<SysUser> SysUsers { get; set; } = null!;
        public DbSet<SysConfig> SysConfigs { get; set; } = null!;
        public DbSet<LogImport> LogImports { get; set; } = null!;
        public DbSet<LogExport> LogExports { get; set; } = null!;
        public DbSet<LogAction> LogActions { get; set; } = null!;
        public DbSet<LogPush> LogPushes { get; set; } = null!;
        public DbSet<SysNoticeSetting> SysNoticeSettings { get; set; } = null!;

        // ==========================================
        // 新增 #2 資料表的 DbSet 宣告
        // ==========================================
        public DbSet<SysModule> SysModules { get; set; } = null!;
        public DbSet<SysNoticeEvent> SysNoticeEvents { get; set; } = null!;
        public DbSet<SysNoticeChannel> SysNoticeChannels { get; set; } = null!;
        public DbSet<SysIntegration> SysIntegrations { get; set; } = null!;
        public DbSet<SysIntegrationConfig> SysIntegrationConfigs { get; set; } = null!;
        public DbSet<SysIpWhitelist> SysIpWhitelists { get; set; } = null!;
        public DbSet<SysJob> SysJobs { get; set; } = null!;
        public DbSet<SysJobLog> SysJobLogs { get; set; } = null!;
        public DbSet<DataNotice> DataNotices { get; set; } = null!;

        // ==========================================
        // 【全新新增】待辦事項與公告已讀的 DbSet 宣告
        // ==========================================
        public DbSet<DataTodoCalendar> DataTodoCalendar { get; set; } = null!;

        // 💡 新增：會員公告已讀紀錄表
        public DbSet<DataNoticeRead> DataNoticeReads { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region 原有實體與 Table Splitting 設定

            // 兩個實體共享同一資料表（table splitting / shared table）
            modelBuilder.Entity<DataMember>().ToTable("data_member");
            modelBuilder.Entity<Member>().ToTable("data_member");

            modelBuilder.Entity<DataMember>().HasKey(d => d.Nid);
            modelBuilder.Entity<Member>().HasKey(m => m.Nid);

            modelBuilder.Entity<Member>()
                .HasOne<DataMember>()
                .WithOne()
                .HasForeignKey<Member>(m => m.Nid);

            // 統一設定 DataMember 的欄位屬性
            modelBuilder.Entity<DataMember>(b =>
            {
                b.Property(d => d.Sid).HasMaxLength(20);
                b.Property(d => d.Mid).HasMaxLength(20);
                b.Property(d => d.ParentSid).HasMaxLength(20);
                b.Property(d => d.Name).HasMaxLength(50);
                b.Property(d => d.EngName).HasMaxLength(50);
                b.Property(d => d.HeadImg).HasMaxLength(255);
                b.Property(d => d.Iden).HasMaxLength(255);
                b.Property(d => d.CmpCode).HasMaxLength(10);
                b.Property(d => d.Role).HasMaxLength(3);
                b.Property(d => d.AuthorizationPage).HasMaxLength(100);
                b.Property(d => d.Address).HasMaxLength(500);
                b.Property(d => d.Mobile).HasMaxLength(20);
                b.Property(d => d.Tel).HasMaxLength(20);
                b.Property(d => d.Fax).HasMaxLength(20);
                b.Property(d => d.Email).HasMaxLength(100);
                b.Property(d => d.Avalible).HasMaxLength(2);
                b.Property(d => d.SignaturePic).HasMaxLength(255);
                b.Property(d => d.AdvertisingId).HasMaxLength(200);
                b.Property(d => d.DeviceId).HasMaxLength(200);
                b.Property(d => d.EditSid).HasMaxLength(20);
            });

            // 為 Sid 建立唯一索引並設為 alternate/principal key
            modelBuilder.Entity<DataMember>()
                .HasIndex(d => d.Sid)
                .IsUnique();

            modelBuilder.Entity<DataMember>()
                .HasAlternateKey(d => d.Sid)
                .HasName("AK_DataMember_Sid");

            // 原有外鍵關係設定
            modelBuilder.Entity<SysUser>()
                .HasOne(s => s.Member)
                .WithMany()
                .HasForeignKey(s => s.MemberSid)
                .HasPrincipalKey(m => m.Sid);

            modelBuilder.Entity<LogPush>()
                .HasOne(lp => lp.Member)
                .WithMany(dm => dm.LogPushes)
                .HasForeignKey(lp => lp.MemberSid)
                .HasPrincipalKey(m => m.Sid);

            modelBuilder.Entity<DataMemberFile>(b =>
            {
                b.Property(f => f.MemberSid).HasMaxLength(20);
                b.HasOne(f => f.Member)
                 .WithMany(m => m.Files)
                 .HasForeignKey(f => f.MemberSid)
                 .HasPrincipalKey(m => m.Sid);
            });

            #endregion

            #region 新增 #2 資料表 Fluent API 設定

            // 1. 系統參數設定表
            modelBuilder.Entity<SysConfig>(b =>
            {
                b.ToTable("sys_config");
                b.HasKey(c => c.Nid);
                b.HasIndex(c => c.ConfigKey).IsUnique();
                b.Property(c => c.ConfigKey).HasMaxLength(100).IsRequired();
                b.Property(c => c.ConfigName).HasMaxLength(100).IsRequired();
                b.Property(c => c.ConfigType).HasMaxLength(20).HasDefaultValue("TEXT");
                b.Property(c => c.Category).HasMaxLength(50);
            });

            // 2. 系統功能模組設定
            modelBuilder.Entity<SysModule>(b =>
            {
                b.ToTable("sys_module");
                b.HasKey(m => m.Nid);
                b.HasIndex(m => m.ModuleCode).IsUnique();
                b.Property(m => m.ModuleCode).HasMaxLength(50).IsRequired();
                b.Property(m => m.ModuleName).HasMaxLength(100).IsRequired();
                b.Property(m => m.ModuleDesc).HasMaxLength(500);
                b.Property(m => m.Icon).HasMaxLength(100);
                b.Property(m => m.RouteUrl).HasMaxLength(255);
                b.Property(m => m.SortNo).HasDefaultValue(0);
                b.Property(m => m.Avalible).HasMaxLength(2).HasDefaultValue("Y");
            });

            // 3. 通知事件設定
            modelBuilder.Entity<SysNoticeEvent>(b =>
            {
                b.ToTable("sys_notice_event");
                b.HasKey(e => e.Nid);
                b.HasIndex(e => e.EventCode).IsUnique();
                b.Property(e => e.EventCode).HasMaxLength(50).IsRequired();
                b.Property(e => e.EventName).HasMaxLength(100).IsRequired();
                b.Property(e => e.Script).HasMaxLength(500);
                b.Property(e => e.Avalible).HasMaxLength(2).HasDefaultValue("Y");
            });

            // 4. 通知發送管道設定
            modelBuilder.Entity<SysNoticeChannel>(b =>
            {
                b.ToTable("sys_notice_channel");
                b.HasKey(c => c.Nid);
                b.Property(c => c.EventCode).HasMaxLength(50).IsRequired();
                b.Property(c => c.ChannelType).HasMaxLength(20).IsRequired();
                b.Property(c => c.ReceiverType).HasMaxLength(20).IsRequired();
                b.Property(c => c.Avalible).HasMaxLength(2).HasDefaultValue("Y");

                b.HasOne(c => c.NoticeEvent)
                 .WithMany(e => e.NoticeChannels)
                 .HasForeignKey(c => c.EventCode)
                 .HasPrincipalKey(e => e.EventCode)
                 .HasConstraintName("fk_notice_event");
            });

            // 5. 通知範本設定
            modelBuilder.Entity<SysNoticeSetting>(b =>
            {
                b.ToTable("sys_notice_setting");
                b.HasKey(s => s.Nid);
                b.Property(s => s.NoticeType).HasMaxLength(50).IsRequired();
                b.Property(s => s.ChannelType).HasMaxLength(20).IsRequired();
                b.Property(s => s.DaysBefore).HasDefaultValue(0);
                b.Property(s => s.Title).HasMaxLength(200).IsRequired();
                b.Property(s => s.Content).IsRequired();
                b.Property(s => s.Avalible).HasMaxLength(2).HasDefaultValue("Y");
            });

            // 6. 第三方服務整合設定
            modelBuilder.Entity<SysIntegration>(b =>
            {
                b.ToTable("sys_integration");
                b.HasKey(i => i.Nid);
                b.HasIndex(i => i.ServiceCode).IsUnique();
                b.Property(i => i.ServiceCode).HasMaxLength(50).IsRequired();
                b.Property(i => i.ServiceName).HasMaxLength(100).IsRequired();
                b.Property(i => i.ServiceType).HasMaxLength(20).IsRequired();
                b.Property(i => i.ConnectStatus).HasMaxLength(20).HasDefaultValue("DISCONNECT");
                b.Property(i => i.Avalible).HasMaxLength(2).HasDefaultValue("Y");
            });

            // 7. 第三方服務參數
            modelBuilder.Entity<SysIntegrationConfig>(b =>
            {
                b.ToTable("sys_integration_config");
                b.HasKey(ic => ic.Nid);
                b.Property(ic => ic.ServiceCode).HasMaxLength(50).IsRequired();
                b.Property(ic => ic.ConfigKey).HasMaxLength(100).IsRequired();

                b.HasIndex(ic => new { ic.ServiceCode, ic.ConfigKey })
                 .IsUnique()
                 .HasDatabaseName("uk_service_key");
            });

            // 8. IP白名單
            modelBuilder.Entity<SysIpWhitelist>(b =>
            {
                b.ToTable("sys_ip_whitelist");
                b.HasKey(ip => ip.Nid);
                b.Property(ip => ip.IpAddress).HasMaxLength(100).IsRequired();
                b.Property(ip => ip.IpName).HasMaxLength(100);
                b.Property(ip => ip.Avalible).HasMaxLength(2).HasDefaultValue("Y");
            });

            // 9. 系統排程工作
            modelBuilder.Entity<SysJob>(b =>
            {
                b.ToTable("sys_job");
                b.HasKey(j => j.Nid);
                b.HasIndex(j => j.JobCode).IsUnique();
                b.Property(j => j.JobCode).HasMaxLength(50).IsRequired();
                b.Property(j => j.JobName).HasMaxLength(100).IsRequired();
                b.Property(j => j.CronExpression).HasMaxLength(100).IsRequired();
                b.Property(j => j.Avalible).HasMaxLength(2).HasDefaultValue("Y");
            });

            // 10. 排程執行紀錄
            modelBuilder.Entity<SysJobLog>(b =>
            {
                b.ToTable("sys_job_log");
                b.HasKey(jl => jl.Nid);
                b.Property(jl => jl.JobCode).HasMaxLength(50).IsRequired();
                b.Property(jl => jl.ExecuteResult).HasMaxLength(20);
            });

            // 11. 系統公告
            modelBuilder.Entity<DataNotice>(b =>
            {
                b.ToTable("data_notice");
                b.HasKey(n => n.Nid);
                b.HasIndex(n => n.Sid).IsUnique();
                b.Property(n => n.Sid).HasMaxLength(32).IsRequired();
                b.Property(n => n.Title).HasMaxLength(255).IsRequired();
                b.Property(n => n.Content).IsRequired();
                b.Property(n => n.Status).HasMaxLength(10).HasDefaultValue("啟用").IsRequired();
                b.Property(n => n.Category).HasMaxLength(20).HasDefaultValue("一般公告").IsRequired();
                b.Property(n => n.PublishUnit).HasMaxLength(50).HasDefaultValue("系統管理部").IsRequired();
                b.Property(n => n.Avalible).HasMaxLength(2).HasDefaultValue("Y").IsRequired();
                b.Property(n => n.ClickCount).HasDefaultValue(0).IsRequired();
                b.Property(n => n.IsTop).HasDefaultValue(0).IsRequired();
                b.Property(n => n.PublishDate).IsRequired();
                b.Property(n => n.StartTime);
                b.Property(n => n.EndTime);
                b.Property(n => n.StartDate);
                b.Property(n => n.EndDate);
                b.Property(n => n.CreateUserSid).HasMaxLength(32);
                b.Property(n => n.CreateDate).IsRequired();
                b.Property(n => n.ModifyUserSid).HasMaxLength(32);
                b.Property(n => n.ModifyDate);
            });

            // 💡 新增：11-2. 會員公告已讀紀錄表
            modelBuilder.Entity<DataNoticeRead>(b =>
            {
                b.ToTable("data_notice_read");
                b.HasKey(r => r.Id);
                b.Property(r => r.NoticeSid).HasColumnName("notice_sid").HasMaxLength(32).IsRequired();
                b.Property(r => r.MemberSid).HasColumnName("member_sid").HasMaxLength(32).IsRequired();
                b.Property(r => r.ReadTime).HasColumnName("read_time").IsRequired();

                // 建立 UNIQUE 複合索引：(member_sid, notice_sid)，對齊之前的 SQL 設計
                b.HasIndex(r => new { r.MemberSid, r.NoticeSid })
                 .IsUnique()
                 .HasDatabaseName("uid_member_notice");
            });

            #endregion

            #region 【全新新增】12. 待辦事項行事曆 Fluent API 設定
            modelBuilder.Entity<DataTodoCalendar>(b =>
            {
                // 對應資料表名稱
                b.ToTable("data_todo_calendar");

                // 主鍵設定
                b.HasKey(t => t.Nid);
                b.Property(t => t.Nid).HasColumnName("nid").ValueGeneratedOnAdd();

                // 唯一識別碼 Sid
                b.HasIndex(t => t.Sid).IsUnique();
                b.Property(t => t.Sid)
                    .HasColumnName("sid")
                    .HasMaxLength(32)
                    .IsRequired();

                // 關聯使用者
                b.Property(t => t.MemberSid)
                    .HasColumnName("member_sid")
                    .HasMaxLength(32)
                    .IsRequired();

                // 事項核心內容
                b.Property(t => t.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
                b.Property(t => t.Category).HasColumnName("category").HasMaxLength(50).HasDefaultValue("work").IsRequired();
                b.Property(t => t.ClassName).HasColumnName("class_name").HasMaxLength(50).HasDefaultValue("bg-work-event").IsRequired();

                // 時間與排程相關
                b.Property(t => t.StartDate).HasColumnName("start_date").HasColumnType("datetime");
                b.Property(t => t.EndDate).HasColumnName("end_date").HasColumnType("datetime");
                b.Property(t => t.IsAllDay).HasColumnName("is_all_day").HasDefaultValue(false).IsRequired();
                b.Property(t => t.IsScheduled).HasColumnName("is_scheduled").HasDefaultValue(false).IsRequired();

                // 狀態與軌跡
                b.Property(t => t.IsCompleted).HasColumnName("is_completed").HasDefaultValue(false).IsRequired();
                b.Property(t => t.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y").IsRequired();

                // 審計欄位
                b.Property(t => t.CreateSid).HasColumnName("create_sid").HasMaxLength(32);
                b.Property(t => t.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();

                // 💡 修正點：明確對齊資料庫的 modify_date 欄位，避免 Runtime 錯誤
                b.Property(t => t.ModifyDate).HasColumnName("modify_date").HasColumnType("datetime").IsRequired();
            });
            #endregion
        }

        // ==========================================
        // 欄位時間自動更新機制（不變）
        // ==========================================
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var now = DateTime.Now;

            foreach (var entry in ChangeTracker.Entries())
            {
                var hasCreate = entry.Metadata.FindProperty("CreateDate") is not null;
                var hasModify = entry.Metadata.FindProperty("ModifyDate") is not null;

                if (!hasCreate && !hasModify)
                    continue;

                if (entry.State == EntityState.Added)
                {
                    if (hasCreate) entry.Property("CreateDate").CurrentValue = now;
                    if (hasModify) entry.Property("ModifyDate").CurrentValue = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (hasModify) entry.Property("ModifyDate").CurrentValue = now;
                    if (hasCreate) entry.Property("CreateDate").IsModified = false;
                }
            }
        }
    }
}