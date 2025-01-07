using System.Security.Claims;

namespace Application.Interfaces.Shared;
public interface ILoggedInUser
{
    bool IsAuthenticated => this.Username is not null && this.UserId is not null && (this.User?.Identity?.IsAuthenticated ?? false);
    string? Roles { get; }
    ClaimsPrincipal? User { get; }
    string? UserId { get; }
    string? Username { get; }
}