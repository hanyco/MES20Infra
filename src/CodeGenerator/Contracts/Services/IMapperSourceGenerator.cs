using System.Diagnostics.CodeAnalysis;

using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.CodeGeneration;
using Library.Interfaces;

namespace Contracts.Services;

public interface IMapperSourceGenerator : IBusinessService, ICodeGenerator<MapperSourceGeneratorArguments>
{
}

public sealed record MapperSourceGeneratorArguments(
    [DisallowNull] in (DtoViewModel Model, TypePath? Type) Source,
    [DisallowNull] in (DtoViewModel Model, TypePath? Type) Destination,
    [DisallowNull] in string DtoNameSpace,
    in string? FileName = null,

    in string ClassName = "ModelConverter",
    in bool IsPartial = true,

    in string MethodName = "ToViewModel",
    in string InputArgumentName = "model",
    in bool IsExtension = true,

    bool GenerateListConverter = true)
{
    public static MapperSourceGeneratorArguments New(
        [DisallowNull] in (DtoViewModel Model, TypePath? Type) source,
        [DisallowNull] in (DtoViewModel Model, TypePath? Type) destination,
        [DisallowNull] in string dtoNameSpace,
        in string? fileName = null,
        in string className = "ModelConverter",
        in bool isPartial = true,
        in string methodName = "ToViewModel",
        in string inputArgumentName = "model",
        in bool isExtension = true,
        bool generateListConverter = true) =>
        new(source, destination, dtoNameSpace, fileName, className, isPartial, methodName, inputArgumentName, isExtension, generateListConverter);
    public static MapperSourceGeneratorArguments New(
        [DisallowNull] in DtoViewModel sourceModel,
        [DisallowNull] in DtoViewModel destinationModel,
        [DisallowNull] in string dtoNameSpace,
        in string? fileName = null,
        in string className = "ModelConverter",
        in bool isPartial = true,
        in string methodName = "ToViewModel",
        in string inputArgumentName = "model",
        in bool isExtension = true,
        bool generateListConverter = true) =>
        New((sourceModel, null), (destinationModel, null), dtoNameSpace, fileName, className, isPartial, methodName, inputArgumentName, isExtension, generateListConverter);
}