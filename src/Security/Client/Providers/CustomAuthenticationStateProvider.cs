using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;

namespace HanyCo.Infra.Security.Client.Providers;

public sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Anonymous user (not logged in)
        var identity = new ClaimsIdentity();
        //// Anonymous user (logged in)
        //var identity = new ClaimsIdentity(authenticationType: "MES - Default Authentication Default Type");
        identity.AddClaims([
            new Claim(ClaimTypes.Name, "AnonymousUser"),
            new Claim(ClaimTypes.Role, "Anonymous User"),
            new Claim("FirstName", "Anonymous"),
            new Claim("LastName", "User"),
            ]);

        var user = new ClaimsPrincipal(identity);

        var result = new AuthenticationState(user);
        return Task.FromResult(result);
    }
}