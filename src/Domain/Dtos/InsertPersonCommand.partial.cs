using MediatR;
using Mes.HumanResources.Dtos;

namespace Mes.HumanResources.Dtos;
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