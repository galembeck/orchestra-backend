using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using Domain.Enumerators;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBUser")]
public class User : BaseEntity, IBaseEntity<User>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cellphone { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public ProfileType? ProfileType { get; set; }
    public AccountType AccountType { get; set; } = AccountType.CLIENT;

    public string Password { get; set; } = string.Empty;

    public bool? ReceiveWhatsappOffers { get; set; }
    public bool? ReceiveEmailOffers { get; set; }

    public bool AcceptedTerms { get; set; }
    public DateTimeOffset? AcceptedTermsAt { get; set; }

    public string? Zipcode { get; set; }
    public string? Address { get; set; }
    public string? Number { get; set; }
    public string? Complement { get; set; }
    public string? Neighborhood { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }

    public string? AvatarUrl { get; set; }
    public string? AvatarPath { get; set; }

    public DateTimeOffset? LastAccessAt { get; set; }

    public string? PasswordChangeToken { get; set; }
    public DateTimeOffset? PasswordChangeTokenExpiresAt { get; set; }

    [NotMapped]
    public string HashId { get; set; } = string.Empty;

    public User WithoutRelations(User entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new User
        {
            Name = entity.Name,
            Email = entity.Email,
            Cellphone = entity.Cellphone,
            Document = entity.Document,
            ProfileType = entity.ProfileType,
            AccountType = entity.AccountType,

            Password = entity.Password,

            ReceiveWhatsappOffers = entity.ReceiveWhatsappOffers,
            ReceiveEmailOffers = entity.ReceiveEmailOffers,

            AcceptedTerms = entity.AcceptedTerms,
            AcceptedTermsAt = entity.AcceptedTermsAt,

            Zipcode = entity.Zipcode,
            Address = entity.Address,
            Number = entity.Number,
            Complement = entity.Complement,
            Neighborhood = entity.Neighborhood,
            City = entity.City,
            State = entity.State,

            AvatarUrl = entity.AvatarUrl,
            AvatarPath = entity.AvatarPath,

            LastAccessAt = entity.LastAccessAt,

            PasswordChangeToken = entity.PasswordChangeToken,
            PasswordChangeTokenExpiresAt = entity.PasswordChangeTokenExpiresAt,
            HashId = entity.HashId,
        };

        newEntity.InitializeInstance(entity);

        return newEntity;
    }
}
