using Library.Validations;

using Microsoft.Extensions.Configuration;

using System.Diagnostics.CodeAnalysis;

namespace Services.Helpers;

public static class ConfigurationHelper
{
    [return: NotNull]
    public static string GetApplicationConnectionString([DisallowNull] this IConfiguration configuration) =>
        configuration
        .ArgumentNotNull()
        .GetConnectionString("ApplicationConnectionString")
        .NotNull();
}