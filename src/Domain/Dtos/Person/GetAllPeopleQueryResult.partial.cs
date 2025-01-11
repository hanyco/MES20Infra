using System.Collections.Generic;
using Mes.HumanResourcesManagement.Dtos;

namespace Mes.HumanResourcesManagement.Dtos;
public sealed partial class GetAllPeopleQueryResult
{
    public List<PersonDto> People { get; set; }

    public GetAllPeopleQueryResult(List<PersonDto> people)
    {
        this.People = people;
    }
}