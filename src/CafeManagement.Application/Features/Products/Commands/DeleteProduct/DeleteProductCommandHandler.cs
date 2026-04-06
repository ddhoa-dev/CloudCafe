using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// Handler xử lý xóa sản phẩm (Soft Delete)
/// </summary>
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Soft Delete: Đánh dấu IsDeleted = true thay vì xóa thật
    /// Lý do: Giữ lại dữ liệu lịch sử đơn hàng
    /// </summary>
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException("Product", request.Id);
        }

        // Soft Delete: Đánh dấu IsDeleted thay vì xóa thật
        product.IsDeleted = true;
        product.IsAvailable = false;  // Đồng thời set không available

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
