using System.Collections.Generic;
using Mes.HumanResources.Dtos;

namespace Mes.HumanResources.Dtos;
public sealed partial class GetAllPeopleQueryResult
{
    public List<PersonDto> People { get; set; }

    public GetAllPeopleQueryResult(List<PersonDto> people)
    {
        this.People = people;
    }
}