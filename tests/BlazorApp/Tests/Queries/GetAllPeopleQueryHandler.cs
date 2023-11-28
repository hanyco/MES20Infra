using System.Threading.Tasks;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Queries;
public sealed partial class GetAllPeopleQueryHandler
{
    public Task<GetAllPeopleQueryResult> HandleAsync(GetAllPeopleQuery query)
    {
        var dbQuery = $@"SELECT [Id], [FirstName], [LastName], [DateOfBirth], [Height]   FROM [Person]";
        var dbResult = this._sql.Select<Test.HumanResources.Dtos.GetAllPeopleResult>(dbQuery).ToList();
        var result = new Test.HumanResources.Dtos.GetAllPeopleQueryResult(dbResult);
        return Task.FromResult(result);
    }
}