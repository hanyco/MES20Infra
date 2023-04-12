﻿using Library.CodeGeneration.Models;
using Library.Results;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

public interface IBehindCodeGenerator
{
    GenerateCodeResult GenerateBehindCode(in GenerateCodesParameters? arguments);
}

public interface ICodeGenerator
{
    Codes GenerateCodes(in GenerateCodesParameters? arguments = null);
}

public interface ICodeGeneratorUnit
{
    Code GenerateCode(in GenerateCodesParameters? arguments = null);
}

public interface ICommandCqrsSergregation : ICqrsSegregation
{
}

public interface IComponentCodeUnit : IUiCodeGenerator, IBehindCodeGenerator
{
}

public interface ICqrsSegregation
{
    string Name { get; }

    MethodArgument? Parameter { get; }

    MethodArgument? Result { get; }
}

public interface IQueryCqrsSergregation : ICqrsSegregation
{
}

public interface ISupportsBehindCodeMember
{
    IEnumerable<GenerateCodeTypeMemberResult> GenerateTypeMembers(GenerateCodesParameters arguments);
}

public interface IUiCodeGenerator
{
    Code GenerateUiCode(in GenerateCodesParameters? arguments = null);
}

public interface ICodeGeneratorService<TViewModel>
{
    Result<Codes> GenerateCodes(in TViewModel viewModel, GenerateCodesParameters? arguments = null);
}

public interface IAsyncCodeGeneratorService<TViewModel>
{
    Task<Result<Codes>> GenerateCodesAsync(TViewModel viewModel, GenerateCodesParameters? arguments = null, CancellationToken token = default);
}