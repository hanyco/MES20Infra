using Library.Security.Claims;
using Library.Web.Api;

namespace HanyCo.Infra.Security.Model;

public record struct AuthorizationInfo(in ClaimInfo? ClaimInfo, in string? Policy, in string? Roles, in string? AuthenticationSchemes);

public record struct ApiAuthorizationInfo(ApiInfo Api, in AuthorizationInfo? Authorization);