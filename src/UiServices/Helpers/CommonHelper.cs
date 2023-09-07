namespace Services.Helpers;

internal class CommonHelper
{
    public static string? Purify(string? name) =>
        name?.TrimEnd(".")
             .TrimEnd("Dto")
             .TrimEnd("Params")
             .TrimEnd("Result")
             .TrimEnd("ViewModel")
             .TrimEnd("ViewModels")
             .TrimEnd("Dto")
             .TrimEnd(".");
}