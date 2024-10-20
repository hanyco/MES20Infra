using MediatR;
using Mes.System.Security.Dtos;
using Mes.System.Security;

namespace Mes.System.Security.Dtos;
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