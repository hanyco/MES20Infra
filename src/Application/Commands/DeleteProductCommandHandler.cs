using Domain.Commands;

using Library.Results;

namespace Application.Commands;
internal partial class DeleteProductCommandHandler
{
    private partial Task OnHanded(DeleteProductCommand request, Result response, CancellationToken cancellationToken)
        => Task.CompletedTask;
    private partial Task OnHanding(DeleteProductCommand request, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
