namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;

public sealed class CodeGenMethod : CodeGenMember, ICodeGenMethod
{
    public CodeGenMethod(in string name, in CodeGenType returnType, params (ICodeGenType Type, string Name)[] parameters)
        : base(name)
    {
        this.ReturnType = returnType;
        this.Parameters = parameters;
    }

    public IEnumerable<(ICodeGenType Type, string Name)> Parameters { get; }
    public ICodeGenType ReturnType { get; }
}
