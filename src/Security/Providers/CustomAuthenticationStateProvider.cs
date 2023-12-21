using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;

namespace HanyCo.Infra.Security.Providers;

public sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        // Anonymous user
        var user = new ClaimsPrincipal(identity);
        var result = new AuthenticationState(user);
        return Task.FromResult(result);
    }
}