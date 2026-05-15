using Domain.Data.Entities;

namespace API.Public.DTOs;

public class PublicServiceDTO
{
    public string Id { get; set; } = string.Empty;

    public string CompanyId { get; set; } = string.Empty;
    public string CompanyFantasyName { get; set; } = string.Empty;

    public string CategoryId { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = string.Empty;

    public string ServiceType { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool? Budgetable { get; set; }

    public double Rating { get; set; }
    public int ReviewsCount { get; set; }

    public bool IsActive { get; set; }

    // Address snapshot pulled from the company on read.
    public string Zipcode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public string Neighborhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public static PublicServiceDTO ModelToDTO(Service s, ServiceCategory cat, Company co) => new()
    {
        Id = s.Id,
        CompanyId = co.Id,
        CompanyFantasyName = co.FantasyName,
        CategoryId = cat.Id,
        CategoryName = cat.Name,
        CategoryIcon = cat.Icon,
        ServiceType = s.ServiceType,
        Price = s.Price,
        Budgetable = s.Budgetable,
        Rating = s.Rating,
        ReviewsCount = s.ReviewsCount,
        IsActive = s.IsActive,
        Zipcode = co.Zipcode,
        Address = co.Address,
        Number = co.Number,
        Complement = co.Complement,
        Neighborhood = co.Neighborhood,
        City = co.City,
        State = co.State,
        CreatedAt = s.CreatedAt,
    };

    public static List<PublicServiceDTO> ModelToDTO(IEnumerable<(Service s, ServiceCategory c, Company co)> list) =>
        list.Select(t => ModelToDTO(t.s, t.c, t.co)).ToList();
}
