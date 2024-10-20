using System.Collections.Generic;
using Mes.Infra.Security;

namespace Mes.Infra.Security;
public sealed class AspNetUserViewModel
{
    public List<AspNetUserDto?>? AspNetUsersListDto { get; set; }
    public AspNetUserDto? AspNetUserDetailsDto { get; set; }
}