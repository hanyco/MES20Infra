using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.Security.Identity.AuthorizationStrategies;

internal static class PolicyManager
{
    [return: NotNull]
    internal static IEnumerable<(string ClaimType, string ClaimValue)> GetClaimsByRoute(string localPath) => [("Admin", string.Empty)];
}