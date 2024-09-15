using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Queries;
public sealed partial class GetAllPeopleQueryHandler
{
    public Task<GetAllPeopleQueryResult> Handle(GetAllPeopleQuery query)
    {
        var dbQuery = $@"Library.Data.SqlServer.SqlStatementBuilder+SelectStatement";
        var dbResult = this._sql.FirstOrDefault<HumanResources.Dtos.GetAllPeopleResult>(dbQuery);
        var result = new HumanResources.Dtos.GetAllPeopleQueryResult(dbResult);
        return Task.FromResult(result);
    }
}