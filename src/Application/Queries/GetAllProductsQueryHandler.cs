using Domain.Queries;

using MediatR;

namespace Application.Queries;

public sealed class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery>
{
    public Task Handle(GetAllProductsQuery request, CancellationToken cancellationToken) => throw new NotImplementedException();
}