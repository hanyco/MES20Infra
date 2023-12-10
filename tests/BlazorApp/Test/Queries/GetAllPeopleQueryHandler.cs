using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Queries;
public sealed partial class GetAllPeopleQueryHandler
{
    public Task<GetAllPeopleQueryResult> HandleAsync(GetAllPeopleQuery query)
    {
        var dbQuery = $@"SELECT [Id], [FirstName], [LastName], [DateOfBirth], [Height]   FROM [Person]";
        var dbResult = this._sql.Select<HumanResources.Dtos.GetAllPeopleResult>(dbQuery).ToList();
        var result = new HumanResources.Dtos.GetAllPeopleQueryResult(dbResult);
        return Task.FromResult(result);
    }
}