using HumanResources.Dtos;
    using System.Collections.Generic;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed class PersonViewModel
{
    public List<PersonDto>? PeopleListDto { get; set; }
    public PersonDto? PersonDetailsDto { get; set; }
}