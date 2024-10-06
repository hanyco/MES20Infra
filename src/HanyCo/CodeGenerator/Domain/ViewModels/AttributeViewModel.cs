using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

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