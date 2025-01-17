using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface ICqrsCommandService : IBusinessService, IAsyncCrud<CqrsCommandViewModel>, IAsyncCreator<CqrsCommandViewModel>, IResetChanges
{
    Task<bool> AnyByName(string name);

    Task<Result> DeleteById(long commandId, bool persist = true, CancellationToken token = default);

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