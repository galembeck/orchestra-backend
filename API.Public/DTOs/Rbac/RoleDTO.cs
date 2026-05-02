using Domain.Data.Entities;

namespace API.Public.DTOs;

public class RoleDTO
{
    public string Id { get; set; } = string.Empty;
    public string? CompanyId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystem { get; set; }

    public static RoleDTO ModelToDTO(Role r) => new()
    {
        Id = r.Id,
        CompanyId = r.CompanyId,
        Key = r.Key,
        Name = r.Name,
        Description = r.Description,
        IsSystem = r.IsSystem,
    };

    public static List<RoleDTO> ModelToDTO(IEnumerable<Role> list) =>
        list.Select(ModelToDTO).ToList();
}

public class CreateRoleDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class UpdateRoleDTO
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class SetRolePermissionsDTO
{
    public List<string> Permissions { get; set; } = new();
}
