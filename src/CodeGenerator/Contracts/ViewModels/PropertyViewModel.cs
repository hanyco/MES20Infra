using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.Coding;
using Library.Validations;

namespace Contracts.ViewModels;

public sealed class PropertyViewModel : InfraViewModelBase
{
    private string? _comment;
    private DbObjectViewModel? _dbObject;
    private DtoViewModel? _dto;
    private bool? _isList;
    private bool? _isNullable;
    private long _parentEntityId;
    private PropertyType _type;
    private string? _typeFullName;

    public PropertyViewModel()
    {
    }

    public PropertyViewModel(PropertyViewModel original)
        : base(original.ArgumentNotNull().Id, original.Name)
    {
        this._type = original._type;
        this._comment = original._comment;
        this._dbObject = original._dbObject;
        this._dto = original._dto;
        this._isList = original._isList;
        this._isNullable = original._isNullable;
        this._parentEntityId = original._parentEntityId;
        this._typeFullName = original._typeFullName;
    }

    public PropertyViewModel(string name, PropertyType type)
        => (this.Name, this.Type) = (name, type);

    public string? Comment
    {
        get => this._comment;
        set => this.SetProperty(ref this._comment, value);
    }

    public DbObjectViewModel? DbObject
    {
        get => this._dbObject ?? (DbObject = new(this.Name, type: PropertyTypeHelper.ToDbTypeName(this.Type)));
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

    public PropertyType Type
    {
        get
        {
            if (this._type is PropertyType.None && this.DbObject is DbColumnViewModel dbColumn)
            {
                var dbType = dbColumn.DbType;
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