using Library.Cqrs.Models.Commands;
using HumanResources.Dtos;
using Library.Cqrs.Models.Queries;
using Library.Data.SqlServer;

namespace HumanResources.Commands;
public sealed partial class DeletePersonCommandHandler : ICommandHandler<DeletePersonCommand, DeletePersonCommandResult>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly Sql _sql;
    public DeletePersonCommandHandler(ICommandProcessor commandProcessor, IQueryProcessor queryProcessor, Sql sql)
    {
        (this._commandProcessor, this._queryProcessor) = (commandProcessor, queryProcessor);
        this._sql = sql;
    }
}