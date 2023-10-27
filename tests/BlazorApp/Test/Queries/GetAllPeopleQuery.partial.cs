using Library.Cqrs.Models.Queries;
using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Data.SqlServer;

namespace Test.HumanResources.Queries
{
    public sealed partial class GetAllPeopleQueryHandler : IQueryHandler<GetAllPeopleParams, GetAllPeopleResult>
    {
        public ICommandProcessor _commandProcessor;
        public IQueryProcessor _queryProcessor;
        public Sql _sql;
        public GetAllPeopleQueryHandler(ICommandProcessor commandProcessor, IQueryProcessor queryProcessor, Sql sql)
        {
            (this._commandProcessor, this._queryProcessor) = (commandProcessor, queryProcessor);
            this._sql = sql;
        }
    }
}