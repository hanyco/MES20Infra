using HanyCo.Infra.Internals.Data.DataSources;

namespace HanyCo.Infra.CodeGen.Contracts.ViewModels;

public sealed class CqrsCommandViewModel : CqrsViewModelBase
{
    protected override CqrsSegregateType SegregateType { get; } = CqrsSegregateType.Command;
}
