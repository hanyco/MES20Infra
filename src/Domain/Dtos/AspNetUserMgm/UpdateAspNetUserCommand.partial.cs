using MediatR;
using Mes.HumanResources.Dtos;
using System;

namespace Mes.HumanResources.Dtos;
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