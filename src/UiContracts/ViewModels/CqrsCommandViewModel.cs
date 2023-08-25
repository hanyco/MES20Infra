using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

namespace HanyCo.Infra.UI.ViewModels;

public sealed class CqrsCommandViewModel : CqrsViewModelBase
{
    protected override CqrsSegregateType SegregateType { get; } = CqrsSegregateType.Command;
}
