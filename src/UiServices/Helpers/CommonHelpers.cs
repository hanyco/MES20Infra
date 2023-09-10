using System.Diagnostics.CodeAnalysis;

using Contracts.ViewModels;

namespace Services.Helpers;

internal static class CommonHelpers
{
    public static string? Purify(string? name) =>
        name?.TrimEnd(".")
             .TrimEnd("Dto")
             .TrimEnd("Params")
             .TrimEnd("Result")
             .TrimEnd("ViewModels")
             .TrimEnd("ViewModel")
             .TrimEnd("Dto")
             .TrimEnd(".");

    [return: NotNull]
    public static IEnumerable<string> ToSecurityKeys(this IEnumerable<ClaimViewModel> claims) =>
        claims.Select(x => x.Key).Compact();
}