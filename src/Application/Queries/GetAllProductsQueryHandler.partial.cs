using Domain.Dtos;
using Domain.Queries;

using Library.Data.SqlServer;

using MediatR;

namespace Application.Queries;

public sealed partial class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, GetAllProductsQueryResponse>
{
    public GetAllProductsQueryHandler(Sql sql)
    {
    }

    public async Task<GetAllProductsQueryResponse> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        await this.OnHanding(request, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return await Task.FromCanceled<GetAllProductsQueryResponse>(cancellationToken);
        }

        var result = new GetAllProductsQueryResponse([new ProductDto() { Id = 1, Name = "Monitor", Price = 9_000_000 }]);

        await this.OnHanded(request, result, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return await Task.FromCanceled<GetAllProductsQueryResponse>(cancellationToken);
        }

        return await Task.FromResult(result);
    }

    private partial Task OnHanded(GetAllProductsQuery request, GetAllProductsQueryResponse response, CancellationToken cancellationToken);

    private partial Task OnHanding(GetAllProductsQuery request, CancellationToken cancellationToken);
}