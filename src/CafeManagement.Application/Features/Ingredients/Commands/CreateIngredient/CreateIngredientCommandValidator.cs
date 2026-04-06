using FluentValidation;

namespace CafeManagement.Application.Features.Ingredients.Commands.CreateIngredient;

public class CreateIngredientCommandValidator : AbstractValidator<CreateIngredientCommand>
{
    public CreateIngredientCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên nguyên liệu không được để trống")
            .MaximumLength(200).WithMessage("Tên nguyên liệu không được quá 200 ký tự");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("Đơn vị tính không được để trống")
            .MaximumLength(50).WithMessage("Đơn vị tính không được quá 50 ký tự");

        RuleFor(x => x.QuantityInStock)
            .GreaterThanOrEqualTo(0).WithMessage("Số lượng tồn kho phải >= 0");

        RuleFor(x => x.MinimumStockLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Ngưỡng tồn kho tối thiểu phải >= 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Giá đơn vị phải > 0");
    }
}
