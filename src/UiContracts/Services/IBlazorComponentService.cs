using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;

namespace Contracts.Services;

public interface IBlazorComponentService
    : IBusinessService
    , IAsyncCrudService<UiComponentViewModel>
{
    Task<IEnumerable<UiComponentViewModel>> GetByPageDataContextIdAsync(long dtoId, CancellationToken token = default);
}