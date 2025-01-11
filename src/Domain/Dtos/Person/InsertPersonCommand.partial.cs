using MediatR;
using Mes.HumanResourcesManagement.Dtos;

namespace Mes.HumanResourcesManagement.Dtos;
public sealed partial class InsertPersonCommand : IRequest<InsertPersonCommandResult>
{
    public InsertPersonCommand()
    {
    }

    public PersonDto Person { get; set; }

    public InsertPersonCommand(PersonDto person)
    {
        this.Person = person;
    }
}