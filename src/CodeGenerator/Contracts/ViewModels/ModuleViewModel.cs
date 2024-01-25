namespace HanyCo.Infra.CodeGen.Contracts.ViewModels;

public sealed class ModuleViewModel : InfraViewModelBase
{
    private ModuleViewModel? _parent;

    public ModuleViewModel()
    {
    }

    public ModuleViewModel(long? id, string name)
        : base(id, name)
    {
    }

    public ModuleViewModel? Parent
    {
        get => this._parent;
        set => this.SetProperty(ref this._parent, value);
    }
}