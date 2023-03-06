using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using HanyCo.Infra.Cqrs;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Queries;

[Fluent]
public sealed record CodeGenQueryParam : CodeGenCqrsSegregateType
{
    private CodeGenQueryParam(IEnumerable<CodeGenProp>? props = null)
        : base("Parameter", null, props)
    {
    }

    public override SegregationRole Role { get; } = SegregationRole.QueryParameter;

    public static CodeGenQueryParam New(IEnumerable<CodeGenProp>? props = null)
        => new(props);

    protected override IEnumerable<string> OnGetRequiredIntefaces(string cqrsName)
    { yield return $"{typeof(IQueryParameter).FullName}<{cqrsName}Result>"; }
}