using System.Diagnostics.CodeAnalysis;

namespace Services.Helpers;

internal static class CommonHelpers
{
    [return: NotNullIfNotNull(nameof(name))]
    public static string? Purify(string? name) =>
        name?.TrimEnd(".")
             .TrimEnd("Dto")
             .TrimEnd("Params")
             .TrimEnd("Result")
             .TrimEnd("Query")
             .TrimEnd("Command")
             .TrimEnd("ViewModels")
             .TrimEnd("ViewModel")
             .TrimEnd("Dto")
             .TrimEnd(".");
}