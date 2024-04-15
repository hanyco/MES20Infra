using Domain.Dtos;

namespace Domain.Queries;

public class GetProductByIdQueryResponse
{
    public GetProductByIdQueryResponse(Product? result)
    {
        this.Result = result;
    }

    public Product? Result { get; }
}