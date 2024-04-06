using Library.Results;

using MediatR;

namespace Domain.Commands;

public sealed class DeleteProductCommand : IRequest<Result>
{
    public DeleteProductCommand(long id)
    {
        this.Id = id;
    }

    public long Id { get; }
}