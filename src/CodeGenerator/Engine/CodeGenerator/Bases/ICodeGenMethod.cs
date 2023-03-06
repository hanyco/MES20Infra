namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;

public interface ICodeGenMethod : ICodeGenMember
{
    IEnumerable<(ICodeGenType Type, string Name)> Parameters { get; }
    ICodeGenType ReturnType { get; }
}
