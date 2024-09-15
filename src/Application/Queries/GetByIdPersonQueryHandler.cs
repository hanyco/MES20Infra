using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Queries;
public sealed partial class GetByIdPersonQueryHandler
{
    public Task<GetByIdPersonQueryResult> Handle(GetByIdPersonQuery query)
    {
        var dbQuery = $@"Library.Data.SqlServer.SqlStatementBuilder+SelectStatement";
        var dbResult = this._sql.FirstOrDefault<HumanResources.Dtos.GetByIdPersonResult>(dbQuery);
        var result = new HumanResources.Dtos.GetByIdPersonQueryResult(dbResult);
        return Task.FromResult(result);
    }
}