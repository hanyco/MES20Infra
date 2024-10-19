using MediatR;
using Mes.HumanResources.Dtos;
using System;

namespace Mes.HumanResources.Dtos;
public sealed partial class DeleteAspNetUserCommand : IRequest<DeleteAspNetUserCommandResult>
{
    public DeleteAspNetUserCommand()
    {
    }

    public String Id { get; set; }

    public DeleteAspNetUserCommand(String id)
    {
        this.Id = id;
    }
}