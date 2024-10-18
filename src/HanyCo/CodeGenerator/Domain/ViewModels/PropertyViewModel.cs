using HanyCo.Infra.Internals.Data.DataSources;

using Library.Validations;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

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
    public PropertyViewModel(PropertyViewModel original) : base(original.ArgumentNotNull().Id, original.Name)
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

    public PropertyViewModel(string? name, PropertyType type) : this(name) => 
        this.Type = type;

    public PropertyViewModel(string? name) => 
        this.Name = name;

    public PropertyViewModel(DbColumnViewModel dbColumnViewModel) : this((DbObjectViewModel)dbColumnViewModel) => 
        this.IsNullable = dbColumnViewModel.IsNullable;

    public PropertyViewModel(DbObjectViewModel dbColumnViewModel) : this(dbColumnViewModel.Name, PropertyTypeHelper.FromDbType(dbColumnViewModel.Type))
    {
        this.TypeFullName = dbColumnViewModel.Type;
        this.DbObject = dbColumnViewModel;
        this.Id = dbColumnViewModel.ObjectId;
    }

    public string? Comment
    {
        get => this._comment;
        set => this.SetProperty(ref this._comment, value);
    }

    public DbObjectViewModel DbObject
    {
        get => this._dbObject ??= (this._dbObject = new(this.Name, type: this._type.ToDbTypeName()));
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
                var dbType = dbColumn.Type;
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
        get
        {
            string result;
            if (this.Type is not PropertyType.Dto || !this._typeFullName.IsNullOrEmpty())
            {
                result = this.Type.ToFullTypeName(this._typeFullName);
            }
            else
            {
                result = this.Type.ToFullTypeName(this.Dto?.FullName);
            }
            if (this.IsNullable ?? false)
            {
                result += "?";
            }

            return result;
        }

        set => this.SetProperty(ref this._typeFullName, value);
    }
}

public static class PropertyViewModelHelper
{
    public static IEnumerable<PropertyViewModel> ExcludeId(this IEnumerable<PropertyViewModel> properties)
        => properties.Where(x => !x.Name.EqualsTo("Id"));

    public static PropertyViewModel? FindId(this IEnumerable<PropertyViewModel> properties)
            => properties.FirstOrDefault(p => p.Name.EqualsTo("Id"));
}