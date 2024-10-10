using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public class DbObjectViewModel : InfraViewModelBase
{
    private long _objectId;
    private string? _schema;
    private string? _type;

    public DbObjectViewModel(string name, long objectId = -1, string? schema = null, string? type = null)
        : base(null, name)
    {
        this.Name = name;
        this.ObjectId = objectId;
        this.Schema = schema;
        this.Type = type;
    }

    public long ObjectId
    {
        get => this._objectId;
        set => this.SetProperty(ref this._objectId, value);
    }
    public string? Schema
    {
        get => this._schema;
        set => this.SetProperty(ref this._schema, value);
    }
    public string? Type
    {
        get => this._type;
        set => this.SetProperty(ref this._type, value);
    }

    
    public override string ToString() =>
        this.Schema is not null ? $"[{this.Schema}].[{this.Name ?? "No Name!"}]" : $"[{this.Name ?? "No Name!"}]";

    /// <summary>
    /// Format: schema.name.type.dbObjectId
    /// </summary>
    /// <returns></returns>
    [return: MaybeNull]
    public string ToDbFormat() =>
        $"{Schema}.{Name}.{Type}.{ObjectId}";

    [return: NotNullIfNotNull(nameof(dbFormat))]
    public static DbObjectViewModel? FromDbFormat(string? dbFormat)
    {
        if (dbFormat.IsNullOrEmpty())
        {
            return null;
        }
        var parts = dbFormat.Split('.');
        var result = new DbObjectViewModel(parts[1], parts[3].Cast().ToLong(), parts[0], parts[2]);
        return result;
    }
}
