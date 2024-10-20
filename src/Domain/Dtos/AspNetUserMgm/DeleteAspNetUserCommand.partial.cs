using MediatR;
using Mes.System.Security.Dtos;
using System;

namespace Mes.System.Security.Dtos;
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