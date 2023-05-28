using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Validations;

namespace Contracts.Services;

public interface IBlazorPageService
    : IBusinessService
    , IAsyncCrud<UiPageViewModel>
    , IValidator<UiPageViewModel>
    , ICodeGeneratorService<UiPageViewModel>

{
    UiPageViewModel CreateViewModel(DtoViewModel dto, string? name = null)
        => new() { Dto = dto, Name = name, ClassName = name };
}