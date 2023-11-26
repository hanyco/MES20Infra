using Library.Cqrs.Models.Queries;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed class GetAllPeopleQuery : IQuery<GetAllPeopleQueryResult>
{
    public GetAllPeopleQuery(GetAllPeople @params)
    {
        this.Params = @params;
    }

    public GetAllPeople Params { get; set; }
}