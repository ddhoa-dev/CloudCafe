using FluentValidation;

namespace CafeManagement.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username không được để trống")
            .MinimumLength(3).WithMessage("Username phải có ít nhất 3 ký tự")
            .MaximumLength(50).WithMessage("Username không được quá 50 ký tự")
            .Matches("^[a-zA-Z0-9_]*$").WithMessage("Username chỉ được chứa chữ cái, số và dấu gạch dưới");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không hợp lệ");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password không được để trống")
            .MinimumLength(6).WithMessage("Password phải có ít nhất 6 ký tự")
            .Matches("[A-Z]").WithMessage("Password phải có ít nhất 1 chữ hoa")
            .Matches("[a-z]").WithMessage("Password phải có ít nhất 1 chữ thường")
            .Matches("[0-9]").WithMessage("Password phải có ít nhất 1 số");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .MaximumLength(200).WithMessage("Họ tên không được quá 200 ký tự");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^(0|\+84)[0-9]{9,10}$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Số điện thoại không hợp lệ");
    }
}
