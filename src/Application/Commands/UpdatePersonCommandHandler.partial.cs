using Library.Cqrs.Models.Commands;
using HumanResource.Dtos;
using Library.Cqrs.Models.Queries;
using Library.Data.SqlServer;

namespace HumanResource.Commands;
public sealed partial class UpdatePersonCommandHandler : ICommandHandler<UpdatePersonCommand, UpdatePersonCommandResult>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly Sql _sql;
    public UpdatePersonCommandHandler(ICommandProcessor commandProcessor, IQueryProcessor queryProcessor, Sql sql)
    {
        (this._commandProcessor, this._queryProcessor) = (commandProcessor, queryProcessor);
        this._sql = sql;
    }
}