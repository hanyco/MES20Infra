using System.Collections.Generic;
using Mes.Security;

namespace Mes.Security;
public sealed class AspNetUserViewModel
{
    public List<AspNetUserDto?>? AspNetUsersListDto { get; set; }
    public AspNetUserDto? AspNetUserDetailsDto { get; set; }
}