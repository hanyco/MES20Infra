using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Contracts.ViewModels;

namespace HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;

public sealed class AttributeViewModel : InfraViewModelBase
{
    public AttributeViewModel()
    {
    }

    public AttributeViewModel(long? id, string? name) : base(id, name)
    {
    }

    [NotNull]
    public ObservableCollection<(string Key, string? Value)> Properties { get; } = [];
}