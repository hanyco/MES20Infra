using MediatR;

namespace Domain.Queries;

public sealed class GetProductByIdQuery : IRequest<GetProductByIdQueryRequest>
{
    public GetProductByIdQuery(long id) => this.Id = id;

    public long Id { get; }
}