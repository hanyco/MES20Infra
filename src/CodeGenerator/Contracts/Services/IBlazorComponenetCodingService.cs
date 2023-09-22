using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.CodeGeneration.Models;
using Library.Interfaces;
using Library.Results;

namespace Contracts.Services;

public interface IBlazorComponentCodingService : IBusinessService, ICodeGenerator<UiViewModel, GenerateCodesArgs>
{
    bool ControlTypeHasPropertiesPage(ControlType controlType);

    bool HasPropertiesPage(ControlType? ct);
}