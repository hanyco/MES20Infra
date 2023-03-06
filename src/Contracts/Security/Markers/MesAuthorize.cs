using HanyCo.Infra.Security.Model;
using Microsoft.AspNetCore.Authorization;

namespace HanyCo.Infra.Security.Markers;

[AttributeUsage(AttributeTargets.Class)]
public sealed class MesAuthorizeAttribute : AuthorizeAttribute
{
    public MesAuthorizeAttribute(string claimType, string? claimValue = null)
        => (this.ClaimType, this.ClaimValue) = (claimType, claimValue);

    public MesAuthorizeAttribute(string claimType, Permission claimValue)
        => (this.ClaimType, this.ClaimValue) = (claimType, claimValue.ToString());

    public string ClaimType { get; }
    public string? ClaimValue { get; }
}
