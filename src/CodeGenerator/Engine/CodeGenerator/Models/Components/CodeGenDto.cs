using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using HanyCo.Infra.Markers;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components;

[Fluent]
public sealed record CodeGenDto : CodeGenType, ISupportCommenting
{
    private CodeGenDto(
        in string fullName,
        CodeGenType? baseClass = null,
        IEnumerable<CodeGenProp>? props = null)
        : base(in fullName, baseClass, props)
    {
    }

    public string? Comment { get; set; }

    public static CodeGenDto New(
        in string fullName,
        in CodeGenType? baseClass = null,
        in IEnumerable<CodeGenProp>? props = null)
        => new(in fullName, baseClass ?? new CodeGenType(typeof(IDto)), props);
}
