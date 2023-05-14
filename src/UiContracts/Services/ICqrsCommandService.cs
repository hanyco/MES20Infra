using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;

namespace Contracts.Services;

public interface ICqrsCommandService : IBusinessService, IAsyncSaveService, IAsyncCrudService<CqrsCommandViewModel>, IAsyncCreator<CqrsCommandViewModel>
{
    Task<CqrsCommandViewModel> FillByDbEntity(CqrsCommandViewModel model,
        long id,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null,
        CancellationToken token = default);

    Task<CqrsCommandViewModel> FillViewModelAsync(CqrsCommandViewModel model,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null,
         CancellationToken token = default);
}