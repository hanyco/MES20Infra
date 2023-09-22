using Contracts.ViewModels;

using Library.Interfaces;

namespace Contracts.Services;

public interface IBlazorComponentService
    : IBusinessService
    , IAsyncCrud<UiViewModel>
{
    Task<IEnumerable<UiViewModel>> GetByPageDataContextIdAsync(long dtoId, CancellationToken token = default);

    UiPropertyViewModel CreateBoundPropertyByDto(DtoViewModel viewModel);

    Task<UiViewModel> CreateNewComponentAsync(CancellationToken token = default);

    Task<UiViewModel> CreateNewComponentByDtoAsync(DtoViewModel dto, CancellationToken token = default);

    UiViewModel CreateViewModel(DtoViewModel dto);

    UiComponentCustomButtonViewModel CreateUnboundAction();

    UiPropertyViewModel CreateUnboundProperty();

    Task<UiPropertyViewModel?> FillUiComponentPropertyViewModelAsync(UiPropertyViewModel? prop, CancellationToken token = default);
}