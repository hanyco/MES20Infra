using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;

using Library.CodeGeneration;

using static Services.CodeGen.Helpers.CommonHelpers;

namespace Services.Helpers;

internal static class CqrsHelpers
{
    // GetAllPeopleQueryHandler
    public static TypePath GetSegregateHandlerType(this CqrsViewModelBase model) =>
        TypePath.New(model.Name!.EndsWith("Handler") ? model.Name : $"{model.Name}Handler", model.CqrsNameSpace);

    // GetAllPeopleQuery
    public static TypePath GetSegregateParamsType(this CqrsViewModelBase model, string? kind) =>
        TypePath.New(model.ParamsDto.Name!, model.ParamsDto.NameSpace ?? model.CqrsNameSpace);

    // GetAllPeopleQueryResult
    public static TypePath GetSegregateResultParamsType(this CqrsViewModelBase model, string? kind) =>
        TypePath.New(model.ResultDto.Name!, model.ResultDto.NameSpace ?? model.CqrsNameSpace);

    // GetAllPeopleQueryResult
    public static TypePath GetSegregateResultType(this CqrsViewModelBase model, string kind) =>
        TypePath.New($"{Purify(model.Name)}{kind}Result", model.DtoNameSpace ?? model.ResultDto?.NameSpace);

    // GetAllPeopleQuery
    public static TypePath GetSegregateType(this CqrsViewModelBase model, string kind) =>
        TypePath.New($"{Purify(model.Name)}{kind}", model.DtoNameSpace ?? model.ParamsDto?.NameSpace);

    public static TypePath GetSegregateValidatorType(this CqrsViewModelBase model, string kind) =>
            TypePath.New($"{Purify(model.Name)}{kind}Validator", model.DtoNameSpace);

    // PersonDto
    public static TypePath GetSourceDtoType(this DtoViewModel dto) =>
        TypePath.New(dto.Name!, dto.NameSpace);
}