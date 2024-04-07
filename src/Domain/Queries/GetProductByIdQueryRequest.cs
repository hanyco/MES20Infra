using Domain.Dtos;

namespace Domain.Queries;

public class GetProductByIdQueryRequest
{
    public GetProductByIdQueryRequest(Product? result)
    {
        this.Result = result;
    }

    public Product? Result { get; }
}