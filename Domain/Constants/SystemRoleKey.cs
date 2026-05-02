namespace Domain.Constants;

// Default roles seeded for every new company. Owners and admins map to the
// "Administrator" column of the permission matrix; member/billing follow the
// matrix as written. Conditional cells (warning icons) are enforced at the
// service layer — e.g. MEMBER has project:update but the service must verify
// the project belongs to that user before allowing the action.
public static class SystemRoleKey
{
    public const string Owner          = "OWNER";
    public const string Administrator  = "ADMINISTRATOR";
    public const string Member         = "MEMBER";
    public const string Billing        = "BILLING";

    public static readonly string[] All = { Owner, Administrator, Member, Billing };

    public static IReadOnlyDictionary<string, string[]> DefaultPermissions { get; } =
        new Dictionary<string, string[]>
        {
            // Owner: every company-scoped permission, including transfer ownership.
            [Owner] = new[]
            {
                PermissionKey.CompanyRead, PermissionKey.CompanyUpdate, PermissionKey.CompanyDelete,
                PermissionKey.CompanyTransferOwnership,
                PermissionKey.MemberList, PermissionKey.MemberInvite, PermissionKey.MemberRevokeInvite,
                PermissionKey.MemberUpdateRole, PermissionKey.MemberDelete,
                PermissionKey.ProjectList, PermissionKey.ProjectCreate, PermissionKey.ProjectUpdate, PermissionKey.ProjectDelete,
                PermissionKey.BillingRead, PermissionKey.BillingExport,
                PermissionKey.RoleRead, PermissionKey.RoleCreate, PermissionKey.RoleUpdate, PermissionKey.RoleDelete,
            },

            // Administrator: matches the Administrator column of the matrix.
            // No transfer-ownership (owner-only).
            [Administrator] = new[]
            {
                PermissionKey.CompanyRead, PermissionKey.CompanyUpdate, PermissionKey.CompanyDelete,
                PermissionKey.MemberList, PermissionKey.MemberInvite, PermissionKey.MemberRevokeInvite,
                PermissionKey.MemberUpdateRole, PermissionKey.MemberDelete,
                PermissionKey.ProjectList, PermissionKey.ProjectCreate, PermissionKey.ProjectUpdate, PermissionKey.ProjectDelete,
                PermissionKey.BillingRead, PermissionKey.BillingExport,
                PermissionKey.RoleRead, PermissionKey.RoleCreate, PermissionKey.RoleUpdate, PermissionKey.RoleDelete,
            },

            // Member: list members & projects, create projects, update/delete OWN
            // projects (service layer enforces ownership), and remove SELF.
            [Member] = new[]
            {
                PermissionKey.CompanyRead,
                PermissionKey.MemberList,
                PermissionKey.MemberDelete,
                PermissionKey.ProjectList, PermissionKey.ProjectCreate,
                PermissionKey.ProjectUpdate, PermissionKey.ProjectDelete,
            },

            // Billing: read-only on members/projects, full access to billing.
            [Billing] = new[]
            {
                PermissionKey.CompanyRead,
                PermissionKey.MemberList,
                PermissionKey.ProjectList,
                PermissionKey.BillingRead, PermissionKey.BillingExport,
            },
        };
}
