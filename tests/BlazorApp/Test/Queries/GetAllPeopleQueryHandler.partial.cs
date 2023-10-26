using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Data.SqlServer;

namespace CodeGen.UnitTests.Module.Queries
{
    using Dtos.HumanResources.Test;
    using global::Queries.HumanResources.Test;

    using Library.Cqrs.Models.Queries;

    public sealed partial class GetAllPeopleQueryHandler : IQueryHandler<GetAllPeopleQueryParameter, GetAllPeopleQueryResult>
    {
        private ICommandProcessor _commandProcessor;
        private IQueryProcessor _queryProcessor;
        private Sql _sql;
        public GetAllPeopleQueryHandler(ICommandProcessor commandProcessor, IQueryProcessor queryProcessor, Sql sql)
        {
            this._commandProcessor = commandProcessor;
            this._queryProcessor = queryProcessor;
            this._sql = sql;
        }
    }
}
