using System.Collections.Generic;
using HumanResources;

namespace HumanResources;
public sealed class PersonViewModel
{
    public List<PersonDto?>? PeopleListDto { get; set; }
    public PersonDto? PersonDetailsDto { get; set; }
}