using Domain.Utils;

namespace Domain.Enumerators;

public enum Segment
{
    [EnumDescription("RESIDENTIAL")]
    RESIDENTIAL = 1,

    [EnumDescription("BUSINESS")]
    BUSINESS = 2,

    [EnumDescription("INDUSTRIAL")]
    INDUSTRIAL = 3,
}
