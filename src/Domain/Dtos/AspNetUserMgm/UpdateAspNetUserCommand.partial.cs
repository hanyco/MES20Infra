using MediatR;
using Mes.Security.Dtos;
using System;
using Mes.Security;

namespace Mes.Security.Dtos;
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