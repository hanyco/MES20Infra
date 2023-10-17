using System.Diagnostics;

using Library.CodeGeneration.Models;
using Library.DesignPatterns.Markers;
using Library.Results;

namespace Contracts.ViewModels;

[Immutable]
[StackTraceHidden]
[DebuggerStepThrough]
public readonly record struct CodeGeneratorResult(Result<Codes> Result)
{
    public Codes Codes => this.Result.Value;

    public void Deconstruct(out Result<Codes> result, out Codes codes) =>
        (result, codes) = (this.Result, this.Result.Value);

    public Codes ToCodes() =>
        this;

    public Result<Codes> ToResult() =>
        this;

    public static implicit operator Codes(CodeGeneratorResult result) =>
        result.Codes;

    public static implicit operator CodeGeneratorResult(Codes codes) =>
        new(new(codes));

    public static implicit operator CodeGeneratorResult(Result<Codes> result) =>
        new(result);

    //! Not functional. Has side-effect
    //x public static implicit operator CodeGeneratorResult(Result<Code> result) =>
    //x     new(result.WithValue(result.Value.ToCodes()));

    public static implicit operator CodeGeneratorResult(Code code) =>
        new(new(code.ToCodes()));

    public static implicit operator Result<Codes>(CodeGeneratorResult result) =>
        result.Result;

    public CodeGeneratorResult ToCodeGeneratorResult() =>
        this;
}