using Contracts.ViewModels;

using Library.CodeGeneration;

using static Services.Helpers.CommonHelpers;

namespace Services.Helpers;

internal static class CqrsHelpers
{
    // GetAllPeopleQueryHandle
    public static TypePath GetSegregateHandlerType(this CqrsViewModelBase model, string kind) =>
        TypePath.New($"{Purify(model.Name)}{kind}Handler", model.CqrsNameSpace);

    // GetAllPeopleQuery
    public static TypePath GetSegregateType(this CqrsViewModelBase model, string kind) =>
        TypePath.New($"{Purify(model.Name)}{kind}", model.DtoNameSpace);

    // GetAllPeople
    public static TypePath GetSegregateParamsType(this CqrsViewModelBase model, string? kind) =>
        TypePath.New($"{model.ParamsDto.Name}", model.ParamsDto.NameSpace);

    // GetAllPeopleQueryResult
    public static TypePath GetSegregateResultType(this CqrsViewModelBase model, string kind) =>
        TypePath.New($"{Purify(model.Name)}{kind}Result", model.DtoNameSpace);

    // GetAllPeopleResult
    public static TypePath GetSegregateResultParamsType(this CqrsViewModelBase model, string? kind) =>
        TypePath.New($"{model.ResultDto.Name}", model.ResultDto.NameSpace);
}