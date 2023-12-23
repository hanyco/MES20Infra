using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Library.Results;

namespace HanyCo.Infra.Security.Helpers;

public static class JwtHelpers
{
    [Pure]
    [return: NotNull]
    public static Result<IEnumerable<Claim>> DecodeJwt(string jwtToken)
    {
        return TryResult(operate, jwtToken);

        static IEnumerable<Claim> operate(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadJwtToken(token);
            return tokenS.Claims;
        }
    }
}