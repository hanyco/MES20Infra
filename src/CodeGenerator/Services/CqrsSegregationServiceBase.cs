using HanyCo.Infra.Internals.Data.DataSources;

namespace Services;

internal abstract class CqrsSegregationServiceBase
{
    protected abstract CqrsSegregateType SegregateType { get; }
}