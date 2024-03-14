using MediatR;

namespace Domain.Queries;

public sealed class GetAllProductsQuery : IRequest<GetAllProductsQueryRequest>
{
}