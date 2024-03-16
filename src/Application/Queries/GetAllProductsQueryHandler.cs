using Domain.Queries;

namespace Application.Queries;

public partial class GetAllProductsQueryHandler
{
    private partial Task OnHanded(GetAllProductsQuery request, GetAllProductsQueryResponse response, CancellationToken cancellationToken)
        => Task.CompletedTask;

    private partial Task OnHanding(GetAllProductsQuery request, CancellationToken cancellationToken)
        => Task.CompletedTask;
}