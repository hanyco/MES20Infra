using HanyCo.Infra.CodeGen.Contracts.ViewModels;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;
using Library.Validations;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface IBlazorPageCodeService : IBusinessService, ICodeGenerator<UiPageViewModel, GenerateCodesParameters>, IValidator<UiPageViewModel>
{
}