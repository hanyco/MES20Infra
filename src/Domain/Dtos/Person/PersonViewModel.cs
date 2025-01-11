using System.Collections.Generic;
using Mes.HumanResourcesManagement.Dtos;

namespace Mes.HumanResourcesManagement.Dtos;
public sealed class PersonViewModel
{
    public List<PersonDto?>? PeopleListDto { get; set; }
    public PersonDto? PersonDetailsDto { get; set; }
}