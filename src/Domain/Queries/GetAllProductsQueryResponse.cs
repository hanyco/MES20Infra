using Domain.Dtos;

namespace Domain.Queries;

public class GetAllProductsQueryResponse
{
    public GetAllProductsQueryResponse(IEnumerable<ProductDto> products) => this.Result = products;

    public IEnumerable<ProductDto> Result { get; }
}