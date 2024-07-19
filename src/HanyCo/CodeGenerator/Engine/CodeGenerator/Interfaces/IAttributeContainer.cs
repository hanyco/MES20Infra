using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;

public interface IAttributeContainer
{
    IList<CodeGenAttr> Attributes { get; }
}
