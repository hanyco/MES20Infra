using System.Collections.Generic;
using Mes.HumanResources.Dtos;

namespace Mes.HumanResources.Dtos;
public sealed partial class GetAllAspNetUsersQueryResult
{
    public List<AspNetUserDto> AspNetUsers { get; set; }

    public GetAllAspNetUsersQueryResult(List<AspNetUserDto> aspNetUsers)
    {
        this.AspNetUsers = aspNetUsers;
    }
}