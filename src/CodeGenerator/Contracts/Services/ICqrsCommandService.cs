using HanyCo.Infra.CodeGen.Contracts.ViewModels;

using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface ICqrsCommandService : IBusinessService, IAsyncSaveChanges, IAsyncCrud<CqrsCommandViewModel>, IAsyncCreator<CqrsCommandViewModel>, IResetChanges
{
    Task<Result> DeleteByIdAsync(long commandId, CancellationToken token);

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