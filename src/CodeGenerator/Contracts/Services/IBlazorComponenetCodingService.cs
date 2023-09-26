using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Interfaces;

namespace Contracts.Services;

public interface IBlazorComponentCodingService : IBusinessService, ICodeGenerator<UiComponentViewModel>
{
    bool ControlTypeHasPropertiesPage(ControlType controlType);

    bool HasPropertiesPage(ControlType? ct);
}