#nullable disable

using HanyCo.Infra.Internals.Data.DataSources;

using System.Collections.ObjectModel;

namespace HanyCo.Infra.UI.ViewModels;

public abstract class CqrsViewModelBase : InfraViewModelBase, IHasSecurityDescriptor
{
    private ModuleViewModel _module;
    public ModuleViewModel Module
    {
        get => this._module;
        set => this.SetProperty(ref this._module, value);
    }

    private DtoViewModel _resultDto;
    public DtoViewModel ResultDto
    {
        get => this._resultDto;
        set => this.SetProperty(ref this._resultDto, value);
    }

    private string _friendlyName;
    public string FriendlyName
    {
        get => this._friendlyName;
        set => this.SetProperty(ref this._friendlyName, value);
    }

    private bool _hasPartialOnInitialize;
    public bool HasPartialOnInitialize
    {
        get => this._hasPartialOnInitialize;
        set => this.SetProperty(ref this._hasPartialOnInitialize, value);
    }

    private bool _hasPartialHandller;
    public bool HasPartialHandller
    {
        get => this._hasPartialHandller;
        set => this.SetProperty(ref this._hasPartialHandller, value);
    }

    private DbObjectViewModel _dbObject;
    public DbObjectViewModel DbObject
    {
        get => this._dbObject;
        set => this.SetProperty(ref this._dbObject, value);
    }

    private string _comment;
    public string Comment
    {
        get => this._comment;
        set => this.SetProperty(ref this._comment, value);
    }

    private DtoViewModel _paramDto;
    public DtoViewModel ParamDto
    {
        get => this._paramDto;
        set => this.SetProperty(ref this._paramDto, value);
    }

    private CqrsSegregateCategory _category;

    public CqrsSegregateCategory Category
    {
        get => this._category;
        set => this.SetProperty(ref this._category, value);
    }

    private string _cqrsNameSpace;
    public string CqrsNameSpace
    {
        get => this._cqrsNameSpace;
        set => this.SetProperty(ref this._cqrsNameSpace, value);
    }

    private string _dtoNameSpace;
    public string DtoNameSpace
    {
        get => this._dtoNameSpace;
        set => this.SetProperty(ref this._dtoNameSpace, value);
    }

    protected abstract CqrsSegregateType SegregateType { get; }

    public ObservableCollection<SecurityDescriptorViewModel> SecurityDescriptors { get; } = new();

    public override string ToString() => base.ToString();
}
