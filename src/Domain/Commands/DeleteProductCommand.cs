using MediatR;

namespace Domain.Commands;

public sealed class DeleteProductCommand : IRequest<DeleteProductCommandResponse>
{
    public DeleteProductCommand(long id) => this.Id = id;

    public long Id { get; }
}