using System.Security.Claims;

using Application.Interfaces.Shared;

namespace API.Services.Identity;

public class AuthenticatedUserService : IAuthenticatedUserService
{
    public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
    {
        this.User = httpContextAccessor.HttpContext?.User ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        this.UserId = httpContextAccessor.HttpContext.User.FindFirstValue("uid");
        this.Roles = httpContextAccessor.HttpContext.User.FindFirstValue("strRoles");

    }
    public string Roles { get; }
    public ClaimsPrincipal User { get; }
    public string UserId { get; }
    public string Username { get; }
}
