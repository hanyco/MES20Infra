using HanyCo.Infra.CodeGen.Contracts.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface ICqrsQueryService : IBusinessService, IAsyncCrud<CqrsQueryViewModel>, IAsyncCreator<CqrsQueryViewModel>, IResetChanges
{
    Task<int> DeleteByIdAsync(long id, CancellationToken token = default);

    Task<CqrsQueryViewModel> FillByDbEntity(CqrsQueryViewModel @this, long dbQueryId, string? moduleName = null, string? paramDtoName = null, string? resultDtoName = null, CancellationToken token = default);

    CqrsQueryViewModel FillByDbEntity(CqrsQueryViewModel @this, CqrsSegregate dbQuery, string? moduleName = null, string? paramDtoName = null, string? resultDtoName = null);

    CqrsQueryViewModel FillByDbEntity(CqrsQueryViewModel @this,
        CqrsSegregate segregate,
        Module infraModule,
        Dto parameterDto,
        IEnumerable<Property> parameterDtoProperties,
        Dto resultDto,
        IEnumerable<Property> resultDtoProperties);

    Task<CqrsQueryViewModel> FillViewModelAsync(CqrsQueryViewModel model,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null, CancellationToken token = default);

    Task<IReadOnlyList<CqrsQueryViewModel>> GetQueriesByDtoIdAsync(long dtoId, CancellationToken token = default);
}