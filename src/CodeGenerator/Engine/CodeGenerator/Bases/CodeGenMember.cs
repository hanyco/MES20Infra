using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;

[Immutable]
public abstract class CodeGenMember : ICodeGenMember, ISupportCommenting
{
    protected CodeGenMember(in string name)
        => this.Name = name;

    public string? Comment { get; set; }

    public string Name { get; }
}
