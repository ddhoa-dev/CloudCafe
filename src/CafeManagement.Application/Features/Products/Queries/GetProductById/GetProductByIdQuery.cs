using CafeManagement.Application.DTOs.Products;
using MediatR;

namespace CafeManagement.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// Query để lấy chi tiết 1 sản phẩm kèm công thức
/// </summary>
public class GetProductByIdQuery : IRequest<ProductDto>
{
    public Guid Id { get; set; }

    public GetProductByIdQuery(Guid id)
    {
        Id = id;
    }
}
