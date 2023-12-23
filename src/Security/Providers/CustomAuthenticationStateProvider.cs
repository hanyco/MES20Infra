using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

using Library.Results;
using Library.Validations;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

namespace HanyCo.Infra.Security.Providers;

public sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Anonymous user (not logged in)
        var identity = new ClaimsIdentity();
        //// Anonymous user (logged in)
        //var identity = new ClaimsIdentity(authenticationType: "MES - Default Authentication Default Type");
        identity.AddClaims([
            new Claim(ClaimTypes.Name, "AnonymousUser"),
            new Claim(ClaimTypes.Role, "Anonymous User"),
            new Claim("FirstName", "Anonymous"),
            new Claim("LastName", "User"),
            ]);

        var user = new ClaimsPrincipal(identity);

        var result = new AuthenticationState(user);
        return Task.FromResult(result);
    }

    //public static IEnumerable<Claim> DecodeJwt(string jwtToken)
    //{
    //    var handler = new JwtSecurityTokenHandler();
    //    var tokenS = handler.ReadJwtToken(jwtToken);
    //    return tokenS.Claims;
    //}
}