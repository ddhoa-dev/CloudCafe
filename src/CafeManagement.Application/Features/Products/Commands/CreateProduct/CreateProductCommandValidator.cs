using FluentValidation;

namespace CafeManagement.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
            .MaximumLength(200).WithMessage("Tên sản phẩm không được quá 200 ký tự");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Giá phải lớn hơn 0");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Loại sản phẩm không hợp lệ");

        RuleForEach(x => x.Ingredients).ChildRules(ingredient =>
        {
            ingredient.RuleFor(x => x.IngredientId)
                .NotEmpty().WithMessage("IngredientId không được để trống");

            ingredient.RuleFor(x => x.QuantityRequired)
                .GreaterThan(0).WithMessage("Số lượng nguyên liệu phải > 0");
        });
    }
}
