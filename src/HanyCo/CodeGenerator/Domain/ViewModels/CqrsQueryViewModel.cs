using HanyCo.Infra.Internals.Data.DataSources;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public sealed class CqrsQueryViewModel : CqrsViewModelBase
{
    protected override CqrsSegregateType SegregateType { get; } = CqrsSegregateType.Query;
}
