using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace HanyCo.Infra.Security.Identity.AuthorizationStrategies;

public sealed record ClaimBaseAuthorizationRequirement : IAuthorizationRequirement;

public sealed class ClaimBaseAuthorizationHandler : AuthorizationHandler<ClaimBaseAuthorizationRequirement>
{
    private readonly ISecurityService _securityService;
    private readonly IHttpContextAccessor _contextAccessor;

    public ClaimBaseAuthorizationHandler(ISecurityService securityService, IHttpContextAccessor contextAccessor)
    {
        this._securityService = securityService;
        this._contextAccessor = contextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimBaseAuthorizationRequirement requirement)
    {
        if (context is null || requirement is null)
        {
            context?.Succeed(requirement);
            return Task.CompletedTask;
        }

        var claim = this._contextAccessor?.HttpContext?.GetClaimValue();
        if (claim is null)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        else if (!this._securityService.IsSignedIn(context.User))
        {
            return Task.CompletedTask;
        }
        else if (this._securityService.UserHasClaim(context.User, claim.Value))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
