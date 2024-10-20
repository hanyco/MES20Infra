using MediatR;
using Mes.Infra.Auth.Dtos;
using Mes.Infra.Auth;

namespace Mes.Infra.Auth.Dtos;
public sealed partial class InsertAspNetUserCommand : IRequest<InsertAspNetUserCommandResult>
{
    public InsertAspNetUserCommand()
    {
    }

    public AspNetUserDto AspNetUser { get; set; }

    public InsertAspNetUserCommand(AspNetUserDto aspNetUser)
    {
        this.AspNetUser = aspNetUser;
    }
}