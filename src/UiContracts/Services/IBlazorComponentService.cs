using HanyCo.Infra.UI.ViewModels;
using Library.Interfaces;
using Library.Validations;

namespace HanyCo.Infra.UI.Services
{
    public interface IBlazorComponentService
        : IBusinesService
        , IAsyncCrudService<UiComponentViewModel>
    {
        Task<IEnumerable<UiComponentViewModel>> GetByPageDataContextIdAsync(long dtoId);
    }
}
