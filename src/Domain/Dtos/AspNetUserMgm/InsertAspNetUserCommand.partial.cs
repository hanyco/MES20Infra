using MediatR;
using Mes.HumanResources.Dtos;

namespace Mes.HumanResources.Dtos;
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