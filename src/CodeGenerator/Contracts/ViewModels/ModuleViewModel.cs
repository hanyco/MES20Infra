using System.Collections.ObjectModel;

using Contracts.ViewModels;

namespace HanyCo.Infra.UI.ViewModels;

public sealed class ModuleViewModel : InfraViewModelBase, IHasSecurityDescriptor
{
    private ModuleViewModel _parent;

    public ModuleViewModel()
    {
    }

    public ModuleViewModel(long? id, string name)
        : base(id, name)
    {
    }

    public ModuleViewModel Parent
    {
        get => this._parent;
        set => this.SetProperty(ref this._parent, value);
    }

    public ObservableCollection<SecurityDescriptorViewModel> SecurityDescriptors { get; } = new();
}
