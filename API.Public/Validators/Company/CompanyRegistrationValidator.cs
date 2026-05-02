using API.Public.DTOs;
using API.Public.Validators._Base;
using Domain.Utils;
using FluentValidation;

namespace API.Public.Validators;

public class CompanyRegistrationValidator : BaseValidator<RegisterCompanyDTO>
{
    public CompanyRegistrationValidator()
    {
        // Owner
        RuleFor(m => m.OwnerName).NotEmpty().Length(3, 100).WithMessage("INVALID_LENGHT");
        RuleFor(m => m.OwnerEmail).NotEmpty().EmailAddress().WithMessage("INVALID_EMAIL");
        RuleFor(m => m.OwnerCellphone).NotEmpty().Length(10, 16);
        RuleFor(m => m.OwnerPassword).NotEmpty().Must(SecurityUtil.GetPasswordStrength).WithMessage("INVALID_PASSWORD");
        RuleFor(m => m.AcceptTerms).Equal(true).WithMessage("TERMS_MUST_BE_ACCEPTED");
        RuleFor(m => m.OwnerZipcode).NotEmpty();
        RuleFor(m => m.OwnerAddress).NotEmpty();
        RuleFor(m => m.OwnerNumber).NotEmpty();
        RuleFor(m => m.OwnerNeighborhood).NotEmpty();
        RuleFor(m => m.OwnerCity).NotEmpty();
        RuleFor(m => m.OwnerState).NotEmpty().Length(2);

        // Company
        RuleFor(m => m.Segment).IsInEnum().WithMessage("INVALID_PARAMETER");
        RuleFor(m => m.Cnpj).NotEmpty().Must(StringUtil.IsValidCNPJ).WithMessage("INVALID_DOCUMENT");
        RuleFor(m => m.SocialReason).NotEmpty();
        RuleFor(m => m.FantasyName).NotEmpty();
        RuleFor(m => m.Zipcode).NotEmpty();
        RuleFor(m => m.Address).NotEmpty();
        RuleFor(m => m.Number).NotEmpty();
        RuleFor(m => m.Neighborhood).NotEmpty();
        RuleFor(m => m.City).NotEmpty();
        RuleFor(m => m.State).NotEmpty().Length(2);

        // Documents
        RuleFor(m => m.CnpjDocument).NotNull().WithMessage("MISSING_REQUIRED_DOCUMENTS");
        RuleFor(m => m.AddressProof).NotNull().WithMessage("MISSING_REQUIRED_DOCUMENTS");
        RuleFor(m => m.OwnerIdentity).NotNull().WithMessage("MISSING_REQUIRED_DOCUMENTS");
        RuleFor(m => m.OperatingLicense).NotNull().WithMessage("MISSING_REQUIRED_DOCUMENTS");
    }
}
