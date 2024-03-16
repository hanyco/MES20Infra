using Domain.Dtos;
using Domain.Queries;

using MediatR;

namespace Application.Queries;

public sealed class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, GetAllProductsQueryRequest>
{
    public Task<GetAllProductsQueryRequest> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var result = new GetAllProductsQueryRequest([new ProductDto() { Id = 1, Name = "Monitor", Price = 9_000_000 }]);
        return Task.FromResult(result);
    }
}