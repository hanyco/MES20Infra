using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;
using Library.Mapping;

namespace HanyCo.Infra.UI.Services.Imp;

internal abstract class CqrsSegregationServiceBase
{
    private readonly IEntityViewModelConverter _converter;

    protected CqrsSegregationServiceBase(IEntityViewModelConverter converter) => 
        this._converter = converter;

    protected abstract CqrsSegregateType SegregateType { get; }
}