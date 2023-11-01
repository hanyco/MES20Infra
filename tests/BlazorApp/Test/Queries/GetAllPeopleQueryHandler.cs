using System.Threading.Tasks;
using Test.HumanResources.Dtos;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Queries
{
    public sealed partial class GetAllPeopleQueryHandler
    {
        public Task<GetAllPeopleQueryResult> HandleAsync(GetAllPeopleQueryParams query)
        {
            var dbQuery = @"SELECT [Id], [FirstName], [LastName], [DateOfBirth], [Height]   FROM [Person]";
            var dbResult = this._sql.Select<GetAllPeopleResult>(dbQuery).ToList();
            var result = new GetAllPeopleQueryResult(dbResult);
            return Task.FromResult(result);
        }
    }
}