using Library.Cqrs.Models.Queries;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed partial class DeletePersonCommand : IQuery<DeletePersonCommandResult>
{
    public PersonDto PersonDto { get; set; }

    public DeletePersonCommand(PersonDto personDto)
    {
        this.PersonDto = personDto;
    }
}