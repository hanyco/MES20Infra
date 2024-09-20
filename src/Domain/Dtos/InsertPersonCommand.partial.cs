using Library.Cqrs.Models.Queries;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed partial class InsertPersonCommand : IQuery<InsertPersonCommandResult>
{
    public PersonDto PersonDto { get; set; }

    public InsertPersonCommand(PersonDto personDto)
    {
        this.PersonDto = personDto;
    }
}