using System.Security.Claims;

using HanyCo.Infra.Security.Identity.Model;

using Microsoft.AspNetCore.Components.Authorization;

namespace HanyCo.Infra.Security.Client.Providers;

public sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = getSampleSupervisorIdentity();
        var principal = new ClaimsPrincipal(identity);
        var result = new AuthenticationState(principal);

        return Task.FromResult(result);
    }

    private static ClaimsIdentity getNotLoggedInIdentity() =>
        new();

    private static ClaimsIdentity getSampleAdminIdentity()
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "Administrator"),
            new Claim(ClaimTypes.Role, InfraAuthenticationValues.RoleAdminValue),
            new Claim(InfraAuthenticationValues.ClaimFirstName, "Mohammad-Admin"),
            new Claim(InfraAuthenticationValues.ClaimLastName, "Mirmostafa"),
        ], InfraAuthenticationValues.LoggedInAuthenticationType);
        return identity;
    }

    private static ClaimsIdentity getSampleSupervisorIdentity()
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "User"),
            new Claim(ClaimTypes.Role, InfraAuthenticationValues.RoleSupervisor),
            new Claim(InfraAuthenticationValues.ClaimFirstName, "Mohammad-Supervisor"),
            new Claim(InfraAuthenticationValues.ClaimLastName, "Mirmostafa"),
        ], InfraAuthenticationValues.LoggedInAuthenticationType);
        return identity;
    }

    private static ClaimsIdentity getSampleUserIdentity()
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "User"),
            new Claim(ClaimTypes.Role, InfraAuthenticationValues.RoleUser),
            new Claim(InfraAuthenticationValues.ClaimFirstName, "Mohammad-User"),
            new Claim(InfraAuthenticationValues.ClaimLastName, "Mirmostafa"),
        ], InfraAuthenticationValues.LoggedInAuthenticationType);
        return identity;
    }
}