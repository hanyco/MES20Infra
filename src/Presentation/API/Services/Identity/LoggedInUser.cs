using System.Security.Claims;

using Application.Interfaces.Shared;

namespace API.Services.Identity;

public class LoggedInUser : ILoggedInUser
{
    public string? Roles { get; }
    public ClaimsPrincipal? User { get; }
    public string? UserId { get; }
    public string? Username { get; }

    public LoggedInUser(IHttpContextAccessor httpContextAccessor)
    {
        this.Roles = httpContextAccessor.HttpContext?.User?.FindFirstValue("strRoles");
        this.User = httpContextAccessor.HttpContext?.User;
        this.UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");
        this.Username = httpContextAccessor.HttpContext?.User?.Identity?.Name;
    }
}
