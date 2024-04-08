using Domain.Queries;

namespace Application.Queries;

public partial class GetProductByIdQueryHandler
{
    private partial Task OnHanded(GetProductByIdQuery request, GetProductByIdQueryResponse response, CancellationToken cancellationToken)
        => Task.CompletedTask;

    private partial Task OnHanding(GetProductByIdQuery request, CancellationToken cancellationToken)
        => Task.CompletedTask;
}