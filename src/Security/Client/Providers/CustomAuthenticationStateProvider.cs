using System.Security.Claims;

using HanyCo.Infra.Security.Exceptions;
using HanyCo.Infra.Security.Helpers;
using HanyCo.Infra.Security.Identity;
using HanyCo.Infra.Security.Model;

using Library.Results;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using HanyCo.Infra.Security.Services;

namespace HanyCo.Infra.Security.Client.Providers;

internal sealed class CustomAuthenticationStateProvider(InfraUserManager userManager, IStorage storage, IOptions<JwtOptions> jwtOptions) : AuthenticationStateProvider
{
    private const string JWT_KEY = "MesIdentityJwtToken";
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var jwt = await storage.LoadAsync(JWT_KEY);
        ClaimsIdentity identity;
        if (jwt.IsSucceed && !jwt.Value.IsNullOrEmpty())
        {
            identity = SecurityService.GetLoggedInIdentity();
            var claims = JwtHelpers.Decode(jwt.Value);
            if (claims.IsSucceed)
            {
                identity.AddClaims(claims.Value);
            }
        }
        else
        {
            identity = SecurityService.GetNotLoggedInIdentity();
        }

        var result = await SecurityService.GetAuthenticationStateAsync(identity);

        return result;
    }

    
}