using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration;
using Library.Wpf.Markers;

namespace Contracts.ViewModels;

[ViewModel]
public sealed class DtoViewModel : InfraViewModelBase
{
    private string? _comment;
    private DbObjectViewModel _dbObject = null!;
    private FunctionalityViewModel? _functionality;
    private bool _isList;
    private bool _isParamsDto;
    private bool _isResultDto;
    private bool _isViewModel;
    private ModuleViewModel _module = null!;
    private string _nameSpace = null!;
    private IEnumerable<ClaimViewModel>? _securityClaims;

    public DtoViewModel()
    { }

    public DtoViewModel(long? id, string name)
        : base(id, name) { }

    public string? Comment { get => this._comment; set => this.SetProperty(ref this._comment, value); }

    public DbObjectViewModel DbObject { get => this._dbObject; set => this.SetProperty(ref this._dbObject, value); }

    public ObservableCollection<PropertyViewModel> DeletedProperties { get; } = [];

    public string FullName => TypePath.Combine(this.NameSpace, this.Name);

    public FunctionalityViewModel? Functionality { get => this._functionality; set => this.SetProperty(ref this._functionality, value); }

    public bool IsList { get => this._isList; set => this.SetProperty(ref this._isList, value); }

    public bool IsParamsDto { get => this._isParamsDto; set => this.SetProperty(ref this._isParamsDto, value); }

    public bool IsResultDto { get => this._isResultDto; set => this.SetProperty(ref this._isResultDto, value); }

    public bool IsViewModel { get => this._isViewModel; set => this.SetProperty(ref this._isViewModel, value); }

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
        set => this.SetProperty(ref this._name, value, orderPropertyNames: new[] { nameof(this.FullName) });
    }

    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }

    public ObservableCollection<PropertyViewModel> Properties { get; } = [];
}