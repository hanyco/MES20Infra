

using HanyCo.Infra.Internals.Data.DataSources;


using Library.Mapping;

namespace HanyCo.Infra.UI.Services.Imp;

internal abstract class CqrsSegregationServiceBase
{
    protected abstract CqrsSegregateType SegregateType { get; }
}