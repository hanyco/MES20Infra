using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Commands;
using Domain.Queries;

using Library.Data.Ado;
using Library.Results;
using MediatR;

namespace Application.Commands;
internal partial class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly AdoGenericRepository _repository;
    private readonly IMediator _mediator;

    public DeleteProductCommandHandler(AdoGenericRepository repository, IMediator mediator)
    {
        this._repository = repository;
        this._mediator = mediator;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        await this.OnHanding(request, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return await Task.FromCanceled<Result>(cancellationToken);
        }
        var productResult = await _mediator.Send(new GetProductByIdQuery(request.Id), cancellationToken);
        
        var dbResult = await this._repository.DeleteAsync(productResult.Result, true, cancellationToken);
        var result = Result.Success();

        await this.OnHanded(request, dbResult, cancellationToken);
        return cancellationToken.IsCancellationRequested
            ? await Task.FromCanceled<Result>(cancellationToken)
            : await Task.FromResult(result);
    }

    private partial Task OnHanded(DeleteProductCommand request, Result response, CancellationToken cancellationToken);

    private partial Task OnHanding(DeleteProductCommand request, CancellationToken cancellationToken);
}
