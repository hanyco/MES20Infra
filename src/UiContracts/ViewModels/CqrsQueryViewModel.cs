using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

namespace HanyCo.Infra.UI.ViewModels;

public sealed class CqrsQueryViewModel : CqrsViewModelBase
{
    protected override CqrsSegregateType SegregateType { get; } = CqrsSegregateType.Query;
}
