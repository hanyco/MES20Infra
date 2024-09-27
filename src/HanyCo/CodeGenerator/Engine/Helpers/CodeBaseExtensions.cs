using HanyCo.Infra.CodeGeneration.Definitions;

namespace HanyCo.Infra.CodeGeneration.Helpers;

public static class CodeBaseExtensions
{
    public static TCodeBase AddUsings<TCodeBase>(this TCodeBase codeBase, params IEnumerable<string> usings)
        where TCodeBase : ICodeBase
    {
        _ = codeBase.AdditionalUsings.AddRange(usings);
        return codeBase;
    }
}