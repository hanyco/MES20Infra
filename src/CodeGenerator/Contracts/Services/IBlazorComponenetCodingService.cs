using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Interfaces;

namespace Contracts.Services;

public interface IBlazorComponentCodingService : IBusinessService, ICodeGenerator<UiComponentViewModel, GenerateCodesParameters>
{
    bool ControlTypeHasPropertiesPage(ControlType controlType);

    bool HasPropertiesPage(ControlType? ct);
}