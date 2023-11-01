using System.Threading.Tasks;
using Test.HumanResources.Dtos;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Queries
{
    public sealed partial class GetByIdPersonQueryHandler
    {
        public Task<GetByIdPersonQueryResult> HandleAsync(GetByIdPersonQueryParams query)
        {
            var dbQuery = @"SELECT TOP (1) [Id], [FirstName], [LastName], [DateOfBirth], [Height]   FROM [Person]";
            var dbResult = this._sql.FirstOrDefault<GetByIdPersonResult>(dbQuery);
            var result = new GetByIdPersonQueryResult(dbResult);
            return Task.FromResult(result);
        }
    }
}