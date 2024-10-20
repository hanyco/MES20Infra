using System.Collections.Generic;
using Mes.Infra.Security;

namespace Mes.Infra.Security.Dtos;
public sealed partial class GetAllAspNetUsersQueryResult
{
    public List<AspNetUserDto> AspNetUsers { get; set; }

    public GetAllAspNetUsersQueryResult(List<AspNetUserDto> aspNetUsers)
    {
        this.AspNetUsers = aspNetUsers;
    }
}