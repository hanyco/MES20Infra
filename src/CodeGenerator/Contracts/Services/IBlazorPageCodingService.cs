using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;

namespace Contracts.Services;

public interface IBlazorPageCodingService : IBusinessService, ICodeGenerator<UiPageViewModel, GenerateCodesArgs>
{
}