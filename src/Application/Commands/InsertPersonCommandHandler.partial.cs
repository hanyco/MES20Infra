using Library.Cqrs.Models.Commands;
using MediatR;
using Library.Data.SqlServer;
using HumanResources.Dtos;

namespace HumanResources.Commands;
internal sealed partial class InsertPersonCommandHandler : ICommandHandler<InsertPersonCommand, InsertPersonCommandResult>
{
    private readonly IMediator _mediator;
    private readonly Sql _sql;
    public InsertPersonCommandHandler(IMediator mediator, Sql sql)
    {
        this._mediator )  =  mediator ; 
        this._sql = sql;
    }
}