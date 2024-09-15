using HumanResources.Dtos.PersonDto ? . System . Collections . Generic;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed class PersonViewModel
{
    public List? PeopleListDto { get; set; }
    public PersonDto? PersonDetailsDto { get; set; }
}