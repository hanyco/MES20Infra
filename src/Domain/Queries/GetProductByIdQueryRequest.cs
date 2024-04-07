using Domain.Dtos;

namespace Domain.Queries;

public class GetProductByIdQueryRequest
{
    public GetProductByIdQueryRequest(ProductDto? result)
    {
        this.Result = result;
    }

    public ProductDto? Result { get; }
}