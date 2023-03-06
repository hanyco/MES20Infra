using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;

public interface ICanInherit
{
    CodeGenType? BaseClass { get; set; }

    IList<CodeGenType> Interfaces { get; }
}
