using Domain.Commands;

using Library.Data.Ado;
using Library.Results;

using MediatR;

namespace Application.Commands;

internal partial class InsertProductCommandHandler : IRequestHandler<InsertProductCommand, Result<long?>>
{
    private readonly AdoGenericRepository _repository;

    public InsertProductCommandHandler(AdoGenericRepository repository)
    {
        this._repository = repository;
    }

    public async Task<Result<long?>> Handle(InsertProductCommand request, CancellationToken cancellationToken)
    {
        await this.OnHanding(request, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return await Task.FromCanceled<Result<long?>>(cancellationToken);
        }

        var dbResult = await this._repository.InsertAsync(request.Product, true, cancellationToken);
        var result = Result.Success<long?>(dbResult.Value.Id);

        await this.OnHanded(request, dbResult, cancellationToken);
        return cancellationToken.IsCancellationRequested
            ? await Task.FromCanceled<Result<long?>>(cancellationToken)
            : await Task.FromResult(result);
    }

    private partial Task OnHanded(InsertProductCommand request, Result<Domain.Dtos.Product> response, CancellationToken cancellationToken);

    private partial Task OnHanding(InsertProductCommand request, CancellationToken cancellationToken);
}