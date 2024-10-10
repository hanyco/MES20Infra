using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.Interfaces;
using Library.Validations;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface IBlazorPageService : IBusinessService
    , IAsyncCrud<UiPageViewModel>
    , IValidator<UiPageViewModel>
{
    [return: NotNullIfNotNull(nameof(dto))]
    UiPageViewModel? CreateViewModel(DtoViewModel dto, string? name = null, ModuleViewModel? module = null, string? nameSpace = null, Guid? guid = null, string? route = null, DbObjectViewModel? propertyDbObject = null);
}