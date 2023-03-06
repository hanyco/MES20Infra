using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.Markers;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components;

[Fluent]
public sealed record CodeGenSecurityDescriptor : CodeGenType
{
    public CodeGenSecurityDescriptor()
        : base(
            typeof(SecurityDescriptorAttribute).FullName!,
            New(typeof(Attribute)),
            new[] { CodeGenProp.New(New(typeof(Guid)), nameof(SecurityDescriptorAttribute.Guid)) })
    {

    }
}