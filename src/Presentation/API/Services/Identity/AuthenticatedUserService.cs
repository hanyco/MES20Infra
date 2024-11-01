using Application.Interfaces.Shared;

using System.Security.Claims;

namespace API.Services.Identity;

public class AuthenticatedUserService : IAuthenticatedUserService
{
    public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
    {

        User = httpContextAccessor.HttpContext?.User;
        UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");
        Roles = httpContextAccessor.HttpContext?.User?.FindFirstValue("strRoles");

    }
    public ClaimsPrincipal User { get; set; }
    public string UserId { get; }
    public string Username { get; }
    public string Roles { get; }
}
