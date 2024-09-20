using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Data.SqlServer;
using HumanResources.Dtos;

namespace HumanResources.Commands;
internal sealed partial class InsertPersonCommandHandler : ICommandHandler<InsertPersonCommand, InsertPersonCommandResult>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly Sql _sql;
    public InsertPersonCommandHandler(ICommandProcessor commandProcessor, IQueryProcessor queryProcessor, Sql sql)
    {
        (this._commandProcessor, this._queryProcessor) = (commandProcessor, queryProcessor);
        this._sql = sql;
    }
}