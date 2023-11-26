using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.CodeGeneration;
using Library.Interfaces;

namespace Contracts.Services;

public interface IMapperSourceGenerator : IBusinessService, ICodeGenerator<MapperSourceGeneratorArguments>
{
}

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed record MapperSourceGeneratorArguments(
    [DisallowNull] in (DtoViewModel Model, TypePath? Type) Source,
    [DisallowNull] in (DtoViewModel Model, TypePath? Type) Destination,
    [DisallowNull] in string NameSpace,

    in string ClassName = "ModelConverter",
    in bool IsPartial = true,

    in string MethodName = "ToViewModel",
    in bool IsExtension = true,
    in string InputArgumentName = "model",
    bool GenerateListConverter = true,

    in string? FileName = null)
{
    public MapperSourceGeneratorArguments(
        [DisallowNull] in DtoViewModel sourceModel,
        [DisallowNull] in DtoViewModel destinationModel,
        [DisallowNull] in string nameSpace,
        in string? fileName = null,
        in string className = "ModelConverter",
        in bool isPartial = true,
        in string methodName = "ToViewModel",
        in string inputArgumentName = "model",
        in bool isExtension = true,
        bool generateListConverter = true)
        : this((sourceModel, null), (destinationModel, null), nameSpace, className, isPartial, methodName, isExtension, inputArgumentName, generateListConverter, fileName)
    {
    }

    public override string ToString() =>
        $"Map `{this.Source.Type}` to `{this.Destination.Type}`";

    private string GetDebuggerDisplay() =>
        this.ToString();
}