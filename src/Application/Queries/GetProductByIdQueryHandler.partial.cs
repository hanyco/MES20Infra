using Domain.Dtos;
using Domain.Queries;
using Library.Data.Ado;
using MediatR;

namespace Application.Queries;

public sealed partial class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdQueryResponse>
{
    private readonly AdoGenericRepository _repository;

    public GetProductByIdQueryHandler(AdoGenericRepository repository)
    {
        this._repository = repository;
    }

    public async Task<GetProductByIdQueryResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        await this.OnHanding(request, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return await Task.FromCanceled<GetProductByIdQueryResponse>(cancellationToken);
        }

        var dbResult = await _repository.GetByIdAsync<Product>(request.Id, cancellationToken);
        var result = new GetProductByIdQueryResponse(dbResult);

        await this.OnHanded(request, result, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return await Task.FromCanceled<GetProductByIdQueryResponse>(cancellationToken);
        }

        return await Task.FromResult(result);
    }

    private partial Task OnHanded(GetProductByIdQuery request, GetProductByIdQueryResponse response, CancellationToken cancellationToken);

    private partial Task OnHanding(GetProductByIdQuery request, CancellationToken cancellationToken);
}