using HanyCo.Infra.Security.Markers;
using HanyCo.Infra.Security.Model;
using Library.Security.Claims;
using Library.Validations;
using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.Security.Helpers;

public static class AuthorizationAttributeHelper
{
    public static AuthorizationInfo ToAuthorizationInfo([DisallowNull] this MesAuthorizeAttribute attribute)
    {
        Check.MustBeArgumentNotNull(attribute);

        ClaimInfo? claim = attribute.ClaimType != null ? new(attribute.ClaimType, attribute.ClaimValue) : null;
        return new(claim, attribute.Policy, attribute.Roles, attribute.AuthenticationSchemes);
    }
}
