using Library.CodeGeneration.Models;
using Library.Results;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

public interface IBehindCodeGenerator
{
    GenerateCodeResult GenerateBehindCode(in GenerateCodesParameters? arguments);
}

public interface ICodeGenerator
{
    /// <summary>
    /// Generates codes based on the given parameters.
    /// </summary>
    /// <param name="arguments">The parameters used to generate the codes.</param>
    /// <returns>The generated codes.</returns>
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
    /// <summary>
    /// Generates UI code based on the given parameters.
    /// </summary>
    /// <param name="arguments">Optional parameters for code generation.</param>
    /// <returns>Generated UI code.</returns>
    Code GenerateUiCode(in GenerateCodesParameters? arguments = null);
}

public interface ICodeGeneratorService<TViewModel>
{
    /// <summary>
    /// Generates codes based on the given view model and optional parameters.
    /// </summary>
    /// <param name="viewModel">The view model to generate codes from.</param>
    /// <param name="arguments">Optional parameters for generating codes.</param>
    /// <returns>A result containing the generated codes.</returns>
    Result<Codes> GenerateCodes(in TViewModel viewModel, GenerateCodesParameters? arguments = null);
}

public interface IAsyncCodeGeneratorService<TViewModel>
{
    /// <summary>
    /// Generates codes asynchronously based on the given view model and optional parameters.
    /// </summary>
    Task<IEnumerable<Result<Codes>>> GenerateCodesAsync(TViewModel viewModel, GenerateCodesParameters? arguments = null, CancellationToken token = default);
}