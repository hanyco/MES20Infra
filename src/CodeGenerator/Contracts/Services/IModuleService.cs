using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

using Library.Interfaces;

namespace Contracts.Services;

public interface IModuleService
    : IBusinessService
    , IAsyncRead<ModuleViewModel>
    , IHierarchicalDbEntityActor<Module>
{
}