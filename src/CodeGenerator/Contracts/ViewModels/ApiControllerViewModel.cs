using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Contracts.ViewModels;

using Library.CodeGeneration;

namespace HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;

public sealed class ApiControllerViewModel : InfraViewModelBase
{
    private string? _comment;
    private FunctionalityViewModel? _functionality;
    private ModuleViewModel _module = null!;
    private string? _nameSpace;

    public ApiControllerViewModel()
    {
    }

    public ApiControllerViewModel(long? id, string? name) : base(id, name)
    {
    }

    public string? Comment { get => this._comment; set => this.SetProperty(ref this._comment, value); }

    public string FullName => TypePath.Combine(this.NameSpace, this.Name);

    public FunctionalityViewModel? Functionality { get => this._functionality; set => this.SetProperty(ref this._functionality, value); }

    [NotNull]
    public ObservableCollection<ApiMethodViewModel> Methods { get; } = [];

    [NotNull]
    public ModuleViewModel Module
    {
        get
        {
            if (this._module == null)
            {
                this.Module = new();
            }

            return this._module!;
        }
        set => this.SetProperty(ref this._module, value);
    }

    public override string? Name
    {
        get => base.Name;
        set => this.SetProperty(ref this._name, value, orderPropertyNames: [nameof(this.FullName)]);
    }

    public string? NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }

    [NotNull]
    public ObservableCollection<PropertyViewModel> Properties { get; } = [];
}

public sealed class ApiMethodViewModel : InfraViewModelBase
{

}