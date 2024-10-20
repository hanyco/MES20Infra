using System.Collections.Generic;
using Mes.Infra.Auth;

namespace Mes.Infra.Auth;
public sealed class AspNetUserViewModel
{
    public List<AspNetUserDto?>? AspNetUsersListDto { get; set; }
    public AspNetUserDto? AspNetUserDetailsDto { get; set; }
}