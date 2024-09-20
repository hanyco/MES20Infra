using MediatR;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed partial class InsertPersonCommand : IRequest<InsertPersonCommandResult>
{
    public PersonDto Person { get; set; }

    public InsertPersonCommand(PersonDto person)
    {
        this.Person = person;
    }
}