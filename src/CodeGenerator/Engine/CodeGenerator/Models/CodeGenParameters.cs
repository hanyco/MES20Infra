using Library.CodeGeneration.Models;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models;

[Immutable]
public readonly struct GenerateCodeResult(in Code? main, in Code? partial)
{
    public Code? Main { get; } = main;
    public Code? Partial { get; } = partial;

    public void Deconstruct(out Code? main, out Code? partial)
        => (main, partial) = (this.Main, this.Partial);
}

/// <summary>
/// This class is used to hold parameters for code generation.
/// </summary>
[Immutable]
public record GenerateCodesParameters(
    /// <summary>
    /// A flag indicating whether to generate the main code.
    /// </summary>
    in bool GenerateMainCode = true,

    /// <summary>
    /// A flag indicating whether to generate the partial code.
    /// </summary>
    in bool GeneratePartialCode = true,

    /// <summary>
    /// A flag indicating whether to generate the UI code.
    /// </summary>
    in bool GenerateUiCode = true,

    /// <summary>
    /// The name of the backend file.
    /// </summary>
    in string? BackendFileName = null,

    /// <summary>
    /// The name of the frontend file.
    /// </summary>
    in string? FrontFileName = null)
{
    /// <summary>
    /// Copy constructor. Creates a new instance of GenerateCodesParameters with the same values as
    /// the original.
    /// </summary>
    public GenerateCodesParameters(GenerateCodesParameters original) =>
        (this.GenerateMainCode, this.GeneratePartialCode, this.GenerateUiCode, this.BackendFileName, this.FrontFileName) = original;

    /// <summary>
    /// Factory method to create a new instance of GenerateCodesParameters with all flags set to true.
    /// </summary>
    public static GenerateCodesParameters FullCode() =>
        new(true, true, true);

    /// <summary>
    /// Deconstructs the object into its full parameters.
    /// </summary>
    public void Deconstruct(out bool generateMainCode, out bool generatePartialCode, out bool generateUiCode, out string? backendFileName, out string? frontFileName) =>
        (generateMainCode, generatePartialCode, generateUiCode, backendFileName, frontFileName) = (this.GenerateMainCode, this.GeneratePartialCode, this.GenerateUiCode, this.BackendFileName, this.FrontFileName);

    /// <summary>
    /// Deconstructs the object into its code generation parameters.
    /// </summary>
    public void Deconstruct(out bool generateMainCode, out bool generatePartialCode, out bool generateUiCode) =>
        (generateMainCode, generatePartialCode, generateUiCode) = (this.GenerateMainCode, this.GeneratePartialCode, this.GenerateUiCode);

    /// <summary>
    /// Deconstructs the object into its file name parameters.
    /// </summary>
    public void Deconstruct(out string? backendFileName, out string? frontFileName) =>
        (backendFileName, frontFileName) = (this.BackendFileName, this.FrontFileName);
}