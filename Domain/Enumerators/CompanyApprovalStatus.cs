using Domain.Utils;

namespace Domain.Enumerators;

public enum CompanyApprovalStatus
{
    [EnumDescription("PENDING")]
    PENDING = 1,

    [EnumDescription("APPROVED")]
    APPROVED = 2,

    [EnumDescription("REJECTED")]
    REJECTED = 3,
}
