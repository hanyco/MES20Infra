using System.Security.Claims;

using HanyCo.Infra.Security.Helpers;
using HanyCo.Infra.Security.Identity;
using HanyCo.Infra.Security.Model;
using HanyCo.Infra.Services;

using Library.Results;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace HanyCo.Infra.Security.Services;

internal class SecurityService(InfraSignInManager signInManager, IStorage storage, IOptions<JwtOptions> jwtOptions, AuthenticationStateProvider stateProvider) : ISecurityService
{
    private const string JWT_KEY = "MesIdentityJwtToken";
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public Task<Result> CreateAsync(InfraIdentityUser user, string password) => throw new NotImplementedException();

    public Task<InfraIdentityUser> GetUserByIdAsync(Guid id) => throw new NotImplementedException();

    public IEnumerable<InfraIdentityUser> GetUsers() => throw new NotImplementedException();

    public async Task<Result> LogInAsync(string username, string password, bool isPersist)
    {
        //// Validate username and password
        //var vr = await this.ValidateUser(username, password);

        //// On any error, return thr error
        //if (vr.IsFailure)
        //{
        //    return vr;
        //}

        //// Get claims from UserManager and add it to the user
        //var claims = await userManager.GetClaimsAsync(vr!);
        //var identity = getLoggedInIdentity();
        //identity.AddClaims(claims);

        var identity = GetSampleAdminIdentity();

        // Save credentials to be used later.
        _ = await storage.SaveAsync(JWT_KEY, JwtHelpers.Encode(identity, this._jwtOptions.Issuer, this._jwtOptions.Audience, this._jwtOptions.SecretKey, this._jwtOptions.ExpirationDate));

        // Notify Identity about authentication is changed.
        //x stateProvider.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync(identity));
        return Result.Success;
    }

    public async Task<Result> LogOutAsync()
    {
        // Clear current credentials storage.
        _ = await storage.DeleteAsync(JWT_KEY);

        // Get anonymous user and fake log it in.
        var identity = GetNotLoggedInIdentity();
        // Notify Identity about authentication is changed.
        //x this.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync(identity));
        return Result.Success;
    }

    public Task<Result> SetClaimAsync(InfraIdentityUser user, string claimType, string claimValue) => throw new NotImplementedException();

    public Task<Result> UpdateAsync(InfraIdentityUser user) => throw new NotImplementedException();

    internal static Task<AuthenticationState> GetAuthenticationStateAsync(ClaimsIdentity identity)
    {
        var principal = new ClaimsPrincipal(identity);
        var result = new AuthenticationState(principal);
        return Task.FromResult(result);
    }

    internal static ClaimsIdentity GetLoggedInIdentity() =>
        new(InfraIdentityValues.LoggedInAuthenticationType);

    internal static ClaimsIdentity GetNotLoggedInIdentity() =>
        new();

    internal static ClaimsIdentity GetSampleAdminIdentity()
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

    internal static ClaimsIdentity GetSampleSupervisorIdentity()
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