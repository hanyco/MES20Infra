using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using HanyCo.Infra.Cqrs;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Queries;

[Fluent]
public sealed record CodeGenQueryParams : CodeGenCqrsSegregateType
{
    private CodeGenQueryParams(IEnumerable<string>? securityKeys, IEnumerable<CodeGenProp>? props = null)
        : base("Parameter", securityKeys, null, props)
    {
    }

    public override SegregationRole Role { get; } = SegregationRole.QueryParameter;

    public static CodeGenQueryParams New(IEnumerable<string>? securityKeys, IEnumerable<CodeGenProp>? props = null)
        => new(securityKeys, props);

    [Obsolete]
    protected override IEnumerable<string> OnGetRequiredInterfaces(string cqrsName)
    { yield return $"{typeof(IQueryParameter).FullName}<{cqrsName}Result>"; }

    public override string ToString() =>
        this.FullName;
}