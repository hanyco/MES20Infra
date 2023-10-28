using System.Diagnostics.CodeAnalysis;

using Contracts.ViewModels;

using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration;

namespace Services.Helpers;

internal static class CommonHelpers
{
    public static string GetCqrsInOutName(CqrsQueryViewModel model, string kind, string part) =>
        $"{Purify(model.Name)}{kind}{part}";

    public static TypePath GetHandlerClassName(CqrsQueryViewModel model, string kind) =>
        TypePath.New($"{Purify(model.Name)}{kind}Handler", model.CqrsNameSpace);

    public static TypePath GetParamsType(CqrsQueryViewModel model, string kind) =>
        TypePath.New($"{Purify(model.ParamsDto.Name)}{kind}Params", model.ParamsDto.NameSpace);

    public static string GetQueryClassName(DtoViewModel model, string kind, string part) =>
        $"{Purify(model.Name)}{kind}{part}";

    public static TypePath GetParamsParam(CqrsQueryViewModel model) =>
        TypePath.New($"{Purify(model.ParamsDto.Name)}Params", model.ParamsDto.NameSpace);

    public static TypePath GetResultParam(CqrsQueryViewModel model) =>
        TypePath.New($"{Purify(model.ResultDto.Name)}Result", model.ResultDto.NameSpace);

    public static TypePath GetResultType(CqrsQueryViewModel model, string kind) =>
        TypePath.New($"{Purify(model.ResultDto.Name)}{kind}Result", model.ResultDto.NameSpace);

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

    [return: NotNull]
    public static IEnumerable<string> ToSecurityKeys(this IEnumerable<ClaimViewModel> claims) =>
        claims.Select(x => x.Key).Compact();
}