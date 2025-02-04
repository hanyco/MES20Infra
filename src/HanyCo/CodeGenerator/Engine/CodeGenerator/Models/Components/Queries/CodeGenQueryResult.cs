﻿using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using HanyCo.Infra.Cqrs;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Queries;

[Fluent]
public sealed record CodeGenQueryResult : CodeGenCqrsSegregateType
{
    private CodeGenQueryResult(IEnumerable<string>? securityKeys,IEnumerable<CodeGenProp>? props = null)
        : base("Result", securityKeys, null, props)
    {
    }

    public override SegregationRole Role { get; } = SegregationRole.QueryResult;

    public static CodeGenQueryResult New(IEnumerable<string>? securityKeys, IEnumerable<CodeGenProp>? props = null)
        => new(securityKeys, props);

    protected override IEnumerable<string> OnGetRequiredInterfaces(string cqrsName)
    {
        var prop = this.Cast().As<IPropertyContainer>()!.Properties.FirstOrDefault(p => p.Name.EqualsTo("result"));
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
                propType = $"List<{propType}>";
            }

            if (prop.IsNullable)
            {
                propType = $"{propType}?";
            }

            result = $"{typeof(IQueryResult).FullName!}<{propType}>";
        }

        yield return result;
    }

    public override string ToString() =>
        this.FullName;
}