using Library.CodeGeneration.Models;
using Library.Results;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Bases
{
    /// <summary>
    /// Represents an interface for generating code based on provided parameters.
    /// </summary>
    public interface IBehindCodeGenerator
    {
        GenerateCodeResult GenerateBehindCode(in GenerateCodesParameters? arguments);
    }

    /// <summary>
    /// Represents an interface for generating codes based on provided parameters.
    /// </summary>
    public interface ICodeGenerator
    {
        /// <summary>
        /// Generates codes based on the given parameters.
        /// </summary>
        /// <param name="arguments">The parameters used to generate the codes.</param>
        /// <returns>The generated codes.</returns>
        Codes GenerateCodes(in GenerateCodesParameters? arguments = null);
    }

    /// <summary>
    /// Represents an interface for generating codes based on a specific view model and optional parameters.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public interface ICodeGenerator<TViewModel>
    {
        /// <summary>
        /// Generates codes based on the given view model and optional parameters.
        /// </summary>
        /// <param name="viewModel">The view model to generate codes from.</param>
        /// <param name="arguments">Optional parameters for generating codes.</param>
        /// <returns>A result containing the generated codes.</returns>
        Result<Codes> GenerateCodes(TViewModel viewModel, GenerateCodesParameters? arguments = null);
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
        IEnumerable<GenerateCodeTypeMemberResult> GenerateTypeMembers(GenerateCodesParameters arguments);
    }

    /// <summary>
    /// Represents an interface for generating UI code.
    /// </summary>
    public interface IUiCodeGenerator
    {
        /// <summary>
        /// Generates UI code based on the given parameters.
        /// </summary>
        /// <param name="arguments">Optional parameters for code generation.</param>
        /// <returns>Generated UI code.</returns>
        Code GenerateUiCode(in GenerateCodesParameters? arguments = null);
    }

    /// <summary>
    /// Represents an interface for asynchronously generating codes based on a view model and arguments.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TArgs">The type of the arguments.</typeparam>
    public interface IAsyncCodeGenerator<in TViewModel, in TArgs>
    {
        /// <summary>
        /// Generates codes asynchronously based on the given view model and optional parameters.
        /// </summary>
        /// <param name="viewModel">
        /// The ViewModel instance.
        /// </param>
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
    public interface IAsyncCodeGenerator<in TViewModel>
    {
        /// <summary>
        /// Generates codes asynchronously based on the given view model and optional parameters.
        /// </summary>
        Task<Result<Codes>> GenerateCodesAsync(TViewModel viewModel, CancellationToken token = default);
    }
}
