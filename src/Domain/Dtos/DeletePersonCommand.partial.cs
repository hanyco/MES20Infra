using MediatR;
using Mes.HumanResources.Dtos;
using System;

namespace Mes.HumanResources.Dtos;
public sealed partial class DeletePersonCommand : IRequest<DeletePersonCommandResult>
{
    public DeletePersonCommand()
    {
    }

    public Int64 Id { get; set; }

    public DeletePersonCommand(Int64 id)
    {
        this.Id = id;
    }
}