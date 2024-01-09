using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Library.Results;
using Library.Validations;

using Microsoft.IdentityModel.Tokens;

namespace HanyCo.Infra.Security.Helpers;

public static class JwtHelpers
{
    [Pure]
    [return: NotNull]
    public static Result<IEnumerable<Claim>> Decode(string jwtToken)
    {
        return CatchResult(operate, jwtToken);

        static IEnumerable<Claim> operate(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadJwtToken(token);
            return tokenS.Claims;
        }
    }

    public static string Encode(IEnumerable<Claim> claims, string issuer = "MES Infra", string audience = "MES", string secretKey = "HanyCo MES Infra JWT Token Secret Key", DateTime? expiresOn = null)
    {
        var key = GetIssuerSigningKey(secretKey);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresOn ?? DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public static string Encode(ClaimsIdentity identity, string issuer = "MES Infra", string audience = "MES", string secretKey = "HanyCo MES Infra JWT Token Secret Key", DateTime? expiresOn = null) =>
        Encode(identity.ArgumentNotNull().Claims, issuer, audience, secretKey, expiresOn);

    public static SymmetricSecurityKey GetIssuerSigningKey(string secretKey) =>
            new(Encoding.UTF8.GetBytes(secretKey));
}