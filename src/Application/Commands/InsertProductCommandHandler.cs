using Domain.Commands;
using Domain.Dtos;

using Library.Results;

namespace Application.Commands;

internal partial class InsertProductCommandHandler
{
    private partial Task OnHanded(InsertProductCommand request, Result<Product> response, CancellationToken cancellationToken)
        => Task.CompletedTask;

    private partial Task OnHanding(InsertProductCommand request, CancellationToken cancellationToken)
        => Task.CompletedTask;
}