using Library.CodeGeneration.Models;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models;

public readonly struct GenerateCodeResult(in Code? main, in Code? partial)
{
    public Code? Main { get; } = main;
    public Code? Partial { get; } = partial;

    public static bool operator !=(GenerateCodeResult left, GenerateCodeResult right)
    {
        return !(left == right);
    }

    public static bool operator ==(GenerateCodeResult left, GenerateCodeResult right)
    {
        return left.Equals(right);
    }

    public void Deconstruct(out Code? main, out Code? partial)
                => (main, partial) = (this.Main, this.Partial);

    public override bool Equals(object obj)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// This class is used to hold parameters for code generation.
/// </summary>
[Immutable]
public record GenerateCodesParameters(
    bool GenerateMainCode = true,
    bool GeneratePartialCode = true,
    bool GenerateUiCode = true,
    in string? BackendFileName = null,
    in string? FrontFileName = null,
    bool IsEditForm = false,
    IEnumerable<(string Key, string Value)>? EditFormAttributes = null)
{
    /// <summary>
    /// Factory method to create a new instance of GenerateCodesParameters with all the code generating flags set to true.
    /// </summary>
    public static GenerateCodesParameters FullCode() =>
        new(true, true, true);
}