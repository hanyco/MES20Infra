using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;

namespace HanyCo.Infra.Security.Providers;

public sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        // Anonymous user (not logged in)
        
        //var identity = new ClaimsIdentity(authenticationType: "MES - Default Authentication Default Type");
        //// Anonymous user (logged in)
        
        var user = new ClaimsPrincipal(identity);

        var result = new AuthenticationState(user);
        return Task.FromResult(result);
    }
}