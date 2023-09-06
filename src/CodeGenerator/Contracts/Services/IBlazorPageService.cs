using Contracts.ViewModels;

using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Validations;

namespace Contracts.Services;

public interface IBlazorPageService : IBusinessService
    , IAsyncCrud<UiPageViewModel>
    , IValidator<UiPageViewModel>
    , IAsyncSaveChanges
    , IResetChanges
    , IAsyncValidator<UiPageViewModel>
    , ILoggerContainer

{
    UiPageViewModel CreateViewModel(DtoViewModel dto, string? name = null) =>
        new()
        {
            Dto = dto,
            Name = name ?? dto.Name.Puralize()?.AddEnd("Page"),
            ClassName = name ?? dto.Name.Puralize()?.AddEnd("Page")
        };
}

file static class Puralizer
{
    internal static string? Puralize(this string? name) =>
        name?.TrimEnd("Dto")
             .TrimEnd("Params")
             .TrimEnd("Result")
             .TrimEnd("ViewModel");
}