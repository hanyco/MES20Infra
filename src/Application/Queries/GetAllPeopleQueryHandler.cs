using System.Threading.Tasks;
using HumanResource.Dtos;

namespace HumanResource.Queries;
public sealed partial class GetAllPeopleQueryHandler
{
    public Task<GetAllPeopleQueryResult> Handle(GetAllPeopleQuery query)
    {
        var dbQuery = $@"Library.Data.SqlServer.SqlStatementBuilder+SelectStatement";
        var dbResult = this._sql.FirstOrDefault<HumanResource.Dtos.GetAllPeopleQueryResult>(dbQuery);
        var result = new HumanResource.Dtos.GetAllPeopleQueryResult(dbResult);
        return Task.FromResult(result);
    }
}