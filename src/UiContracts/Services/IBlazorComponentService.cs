using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;

namespace Contracts.Services;

public interface IBlazorComponentService
    : IBusinessService
    , IAsyncCrud<UiComponentViewModel>
{
    Task<IEnumerable<UiComponentViewModel>> GetByPageDataContextIdAsync(long dtoId, CancellationToken token = default);
}