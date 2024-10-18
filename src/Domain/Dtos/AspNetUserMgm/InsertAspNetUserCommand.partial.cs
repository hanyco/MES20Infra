using MediatR;
using Mes.Security.Dtos;
using Mes.Security;

namespace Mes.Security.Dtos;
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