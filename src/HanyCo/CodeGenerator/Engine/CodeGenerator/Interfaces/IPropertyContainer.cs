using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;

public interface IPropertyContainer
{
    IList<CodeGenProp> Properties { get; }
}
