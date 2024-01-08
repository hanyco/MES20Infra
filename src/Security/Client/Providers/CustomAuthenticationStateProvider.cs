using System.Security.Claims;

using HanyCo.Infra.Security.Helpers;
using HanyCo.Infra.Security.Identity;

using Library.Results;

using Microsoft.AspNetCore.Components.Authorization;

namespace HanyCo.Infra.Security.Client.Providers;

public sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var test = JwtHelpers.Encode(getSampleAdminIdentity().Claims);
        var identity = getSampleSupervisorIdentity();
        var principal = new ClaimsPrincipal(identity);
        var result = new AuthenticationState(principal);

        return Task.FromResult(result);
    }

    public Task<Result> LogInAsync(string username, string password)
    {
        this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
        return Task.FromResult(Result.Success);
    }

    public Task<Result> LogOutAsync(string username, string password)
    {
        this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
        return Task.FromResult(Result.Success);
    }

    private static ClaimsIdentity getNotLoggedInIdentity() =>
            new();

    private static ClaimsIdentity getSampleAdminIdentity()
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "Administrator"),
            new Claim(ClaimTypes.Role, InfraIdentityValues.RoleAdminValue),
            new Claim(InfraIdentityValues.ClaimFirstName, "Mohammad-Admin"),
            new Claim(InfraIdentityValues.ClaimLastName, "Mirmostafa"),
        ], InfraIdentityValues.LoggedInAuthenticationType);
        return identity;
    }

    private static ClaimsIdentity getSampleSupervisorIdentity()
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "User"),
            new Claim(ClaimTypes.Role, InfraIdentityValues.RoleSupervisor),
            new Claim(InfraIdentityValues.ClaimFirstName, "Mohammad-Supervisor"),
            new Claim(InfraIdentityValues.ClaimLastName, "Mirmostafa"),
        ], InfraIdentityValues.LoggedInAuthenticationType);
        return identity;
    }

    private static ClaimsIdentity getSampleUserIdentity()
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "User"),
            new Claim(ClaimTypes.Role, InfraIdentityValues.RoleUser),
            new Claim(InfraIdentityValues.ClaimFirstName, "Mohammad-User"),
            new Claim(InfraIdentityValues.ClaimLastName, "Mirmostafa"),
        ], InfraIdentityValues.LoggedInAuthenticationType);
        return identity;
    }
}