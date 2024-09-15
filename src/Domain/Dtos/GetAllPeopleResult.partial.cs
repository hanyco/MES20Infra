using System.Collections;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed partial class GetAllPeopleResult
{
    public IList<PersonDto?> People { get; set; }

    public GetAllPeopleResult(IList<PersonDto?> people)
    {
        this.People = people;
    }

    public GetAllPeopleResult()
    {
    }
}