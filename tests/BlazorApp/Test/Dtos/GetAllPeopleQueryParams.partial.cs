using Library.Cqrs.Models.Queries;

namespace Test.HumanResources.Dtos;
public sealed class GetAllPeopleQuery : IQuery<GetAllPeopleQueryResult>
{
    public GetAllPeopleQuery()
    {
    }
}