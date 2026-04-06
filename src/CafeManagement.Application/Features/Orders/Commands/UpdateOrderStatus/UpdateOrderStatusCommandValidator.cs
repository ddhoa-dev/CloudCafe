using FluentValidation;

namespace CafeManagement.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("OrderId không được để trống");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Trạng thái không hợp lệ");
    }
}
