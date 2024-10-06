using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface IBlazorComponentCodeService : IBusinessService, ICodeGenerator<UiComponentViewModel, GenerateCodesParameters>
{
    bool ControlTypeHasPropertiesPage(ControlType controlType);

    bool HasPropertiesPage(ControlType? ct);
}