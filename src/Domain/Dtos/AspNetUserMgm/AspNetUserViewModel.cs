using System.Collections.Generic;
using Mes.HumanResources.Dtos;

namespace Mes.HumanResources.Dtos;
public sealed class AspNetUserViewModel
{
    public List<AspNetUserDto?>? AspNetUsersListDto { get; set; }
    public AspNetUserDto? AspNetUserDetailsDto { get; set; }
}