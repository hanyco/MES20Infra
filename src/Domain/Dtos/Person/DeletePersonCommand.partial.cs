using MediatR;
using Mes.HumanResourcesManagement.Dtos;
using System;

namespace Mes.HumanResourcesManagement.Dtos;
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