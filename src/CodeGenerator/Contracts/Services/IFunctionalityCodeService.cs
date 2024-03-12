using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Contracts.ViewModels;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.DesignPatterns.Markers;
using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Contracts.CodeGen.Services;

public interface IFunctionalityCodeService : IBusinessService, ICodeGenerator<FunctionalityViewModel, FunctionalityCodeServiceAsyncCodeGeneratorArgs>
{
    Task<Result<FunctionalityViewModel?>> GenerateViewModelAsync([DisallowNull] FunctionalityViewModel model, CancellationToken token = default);
}

public sealed record FunctionalityCodeServiceAsyncCodeGeneratorArgs(bool UpdateModelView, CancellationToken Token = default);