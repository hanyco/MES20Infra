using System.Threading.Tasks;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Queries;
public sealed partial class GetAllPeopleQueryHandler
{
    public Task<GetAllPeopleQueryResult> HandleAsync(GetAllPeopleQuery query)
    {
        var dbQuery = $@"SELECT [Id], [FirstName], [LastName], [DateOfBirth], [Height]   FROM [Person]   WHERE ID = %Id%";
        var dbResult = this._sql.Select<GetAllPeopleResult>(dbQuery).ToList();
        var result = new GetAllPeopleQueryResult(dbResult);
        return Task.FromResult(result);
    }
}