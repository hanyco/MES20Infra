using Domain.Dtos;

namespace Domain.Queries;

public class GetProductByIdQueryRequest
{
    public GetProductByIdQueryRequest(ProductDto product)
    {
        this.Product = product;
    }

    public ProductDto? Product { get; }
}