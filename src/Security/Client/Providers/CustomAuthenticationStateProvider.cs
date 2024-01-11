using HanyCo.Infra.Security.Identity;
using HanyCo.Infra.Security.Model;
using HanyCo.Infra.Security.Services;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace HanyCo.Infra.Security.Client.Providers;

internal sealed class CustomAuthenticationStateProvider(InfraUserManager userManager, IStorage storage, IOptions<JwtOptions> jwtOptions) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = SecurityService.GetNotLoggedInIdentity();
        var result = await SecurityService.GetAuthenticationStateAsync(identity);

        return result;
    }
}