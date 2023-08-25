#nullable disable

using System.Collections.ObjectModel;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

namespace Contracts.ViewModels;

public abstract class CqrsViewModelBase : InfraViewModelBase, IHasSecurityDescriptor
{
    private CqrsSegregateCategory _category;
    private string _comment;
    private string _cqrsNameSpace;
    private DbObjectViewModel _dbObject;
    private string _dtoNameSpace;
    private string _friendlyName;
    private bool _hasPartialHandler;
    private bool _hasPartialOnInitialize;
    private ModuleViewModel _module;
    private DtoViewModel _paramDto;
    private DtoViewModel _resultDto;

    public CqrsSegregateCategory Category
    {
        get => this._category;
        set => this.SetProperty(ref this._category, value);
    }

    public string Comment
    {
        get => this._comment;
        set => this.SetProperty(ref this._comment, value);
    }

    public string CqrsNameSpace
    {
        get => this._cqrsNameSpace;
        set => this.SetProperty(ref this._cqrsNameSpace, value);
    }

    public DbObjectViewModel DbObject
    {
        get => this._dbObject;
        set => this.SetProperty(ref this._dbObject, value);
    }

    public string DtoNameSpace
    {
        get => this._dtoNameSpace;
        set => this.SetProperty(ref this._dtoNameSpace, value);
    }

    public string FriendlyName
    {
        get => this._friendlyName;
        set => this.SetProperty(ref this._friendlyName, value);
    }

    public bool HasPartialHandler
    {
        get => this._hasPartialHandler;
        set => this.SetProperty(ref this._hasPartialHandler, value);
    }

    public bool HasPartialOnInitialize
    {
        get => this._hasPartialOnInitialize;
        set => this.SetProperty(ref this._hasPartialOnInitialize, value);
    }

    public ModuleViewModel Module
    {
        get => this._module;
        set => this.SetProperty(ref this._module, value);
    }

    public DtoViewModel ParamDto
    {
        get => this._paramDto;
        set => this.SetProperty(ref this._paramDto, value);
    }

    public DtoViewModel ResultDto
    {
        get => this._resultDto;
        set => this.SetProperty(ref this._resultDto, value);
    }

    public ObservableCollection<SecurityDescriptorViewModel> SecurityDescriptors { get; } = new();
    protected abstract CqrsSegregateType SegregateType { get; }

    public override string ToString() => base.ToString();
}