using Domain.Commands;

using Library.Data.Ado;
using Library.Results;

using MediatR;

namespace Application.Commands;

internal partial class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly AdoGenericRepository _repository;

    public UpdateProductCommandHandler(AdoGenericRepository repository)
    {
        this._repository = repository;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        await this.OnHanding(request, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return await Task.FromCanceled<Result>(cancellationToken);
        }

        var dbResult = await this._repository.UpdateAsync(request.Id, request.Product, true, cancellationToken);
        var result = Result.Succeed;

        await this.OnHanded(request, dbResult, cancellationToken);
        return cancellationToken.IsCancellationRequested
            ? await Task.FromCanceled<Result>(cancellationToken)
            : await Task.FromResult(result);
    }

    private partial Task OnHanded(UpdateProductCommand request, Result<Domain.Dtos.Product> response, CancellationToken cancellationToken);

    private partial Task OnHanding(UpdateProductCommand request, CancellationToken cancellationToken);
}