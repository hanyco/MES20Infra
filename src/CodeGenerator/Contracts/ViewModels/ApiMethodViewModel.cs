using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Contracts.ViewModels;

namespace HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;

public sealed class ApiMethodViewModel : InfraViewModelBase
{
    public ApiMethodViewModel()
    {
    }

    public ApiMethodViewModel(long? id, string? name) : base(id, name)
    {
    }

    [NotNull]
    public ObservableCollection<AttributeViewModel> Attributes { get; } = [];
}
