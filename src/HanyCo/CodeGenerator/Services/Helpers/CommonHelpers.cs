using System.Diagnostics.CodeAnalysis;

namespace Services.CodeGen.Helpers;

internal static class CommonHelpers
{
    [return: NotNullIfNotNull(nameof(name))]
    public static string? Purify(string? name)
    {
        if (name.IsNullOrEmpty())
        {
            return name;
        }

        var span = name.AsSpan();
        var suffixes = new string[]
        {
            "Dto",
            "Dtos",
            "Params",
            "Result",
            "Query",
            "Command",
            "ViewModels",
            "ViewModel",
            "."
        };

        bool suffixRemoved;
        do
        {
            suffixRemoved = false;
            foreach (var suffix in suffixes)
            {
                if (span.EndsWith(suffix, StringComparison.Ordinal))
                {
                    span = span[..^suffix.Length];
                    suffixRemoved = true;
                    break;
                }
            }
        } while (suffixRemoved);

        return span.ToString();
    }
}