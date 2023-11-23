using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;

using Library.CodeGeneration.Models;
using Library.Results;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

/// <summary>
/// Represents an interface for asynchronously generating codes based on a view model and arguments.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <typeparam name="TArgs">The type of the arguments.</typeparam>
[Obsolete("Violates the SRP rule in SOLD.", true)]
public interface IAsyncCodeGenerator<in TViewModel, in TArgs>
{
    /// <summary>
    /// Generates codes asynchronously based on the given view model and optional parameters.
    /// </summary>
    /// <param name="viewModel">The ViewModel instance.</param>
    /// <param name="args">Optional arguments for code generation.</param>
    /// <param name="token">Cancellation token to cancel the code generation process.</param>
    /// <returns>
    /// A Result containing the generated codes or a failure message if no codes were generated.
    /// </returns>
    Task<Result<Codes>> GenerateCodesAsync(TViewModel viewModel, TArgs? arguments = default, CancellationToken token = default);
}

/// <summary>
/// Represents an interface for asynchronously generating codes based on a view model.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
[Obsolete("Violates the SRP rule in SOLD.", true)]
public interface IAsyncCodeGenerator<in TViewModel>
{
    /// <summary>
    /// Generates codes asynchronously based on the given view model and optional parameters.
    /// </summary>
    Task<Result<Codes>> GenerateCodesAsync(TViewModel viewModel, CancellationToken token = default);
}

/// <summary>
/// Represents an interface for generating code based on provided parameters.
/// </summary>
public interface IBehindCodeGenerator
{
    GenerateCodeResult GenerateBehindCode(in GenerateCodesParameters? arguments);
}

/// <summary>
/// Represents an interface for generating codes based on a specific parameter.
/// </summary>
/// <typeparam name="TArguments">The type of the parameter.</typeparam>
public interface ICodeGenerator<in TArguments>
{
    Result<Codes> GenerateCodes([DisallowNull] TArguments args);
}

/// <summary>
/// Represents an interface for generating codes based on a specific view model and optional parameters.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
public interface ICodeGenerator<in TViewModel, in TArguments>
{
    /// <summary>
    /// Generates codes based on the given view model and optional parameters.
    /// </summary>
    /// <param name="viewModel">The view model to generate codes from.</param>
    /// <param name="arguments">Parameters for generating codes.</param>
    /// <returns>A result containing the generated codes.</returns>
    Result<Codes> GenerateCodes(TViewModel viewModel, TArguments? arguments = default);
}

/// <summary>
/// Represents an interface for generating code units.
/// </summary>
public interface ICodeGeneratorUnit
{
    Code GenerateCode(in GenerateCodesParameters? arguments = null);
}

/// <summary>
/// Represents an interface for Command-specific CQRS segregation.
/// </summary>
public interface ICommandCqrsSegregation : ICqrsSegregation
{
}

/// <summary>
/// Represents an interface for generating component code units with UI and behind code.
/// </summary>
public interface IComponentCodeUnit : IUiCodeGenerator, IBehindCodeGenerator
{
}

/// <summary>
/// Represents a common interface for CQRS segregation.
/// </summary>
public interface ICqrsSegregation
{
    string Name { get; }

    MethodArgument? Parameter { get; }

    MethodArgument? Result { get; }
}

/// <summary>
/// Represents an interface for Query-specific CQRS segregation.
/// </summary>
public interface IQueryCqrsSegregation : ICqrsSegregation
{
}

/// <summary>
/// Represents an interface for generating type members supporting behind code.
/// </summary>
public interface ISupportsBehindCodeMember
{
    IEnumerable<CodeTypeMembers> GenerateTypeMembers(GenerateCodesParameters arguments);
}

/// <summary>
/// Represents an interface for generating UI code.
/// </summary>
public interface IUiCodeGenerator : IUiCodeGenerator<GenerateCodesParameters>
{
}

public interface IUiCodeGenerator<in TArgs>
{
    /// <summary>
    /// Generates UI code based on the given parameters.
    /// </summary>
    /// <param name="arguments">Optional parameters for code generation.</param>
    /// <returns>Generated UI code.</returns>
    Code GenerateUiCode(TArgs? arguments = default);
}