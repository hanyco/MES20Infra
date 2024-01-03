using System.Security.Claims;

using HanyCo.Infra.Security.Identity.Model;

using Microsoft.AspNetCore.Components.Authorization;

namespace HanyCo.Infra.Security.Client.Providers;

public sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Anonymous user (not logged in)
        //var identity = new ClaimsIdentity();
        var identity = getSampleAdminIdentity();

        var principal = new ClaimsPrincipal(identity);

        var result = new AuthenticationState(principal);
        return Task.FromResult(result);

        static ClaimsIdentity getSampleAdminIdentity()
        {
            var identity = new ClaimsIdentity(authenticationType: InfraAuthenticationValues.DefaultAuthenticationType);
            identity.AddClaims(
            [
                new Claim(ClaimTypes.Name, "Administrator"),
                new Claim(ClaimTypes.Role, InfraAuthenticationValues.RoleAdminValue),
                new Claim(InfraAuthenticationValues.FirstName, "Mohammad"),
                new Claim(InfraAuthenticationValues.LastName, "Mirmostafa"),
            ]);
            return identity;
        }
    }
}