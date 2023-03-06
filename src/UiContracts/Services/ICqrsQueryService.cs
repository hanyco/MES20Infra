using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;

namespace HanyCo.Infra.UI.Services;

public interface ICqrsQueryService : IBusinesService, IAsyncCrudService<CqrsQueryViewModel>, IAsyncCreator<CqrsQueryViewModel>
{
    Task<int> DeleteByIdAsync(long id);

    Task<CqrsQueryViewModel> FillByDbEntity(CqrsQueryViewModel @this, long dbQueryId, string? moduleName = null, string? paramDtoName = null, string? resultDtoName = null);

    CqrsQueryViewModel FillByDbEntity(CqrsQueryViewModel @this, CqrsSegregate dbQuery, string? moduleName = null, string? paramDtoName = null, string? resultDtoName = null);

    CqrsQueryViewModel FillByDbEntity(CqrsQueryViewModel @this,
        CqrsSegregate sergregate,
        Module infraModule,
        Internals.Data.DataSources.Dto parameterDto,
        IEnumerable<Property> parameterDtoProperties,
        Internals.Data.DataSources.Dto resultDto,
        IEnumerable<Property> resultDtoProperties);

    Task<CqrsQueryViewModel> FillViewModelAsync(CqrsQueryViewModel model,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null);

    Task<IReadOnlyList<CqrsQueryViewModel>> GetQueriesByDtoIdAsync(long dtoId);
}