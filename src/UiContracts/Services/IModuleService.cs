﻿using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;

namespace Contracts.Services;

public interface IModuleService
    : IBusinessService
    , IAsyncReadService<ModuleViewModel>
    , IHierarchicalDbEntityService<Module>
{
}