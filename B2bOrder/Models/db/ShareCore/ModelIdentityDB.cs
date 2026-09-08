using System;
using System.Collections.Generic;

namespace B2bOrder.Resources.ShareCore.Models
{
    #region 01. User Core & Passwords
    public class IdnUser
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Account { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string UserType { get; set; } = "INTERNAL";
        public string? MemberSid { get; set; }
        public string? EmployeeSid { get; set; }
        public string? CompanySid { get; set; }
        public string? BusinessUnitSid { get; set; }
        public string? DepartmentSid { get; set; }
        public string? LanguageSid { get; set; }
        public string TimezoneCode { get; set; } = "Asia/Taipei";
        public string AccountStatus { get; set; } = "PENDING";
        public bool EmailVerified { get; set; }
        public bool MobileVerified { get; set; }
        public bool ForcePasswordChange { get; set; }
        public DateTime? PasswordChangedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? LastLoginIp { get; set; }
        public int FailedLoginCount { get; set; }
        public DateTime? LockedUntil { get; set; }
        public DateTime? LastReadAllNoticesTime { get; set; }
        public ulong VersionNo { get; set; }
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual ICollection<IdnUserPassword> UserPasswords { get; set; } = new List<IdnUserPassword>();
        public virtual ICollection<IdnUserPolicy> UserPolicies { get; set; } = new List<IdnUserPolicy>();
        public virtual ICollection<IdnUserRole> UserRoles { get; set; } = new List<IdnUserRole>();
        public virtual ICollection<IdnUserDataScope> UserDataScopes { get; set; } = new List<IdnUserDataScope>();
        public virtual ICollection<IdnMfaMethod> MfaMethods { get; set; } = new List<IdnMfaMethod>();
        public virtual ICollection<IdnRecoveryCode> RecoveryCodes { get; set; } = new List<IdnRecoveryCode>();
        public virtual ICollection<IdnSession> Sessions { get; set; } = new List<IdnSession>();
        public virtual ICollection<IdnExternalIdentity> ExternalIdentities { get; set; } = new List<IdnExternalIdentity>();
        public virtual ICollection<IdnGroupMember> GroupMembers { get; set; } = new List<IdnGroupMember>();
    }

    public class IdnUserPassword
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong UserNid { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string PasswordAlgorithm { get; set; } = "ARGON2ID";
        public string? PasswordSalt { get; set; }
        public int PasswordVersion { get; set; } = 1;
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool CurrentMark { get; set; } = true;
        public bool CompromisedMark { get; set; }

        public virtual IdnUser User { get; set; } = null!;
    }

    public class IdnPasswordReset
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string UserSid { get; set; } = null!;
        public string ResetTokenHash { get; set; } = null!;
        public string ResetChannel { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public DateTime? UsedDate { get; set; }
        public string? RequestIp { get; set; }
        public string? RequestUserAgent { get; set; }
        public string ResetStatus { get; set; } = "PENDING";
    }
    #endregion

    #region 02. Password Policies
    public class IdnPasswordPolicy
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string PolicyCode { get; set; } = null!;
        public string PolicyName { get; set; } = null!;
        public string? UserType { get; set; }
        public int MinimumLength { get; set; } = 8;
        public int MaximumLength { get; set; } = 128;
        public bool RequireUppercase { get; set; } = true;
        public bool RequireLowercase { get; set; } = true;
        public bool RequireNumber { get; set; } = true;
        public bool RequireSymbol { get; set; } = true;
        public int HistoryCount { get; set; } = 5;
        public int? ExpiryDays { get; set; }
        public int? WarningDays { get; set; }
        public int MaximumFailedAttempts { get; set; } = 5;
        public int LockMinutes { get; set; } = 30;
        public bool MfaRequired { get; set; }
        public string PolicyStatus { get; set; } = "ACTIVE";
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual ICollection<IdnUserPolicy> UserPolicies { get; set; } = new List<IdnUserPolicy>();
    }

    public class IdnUserPolicy
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong UserNid { get; set; }
        public ulong PasswordPolicyNid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual IdnUser User { get; set; } = null!;
        public virtual IdnPasswordPolicy PasswordPolicy { get; set; } = null!;
    }
    #endregion

    #region 03. Roles, Resources & Permissions
    public class IdnRole
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string RoleCode { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string RoleType { get; set; } = "BUSINESS";
        public string? ParentSid { get; set; }
        public bool SystemMark { get; set; }
        public bool AssignableMark { get; set; } = true;
        public string RoleStatus { get; set; } = "ACTIVE";
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual ICollection<IdnRolePermission> RolePermissions { get; set; } = new List<IdnRolePermission>();
        public virtual ICollection<IdnUserRole> UserRoles { get; set; } = new List<IdnUserRole>();
        public virtual ICollection<IdnRoleDataScope> RoleDataScopes { get; set; } = new List<IdnRoleDataScope>();
        public virtual ICollection<IdnGroupRole> GroupRoles { get; set; } = new List<IdnGroupRole>();
    }

    public class IdnResource
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ServiceCode { get; set; } = null!;
        public string ResourceCode { get; set; } = null!;
        public string ResourceName { get; set; } = null!;
        public string ResourceType { get; set; } = null!;
        public string? ParentSid { get; set; }
        public string? RoutePath { get; set; }
        public string? ApiPattern { get; set; }
        public string? HttpMethod { get; set; }
        public int SortNo { get; set; }
        public string ResourceStatus { get; set; } = "ACTIVE";
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }
    }

    public class IdnPermission
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string PermissionCode { get; set; } = null!;
        public string PermissionName { get; set; } = null!;
        public string PermissionType { get; set; } = "ACTION";
        public string ActionCode { get; set; } = null!;
        public string ResourceSid { get; set; } = null!;
        public bool SensitiveMark { get; set; }
        public bool RequireMfa { get; set; }
        public string PermissionStatus { get; set; } = "ACTIVE";
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual ICollection<IdnRolePermission> RolePermissions { get; set; } = new List<IdnRolePermission>();
    }

    public class IdnRolePermission
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong RoleNid { get; set; }
        public ulong PermissionNid { get; set; }
        public string EffectType { get; set; } = "ALLOW";
        public string? ConditionJson { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual IdnRole Role { get; set; } = null!;
        public virtual IdnPermission Permission { get; set; } = null!;
    }

    public class IdnUserRole
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong UserNid { get; set; }
        public ulong RoleNid { get; set; }
        public string AssignmentType { get; set; } = "DIRECT";
        public string? AssignmentSourceSid { get; set; }
        public string? CompanySid { get; set; }
        public string? BusinessUnitSid { get; set; }
        public string? DepartmentSid { get; set; }
        public string? ProjectSid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? GrantedUserSid { get; set; }
        public string? GrantReason { get; set; }
        public string AssignmentStatus { get; set; } = "ACTIVE";

        public virtual IdnUser User { get; set; } = null!;
        public virtual IdnRole Role { get; set; } = null!;
    }
    #endregion

    #region 04. Data Scopes
    public class IdnDataScope
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ScopeCode { get; set; } = null!;
        public string ScopeName { get; set; } = null!;
        public string ScopeType { get; set; } = null!;
        public string? ScopeExpression { get; set; }
        public string? ScopeConfig { get; set; }
        public string ScopeStatus { get; set; } = "ACTIVE";
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual ICollection<IdnRoleDataScope> RoleDataScopes { get; set; } = new List<IdnRoleDataScope>();
        public virtual ICollection<IdnUserDataScope> UserDataScopes { get; set; } = new List<IdnUserDataScope>();
    }

    public class IdnRoleDataScope
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong RoleNid { get; set; }
        public ulong DataScopeNid { get; set; }
        public string? ResourceType { get; set; }
        public string EffectType { get; set; } = "ALLOW";

        public virtual IdnRole Role { get; set; } = null!;
        public virtual IdnDataScope DataScope { get; set; } = null!;
    }

    public class IdnUserDataScope
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong UserNid { get; set; }
        public ulong DataScopeNid { get; set; }
        public string? ResourceType { get; set; }
        public string EffectType { get; set; } = "ALLOW";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual IdnUser User { get; set; } = null!;
        public virtual IdnDataScope DataScope { get; set; } = null!;
    }
    #endregion

    #region 05. MFA & Verification Codes
    public class IdnMfaMethod
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong UserNid { get; set; }
        public string MethodType { get; set; } = null!;
        public string? MethodName { get; set; }
        public string? SecretEncrypted { get; set; }
        public string? EndpointMasked { get; set; }
        public string? CredentialId { get; set; }
        public string? PublicKeyData { get; set; }
        public bool Verified { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public bool PrimaryMark { get; set; }
        public string MethodStatus { get; set; } = "ACTIVE";
        public DateTime? LastUsedDate { get; set; }

        public virtual IdnUser User { get; set; } = null!;
    }

    public class IdnVerificationCode
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string? UserSid { get; set; }
        public string PurposeCode { get; set; } = null!;
        public string ChannelType { get; set; } = null!;
        public string? EndpointHash { get; set; }
        public string CodeHash { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public DateTime? UsedDate { get; set; }
        public int AttemptCount { get; set; }
        public int MaxAttemptCount { get; set; } = 5;
        public string VerificationStatus { get; set; } = "PENDING";
        public string? RequestIp { get; set; }
    }

    public class IdnRecoveryCode
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong UserNid { get; set; }
        public string CodeHash { get; set; } = null!;
        public DateTime? UsedDate { get; set; }
        public string RecoveryStatus { get; set; } = "ACTIVE";

        public virtual IdnUser User { get; set; } = null!;
    }
    #endregion

    #region 06. Sessions & Token Revocation
    public class IdnSession
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong UserNid { get; set; }
        public string SessionTokenHash { get; set; } = null!;
        public string? RefreshTokenHash { get; set; }
        public string ClientType { get; set; } = null!;
        public string? ClientId { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? CountryCode { get; set; }
        public bool MfaVerified { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime? RefreshExpiryDate { get; set; }
        public DateTime LastActivityDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string? RevokeReason { get; set; }
        public string SessionStatus { get; set; } = "ACTIVE";

        public virtual IdnUser User { get; set; } = null!;
    }

    public class IdnTokenRevocation
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string TokenType { get; set; } = null!;
        public string TokenHash { get; set; } = null!;
        public string? UserSid { get; set; }
        public string? ClientId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? RevokeReason { get; set; }
        public string? RevokedUserSid { get; set; }
    }
    #endregion

    #region 07. External Identity Providers (SSO)
    public class IdnIdentityProvider
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ProviderCode { get; set; } = null!;
        public string ProviderName { get; set; } = null!;
        public string ProviderType { get; set; } = null!;
        public string? IssuerUrl { get; set; }
        public string? AuthorizationUrl { get; set; }
        public string? TokenUrl { get; set; }
        public string? UserInfoUrl { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecretEncrypted { get; set; }
        public string? Scopes { get; set; }
        public string? AttributeMapping { get; set; }
        public bool AutoCreateUser { get; set; }
        public string ProviderStatus { get; set; } = "ACTIVE";
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual ICollection<IdnExternalIdentity> ExternalIdentities { get; set; } = new List<IdnExternalIdentity>();
    }

    public class IdnExternalIdentity
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong UserNid { get; set; }
        public ulong IdentityProviderNid { get; set; }
        public string ExternalSubject { get; set; } = null!;
        public string? ExternalAccount { get; set; }
        public string? ExternalEmail { get; set; }
        public string? ProfileData { get; set; }
        public DateTime LinkedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string LinkStatus { get; set; } = "ACTIVE";

        public virtual IdnUser User { get; set; } = null!;
        public virtual IdnIdentityProvider IdentityProvider { get; set; } = null!;
    }
    #endregion

    #region 08. API Clients & API Keys
    public class IdnApiClient
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ClientId { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string ClientType { get; set; } = null!;
        public string OwnerType { get; set; } = null!;
        public string? OwnerSid { get; set; }
        public string? ClientSecretHash { get; set; }
        public string? RedirectUris { get; set; }
        public string? AllowedGrantTypes { get; set; }
        public string? AllowedScopes { get; set; }
        public int TokenLifetimeSeconds { get; set; } = 3600;
        public int RefreshLifetimeSeconds { get; set; } = 2592000;
        public string? IpAllowlist { get; set; }
        public string ClientStatus { get; set; } = "ACTIVE";
        public DateTime? LastUsedDate { get; set; }
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual ICollection<IdnApiKey> ApiKeys { get; set; } = new List<IdnApiKey>();
    }

    public class IdnApiKey
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public ulong ApiClientNid { get; set; }
        public string KeyName { get; set; } = null!;
        public string KeyPrefix { get; set; } = null!;
        public string KeyHash { get; set; } = null!;
        public string? AllowedScopes { get; set; }
        public string? IpAllowlist { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string? RevokeReason { get; set; }
        public string KeyStatus { get; set; } = "ACTIVE";

        public virtual IdnApiClient ApiClient { get; set; } = null!;
    }
    #endregion

    #region 09. Audit & Login Events
    public class IdnLoginEvent
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string? UserSid { get; set; }
        public string? AccountSnapshot { get; set; }
        public string EventCode { get; set; } = null!;
        public string? AuthenticationMethod { get; set; }
        public string? ClientType { get; set; }
        public string? ClientId { get; set; }
        public string? SessionSid { get; set; }
        public string Result { get; set; } = null!;
        public string? FailReasonCode { get; set; }
        public string? FailReason { get; set; }
        public string? IpAddress { get; set; }
        public string? CountryCode { get; set; }
        public string? DeviceId { get; set; }
        public string? UserAgent { get; set; }
        public decimal? RiskScore { get; set; }
        public string? RiskFlags { get; set; }
    }
    #endregion

    #region 10. Groups
    public class IdnGroup
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string GroupCode { get; set; } = null!;
        public string GroupName { get; set; } = null!;
        public string GroupType { get; set; } = "CUSTOM";
        public string? ReferenceSid { get; set; }
        public string GroupStatus { get; set; } = "ACTIVE";
        public string Avalible { get; set; } = "Y";
        public string? Remark { get; set; }

        public virtual ICollection<IdnGroupMember> GroupMembers { get; set; } = new List<IdnGroupMember>();
        public virtual ICollection<IdnGroupRole> GroupRoles { get; set; } = new List<IdnGroupRole>();
    }

    public class IdnGroupMember
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong GroupNid { get; set; }
        public ulong UserNid { get; set; }
        public string MemberRole { get; set; } = "MEMBER";
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string MemberStatus { get; set; } = "ACTIVE";

        public virtual IdnGroup Group { get; set; } = null!;
        public virtual IdnUser User { get; set; } = null!;
    }

    public class IdnGroupRole
    {
        public ulong Nid { get; set; }
        public string Sid { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public ulong GroupNid { get; set; }
        public ulong RoleNid { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual IdnGroup Group { get; set; } = null!;
        public virtual IdnRole Role { get; set; } = null!;
    }
    #endregion
}