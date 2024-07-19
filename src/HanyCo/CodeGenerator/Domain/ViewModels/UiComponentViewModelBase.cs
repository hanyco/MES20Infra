#nullable disable


using System.Collections.ObjectModel;

using Library.CodeGeneration;

namespace HanyCo.Infra.CodeGen.Contracts.ViewModels;

public abstract class UiComponentViewModelBase : InfraViewModelBase
{
    public ObservableCollection<(TypePath Type, string Name)> Parameters { get; } = [];
}