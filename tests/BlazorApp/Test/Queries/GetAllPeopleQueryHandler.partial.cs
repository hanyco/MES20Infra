using Library.Cqrs.Models.Queries;
using HumanResources.Dtos;
using Library.Cqrs.Models.Commands;
using Library.Data.SqlServer;

namespace HumanResources.Queries;
public sealed partial class GetAllPeopleQueryHandler : IQueryHandler<GetAllPeopleQuery, GetAllPeopleQueryResult>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly Sql _sql;
    public GetAllPeopleQueryHandler(ICommandProcessor commandProcessor, IQueryProcessor queryProcessor, Sql sql)
    {
        (this._commandProcessor, this._queryProcessor) = (commandProcessor, queryProcessor);
        this._sql = sql;
    }
}