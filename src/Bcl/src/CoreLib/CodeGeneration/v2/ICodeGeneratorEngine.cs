﻿using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2.Back;
using Library.Results;

namespace Library.CodeGeneration.v2;

public interface ICodeGeneratorEngine
{
    Result<string> Generate(INamespace nameSpace);
}

public static class CodeGeneratorExtensions
{
    public static Result<Code> Generate(this ICodeGeneratorEngine codeGenerator, INamespace nameSpace, [DisallowNull] in string name, [DisallowNull] Language language, bool isPartial)
    {
        var genResult = codeGenerator.Generate(nameSpace);
        var code = new Code(name, language, genResult, isPartial);
        return Result<Code>.From(genResult, code);
    }
}