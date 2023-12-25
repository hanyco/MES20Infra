using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HanyCo.Infra.Security.Client.Providers;

public sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Anonymous user (not logged in)
        //var identity = new ClaimsIdentity();
        // Anonymous user (logged in)
        var identity = new ClaimsIdentity(authenticationType: "MES Infra Authentication Type");
        identity.AddClaims([
            new Claim(ClaimTypes.Name, "Administrator"),
            new Claim(ClaimTypes.Role, "Administrators"),
            new Claim("FirstName", "Mohammad"),
            new Claim("LastName", "Mirmostafa"),
            ]);

        var user = new ClaimsPrincipal(identity);
        

        var result = new AuthenticationState(user);
        return Task.FromResult(result);
    }
}