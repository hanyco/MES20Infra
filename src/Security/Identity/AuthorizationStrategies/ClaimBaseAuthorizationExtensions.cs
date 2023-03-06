using Library.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HanyCo.Infra.Security.Identity.AuthorizationStrategies;

public static class ClaimBaseAuthorizationExtensions
{
    public static ClaimInfo? GetClaimValue(this HttpContext context) =>
        MvcHelper.GetCurrentAction(context).GetInfo(context)?.Authorization?.ClaimInfo;
}