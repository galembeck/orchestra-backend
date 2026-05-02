namespace Domain.Constants;

public static class PermissionKey
{
    // Company / organization
    public const string CompanyRead              = "company:read";
    public const string CompanyUpdate            = "company:update";
    public const string CompanyDelete            = "company:delete";
    public const string CompanyTransferOwnership = "company:transfer-ownership";

    // Platform-only (PLATFORM_DEVELOPER profile)
    public const string CompanyApprove           = "company:approve";
    public const string CompanyReject            = "company:reject";

    // Members
    public const string MemberList               = "member:list";
    public const string MemberInvite             = "member:invite";
    public const string MemberRevokeInvite       = "member:revoke-invite";
    public const string MemberUpdateRole         = "member:update-role";
    public const string MemberDelete             = "member:delete";

    // Projects (services that the company offers)
    public const string ProjectList              = "project:list";
    public const string ProjectCreate            = "project:create";
    public const string ProjectUpdate            = "project:update";
    public const string ProjectDelete            = "project:delete";

    // Billing
    public const string BillingRead              = "billing:read";
    public const string BillingExport            = "billing:export";

    // Roles
    public const string RoleRead                 = "role:read";
    public const string RoleCreate               = "role:create";
    public const string RoleUpdate               = "role:update";
    public const string RoleDelete               = "role:delete";

    public static readonly string[] All =
    {
        CompanyRead, CompanyUpdate, CompanyDelete, CompanyTransferOwnership,
        CompanyApprove, CompanyReject,
        MemberList, MemberInvite, MemberRevokeInvite, MemberUpdateRole, MemberDelete,
        ProjectList, ProjectCreate, ProjectUpdate, ProjectDelete,
        BillingRead, BillingExport,
        RoleRead, RoleCreate, RoleUpdate, RoleDelete,
    };

    public static readonly string[] PlatformOnly = { CompanyApprove, CompanyReject };
}
