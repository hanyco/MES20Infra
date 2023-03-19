using System.Collections.ObjectModel;

using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Helpers;

namespace HanyCo.Infra.UI.ViewModels;

public sealed class PropertyViewModel : InfraViewModelBase, IHasSecurityDescriptor
{
    private string? _comment;
    private DbObjectViewModel? _dbObject;
    private DtoViewModel? _dto;
    private bool? _isList;
    private bool? _isNullable;
    private long _parentEntityId;
    private PropertyType _type;
    private string? _typeFullName;

    public string? Comment
    {
        get => this._comment;
        set => this.SetProperty(ref this._comment, value);
    }

    public DbObjectViewModel? DbObject
    {
        get => this._dbObject;
        set => this.SetProperty(ref this._dbObject, value);
    }

    public DtoViewModel? Dto
    {
        get => this._dto;
        set => this.SetProperty(ref this._dto, value);
    }

    public bool? IsList
    {
        get => this._isList;
        set => this.SetProperty(ref this._isList, value);
    }

    public bool? IsNullable
    {
        get => this._isNullable;
        set => this.SetProperty(ref this._isNullable, value);
    }

    public long ParentEntityId
    {
        get => this._parentEntityId;
        set => this.SetProperty(ref this._parentEntityId, value);
    }

    public ObservableCollection<SecurityDescriptorViewModel> SecurityDescriptors { get; } = new();

    public PropertyType Type
    {
        get
        {
            if (this._type is PropertyType.None && this.DbObject is not null)
            {
                var dbType = this.DbObject.As<DbColumnViewModel>()?.DbType;
                if (!dbType.IsNullOrEmpty())
                {
                    this._type = PropertyTypeHelper.FromDbType(dbType);
                }

                this.OnPropertyChanged(nameof(this.Type));
            }
            return this._type;
        }

        set => this.SetProperty(ref this._type, value);
    }

    public string TypeFullName
    {
        get => this.Type is not PropertyType.Dto || !this._typeFullName.IsNullOrEmpty()
            ? this.Type.ToFullTypeName(this._typeFullName)
            : this.Type.ToFullTypeName(this.Dto?.FullName);
        set => this.SetProperty(ref this._typeFullName, value);
    }
}