using MediatR;
using HumanResources.Dtos;
using System;

namespace HumanResources.Dtos;
public sealed partial class InsertPersonCommand : IRequest<InsertPersonCommandResult>
{
    public InsertPersonCommand(PersonDto personDto)
    {
    }
}