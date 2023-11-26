using Library.Cqrs.Models.Queries;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed class GetByIdPersonQuery : IQuery<GetByIdPersonQueryResult>
{
    public GetByIdPersonQuery(GetByIdPerson @params)
    {
        this.Params = @params;
    }

    public GetByIdPerson Params { get; set; }
}