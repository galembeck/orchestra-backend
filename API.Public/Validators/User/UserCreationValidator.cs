using API.Public.DTOs;
using API.Public.Validators._Base;
using Domain.Utils;
using FluentValidation;

namespace API.Public.Validators;

public class UserCreationValidator : BaseValidator<CreateUserDTO>
{
    public UserCreationValidator()
    {
        RuleFor(m => m.Name)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .Length(3, 100).WithMessage("INVALID_LENGHT");

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .EmailAddress().WithMessage("INVALID_EMAIL");

        RuleFor(c => c.Cellphone)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .Length(10, 16).WithMessage("INVALID_LENGHT");

        RuleFor(m => m.Document)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .Must(StringUtil.IsValidCPF).WithMessage("INVALID_DOCUMENT");

        RuleFor(m => m.Password)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .Must(SecurityUtil.GetPasswordStrength).WithMessage("INVALID_PASSWORD");

        RuleFor(m => m.AcceptTerms)
            .Equal(true).WithMessage("TERMS_MUST_BE_ACCEPTED");

        RuleFor(m => m.Zipcode).NotEmpty().WithMessage("CANNOT_BE_EMPTY");
        RuleFor(m => m.Address).NotEmpty().WithMessage("CANNOT_BE_EMPTY");
        RuleFor(m => m.Number).NotEmpty().WithMessage("CANNOT_BE_EMPTY");
        RuleFor(m => m.Neighborhood).NotEmpty().WithMessage("CANNOT_BE_EMPTY");
        RuleFor(m => m.City).NotEmpty().WithMessage("CANNOT_BE_EMPTY");
        RuleFor(m => m.State).NotEmpty().Length(2).WithMessage("INVALID_LENGHT");
    }
}
