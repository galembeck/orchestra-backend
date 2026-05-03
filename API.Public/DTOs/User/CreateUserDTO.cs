using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Utils;
using System.Globalization;

namespace API.Public.DTOs;

public class CreateUserDTO
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cellphone { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool AcceptTerms { get; set; }

    public string Zipcode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public string Neighborhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public bool? ReceiveWhatsappOffers { get; set; }
    public bool? ReceiveEmailOffers { get; set; }

    public static User DTOToModel(CreateUserDTO o) => new()
    {
        Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(o.Name.ToLower().Trim()),
        Email = o.Email.ToLower().Trim(),
        Cellphone = o.Cellphone,
        Document = StringUtil.Slugify(o.Document.Trim()),
        Password = o.Password.Trim(),
        ProfileType = ProfileType.CLIENT,
        AccountType = AccountType.CLIENT,
        AcceptedTerms = o.AcceptTerms,
        AcceptedTermsAt = o.AcceptTerms ? DateTimeOffset.UtcNow : null,
        Zipcode = o.Zipcode,
        Address = o.Address,
        Number = o.Number,
        Complement = o.Complement,
        Neighborhood = o.Neighborhood,
        City = o.City,
        State = o.State,
        ReceiveWhatsappOffers = o.ReceiveWhatsappOffers,
        ReceiveEmailOffers = o.ReceiveEmailOffers,
    };
}
