using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;

public interface ICodeGenCqrsModel
{
    string? CqrsNameSpace { get; }
    string? DtoNameSpace { get; }
    IEnumerable<CodeGenDto> Dtos { get; }
    string Name { get; }
    IEnumerable<ICodeGenCqrsSegregate> Segregates { get; }
}
