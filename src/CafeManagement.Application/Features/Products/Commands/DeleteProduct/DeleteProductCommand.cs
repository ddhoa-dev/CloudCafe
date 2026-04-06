using MediatR;

namespace CafeManagement.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// Command để xóa sản phẩm (Soft Delete)
/// </summary>
public class DeleteProductCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }
}
