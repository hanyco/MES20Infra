using System.Security.Claims;

using Application.Interfaces.Shared;

namespace API.Services.Identity;

public class AuthenticatedUserService(IHttpContextAccessor httpContextAccessor) : IAuthenticatedUserService
{
    public string Roles { get; } = httpContextAccessor.HttpContext?.User?.FindFirstValue("strRoles") ?? throw new ArgumentNullException("Roles");
    public ClaimsPrincipal User { get; } = httpContextAccessor.HttpContext?.User ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    public string UserId { get; } = httpContextAccessor.HttpContext.User.FindFirstValue("uid") ?? throw new ArgumentNullException("UserId");
    public string Username { get; } = httpContextAccessor.HttpContext.User.Identity?.Name ?? throw new ArgumentNullException("Username");
}
