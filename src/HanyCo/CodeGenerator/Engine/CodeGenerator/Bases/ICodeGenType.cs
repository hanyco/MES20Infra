namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;

public interface ICodeGenType
{
    string Name { get; }
    IEnumerable<string> Namespaces { get; }
}
