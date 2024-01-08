using System.Security.Claims;

using HanyCo.Infra.Security.Exceptions;
using HanyCo.Infra.Security.Identity;

using Library.Results;

using Microsoft.AspNetCore.Components.Authorization;

namespace HanyCo.Infra.Security.Client.Providers;

public sealed class CustomAuthenticationStateProvider(InfraUserManager userManager) : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = getNotLoggedInIdentity();
        var result = GetAuthenticationStateAsync(identity);

        return result;
    }

    public async Task<Result> LogInAsync(string username, string password, bool isPersist)
    {
        var user = await this.ValidateUser(username, password);
        if (user.IsFailure)
        {
            return user;
        }
        var claims = await userManager.GetClaimsAsync(user!);
        var identity = getLoggedInIdentity();
        identity.AddClaims(claims);
        if (isPersist)
        {
            // بی معنی هست!
            //await storage.SaveAsync()
        }
        this.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync(identity));
        return Result.Success;
    }

    public Task<Result> LogOutAsync(string username, string password)
    {
        this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
        return Task.FromResult(Result.Success);
    }

    public async Task<Result<InfraIdentityUser?>> ValidateUser(string username, string password)
    {
        try
        {
            var user = await userManager.FindByNameAsync(username);
            return user == null
                ? Result<InfraIdentityUser?>.CreateFailure<InvalidUsernameOrPasswordException>()
                : await userManager.CheckPasswordAsync(user, password)
                    ? Result<InfraIdentityUser?>.CreateSuccess(user)
                    : Result<InfraIdentityUser?>.CreateFailure<InvalidUsernameOrPasswordException>();
        }
        catch (Exception ex)
        {
            return Result<InfraIdentityUser?>.CreateFailure(ex);
        }
    }

    private static ClaimsIdentity getAnonymousUserIdentity()
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

    private static Task<AuthenticationState> GetAuthenticationStateAsync(ClaimsIdentity identity)
    {
        var principal = new ClaimsPrincipal(identity);
        var result = new AuthenticationState(principal);
        return Task.FromResult(result);
    }

    private static ClaimsIdentity getLoggedInIdentity() =>
        new(InfraIdentityValues.LoggedInAuthenticationType);

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
}