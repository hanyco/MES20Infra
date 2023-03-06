using Contracts.Services;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.Mapping;

namespace HanyCo.Infra.UI.Services.Imp;

internal abstract class CqrsSegregationServiceBase
{
    protected abstract CqrsSegregateType SegregateType { get; }
}