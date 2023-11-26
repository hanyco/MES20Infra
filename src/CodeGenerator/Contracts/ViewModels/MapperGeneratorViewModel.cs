using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using Contracts.Services;

using Library.CodeGeneration;
using Library.Wpf.Markers;

namespace Contracts.ViewModels;

[ViewModel]
public sealed class MapperGeneratorViewModel : InfraViewModelBase
{
    public MapperGeneratorViewModel()
    {
    }

    public MapperGeneratorViewModel(long? id, string? name) : base(id, name)
    {
    }

    public Collection<MapperSourceGeneratorArguments> Arguments { get; } = [];
}