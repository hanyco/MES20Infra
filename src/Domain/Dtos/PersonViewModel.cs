using System.Collections.Generic;
using Mes.HumanResources.Dtos;

namespace Mes.HumanResources.Dtos;
public sealed class PersonViewModel
{
    public List<PersonDto?>? PeopleListDto { get; set; }
    public PersonDto? PersonDetailsDto { get; set; }
}