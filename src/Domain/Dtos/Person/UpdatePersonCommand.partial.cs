using MediatR;
using Mes.HumanResourcesManagement.Dtos;
using System;

namespace Mes.HumanResourcesManagement.Dtos;
public sealed partial class UpdatePersonCommand : IRequest<UpdatePersonCommandResult>
{
    public UpdatePersonCommand()
    {
    }

    public Int64 Id { get; set; }
    public PersonDto Person { get; set; }

    public UpdatePersonCommand(Int64 id, PersonDto person)
    {
        this.Id = id;
        this.Person = person;
    }
}