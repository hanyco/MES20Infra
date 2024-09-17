﻿using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.CodeGen.Contracts.ViewModels;

using Library.Interfaces;
using Library.Validations;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface IBlazorPageService : IBusinessService
    , IAsyncCrud<UiPageViewModel>
    , IValidator<UiPageViewModel>
    , IAsyncSaveChanges
    , IResetChanges
    //, IAsyncValidator<UiPageViewModel>
    , ILoggerContainer
{
    [return: NotNullIfNotNull(nameof(dto))]
    UiPageViewModel? CreateViewModel(DtoViewModel dto, string? name = null, ModuleViewModel? module = null, string? nameSpace = null, Guid? guid = null, string? route = null, DbObjectViewModel? propertyDbObject = null);
}