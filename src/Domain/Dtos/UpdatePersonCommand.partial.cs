using MediatR;
using HumanResources.Dtos;
using System;

namespace HumanResources.Dtos;
public sealed partial class UpdatePersonCommand : IRequest<UpdatePersonCommandResult>
{
    public UpdatePersonCommand(Int64 id, PersonDto personDto)
    {
    }
}