using MediatR;
using Mes.Infra.Security.Dtos;
using System;
using Mes.Infra.Security;

namespace Mes.Infra.Security.Dtos;
public sealed partial class UpdateAspNetUserCommand : IRequest<UpdateAspNetUserCommandResult>
{
    public UpdateAspNetUserCommand()
    {
    }

    public String Id { get; set; }
    public AspNetUserDto AspNetUser { get; set; }

    public UpdateAspNetUserCommand(String id, AspNetUserDto aspNetUser)
    {
        this.Id = id;
        this.AspNetUser = aspNetUser;
    }
}