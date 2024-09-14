using System.Collections;
using HumanResources.Dtos;

namespace HumanResource.Dtos;
public sealed partial class GetAllPeopleQueryResult
{
    public IList<PersonDto?> People { get; set; }

    public GetAllPeopleQueryResult(IList<PersonDto?> people)
    {
        this.People = people;
    }
}