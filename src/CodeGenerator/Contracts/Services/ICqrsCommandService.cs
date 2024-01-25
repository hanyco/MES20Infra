using HanyCo.Infra.CodeGen.Contracts.ViewModels;

using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface ICqrsCommandService : IBusinessService, IAsyncSaveChanges, IAsyncCrud<CqrsCommandViewModel>, IAsyncCreator<CqrsCommandViewModel>, IResetChanges
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