using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;

using Library.CodeGeneration;

using static Library.Helpers.CodeGen.TypeMemberNameHelper;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;

public record CodeGenType : ICanInherit, IPropertyContainer, IAttributeContainer, ICodeGenType
{
    public CodeGenType(
        in string fullName,
        in CodeGenType? baseClass = null,
        in IEnumerable<CodeGenProp>? props = null,
        in IEnumerable<CodeGenAttr>? attributes = null)
    {
        if (props is not null)
        {
            _ = this.Properties.AddRange(props);
        }
        if (attributes is not null && attributes.Any())
        {
            _ = this.Attributes.AddRange(attributes);
        }
        this.FullName = fullName;
        this.BaseClass = baseClass;
    }

    public CodeGenType(in Type primitiveType)
        : this(primitiveType.FullName!)
    {
    }

    public string FullName { get; }

    public string Name => TypePath.GetName(this.FullName);
    public IEnumerable<string> Namespaces => TypePath.GetNameSpaces(this.FullName);

    public IList<CodeGenAttr> Attributes { get; } = new List<CodeGenAttr>();
    public IList<CodeGenProp> Properties { get; } = new List<CodeGenProp>();
    public IList<CodeGenType> Interfaces { get; } = new List<CodeGenType>();
    public CodeGenType? BaseClass { get; set; }

    public static CodeGenType New(
        in string fullName,
        in CodeGenType? baseClass = null,
        in bool isNullable = false,
        in bool isList = false,
        in IEnumerable<CodeGenProp>? props = null)
        => new(in fullName, baseClass, props);
    public static CodeGenType New(in Type primitiveType) =>
        new(primitiveType);

    public override string ToString() =>
        this.FullName;
}