namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;

[Fluent]
public sealed class CodeGenProp : CodeGenMember
{
    private CodeGenProp(in CodeGenType type, in string name, bool isList = false, bool isNullable = false, bool hasGetter = true, in bool hasSetter = true,
        string? comment = null)
        : base(name)
    {
        this.Type = type;
        this.IsList = isList;
        this.IsNullable = isNullable;
        this.HasGetter = hasGetter;
        this.HasSetter = hasSetter;
        this.Comment = comment;
    }

    public bool HasGetter { get; }

    public bool HasSetter { get; }

    public bool IsList { get; }

    public bool IsNullable { get; }

    public CodeGenType Type { get; }

    public static CodeGenProp New(in CodeGenType type, in string name, bool isList = false, bool isNullable = false, bool hasGetter = true, in bool hasSetter = true,
        string? comment = null)
        => new(type, name, isList, isNullable, hasGetter, hasSetter, comment);

    public override string ToString() => 
        this.Name;
}
