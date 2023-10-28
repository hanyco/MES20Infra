using System.Threading.Tasks;
using Test.HumanResources.Dtos;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Queries
{
    public sealed partial class GetByIdPersonQueryHandler
    {
        public Task<GetByIdPersonQueryResult> HandleAsync(GetByIdPersonQueryParams query)
        {
            var dbQuery = @"SELECT [Id] FROM[Person] "; 
            var dbResult = this._sql.Select<GetByIdPersonResult>(dbQuery).ToList();
            var result = new GetByIdPersonQueryResult(dbResult);
            return Task.FromResult(result);
        }
    }
}