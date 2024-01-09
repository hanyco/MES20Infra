using System.Security.Claims;

using HanyCo.Infra.Security.Exceptions;
using HanyCo.Infra.Security.Helpers;
using HanyCo.Infra.Security.Identity;
using HanyCo.Infra.Security.Model;

using Library.Results;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace HanyCo.Infra.Security.Client.Providers;

public sealed class CustomAuthenticationStateProvider(InfraUserManager userManager, IStorage storage, IOptions<JwtOptions> jwtOptions) : AuthenticationStateProvider
{
    private const string JWT_KEY = "MesIdentityJwtToken";
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var jwt = await storage.LoadAsync(JWT_KEY);
        ClaimsIdentity identity;
        if (jwt.IsSucceed && !jwt.Value.IsNullOrEmpty())
        {
            identity = getLoggedInIdentity();
            var claims = JwtHelpers.Decode(jwt.Value);
            if (claims.IsSucceed)
            {
                identity.AddClaims(claims.Value);
            }
        }
        else
        {
            identity = getNotLoggedInIdentity();
        }

        var result = await GetAuthenticationStateAsync(identity);

        return result;
    }

    public async Task<Result> LogInAsync(string username, string password, bool isPersist)
    {
        // Validate username and password
        var vr = await this.ValidateUser(username, password);

        // On any error, return thr error
        if (vr.IsFailure)
        {
            return vr;
        }

        // Get claims from UserManager and add it to the user
        var claims = await userManager.GetClaimsAsync(vr!);
        var identity = getLoggedInIdentity();
        identity.AddClaims(claims);

        // Save credentials to be used later.
        await storage.SaveAsync(JWT_KEY, JwtHelpers.Encode(claims, _jwtOptions.Issuer, _jwtOptions.Audience, _jwtOptions.SecretKey, _jwtOptions.ExpirationDate));

        // Notify Identity about authentication is changed.
        this.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync(identity));
        return Result.Success;
    }

    public async Task<Result> LogOutAsync()
    {
        // Get anonymous user and fake log it in.
        var identity = getNotLoggedInIdentity();
        // Clear current credentials storage.

        // Notify Identity about authentication is changed.
        this.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync(identity));
        return Result.Success;
    }

    public async Task<Result<InfraIdentityUser?>> ValidateUser(string username, string password)
    {
        try
        {
            // Find user by username
            var user = await userManager.FindByNameAsync(username);
            // If not found return invalid credentials error
            if (user == null)
            {
                return Result<InfraIdentityUser?>.CreateFailure<InvalidUsernameOrPasswordException>();
            }
            // Otherwise check the password
            else
            {
                if (await userManager.CheckPasswordAsync(user, password))
                {
                    // If password is ok, return the user.
                    return Result<InfraIdentityUser?>.CreateSuccess(user);
                }
                else
                {
                    // Id password is not valid return invalid credentials error.
                    return Result<InfraIdentityUser?>.CreateFailure<InvalidUsernameOrPasswordException>();
                }
            }
        }
        catch (Exception ex)
        {
            return Result<InfraIdentityUser?>.CreateFailure(ex);
        }
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