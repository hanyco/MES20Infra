using Contracts.ViewModels;

using Library.CodeGeneration;
using Library.Wpf.Bases;

using static Services.Helpers.CommonHelpers;

namespace Services.Helpers;

internal static class CqrsHelpers
{
    public static TypePath GetHandlerClass(this CqrsViewModelBase model, string kind) =>
    TypePath.New($"{Purify(model.Name)}{kind}Handler", model.CqrsNameSpace);

    public static TypePath GetParamsParam(this CqrsViewModelBase model) =>
        TypePath.New($"{Purify(model.ParamsDto.Name)}Params", model.ParamsDto.NameSpace);

    public static TypePath GetParamsType(this CqrsViewModelBase model, string kind) =>
            TypePath.New($"{Purify(model.ParamsDto.Name)}{kind}Params", model.ParamsDto.NameSpace);

    public static TypePath GetResultParam(this CqrsViewModelBase model) =>
        TypePath.New($"{Purify(model.ResultDto.Name)}Result", model.ResultDto.NameSpace);

    public static TypePath GetResultType(this CqrsViewModelBase model, string kind) =>
        TypePath.New($"{Purify(model.ResultDto.Name)}{kind}Result", model.ResultDto.NameSpace);

    public static string GetRevertedDtoName(this DtoViewModel model) =>
        $"{model.DbObject.Name}Dto";

    public static IEnumerable<string> GetSecurityKeys(this CqrsViewModelBase viewModel) =>
        viewModel.SecurityClaims.Select(x => x.Key).Compact() ?? Enumerable.Empty<string>();

    public static string GetSegregateClassName(this DtoViewModel model, string? kind, string part) =>
        $"{Purify(model.Name)}{kind}{part}";
}