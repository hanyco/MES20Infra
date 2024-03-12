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

[Immutable]
public sealed class FunctionalityCodeServiceAsyncCodeGeneratorArgs(bool updateModelView, CancellationToken token = default)
{
    public CancellationToken Token { get; set; } = token;
    public bool UpdateModelView { get; } = updateModelView;
}