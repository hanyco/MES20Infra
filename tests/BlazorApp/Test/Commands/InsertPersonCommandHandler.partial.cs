using Library.Cqrs.Models.Queries;
using Test.HumanResources.Dtos;
using Library.Cqrs.Models.Commands;
using Library.Data.SqlServer;

namespace Test.HumanResources.Commands
{
    public sealed partial class InsertPersonCommandHandler : IQueryHandler<InsertPersonCommandParams, InsertPersonCommandResult>
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
}