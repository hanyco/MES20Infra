using MediatR;
using Library.Data.SqlServer;
using HumanResources.Dtos;

namespace HumanResources.Commands;
internal sealed partial class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, DeletePersonCommandResult>
{
    private readonly IMediator _mediator;
    private readonly Sql _sql;
    public DeletePersonCommandHandler(IMediator mediator, Sql sql)
    {
        this._mediator =  mediator ; 
        this._sql = sql;
    }
}