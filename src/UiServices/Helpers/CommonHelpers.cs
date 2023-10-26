using System.Diagnostics.CodeAnalysis;

using Contracts.ViewModels;

using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration;

namespace Services.Helpers;

internal static class CommonHelpers
{
    public static TypePath GetHandlerClassName(CqrsQueryViewModel model) =>
        TypePath.New($"{model.Name?.TrimEnd("Query")}QueryHandler", model.CqrsNameSpace);

    public static TypePath GetParamsType(CqrsQueryViewModel model) =>
        TypePath.New(model.ParamsDto.Name, model.ParamsDto.NameSpace);

    public static TypePath GetResultParam(CqrsQueryViewModel model) =>
        TypePath.New($"{Purify(model.ResultDto.Name)}Result", model.ResultDto.NameSpace);

    public static TypePath GetResultType(CqrsQueryViewModel model) =>
        TypePath.New($"{Purify(model.ResultDto.Name)}QueryResult", model.ResultDto.NameSpace);

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