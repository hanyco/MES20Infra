using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;

public interface IMethodContainer
{
    List<CodeGenMethod> Methods { get; }
}
