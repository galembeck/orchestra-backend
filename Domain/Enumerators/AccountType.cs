using Domain.Utils;

namespace Domain.Enumerators;

public enum AccountType
{
    [EnumDescription("CLIENT")]
    CLIENT = 1,

    [EnumDescription("COMPANY")]
    COMPANY = 2,

    [EnumDescription("WORKER")]
    WORKER = 3,
}
