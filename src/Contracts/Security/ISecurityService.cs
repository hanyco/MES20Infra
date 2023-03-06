using System.Security.Claims;
using Library.Security.Claims;

namespace HanyCo.Infra.Security;
public interface ISecurityService
{
    bool IsSignedIn(ClaimsPrincipal user);
    bool IsSignedIn(string token);

    bool UserHasClaim(ClaimsPrincipal user, ClaimInfo claim);
    bool UserHasClaim(ClaimsPrincipal user, string claimType, string? claimValue) =>
        this.UserHasClaim(user, new(claimType, claimValue));

    bool UserHasClaim(string token, ClaimInfo claim);
    bool UserHasClaim(string token, string claimType, string? claimValue) =>
        this.UserHasClaim(token, new(claimType, claimValue));
}