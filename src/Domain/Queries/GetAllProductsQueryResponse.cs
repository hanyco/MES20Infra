using Domain.Dtos;

namespace Domain.Queries;

public class GetAllProductsQueryResponse
{
    public GetAllProductsQueryResponse(IEnumerable<Product> products)
    {
        this.Result = products;
    }

    public IEnumerable<Product> Result { get; }
}