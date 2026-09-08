using Microsoft.EntityFrameworkCore;
using B2bOrder.Resources.ShareCore.Models;

namespace B2bOrder.Resources.ShareCore
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        public DbSet<IdnUser> IdnUsers { get; set; } = null!;
        public DbSet<IdnUserPassword> IdnUserPasswords { get; set; } = null!;
        public DbSet<IdnPasswordReset> IdnPasswordResets { get; set; } = null!;
        public DbSet<IdnPasswordPolicy> IdnPasswordPolicies { get; set; } = null!;
        public DbSet<IdnUserPolicy> IdnUserPolicies { get; set; } = null!;
        public DbSet<IdnRole> IdnRoles { get; set; } = null!;
        public DbSet<IdnResource> IdnResources { get; set; } = null!;
        public DbSet<IdnPermission> IdnPermissions { get; set; } = null!;
        public DbSet<IdnRolePermission> IdnRolePermissions { get; set; } = null!;
        public DbSet<IdnUserRole> IdnUserRoles { get; set; } = null!;
        public DbSet<IdnDataScope> IdnDataScopes { get; set; } = null!;
        public DbSet<IdnRoleDataScope> IdnRoleDataScopes { get; set; } = null!;
        public DbSet<IdnUserDataScope> IdnUserDataScopes { get; set; } = null!;
        public DbSet<IdnMfaMethod> IdnMfaMethods { get; set; } = null!;
        public DbSet<IdnVerificationCode> IdnVerificationCodes { get; set; } = null!;
        public DbSet<IdnRecoveryCode> IdnRecoveryCodes { get; set; } = null!;
        public DbSet<IdnSession> IdnSessions { get; set; } = null!;
        public DbSet<IdnTokenRevocation> IdnTokenRevocations { get; set; } = null!;
        public DbSet<IdnIdentityProvider> IdnIdentityProviders { get; set; } = null!;
        public DbSet<IdnExternalIdentity> IdnExternalIdentities { get; set; } = null!;
        public DbSet<IdnApiClient> IdnApiClients { get; set; } = null!;
        public DbSet<IdnApiKey> IdnApiKeys { get; set; } = null!;
        public DbSet<IdnLoginEvent> IdnLoginEvents { get; set; } = null!;
        public DbSet<IdnGroup> IdnGroups { get; set; } = null!;
        public DbSet<IdnGroupMember> IdnGroupMembers { get; set; } = null!;
        public DbSet<IdnGroupRole> IdnGroupRoles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 01. IdnUser
            modelBuilder.Entity<IdnUser>(entity =>
            {
                entity.ToTable("idn_user");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.Account).HasColumnName("account").HasMaxLength(100).IsRequired();
                entity.Property(e => e.DisplayName).HasColumnName("display_name").HasMaxLength(200);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(200);
                entity.Property(e => e.Mobile).HasColumnName("mobile").HasMaxLength(50);
                entity.Property(e => e.UserType).HasColumnName("user_type").HasMaxLength(30).HasDefaultValue("INTERNAL");
                entity.Property(e => e.MemberSid).HasColumnName("member_sid").HasMaxLength(32);
                entity.Property(e => e.EmployeeSid).HasColumnName("employee_sid").HasMaxLength(32);
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32);
                entity.Property(e => e.BusinessUnitSid).HasColumnName("business_unit_sid").HasMaxLength(32);
                entity.Property(e => e.DepartmentSid).HasColumnName("department_sid").HasMaxLength(32);
                entity.Property(e => e.LanguageSid).HasColumnName("language_sid").HasMaxLength(32);
                entity.Property(e => e.TimezoneCode).HasColumnName("timezone_code").HasMaxLength(100).HasDefaultValue("Asia/Taipei");
                entity.Property(e => e.AccountStatus).HasColumnName("account_status").HasMaxLength(30).HasDefaultValue("PENDING");
                entity.Property(e => e.EmailVerified).HasColumnName("email_verified").HasColumnType("tinyint(1)");
                entity.Property(e => e.MobileVerified).HasColumnName("mobile_verified").HasColumnType("tinyint(1)");
                entity.Property(e => e.ForcePasswordChange).HasColumnName("force_password_change").HasColumnType("tinyint(1)");
                entity.Property(e => e.PasswordChangedDate).HasColumnName("password_changed_date");
                entity.Property(e => e.LastLoginDate).HasColumnName("last_login_date");
                entity.Property(e => e.LastLoginIp).HasColumnName("last_login_ip").HasMaxLength(50);
                entity.Property(e => e.FailedLoginCount).HasColumnName("failed_login_count");
                entity.Property(e => e.LockedUntil).HasColumnName("locked_until");
                entity.Property(e => e.LastReadAllNoticesTime).HasColumnName("last_read_all_notices_time");
                entity.Property(e => e.VersionNo).HasColumnName("version_no");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.Account).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // IdnUserPassword
            modelBuilder.Entity<IdnUserPassword>(entity =>
            {
                entity.ToTable("idn_user_password");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.UserNid).HasColumnName("user_nid");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(500).IsRequired();
                entity.Property(e => e.PasswordAlgorithm).HasColumnName("password_algorithm").HasMaxLength(50).HasDefaultValue("ARGON2ID");
                entity.Property(e => e.PasswordSalt).HasColumnName("password_salt").HasMaxLength(255);
                entity.Property(e => e.PasswordVersion).HasColumnName("password_version").HasDefaultValue(1);
                entity.Property(e => e.EffectiveDate).HasColumnName("effective_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(e => e.CurrentMark).HasColumnName("current_mark").HasColumnType("tinyint(1)").HasDefaultValue(true);
                entity.Property(e => e.CompromisedMark).HasColumnName("compromised_mark").HasColumnType("tinyint(1)");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.UserNid, e.PasswordVersion }).IsUnique();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserPasswords)
                    .HasForeignKey(d => d.UserNid)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // IdnPasswordReset
            modelBuilder.Entity<IdnPasswordReset>(entity =>
            {
                entity.ToTable("idn_password_reset");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UserSid).HasColumnName("user_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.ResetTokenHash).HasColumnName("reset_token_hash").HasMaxLength(255).IsRequired();
                entity.Property(e => e.ResetChannel).HasColumnName("reset_channel").HasMaxLength(20).IsRequired();
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(e => e.UsedDate).HasColumnName("used_date");
                entity.Property(e => e.RequestIp).HasColumnName("request_ip").HasMaxLength(50);
                entity.Property(e => e.RequestUserAgent).HasColumnName("request_user_agent").HasMaxLength(1000);
                entity.Property(e => e.ResetStatus).HasColumnName("reset_status").HasMaxLength(20).HasDefaultValue("PENDING");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ResetTokenHash).IsUnique();
            });

            // 02. IdnPasswordPolicy
            modelBuilder.Entity<IdnPasswordPolicy>(entity =>
            {
                entity.ToTable("idn_password_policy");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.PolicyCode).HasColumnName("policy_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.PolicyName).HasColumnName("policy_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.UserType).HasColumnName("user_type").HasMaxLength(30);
                entity.Property(e => e.MinimumLength).HasColumnName("minimum_length").HasDefaultValue(8);
                entity.Property(e => e.MaximumLength).HasColumnName("maximum_length").HasDefaultValue(128);
                entity.Property(e => e.RequireUppercase).HasColumnName("require_uppercase").HasColumnType("tinyint(1)").HasDefaultValue(true);
                entity.Property(e => e.RequireLowercase).HasColumnName("require_lowercase").HasColumnType("tinyint(1)").HasDefaultValue(true);
                entity.Property(e => e.RequireNumber).HasColumnName("require_number").HasColumnType("tinyint(1)").HasDefaultValue(true);
                entity.Property(e => e.RequireSymbol).HasColumnName("require_symbol").HasColumnType("tinyint(1)").HasDefaultValue(true);
                entity.Property(e => e.HistoryCount).HasColumnName("history_count").HasDefaultValue(5);
                entity.Property(e => e.ExpiryDays).HasColumnName("expiry_days");
                entity.Property(e => e.WarningDays).HasColumnName("warning_days");
                entity.Property(e => e.MaximumFailedAttempts).HasColumnName("maximum_failed_attempts").HasDefaultValue(5);
                entity.Property(e => e.LockMinutes).HasColumnName("lock_minutes").HasDefaultValue(30);
                entity.Property(e => e.MfaRequired).HasColumnName("mfa_required").HasColumnType("tinyint(1)");
                entity.Property(e => e.PolicyStatus).HasColumnName("policy_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.PolicyCode).IsUnique();
            });

            // IdnUserPolicy
            modelBuilder.Entity<IdnUserPolicy>(entity =>
            {
                entity.ToTable("idn_user_policy");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UserNid).HasColumnName("user_nid");
                entity.Property(e => e.PasswordPolicyNid).HasColumnName("password_policy_nid");
                entity.Property(e => e.StartDate).HasColumnName("start_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EndDate).HasColumnName("end_date");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.UserNid, e.PasswordPolicyNid, e.StartDate }).IsUnique();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserPolicies)
                    .HasForeignKey(d => d.UserNid);

                entity.HasOne(d => d.PasswordPolicy)
                    .WithMany(p => p.UserPolicies)
                    .HasForeignKey(d => d.PasswordPolicyNid);
            });

            // 03. IdnRole
            modelBuilder.Entity<IdnRole>(entity =>
            {
                entity.ToTable("idn_role");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.RoleCode).HasColumnName("role_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.RoleName).HasColumnName("role_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.RoleType).HasColumnName("role_type").HasMaxLength(30).HasDefaultValue("BUSINESS");
                entity.Property(e => e.ParentSid).HasColumnName("parent_sid").HasMaxLength(32);
                entity.Property(e => e.SystemMark).HasColumnName("system_mark").HasColumnType("tinyint(1)");
                entity.Property(e => e.AssignableMark).HasColumnName("assignable_mark").HasColumnType("tinyint(1)").HasDefaultValue(true);
                entity.Property(e => e.RoleStatus).HasColumnName("role_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.RoleCode).IsUnique();
            });

            // IdnResource
            modelBuilder.Entity<IdnResource>(entity =>
            {
                entity.ToTable("idn_resource");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.ServiceCode).HasColumnName("service_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.ResourceCode).HasColumnName("resource_code").HasMaxLength(200).IsRequired();
                entity.Property(e => e.ResourceName).HasColumnName("resource_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.ResourceType).HasColumnName("resource_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.ParentSid).HasColumnName("parent_sid").HasMaxLength(32);
                entity.Property(e => e.RoutePath).HasColumnName("route_path").HasMaxLength(500);
                entity.Property(e => e.ApiPattern).HasColumnName("api_pattern").HasMaxLength(1000);
                entity.Property(e => e.HttpMethod).HasColumnName("http_method").HasMaxLength(20);
                entity.Property(e => e.SortNo).HasColumnName("sort_no");
                entity.Property(e => e.ResourceStatus).HasColumnName("resource_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.ServiceCode, e.ResourceCode }).IsUnique();
            });

            // IdnPermission
            modelBuilder.Entity<IdnPermission>(entity =>
            {
                entity.ToTable("idn_permission");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.PermissionCode).HasColumnName("permission_code").HasMaxLength(200).IsRequired();
                entity.Property(e => e.PermissionName).HasColumnName("permission_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.PermissionType).HasColumnName("permission_type").HasMaxLength(30).HasDefaultValue("ACTION");
                entity.Property(e => e.ActionCode).HasColumnName("action_code").HasMaxLength(50).IsRequired();
                entity.Property(e => e.ResourceSid).HasColumnName("resource_sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.SensitiveMark).HasColumnName("sensitive_mark").HasColumnType("tinyint(1)");
                entity.Property(e => e.RequireMfa).HasColumnName("require_mfa").HasColumnType("tinyint(1)");
                entity.Property(e => e.PermissionStatus).HasColumnName("permission_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.PermissionCode).IsUnique();
            });

            // IdnRolePermission
            modelBuilder.Entity<IdnRolePermission>(entity =>
            {
                entity.ToTable("idn_role_permission");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.RoleNid).HasColumnName("role_nid");
                entity.Property(e => e.PermissionNid).HasColumnName("permission_nid");
                entity.Property(e => e.EffectType).HasColumnName("effect_type").HasMaxLength(10).HasDefaultValue("ALLOW");
                entity.Property(e => e.ConditionJson).HasColumnName("condition_json");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.EndDate).HasColumnName("end_date");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.RoleNid, e.PermissionNid }).IsUnique();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.RoleNid);

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.PermissionNid);
            });

            // IdnUserRole
            modelBuilder.Entity<IdnUserRole>(entity =>
            {
                entity.ToTable("idn_user_role");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UserNid).HasColumnName("user_nid");
                entity.Property(e => e.RoleNid).HasColumnName("role_nid");
                entity.Property(e => e.AssignmentType).HasColumnName("assignment_type").HasMaxLength(20).HasDefaultValue("DIRECT");
                entity.Property(e => e.AssignmentSourceSid).HasColumnName("assignment_source_sid").HasMaxLength(32);
                entity.Property(e => e.CompanySid).HasColumnName("company_sid").HasMaxLength(32);
                entity.Property(e => e.BusinessUnitSid).HasColumnName("business_unit_sid").HasMaxLength(32);
                entity.Property(e => e.DepartmentSid).HasColumnName("department_sid").HasMaxLength(32);
                entity.Property(e => e.ProjectSid).HasColumnName("project_sid").HasMaxLength(32);
                entity.Property(e => e.StartDate).HasColumnName("start_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.GrantedUserSid).HasColumnName("granted_user_sid").HasMaxLength(32);
                entity.Property(e => e.GrantReason).HasColumnName("grant_reason").HasMaxLength(1000);
                entity.Property(e => e.AssignmentStatus).HasColumnName("assignment_status").HasMaxLength(20).HasDefaultValue("ACTIVE");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new {
                    e.UserNid,
                    e.RoleNid,
                    e.AssignmentType,
                    e.AssignmentSourceSid,
                    e.CompanySid,
                    e.BusinessUnitSid,
                    e.DepartmentSid,
                    e.ProjectSid
                }, "uk_iur_assignment").IsUnique();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserNid);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleNid);
            });

            // 04. IdnDataScope
            modelBuilder.Entity<IdnDataScope>(entity =>
            {
                entity.ToTable("idn_data_scope");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.ScopeCode).HasColumnName("scope_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ScopeName).HasColumnName("scope_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.ScopeType).HasColumnName("scope_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.ScopeExpression).HasColumnName("scope_expression");
                entity.Property(e => e.ScopeConfig).HasColumnName("scope_config");
                entity.Property(e => e.ScopeStatus).HasColumnName("scope_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ScopeCode).IsUnique();
            });

            // IdnRoleDataScope
            modelBuilder.Entity<IdnRoleDataScope>(entity =>
            {
                entity.ToTable("idn_role_data_scope");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.RoleNid).HasColumnName("role_nid");
                entity.Property(e => e.DataScopeNid).HasColumnName("data_scope_nid");
                entity.Property(e => e.ResourceType).HasColumnName("resource_type").HasMaxLength(100);
                entity.Property(e => e.EffectType).HasColumnName("effect_type").HasMaxLength(10).HasDefaultValue("ALLOW");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.RoleNid, e.DataScopeNid, e.ResourceType }).IsUnique();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleDataScopes)
                    .HasForeignKey(d => d.RoleNid);

                entity.HasOne(d => d.DataScope)
                    .WithMany(p => p.RoleDataScopes)
                    .HasForeignKey(d => d.DataScopeNid);
            });

            // IdnUserDataScope
            modelBuilder.Entity<IdnUserDataScope>(entity =>
            {
                entity.ToTable("idn_user_data_scope");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UserNid).HasColumnName("user_nid");
                entity.Property(e => e.DataScopeNid).HasColumnName("data_scope_nid");
                entity.Property(e => e.ResourceType).HasColumnName("resource_type").HasMaxLength(100);
                entity.Property(e => e.EffectType).HasColumnName("effect_type").HasMaxLength(10).HasDefaultValue("ALLOW");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.EndDate).HasColumnName("end_date");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.UserNid, e.DataScopeNid, e.ResourceType }).IsUnique();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserDataScopes)
                    .HasForeignKey(d => d.UserNid);

                entity.HasOne(d => d.DataScope)
                    .WithMany(p => p.UserDataScopes)
                    .HasForeignKey(d => d.DataScopeNid);
            });

            // 05. IdnMfaMethod
            modelBuilder.Entity<IdnMfaMethod>(entity =>
            {
                entity.ToTable("idn_mfa_method");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.UserNid).HasColumnName("user_nid");
                entity.Property(e => e.MethodType).HasColumnName("method_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.MethodName).HasColumnName("method_name").HasMaxLength(100);
                entity.Property(e => e.SecretEncrypted).HasColumnName("secret_encrypted");
                entity.Property(e => e.EndpointMasked).HasColumnName("endpoint_masked").HasMaxLength(200);
                entity.Property(e => e.CredentialId).HasColumnName("credential_id").HasMaxLength(1000);
                entity.Property(e => e.PublicKeyData).HasColumnName("public_key_data");
                entity.Property(e => e.Verified).HasColumnName("verified").HasColumnType("tinyint(1)");
                entity.Property(e => e.VerifiedDate).HasColumnName("verified_date");
                entity.Property(e => e.PrimaryMark).HasColumnName("primary_mark").HasColumnType("tinyint(1)");
                entity.Property(e => e.MethodStatus).HasColumnName("method_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.LastUsedDate).HasColumnName("last_used_date");

                entity.HasIndex(e => e.Sid).IsUnique();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MfaMethods)
                    .HasForeignKey(d => d.UserNid);
            });

            // IdnVerificationCode
            modelBuilder.Entity<IdnVerificationCode>(entity =>
            {
                entity.ToTable("idn_verification_code");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UserSid).HasColumnName("user_sid").HasMaxLength(32);
                entity.Property(e => e.PurposeCode).HasColumnName("purpose_code").HasMaxLength(50).IsRequired();
                entity.Property(e => e.ChannelType).HasColumnName("channel_type").HasMaxLength(20).IsRequired();
                entity.Property(e => e.EndpointHash).HasColumnName("endpoint_hash").HasMaxLength(128);
                entity.Property(e => e.CodeHash).HasColumnName("code_hash").HasMaxLength(255).IsRequired();
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(e => e.UsedDate).HasColumnName("used_date");
                entity.Property(e => e.AttemptCount).HasColumnName("attempt_count");
                entity.Property(e => e.MaxAttemptCount).HasColumnName("max_attempt_count").HasDefaultValue(5);
                entity.Property(e => e.VerificationStatus).HasColumnName("verification_status").HasMaxLength(20).HasDefaultValue("PENDING");
                entity.Property(e => e.RequestIp).HasColumnName("request_ip").HasMaxLength(50);

                entity.HasIndex(e => e.Sid).IsUnique();
            });

            // IdnRecoveryCode
            modelBuilder.Entity<IdnRecoveryCode>(entity =>
            {
                entity.ToTable("idn_recovery_code");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UserNid).HasColumnName("user_nid");
                entity.Property(e => e.CodeHash).HasColumnName("code_hash").HasMaxLength(255).IsRequired();
                entity.Property(e => e.UsedDate).HasColumnName("used_date");
                entity.Property(e => e.RecoveryStatus).HasColumnName("recovery_status").HasMaxLength(20).HasDefaultValue("ACTIVE");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.CodeHash).IsUnique();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RecoveryCodes)
                    .HasForeignKey(d => d.UserNid);
            });

            // 06. IdnSession
            modelBuilder.Entity<IdnSession>(entity =>
            {
                entity.ToTable("idn_session");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.UserNid).HasColumnName("user_nid");
                entity.Property(e => e.SessionTokenHash).HasColumnName("session_token_hash").HasMaxLength(255).IsRequired();
                entity.Property(e => e.RefreshTokenHash).HasColumnName("refresh_token_hash").HasMaxLength(255);
                entity.Property(e => e.ClientType).HasColumnName("client_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.ClientId).HasColumnName("client_id").HasMaxLength(100);
                entity.Property(e => e.DeviceId).HasColumnName("device_id").HasMaxLength(200);
                entity.Property(e => e.DeviceName).HasColumnName("device_name").HasMaxLength(200);
                entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(1000);
                entity.Property(e => e.CountryCode).HasColumnName("country_code").HasMaxLength(10);
                entity.Property(e => e.MfaVerified).HasColumnName("mfa_verified").HasColumnType("tinyint(1)");
                entity.Property(e => e.IssuedDate).HasColumnName("issued_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(e => e.RefreshExpiryDate).HasColumnName("refresh_expiry_date");
                entity.Property(e => e.LastActivityDate).HasColumnName("last_activity_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.RevokedDate).HasColumnName("revoked_date");
                entity.Property(e => e.RevokeReason).HasColumnName("revoke_reason").HasMaxLength(500);
                entity.Property(e => e.SessionStatus).HasColumnName("session_status").HasMaxLength(20).HasDefaultValue("ACTIVE");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.SessionTokenHash).IsUnique();
                entity.HasIndex(e => e.RefreshTokenHash).IsUnique();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.UserNid);
            });

            // IdnTokenRevocation
            modelBuilder.Entity<IdnTokenRevocation>(entity =>
            {
                entity.ToTable("idn_token_revocation");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.TokenType).HasColumnName("token_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.TokenHash).HasColumnName("token_hash").HasMaxLength(255).IsRequired();
                entity.Property(e => e.UserSid).HasColumnName("user_sid").HasMaxLength(32);
                entity.Property(e => e.ClientId).HasColumnName("client_id").HasMaxLength(100);
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(e => e.RevokeReason).HasColumnName("revoke_reason").HasMaxLength(500);
                entity.Property(e => e.RevokedUserSid).HasColumnName("revoked_user_sid").HasMaxLength(32);

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.TokenHash).IsUnique();
            });

            // 07. IdnIdentityProvider
            modelBuilder.Entity<IdnIdentityProvider>(entity =>
            {
                entity.ToTable("idn_identity_provider");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.ProviderCode).HasColumnName("provider_code").HasMaxLength(80).IsRequired();
                entity.Property(e => e.ProviderName).HasColumnName("provider_name").HasMaxLength(150).IsRequired();
                entity.Property(e => e.ProviderType).HasColumnName("provider_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.IssuerUrl).HasColumnName("issuer_url").HasMaxLength(1000);
                entity.Property(e => e.AuthorizationUrl).HasColumnName("authorization_url").HasMaxLength(1000);
                entity.Property(e => e.TokenUrl).HasColumnName("token_url").HasMaxLength(1000);
                entity.Property(e => e.UserInfoUrl).HasColumnName("user_info_url").HasMaxLength(1000);
                entity.Property(e => e.ClientId).HasColumnName("client_id").HasMaxLength(300);
                entity.Property(e => e.ClientSecretEncrypted).HasColumnName("client_secret_encrypted");
                entity.Property(e => e.Scopes).HasColumnName("scopes").HasMaxLength(1000);
                entity.Property(e => e.AttributeMapping).HasColumnName("attribute_mapping");
                entity.Property(e => e.AutoCreateUser).HasColumnName("auto_create_user").HasColumnType("tinyint(1)");
                entity.Property(e => e.ProviderStatus).HasColumnName("provider_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ProviderCode).IsUnique();
            });

            // IdnExternalIdentity
            modelBuilder.Entity<IdnExternalIdentity>(entity =>
            {
                entity.ToTable("idn_external_identity");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.UserNid).HasColumnName("user_nid");
                entity.Property(e => e.IdentityProviderNid).HasColumnName("identity_provider_nid");
                entity.Property(e => e.ExternalSubject).HasColumnName("external_subject").HasMaxLength(500).IsRequired();
                entity.Property(e => e.ExternalAccount).HasColumnName("external_account").HasMaxLength(300);
                entity.Property(e => e.ExternalEmail).HasColumnName("external_email").HasMaxLength(300);
                entity.Property(e => e.ProfileData).HasColumnName("profile_data");
                entity.Property(e => e.LinkedDate).HasColumnName("linked_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.LastLoginDate).HasColumnName("last_login_date");
                entity.Property(e => e.LinkStatus).HasColumnName("link_status").HasMaxLength(20).HasDefaultValue("ACTIVE");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.IdentityProviderNid, e.ExternalSubject }).IsUnique();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ExternalIdentities)
                    .HasForeignKey(d => d.UserNid);

                entity.HasOne(d => d.IdentityProvider)
                    .WithMany(p => p.ExternalIdentities)
                    .HasForeignKey(d => d.IdentityProviderNid);
            });

            // 08. IdnApiClient
            modelBuilder.Entity<IdnApiClient>(entity =>
            {
                entity.ToTable("idn_api_client");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.ClientId).HasColumnName("client_id").HasMaxLength(150).IsRequired();
                entity.Property(e => e.ClientName).HasColumnName("client_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.ClientType).HasColumnName("client_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.OwnerType).HasColumnName("owner_type").HasMaxLength(30).IsRequired();
                entity.Property(e => e.OwnerSid).HasColumnName("owner_sid").HasMaxLength(32);
                entity.Property(e => e.ClientSecretHash).HasColumnName("client_secret_hash").HasMaxLength(500);
                entity.Property(e => e.RedirectUris).HasColumnName("redirect_uris");
                entity.Property(e => e.AllowedGrantTypes).HasColumnName("allowed_grant_types");
                entity.Property(e => e.AllowedScopes).HasColumnName("allowed_scopes");
                entity.Property(e => e.TokenLifetimeSeconds).HasColumnName("token_lifetime_seconds").HasDefaultValue(3600);
                entity.Property(e => e.RefreshLifetimeSeconds).HasColumnName("refresh_lifetime_seconds").HasDefaultValue(2592000);
                entity.Property(e => e.IpAllowlist).HasColumnName("ip_allowlist");
                entity.Property(e => e.ClientStatus).HasColumnName("client_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.LastUsedDate).HasColumnName("last_used_date");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.ClientId).IsUnique();
            });

            // IdnApiKey
            modelBuilder.Entity<IdnApiKey>(entity =>
            {
                entity.ToTable("idn_api_key");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.ApiClientNid).HasColumnName("api_client_nid");
                entity.Property(e => e.KeyName).HasColumnName("key_name").HasMaxLength(150).IsRequired();
                entity.Property(e => e.KeyPrefix).HasColumnName("key_prefix").HasMaxLength(30).IsRequired();
                entity.Property(e => e.KeyHash).HasColumnName("key_hash").HasMaxLength(500).IsRequired();
                entity.Property(e => e.AllowedScopes).HasColumnName("allowed_scopes");
                entity.Property(e => e.IpAllowlist).HasColumnName("ip_allowlist");
                entity.Property(e => e.StartDate).HasColumnName("start_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(e => e.LastUsedDate).HasColumnName("last_used_date");
                entity.Property(e => e.RevokedDate).HasColumnName("revoked_date");
                entity.Property(e => e.RevokeReason).HasColumnName("revoke_reason").HasMaxLength(500);
                entity.Property(e => e.KeyStatus).HasColumnName("key_status").HasMaxLength(20).HasDefaultValue("ACTIVE");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.KeyHash).IsUnique();

                entity.HasOne(d => d.ApiClient)
                    .WithMany(p => p.ApiKeys)
                    .HasForeignKey(d => d.ApiClientNid);
            });

            // 09. IdnLoginEvent
            modelBuilder.Entity<IdnLoginEvent>(entity =>
            {
                entity.ToTable("idn_login_event");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UserSid).HasColumnName("user_sid").HasMaxLength(32);
                entity.Property(e => e.AccountSnapshot).HasColumnName("account_snapshot").HasMaxLength(150);
                entity.Property(e => e.EventCode).HasColumnName("event_code").HasMaxLength(50).IsRequired();
                entity.Property(e => e.AuthenticationMethod).HasColumnName("authentication_method").HasMaxLength(30);
                entity.Property(e => e.ClientType).HasColumnName("client_type").HasMaxLength(30);
                entity.Property(e => e.ClientId).HasColumnName("client_id").HasMaxLength(150);
                entity.Property(e => e.SessionSid).HasColumnName("session_sid").HasMaxLength(32);
                entity.Property(e => e.Result).HasColumnName("result").HasMaxLength(20).IsRequired();
                entity.Property(e => e.FailReasonCode).HasColumnName("fail_reason_code").HasMaxLength(100);
                entity.Property(e => e.FailReason).HasColumnName("fail_reason").HasMaxLength(500);
                entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(50);
                entity.Property(e => e.CountryCode).HasColumnName("country_code").HasMaxLength(10);
                entity.Property(e => e.DeviceId).HasColumnName("device_id").HasMaxLength(200);
                entity.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(1000);
                entity.Property(e => e.RiskScore).HasColumnName("risk_score").HasPrecision(8, 2);
                entity.Property(e => e.RiskFlags).HasColumnName("risk_flags");

                entity.HasIndex(e => e.Sid).IsUnique();
            });

            // 10. IdnGroup
            modelBuilder.Entity<IdnGroup>(entity =>
            {
                entity.ToTable("idn_group");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
                entity.Property(e => e.GroupCode).HasColumnName("group_code").HasMaxLength(100).IsRequired();
                entity.Property(e => e.GroupName).HasColumnName("group_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.GroupType).HasColumnName("group_type").HasMaxLength(30).HasDefaultValue("CUSTOM");
                entity.Property(e => e.ReferenceSid).HasColumnName("reference_sid").HasMaxLength(32);
                entity.Property(e => e.GroupStatus).HasColumnName("group_status").HasMaxLength(20).HasDefaultValue("ACTIVE");
                entity.Property(e => e.Avalible).HasColumnName("avalible").HasMaxLength(2).HasDefaultValue("Y");
                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => e.GroupCode).IsUnique();
            });

            // IdnGroupMember
            modelBuilder.Entity<IdnGroupMember>(entity =>
            {
                entity.ToTable("idn_group_member");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.GroupNid).HasColumnName("group_nid");
                entity.Property(e => e.UserNid).HasColumnName("user_nid");
                entity.Property(e => e.MemberRole).HasColumnName("member_role").HasMaxLength(30).HasDefaultValue("MEMBER");
                entity.Property(e => e.StartDate).HasColumnName("start_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.MemberStatus).HasColumnName("member_status").HasMaxLength(20).HasDefaultValue("ACTIVE");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.GroupNid, e.UserNid }).IsUnique();

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.GroupMembers)
                    .HasForeignKey(d => d.GroupNid);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.GroupMembers)
                    .HasForeignKey(d => d.UserNid);
            });

            // IdnGroupRole
            modelBuilder.Entity<IdnGroupRole>(entity =>
            {
                entity.ToTable("idn_group_role");
                entity.HasKey(e => e.Nid);
                entity.Property(e => e.Nid).HasColumnName("nid");
                entity.Property(e => e.Sid).HasColumnName("sid").HasMaxLength(32).IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.GroupNid).HasColumnName("group_nid");
                entity.Property(e => e.RoleNid).HasColumnName("role_nid");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.EndDate).HasColumnName("end_date");

                entity.HasIndex(e => e.Sid).IsUnique();
                entity.HasIndex(e => new { e.GroupNid, e.RoleNid }).IsUnique();

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.GroupRoles)
                    .HasForeignKey(d => d.GroupNid);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.GroupRoles)
                    .HasForeignKey(d => d.RoleNid);
            });
        }
    }
}