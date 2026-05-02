using Domain.Data.Entities;

namespace API.Public.DTOs;

public class UpdateCompanyDTO
{
    public string? SocialReason { get; set; }
    public string? FantasyName { get; set; }

    public string? Zipcode { get; set; }
    public string? Address { get; set; }
    public string? Number { get; set; }
    public string? Complement { get; set; }
    public string? Neighborhood { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }

    public Company ToModel() => new()
    {
        SocialReason = SocialReason ?? string.Empty,
        FantasyName = FantasyName ?? string.Empty,
        Zipcode = Zipcode ?? string.Empty,
        Address = Address ?? string.Empty,
        Number = Number ?? string.Empty,
        Complement = Complement,
        Neighborhood = Neighborhood ?? string.Empty,
        City = City ?? string.Empty,
        State = State ?? string.Empty,
    };
}
