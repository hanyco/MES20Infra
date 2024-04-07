using Domain.Dtos;
using Domain.Queries;

using Library.Data.Ado;
using Library.Data.SqlServer;

using MediatR;

namespace Application.Queries;

public sealed partial class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, GetAllProductsQueryResponse>
{
    private readonly AdoGenericRepository _repository;

    public GetAllProductsQueryHandler(AdoGenericRepository repository)
    {
        this._repository = repository;
    }

    public async Task<GetAllProductsQueryResponse> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        await this.OnHanding(request, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return await Task.FromCanceled<GetAllProductsQueryResponse>(cancellationToken);
        }

        var dbResult =await _repository.GetAllAsync<ProductDto>(cancellationToken);
        var result = new GetAllProductsQueryResponse(dbResult);

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