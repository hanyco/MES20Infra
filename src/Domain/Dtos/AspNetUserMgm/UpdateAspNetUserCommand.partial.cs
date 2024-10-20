using MediatR;
using Mes.Infra.Auth.Dtos;
using System;
using Mes.Infra.Auth;

namespace Mes.Infra.Auth.Dtos;
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