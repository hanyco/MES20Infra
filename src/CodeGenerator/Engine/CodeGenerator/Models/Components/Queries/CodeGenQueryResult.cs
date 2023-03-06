using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using HanyCo.Infra.Cqrs;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Queries;

[Fluent]
public sealed record CodeGenQueryResult : CodeGenCqrsSegregateType
{
    private CodeGenQueryResult(IEnumerable<CodeGenProp>? props = null)
        : base("Result", null, props)
    {
    }

    public override SegregationRole Role { get; } = SegregationRole.QueryResult;

    public static CodeGenQueryResult New(IEnumerable<CodeGenProp>? props = null)
        => new(props);

    protected override IEnumerable<string> OnGetRequiredIntefaces(string cqrsName)
    {
        var prop = this.As<IPropertyContainer>()!.Properties.FirstOrDefault(p => p.Name.EqualsTo("result"));
        string result;
        if (prop is null)
        {
            result = typeof(IQueryResult).FullName!;
        }
        else
        {
            var propType = prop.Type.FullName;
            if (prop.IsList)
            {
                propType = $"IEnumerable<{propType}>";
            }

            if (prop.IsNullable)
            {
                propType = $"{propType}?";
            }

            result = $"{typeof(IQueryResult).FullName!}<{propType}>";
        }

        yield return result;
    }
}