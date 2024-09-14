using Library.Cqrs.Models.Queries;
using HumanResource.Dtos;

namespace HumanResource.Dtos;
public sealed partial class GetAllPeopleQuery : IQuery<GetAllPeopleQueryResult>
{
    public GetAllPeopleQuery()
    {
    }

    public GetAllPeopleQuery Params { get; set; }
}