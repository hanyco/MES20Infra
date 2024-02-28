﻿using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

using HanyCo.Infra.Security.Client.Providers;
using HanyCo.Infra.Security.Exceptions;
using HanyCo.Infra.Security.Identity;
using HanyCo.Infra.Security.Identity.Entity;
using HanyCo.Infra.Security.Model;
using HanyCo.Infra.Services;

using Library.Results;
using Library.Validations;

using Microsoft.AspNetCore.Components.Authorization;

namespace HanyCo.Infra.Security.Services;

internal class SecurityService(InfraUserManager userManager, InfraSignInManager signInManager, CustomAuthenticationStateProvider stateProvider) : ISecurityService
{
    public async Task<Result> CreateUserAsync([DisallowNull] InfraIdentityUser user, [DisallowNull] string password)
    {
        // Assuming userManager is an instance of UserManager<InfraIdentityUser> that has been
        // injected previously.
        var result = await userManager.CreateAsync(user, password);

        // Return the creation result
        return result.ToResult();
    }

    public Task<Result> DeleteRoleByIdAsync(Guid id) => throw new NotImplementedException();

    public Task<Result> DeleteUserByIdAsync(Guid id) => throw new NotImplementedException();

    public IEnumerable<InfraIdentityRole> GetRoles() => throw new NotImplementedException();

    public Task<InfraIdentityUser> GetUserByIdAsync(Guid id) => throw new NotImplementedException();

    public IEnumerable<InfraIdentityUser> GetUsers() =>
        userManager.Users;

    public async Task<Result> LogInAsync([DisallowNull] string username, [DisallowNull] string password, bool isPersist)
    {
        // Validate inputs
        Check.MustBeArgumentNotNull(username);
        Check.MustBeArgumentNotNull(password);

        // If no user is created, return NoUserFound.
        if (!userManager.Users.Any())
        {
            return Result.Fail<NoUserFoundException>();
        }

        // Validate username and password
        var vr = await validateUser(username, password);
        if (vr.IsFailure)
        {
            return vr;
        }

        //await signInManager.SignInAsync(vr!, isPersist);
        var claims = await userManager.GetClaimsAsync(vr);
        stateProvider.SignIn(vr, claims);
        return Result.Succeed;

        async Task<Result<InfraIdentityUser?>> validateUser(string username, string password)
        {
            // Find user by username
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Result.Fail<InfraIdentityUser, InvalidUsernameOrPasswordException>();
            }

            // Can user sign in?
            if (!await signInManager.CanSignInAsync(user))
            {
                return Result.Fail<InfraIdentityUser, UserCannotLoginException>();
            }

            // Is user locked out?
            if (userManager.SupportsUserLockout && await userManager.IsLockedOutAsync(user))
            {
                return Result.Fail<InfraIdentityUser, UserIsLockedOutException>();
            }

            // Check password
            var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                // Increase the count of login failures., if supported.
                if (userManager.SupportsUserLockout)
                {
                    _ = await userManager.AccessFailedAsync(user);
                }
                return Result.Fail<InfraIdentityUser, InvalidUsernameOrPasswordException>();
            }

            // Return user and succeed result
            return Result.Success<InfraIdentityUser?>(user);
        }
    }

    public async Task<Result> LogOutAsync()
    {
        stateProvider.SignOut();
        return Result.Succeed;
    }

    public async Task<Result> SetUserClaimAsync([DisallowNull] InfraIdentityUser user, [DisallowNull] string claimType, string claimValue)
    {
        Check.MustBeArgumentNotNull(user);
        Check.MustBeArgumentNotNull(claimType);
        Check.MustBeArgumentNotNull(claimValue);

        var claims = await userManager.GetClaimsAsync(user);
        var oldClaim = claims.FirstOrDefault(x => x.Type == claimType);
        var newClaim = new Claim(claimType, claimValue);
        var result = oldClaim != null
            ? await userManager.ReplaceClaimAsync(user, oldClaim, newClaim)
            : await userManager.AddClaimAsync(user, newClaim);
        return result.ToResult();
    }

    public Task<Result> UpdateUserAsync([DisallowNull] InfraIdentityUser user) => throw new NotImplementedException();

    internal static Task<AuthenticationState> GetAuthenticationStateAsync([DisallowNull] ClaimsIdentity identity)
    {
        var principal = new ClaimsPrincipal(identity);
        var result = new AuthenticationState(principal);
        return Task.FromResult(result);
    }

    internal static ClaimsIdentity GetNotLoggedInIdentity() =>
        new();

    internal static ClaimsIdentity GetSampleAdminIdentity() =>
        new(
        [
            new Claim(ClaimTypes.Name, "Administrator"),
            new Claim(ClaimTypes.Role, InfraIdentityValues.RoleAdminValue),
            new Claim(InfraIdentityValues.ClaimFirstName, "Mohammad-Admin"),
            new Claim(InfraIdentityValues.ClaimLastName, "Mirmostafa"),
        ], InfraIdentityValues.LoggedInAuthenticationType);

    internal static ClaimsIdentity GetSampleLoggedInIdentity() =>
                new(InfraIdentityValues.LoggedInAuthenticationType);

    internal static ClaimsIdentity GetSampleSupervisorIdentity() =>
        new(
        [
            new Claim(ClaimTypes.Name, "User"),
            new Claim(ClaimTypes.Role, InfraIdentityValues.RoleSupervisor),
            new Claim(InfraIdentityValues.ClaimFirstName, "Mohammad-Supervisor"),
            new Claim(InfraIdentityValues.ClaimLastName, "Mirmostafa"),
        ], InfraIdentityValues.LoggedInAuthenticationType);
}