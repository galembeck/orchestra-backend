using API.Public.DTOs._Base;
using Domain.Data.Entities;
using Domain.Enumerators;

namespace API.Public.DTOs;

public class PublicUserDTO : PublicBaseDTO<User>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cellphone { get; set; }
    public string Document { get; set; }
    public ProfileType? ProfileType { get; set; }
    public AccountType? AccountType { get; set; }



    public bool? ReceiveWhatsappOffers { get; set; }
    public bool? ReceiveEmailOffers { get; set; }



    public string? Zipcode { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string? Number { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public string? Neighborhood { get; set; } = string.Empty;
    public string? City { get; set; } = string.Empty;
    public string? State { get; set; } = string.Empty;



    public string? AvatarUrl { get; set; }



    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastAccessAt { get; set; }



    public WorkerContextDTO? Worker { get; set; }
    public CompanyContextDTO? Company { get; set; }



    public PublicUserDTO(User o) : base(o)
    {
        if (o == null) return;

        Id = o.Id;
        Name = o.Name;
        Email = o.Email;
        Cellphone = o.Cellphone;
        Document = o.Document;
        ProfileType = o.ProfileType;
        AccountType = o.AccountType;
        ReceiveWhatsappOffers = o.ReceiveWhatsappOffers;
        ReceiveEmailOffers = o.ReceiveEmailOffers;
        AvatarUrl = o.AvatarUrl;

        if (o.AccountType == Domain.Enumerators.AccountType.CLIENT)
        {
            Zipcode = o.Zipcode ?? string.Empty;
            Address = o.Address ?? string.Empty;
            Number = o.Number ?? string.Empty;
            Complement = o.Complement;
            Neighborhood = o.Neighborhood ?? string.Empty;
            City = o.City ?? string.Empty;
            State = o.State ?? string.Empty;
        }
        CreatedAt = o.CreatedAt;
        LastAccessAt = o.LastAccessAt;
    }

    public static PublicUserDTO ModelToDTO(User o) => o == null ? null : new PublicUserDTO(o);

    public static List<PublicUserDTO> ModelToDTO(IEnumerable<User> users) => users.Select(user => new PublicUserDTO(user)).ToList();

    public static User DTOToModel(PublicUserDTO o)
    {
        if (o == null) return null;

        var model = new User()
        {
            Name = o.Name,
            Email = o.Email,
            Cellphone = o.Cellphone,
            Document = o.Document,
            ProfileType = o.ProfileType,
            AccountType = o.AccountType ?? Domain.Enumerators.AccountType.CLIENT,
            ReceiveWhatsappOffers = o.ReceiveWhatsappOffers,
            ReceiveEmailOffers = o.ReceiveEmailOffers,
            AvatarUrl = o.AvatarUrl,
            Zipcode = o.Zipcode,
            Address = o.Address,
            Number = o.Number,
            Complement = o.Complement,
            Neighborhood = o.Neighborhood,
            City = o.City,
            State = o.State,
            CreatedAt = o.CreatedAt,
            LastAccessAt = o.LastAccessAt,
        };

        return o.InitializeInstance(model);
    }
}