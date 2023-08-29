using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;

namespace HanyCo.Infra.UI.Services;

public interface ICqrsQueryService : IBusinessService, IAsyncCrud<CqrsQueryViewModel>, IAsyncCreator<CqrsQueryViewModel>, IResetChanges
{
    Task<int> DeleteByIdAsync(long id, CancellationToken token = default);

    Task<CqrsQueryViewModel> FillByDbEntity(CqrsQueryViewModel @this, long dbQueryId, string? moduleName = null, string? paramDtoName = null, string? resultDtoName = null, CancellationToken token = default);

    CqrsQueryViewModel FillByDbEntity(CqrsQueryViewModel @this, CqrsSegregate dbQuery, string? moduleName = null, string? paramDtoName = null, string? resultDtoName = null);

    CqrsQueryViewModel FillByDbEntity(CqrsQueryViewModel @this,
        CqrsSegregate segregate,
        Module infraModule,
        Internals.Data.DataSources.Dto parameterDto,
        IEnumerable<Property> parameterDtoProperties,
        Internals.Data.DataSources.Dto resultDto,
        IEnumerable<Property> resultDtoProperties);

    Task<CqrsQueryViewModel> FillViewModelAsync(CqrsQueryViewModel model,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null, CancellationToken token = default);

    Task<IReadOnlyList<CqrsQueryViewModel>> GetQueriesByDtoIdAsync(long dtoId, CancellationToken token = default);
}