using System.Threading.Tasks;
using HumanResource.Dtos;

namespace HumanResource.Queries;
public sealed partial class GetByIdPersonQueryHandler
{
    public Task<GetByIdPersonQueryResult> Handle(GetByIdPersonQuery query)
    {
        var dbQuery = $@"Library.Data.SqlServer.SqlStatementBuilder+SelectStatement";
        var dbResult = this._sql.FirstOrDefault<HumanResource.Dtos.GetByIdPersonResult>(dbQuery);
        var result = new HumanResource.Dtos.GetByIdPersonQueryResult(dbResult);
        return Task.FromResult(result);
    }
}