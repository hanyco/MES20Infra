using System.Diagnostics.CodeAnalysis;

using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace Contracts.Services;

public interface IMapperSourceGenerator : IBusinessService, ICodeGenerator<MapperSourceGeneratorArguments>
{
}

public sealed record MapperSourceGeneratorArguments(
    [DisallowNull] in DtoViewModel Source,
    [DisallowNull] in DtoViewModel Destination,
    [DisallowNull] in string DtoNameSpace,
    in string? FileName = null,

    in string ClassName = "ModelConverter",
    in bool IsPartial = true,

    in string MethodName = "ToViewModel",
    in string InputArgumentName = "model",
    in bool IsExtension = true);