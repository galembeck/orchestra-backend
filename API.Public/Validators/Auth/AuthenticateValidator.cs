using API.Public.DTOs;
using API.Public.Validators._Base;
using FluentValidation;

namespace API.Public.Validators;

public class AuthenticateValidator : BaseValidator<AuthenticateDTO>
{
    public AuthenticateValidator()
    {
        RuleFor(m => m.Identifier)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .MinimumLength(3).WithMessage("INVALID_LENGHT");

        RuleFor(m => m.Password)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .Length(6, 30).WithMessage("INVALID_LENGHT");
    }
}
