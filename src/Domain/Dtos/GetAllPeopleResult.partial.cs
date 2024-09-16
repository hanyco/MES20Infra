using System.Collections;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed partial class GetAllPeopleQueryResult
{
    public IList<PersonDto?> People { get; set; }

    public GetAllPeopleQueryResult(IList<PersonDto?> people)
    {
        this.People = people;
    }

    public GetAllPeopleQueryResult()
    {
    }
}