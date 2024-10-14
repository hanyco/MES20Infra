using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Shared;
public interface IAuthenticatedUserService
{
    public ClaimsPrincipal User { get; set; }
    public string UserId { get; }
    public string Username { get; }
    public string Roles { get; }
}