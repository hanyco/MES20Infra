using System.Security.Claims;

using HanyCo.Infra.Security.Model;

using Library.Validations;

using Microsoft.AspNetCore.Components.Authorization;

namespace HanyCo.Infra.Security.Client.Providers;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser;

    public CustomAuthenticationStateProvider() => 
        this._currentUser = new ClaimsPrincipal(new ClaimsIdentity()); // Anonymous user by default

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(new AuthenticationState(this._currentUser));

    public void SignIn(InfraIdentityUser user, IEnumerable<Claim> claims)
    {
        Check.MustBeArgumentNotNull(user);

        var cl = claims.ToList();
        if (!cl.Where(x => x.Type == ClaimTypes.Name).Any())
        {
            cl.Add(new(ClaimTypes.Name, user.UserName));
        }

        var identity = new ClaimsIdentity(cl, InfraIdentityValues.LoggedInAuthenticationType);
        this._currentUser = new ClaimsPrincipal(identity);
        this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
    }

    // Add a sign-out method if needed
    public void SignOut()
    {
        this._currentUser = new ClaimsPrincipal(new ClaimsIdentity()); // Reset to anonymous user
        this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
    }
}