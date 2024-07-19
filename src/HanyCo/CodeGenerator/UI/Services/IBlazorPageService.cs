using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Validations;

namespace HanyCo.Infra.UI.Services;

public interface IBlazorPageService
    : IService
    , IAsyncCrudService<UiPageViewModel>
    , IValidator<UiPageViewModel>
    , ICodeGeneratorService<UiPageViewModel>

{
    UiPageViewModel CreateViewModel(DtoViewModel dto, string? name = null)
        => new() { Dto = dto, Name = name, ClassName = name };
}