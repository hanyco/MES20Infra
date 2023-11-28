using System.Threading.Tasks;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Queries;
public sealed partial class GetByIdPersonQueryHandler
{
    public Task<GetByIdPersonQueryResult> HandleAsync(GetByIdPersonQuery query)
    {
        var dbQuery = $@"SELECT TOP (1) [Id], [FirstName], [LastName], [DateOfBirth], [Height]   FROM [Person]   WHERE [ID] = {query.Params.Id}";
        var dbResult = this._sql.FirstOrDefault<Test.HumanResources.Dtos.GetByIdPersonResult>(dbQuery);
        var result = new Test.HumanResources.Dtos.GetByIdPersonQueryResult(dbResult);
        return Task.FromResult(result);
    }
}