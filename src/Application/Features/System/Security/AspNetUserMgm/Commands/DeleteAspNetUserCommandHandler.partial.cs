using MediatR;
using Library.Data.SqlServer;
using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Commands;
internal sealed partial class DeleteAspNetUserCommandHandler : IRequestHandler<DeleteAspNetUserCommand, DeleteAspNetUserCommandResult>
{
    private readonly IMediator _mediator;
    private readonly Sql _sql;
    public DeleteAspNetUserCommandHandler(IMediator mediator, Sql sql)
    {
        this._mediator = mediator;
        this._sql = sql;
    }

    public async Task<DeleteAspNetUserCommandResult> Handle(DeleteAspNetUserCommand request, CancellationToken cancellationToken)
    {
        var dbCommand = $@"DELETE FROM [dbo].[AspNetUsers]   WHERE [Id] = {request.Id}";
        await this._sql.ExecuteNonQueryAsync(dbCommand, cancellationToken: cancellationToken);
        var result = new DeleteAspNetUserCommandResult();
        return result;
    }
}