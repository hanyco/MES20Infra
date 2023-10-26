using Dtos.HumanResources.Test;

using Queries.HumanResources.Test;

namespace CodeGen.UnitTests.Module.Queries;

public sealed partial class GetAllPeopleQueryHandler
{
    public Task<GetAllPeopleQueryResult> HandleAsync(GetAllPeopleQueryParameter query)
    {
        var dbQuery = @"SELECT [Id], [Name], [Age]
    FROM [Person]";
        var dbResult = this._sql.Select<GetAllPeopleResult>(dbQuery).ToList();
        var result = new GetAllPeopleQueryResult(dbResult);
        return Task.FromResult(result);
    }
}