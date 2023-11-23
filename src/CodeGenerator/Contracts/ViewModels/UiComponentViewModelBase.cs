#nullable disable


using System.Collections.ObjectModel;

using Contracts.ViewModels;

using Library.CodeGeneration;

namespace HanyCo.Infra.UI.ViewModels;

public abstract class UiComponentViewModelBase : InfraViewModelBase
{
    public ObservableCollection<(TypePath Type, string Name)> Parameters { get; } = [];
}