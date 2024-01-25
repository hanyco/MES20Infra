using HanyCo.Infra.CodeGen.Contracts.ViewModels;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.DesignPatterns.Markers;
using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface IFunctionalityCodeService : IBusinessService, ICodeGenerator<FunctionalityViewModel, FunctionalityCodeServiceAsyncCodeGeneratorArgs>
{
}

[Immutable]
public sealed class FunctionalityCodeServiceAsyncCodeGeneratorArgs(bool updateModelView, CancellationToken token = default)
{
    public bool UpdateModelView { get; } = updateModelView;
    public CancellationToken Token { get; set; } = token;
}