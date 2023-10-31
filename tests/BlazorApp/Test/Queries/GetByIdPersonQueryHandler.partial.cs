using Library.Cqrs.Models.Queries;
using Test.HumanResources.Dtos;
using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Data.SqlServer;

namespace Test.HumanResources.Queries
{
    public sealed partial class GetByIdPersonQueryHandler : IQueryHandler<GetByIdPersonQueryParams, GetByIdPersonQueryResult>
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly Sql _sql;
        public GetByIdPersonQueryHandler(ICommandProcessor commandProcessor, IQueryProcessor queryProcessor, Sql sql)
        {
            (this._commandProcessor, this._queryProcessor) = (commandProcessor, queryProcessor);
            this._sql = sql;
        }
    }
}