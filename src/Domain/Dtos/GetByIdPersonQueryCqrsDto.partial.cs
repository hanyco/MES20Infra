using Library.Cqrs.Models.Queries;
using HumanResource.Dtos;

namespace HumanResource.Dtos;
public sealed class GetByIdPersonQuery : IQuery<GetByIdPersonQueryResult>
{
    public GetByIdPersonQuery(GetByIdPerson @params)
    {
        this.Params = @params;
    }

    public GetByIdPerson Params { get; set; }
}