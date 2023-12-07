using Contracts.ViewModels;

using Library.Interfaces;

namespace Contracts.Services;

public interface IBlazorComponentService
    : IBusinessService
    , IAsyncCrud<UiComponentViewModel>
{
    Task<IEnumerable<UiComponentViewModel>> GetByPageDataContextIdAsync(long dtoId, CancellationToken token = default);

    UiPropertyViewModel CreateBoundPropertyByDto(DtoViewModel viewModel);

    Task<UiComponentViewModel> CreateNewComponentAsync(CancellationToken token = default);

    Task<UiComponentViewModel> CreateNewComponentByDtoAsync(DtoViewModel dto, CancellationToken token = default);

    UiComponentViewModel CreateViewModel(DtoViewModel dto);

    UiComponentCustomButton CreateUnboundAction();

    UiPropertyViewModel CreateUnboundProperty();

    Task<UiPropertyViewModel?> FillUiComponentPropertyViewModelAsync(UiPropertyViewModel? prop, CancellationToken token = default);
}