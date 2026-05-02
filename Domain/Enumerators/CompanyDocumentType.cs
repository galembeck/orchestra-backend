using Domain.Utils;

namespace Domain.Enumerators;

public enum CompanyDocumentType
{
    [EnumDescription("CNPJ_DOCUMENT")]
    CNPJ_DOCUMENT = 1,

    [EnumDescription("ADDRESS_PROOF")]
    ADDRESS_PROOF = 2,

    [EnumDescription("OWNER_IDENTITY")]
    OWNER_IDENTITY = 3,

    [EnumDescription("OPERATING_LICENSE")]
    OPERATING_LICENSE = 4,
}
