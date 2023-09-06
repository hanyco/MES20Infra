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
    //, IAsyncValidator<UiPageViewModel>
    , ILoggerContainer
{
    UiPageViewModel CreateViewModel(DtoViewModel dto, string? name = null, ModuleViewModel? module = null, string? nameSpace = null, Guid? guid = null, string? route = null, DbObjectViewModel? propertyDbObject = null);
}