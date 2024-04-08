using Library.Results;

using MediatR;

namespace Domain.Queries;

public sealed class GetProductByIdQuery : IRequest<GetProductByIdQueryResponse>
{
    public GetProductByIdQuery(long id)
    {
        this.Id = id;
    }

    public long Id { get; }
}