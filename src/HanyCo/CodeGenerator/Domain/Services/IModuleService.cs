using HanyCo.Infra.CodeGen.Contracts.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface IModuleService
    : IBusinessService
    , IAsyncRead<ModuleViewModel>
    , IHierarchicalDbEntityActor<Module>
{
}