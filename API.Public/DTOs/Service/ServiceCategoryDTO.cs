using Domain.Data.Entities;

namespace API.Public.DTOs;

public class ServiceCategoryDTO
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public static ServiceCategoryDTO ModelToDTO(ServiceCategory c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        Icon = c.Icon,
    };

    public static List<ServiceCategoryDTO> ModelToDTO(IEnumerable<ServiceCategory> list) =>
        list.Select(ModelToDTO).ToList();
}
