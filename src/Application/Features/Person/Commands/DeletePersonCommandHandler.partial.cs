using MediatR;
using Library.Data.SqlServer;
using System.Threading.Tasks;
using Mes.HumanResourcesManagement.Dtos;

namespace Mes.HumanResourcesManagement.Commands;
internal sealed partial class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, DeletePersonCommandResult>
{
    private readonly IMediator _mediator;
    private readonly Sql _sql;
    public DeletePersonCommandHandler(IMediator mediator, Sql sql)
    {
        this._mediator = mediator;
        this._sql = sql;
    }

    public async Task<DeletePersonCommandResult> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        var dbCommand = $@"DELETE FROM [dbo].[Person]   WHERE [Id] = {request.Id}";
        await this._sql.ExecuteNonQueryAsync(dbCommand, cancellationToken: cancellationToken);
        var result = new DeletePersonCommandResult();
        return result;
    }
}