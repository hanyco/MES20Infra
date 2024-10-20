using MediatR;
using Mes.Infra.Security.Dtos;
using Mes.Infra.Security;

namespace Mes.Infra.Security.Dtos;
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