using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.DesignPatterns.Markers;
using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface IDtoCodeService : IBusinessService
    , ICodeGenerator<DtoViewModel, DtoCodeServiceAsyncCodeGeneratorArgs>
{

}

[Immutable]
public sealed record DtoCodeServiceAsyncCodeGeneratorArgs(string? TypeName = null,
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
    in string? FrontFileName = null) : GenerateCodesParameters(GenerateMainCode, GeneratePartialCode, GenerateUiCode, BackendFileName, FrontFileName);
