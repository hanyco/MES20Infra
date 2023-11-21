using Library.Cqrs.Models.Queries;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Dtos;
public sealed class GetByIdPersonQuery : IQuery<GetByIdPersonQueryResult>
{
    public GetByIdPersonQuery(GetByIdPerson @params)
    {
        this.Params = @params;
    }

    public GetByIdPerson Params { get; set; }
}