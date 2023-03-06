namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;

public interface ICodeGenCqrsSegregate : ICanInherit, IPropertyContainer, ISupportCommenting, ISupportsPartiality
{
    bool HasPartialClass { get; set; }

    SegregationRole Role { get; }

    string Suffix { get; }

    IEnumerable<string> GetIntefaces(string cqrsName);
}
