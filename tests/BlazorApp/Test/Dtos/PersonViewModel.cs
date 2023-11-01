using Test.HumanResources;
using Test.HumanResources;

namespace Test.HumanResources
{
    public sealed class PersonViewModel
    {
        public IEnumerable<PersonDto> PeopleListDto { get; set; }
        public PersonDto PersonDetailsDto { get; set; }
    }
}