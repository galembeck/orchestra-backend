using Domain.Data.Entities;

namespace API.Public.DTOs;

public class CreateServiceDTO
{
    public string CategoryId { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool? Budgetable { get; set; }

    public Service ToModel() => new()
    {
        CategoryId = CategoryId,
        ServiceType = ServiceType,
        Price = Price,
        Budgetable = Budgetable,
    };
}
