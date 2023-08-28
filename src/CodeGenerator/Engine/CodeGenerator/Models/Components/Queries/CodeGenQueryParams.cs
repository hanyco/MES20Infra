using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using HanyCo.Infra.Cqrs;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Queries;

[Fluent]
public sealed record CodeGenQueryParams : CodeGenCqrsSegregateType
{
    private CodeGenQueryParams(IEnumerable<CodeGenProp>? props = null)
        : base("Parameter", null, props)
    {
    }

    public override SegregationRole Role { get; } = SegregationRole.QueryParameter;

    public static CodeGenQueryParams New(IEnumerable<CodeGenProp>? props = null)
        => new(props);

    protected override IEnumerable<string> OnGetRequiredInterfaces(string cqrsName)
    { yield return $"{typeof(IQueryParameter).FullName}<{cqrsName}Result>"; }

    public override string ToString() => 
        this.FullName;
}