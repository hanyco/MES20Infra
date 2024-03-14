using Domain.Dtos;

namespace Domain.Queries;

public class GetAllProductsQueryRequest
{
    public GetAllProductsQueryRequest(IEnumerable<ProductDto> products)
    {
        this.Products = products;
    }

    public IEnumerable<ProductDto> Products { get; }
}