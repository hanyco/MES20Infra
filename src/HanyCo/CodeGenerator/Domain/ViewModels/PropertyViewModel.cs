using HanyCo.Infra.Internals.Data.DataSources;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public sealed class PropertyViewModel : InfraViewModelBase
{
    private PropertyType _type;

    public PropertyViewModel()
    {
    }

    public PropertyViewModel(string? name, PropertyType type, long? id = null)
        : base(id, name) => this.Type = type;

    public PropertyViewModel(DbColumnViewModel dbColumnViewModel)
        : this(dbColumnViewModel.Name, PropertyTypeHelper.FromDbType(dbColumnViewModel.Type))
    {
        this.TypeFullName = dbColumnViewModel.Type;
        this.DbObject = dbColumnViewModel;
        this.Id = dbColumnViewModel.ObjectId;
        this.IsNullable = dbColumnViewModel.IsNullable;
    }

    public string? Comment
    {
        get;
        set => this.SetProperty(ref field, value);
    }

    public DbColumnViewModel? DbObject
    {
        get => field ??= (field = new(this.Name, this.Id ?? default, this._type.ToDbTypeName(), this.IsNullable ?? default));
        set => this.SetProperty(ref field, value);
    }

    public DtoViewModel? Dto
    {
        get;
        set => this.SetProperty(ref field, value);
    }

    public bool? IsList
    {
        get;
        set => this.SetProperty(ref field, value);
    }

    public bool? IsNullable
    {
        get;
        set => this.SetProperty(ref field, value);
    }

    public long ParentEntityId
    {
        get;
        set => this.SetProperty(ref field, value);
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
            if (this.Type is not PropertyType.Dto || !field.IsNullOrEmpty())
            {
                result = this.Type.ToFullTypeName(field);
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

        set => this.SetProperty(ref field, value);
    }
}

public static class PropertyViewModelHelper
{
    public static IEnumerable<PropertyViewModel> ExcludeId(this IEnumerable<PropertyViewModel> properties)
        => properties.Where(x => !x.Name.EqualsTo("Id"));

    public static PropertyViewModel? FindId(this IEnumerable<PropertyViewModel> properties)
        => properties.FirstOrDefault(p => (p.DbObject?.IsIdentity is true) || p.Name.EqualsTo("Id"));
}