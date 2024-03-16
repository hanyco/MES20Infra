using Domain.Dtos;

namespace Domain.Queries;

public class GetAllProductsQueryResponse
{
    public GetAllProductsQueryResponse(IEnumerable<ProductDto> products) => this.Products = products;

    public IEnumerable<ProductDto> Products { get; }
}