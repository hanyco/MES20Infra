using MediatR;
using Mes.System.Security.Dtos;
using System;
using Mes.System.Security;

namespace Mes.System.Security.Dtos;
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