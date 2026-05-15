using Domain.Data.Entities;
using Domain.Enumerators;

namespace API.Public.DTOs;

public class UpdateCompanyConfigurationDTO
{
    public string? OpeningHour { get; set; }
    public string? ClosingHour { get; set; }
    public TeamSize? TeamSize { get; set; }
    public int? ServiceRadius { get; set; }
    public List<ServiceType>? ServiceTypes { get; set; }
    public CompanySchedule? Schedule { get; set; }

    public Company ToModel() => new()
    {
        OpeningHour = !string.IsNullOrWhiteSpace(OpeningHour) ? TimeOnly.Parse(OpeningHour) : null,
        ClosingHour = !string.IsNullOrWhiteSpace(ClosingHour) ? TimeOnly.Parse(ClosingHour) : null,
        TeamSize = TeamSize,
        ServiceRadius = ServiceRadius,
        ServiceTypes = ServiceTypes is { Count: > 0 }
            ? ServiceTypes.Aggregate((ServiceType)0, (acc, s) => acc | s)
            : null,
        Schedule = Schedule,
    };
}
