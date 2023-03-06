namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;

public record CodeGenAttr : CodeGenType
{
    public CodeGenAttr(in Type primitiveType)
        : base(primitiveType)
    {
    }

    public CodeGenAttr(in string fullName, in CodeGenType? baseClass = null, in IEnumerable<CodeGenProp>? props = null, in IEnumerable<CodeGenAttr>? attributes = null)
        : base(fullName, baseClass, props, attributes)
    {
    }

    protected CodeGenAttr(CodeGenType original)
        : base(original)
    {
    }
}
