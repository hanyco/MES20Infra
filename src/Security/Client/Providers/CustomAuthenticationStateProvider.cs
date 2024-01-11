using System.Security.Claims;

using HanyCo.Infra.Security.Model;

using Microsoft.AspNetCore.Components.Authorization;

namespace HanyCo.Infra.Security.Client.Providers;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal? _user;

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => 
        Task.FromResult(new AuthenticationState(this._user));

    public void NotifyAuthenticationStateChanged(ClaimsPrincipal? user)
    {
        this._user = user;
        this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
    }

    public void SignIn(InfraIdentityUser user, IEnumerable<Claim> claims)
    {
        //var claims = new List<Claim>
        //{
        //    new(ClaimTypes.Name, userName),
        //    // Add other claims as needed
        //};

        var identity = new ClaimsIdentity(claims, InfraIdentityValues.LoggedInAuthenticationType);
        var principal = new ClaimsPrincipal(identity);

        //var authStateProvider = new CustomAuthenticationStateProvider();
        //authStateProvider.NotifyAuthenticationStateChanged(principal);
        this.NotifyAuthenticationStateChanged(principal);
    }

    public void SignOut() =>
        //var authStateProvider = new CustomAuthenticationStateProvider();
        //authStateProvider.NotifyAuthenticationStateChanged(null);
        this.NotifyAuthenticationStateChanged(null);
}