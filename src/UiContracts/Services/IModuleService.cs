using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;
using Library.Interfaces;

namespace HanyCo.Infra.UI.Services
{
    public interface IModuleService
        : IBusinesService
        , IAsyncReadService<ModuleViewModel>
        , IHierarchicalDbEntityService<Module>
    {
    }
}
