using MediatR;
using Mes.Infra.Auth.Dtos;
using System;

namespace Mes.Infra.Auth.Dtos;
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