using FluentValidation;

namespace CafeManagement.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username không được để trống");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password không được để trống");
    }
}
