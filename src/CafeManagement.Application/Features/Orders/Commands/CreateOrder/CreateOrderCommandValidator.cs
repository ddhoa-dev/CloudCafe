using FluentValidation;

namespace CafeManagement.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Đơn hàng phải có ít nhất 1 sản phẩm");

        RuleFor(x => x.CustomerPhone)
            .Matches(@"^(0|\+84)[0-9]{9,10}$")
            .When(x => !string.IsNullOrEmpty(x.CustomerPhone))
            .WithMessage("Số điện thoại không hợp lệ");

        RuleFor(x => x.DiscountAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DiscountAmount.HasValue)
            .WithMessage("Giảm giá phải >= 0");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId không được để trống");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Số lượng phải > 0");
        });
    }
}
