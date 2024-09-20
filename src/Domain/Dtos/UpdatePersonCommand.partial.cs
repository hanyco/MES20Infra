using Library.Cqrs.Models.Queries;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed partial class UpdatePersonCommand : IQuery<UpdatePersonCommandResult>
{
    public PersonDto PersonDto { get; set; }

    public UpdatePersonCommand(PersonDto personDto)
    {
        this.PersonDto = personDto;
    }
}