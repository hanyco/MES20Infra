using HanyCo.Infra.Internals.Data.DataSources;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public sealed class CqrsCommandViewModel : CqrsViewModelBase
{
    protected override CqrsSegregateType SegregateType { get; } = CqrsSegregateType.Command;
}
