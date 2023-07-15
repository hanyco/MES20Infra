using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.DesignPatterns.Markers;
using Library.Interfaces;

namespace Contracts.Services;

public interface IFunctionalityCodeService : IBusinessService, IAsyncCodeGenerator<FunctionalityViewModel, FunctionalityCodeServiceAsyncCodeGeneratorArgs>
{
}

[Immutable]
public sealed class FunctionalityCodeServiceAsyncCodeGeneratorArgs(bool updateModelView)
{
    public bool UpdateModelView { get; } = updateModelView;
}