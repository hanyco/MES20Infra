using Library.Cqrs.Models.Queries;
using Library.Cqrs.Models.Commands;
using Library.Data.SqlServer;
using HumanResources.Dtos;

namespace HumanResources.Queries;
internal sealed partial class GetAllPeopleQueryHandler : IQueryHandler<GetAllPeopleQuery, GetAllPeopleQueryResult>
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